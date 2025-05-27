using Microsoft.AspNetCore.Mvc;

namespace InternshipApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
