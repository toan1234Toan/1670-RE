namespace recruitment.Models
{
	public class JobCategory
	{
		public int JobCategoryId { get; set; }
		public string CategoryName { get; set; }
		public string Description { get; set; }
		public ICollection<JobListing> JobListings { get; set; }
	}
}
