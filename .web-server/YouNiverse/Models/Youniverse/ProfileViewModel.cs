using System.ComponentModel.DataAnnotations;

namespace YouNiverse.Models.Youniverse;

public class ProfileViewModel
{
	public string? Catchphrase { get; set; }
	public EGameRole Role { get; set; } = EGameRole.Other;
}
