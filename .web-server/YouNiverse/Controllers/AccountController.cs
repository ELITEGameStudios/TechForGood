using System.Drawing;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models;
using YouNiverse.Models.LabSignin;
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
		YouAccount user = (await _context.Users.FindAsync(userId))!;
		LabAccount lab = (await _context.LabUsers.FindAsync(userId))!;

		AccountViewModel model = new()
		{
			StudentId = user.Id,
			FirstName = lab.FirstName,
			LastName = lab.LastName
		};

		return View(model);
	}

	[Authorize]
	public async Task<IActionResult> DressRoom()
	{
		int userId = int.Parse(User.Identity!.Name!);
		YouAccount user = (await _context.Users.FindAsync(userId))!;

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
		YouAccount user = (await _context.Users.FindAsync(userId))!;

		int nCategories = Enum.GetValues(typeof(EItemSlot)).Length;

		for (int i = 0; i < nCategories; ++i)
		{
			if (i >= model.SelectedItems.Length)
			{
				Console.WriteLine("ERROR: Trying to set more categories than sent in request.");
				break;
			}

			int item = model.SelectedItems[i];
			bool ownsItem = await _context.Users
				.Include(u => u.Unlocks)
				.Where(u => u.Unlocks.Any(u => u.ItemId == item))
				.AnyAsync();

			if (ownsItem)
			{
				user.Loadout.SetWithSlotIndex(item, (EItemSlot)i);
			}
		}

		await _context.SaveChangesAsync();

		return RedirectToAction("Index");
	}

	[Authorize]
	public async Task<IActionResult> Profile()
	{
		int userId = int.Parse(User.Identity!.Name!);
		YouAccount user = (await _context.Users.FindAsync(userId))!;

		ProfileViewModel model = new()
		{
			Catchphrase = user.Catchphrase,
			Role = user.Role,
			PrimaryColor = user.PrimaryColor,
			SecondaryColor = user.SecondaryColor,
		};

		return View(model);
	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Profile(ProfileViewModel model)
	{
		int userId = int.Parse(User.Identity!.Name!);
		YouAccount user = (await _context.Users.FindAsync(userId))!;

		user.Catchphrase = model.Catchphrase;
		user.Role = model.Role;

		Color primary = ColorTranslator.FromHtml(model.PrimaryColor);
		user.PrimaryColor = $"#{primary.R:X2}{primary.G:X2}{primary.B:X2}";

		Color secondary = ColorTranslator.FromHtml(model.SecondaryColor);
		user.SecondaryColor = $"#{secondary.R:X2}{secondary.G:X2}{secondary.B:X2}";

		await _context.SaveChangesAsync();

		return RedirectToAction("Index");
	}

	public IActionResult Signin()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Signin(SigninViewModel model, string? ReturnUrl)
	{
		if (model.StudentId.ToString().Length != 9)
		{
			ViewData["loginError"] = "Invalid student ID.";
			return Redirect("/");
		}

		Console.WriteLine($"Signin request for {model.StudentId}");

		LabAccount? lab = await _context.LabUsers.FirstOrDefaultAsync(
			u => u.AccountType == EAccountType.LabAndYouniverse && u.StudentId == model.StudentId);
		if (lab == null)
		{
			UserRegisterGetModel registerModel = new()
			{
				StudentId = model.StudentId,
			};
			return RedirectToAction("Register", registerModel);
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
		return RedirectToAction("Index", "Lab");
	}

	public IActionResult Register(UserRegisterGetModel model)
	{
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> Register(UserRegisterPostModel model)
	{
		UserRegisterGetModel? errors = await RegisterAsync(model, _context);

		if (errors != null)
		{
			return View(errors);
		}

		await SignInAsync(model.StudentId);

		return RedirectToAction("Index");
	}

	public static async Task<UserRegisterGetModel?> RegisterAsync(UserRegisterPostModel model, UserContext _context)
	{
		bool valid = true;

		LabAccount? lab = await _context.LabUsers.FirstOrDefaultAsync(
			u => u.StudentId == model.StudentId);

		UserRegisterGetModel errors = new()
		{
			StudentId = model.StudentId,
			FirstName = model.FirstName,
			LastName = model.LastName,
		};

		if (lab != null && lab.AccountType == EAccountType.LabAndYouniverse)
		{
			errors.StudentIdError ??= "Student ID already registered.";
			valid = false;
		}

		if (model.StudentId.ToString().Length != 9)
		{
			errors.StudentIdError ??= "Invalid Student ID.";
			valid = false;
		}

		if (model.FirstName == null)
		{
			errors.FirstNameError = "Required.";
			valid = false;
		}

		if (model.LastName == null)
		{
			errors.LastNameError = "Required.";
			valid = false;
		}

		if (!valid)
		{
			return errors;
		}

		if (lab == null)
		{
			lab = new()
			{
				AccountType = EAccountType.LabAndYouniverse,
				StudentId = model.StudentId,
				FirstName = model.FirstName!,
				LastName = model.LastName!,
				Hours = 0,
			};
			await _context.LabUsers.AddAsync(lab);
			await _context.SaveChangesAsync();
		}
		else
		{
			lab.AccountType = EAccountType.LabAndYouniverse;
		}

		YouAccount newUser = new()
		{
			Id = lab.Id,
		};
		await _context.Users.AddAsync(newUser);

		// Give default items
		await _context.Cosmetics.Where(c => c.IsDefault).ForEachAsync(async c =>
		{
			UnlockEntry unlock = new()
			{
				UserId = lab.Id,
				ItemId = c.Id,
				UnlockDate = DateTime.Now
			};
			await _context.Unlocks.AddAsync(unlock);
		});
		await _context.SaveChangesAsync();

		return null;
	}

	async Task SignInAsync(int studentId)
	{
		// todo:  verify password here!

		LabAccount? lab = await _context.LabUsers.FirstOrDefaultAsync(
			u => u.StudentId == studentId && u.AccountType == EAccountType.LabAndYouniverse);
		if (lab == null)
		{
			return;
		}

		var claims = new List<Claim>
		{
			new(ClaimTypes.Name, lab.Id.ToString()),
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
