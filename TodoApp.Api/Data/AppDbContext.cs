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

            entity.HasIndex(x => x.StartAt);
        });

        modelBuilder.Entity<Habit>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(80);
            entity.Property(x => x.Emoji).IsRequired().HasMaxLength(8).HasDefaultValue("✅");
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
