using Microsoft.AspNetCore.Mvc;

namespace BloggingApp.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
