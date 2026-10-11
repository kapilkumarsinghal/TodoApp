using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;
using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _context;

    public TodoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TodoResponse>> GetAllAsync()
    {
        var items = await _context.Todos
            .AsNoTracking()
            .OrderByDescending(todo => todo.CreatedAt)
            .ToListAsync();

        return items.Select(ToResponse).ToList();
    }

    public async Task<TodoResponse?> GetByIdAsync(int id)
    {
        var item = await _context.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(todo => todo.Id == id);

        return item is null ? null : ToResponse(item);
    }

    public async Task<TodoResponse> CreateAsync(TodoCreateRequest request)
    {
        var item = new TodoItem
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            IsCompleted = false,
            DueDate = request.DueDate,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            ReminderMinutesBefore = request.ReminderMinutesBefore,
            RepeatEveryHours = request.RepeatEveryHours,
            Category = NormalizeCategory(request.Category),
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow
        };

        _context.Todos.Add(item);
        await _context.SaveChangesAsync();

        return ToResponse(item);
    }

    public async Task<TodoResponse?> UpdateAsync(TodoUpdateRequest request)
    {
        var item = await _context.Todos.FindAsync(request.Id);
        if (item is null)
        {
            return null;
        }

        item.Title = request.Title.Trim();
        item.Description = request.Description?.Trim();
        item.IsCompleted = request.IsCompleted;
        item.DueDate = request.DueDate;
        item.StartAt = request.StartAt;
        item.EndAt = request.EndAt;
        item.ReminderMinutesBefore = request.ReminderMinutesBefore;
        item.RepeatEveryHours = request.RepeatEveryHours;
        item.Category = NormalizeCategory(request.Category);
        item.Priority = request.Priority;

        await _context.SaveChangesAsync();
        return ToResponse(item);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Todos.FindAsync(id);
        if (item is null)
        {
            return false;
        }

        _context.Todos.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TodoResponse ToResponse(TodoItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Description = item.Description,
        IsCompleted = item.IsCompleted,
        DueDate = item.DueDate,
        StartAt = item.StartAt,
        EndAt = item.EndAt,
        ReminderMinutesBefore = item.ReminderMinutesBefore,
        RepeatEveryHours = item.RepeatEveryHours,
        Category = item.Category,
        Priority = item.Priority,
        CreatedAt = item.CreatedAt
    };

    private static string NormalizeCategory(string? category) =>
        string.IsNullOrWhiteSpace(category) ? "Personal" : category.Trim();
}
