using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YouNiverse.Models;

namespace YouNiverse.Controllers;

public class YouController : Controller
{
	private readonly UserContext _context;

	public YouController(UserContext context)
	{
		_context = context;
	}

	public IActionResult Index()
	{
		return View();
	}
}
