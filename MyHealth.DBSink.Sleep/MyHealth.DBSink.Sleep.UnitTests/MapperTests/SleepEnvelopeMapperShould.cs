using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using MyHealth.DBSink.Sleep.Mappers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.UnitTests.MapperTests
{
    public class SleepEnvelopeMapperShould
    {
        private ISleepEnvelopeMapper _sut;

        public SleepEnvelopeMapperShould()
        {
            _sut = new SleepEnvelopeMapper();
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
            var testSleep = fixture.Create<mdl.Sleep>();
            testSleep.SleepDate = "2021-08-28";

            var expectedSleepEnvelope = _sut.MapSleepToSleepEnvelope(testSleep);

            using (new AssertionScope())
            {
                expectedSleepEnvelope.Should().BeOfType<mdl.SleepEnvelope>();
                expectedSleepEnvelope.Sleep.Should().Be(testSleep);
                expectedSleepEnvelope.DocumentType.Should().Be("Sleep");
                expectedSleepEnvelope.Date.Should().Be(DateTime.Parse(testSleep.SleepDate));
            }
        }
    }
}
