using Microsoft.EntityFrameworkCore;
using JobAppTracker.Server.Models;

namespace JobAppTracker.Server.Data
{
    public class JobTrackerAppDbContext : DbContext
    {
        public JobTrackerAppDbContext(DbContextOptions<JobTrackerAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }  // optional
    }
}
