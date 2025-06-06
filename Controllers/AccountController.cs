﻿using Microsoft.AspNetCore.Mvc;
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

           if (Role?.ToLower() == "driver")
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
                    return StatusCode(400, "Email or password is missing.");

                var user = _context.Users.FirstOrDefault(u => u.Email == Email);

                if (user == null || user.Password != Password)
                    return StatusCode(401, "Invalid login.");

                // ✅ Generate JWT token
                var token = GenerateJwtToken(user);

                // ✅ Store token in a cookie so Razor can read it
                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });

                return RedirectToAction("Index", "Home");
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
        [HttpGet]
        [HttpGet]
        public IActionResult Profile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("SignUp");

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            List<Ride> rides;

            if (role == "driver")
            {
                var driver = _context.Drivers.FirstOrDefault(d => d.UserID == user.ID);
                if (driver == null) return NotFound();

                rides = _context.Rides.Where(r => r.DriverID == driver.DriverID).ToList();
            }
            else
            {
                rides = _context.Rides.Where(r => r.UserID == user.ID).ToList();
            }

            ViewBag.Rides = rides;
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(User updatedUser)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null) return NotFound();

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            _context.SaveChanges();

            ViewData["Success"] = "Profile updated successfully!";

            var rides = _context.Rides.Where(r => r.UserID == user.ID).ToList();
            ViewBag.Rides = rides;

            return View(user);
        }
        
        [HttpPost]
        [HttpPost]
        public IActionResult ChangePassword(string CurrentPassword, string NewPassword)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || user.Password != CurrentPassword)
            {
                ViewData["Error"] = "Current password is incorrect.";
                return View("Profile", user);
            }

            user.Password = NewPassword;
            _context.SaveChanges();

            ViewData["Success"] = "Password changed successfully!";

            // Load rides again to display them in the view
            List<Ride> rides;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "driver")
            {
                var driver = _context.Drivers.FirstOrDefault(d => d.UserID == user.ID);
                rides = _context.Rides.Where(r => r.DriverID == driver.DriverID).ToList();
            }
            else
            {
                rides = _context.Rides.Where(r => r.UserID == user.ID).ToList();
            }

            ViewBag.Rides = rides;

            return View("Profile", user);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt"); // just removes the JWT token
            return RedirectToAction("SignUp", "Account");
        }



        public IActionResult AboutUs()
        {
            return View();
        }

    }
}
