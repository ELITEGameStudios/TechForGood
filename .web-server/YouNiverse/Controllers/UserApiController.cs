using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models;
using YouNiverse.Models.LabSignin;
using YouNiverse.Models.Youniverse;

namespace YouNiverse.Controllers;

public class UserApiController : Controller
{
	private readonly UserContext _context;

	public UserApiController(UserContext context)
	{
		_context = context;
	}

	// todo: require sign in
	[HttpGet]
	public async Task<IActionResult> GetAvatar(string id)
	{
		if (!int.TryParse(id, out int nId))
			return StatusCode(StatusCodes.Status500InternalServerError, "Bad student id.");

		YouAccount? user = await _context.Users.FindAsync(nId);

		if (user == null)
			return StatusCode(StatusCodes.Status500InternalServerError, "User does not exist.");

		if (user.SkinColor == "")
			user.SkinColor = YouAccount.k_DefaultSkinColor;

		AvatarRequest request = new()
		{
			Loadout = user.Loadout,
			SkinColor = user.SkinColor,
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
		TimeEntry[] entries = await _context.TimeEntries.Include(e => e.User)
			.Where(e => e.ClockOut == null &&
				e.User != null &&
				e.User!.AccountType == EAccountType.LabAndYouniverse)
			.ToArrayAsync();

		using MemoryStream stream = new();
		await JsonSerializer.SerializeAsync(stream, entries.Select(e => e.UserId).ToArray());

		stream.Position = 0;
		using var reader = new StreamReader(stream);

		return Content(await reader.ReadToEndAsync(), "application/json");
	}

	// todo: require sign in
	[HttpGet]
	public async Task<IActionResult> GetProfile(int id)
	{
		YouAccount? user = await _context.Users.FindAsync(id);
		LabAccount? lab = await _context.LabUsers.FindAsync(id);
		if (user == null || lab == null)
			return StatusCode(StatusCodes.Status500InternalServerError, "User does not exist.");

		if (user.PrimaryColor == "")
			user.PrimaryColor = YouAccount.k_DefaultPrimaryColor;

		if (user.SecondaryColor == "")
			user.SecondaryColor = YouAccount.k_DefaultSecondaryColor;

		BioRequest request = new()
		{
			FirstName = lab.FirstName,
			LastName = lab.LastName,
			Catchphrase = user.Catchphrase,
			Role = EnumHelpers.GetDisplayName(user.Role),
			PrimaryColor = user.PrimaryColor,
			SecondaryColor = user.SecondaryColor,
			Hours = lab.Hours,

			Year = user.Year,
			Pronouns = user.Pronouns,
			GDWTeam = user.GDWTeam,
			FavouriteGame = user.FavouriteGame,
		};

		using MemoryStream stream = new();
		await JsonSerializer.SerializeAsync(stream, request);

		stream.Position = 0;
		using var reader = new StreamReader(stream);

		return Content(await reader.ReadToEndAsync(), "application/json");
	}

	struct AvatarRequest
	{
		public ItemLoadout Loadout { get; set; }
		public string SkinColor { get; set; }
	}

	struct BioRequest
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? Catchphrase { get; set; }
		public string Role { get; set; }
		public string PrimaryColor { get; set; }
		public string SecondaryColor { get; set; }
		public float Hours { get; set; }

		public int Year { get; set; }
		public string Pronouns { get; set; }
		public string GDWTeam { get; set; }
		public string FavouriteGame { get; set; }
	}
}
