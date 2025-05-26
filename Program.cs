
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                options.AddPolicy("AllowFrontEnd", builder =>
                {
                    builder.WithOrigins("http://localhost:3000",
                        "https://bike-maintenance-tracker-9b3b9.web.app"
                        ) // Allow specific origin
                           .AllowAnyHeader() // Allow any headers
                           .AllowAnyMethod(); // Allow any methods (GET, POST, etc.)
                });
            });
            // Add authentication services
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "azurewebsites.net",
                        ValidAudience = "bike-maintenance-tracker-9b3b9.web.app",
                        // Note:  Move to Azure Key Vault:
                        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EkIJzl3EmmxZku1VfyXHkZIqYaL3B89xtG3iWpZsP1M="))  
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("EkIJzl3EmmxZku1VfyXHkZIqYaL3B89xtG3iWpZsP1M="))
                    };
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
            app.UseAuthentication(); // Enable authentication middleware
            app.UseAuthorization(); // Enable authorization


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //builder.WebHost.UseUrls("http://localhost:5000");

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowFrontEnd");

            app.MapControllers();

            app.MapFallbackToFile("/index.html");
            //TestDB.test(DBConnection);


            app.Run();

        }
    }
}
