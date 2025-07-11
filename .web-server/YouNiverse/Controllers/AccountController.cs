using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models.Youniverse;

namespace YouNiverse.Controllers;

public class AccountController : Controller
{
	private readonly UserContext _context;

	public AccountController(UserContext context)
	{
		_context = context;
	}

	[Authorize]
	public async Task<IActionResult> Index()
	{
		UserItem? user = await AuthenticateUser();
		if (user == null) return View("Signin");

		AccountViewModel model = new()
		{
			StudentId = user.Id,
			FirstName = user.FirstName,
			LastName = user.LastName
		};

		return View(model);
	}

	[Authorize]
	public async Task<IActionResult> DressRoom()
	{
		UserItem? user = await AuthenticateUser();
		if (user == null) return View("Signin");

		List<UnlockEntry> unlocks = await _context.Unlocks
			.Include(u => u.Item)
			.Where(i => i.UserId == user.Id)
			.ToListAsync();

		int nCategories = Enum.GetValues(typeof(EItemSlot)).Length;

		DressRoomViewModel model = new()
		{
			Categories = new DressRoomViewModel.CategoryData[nCategories]
		};

		for (int i = 0; i < nCategories; ++i)
		{
			var slot = (EItemSlot)i;

			CosmeticItem[] catItems = [.. unlocks
				.Where(u => u.Item.ItemSlot == slot)
				.Select(u => u.Item)];

			model.Categories[i] = new DressRoomViewModel.CategoryData
			{
				UnlockedItems = catItems
			};
		}

		return View(model);
	}

	public IActionResult Signin()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Signin(SigninViewModel model, string? ReturnUrl)
	{
		Console.WriteLine($"Signin request for {model.StudentId}");

		UserItem? user = await _context.UserItems.FindAsync(model.StudentId);
		if (user == null)
		{
			UserRegisterModel registerModel = new()
			{
				StudentId = model.StudentId,
			};
			return View("Register", registerModel);
		}

		// todo: verify password

		await SignInAsync(model.StudentId);

		if (ReturnUrl != null)
			return Redirect(ReturnUrl);

		return RedirectToAction("Index");
	}

	[HttpPost]
	public async Task<IActionResult> Signout()
	{
		await SignOutAsync();

		return RedirectToAction("Index");
	}

	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Register(UserRegisterModel model)
	{
		bool valid = true;

		UserItem? user = await _context.UserItems.FindAsync(model.StudentId);
		if (user != null)
		{
			ViewData["studentIdError"] ??= "Student ID already registered.";
			valid = false;
		}

		if (model.StudentId <= 0)
		{
			ViewData["studentIdError"] ??= "Invalid Student ID.";
			valid = false;
		}

		if (model.FirstName == null)
		{
			ViewData["firstNameError"] = "Required.";
			valid = false;
		}

		if (model.LastName == null)
		{
			ViewData["lastNameError"] = "Required.";
			valid = false;
		}

		if (!valid)
		{
			return View(model);
		}

		UserItem newUser = new()
		{
			Id = model.StudentId,
			FirstName = model.FirstName!,
			LastName = model.LastName!,
			Loadout = new ItemLoadout(),
		};
		await _context.UserItems.AddAsync(newUser);
		await _context.SaveChangesAsync();

		await SignInAsync(model.StudentId);

		return RedirectToAction("Index");
	}

	async Task SignInAsync(int studentId)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, studentId.ToString()),
			new(ClaimTypes.Role, "Student"),
		};

		var claimsIdentity = new ClaimsIdentity(
			claims, CookieAuthenticationDefaults.AuthenticationScheme);

		var authProperties = new AuthenticationProperties
		{
			//AllowRefresh = <bool>,
			// Refreshing the authentication session should be allowed.

			//ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
			// The time at which the authentication ticket expires. A 
			// value set here overrides the ExpireTimeSpan option of 
			// CookieAuthenticationOptions set with AddCookie.

			//IsPersistent = true,
			// Whether the authentication session is persisted across 
			// multiple requests. When used with cookies, controls
			// whether the cookie's lifetime is absolute (matching the
			// lifetime of the authentication ticket) or session-based.

			//IssuedUtc = <DateTimeOffset>,
			// The time at which the authentication ticket was issued.

			//RedirectUri = <string>
			// The full path or absolute URI to be used as an http 
			// redirect response value.
		};

		await HttpContext.SignInAsync(
			new ClaimsPrincipal(claimsIdentity),
			authProperties);
	}

	async Task SignOutAsync()
	{
		await HttpContext.SignOutAsync();
	}

	async Task<UserItem?> AuthenticateUser()
	{
		AuthenticateResult auth = await HttpContext.AuthenticateAsync();

		if (auth == null || auth.Principal == null)
		{
			return null;
		}

		var claim = auth.Principal.Claims.First(e => e.Type == ClaimTypes.Name);
		int studentId = int.Parse(claim.Value);

		UserItem? user = await _context.UserItems.FindAsync(studentId);
		if (user == null)
		{
			Console.WriteLine($"Error: Authenticated user {studentId} but the aren't in the database!?");
			ViewData["loginError"] = "Your account isn't in the database! Get help!";
			await HttpContext.SignOutAsync();
			return null;
		}

		return user;
	}
}
