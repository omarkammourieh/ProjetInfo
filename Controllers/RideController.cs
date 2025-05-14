using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetInfo.Data;
using ProjetInfo.Models;
using System;
using System.Linq;
using System.Security.Claims;

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
        [HttpPost]
        public IActionResult BookRide(string pickup, string dropoff)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();

            // Create the ride without assigning a driver yet
            // 1. Find an available driver
            var availableDriver = _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Vehicles)
                .Where(d => d.Availability == true)
                .FirstOrDefault();

            if (availableDriver == null)
            {
                return Json(new { message = "❌ No drivers available.", driver = (object)null });
            }

            // 2. Create the ride
            var ride = new Ride
            {
                UserID = user.ID,
                DriverID = availableDriver.DriverID,
                PickupLocation = pickup,
                DropOffLocation = dropoff,
                RideDateTime = DateTime.Now,
                Status = "Booked"
            };

            _context.Rides.Add(ride);
            _context.SaveChanges();

            // 3. Get driver's vehicle info
            var vehicle = availableDriver.Vehicles.FirstOrDefault();

            // 4. Optional: calculate average rating
            var ratings = _context.RideFeedbacks
                .Where(f => f.DriverID == availableDriver.DriverID)
                .Select(f => f.Rating)
                .ToList();

            double avgRating = ratings.Any() ? Math.Round(ratings.Average(), 1) : 0;

            // 5. Return JSON
            return Json(new
            {
                message = "✅ Ride booked!",
                driver = new
                {
                    id = availableDriver.DriverID,
                    name = availableDriver.User.FullName,
                    phone = availableDriver.User.PhoneNumber,
                    rating = avgRating,
                    vehicle = vehicle != null ? $"{vehicle.Brand} {vehicle.Model}" : "N/A",
                    plate = vehicle?.PlateNumber ?? "N/A"
                }
            });
            // Save ride
            _context.Rides.Add(ride);

            // ⏬ Mark driver as unavailable
            availableDriver.Availability = false;

            // ⏬ Save both ride and driver update
            _context.SaveChanges();


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
        [HttpPost]
        public IActionResult Rate(int driverId, int rating, string comment)
        {
            // ✅ Get the logged-in user using claims
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return BadRequest("User not found.");

            // ✅ Find the correct ride to attach feedback
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

        [HttpGet]
        public IActionResult AvailableRides()
        {
            var rides = _context.Rides
     .Include(r => r.User)
     .Where(r => r.DriverID == null && r.Status == "Pending")
     .ToList();


            return View(rides);
        }

        [HttpPost]
        [HttpPost]
        [HttpPost]
        public IActionResult AcceptRide(int rideId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.Role != "driver") return Unauthorized();

            var driver = _context.Drivers.FirstOrDefault(d => d.UserID == user.ID);
            if (driver == null) return NotFound();

            var ride = _context.Rides.FirstOrDefault(r => r.RideID == rideId);
            if (ride == null || ride.Status != "Pending") return NotFound();

            ride.DriverID = driver.DriverID;
            ride.Status = "Accepted";
            _context.SaveChanges();

            return RedirectToAction("AvailableRides");
        }



        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
