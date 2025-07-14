using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace YouNiverse.Models.Youniverse;

public enum EItemSlot
{
	[Display(Name = "Head")]
	Head,
	[Display(Name = "Face")]
	Face,
	[Display(Name = "Shirt")]
	Shirt,
	[Display(Name = "Pants")]
	Pants,
	[Display(Name = "Shoes")]
	Shoes,
	[Display(Name = "Pets")]
	Pet,
}

public static class EnumHelpers
{
	public static string GetDisplayName(Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		var attr = field?.GetCustomAttribute<DisplayAttribute>();
		return attr?.Name ?? value.ToString();
	}
}


public class CosmeticItem
{
	// Required
	public int Id { get; set; }

	public string Name { get; set; } = default!;
	public EItemSlot ItemSlot { get; set; }
	public bool IsDefault { get; set; }

	public DateTime AddDate { get; set; }
}