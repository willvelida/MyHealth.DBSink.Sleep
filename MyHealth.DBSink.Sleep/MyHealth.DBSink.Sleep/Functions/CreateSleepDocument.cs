using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.DBSink.Sleep.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Functions
{
    public class CreateSleepDocument
    {
        private readonly IConfiguration _configuration;
        private readonly ISleepService _sleepService;
        private readonly IServiceBusHelpers _serviceBusHelpers;

        public CreateSleepDocument(
            IConfiguration configuration,
            ISleepService sleepService,
            IServiceBusHelpers serviceBusHelpers)
        {
            _configuration = configuration;
            _sleepService = sleepService;
            _serviceBusHelpers = serviceBusHelpers;
        }

        [FunctionName(nameof(CreateSleepDocument))]
        public async Task Run([ServiceBusTrigger("myhealthsleeptopic", "myhealthsleepsubscription", Connection = "ServiceBusConnectionString")] string mySbMsg, ILogger logger)
        {
            try
            {
                // Convert incoming message into Sleep Model
                var sleep = JsonConvert.DeserializeObject<mdl.Sleep>(mySbMsg);

                var sleepEnvelope = _sleepService.MapSleepToSleepEnvelope(sleep);

                // Persist Sleep object to Cosmos DB
                await _sleepService.AddSleepDocument(sleepEnvelope);
                logger.LogInformation($"Sleep Document with {sleep.StartTime} has been persisted");
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception thrown in {nameof(CreateSleepDocument)}. Exception: {ex.Message}");
                await _serviceBusHelpers.SendMessageToQueue(_configuration["ExceptionQueue"], ex);
                throw ex;
            }
        }
    }
}
