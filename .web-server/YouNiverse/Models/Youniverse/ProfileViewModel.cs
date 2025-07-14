namespace YouNiverse.Models.Youniverse;

public class ProfileViewModel
{
	public string? Catchphrase { get; set; }
	public EGameRole Role { get; set; } = EGameRole.Other;

	public string PrimaryColor { get; set; } = default!;
	public string SecondaryColor { get; set; } = default!;
}
