
using EmployeeManagementSy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace EmployeeManagementSy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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

        public DbSet<SystemProfile> SystemProfiles { get; set; }

        public DbSet<Audit> AuditLogs { get; set; }



        public virtual async Task<int> SaveChangesAsync(string userId = null)
        {
            OnBeforeSavingChanges(userId);
            var result = await base.SaveChangesAsync();

            return result;

        }

        private void OnBeforeSavingChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var audiEntries = new List<AuditEntry>();

            foreach(var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var audiEntry = new AuditEntry(entry);
                audiEntry.TableName = entry.Entity.GetType().Name;
                audiEntry.UserId = userId;
                audiEntries.Add(audiEntry);


               foreach(var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        audiEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch(entry.State)
                    {
                        case EntityState.Added:
                            audiEntry.AuditType = AuditType.Create;
                            audiEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            audiEntry.AuditType = AuditType.Delete;
                            audiEntry.OldValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                audiEntry.ChangedColumns.Add(propertyName);
                                audiEntry.AuditType = AuditType.Update;
                                audiEntry.OldValues[propertyName] = property.OriginalValue;
                                audiEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                           
                            break;
                    }
               }
            }
            foreach (var audientry in audiEntries)
            {
                AuditLogs.Add(audientry.ToAudi());

            }
        }

    }
}