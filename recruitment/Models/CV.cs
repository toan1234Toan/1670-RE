using System.ComponentModel.DataAnnotations;

namespace recruitment.Models
{
	public class CV
	{
		public int CVId { get; set; }
		public string Experience { get; set; }
		public string Specialization { get; set; }
		public string SelfDescription { get; set; }
        public string UserId { get; set; } 

        public User? User { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}
