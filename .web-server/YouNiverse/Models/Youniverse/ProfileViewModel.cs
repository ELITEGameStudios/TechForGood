namespace YouNiverse.Models.Youniverse;

public class ProfileViewModel
{
	public string? Catchphrase { get; set; }
	public EGameRole Role { get; set; } = EGameRole.Other;

	public string PrimaryColor { get; set; } = default!;
	public string SecondaryColor { get; set; } = default!;

	public int Year { get; set; }
	public string GDWTeam { get; set; } = default!;
	public string Pronouns { get; set; } = default!;
	public string FavouriteGame { get; set; } = default!;
}
