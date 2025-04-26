using Microsoft.AspNetCore.Mvc;

namespace TAIDE.BACKEND.Controllers
{
    public class PMController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
