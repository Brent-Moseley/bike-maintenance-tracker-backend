
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BikeMaintTracker.Server
{
    public class Program
    {
        private static string? _DBConnection;

        public static string? getDBConnection()
        {
            return _DBConnection;
        }
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var DBConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            _DBConnection = DBConnection;

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(DBConnection));
            Console.WriteLine("Using DB connection: " + DBConnection);

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");
            TestDB.test(DBConnection);


            app.Run();

        }
    }
}
