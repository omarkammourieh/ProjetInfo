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
        private readonly string _jwtKey = "this_is_a_very_secret_key_123"; // TODO: move to appsettings.json

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
        public IActionResult SignUp(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.Users.Any(u => u.Email == user.Email))
                    {
                        ViewData["Error"] = "Email already registered.";
                        return View(user);
                    }

                    user.Role ??= "regular";

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    return RedirectToAction("SignUp");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = $"An error occurred: {ex.Message}";
                return View(user);
            }
        }

        // GET: SignIn page
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: Handle SignIn with JWT (no hashing)
        [HttpPost]
        public IActionResult SignIn(string Email, string Password)
        {
            try
            {
                // Log what's received
                Console.WriteLine("LOGIN ATTEMPT:");
                Console.WriteLine("Email: " + Email);
                Console.WriteLine("Password: " + Password);

                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    return StatusCode(400, "Email or password is missing.");
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == Email);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return StatusCode(401, "Invalid login.");
                }

                if (user.Password != Password)
                {
                    Console.WriteLine("Wrong password.");
                    return StatusCode(401, "Invalid login.");
                }

                var token = GenerateJwtToken(user);

                Console.WriteLine("Login successful. Returning token.");
                return Json(new { token });
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 CRASHED: " + ex.Message);
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
    }
}
