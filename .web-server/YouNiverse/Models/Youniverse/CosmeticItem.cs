using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace YouNiverse.Models.Youniverse;

public enum EItemSlot
{
	[Display(Name = "Hats")]
	Hat,
	[Display(Name = "Pets")]
	Pet,
	[Display(Name = "Shoes")]
	Shoes,
	[Display(Name = "Shirts")]
	Shirt,
	[Display(Name = "Pants")]
	Pants,
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
}