using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouNiverse.Models;

namespace YouNiverse.Controllers;

public class AccountController : Controller
{
	private readonly UserContext _context;

	public AccountController(UserContext context)
	{
		_context = context;
	}

	[Authorize]
	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Signin()
	{
		return View();
	}
}
