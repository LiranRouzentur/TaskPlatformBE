using Microsoft.EntityFrameworkCore;
using TaskPlatformBE.Models;

namespace TaskPlatformBE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<TaskStatusItem> TaskStatusItems { get; set; }
        public DbSet<TaskStatusRequirement> TaskStatusRequirements { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TaskItem
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Requirement).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Relationships
                entity.HasOne(e => e.TaskType)
                    .WithMany()
                    .HasForeignKey(e => e.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.NextAssignedUser)
                    .WithMany()
                    .HasForeignKey(e => e.NextAssignedUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure TaskType
            modelBuilder.Entity<TaskType>(entity =>
            {
                entity.HasKey(e => e.TypeId);
                entity.Property(e => e.TypeId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            });

            // Configure TaskStatusItem
            modelBuilder.Entity<TaskStatusItem>(entity =>
            {
                entity.HasKey(e => new { e.TypeId, e.StatusId });
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Requirement).HasMaxLength(200);
                
                // Relationship with TaskType
                entity.HasOne(e => e.TaskType)
                    .WithMany(t => t.Statuses)
                    .HasForeignKey(e => e.TypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure TaskStatusRequirement
            modelBuilder.Entity<TaskStatusRequirement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Requirement).HasMaxLength(500).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // Relationship with TaskItem
                entity.HasOne(e => e.Task)
                    .WithMany()
                    .HasForeignKey(e => e.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            });
        }
    }
} 