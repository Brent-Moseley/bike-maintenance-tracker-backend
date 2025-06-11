using BikeMaintTracker.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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


        private string GenerateJwtToken(string username)
        {
            // TODO:  store security key in external store instead of hardcoded.
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String("EkIJzl3EmmxZku1VfyXHkZIqYaL3B89xtG3iWpZsP1M="));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "azurewebsites.net",
                audience: "bike-maintenance-tracker-9b3b9.web.app",     // front end web app
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET Bike/GetUser
        [HttpGet("GetUser/")]
        public UsersWithToken? GetUser(string user, string passCode)
        {
            // One time migration of alert statuses.  
            // Run when table is empty.
            //var count = TestDB.ConvertAlertStatuses();
            //Trace.WriteLine("We have now converted " + count.ToString() + "alert status records.");

            var userRec = TestDB.GetUser(user, passCode);

            UsersWithToken ret = new UsersWithToken();
            if (userRec != null)
            {
                ret.id = userRec.id;
                ret.email = userRec.email;
                ret.name = userRec.name;
                ret.passCode = userRec.passCode;

                ret.token = GenerateJwtToken(userRec.name);
            }

            return ret;
        }

        [Authorize]
        // GET Bike/{user}
        [HttpGet("{user}")]
        public IEnumerable<Bikes> Get(string user)
        {
            return TestDB.GetBikes(user);
        }

        [Authorize]
        // GET Bike/GetBikesOrdered
        [HttpGet("GetBikesOrdered/")]
        public IEnumerable<Bikes> GetOrdered(string user, string orderBy)
        {
            return TestDB.GetBikesOrdered(user, orderBy);
        }

        [Authorize]
        // POST Bike
        [HttpPost]
        public void AddBike([FromBody] Bikes bike)
        {
            TestDB.AddBike(bike);
        }

        [Authorize]
        // PUT Bike
        [HttpPut]
        public IActionResult UpdateBike([FromBody] Bikes bike)
        {
            var result = "success";
            TestDB.UpdateBike(bike);

            return Ok(JsonSerializer.Serialize(result));
        }

        [Authorize]
        // GET Bike/GetMaintLog
        [HttpGet("GetMaintLog/")]
        public IEnumerable<MaintLog> GetMaintLog(string user, string bike)
        {
            return TestDB.GetMaintLog(user, bike);
        }

        [Authorize]
        // POST Bike/AddMaintLog
        [HttpPost("AddMaintLog/")]
        public void AddMaintLogs([FromBody] MaintLog[] set)
        {
            TestDB.AddMaintLogs(set);
        }

        [Authorize]
        // DELETE Bike/DeleteMaintLog
        [HttpDelete("DeleteMaintLog/{ids}")]
        public void DeleteMaintLogs(string ids)
        {
            var list = JsonSerializer.Deserialize<string[]>(ids);
            if (list.Length == 0) { return; }
            TestDB.DeleteMaintLogs(list);
        }

        [Authorize]
        // GET Bike/GetAlerts
        [HttpGet("GetAlerts/")]
        public IEnumerable<Alert> GetAlerts(string user, string? bike)
        {
            var alerts =  TestDB.GetAlerts(user, bike);
            return alerts;
        }

        [Authorize]
        // POST Bike/AddAlerts
        [HttpPost("AddAlerts/")]
        public void AddAlerts([FromBody] Alert[] set)
        {
            TestDB.AddAlerts(set);
        }

        [Authorize]
        // DELETE Bike/DeleteAlerts
        [HttpDelete("DeleteAlerts/{ids}")]
        public void DeleteAlerts(string ids)
        {
            var list = JsonSerializer.Deserialize<string[]>(ids);
            if (list.Length == 0) { return; }

            TestDB.DeleteAlerts(list);
        }

        [Authorize]
        // GET Bike/GetAlertStatus
        [HttpGet("GetAlertStatus/{user}")]
        public string GetAlertStatus(string user)   
        {
            return TestDB.GetAlertStatus(user);
        }

        [Authorize]
        // POST Bike/SetAlertStatus
        [HttpPost("SetAlertStatus/")]
        public void SetAlertStatus([FromBody] AlertStatusUpload data)
        {
            TestDB.SetAlertStatus(data.user, data.update);
        }
    }
}


