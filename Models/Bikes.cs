namespace BikeMaintTracker.Server.Models
{
    public class Bikes
    {
        public string userID { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string spec { get; set; }
        public string notes { get; set; }
        public DateTime monthYearPurchased { get; set; }
        public DateTime dateLastServiced { get; set; }
        public int milesLastServiced { get; set; }
        public int totalMiles { get; set; }
        public string trackBy
        {
            get; set;
        }
    }

}
