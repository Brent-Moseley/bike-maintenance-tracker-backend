using BikeMaintTracker.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (userVal != null) SetAlertStatus(userVal.id, "");     // Back up Alert Status set.
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

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var bikes = new List<Bikes>();
                if (orderBy.Length > 0)
                {
                    bikes = context.Bikes.Where(bike => bike.userID == user).OrderBy<Bikes, string>(bike => bike.id).ToList();
                }
                else bikes = context.Bikes.Where(bike => bike.userID == user).ToList();
                // Below may be unnecessary
                foreach (var bike in bikes)
                {
                    list.Add(bike);
                }

            }
            return list;
        }

        public static void AddBike(Bikes bike)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                //var alertsToDelete = context.Alerts.Where(alert => alertIds.Contains(alert.id)).ToList();


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

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                //var alertsToDelete = context.Alerts.Where(alert => alertIds.Contains(alert.id)).ToList();
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
            return GetMaintLogDB(user, bike);
        }

        public static List<MaintLog> GetMaintLogDB(string user, string bike)
        {
            List<MaintLog> list = new List<MaintLog>();
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return list;

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var logs = new List<MaintLog>();
                logs = context.MaintLogs.Where(log => log.userID == user && log.bikeID == bike).ToList();
                // Below may be unnecessary
                foreach (var log in logs)
                {
                    list.Add(log);
                }

            }
            return list;
        }

        public static List<Alert> GetAlerts(string user, string bike)
        {
            List<Alert> list = new List<Alert>();
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return list;

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var alerts = new List<Alert>();
                alerts = bike?.Length > 0 ? context.Alerts.Where(log => log.userID == user && log.bikeID == bike).ToList() :
                    context.Alerts.Where(log => log.userID == user).ToList();
                foreach (var alert in alerts)
                {
                    list.Add(alert);
                }

            }
            return list;
        }

        public static void AddAlerts(Alert[] set)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return;

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                //var alertsToDelete = context.Alerts.Where(alert => alertIds.Contains(alert.id)).ToList();


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

            // Save this serviceProvider in a static variable?
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

        // Alert status is simply a string of IDs anda short word for status.
        // The UI maintains a table of these, and will save anytime a change is made.
        // The alert set for a user will likely be small, rarely more than 100 records.  Never more than 500.
        // It is a simple matter to get all records for a user and build the status string.  Housekeeping can be done
        // to remove alerts that are expired, if desired.
        // Updates simply imvolve loading the current set for the user, scanning the status string, and finding the differences.
        //  Then updating the model to add, remove, and update.  Then saving these DBSet changes to the database.
        //  In this way, the UI code does not need to change at all and it will maintain scanning efficiency when
        //  running the alert cycle.  
        private class AlertStatusJSONFormat
        {
            public string id { get; set; }     // references the alert id
            public string status { get; set; }   // cleared, created, acknowledged, triggered
        }

        public static string GetAlertStatus(string user)
        {
            // Load all DB records for the given user.
            //   Convert all to the above alert status JSON format.
            //   Return to the UI.
            //   This is also an opportunity to do cleanup and remove all 'cleared' alerts that are older than one month. 

            var connectionString = Program.getDBConnection();
            if (connectionString == "") return "";

            var statusString = "[]";

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                //var status = context.AlertStatus.Where(stat => stat.userId == user).ToList();
                //if (status.Any() && status.Count > 0) statusString = status[0].statusString;
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

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            //var updated = new AlertStatus { id = Guid.NewGuid().ToString(), userId = user, statusString = status };

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                // Copy current (to be deleted) alert status to the history table.
                /*AlertStatus? old = context.AlertStatus.Where(stat => stat.userId == user).FirstOrDefault();
                if (old != null) context.AlertStatusHistory.Add(new AlertStatusHistory { 
                    id = Guid.NewGuid().ToString(), 
                    userId = old.userId, 
                    statusString = old.statusString ,
                    created_at = DateTimeOffset.UtcNow,
                });
                if (status.Length < 10)
                {
                    // Make a backup of current alert status set until a refactoring can be done
                    // to save each status as individual records instead of a JSON encoded array.
                    context.SaveChanges();
                    return;
                }
                await context.AlertStatus.Where(stat => stat.userId == user).ExecuteDeleteAsync();
                context.AlertStatus.Add(updated); */

                Trace.WriteLine("*****************  Updating Alert Statues *******************");
                var current =  context.AlertStatusMain.Where(stat => stat.userId == user)?.ToList();

                var updatedSetFromUI = JsonSerializer.Deserialize<AlertStatusJSONFormat[]>(status)?.ToList();
                Trace.WriteLine($"  Prcessing {updatedSetFromUI.Count} updates");
                if (updatedSetFromUI?.Count > 0)
                {
                    updatedSetFromUI.ForEach(statusInUI =>
                    {
                        Trace.WriteLine("  UI alert: " + statusInUI.id);

                        var seek = current.Find(s => s.alertID == statusInUI.id);
                        if (seek != null)
                        {
                            //Trace.WriteLine("     Found");
                            if (seek.status != statusInUI.status)
                            {
                                Trace.WriteLine($"     Found, status of {statusInUI.status} does not match, so need to update in DB");
                                seek.status = statusInUI.status;
                            }
                            else
                            {
                                Trace.WriteLine("     Found and status already matches, so no need to update.");
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"   {statusInUI.id} with status of {statusInUI.status} not found, adding this to the DB");
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
                    Trace.WriteLine("       ********  Now processing deletes ***********");
                    current?.ForEach(curr =>
                    {
                        //Trace.WriteLine("Looking for deletes, processing: " +  curr.alertID);
                        if (updatedSetFromUI.Find(us => us.id == curr.alertID) == null)
                        {
                            Trace.WriteLine($"     {curr.alertID} in DB but not found in UI table, so this must be deleted.");
                            context.AlertStatusMain.Where(stat => stat.alertID == curr.alertID).ExecuteDelete();
                        }
                        else
                        {
                            Trace.WriteLine($"    {curr.alertID} Found in DB and UI table, no need to delete");
                        }
                    });
                }
            }
        }

        public static int ConvertAlertStatuses()     // used for migration
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return 0;

            var statusString = "";

            // Save this serviceProvider in a static variable?
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

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                //var alertsToDelete = context.Alerts.Where(alert => alertIds.Contains(alert.id)).ToList();


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

            // Save this serviceProvider in a static variable?
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

        /* public static DBReply UpdateGoal(Bikes goal)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return new DBReply { success = false, message="No database connection"};

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                try
                {
                    var updateGoal = context.Bikes.FirstOrDefault(u => u.Id == goal.Id);
                    if (updateGoal == null)
                    {
                        // Create new

                        context.Bikes.Add(goal);
                        context.SaveChanges();
                        return new DBReply { success = true, message = "Saved", isnew = true, newid = goal.Id };
                    }
                    else
                    {
                        // Update
                        updateGoal.description = goal.description;
                        updateGoal.notes1 = goal.notes1;
                        updateGoal.notes2 = goal.notes2;
                        updateGoal.completed = goal.completed;
                        updateGoal.daynumber = goal.daynumber;
                        updateGoal.date = DateTime.SpecifyKind(updateGoal.date, DateTimeKind.Utc);
                        context.Goals.Update(updateGoal);
                        context.SaveChanges();
                        return new DBReply { success = true, message = "Updated", isnew = false, newid = goal.id };
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Handle database update exceptions
                    // Log the error or handle it accordingly
                    return new DBReply { success = false, message = "Update not successful " + ex.Message };
                }
                catch (Exception ex)
                {
                    // Handle all other exceptions
                    System.Diagnostics.Debug.WriteLine("An error occurred: " + ex.Message);
                    // Log the error or handle it accordingly
                    return new DBReply { success = false, message = "General DB error: " + ex.Message };
                }
            }
        } */

        public static void test(string connectionString = "")
        {
            if (connectionString == "") return;

            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                // Use the context to query the database
                Console.WriteLine("All Bikes:");
                var bikes = context.Bikes.ToList();
                foreach (var bike in bikes)
                {
                    Console.WriteLine($"Bike: {bike.name}, Brand: {bike.brand}");
                }

                //Console.WriteLine("Date range restricted Goals:");
                //var startDate = new DateTime(2024, 7, 10, 5, 0, 0, DateTimeKind.Utc);
                //var endDate = new DateTime(2024, 7, 12, 22, 0, 0, DateTimeKind.Utc);

                var bikeslinq = context.Bikes
                    .Where(g => g.totalMiles >= 10 && g.totalMiles <= 500)
                    .ToList();

                foreach (var bike in bikeslinq)
                {
                    Console.WriteLine($"Bike: {bike.name}, {bike.brand}");
                }


            }
        }
    }
}
