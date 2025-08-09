using System;
using Microsoft.EntityFrameworkCore;

using Group_11.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Group_11.DAL
{
    //NOTE: This class definition references the user class for this project.  
    //If your User class is called something other than AppUser, you will need
    //to change it in the line below
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //this code makes sure the database is re-created on the $5/month Azure tier
            builder.HasPerformanceLevel("Basic");
            builder.HasServiceTier("Basic");
            base.OnModelCreating(builder);

            // format relationship with student/interviewer
            builder.Entity<Interview>()
            .HasOne(i => i.Student)
            .WithMany(u => u.InterviewsAsStudent)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Interview>()
            .HasOne(i => i.Interviewer)
            .WithMany(u => u.InterviewsAsInterviewer)
            .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Position> Positions { get; set; }
    }
}
