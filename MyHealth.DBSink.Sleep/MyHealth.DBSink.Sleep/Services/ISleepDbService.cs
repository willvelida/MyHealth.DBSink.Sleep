using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Services
{
    public interface ISleepDbService
    {
        Task AddSleepDocument(mdl.SleepEnvelope sleep);
    }
}
