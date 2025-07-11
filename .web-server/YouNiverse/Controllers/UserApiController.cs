using System.Text.Json;
using System.Threading.Tasks;
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
	public async Task<string> GetAvatar(string id)
	{
		AvatarRequest request;
		if (int.TryParse(id, out int nId))
		{
			UserItem? user = await _context.UserItems.FindAsync(nId);

			if (user != null)
			{
				Dictionary<string, int> userItems = [];
				foreach (EItemSlot slot in Enum.GetValues(typeof(EItemSlot)))
				{
					userItems.Add(Enum.GetName(slot)!, 0);
				}

				request = new()
				{
					result = "success",
					equippedItems = userItems,
				};
			}
			else
			{
				request = new()
				{
					result = "no-user",
				};
			}
		}
		else
		{
			request = new()
			{
				result = "bad-id",
			};
		}

		using var stream = new MemoryStream();
		await JsonSerializer.SerializeAsync(stream, request);

		stream.Position = 0;
		using var reader = new StreamReader(stream);
		return await reader.ReadToEndAsync();
	}

	[HttpGet]
	public async Task<string> GetLabUsers()
	{
		TimeEntry[] entries_all = await _timeSheet.TimeEntries
			.Where(e => e.ClockOut == null)
			.ToArrayAsync();

		TimeEntry[] entries = [.. entries_all.Where(e => _context.UserItems.Find(e.StudentId) != null)];

		using MemoryStream stream = new();
		await JsonSerializer.SerializeAsync(stream, entries.Select(e => e.StudentId).ToArray());

		stream.Position = 0;
		using var reader = new StreamReader(stream);

		return await reader.ReadToEndAsync();
	}

	struct AvatarRequest
	{
		public string result;
		public Dictionary<string, int> equippedItems;
	}
}
