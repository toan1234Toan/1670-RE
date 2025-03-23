namespace recruitment.Models
{
	public class Profile
	{
		public int ProfileId { get; set; }
		public string FullName { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }

		// Foreign key for User
		public int Id { get; set; }
        public User? User { get; set; }

        // Additional properties here if needed
    }

}
