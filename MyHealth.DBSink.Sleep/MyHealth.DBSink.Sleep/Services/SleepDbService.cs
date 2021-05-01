using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyHealth.Common.Models;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Services
{
    public class SleepDbService : ISleepDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _configuration;
        private readonly Container _myHealthContainer;

        public SleepDbService(
            CosmosClient cosmosClient,
            IConfiguration configuration)
        {
            _cosmosClient = cosmosClient;
            _configuration = configuration;
            _myHealthContainer = _cosmosClient.GetContainer(_configuration["DatabaseName"], _configuration["ContainerName"]);
        }

        public async Task AddSleepDocument(mdl.Sleep sleep)
        {
            try
            {
                var sleepEnvelope = new SleepEnvelope
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
                    sleepEnvelope,
                    new PartitionKey(sleepEnvelope.DocumentType),
                    itemRequestOptions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
