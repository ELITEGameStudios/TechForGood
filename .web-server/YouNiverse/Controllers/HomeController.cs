using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YouNiverse.Models;

namespace YouNiverse.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
	}

	bool CheckIfSignedIn(out IActionResult result)
	{
		result = View("SigninRedirect");
		return false;
	}

	public IActionResult Index()
	{
		if (!CheckIfSignedIn(out IActionResult result))
		{
			return result;
		}

		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
