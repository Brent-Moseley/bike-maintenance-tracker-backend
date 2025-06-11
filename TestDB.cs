using BikeMaintTracker.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BikeMaintTracker.Server
{
    public static class TestDB
    {
        public static Users? GetUser(string user, string passCode)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return null;

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            Users? userVal;
            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                 userVal = context.Users.Where(userRec => userRec.name == user && userRec.passCode == passCode).FirstOrDefault();
            }
            return userVal;
        }

        public static List<Bikes> GetBikes(string user)
        {
            return GetBikesDB(user);
        }

        public static List<Bikes> GetBikesOrdered(string user, string orderBy)
        {
            return GetBikesDB("true");
        }
        public static List<Bikes> GetBikesDB(string user, string orderBy = "")
        {
            List<Bikes> list = new List<Bikes>();
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return list;

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            var bikes = new List<Bikes>();
            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                if (orderBy.Length > 0)
                {
                    bikes = context.Bikes.Where(bike => bike.userID == user).OrderBy<Bikes, string>(bike => bike.id).ToList();
                }
                else bikes = context.Bikes.Where(bike => bike.userID == user).ToList();
            }
            return bikes;
        }

        public static void AddBike(Bikes bike)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                if (bike.id != null)
                {
                    context.Bikes.Add(bike);
                    context.SaveChanges();
                }
            }
        }


        public static void UpdateBike(Bikes bike)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var existing = context.Bikes.FirstOrDefault(u => u.id == bike.id);

                if (existing != null)
                {
                    existing.brand = bike.brand;
                    existing.name = bike.name;
                    existing.notes = bike.notes;    
                    existing.totalMiles = bike.totalMiles;
                    existing.spec = bike.spec;
                    existing.dateLastServiced = bike.dateLastServiced;
                    existing.monthYearPurchased = bike.monthYearPurchased;
                    existing.milesLastServiced = bike.milesLastServiced;
                    existing.model = bike.model;
                    existing.trackBy = bike.trackBy;
                    existing.model = bike.model;

                    context.SaveChanges();
                }
            }
        }

        public static List<MaintLog> GetMaintLog(string user, string bike)
        {
            var logs = new List<MaintLog>();
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return logs;

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                logs = context.MaintLogs.Where(log => log.userID == user && log.bikeID == bike).ToList();
            }
            return logs;
        }

        public static List<Alert> GetAlerts(string user, string bike)
        {
            List<Alert> list = new List<Alert>();
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return list;

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                list = bike?.Length > 0 ? context.Alerts.Where(log => log.userID == user && log.bikeID == bike).ToList() :
                    context.Alerts.Where(log => log.userID == user).ToList();
            }
            return list;
        }

        public static void AddAlerts(Alert[] set)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                if (set.Any())
                {
                    foreach (var item in set)
                    {
                        context.Alerts.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }

        public static void DeleteAlerts(string[] alertIds)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var alertStatToDelete = context.AlertStatusMain.Where(alert => alertIds.Contains(alert.alertID)).ToList();
                var alertsToDelete = context.Alerts.Where(alert => alertIds.Contains(alert.id)).ToList();

                if (alertsToDelete.Any())
                {
                    context.AlertStatusMain.RemoveRange(alertStatToDelete);
                    context.SaveChanges();
                    context.Alerts.RemoveRange(alertsToDelete);
                    context.SaveChanges();
                }
            }
        }

        private class AlertStatusJSONFormat
        {
            public string id { get; set; }     // references the alert id
            public string status { get; set; }   // cleared, created, acknowledged, triggered
        }

        public static string GetAlertStatus(string user)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return "";

            var statusString = "[]";

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var statusSet = context.AlertStatusMain.Where(stat => stat.userId == user).ToList();
                List<AlertStatusJSONFormat> converted = [];
                if (statusSet.Any())
                {
                    statusSet.ForEach(status => {
                        converted.Add(new AlertStatusJSONFormat { 
                            id = status.alertID,
                            status = status.status
                        });
                    });
                    statusString = JsonSerializer.Serialize<AlertStatusJSONFormat[]>(converted.ToArray());
                }
            }

            return statusString;
        }

        public static async void SetAlertStatus(string user, string status)
        {
            // Load the current list of ids for the given user from the DB.
            // For each status in the given status string, look for it in the current list.
            //  If not found, add to DB.
            //  If found, and the status sring is different, update in DB.
            //  For all remaining ids in the current list, remove those from the DB (they have been deleted from the UI table).
            if (user.Length == 0 || status.Length < 10) return;

            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                //Trace.WriteLine("*****************  Updating Alert Statues *******************");
                var current = context.AlertStatusMain.Where(stat => stat.userId == user)?.ToList();

                var updatedSetFromUI = JsonSerializer.Deserialize<AlertStatusJSONFormat[]>(status)?.ToList();
                //Trace.WriteLine($"  Prcessing {updatedSetFromUI.Count} updates");
                if (updatedSetFromUI?.Count > 0)
                {
                    updatedSetFromUI.ForEach(statusInUI =>
                    {
                        //Trace.WriteLine("  UI alert: " + statusInUI.id);
                        var seek = current?.Find(s => s.alertID == statusInUI.id);
                        if (seek != null)
                        {
                            //Trace.WriteLine("     Found");
                            if (seek.status != statusInUI.status)
                            {
                                //Trace.WriteLine($"     Found, status of {statusInUI.status} does not match, so need to update in DB");
                                // Status has changed, update status
                                seek.status = statusInUI.status;
                            }
                            //else
                            //{
                            //    Trace.WriteLine("     Found and status already matches, so no need to update.");
                            //}
                        }
                        else
                        {
                            //Trace.WriteLine($"   {statusInUI.id} with status of {statusInUI.status} not found, adding this to the DB");
                            context.AlertStatusMain.Add(new AlertStatusMain
                            {
                                id = Guid.NewGuid().ToString(),
                                userId = user,
                                alertID = statusInUI.id,
                                status = statusInUI.status
                            });
                        }
                    });
                    context.SaveChanges();
                    // Now, everything from current that is NOT in updatedSetFromUI has been deleted, need to remove from DB.
                    //Trace.WriteLine("       ********  Now processing deletes ***********");
                    current?.ForEach(curr =>
                    {
                        //Trace.WriteLine("Looking for deletes, processing: " +  curr.alertID);
                        if (updatedSetFromUI.Find(us => us.id == curr.alertID) == null)
                        {
                            //Trace.WriteLine($"     {curr.alertID} in DB but not found in UI table, so this must be deleted.");
                            context.AlertStatusMain.Where(stat => stat.alertID == curr.alertID).ExecuteDelete();
                        }
                        //else
                        //{
                        //    Trace.WriteLine($"    {curr.alertID} Found in DB and UI table, no need to delete");
                        //}
                    });
                }
            }
        }

        public static int ConvertAlertStatuses()     // used for migration
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return 0;

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            int count = 0;

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var fullSet = context.AlertStatus.ToList();
                if (fullSet.Any() && fullSet.Count > 0)
                {
                    fullSet.ForEach(stat =>
                    {
                        var user = stat.userId; // which user is this for
                        var statusSet = JsonSerializer.Deserialize<AlertStatusJSONFormat[]>(stat.statusString);
                        if (statusSet?.Length > 0)
                        {
                            foreach (var item in statusSet)
                            {
                                context.AlertStatusMain.Add(new AlertStatusMain
                                {
                                    id = Guid.NewGuid().ToString(),
                                    alertID = item.id.ToString(),
                                    userId = user,
                                    status = item.status
                                });
                                count++;
                            }
                        }
                    });
                }
                context.SaveChanges();
            }

            return count;
        }


        public static void AddMaintLogs(MaintLog[] set)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                if (set.Any())
                {
                    foreach (var item in set)
                    {
                        context.MaintLogs.Add(item);
                    }
                    context.SaveChanges();
                }
            }
        }

        public static void DeleteMaintLogs(string[] logIds)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var logsToDelete = context.MaintLogs.Where(log => logIds.Contains(log.id)).ToList();

                if (logsToDelete.Any())
                {
                    context.MaintLogs.RemoveRange(logsToDelete);
                    context.SaveChanges();
                }
            }
        }


        public class DBReply
        {
            public Boolean success { get; set; }
            public String message { get; set; }
            public Boolean isnew { get; set; }
            public Int32 newid { get; set; }
        }

    }
}
