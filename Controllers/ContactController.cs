using Microsoft.AspNetCore.Mvc;
using ProjetInfo.Models;
using ProjetInfo.Data;
using Microsoft.EntityFrameworkCore;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Name,Email,PhoneNumber,Message")] Contact contact)
        {
            Console.WriteLine("POST hit"); // DEBUG

            if (ModelState.IsValid)
            {
                contact.SubmittedAt = DateTime.Now;
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Thank you! Your message has been sent.";
                ModelState.Clear(); // Reset form
            }

            return View();
        }
      
        



    }
}
