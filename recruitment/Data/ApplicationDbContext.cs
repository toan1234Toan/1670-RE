using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using recruitment.Models; // Chắc chắn rằng bạn đã thêm namespace này nếu các models của bạn đặt ở một namespace khác

namespace recruitment.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// Thêm DbSet cho mỗi model/entity
		public DbSet<UserApplication> UserApplications { get; set; }
		public DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
		public DbSet<CV> CVs { get; set; }
		public DbSet<JobCategory> JobCategories { get; set; }
		public DbSet<JobListing> JobListings { get; set; }
		public DbSet<Profile> Profiles { get; set; }

        public DbSet<recruitment.Models.UserApplication> UserApplication { get; set; } = default!;

        public DbSet<recruitment.Models.User> User { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

            // create role
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "a1b2c3d4-role-id-for-admin", // Existing Admin role
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "e5f6g7h8-role-id-for-application", // Unique ID for the Application role
                Name = "Application",
                NormalizedName = "APPLICATION"
            },
            new IdentityRole
            {
                Id = "i9j0k1l2-role-id-for-manager", // Unique ID for the Manager role
                Name = "Manager",
                NormalizedName = "MANAGER"
            });

            // Seed user
            var hasher = new PasswordHasher<IdentityUser>();
            var adminUser = new IdentityUser
            {
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = string.Empty,
                Id = Guid.NewGuid().ToString() 
            };

            adminUser.PasswordHash = hasher.HashPassword(adminUser, "P@ssw0rd");

            modelBuilder.Entity<IdentityUser>().HasData(adminUser);

            // Seed user role
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "a1b2c3d4-role-id-for-admin", // This should match the Role Id provided above
                UserId = adminUser.Id // Make sure this matches the Id of the user you have created
            });

            // Seed Job Categories
            modelBuilder.Entity<JobCategory>().HasData(
                new JobCategory
                {
                    JobCategoryId = 1, // Replace with your IDs
                    CategoryName = "Information Technology",
                    Description = "Jobs related to IT sector."
                },
                new JobCategory
                {
                    JobCategoryId = 2,
                    CategoryName = "Finance",
                    Description = "Jobs in the financial sector."
                }
            );

            // Seed Application Statuses
            modelBuilder.Entity<ApplicationStatus>().HasData(
                new ApplicationStatus
                {
                    ApplicationStatusId = 1, 
                    StatusName = "Pending"
                },
                new ApplicationStatus
                {
                    ApplicationStatusId = 2,
                    StatusName = "Accepted"
                },
                new ApplicationStatus
                {
                    ApplicationStatusId = 3,
                    StatusName = "Rejected"
                }
            // Add other statuses as required
            );
        }
	}
}
