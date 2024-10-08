
using EmployeeManagementSy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace EmployeeManagementSy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set all relationships to restrict delete behavior globally
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Configure the relationship for LeaveApplication.Status
            modelBuilder.Entity<LeaveApplication>()
                .HasOne(f => f.Status)
                .WithMany()
                .HasForeignKey(f => f.StatusId)
                .OnDelete(DeleteBehavior.Restrict);  // Use Restrict to avoid cascade issues
        }


        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<SystemCode> SystemCodes { get; set; }
        public DbSet<SystemCodeDetail> SystemCodesDetail { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<LeaveApplication> LeaveApplications { get; set; }
    }
}