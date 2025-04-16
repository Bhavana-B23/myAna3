using System.Diagnostics;
using DataVizNavigator1.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataVizNavigator1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
