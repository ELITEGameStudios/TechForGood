using Microsoft.EntityFrameworkCore;

namespace YouNiverse.Models.Youniverse;

public class UserContext : DbContext
{
	public UserContext(DbContextOptions<UserContext> options)
		: base(options)
	{
	}

	public DbSet<UserItem> UserItems { get; set; } = default!;
	public DbSet<CosmeticItem> Cosmetics { get; set; } = default!;
	public DbSet<UnlockEntry> Unlocks { get; set; } = default!;
}