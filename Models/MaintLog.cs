namespace BikeMaintTracker.Server.Models
{
    public class MaintLog
    {
        public string id { get; set; }
        public string userID { get; set; }
        public string bikeID { get; set; }
        public DateTime date { get; set; }
        public int? miles { get; set; } // Nullable type
        public string description { get; set; }
    }

}
