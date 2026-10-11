using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TodoApp.Api.Data;

namespace TodoApp.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
public sealed class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
        => BuildCurrentModel(modelBuilder);

    internal static void BuildCurrentModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.20");

        modelBuilder.Entity("TodoApp.Api.Models.TodoItem", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int")
                .UseIdentityColumn();

            entity.Property<string>("Category")
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasMaxLength(40)
                .HasColumnType("nvarchar(40)")
                .HasDefaultValue("Personal");

            entity.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
            entity.Property<DateTime?>("DueDate").HasColumnType("datetime2");
            entity.Property<string>("Description").HasMaxLength(1000).HasColumnType("nvarchar(1000)");
            entity.Property<DateTime?>("EndAt").HasColumnType("datetime2");
            entity.Property<int?>("ReminderMinutesBefore").HasColumnType("int");
            entity.Property<int?>("RepeatEveryHours").HasColumnType("int");
            entity.Property<string>("OwnerId").HasMaxLength(32).HasColumnType("nvarchar(32)");
            entity.Property<bool>("IsCompleted").HasColumnType("bit");
            entity.Property<int>("Priority").HasColumnType("int");
            entity.Property<DateTime?>("StartAt").HasColumnType("datetime2");
            entity.Property<string>("Title").IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");

            entity.HasKey("Id");
            entity.HasIndex("OwnerId");
            entity.HasIndex("StartAt");
            entity.ToTable("Todos");
        });

        modelBuilder.Entity("TodoApp.Api.Models.Habit", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").UseIdentityColumn();
            entity.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
            entity.Property<string>("Emoji").IsRequired().ValueGeneratedOnAdd().HasMaxLength(8).HasColumnType("nvarchar(8)").HasDefaultValue("✅");
            entity.Property<string>("Name").IsRequired().HasMaxLength(80).HasColumnType("nvarchar(80)");
            entity.Property<string>("OwnerId").HasMaxLength(32).HasColumnType("nvarchar(32)");
            entity.HasKey("Id");
            entity.HasIndex("OwnerId");
            entity.ToTable("Habits");
        });

        modelBuilder.Entity("TodoApp.Api.Models.HabitCompletion", entity =>
        {
            entity.Property<int>("HabitId").HasColumnType("int");
            entity.Property<DateOnly>("CompletedOn").HasColumnType("date");
            entity.Property<DateTime>("CompletedAt").HasColumnType("datetime2");
            entity.HasKey("HabitId", "CompletedOn");
            entity.HasIndex("HabitId");
            entity.ToTable("HabitCompletions");
        });

        modelBuilder.Entity("TodoApp.Api.Models.HabitCompletion", entity =>
        {
            entity.HasOne("TodoApp.Api.Models.Habit", "Habit").WithMany("Completions")
                .HasForeignKey("HabitId").OnDelete(DeleteBehavior.Cascade).IsRequired();
            entity.Navigation("Habit");
        });

        modelBuilder.Entity("TodoApp.Api.Models.Habit", entity => entity.Navigation("Completions"));

        modelBuilder.Entity("TodoApp.Api.Models.AppUser", entity =>
        {
            entity.Property<string>("Id").HasMaxLength(32).HasColumnType("nvarchar(32)");
            entity.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
            entity.Property<string>("Email").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.Property<string>("NormalizedEmail").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.Property<string>("PasswordHash").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.HasKey("Id");
            entity.HasIndex("NormalizedEmail").IsUnique();
            entity.ToTable("Users");
        });

        modelBuilder.Entity("TodoApp.Api.Models.Habit", entity =>
        {
            entity.HasOne("TodoApp.Api.Models.AppUser", "Owner").WithMany("Habits")
                .HasForeignKey("OwnerId").OnDelete(DeleteBehavior.Cascade);
            entity.Navigation("Owner");
        });

        modelBuilder.Entity("TodoApp.Api.Models.AppUser", entity => entity.Navigation("Habits"));

        modelBuilder.Entity("TodoApp.Api.Models.TodoItem", entity =>
        {
            entity.HasOne("TodoApp.Api.Models.AppUser", "Owner").WithMany("Todos")
                .HasForeignKey("OwnerId").OnDelete(DeleteBehavior.Cascade);
            entity.Navigation("Owner");
        });
        modelBuilder.Entity("TodoApp.Api.Models.AppUser", entity => entity.Navigation("Todos"));
    }
}
