using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyHealth.Common;
using MyHealth.Common.Models;
using MyHealth.DBSink.Sleep.Functions;
using MyHealth.DBSink.Sleep.Mappers;
using MyHealth.DBSink.Sleep.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.UnitTests.FunctionTests
{
    public class CreateSleepDocumentShould
    {
        private Mock<ILogger> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ISleepDbService> _mockSleepDbService;
        private Mock<ISleepEnvelopeMapper> _mockSleepEnvelopeMapper;
        private Mock<IServiceBusHelpers> _mockServiceBusHelpers;

        private CreateSleepDocument _func;

        public CreateSleepDocumentShould()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger>();
            _mockConfiguration.Setup(x => x["ServiceBusConnectionString"]).Returns("ServiceBusConnectionString");
            _mockSleepDbService = new Mock<ISleepDbService>();
            _mockSleepEnvelopeMapper = new Mock<ISleepEnvelopeMapper>();
            _mockServiceBusHelpers = new Mock<IServiceBusHelpers>();

            _func = new CreateSleepDocument(
                _mockConfiguration.Object,
                _mockSleepDbService.Object,
                _mockSleepEnvelopeMapper.Object,
                _mockServiceBusHelpers.Object);
        }

        [Fact]
        public async Task AddSleepDocumentSuccessfully()
        {
            // Arrange
            var fixture = new Fixture();
            var testSleep = fixture.Create<mdl.Sleep>();
            var testSleepEnvelope = fixture.Create<SleepEnvelope>();
            var testSleepDocumentString = JsonConvert.SerializeObject(testSleep);

            _mockSleepEnvelopeMapper.Setup(x => x.MapSleepToSleepEnvelope(testSleep)).Returns(testSleepEnvelope);
            _mockSleepDbService.Setup(x => x.AddSleepDocument(It.IsAny<Common.Models.SleepEnvelope>())).Returns(Task.CompletedTask);

            // Act
            await _func.Run(testSleepDocumentString, _mockLogger.Object);

            // Assert
            _mockSleepEnvelopeMapper.Verify(x => x.MapSleepToSleepEnvelope(It.IsAny<mdl.Sleep>()), Times.Once);
            _mockSleepDbService.Verify(x => x.AddSleepDocument(It.IsAny<Common.Models.SleepEnvelope>()), Times.Once);
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }

        [Fact]
        public async Task CatchAndLogExceptionWhenSleepEnvelopeMapperThrowsException()
        {
            // Arrange
            var fixture = new Fixture();
            var testSleep = fixture.Create<mdl.Sleep>();
            var testSleepDocumentString = JsonConvert.SerializeObject(testSleep);

            _mockSleepEnvelopeMapper.Setup(x => x.MapSleepToSleepEnvelope(It.IsAny<mdl.Sleep>())).Throws<Exception>();

            // Act
            Func<Task> responseAction = async () => await _func.Run(testSleepDocumentString, _mockLogger.Object);

            // Assert
            _mockSleepEnvelopeMapper.Verify(x => x.MapSleepToSleepEnvelope(It.IsAny<mdl.Sleep>()), Times.Never);
            await responseAction.Should().ThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task CatchAndLogErrorWhenAddSleepDocumentThrowsException()
        {
            // Arrange
            var fixture = new Fixture();
            var testSleep = fixture.Create<mdl.Sleep>();          
            var testSleepEnvelope = fixture.Create<SleepEnvelope>();
            testSleep.SleepDate = "2021-08-28";
            var testSleepDocumentString = JsonConvert.SerializeObject(testSleep);
          
            _mockSleepEnvelopeMapper.Setup(x => x.MapSleepToSleepEnvelope(It.IsAny<mdl.Sleep>())).Returns(testSleepEnvelope);
            _mockSleepDbService.Setup(x => x.AddSleepDocument(It.IsAny<Common.Models.SleepEnvelope>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> responseAction = async () => await _func.Run(testSleepDocumentString, _mockLogger.Object);

            // Assert
            _mockSleepDbService.Verify(x => x.AddSleepDocument(It.IsAny<Common.Models.SleepEnvelope>()), Times.Never);
            await responseAction.Should().ThrowAsync<Exception>();
            _mockServiceBusHelpers.Verify(x => x.SendMessageToQueue(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
