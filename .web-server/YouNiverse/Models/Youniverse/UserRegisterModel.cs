namespace YouNiverse.Models.Youniverse;

public class UserRegisterGetModel
{
	public int? StudentId { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }

	public string? StudentIdError { get; set; }
	public string? FirstNameError { get; set; }
	public string? LastNameError { get; set; }
}

public class UserRegisterPostModel
{
	public int StudentId { get; set; }
	public string FirstName { get; set; } = default!;
	public string LastName { get; set; } = default!;
}
