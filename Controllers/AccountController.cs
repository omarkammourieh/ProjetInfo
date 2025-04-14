using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjetInfo.Data;
using ProjetInfo.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ProjetInfo.Controllers
{
    public class AccountController : Controller
    {
        private readonly RideShareDbContext _context;
        private readonly string _jwtKey = "this_is_a_very_secure_and_long_jwt_key_123456";

        public AccountController(RideShareDbContext context)
        {
            _context = context;
        }

        // GET: SignUp page
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Handle SignUp (no hashing)
        [HttpPost]
        [HttpPost]
public IActionResult SignUp(string FullName, string Email, string PhoneNumber, string Password, string Role, string? CarMake, string? CarModel, string? LicensePlate, string? LicenseNumber)
{
    try
    {
        if (_context.Users.Any(u => u.Email == Email))
        {
            ViewData["Error"] = "Email already registered.";
            return View();
        }

        var user = new User
        {
            FullName = FullName,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Password = Password,
            Role = Role
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        if (Role == "driver")
        {
            var driver = new Driver
            {
                UserID = user.ID,
                LicenseNumber = LicenseNumber ?? "UNKNOWN",
                Availability = true
            };

            _context.Drivers.Add(driver);
            _context.SaveChanges(); // To get DriverID

            var vehicle = new Vehicle
            {
                DriverID = driver.DriverID,
                Brand = CarMake,
                Model = CarModel,
                PlateNumber = LicensePlate
            };

            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        return RedirectToAction("BookRide", "Ride");
    }
    catch (Exception ex)
    {
        ViewData["Error"] = $"Error: {ex.Message}";
        return View();
    }
}


        // GET: SignIn page
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: Handle SignIn with JWT (no hashing)
        [HttpPost]
        [HttpPost]
        public IActionResult SignIn(string Email, string Password)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    return StatusCode(400, "Email or password is missing.");
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == Email);

                if (user == null || user.Password != Password)
                {
                    return StatusCode(401, "Invalid login.");
                }

                // Optional: generate token, store it in session if needed
                var token = GenerateJwtToken(user);

                // ✅ Redirect to booking page
                return RedirectToAction("BookRide", "Ride");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"🔥 Exception: {ex.Message}");
            }
        }

        // JWT-protected Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        // Generate JWT token
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "regular")
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public IActionResult AboutUs()
        {
            return View();
        }

    }
}
