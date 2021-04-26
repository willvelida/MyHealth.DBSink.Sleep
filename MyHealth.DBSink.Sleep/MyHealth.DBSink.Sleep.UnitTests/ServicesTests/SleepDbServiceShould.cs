using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Moq;
using MyHealth.DBSink.Sleep.Models;
using MyHealth.DBSink.Sleep.Services;
using MyHealth.DBSink.Sleep.UnitTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyHealth.DBSink.Sleep.UnitTests.ServicesTests
{
    public class SleepDbServiceShould
    {
        private Mock<CosmosClient> _mockCosmosClient;
        private Mock<Container> _mockContainer;
        private Mock<IConfiguration> _mockConfiguration;

        private SleepDbService _sut;

        public SleepDbServiceShould()
        {
            _mockCosmosClient = new Mock<CosmosClient>();
            _mockContainer = new Mock<Container>();
            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockContainer.Object);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["DatabaseName"]).Returns("db");
            _mockConfiguration.Setup(x => x["ContainerName"]).Returns("col");

            _sut = new SleepDbService(_mockCosmosClient.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task AddSleepDocumentWhenCreateItemAsyncIsCalled()
        {
            // Arrange
            Common.Models.Sleep testSleepDocument = new Common.Models.Sleep
            {

            };

            _mockContainer.SetupCreateItemAsync<Common.Models.Sleep>();

            // Act
            await _sut.AddSleepDocument(testSleepDocument);

            // Assert
            _mockContainer.Verify(x => x.CreateItemAsync(
                It.IsAny<SleepDocument>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
