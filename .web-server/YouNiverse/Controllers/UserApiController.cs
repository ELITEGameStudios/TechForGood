using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models.LabSignin;
using YouNiverse.Models.Youniverse;

namespace YouNiverse.Controllers;

public class UserApiController : Controller
{
	private readonly UserContext _context;
	private readonly TimesheetContext _timeSheet;

	public UserApiController(UserContext context, TimesheetContext timeSheet)
	{
		_context = context;
		_timeSheet = timeSheet;
	}

	// todo: require sign in
	[HttpGet]
	public async Task<IActionResult> GetAvatar(string id)
	{
		AvatarRequest request;
		if (!int.TryParse(id, out int nId))
			return StatusCode(StatusCodes.Status500InternalServerError, "Bad student id.");

		UserItem? user = await _context.UserItems.FindAsync(nId);

		if (user == null)
			return StatusCode(StatusCodes.Status500InternalServerError, "User does not exist.");

		request = new()
		{
			Result = "success",
			Loadout = user.Loadout
		};

		using var stream = new MemoryStream();
		await JsonSerializer.SerializeAsync(stream, request);

		stream.Position = 0;
		using var reader = new StreamReader(stream);
		return Content(await reader.ReadToEndAsync(), "application/json");
	}

	// todo: require sign in
	[HttpGet]
	public async Task<IActionResult> GetLabUsers()
	{
		TimeEntry[] entries_all = await _timeSheet.TimeEntries
			.Where(e => e.ClockOut == null)
			.ToArrayAsync();

		TimeEntry[] entries = [.. entries_all.Where(e => _context.UserItems.Find(e.StudentId) != null)];

		using MemoryStream stream = new();
		await JsonSerializer.SerializeAsync(stream, entries.Select(e => e.StudentId).ToArray());

		stream.Position = 0;
		using var reader = new StreamReader(stream);

		return Content(await reader.ReadToEndAsync(), "application/json");
	}

	struct AvatarRequest
	{
		public string Result { get; set; }
		public ItemLoadout Loadout { get; set; }
	}
}
