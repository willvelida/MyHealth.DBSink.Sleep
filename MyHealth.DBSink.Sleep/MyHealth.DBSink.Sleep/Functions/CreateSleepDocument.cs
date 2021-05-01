using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.DBSink.Sleep.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Functions
{
    public class CreateSleepDocument
    {
        private readonly IConfiguration _configuration;
        private readonly ISleepDbService _sleepDbService;
        private readonly IServiceBusHelpers _serviceBusHelpers;

        public CreateSleepDocument(
            IConfiguration configuration,
            ISleepDbService sleepDbService,
            IServiceBusHelpers serviceBusHelpers)
        {
            _configuration = configuration;
            _sleepDbService = sleepDbService;
            _serviceBusHelpers = serviceBusHelpers;
        }

        [FunctionName(nameof(CreateSleepDocument))]
        public async Task Run([ServiceBusTrigger("myhealthsleeptopic", "myhealthsleepsubscription", Connection = "ServiceBusConnectionString")] string mySbMsg, ILogger logger)
        {
            try
            {
                // Convert incoming message into Sleep Model
                var sleep = JsonConvert.DeserializeObject<mdl.Sleep>(mySbMsg);

                // Persist Sleep object to Cosmos DB
                await _sleepDbService.AddSleepDocument(sleep);
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
