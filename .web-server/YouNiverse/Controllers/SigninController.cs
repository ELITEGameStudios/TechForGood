using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using YouNiverse.Models;

namespace YouNiverse.Controllers;

public class SigninController : Controller
{
	private readonly UserContext _context;

	public SigninController(UserContext context)
	{
		_context = context;
	}

	public IActionResult Index()
	{
		return View();
	}

	public async Task<IActionResult> Register(int? studentId, string? firstName, string? lastName)
	{
		if (studentId == null && firstName == null && lastName == null)
		{
			return View();
		}

		RegisterInput input = new()
		{
			studentId = studentId,
			firstName = firstName,
			lastName = lastName
		};

		RegisterErrors? errors = await IsValidUserToRegister(input);
		if (errors.HasValue)
		{
			ViewData["studentIdError"] = errors.Value.studentIdError;
			ViewData["firstNameError"] = errors.Value.firstNameError;
			ViewData["lastNameError"] = errors.Value.lastNameError;

			return View();
		}

		await RegisterUser(input);

		return View("Index");
	}

	private async Task<RegisterErrors?> IsValidUserToRegister(RegisterInput input)
	{
		RegisterErrors errors = new();

		bool valid = true;

		if (input.studentId == null)
		{
			errors.studentIdError = "Required.";
			valid = false;
		}

		if (input.firstName == null)
		{
			errors.firstNameError = "Required.";
			valid = false;
		}

		if (input.lastName == null)
		{
			errors.lastNameError = "Required.";
			valid = false;
		}

		if (!valid) return null;

		if (await _context.UserItems.FindAsync(input.studentId) != null)
		{
			errors.studentIdError = "That Student ID is already registered.";
			valid = false;
		}

		return valid ? null : errors;
	}

	private async Task RegisterUser(RegisterInput input)
	{
		if (input.studentId == null || input.lastName == null || input.firstName == null)
		{
			Console.WriteLine("Error: registration value was null after passing validation!");
			return;
		}

		UserItem userItem = new()
		{
			Id = (int)input.studentId,
			FirstName = input.firstName,
			LastName = input.lastName,
		};
		await _context.UserItems.AddAsync(userItem);
		await _context.SaveChangesAsync();
	}

	struct RegisterInput
	{
		public int? studentId;
		public string? firstName;
		public string? lastName;
	}

	struct RegisterErrors
	{
		public string? studentIdError;
		public string? firstNameError;
		public string? lastNameError;
	}
}
