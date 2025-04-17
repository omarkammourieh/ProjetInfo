using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetInfo.Data;
using ProjetInfo.Models;
using System;
using System.Linq;

namespace ProjetInfo.Controllers
{
    public class RideController : Controller
    {
        private readonly RideShareDbContext _context;

        public RideController(RideShareDbContext context)
        {
            _context = context;
        }

        // GET: /Ride/BookRide
        [HttpGet]
        public IActionResult BookRide()
        {
            return View();
        }

        // POST: /Ride/BookRide
        [HttpPost]
        public IActionResult BookRide(string pickup, string dropoff)
        {
            // Get all available drivers that have linked user and vehicles
            var availableDrivers = _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Vehicles)
                .Where(d => d.Availability == true)
                .ToList();

            if (!availableDrivers.Any())
            {
                return Json(new { message = "❌ No driver available." });
            }

            //  Pick a random driver
            var random = new Random();
            var driver = availableDrivers[random.Next(availableDrivers.Count)];

            // Get first vehicle of the driver
            var vehicle = driver.Vehicles.FirstOrDefault();

            // Choose user (use actual session logic if needed)
            var user = _context.Users.FirstOrDefault(u => u.Role == "regular");

            // Create the ride
            var ride = new Ride
            {
                UserID = user.ID,
                DriverID = driver.DriverID,
                PickupLocation = pickup,
                DropOffLocation = dropoff,
                StartTime = DateTime.Now,
                Status = "Booked"
            };

            _context.Rides.Add(ride);
            _context.SaveChanges();

            // Calculate average rating
            var ratings = _context.RideFeedbacks
                .Where(f => f.DriverID == driver.DriverID)
                .Select(f => f.Rating)
                .ToList();

            double avgRating = ratings.Any() ? Math.Round(ratings.Average(), 1) : 0;

            // Return JSON response
            return Json(new
            {
                message = "✅ Ride booked!",
                driver = new
                {
                    id = driver.DriverID,
                    name = driver.User.FullName,
                    phone = driver.User.PhoneNumber,
                    rating = avgRating,
                    vehicle = vehicle != null ? $"{vehicle.Brand} {vehicle.Model}" : "N/A",
                    plate = vehicle?.PlateNumber ?? "N/A"
                }
            });
        }

        [HttpGet]
        public IActionResult GetDriverLocation(int driverId)
        {
            var driver = _context.Drivers.FirstOrDefault(d => d.DriverID == driverId);
            if (driver == null) return NotFound();

            return Json(new { lat = driver.Latitude, lng = driver.Longitude });
        }


        [HttpPost]
        public IActionResult RateDriver(int driverId, int rating)
        {
            var userId = _context.Users.First().ID; // Replace with session/JWT logic

            var feedback = new RideFeedback
            {
                UserID = userId,
                DriverID = driverId,
                Rating = rating,
                Comments = ""
            };

            _context.RideFeedbacks.Add(feedback);
            _context.SaveChanges();

            return Ok("Rating saved!");
        }

        [HttpPost]
        public IActionResult Rate(int driverId, int rating, string comment)
        {
            // 🧠 Retrieve the user (this is temporary, replace with session/JWT logic)
            var user = _context.Users.FirstOrDefault(u => u.Role == "regular");
            if (user == null)
                return BadRequest("User not found.");

            //  Create new feedback entry
            var ride = _context.Rides
            .Where(r => r.DriverID == driverId && r.UserID == user.ID)
            .OrderByDescending(r => r.StartTime)
            .FirstOrDefault();

            if (ride == null)
                return BadRequest("Ride not found to attach feedback.");

            var feedback = new RideFeedback
            {
                RideID = ride.RideID,  
                DriverID = driverId,
                UserID = user.ID,
                Rating = rating,
                Comments = comment ?? ""
            };

            _context.RideFeedbacks.Add(feedback);
            _context.SaveChanges();

            return Ok("Thanks for your feedback!");
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
