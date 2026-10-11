using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TodoApp.Api.Data;

namespace TodoApp.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261012000000_AddUserAccounts")]
public sealed class AddUserAccounts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
            EXEC(N'CREATE TABLE [dbo].[Users] (
                [Id] nvarchar(32) NOT NULL,
                [Email] nvarchar(256) NOT NULL,
                [NormalizedEmail] nvarchar(256) NOT NULL,
                [PasswordHash] nvarchar(256) NOT NULL,
                [CreatedAt] datetime2 NOT NULL,
                CONSTRAINT [PK_Users] PRIMARY KEY ([Id]));');

        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Users_NormalizedEmail' AND [object_id] = OBJECT_ID(N'[dbo].[Users]'))
            EXEC(N'CREATE UNIQUE INDEX [IX_Users_NormalizedEmail] ON [dbo].[Users] ([NormalizedEmail]);');

        IF COL_LENGTH(N'dbo.Habits', N'OwnerId') IS NULL
            EXEC(N'ALTER TABLE [dbo].[Habits] ADD [OwnerId] nvarchar(32) NULL;');

        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Habits_OwnerId' AND [object_id] = OBJECT_ID(N'[dbo].[Habits]'))
            EXEC(N'CREATE INDEX [IX_Habits_OwnerId] ON [dbo].[Habits] ([OwnerId]);');

        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_Habits_Users_OwnerId')
            EXEC(N'ALTER TABLE [dbo].[Habits] ADD CONSTRAINT [FK_Habits_Users_OwnerId]
                FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE;');

        EXEC sys.sp_executesql N'
            UPDATE [dbo].[Habits]
                SET [OwnerId] = (SELECT TOP (1) [Id] FROM [dbo].[Users] ORDER BY [CreatedAt], [Id])
                WHERE [OwnerId] IS NULL AND EXISTS (SELECT 1 FROM [dbo].[Users]);';
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_Habits_Users_OwnerId')
            ALTER TABLE [dbo].[Habits] DROP CONSTRAINT [FK_Habits_Users_OwnerId];
        IF EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Habits_OwnerId' AND [object_id] = OBJECT_ID(N'[dbo].[Habits]'))
            DROP INDEX [IX_Habits_OwnerId] ON [dbo].[Habits];
        IF COL_LENGTH(N'dbo.Habits', N'OwnerId') IS NOT NULL ALTER TABLE [dbo].[Habits] DROP COLUMN [OwnerId];
        IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NOT NULL DROP TABLE [dbo].[Users];
        """);

    protected override void BuildTargetModel(ModelBuilder modelBuilder) => AppDbContextModelSnapshot.BuildCurrentModel(modelBuilder);
}
