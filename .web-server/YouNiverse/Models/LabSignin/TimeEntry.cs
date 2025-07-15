namespace YouNiverse.Models.LabSignin;

public class TimeEntry
{
	// Required
	public int Id { get; set; }

	public DateTime ClockIn { get; set; }
	public DateTime? ClockOut { get; set; }

	public int UserId { get; set; }
	public LabAccount? User { get; set; }
}