using Newtonsoft.Json;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Sleep.Models
{
    public class SleepDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public mdl.Sleep Sleep { get; set; }
        public string DocumentType { get; set; }

    }
}
