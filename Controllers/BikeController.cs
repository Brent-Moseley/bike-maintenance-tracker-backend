using BikeMaintTracker.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;

namespace BikeMaintTracker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikeController : ControllerBase
    {

        private readonly ILogger<BikeController> _logger;

        public BikeController(ILogger<BikeController> logger)
        {
            _logger = logger;
        }

        // GET Bike/GetUser
        [HttpGet("GetUser/")]
        public Users? GetUser(string user, string passCode)
        {
             var userRec = TestDB.GetUser(user, passCode);
            return userRec;
        }

        //https://bike-maint-tracker-hxafcdavbkghcmbw.canadacentral-01.azurewebsites.net/Bike/GetUser?user=123&passCode=456
        //    https://bike-maint-tracker-hxafcdavbkghcmbw.canadacentral-01.azurewebsites.net/Bike/12345

        // GET Bike/{user}
        [HttpGet("{user}")]
        public IEnumerable<Bikes> Get(string user)
        {
            return TestDB.GetBikes(user);
        }

        // GET Bike/GetBikesOrdered
        [HttpGet("GetBikesOrdered/")]
        public IEnumerable<Bikes> GetOrdered(string user, string orderBy)
        {
            return TestDB.GetBikesOrdered(user, orderBy);
        }

        // POST Bike
        [HttpPost]
        public void AddBike([FromBody] Bikes bike)
        {
            TestDB.AddBike(bike);
        }

        // PUT Bike
        [HttpPut]
        public IActionResult UpdateBike([FromBody] Bikes bike)
        {
            var result = "success";
            TestDB.UpdateBike(bike);

            return Ok(JsonSerializer.Serialize(result));
        }

        // GET Bike/GetMaintLog
        [HttpGet("GetMaintLog/")]
        public IEnumerable<MaintLog> GetMaintLog(string user, string bike)
        {
            return TestDB.GetMaintLog(user, bike);
        }

        // POST Bike/AddMaintLog
        [HttpPost("AddMaintLog/")]
        public void AddMaintLogs([FromBody] MaintLog[] set)
        {
            TestDB.AddMaintLogs(set);
        }

        // DELETE Bike/DeleteMaintLog
        [HttpDelete("DeleteMaintLog/{ids}")]
        public void DeleteMaintLogs(string ids)
        {
            var list = JsonSerializer.Deserialize<string[]>(ids);
            if (list.Length == 0) { return; }
            TestDB.DeleteMaintLogs(list);
        }

        // GET Bike/GetAlerts
        [HttpGet("GetAlerts/")]
        public IEnumerable<Alert> GetAlerts(string user, string? bike)
        {
            var alerts =  TestDB.GetAlerts(user, bike);
            //foreach (Alert al in alerts)
            //{
            //    al.isoDate = al.date.ToString();
            //}
            return alerts;
        }

        // POST Bike/AddAlerts
        [HttpPost("AddAlerts/")]
        public void AddAlerts([FromBody] Alert[] set)
        {
            //foreach (Alert al in set)
            //{
            //    if (al.date != null) al.date = DateTime.Parse(al.date);
            //}
            TestDB.AddAlerts(set);
        }

        // DELETE Bike/DeleteAlerts
        [HttpDelete("DeleteAlerts/{ids}")]
        public void DeleteAlerts(string ids)
        {
            var list = JsonSerializer.Deserialize<string[]>(ids);
            if (list.Length == 0) { return; }

            TestDB.DeleteAlerts(list);
        }

        // GET Bike/GetAlertStatus
        [HttpGet("GetAlertStatus/{user}")]
        public string GetAlertStatus(string user)   
        {
            return TestDB.GetAlertStatus(user);
        }

        // POST Bike/SetAlertStatus
        [HttpPost("SetAlertStatus/")]
        public void SetAlertStatus([FromBody] AlertStatusUpload data)
        {
            TestDB.SetAlertStatus(data.user, data.update);
        }
    }
}


