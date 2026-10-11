using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TodoApp.Api.Data;

namespace TodoApp.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261011000000_AddHabitsAndEventOptions")]
public sealed class AddHabitsAndEventOptions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        IF COL_LENGTH(N'dbo.Todos', N'ReminderMinutesBefore') IS NULL
            ALTER TABLE [dbo].[Todos] ADD [ReminderMinutesBefore] int NULL;
        IF COL_LENGTH(N'dbo.Todos', N'RepeatEveryHours') IS NULL
            ALTER TABLE [dbo].[Todos] ADD [RepeatEveryHours] int NULL;

        IF OBJECT_ID(N'[dbo].[Habits]', N'U') IS NULL
            CREATE TABLE [dbo].[Habits] (
                [Id] int NOT NULL IDENTITY,
                [Name] nvarchar(80) NOT NULL,
                [Emoji] nvarchar(8) NOT NULL CONSTRAINT [DF_Habits_Emoji] DEFAULT N'✅',
                [CreatedAt] datetime2 NOT NULL,
                CONSTRAINT [PK_Habits] PRIMARY KEY ([Id]));

        IF OBJECT_ID(N'[dbo].[HabitCompletions]', N'U') IS NULL
            CREATE TABLE [dbo].[HabitCompletions] (
                [HabitId] int NOT NULL,
                [CompletedOn] date NOT NULL,
                [CompletedAt] datetime2 NOT NULL,
                CONSTRAINT [PK_HabitCompletions] PRIMARY KEY ([HabitId], [CompletedOn]),
                CONSTRAINT [FK_HabitCompletions_Habits_HabitId] FOREIGN KEY ([HabitId]) REFERENCES [dbo].[Habits] ([Id]) ON DELETE CASCADE);
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        IF OBJECT_ID(N'[dbo].[HabitCompletions]', N'U') IS NOT NULL DROP TABLE [dbo].[HabitCompletions];
        IF OBJECT_ID(N'[dbo].[Habits]', N'U') IS NOT NULL DROP TABLE [dbo].[Habits];
        IF COL_LENGTH(N'dbo.Todos', N'ReminderMinutesBefore') IS NOT NULL ALTER TABLE [dbo].[Todos] DROP COLUMN [ReminderMinutesBefore];
        IF COL_LENGTH(N'dbo.Todos', N'RepeatEveryHours') IS NOT NULL ALTER TABLE [dbo].[Todos] DROP COLUMN [RepeatEveryHours];
        """);

    protected override void BuildTargetModel(ModelBuilder modelBuilder) => AppDbContextModelSnapshot.BuildCurrentModel(modelBuilder);
}
