using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STB.MAP.UI.Models;
using System.Diagnostics;
using System.Security.Claims;
using STB.MAP.UI.Controllers;

namespace STB.MAP.UI.Controllers
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
			try
			{
				ClaimsPrincipal claimsPrincipal = this.User;

				if (claimsPrincipal != null && claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
				{
					string userName = claimsPrincipal.Identity.Name;
					ViewData["UserName"] = userName;
				}
				else
				{
					ViewData["UserName"] = "Bilinmeyen Kullanıcı";
				}

				if (!ModelState.IsValid)
				{
					// ModelState'deki hataları ele al
					var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
					ViewData["ErrorMessage"] = string.Join(", ", errorMessages);
				}

				return View();
			}
			catch (Exception ex)
			{
				ViewData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
				_logger.LogError(ex, "Index sayfasında bir hata oluştu.");
				return View();
			}
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
