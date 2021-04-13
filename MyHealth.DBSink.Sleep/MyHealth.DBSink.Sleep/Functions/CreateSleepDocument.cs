using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.DBSink.Sleep.Services;
using Newtonsoft.Json;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Functions
{
    public class CreateSleepDocument
    {
        private readonly ILogger<CreateSleepDocument> _logger;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly IConfiguration _configuration;
        private readonly ISleepDbService _sleepDbService;

        public CreateSleepDocument(
            ILogger<CreateSleepDocument> logger,
            IServiceBusHelpers serviceBusHelpers,
            IConfiguration configuration,
            ISleepDbService sleepDbService)
        {
            _logger = logger;
            _serviceBusHelpers = serviceBusHelpers;
            _configuration = configuration;
            _sleepDbService = sleepDbService;
        }

        [FunctionName(nameof(CreateSleepDocument))]
        public async Task Run([ServiceBusTrigger("mytopic", "mysubscription", Connection = "")]string mySbMsg)
        {
            try
            {
                var sleep = JsonConvert.DeserializeObject<mdl.Sleep>(mySbMsg);

                await _sleepDbService.AddSleepDocument(sleep);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepDocument)}. Exception: {ex.Message}");
                await _serviceBusHelpers.SendMessageToTopic(_configuration["ExceptionTopic"], ex);
            }
        }
    }
}
