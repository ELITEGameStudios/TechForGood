namespace YouNiverse.Models;

public class TimeEntry
{
	// Required
	public int Id { get; set; }

	public DateTime ClockIn { get; set; }
	public DateTime? ClockOut { get; set; }

	public int StudentId { get; set; }
	public TimesheetUser? Student { get; set; }
}