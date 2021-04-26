using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyHealth.DBSink.Sleep.Functions;
using MyHealth.DBSink.Sleep.Models;
using MyHealth.DBSink.Sleep.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MyHealth.DBSink.Sleep.UnitTests.FunctionTests
{
    public class CreateSleepDocumentShould
    {
        private Mock<ILogger> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ISleepDbService> _mockSleepDbService;

        private CreateSleepDocument _func;

        public CreateSleepDocumentShould()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger>();
            _mockConfiguration.Setup(x => x["ServiceBusConnectionString"]).Returns("ServiceBusConnectionString");
            _mockSleepDbService = new Mock<ISleepDbService>();

            _func = new CreateSleepDocument(_mockConfiguration.Object, _mockSleepDbService.Object);
        }

        [Fact]
        public async Task AddSleepDocumentSuccessfully()
        {
            // Arrange
            var testSleepDocument = new SleepDocument
            {
                Id = Guid.NewGuid().ToString(),
                Sleep = new Common.Models.Sleep
                {
                    MinutesAsleep = 520
                },
                DocumentType = "Test"
            };

            var testSleepDocumentString = JsonConvert.SerializeObject(testSleepDocument);

            _mockSleepDbService.Setup(x => x.AddSleepDocument(It.IsAny<Common.Models.Sleep>())).Returns(Task.CompletedTask);

            // Act
            await _func.Run(testSleepDocumentString, _mockLogger.Object);

            // Assert
            _mockSleepDbService.Verify(x => x.AddSleepDocument(It.IsAny<Common.Models.Sleep>()), Times.Once);
        }

        [Fact]
        public async Task CatchAndLogErrorWhenAddSleepDocumentThrowsException()
        {
            // Arrange
            var testSleepDocument = new SleepDocument
            {
                Id = Guid.NewGuid().ToString(),
                Sleep = new Common.Models.Sleep
                {
                    MinutesAsleep = 520
                },
                DocumentType = "Test"
            };

            var testSleepDocumentString = JsonConvert.SerializeObject(testSleepDocument);

            _mockSleepDbService.Setup(x => x.AddSleepDocument(It.IsAny<Common.Models.Sleep>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> responseAction = async () => await _func.Run(testSleepDocumentString, _mockLogger.Object);

            // Assert
            _mockSleepDbService.Verify(x => x.AddSleepDocument(It.IsAny<Common.Models.Sleep>()), Times.Never);
            await responseAction.Should().ThrowAsync<Exception>();
        }
    }
}
