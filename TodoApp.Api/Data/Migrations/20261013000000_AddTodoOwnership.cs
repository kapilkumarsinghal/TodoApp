using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TodoApp.Api.Data;

namespace TodoApp.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261013000000_AddTodoOwnership")]
public sealed class AddTodoOwnership : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        IF COL_LENGTH(N'dbo.Todos', N'OwnerId') IS NULL
            EXEC(N'ALTER TABLE [dbo].[Todos] ADD [OwnerId] nvarchar(32) NULL;');

        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Todos_OwnerId' AND [object_id] = OBJECT_ID(N'[dbo].[Todos]'))
            EXEC(N'CREATE INDEX [IX_Todos_OwnerId] ON [dbo].[Todos] ([OwnerId]);');

        IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_Todos_Users_OwnerId')
            EXEC(N'ALTER TABLE [dbo].[Todos] ADD CONSTRAINT [FK_Todos_Users_OwnerId]
                FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE;');

        EXEC sys.sp_executesql N'
            UPDATE [dbo].[Todos]
                SET [OwnerId] = (SELECT TOP (1) [Id] FROM [dbo].[Users] ORDER BY [CreatedAt], [Id])
                WHERE [OwnerId] IS NULL AND EXISTS (SELECT 1 FROM [dbo].[Users]);';
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = N'FK_Todos_Users_OwnerId')
            ALTER TABLE [dbo].[Todos] DROP CONSTRAINT [FK_Todos_Users_OwnerId];
        IF EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Todos_OwnerId' AND [object_id] = OBJECT_ID(N'[dbo].[Todos]'))
            DROP INDEX [IX_Todos_OwnerId] ON [dbo].[Todos];
        IF COL_LENGTH(N'dbo.Todos', N'OwnerId') IS NOT NULL ALTER TABLE [dbo].[Todos] DROP COLUMN [OwnerId];
        """);

    protected override void BuildTargetModel(ModelBuilder modelBuilder) => AppDbContextModelSnapshot.BuildCurrentModel(modelBuilder);
}
