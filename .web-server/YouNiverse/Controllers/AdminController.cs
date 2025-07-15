using Microsoft.AspNetCore.Mvc;
using YouNiverse.Models.Youniverse;
using Microsoft.EntityFrameworkCore;
using YouNiverse.Models.LabSignin;
using YouNiverse.Models;
using System.Threading.Tasks;

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
		if (model.SpriteSheet == null || model.ItemName == null)
		{
			ViewData["itemMessage"] = "Failed to add item. An input is null.";
			return View(model);
		}

		CosmeticItem item = new()
		{
			Name = model.ItemName,
			ItemSlot = model.ItemSlot,
			IsDefault = model.IsDefault,
			AddDate = DateTime.Now,
		};
		await _context.Cosmetics.AddAsync(item);
		await _context.SaveChangesAsync();

		try
		{
			// Front image
			string frontPath = $"wwwroot/items/{item.Id}.png";
			using var fs = new FileStream(frontPath, FileMode.Create);
			using var rs = model.SpriteSheet.OpenReadStream();
			await rs.CopyToAsync(fs);
		}
		catch (Exception e)
		{
			CosmeticItem? found = await _context.Cosmetics.FindAsync(item.Id);
			if (found != null)
			{
				_context.Cosmetics.Remove(found);
				await _context.SaveChangesAsync();
			}
			ViewData["itemMessage"] = $"Failed to add item #{item.Id}. {e.Message}";
			return View(model);
		}

		// Unlock for all users
		if (model.IsDefault)
		{
			await _context.Users.ForEachAsync(async u =>
			{
				UnlockEntry unlock = new()
				{
					UserId = u.Id,
					ItemId = item.Id,
					UnlockDate = DateTime.Now
				};
				await _context.Unlocks.AddAsync(unlock);
			});
		}
		await _context.SaveChangesAsync();

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
		LabAccount? lab = await _context.LabUsers.FirstOrDefaultAsync(
			u => u.StudentId == model.StudentId && u.AccountType == EAccountType.LabAndYouniverse);
		if (lab == null)
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

		_context.Unlocks.Include(u => u.Item).Where(u => u.UserId == lab.Id && u.Item.Name == model.ItemName);

		UnlockEntry unlock = new()
		{
			UserId = lab.Id,
			ItemId = item.Id,
			UnlockDate = DateTime.Now,
		};
		await _context.Unlocks.AddAsync(unlock);
		await _context.SaveChangesAsync();

		ViewData["itemMessage"] = $"Gave item {model.ItemName} to {lab.FirstName} {lab.LastName}";
		model.AllItems = await _context.Cosmetics.Select(i => i.Name!).ToArrayAsync();
		return View(model);
	}

	public async Task<IActionResult> ViewItems(ViewItemsModel model)
	{
		CosmeticItem[] arr;
		if (model.Search == null)
		{
			arr = await _context.Cosmetics.ToArrayAsync();
		}
		else
		{
			arr = await _context.Cosmetics.Where(c => c.Name.Contains(model.Search)).ToArrayAsync();
		}

		model.Items = arr;
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> EditItem(CosmeticItem model)
	{
		CosmeticItem? itemFound = await _context.Cosmetics.FindAsync(model.Id);
		if (itemFound == null)
		{
			return RedirectToAction("ViewItems");
		}

		itemFound.Name = model.Name;
		itemFound.IsDefault = model.IsDefault;
		itemFound.ItemSlot = model.ItemSlot;

		if (model.IsDefault)
		{
			// Give default item to all users
			await _context.Users.ForEachAsync(async u =>
			{
				UnlockEntry unlock = new()
				{
					ItemId = itemFound.Id,
					UserId = u.Id,
					UnlockDate = DateTime.Now,
				};
				await _context.Unlocks.AddAsync(unlock);
			});
		}

		await _context.SaveChangesAsync();

		return RedirectToAction("ViewItems");
	}

	[HttpPost]
	public async Task<IActionResult> DeleteItem(int Id)
	{
		CosmeticItem? itemFound = await _context.Cosmetics.FindAsync(Id);
		if (itemFound == null)
		{
			return RedirectToAction("ViewItems");
		}

		_context.Cosmetics.Remove(itemFound);
		await _context.Unlocks.Where(u => u.ItemId == itemFound.Id).ForEachAsync(u => _context.Remove(u));

		await _context.SaveChangesAsync();

		return RedirectToAction("ViewItems");
	}

	public async Task<IActionResult> ViewUsers(ViewUsersModel model)
	{
		IQueryable<LabAccount> query = _context.LabUsers;

		if (model.YouAccounts == true)
		{
			query = query.Where(u => u.AccountType == EAccountType.LabAndYouniverse);
		}

		if (!string.IsNullOrWhiteSpace(model.Search))
		{
			query = query.Where(u => (u.FirstName + " " + u.LastName).Contains(model.Search));
		}

		query = query.Include(u => u.TimeEntries);
		if (model.OnlyClockedIn == true)
		{
			query = query.Include(u => u.TimeEntries).Where(u => u.TimeEntries.Any(u => u.ClockOut == null));
		}

		model.Users = await query.Select(u => new ViewUsersModel.UserData()
		{
			Id = u.Id,
			StudentId = u.StudentId,
			ClockedIn = u.TimeEntries.Any(e => e.ClockOut == null),
			Name = u.FirstName + " " + u.LastName,
			AccountType = u.AccountType,
		}).ToArrayAsync();

		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> ViewUsers(ViewUsersModel model, string? dummy)
	{
		if (!model.EditId.HasValue)
		{
			model.EditId = null;
			model.EditName = null;
			model.EditClockedIn = null;

			return RedirectToAction("ViewUsers", model);
		}

		LabAccount? lab = await _context.LabUsers.FindAsync(model.EditId);

		if (lab == null)
		{
			model.EditId = null;
			model.EditName = null;
			model.EditClockedIn = null;

			return RedirectToAction("ViewUsers", model);
		}

		if (model.EditClockedIn.HasValue)
		{
			TimeEntry? activeEntry = await _context.TimeEntries.FirstOrDefaultAsync(t => t.ClockOut == null && t.UserId == lab.Id);

			if (activeEntry == null &&
				model.EditClockedIn == true)
			{
				activeEntry = new()
				{
					ClockIn = DateTime.Now,
					UserId = lab.Id
				};
				await _context.TimeEntries.AddAsync(activeEntry);
			}
			else if (activeEntry != null &&
				model.EditClockedIn == false)
			{
				activeEntry.ClockOut = DateTime.Now;
			}
		}

		if (model.EditName != null)
		{
			// todo: separate first and last name
		}

		await _context.SaveChangesAsync();

		model.EditId = null;
		model.EditName = null;
		model.EditClockedIn = null;

		return RedirectToAction("ViewUsers", model);
	}

	[HttpPost]
	public async Task<IActionResult> DeleteUser(ViewUsersModel model, string? dummy)
	{
		if (!model.EditId.HasValue)
		{
			model.EditId = null;
			model.EditName = null;
			model.EditClockedIn = null;

			return RedirectToAction("ViewUsers", model);
		}

		LabAccount? lab = await _context.LabUsers.FindAsync(model.EditId);
		YouAccount? you = await _context.Users.FindAsync(model.EditId);

		if (lab != null)
		{
			_context.LabUsers.Remove(lab);
			_context.TimeEntries.RemoveRange(_context.TimeEntries.Where(t => t.UserId == model.EditId));
		}

		if (you != null)
		{
			_context.Users.Remove(you);
		}

		await _context.SaveChangesAsync();

		model.EditId = null;
		model.EditName = null;
		model.EditClockedIn = null;

		return RedirectToAction("ViewUsers", model);
	}
}
