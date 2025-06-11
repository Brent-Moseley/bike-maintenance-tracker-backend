using System.ComponentModel.DataAnnotations.Schema;

namespace BikeMaintTracker.Server.Models
{
    public class Alert
    {
        public string id { get; set; }
        public string userID { get; set; }
        public string bikeID { get; set; }
        public string bikeName { get; set; }
        public DateTime? date { get; set; } // Nullable type

        //[NotMapped] // This property will not be saved in the DB
        //public string? isoDate { get; set; }

        public string description { get; set; }
        public int? miles { get; set; }
        public int? repeatMiles { get; set; }
        public int? repeatDays { get; set; }
        public string status { get; set; }
    }

}
