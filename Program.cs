using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjetInfo.Data;
using ProjetInfo.Models;
using System;

namespace ProjetInfo
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Define secret key for JWT
            var key = "this_is_a_very_secret_key_123"; // TODO: Move to appsettings.json in production

            // Add services
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<RideShareDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure JWT authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add these in the correct order
            app.UseAuthentication();  // First authentication
            app.UseAuthorization();   // Then authorization
            app.UseSession();

            // Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RideShareDbContext>();

                // Uncomment to seed user
                // dbContext.Users.Add(new User
                // {
                //     FullName = "test",
                //     Email = "test1@example.com",
                //     Password = "1234",
                //     PhoneNumber = "123",
                //     Role = "regular",
                // });

                // dbContext.SaveChanges();
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RideShareDbContext>();

                if (!db.Users.Any(u => u.Role == "driver"))
                {
                    var driverUser = new User
                    {
                        FullName = "Ali Kassem",
                        Email = "ali@example.com",
                        PhoneNumber = "71123456",
                        Password = "1234",
                        Role = "driver"
                    };

                    db.Users.Add(driverUser);
                    db.SaveChanges();

                    var driver = new Driver
                    {
                        UserID = driverUser.ID,
                        LicenseNumber = "XYZ123",
                        Availability = true
                    };

                    db.Drivers.Add(driver);
                    db.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
