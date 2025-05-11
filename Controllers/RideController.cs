using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetInfo.Data;
using ProjetInfo.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace ProjetInfo.Controllers
{
    public class RideController : Controller
    {
        private readonly RideShareDbContext _context;
        private readonly UserManager<RideController> _userManager;

        public int GetC { get; private set; }

        public RideController(RideShareDbContext context, UserManager<RideController> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                UserID = user.id,
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
            var userId = _context.Users.First().id; // Replace with session/JWT logic

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
            .Where(r => r.DriverID == driverId && r.UserID == user.id)
            .OrderByDescending(r => r.StartTime)
            .FirstOrDefault();

            if (ride == null)
                return BadRequest("Ride not found to attach feedback.");

            var feedback = new RideFeedback
            {
                RideID = ride.RideID,
                DriverID = driverId,
                UserID = user.id,
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

        public IActionResult AvailableRides()
        {
            var rides = _context.Rides
                                .Where(r => r.RideID == null)
                                .ToList();
            return View(rides);
        }

        [HttpPost]
        public IActionResult ChooseRide(int rideId)
        {
            var ride = _context.Rides.FirstOrDefault(r => r.RideID == rideId);
            if (ride != null)
            {
                ride.DriverID = GetCurrentUserId();
                _context.SaveChanges();
            }
            return RedirectToAction("DriverDashboard");
        }

        public IActionResult DriverHistory()
        {
            var driverId = GetCurrentUserId();
            var rides = _context.Rides.Where(r => r.DriverID == driverId).ToList();
            return View(rides);
        }

        public async Task<IActionResult> PassengerHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            
            var rides = _context.Rides
                .Where(r => r.UserID == user.GetC)
                .OrderByDescending(r => r.ScheduledDateTime)
                .ToList();

            return View(rides);
        }

            [HttpPost]
             async Task<IActionResult> BookRide(string pickup, string dropoff, string scheduledDateTime)
            {
                var driver = _context.Drivers.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
                var user = await _userManager.GetUserAsync(User);

                if (driver == null || user == null)
                    return BadRequest("Driver or user not found");

                DateTime scheduled;
                if (!DateTime.TryParse(scheduledDateTime, out scheduled))
                    return BadRequest("Invalid datetime");

                var ride = new Ride
                {
                    PickupLocation = pickup,
                    DropOffLocation = dropoff,
                    ScheduledDateTime = scheduled,
                    DriverID = driver.DriverID,
                    UserID = user.GetCurrentUserId()
                };

                _context.Rides.Add(ride);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    driver = new
                    {
                        driver.DriverID,
                        driver.Name,
                        driver.Phone,
                        driver.Vehicle,
                        driver.Plate,
                        driver.Rating
                    }
                });
            }



        }
    }

