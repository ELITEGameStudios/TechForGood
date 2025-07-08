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

	public IActionResult Register(int? studentId, string? firstName, string? lastName)
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

		if (!IsValidUserToRegister(input, out RegisterErrors errors))
		{
			ViewData["studentIdError"] = errors.studentIdError;
			ViewData["firstNameError"] = errors.firstNameError;
			ViewData["lastNameError"] = errors.lastNameError;

			return View();
		}

		RegisterUser(input);

		return View("Index");
	}

	private bool IsValidUserToRegister(RegisterInput input, out RegisterErrors errors)
	{
		errors = new RegisterErrors();

		bool valid = true;

		if (input.studentId == null)
		{
			errors.studentIdError = "Required.";
			valid = false;
		}
		else if (_context.UserItems.FindAsync(input.studentId).Result != null)
		{
			errors.studentIdError = "That Student ID is already registered.";
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

		return valid;
	}

	private void RegisterUser(RegisterInput input)
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
		_context.UserItems.Add(userItem);
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
