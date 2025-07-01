using Microsoft.AspNetCore.Mvc;

namespace LibraryEvents.Web.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}