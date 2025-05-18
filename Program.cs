using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjetInfo.Data;
using ProjetInfo.Models;
using ProjetInfo.Hubs;

namespace ProjetInfo
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            // Secret JWT key (you can move this to appsettings.json later)
            var key = "this_is_a_very_secure_and_long_jwt_key_123456";

            // Add services to container
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<RideShareDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ✅ Configure JWT Authentication (BEFORE builder.Build())
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

                // ✅ Tell ASP.NET to read token from cookie
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("jwt"))
                        {
                            context.Token = context.Request.Cookies["jwt"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.MapHub<RideHub>("/rideHub");

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // 👈 must come before authorization
            app.UseAuthorization();

            app.UseSession();

            // Route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=SignUp}/{id?}");

            // ✅ OPTIONAL: Seed test driver if none exist
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
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Chat}/{action=Chat}/{id?}");
            });

            app.Run();
        
        }
    }
}
