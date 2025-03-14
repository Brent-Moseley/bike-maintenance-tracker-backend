namespace BikeMaintTracker.Server.Models
{
    public class Users
    {
        public string id { get; set; } // Corresponds to id (Primary Key, NOT NULL)
        public string passCode { get; set; } // Corresponds to passCode (NOT NULL)
        public string name { get; set; } // Corresponds to name (NOT NULL)
        public string? email { get; set; } // Corresponds to email (nullable)
    }

}
