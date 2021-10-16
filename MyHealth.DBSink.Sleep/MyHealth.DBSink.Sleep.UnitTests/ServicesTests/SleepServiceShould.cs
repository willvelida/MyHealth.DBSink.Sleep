using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using MyHealth.Common.Models;
using MyHealth.DBSink.Sleep.Repository.Interfaces;
using MyHealth.DBSink.Sleep.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MyHealth.DBSink.Sleep.UnitTests.ServicesTests
{
    public class SleepServiceShould
    {
        private Mock<ISleepRepository> _mockSleepRepository;

        private SleepService _sut;

        public SleepServiceShould()
        {
            _mockSleepRepository = new Mock<ISleepRepository>();

            _sut = new SleepService(_mockSleepRepository.Object);
        }

        [Fact]
        public async Task AddSleepDocumentWhenCreateItemAsyncIsCalled()
        {
            // Arrange
            var fixture = new Fixture();
            var testSleepDocument = fixture.Create<SleepEnvelope>();

            // Act
            Func<Task> serviceAction = async () => await _sut.AddSleepDocument(testSleepDocument);

            // Assert
            await serviceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task CatchExceptionWhenCreateItemAsyncThrowsException()
        {
            // Arrange
            var fixture = new Fixture();
            var testSleepDocument = fixture.Create<SleepEnvelope>();

            _mockSleepRepository.Setup(x => x.CreateSleep(It.IsAny<SleepEnvelope>())).Throws(new Exception());

            // Act
            Func<Task> serviceAction = async () => await _sut.AddSleepDocument(testSleepDocument);

            // Assert
            await serviceAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public void ThrowExceptionWhenIncomingSleepObjectIsNull()
        {
            Action sleepEnvelopeMapperAction = () => _sut.MapSleepToSleepEnvelope(null);

            sleepEnvelopeMapperAction.Should().Throw<Exception>().WithMessage("No Sleep Document to Map!");
        }

        [Fact]
        public void MapSleepToSleepEnvelopeCorrectly()
        {
            var fixture = new Fixture();
            var testSleep = fixture.Create<Common.Models.Sleep>();
            testSleep.SleepDate = "2021-08-28";

            var expectedSleepEnvelope = _sut.MapSleepToSleepEnvelope(testSleep);

            using (new AssertionScope())
            {
                expectedSleepEnvelope.Should().BeOfType<SleepEnvelope>();
                expectedSleepEnvelope.Sleep.Should().Be(testSleep);
                expectedSleepEnvelope.DocumentType.Should().Be("Sleep");
                expectedSleepEnvelope.Date.Should().Be(testSleep.SleepDate);
            }
        }
    }
}
