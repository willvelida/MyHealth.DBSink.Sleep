using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public CreateSleepDocument(
            IConfiguration configuration,
            ISleepDbService sleepDbService)
        {
            _configuration = configuration;
            _sleepDbService = sleepDbService;
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
                throw ex;
            }
        }
    }
}
