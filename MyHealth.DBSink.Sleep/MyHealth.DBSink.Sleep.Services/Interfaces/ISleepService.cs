using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Services.Interfaces
{
    public interface ISleepService
    {
        Task AddSleepDocument(mdl.SleepEnvelope sleep);
        mdl.SleepEnvelope MapSleepToSleepEnvelope(mdl.Sleep sleep);
    }
}
