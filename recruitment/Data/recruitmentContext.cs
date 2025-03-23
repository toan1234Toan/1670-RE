using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using recruitment.Models;

namespace recruitment.Data
{
    public class recruitmentContext : DbContext
    {
        public recruitmentContext (DbContextOptions<recruitmentContext> options)
            : base(options)
        {
        }

        public DbSet<recruitment.Models.UserApplication> UserApplication { get; set; } = default!;

    }
}
