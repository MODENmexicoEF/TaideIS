using Microsoft.AspNetCore.Mvc;

namespace TAIDE.BACKEND.Controllers
{
    public class FamiliarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
