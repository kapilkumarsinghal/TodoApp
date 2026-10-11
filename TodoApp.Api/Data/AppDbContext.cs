using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Models;

namespace TodoApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitCompletion> HabitCompletions => Set<HabitCompletion>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.Priority)
                .IsRequired();

            entity.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(40)
                .HasDefaultValue("Personal");

            entity.Property(x => x.OwnerId).HasMaxLength(32);
            entity.HasOne(x => x.Owner).WithMany(x => x.Todos)
                .HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.OwnerId);

            entity.HasIndex(x => x.StartAt);
        });

        modelBuilder.Entity<Habit>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(80);
            entity.Property(x => x.Emoji).IsRequired().HasMaxLength(8).HasDefaultValue("✅");
            entity.Property(x => x.OwnerId).HasMaxLength(32);
            entity.HasOne(x => x.Owner).WithMany(x => x.Habits)
                .HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property(x => x.Id).HasMaxLength(32);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.NormalizedEmail).IsRequired().HasMaxLength(256);
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
            entity.HasIndex(x => x.NormalizedEmail).IsUnique();
        });

        modelBuilder.Entity<HabitCompletion>(entity =>
        {
            entity.HasKey(x => new { x.HabitId, x.CompletedOn });
            entity.Property(x => x.CompletedOn).HasColumnType("date");
            entity.HasOne(x => x.Habit).WithMany(x => x.Completions)
                .HasForeignKey(x => x.HabitId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
