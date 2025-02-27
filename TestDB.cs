using BikeMaintTracker.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BikeMaintTracker.Server
{
    public static class TestDB
    {
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
                var alertsToDelete = context.Alerts.Where(alert => alertIds.Contains(alert.id)).ToList();

                if (alertsToDelete.Any())
                {
                    context.Alerts.RemoveRange(alertsToDelete);
                    context.SaveChanges();
                }
            }
        }

        public static string GetAlertStatus(string user)
        {
            var connectionString = Program.getDBConnection();
            if (connectionString == "") return "";

            var statusString = "";

            // Save this serviceProvider in a static variable?
            var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString))
            .BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<AppDbContext>())
            {
                statusString = context.AlertStatus.Where(stat => stat.userId == user).ToList()[0].statusString;
            }
            return statusString;
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
