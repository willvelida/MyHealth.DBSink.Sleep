using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Mappers
{
    public interface ISleepEnvelopeMapper
    {
        mdl.SleepEnvelope MapSleepToSleepEnvelope(mdl.Sleep sleep);
    }
}
