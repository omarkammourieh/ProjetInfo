using Microsoft.AspNetCore.Mvc;
using ProjetInfo.Models;
using ProjetInfo.Data;

namespace ProjetInfo.Controllers
{
    public class ContactController : Controller
    {
        private readonly RideShareDbContext _context;

        public ContactController(RideShareDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                ViewBag.Message = "Your message has been sent!";
                return View();
            }

            return View(contact);
        }
    }
}
