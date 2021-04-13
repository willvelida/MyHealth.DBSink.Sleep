using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyHealth.Common;
using MyHealth.DBSink.Sleep.Helpers;
using MyHealth.DBSink.Sleep.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Functions
{
    public class CreateSleepDocument
    {
        private readonly ILogger<CreateSleepDocument> _logger;
        private readonly IServiceBusHelpers _serviceBusHelpers;
        private readonly FunctionOptions _functionOptions;
        private readonly ISleepDbService _sleepDbService;

        public CreateSleepDocument(
            ILogger<CreateSleepDocument> logger,
            IServiceBusHelpers serviceBusHelpers,
            IOptions<FunctionOptions> options,
            ISleepDbService sleepDbService)
        {
            _logger = logger;
            _serviceBusHelpers = serviceBusHelpers;
            _functionOptions = options.Value;
            _sleepDbService = sleepDbService;
        }

        [FunctionName(nameof(CreateSleepDocument))]
        public async Task Run([ServiceBusTrigger("mytopic", "mysubscription", Connection = "FunctionOptions:ServiceBusConnectionString")] string mySbMsg)
        {
            try
            {
                var sleep = JsonConvert.DeserializeObject<mdl.Sleep>(mySbMsg);

                await _sleepDbService.AddSleepDocument(sleep);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepDocument)}. Exception: {ex.Message}");
                await _serviceBusHelpers.SendMessageToTopic(_functionOptions.ExceptionTopicSetting, ex);
            }
        }
    }
}
