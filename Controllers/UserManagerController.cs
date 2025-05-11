using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetInfo.Data;

namespace ProjetInfo.Controllers
{
    public class UserManagerController : Controller
    {
        
        private readonly UserManager<RideController> _userManager;

        public UserManagerController(UserManager<RideController> userManager)
        {
            _userManager = userManager;
        }
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User);
        }
        public IActionResult SomeAction()
        {
            var userId = GetCurrentUserId();
            // Use userId...
            return View();
        }
    }
}
