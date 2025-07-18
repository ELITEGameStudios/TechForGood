using Microsoft.EntityFrameworkCore;
using YouNiverse.Models.LabSignin;
using YouNiverse.Models.Youniverse;

namespace YouNiverse.Models;

public class UserContext : DbContext
{
	public UserContext(DbContextOptions<UserContext> options)
		: base(options)
	{
	}

	public DbSet<YouAccount> Users { get; set; } = default!;
	public DbSet<CosmeticItem> Cosmetics { get; set; } = default!;

	public DbSet<LabAccount> LabUsers { get; set; } = default!;
	public DbSet<TimeEntry> TimeEntries { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<YouAccount>()
			.OwnsOne(u => u.Loadout);

		base.OnModelCreating(modelBuilder);
	}
}