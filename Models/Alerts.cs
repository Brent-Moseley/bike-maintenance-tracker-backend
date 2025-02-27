﻿namespace BikeMaintTracker.Server.Models
{
    public class Alert
    {
        public string id { get; set; }
        public string userID { get; set; }
        public string bikeID { get; set; }
        public string bikeName { get; set; }
        public DateTime? date { get; set; } // Nullable type
        public string description { get; set; }
        public int? miles { get; set; } // Nullable type
        public int? repeatMiles { get; set; } // Nullable type
        public int? repeatDays { get; set; } // Nullable type
        public string status { get; set; }
    }

}
