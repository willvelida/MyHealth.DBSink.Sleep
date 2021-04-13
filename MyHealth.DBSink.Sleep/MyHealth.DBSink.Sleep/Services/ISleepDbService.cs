using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Services
{
    public interface ISleepDbService
    {
        Task AddSleepDocument(mdl.Sleep sleep);
    }
}
