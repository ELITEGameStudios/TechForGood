using Microsoft.AspNetCore.Mvc;
using YouNiverse.Models.Youniverse;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YouNiverse.Controllers;

public class AdminController : Controller
{
	private readonly UserContext _context;

	public AdminController(UserContext context)
	{
		_context = context;
	}

	public IActionResult Index()
	{
		// todo: authenticate and authorize
		return View();
	}

	public IActionResult AddItem()
	{
		// todo: authenticate and authorize
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> AddItem(AddItemModel model)
	{
		if (model.FrontImage == null || model.SideImage == null || model.BackImage == null || model.ItemName == null)
		{
			ViewData["itemMessage"] = "Failed to add item. An input is null.";
			return View(model);
		}

		CosmeticItem item = new()
		{
			Name = model.ItemName,
		};
		await _context.Cosmetics.AddAsync(item);
		await _context.SaveChangesAsync();

		try
		{
			// Front image
			string frontPath = $"wwwroot/items/{item.Id}_front.png";
			using (var fs = new FileStream(frontPath, FileMode.CreateNew))
			{
				using var rs = model.FrontImage.OpenReadStream();
				await rs.CopyToAsync(fs);
			}

			// Side image
			string sidePath = $"wwwroot/items/{item.Id}_side.png";
			using (var fs = new FileStream(sidePath, FileMode.CreateNew))
			{
				using var rs = model.SideImage.OpenReadStream();
				await rs.CopyToAsync(fs);
			}

			// Back image
			string backPath = $"wwwroot/items/{item.Id}_back.png";
			using (var fs = new FileStream(backPath, FileMode.CreateNew))
			{
				using var rs = model.BackImage.OpenReadStream();
				await rs.CopyToAsync(fs);
			}
		}
		catch (Exception e)
		{
			_context.Cosmetics.Remove(item);
			await _context.SaveChangesAsync();
			ViewData["itemMessage"] = $"Failed to add item #{item.Id}. {e.Message}";
			return View(model);
		}

		ViewData["itemMessage"] = $"Successfully added item #{item.Id}";
		return View();
	}

	public async Task<IActionResult> UnlockItem()
	{
		GiveItemModel model = new()
		{
			AllItems = await _context.Cosmetics.Select(i => i.Name!).ToArrayAsync(),
		};

		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> UnlockItem(GiveItemModel model)
	{
		UserItem? user = await _context.UserItems.FindAsync(model.StudentId);
		if (user == null)
		{
			ViewData["itemMessage"] = "User does not exist";
			return View(model);
		}

		CosmeticItem? item = await _context.Cosmetics.Where(u => u.Name == model.ItemName).FirstAsync();
		if (item == null)
		{
			ViewData["itemMessage"] = "Item does not exist";
			return View(model);
		}

		_context.Unlocks.Include(u => u.Item).Where(u => u.UserId == user.Id && u.Item.Name == model.ItemName);

		UnlockEntry unlock = new()
		{
			UserId = user.Id,
			ItemId = item.Id,
			UnlockDate = DateTime.Now,
		};
		await _context.Unlocks.AddAsync(unlock);
		await _context.SaveChangesAsync();

		ViewData["itemMessage"] = $"Gave item {model.ItemName} to {user.FirstName} {user.LastName}";
		model.AllItems = await _context.Cosmetics.Select(i => i.Name!).ToArrayAsync();
		return View(model);
	}
}
