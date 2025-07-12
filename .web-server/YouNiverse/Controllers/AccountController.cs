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
		int userId = int.Parse(User.Identity!.Name!);
		UserItem user = (await _context.UserItems.FindAsync(userId))!;

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
		int userId = int.Parse(User.Identity!.Name!);
		UserItem user = (await _context.UserItems.FindAsync(userId))!;

		List<UnlockEntry> unlocks = await _context.Unlocks
			.Include(u => u.Item)
			.Where(i => i.UserId == user.Id)
			.ToListAsync();

		int nCategories = Enum.GetValues(typeof(EItemSlot)).Length;

		DressRoomViewModel model = new()
		{
			UnlockedCategories = new DressRoomViewModel.CategoryData[nCategories],
			SelectedItems = new int[nCategories],
		};

		for (int i = 0; i < nCategories; ++i)
		{
			var slot = (EItemSlot)i;

			CosmeticItem[] catItems = [.. unlocks
				.Where(u => u.Item.ItemSlot == slot)
				.Select(u => u.Item)];

			model.UnlockedCategories[i] = new DressRoomViewModel.CategoryData
			{
				UnlockedItems = catItems,
			};

			model.SelectedItems[i] = user.Loadout.FromSlotIndex(i);
		}

		return View(model);
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> DressRoom(DressRoomViewModel model)
	{
		int userId = int.Parse(User.Identity!.Name!);
		UserItem user = (await _context.UserItems.FindAsync(userId))!;

		int nCategories = Enum.GetValues(typeof(EItemSlot)).Length;

		for (int i = 0; i < nCategories; ++i)
		{
			if (i >= model.SelectedItems.Length)
			{
				Console.WriteLine("ERROR: Trying to set more categories than sent in request.");
				break;
			}

			int item = model.SelectedItems[i];
			bool ownsItem = _context.UserItems
				.Include(u => u.Unlocks)
				.Where(u => u.Unlocks.Any(u => u.ItemId == item))
				.Any();

			if (ownsItem)
			{
				user.Loadout.SetWithSlotIndex(item, (EItemSlot)i);
			}
		}

		await _context.SaveChangesAsync();

		return RedirectToAction("DressRoom");
	}

	[Authorize]
	public async Task<IActionResult> Profile()
	{
		int userId = int.Parse(User.Identity!.Name!);
		UserItem user = (await _context.UserItems.FindAsync(userId))!;

		ProfileViewModel model = new()
		{
			Catchphrase = user.Catchphrase,
			Role = user.Role,
		};

		return View(model);
	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Profile(ProfileViewModel model)
	{
		int userId = int.Parse(User.Identity!.Name!);
		UserItem user = (await _context.UserItems.FindAsync(userId))!;

		user.Catchphrase = model.Catchphrase;
		user.Role = model.Role;

		await _context.SaveChangesAsync();

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
		await HttpContext.SignOutAsync();
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

		// Give default items
		await _context.Cosmetics.Where(c => c.IsDefault).ForEachAsync(async c =>
		{
			UnlockEntry unlock = new()
			{
				UserId = newUser.Id,
				ItemId = c.Id,
				UnlockDate = DateTime.Now
			};
			await _context.Unlocks.AddAsync(unlock);
		});
		await _context.SaveChangesAsync();

		await SignInAsync(model.StudentId);

		return RedirectToAction("Index");
	}

	async Task SignInAsync(int studentId)
	{
		// todo:  verify password here!

		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, studentId.ToString()),
			new(ClaimTypes.Role, "Student"),
		};

		var claimsIdentity = new ClaimsIdentity(
			claims, CookieAuthenticationDefaults.AuthenticationScheme);

		// todo: look at
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
}
