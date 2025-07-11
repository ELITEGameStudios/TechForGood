using Microsoft.AspNetCore.Mvc;
using YouNiverse.Models;
using YouNiverse.Models.Youniverse;
using System.IO;
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
		if (model.FrontImage == null || model.SideImage == null || model.BackImage == null || model.ItemName == null)
		{
			ViewData["itemText"] = "Failed to add item. An input is null.";
			return View("ItemAdded");
		}

		int nextId = _context.Cosmetics.Count();

		// Front image
		string frontPath = $"wwwroot/items/{nextId}_front.png";
		using (var fs = new FileStream(frontPath, FileMode.CreateNew))
		{
			using var rs = model.FrontImage.OpenReadStream();
			await rs.CopyToAsync(fs);
		}

		// Side image
		string sidePath = $"wwwroot/items/{nextId}_side.png";
		using (var fs = new FileStream(sidePath, FileMode.CreateNew))
		{
			using var rs = model.SideImage.OpenReadStream();
			await rs.CopyToAsync(fs);
		}

		// Back image
		string backPath = $"wwwroot/items/{nextId}_back.png";
		using (var fs = new FileStream(backPath, FileMode.CreateNew))
		{
			using var rs = model.BackImage.OpenReadStream();
			await rs.CopyToAsync(fs);
		}

		CosmeticItem item = new()
		{
			Id = nextId,
			Name = model.ItemName,
		};
		await _context.Cosmetics.AddAsync(item);
		await _context.SaveChangesAsync();

		ViewData["itemText"] = $"Successfully added item #{nextId}";
		return View("ItemAdded");
	}
}
