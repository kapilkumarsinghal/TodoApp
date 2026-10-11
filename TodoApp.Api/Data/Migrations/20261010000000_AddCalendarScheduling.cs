using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TodoApp.Api.Data;

namespace TodoApp.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261010000000_AddCalendarScheduling")]
public sealed class AddCalendarScheduling : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'[dbo].[Todos]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[Todos] (
                    [Id] int NOT NULL IDENTITY,
                    [Title] nvarchar(200) NOT NULL,
                    [Description] nvarchar(1000) NULL,
                    [IsCompleted] bit NOT NULL,
                    [DueDate] datetime2 NULL,
                    [StartAt] datetime2 NULL,
                    [EndAt] datetime2 NULL,
                    [Category] nvarchar(40) NOT NULL CONSTRAINT [DF_Todos_Category] DEFAULT N'Personal',
                    [Priority] int NOT NULL,
                    [CreatedAt] datetime2 NOT NULL,
                    CONSTRAINT [PK_Todos] PRIMARY KEY ([Id])
                );
            END;

            IF COL_LENGTH(N'dbo.Todos', N'StartAt') IS NULL
                ALTER TABLE [dbo].[Todos] ADD [StartAt] datetime2 NULL;

            IF COL_LENGTH(N'dbo.Todos', N'EndAt') IS NULL
                ALTER TABLE [dbo].[Todos] ADD [EndAt] datetime2 NULL;

            IF COL_LENGTH(N'dbo.Todos', N'Category') IS NULL
                ALTER TABLE [dbo].[Todos]
                    ADD [Category] nvarchar(40) NOT NULL
                        CONSTRAINT [DF_Todos_Category] DEFAULT N'Personal' WITH VALUES;

            IF NOT EXISTS (
                SELECT 1 FROM sys.indexes
                WHERE [name] = N'IX_Todos_StartAt'
                  AND [object_id] = OBJECT_ID(N'[dbo].[Todos]'))
                CREATE INDEX [IX_Todos_StartAt] ON [dbo].[Todos] ([StartAt]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'[dbo].[Todos]', N'U') IS NOT NULL
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM sys.default_constraints
                    WHERE [name] = N'DF_Todos_Category'
                      AND [parent_object_id] = OBJECT_ID(N'[dbo].[Todos]'))
                    ALTER TABLE [dbo].[Todos] DROP CONSTRAINT [DF_Todos_Category];

                IF EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE [name] = N'IX_Todos_StartAt'
                      AND [object_id] = OBJECT_ID(N'[dbo].[Todos]'))
                    DROP INDEX [IX_Todos_StartAt] ON [dbo].[Todos];

                IF COL_LENGTH(N'dbo.Todos', N'StartAt') IS NOT NULL
                    ALTER TABLE [dbo].[Todos] DROP COLUMN [StartAt];
                IF COL_LENGTH(N'dbo.Todos', N'EndAt') IS NOT NULL
                    ALTER TABLE [dbo].[Todos] DROP COLUMN [EndAt];
                IF COL_LENGTH(N'dbo.Todos', N'Category') IS NOT NULL
                    ALTER TABLE [dbo].[Todos] DROP COLUMN [Category];
            END;
            """);
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
            entity.Property<bool>("IsCompleted").HasColumnType("bit");
            entity.Property<int>("Priority").HasColumnType("int");
            entity.Property<DateTime?>("StartAt").HasColumnType("datetime2");
            entity.Property<string>("Title").IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");

            entity.HasKey("Id");
            entity.HasIndex("StartAt");
            entity.ToTable("Todos");
        });
    }
}
