using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using MyHealth.DBSink.Sleep.Helpers;
using MyHealth.DBSink.Sleep.Models;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Services
{
    public class SleepDbService : ISleepDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly FunctionOptions _functionOptions;
        private readonly Container _myHealthContainer;

        public SleepDbService(
            CosmosClient cosmosClient,
            IOptions<FunctionOptions> options)
        {
            _cosmosClient = cosmosClient;
            _functionOptions = options.Value;
            _myHealthContainer = _cosmosClient.GetContainer(_functionOptions.DatabaseNameSetting, _functionOptions.ContainerNameSetting);
        }

        public async Task AddSleepDocument(mdl.Sleep sleep)
        {
            var sleepDocument = new SleepDocument
            {
                Id = Guid.NewGuid().ToString(),
                Sleep = sleep,
                DocumentType = "Sleep"
            };

            ItemRequestOptions itemRequestOptions = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            await _myHealthContainer.CreateItemAsync(
                sleepDocument,
                new PartitionKey(sleepDocument.DocumentType),
                itemRequestOptions);
        }
    }
}
