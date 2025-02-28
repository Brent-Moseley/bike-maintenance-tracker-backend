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
        [HttpDelete("DeleteMaintLog/")]
        public void DeleteMaintLogs([FromBody] string[] ids)
        {
            TestDB.DeleteMaintLogs(ids);
        }

        // GET Bike/GetAlerts
        [HttpGet("GetAlerts/")]
        public IEnumerable<Alert> GetAlerts(string user, string? bike)
        {
            return TestDB.GetAlerts(user, bike);
        }

        // POST Bike/AddAlerts
        [HttpPost("AddAlerts/")]
        public void AddAlerts([FromBody] Alert[] set)
        {
            TestDB.AddAlerts(set);
        }

        // DELETE Bike/DeleteAlerts
        [HttpDelete("DeleteAlerts/")]
        public void DeleteAlerts([FromBody] string[] ids)
        {
            TestDB.DeleteAlerts(ids);
        }

        // GET Bike/GetAlertStatus
        [HttpGet("GetAlertStatus/")]
        public string GetAlertStatus(string user)
        {
            return TestDB.GetAlertStatus(user);
        }

        // POST Bike/SetAlertStatus
        [HttpPost("SetAlertStatus/")]
        public void SetAlertStatus([FromBody] string user,string update)
        {
            TestDB.SetAlertStatus(user, update);
        }
    }
}


