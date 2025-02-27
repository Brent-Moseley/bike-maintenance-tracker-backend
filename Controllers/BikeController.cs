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

        [HttpGet]
        public IEnumerable<Bikes> Get(string user)
        {
            return TestDB.GetBikes(user);
        }

        [HttpGet("GetBikesOrdered/")]
        public IEnumerable<Bikes> GetOrdered(string user, string orderBy)
        {
            return TestDB.GetBikesOrdered(user, orderBy);
        }

        // PUT: api/users/update
        [HttpPut("updatebike")]
        public IActionResult UpdateUser([FromBody] Bikes goal)
        {
            //var result = TestDB.UpdateBike(goal);
            //System.Diagnostics.Debug.WriteLine("Updating: " + goal.id + " " + goal.daynumber);
            //if (!result.success) System.Diagnostics.Debug.WriteLine("Error: " + result.message);
            var result = "success";

            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpGet("GetMaintLog/")]
        public IEnumerable<MaintLog> GetMaintLog(string user, string bike)
        {
            return TestDB.GetMaintLog(user, bike);
        }

        [HttpPost("AddMaintLog/")]
        public void AddMaintLogs(MaintLog[] set)
        {
            TestDB.AddMaintLogs(set);
        }

        [HttpDelete("DeleteMaintLog/")]
        public void DeleteMaintLogs(string[] ids)
        {
            TestDB.DeleteMaintLogs(ids);
        }

        [HttpGet("GetAlerts/")]
        public IEnumerable<Alert> GetAlerts(string user, string? bike)
        {
            return TestDB.GetAlerts(user, bike);
        }

        [HttpPost("AddAlerts/")]
        public void AddAlerts(Alert[] set)
        {
            TestDB.AddAlerts(set);
        }

        [HttpDelete("DeleteAlerts/")]
        public void DeleteAlerts(string[] ids)
        {
            TestDB.DeleteAlerts(ids);
        }

        [HttpGet("GetAlertStatus/")]
        public IEnumerable<string> GetAlertStatus(string user)
        {
            return TestDB.GetAlertStatus(user);
        }

        [HttpPost("SetAlertStatus/")]
        public void SetAlertStatus(AlertStatus update)
        {
            TestDB.SetAlertStatus(update);
        }
    }
}


// Build confidence, build courage
