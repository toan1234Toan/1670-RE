namespace recruitment.Models
{
	public class UserApplication
	{
		public int Id { get; set; }
		public int CVId { get; set; }
		public int JobListingId { get; set; }
		public int ApplicationStatusId { get; set; }
		public CV? CV { get; set; }
		public JobListing? JobListing { get; set; }
		public ApplicationStatus? ApplicationStatus { get; set; }
	}
}
