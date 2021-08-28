using MyHealth.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Mappers
{
    public class SleepEnvelopeMapper : ISleepEnvelopeMapper
    {
        public mdl.SleepEnvelope MapSleepToSleepEnvelope(mdl.Sleep sleep)
        {
            if (sleep == null)
                throw new Exception("No Sleep Document to Map!");

            mdl.SleepEnvelope sleepEnvelope = new SleepEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                Sleep = sleep,
                DocumentType = "Sleep",
                Date = DateTime.ParseExact(sleep.SleepDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };

            return sleepEnvelope;
        }
    }
}
