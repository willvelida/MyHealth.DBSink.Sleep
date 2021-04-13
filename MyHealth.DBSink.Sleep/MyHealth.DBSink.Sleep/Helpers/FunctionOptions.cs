namespace MyHealth.DBSink.Sleep.Helpers
{
    public class FunctionOptions
    {
        public string CosmosDBConnectionSetting { get; set; }
        public string DatabaseNameSetting { get; set; }
        public string ContainerNameSetting { get; set; }
        public string ServiceBusConnectionSetting { get; set; }
        public string ExceptionTopicSetting { get; set; }
        public string SleepTopicSetting { get; set; }
        public string SleepSubscriptionSetting { get; set; }
    }
}
