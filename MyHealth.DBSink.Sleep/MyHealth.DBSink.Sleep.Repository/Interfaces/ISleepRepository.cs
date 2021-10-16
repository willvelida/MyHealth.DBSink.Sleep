using MyHealth.Common.Models;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Sleep.Repository.Interfaces
{
    public interface ISleepRepository
    {
        Task CreateSleep(SleepEnvelope sleepEnvelope);
    }
}
