using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MMSite6.Areas.Identity.Data;
using MMSite6.Models;

namespace MMSite6.Controllers
{
    public class HomeController : Controller
    {

        private readonly UserManager<MMSite6User> _userManager;
        public HomeController(UserManager<MMSite6User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.userId = _userManager.GetUserId(HttpContext.User);
            return View();
        }

		public IActionResult Biography()
		{
			return View();
		}

		public IActionResult Portfolio()
		{
			return View();
		}


		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
