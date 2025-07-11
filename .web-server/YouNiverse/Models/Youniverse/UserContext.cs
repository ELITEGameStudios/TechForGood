using Microsoft.EntityFrameworkCore;
using YouNiverse.Models.Youniverse;

namespace YouNiverse.Models;

public class UserContext : DbContext
{
	public UserContext(DbContextOptions<UserContext> options)
		: base(options)
	{
	}

	public DbSet<UserItem> UserItems { get; set; } = default!;
	public DbSet<CosmeticItem> Cosmetics { get; set; } = default!;
}