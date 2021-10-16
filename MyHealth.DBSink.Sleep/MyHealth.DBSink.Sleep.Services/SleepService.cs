using MyHealth.DBSink.Sleep.Repository.Interfaces;
using MyHealth.DBSink.Sleep.Services.Interfaces;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Services
{
    public class SleepService : ISleepService
    {
        private readonly ISleepRepository _sleepRepository;

        public SleepService(ISleepRepository sleepRepository)
        {
            _sleepRepository = sleepRepository;
        }

        public async Task AddSleepDocument(mdl.SleepEnvelope sleep)
        {
            try
            {
                await _sleepRepository.CreateSleep(sleep);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mdl.SleepEnvelope MapSleepToSleepEnvelope(mdl.Sleep sleep)
        {
            if (sleep == null)
                throw new Exception("No Sleep Document to Map!");

            mdl.SleepEnvelope sleepEnvelope = new mdl.SleepEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                Sleep = sleep,
                DocumentType = "Sleep",
                Date = sleep.SleepDate
            };

            return sleepEnvelope;
        }
    }
}
