using Microsoft.AspNetCore.Mvc;

namespace TAIDE.BACKEND.Controllers
{
    public class PacienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
