using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models;
using YouNiverse.Models.LabSignin;
using YouNiverse.Models.Youniverse;

namespace YouNiverse.Controllers;

public class LabController : Controller
{
	private readonly UserContext _context;

	public LabController(UserContext userContext)
	{
		_context = userContext;
	}

	public async Task<IActionResult> Index()
	{
		await HttpContext.SignOutAsync();
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Index(LabLoginViewModel model)
	{
		if (model.StudentId.ToString().Length != 9)
		{
			ViewData["loginError"] = "Invalid Student ID.";
			return View();
		}

		LabAccount? student = await _context.LabUsers.FirstOrDefaultAsync(u => u.StudentId == model.StudentId);

		if (student == null)
		{
			LabRegisterModel registerModel = new()
			{
				StudentId = model.StudentId
			};
			return RedirectToAction("Register", registerModel);
		}

		bool isClockedIn = await _context.TimeEntries.AnyAsync(e => e.UserId == student.Id && e.ClockOut == null);

		if (isClockedIn)
		{
			return await ClockOut(student.Id);
		}

		return await ClockIn(student.Id);
	}

	[HttpPost]
	public async Task<IActionResult> Signin(SigninViewModel model, string? ReturnUrl)
	{
		if (model.StudentId == null)
		{
			ViewData["loginError"] = "Please enter a Student ID.";
			return View("Index");
		}

		if (model.StudentId != null && model.StudentId.Length != 9)
		{
			ViewData["loginError"] = "Invalid student ID.";
			return View("Index");
		}

		if (!int.TryParse(model.StudentId, out int nStudentId))
		{
			ViewData["loginError"] = "Invalid student ID.";
			return View("Index");
		}

		Console.WriteLine($"Signin request for {model.StudentId}");

		LabAccount? lab = await _context.LabUsers.FirstOrDefaultAsync(
			u => u.AccountType == EAccountType.LabAndYouniverse && u.StudentId == nStudentId);
		if (lab == null)
		{
			UserRegisterGetModel registerModel = new()
			{
				StudentId = nStudentId,
			};
			return RedirectToAction("Register", registerModel);
		}

		// todo: verify password

		await AccountController.SignInAsync(nStudentId, _context, HttpContext);

		if (ReturnUrl != null)
			return Redirect(ReturnUrl);

		return RedirectToAction("Index", "Account");
	}

	public IActionResult Register(UserRegisterGetModel model)
	{
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> Register(UserRegisterPostModel model)
	{
		UserRegisterGetModel? errors = await AccountController.RegisterAsync(model, _context);

		if (errors != null)
		{
			return View(errors);
		}

		LabAccount lab = await _context.LabUsers.FirstOrDefaultAsync(u => u.StudentId == model.StudentId)
			?? throw new Exception("Failed to get lab user after register");

		return await ClockIn(lab.Id);
	}

	public IActionResult Guest(GuestSigninGetModel model)
	{
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> Guest(GuestSigninPostModel model)
	{
		GuestSigninGetModel get = new()
		{
			FirstName = model.FirstName,
			LastName = model.LastName,
		};

		if (model.FirstName == null || model.LastName == null)
		{
			get.Error = "Error signing in";
			return View(get);
		}

		LabAccount? user = await _context.LabUsers.FirstOrDefaultAsync(
			u => u.FirstName == model.FirstName && u.LastName == model.LastName);

		if (user == null)
		{
			user = new()
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				AccountType = EAccountType.LabGuest,
			};
			await _context.LabUsers.AddAsync(user);
			await _context.SaveChangesAsync();
		}

		bool clockedIn = await _context.TimeEntries.AnyAsync(t => t.ClockOut == null && t.UserId == user.Id);

		if (clockedIn)
		{
			return await ClockOut(user.Id);
		}

		return await ClockIn(user.Id);
	}

	async Task<IActionResult> ClockIn(int userid)
	{
		LabAccount? lab = await _context.LabUsers.FindAsync(userid);

		if (lab == null)
		{
			ViewData["loginError"] = "Error clocking out (couldn't find user).";
			return View();
		}

		ViewData["clockMessage"] = $"Signed in at {DateTime.Now:hh:mm tt}.";

		if (lab.AccountType == EAccountType.LabAndYouniverse && lab.StudentId.HasValue)
		{
			ViewData["clockMessage"] += $"\n\nStudent ID: {lab.StudentId.Value}";
		}

		ViewData["clockMessage"] += $"\n\nName: {lab.FirstName} {lab.LastName}";

		TimeEntry entry = new()
		{
			UserId = userid,
			ClockIn = DateTime.Now,
			ClockOut = null,
		};
		await _context.TimeEntries.AddAsync(entry);
		await _context.SaveChangesAsync();

		return View("ClockInOut");
	}

	async Task<IActionResult> ClockOut(int userId)
	{
		var activeEntry = await _context.TimeEntries
				.Where(e => e.UserId == userId && e.ClockOut == null)
				.OrderByDescending(e => e.ClockIn)
				.FirstOrDefaultAsync();

		if (activeEntry != null)
		{
			activeEntry.ClockOut = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			LabAccount user = (await _context.LabUsers.FindAsync(userId))!;

			float hourDiff = (float)(activeEntry.ClockOut - activeEntry.ClockIn).Value.TotalHours;
			user.Hours += hourDiff;

			await _context.SaveChangesAsync();

			ViewData["clockMessage"] = $"Signed out at {DateTime.Now:hh:mm tt}.";

			if (user.AccountType == EAccountType.LabAndYouniverse && user.StudentId.HasValue)
			{
				ViewData["clockMessage"] += $"\n\nStudent ID: {user.StudentId.Value}";
			}

			ViewData["clockMessage"] += $"\n\nName: {user.FirstName} {user.LastName}";
		}
		else
		{
			ViewData["clockMessage"] = $"Error clocking out.";
		}

		return View("ClockInOut");
	}
}
