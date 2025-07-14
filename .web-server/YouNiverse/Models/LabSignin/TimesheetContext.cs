using Microsoft.EntityFrameworkCore;

namespace YouNiverse.Models.LabSignin;

public class TimesheetContext : DbContext
{
	public TimesheetContext(DbContextOptions<TimesheetContext> options)
		: base(options)
	{
	}

	public DbSet<TimeEntry> TimeEntries { get; set; } = default!;
	public DbSet<TimesheetUser> Users { get; set; } = default!;
}