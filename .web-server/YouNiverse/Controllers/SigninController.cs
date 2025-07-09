using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models;

namespace YouNiverse.Controllers;

public class SigninController : Controller
{
	private readonly TimesheetContext _context;

	public SigninController(TimesheetContext context)
	{
		_context = context;
	}

	public IActionResult Index()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Index(LabLoginViewModel model)
	{
		if (model.StudentId == 0)
		{
			ViewData["loginError"] = "Invalid Student ID.";
			return View();
		}

		TimesheetUser? student = await _context.Users.FindAsync(model.StudentId);

		if (student == null)
		{
			LabRegisterModel registerModel = new()
			{
				StudentId = model.StudentId
			};
			return View("Register", registerModel);
		}

		bool isClockedIn = await _context.TimeEntries.AnyAsync(e => e.StudentId == student.Id && e.ClockOut == null);

		if (isClockedIn)
		{
			return await ClockOut(student.Id);
		}

		return await ClockIn(student.Id);
	}

	[HttpPost]
	public async Task<IActionResult> Register(LabRegisterModel model)
	{
		bool valid = true;
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

		if (model.StudentId == 0)
		{
			ViewData["studentIdError"] = "Required.";
			valid = false;
		}

		if (!valid)
		{
			return View(model);
		}

		if (await _context.Users.FindAsync(model.StudentId) != null)
		{
			ViewData["studentIdError"] = "Student Id already registered.";
			return View(model);
		}

		TimesheetUser user = new()
		{
			Id = model.StudentId,
			FirstName = model.FirstName,
			LastName = model.LastName,
		};
		await _context.Users.AddAsync(user);
		await _context.SaveChangesAsync();

		Console.WriteLine($"Registered student {user.Id}");

		return await ClockIn(model.StudentId);
	}

	async Task<IActionResult> ClockIn(int studentId)
	{
		ViewData["clockMessage"] = $"Clocked in at {DateTime.Now:hh:mm tt}.";

		TimeEntry entry = new()
		{
			StudentId = studentId,
			ClockIn = DateTime.Now,
			ClockOut = null,
		};
		await _context.TimeEntries.AddAsync(entry);
		await _context.SaveChangesAsync();

		return View("ClockInOut");
	}

	async Task<IActionResult> ClockOut(int studentId)
	{
		var activeEntry = await _context.TimeEntries
				.Where(e => e.StudentId == studentId && e.ClockOut == null)
				.OrderByDescending(e => e.ClockIn)
				.FirstOrDefaultAsync();

		if (activeEntry != null)
		{
			activeEntry.ClockOut = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			ViewData["clockMessage"] = $"Clocked out at {DateTime.Now:hh:mm tt}.";
		}
		else
		{
			ViewData["clockMessage"] = $"Error clocking out.";
		}

		return View("ClockInOut");
	}
}
