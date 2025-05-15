namespace BikeMaintTracker.Server.Models
{
    public class AlertStatusHistory
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string statusString { get; set; }
        public DateTimeOffset created_at { get; set; }
    }

    //public class AlertStatusUpload
    //{
    //    public string user { get; set; }
    //    public string update { get; set; }
    //}

}
