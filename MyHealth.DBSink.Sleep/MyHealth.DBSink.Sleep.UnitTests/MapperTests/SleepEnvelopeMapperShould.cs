using FluentAssertions;
using Moq;
using MyHealth.DBSink.Sleep.Mappers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

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
    }
}
