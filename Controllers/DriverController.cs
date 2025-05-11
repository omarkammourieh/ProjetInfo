public class DriverController : Controller
{
    private readonly AppDbContext _context;

    public DriverController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var driver = _context.Drivers.FirstOrDefault(d => d.Email == email && d.Password == password);
        if (driver == null)
        {
            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        HttpContext.Session.SetInt32("DriverId", driver.Id);
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
        .Where(r => r.DriverId == 0)
        .OrderBy(r => r.ScheduledDateTime)
        .ToList();

    return View(availableRides);
}

[HttpPost]
public IActionResult AcceptRide(int rideId)
{
    int? driverId = HttpContext.Session.GetInt32("DriverId");
    if (driverId == null) return RedirectToAction("Login");

    var ride = _context.Rides.Find(rideId);
    if (ride != null)
    {
        ride.DriverId = driverId.Value;
        _context.SaveChanges();
    }

    return RedirectToAction("Dashboard");
}

}
