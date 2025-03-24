
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
            // Define CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost3000", builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // Allow specific origin
                           .AllowAnyHeader() // Allow any headers
                           .AllowAnyMethod(); // Allow any methods (GET, POST, etc.)
                });
                options.AddPolicy("AllowLocalhost3000", builder =>
                {
                    builder.WithOrigins("https://bike-maintenance-tracker-9b3b9.web.app/") // Allow specific origin
                           .AllowAnyHeader() // Allow any headers
                           .AllowAnyMethod(); // Allow any methods (GET, POST, etc.)
                });
            });

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

            //builder.WebHost.UseUrls("http://localhost:5000");

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowLocalhost3000");

            app.MapControllers();

            app.MapFallbackToFile("/index.html");
            //TestDB.test(DBConnection);


            app.Run();

        }
    }
}
