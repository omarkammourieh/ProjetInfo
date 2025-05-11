using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using ProjetInfo.Models;
using ProjetInfo.Data;
using ProjetInfo.Controllers;
using Microsoft.AspNetCore.Identity;   

public class DriverController : Controller
{
    
    private readonly RideShareDbContext _context;
    private readonly UserManager<RideController> _userManager;

    

    public DriverController(RideShareDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var driver = _context.Drivers.FirstOrDefault(d => d.Email == email && d.Password == password);
        if (driver == null)
        {
            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        HttpContext.Session.SetInt32("DriverId", driver.DriverID);
        return RedirectToAction("Dashboard");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    public IActionResult Dashboard()
    {
        int? driverId = HttpContext.Session.GetInt32("DriverId");
        if (driverId == null)
            return RedirectToAction("Login");

        var availableRides = _context.Rides
            .Where(r => r.DriverID == 0 || r.DriverID == null)
            .OrderBy(r => r.ScheduledDateTime)
            .ToList();

        return View(availableRides);
    }

    [HttpPost]
    public IActionResult AcceptRide(int rideId)
    {
        int? driverId = HttpContext.Session.GetInt32("DriverId");
        if (driverId == null)
            return RedirectToAction("Login");

        var ride = _context.Rides.Find(rideId);
        if (ride != null && (ride.DriverID == 0 || ride.DriverID == null))
        {
            ride.DriverID = driverId.Value;
            _context.SaveChanges();
        }

        return RedirectToAction("Dashboard");
    }

    public IActionResult RideHistory()
    {
        int? driverId = HttpContext.Session.GetInt32("DriverId");
        if (driverId == null)
            return RedirectToAction("Login");

        var rides = _context.Rides
            .Where(r => r.DriverID == driverId)
            .OrderByDescending(r => r.ScheduledDateTime)
            .ToList();

        return View(rides);
    }
}
