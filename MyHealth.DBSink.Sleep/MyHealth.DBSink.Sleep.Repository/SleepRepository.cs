using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyHealth.Common.Models;
using MyHealth.DBSink.Sleep.Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Sleep.Repository
{
    public class SleepRepository : ISleepRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _configuration;
        private readonly Container _myHealthContainer;

        public SleepRepository(
            CosmosClient cosmosClient,
            IConfiguration configuration)
        {
            _cosmosClient = cosmosClient;
            _configuration = configuration;
            _myHealthContainer = _cosmosClient.GetContainer(_configuration["DatabaseName"], _configuration["ContainerName"]);
        }

        public async Task CreateSleep(SleepEnvelope sleepEnvelope)
        {
            try
            {
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
