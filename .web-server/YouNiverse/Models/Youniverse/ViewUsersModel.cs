using YouNiverse.Models.LabSignin;

namespace YouNiverse.Models.Youniverse;

public class ViewUsersModel
{
	public UserData[] Users { get; set; } = [];

	// Sent in post/get
	public string? Search { get; set; }
	public bool OnlyClockedIn { get; set; }
	public bool YouAccounts { get; set; }

	// Sent when editing a user
	public int? EditId { get; set; }
	public string? EditName { get; set; }
	public bool? EditClockedIn { get; set; }

	public struct UserData
	{
		public int Id { get; set; }
		public int? StudentId { get; set; }
		public bool ClockedIn { get; set; }
		public string Name { get; set; }
		public EAccountType AccountType { get; set; }
	}
}
