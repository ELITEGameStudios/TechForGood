using YouNiverse.Models.LabSignin;

namespace YouNiverse.Models.Youniverse;

public class ViewUsersModel
{
	public UserData[] Users { get; set; } = [];

	public string? Search { get; set; }
	public bool OnlyClockedIn { get; set; }
	public bool YouAccounts { get; set; }

	public struct UserData
	{
		public int Id { get; set; }
		public int StudentId { get; set; }
		public bool ClockedIn { get; set; }
		public string Name { get; set; }
		public EAccountType AccountType { get; set; } 
	}
}
