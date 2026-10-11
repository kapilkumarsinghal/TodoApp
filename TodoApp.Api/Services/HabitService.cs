using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;
using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public class HabitService : IHabitService
{
    private readonly AppDbContext _context;
    public HabitService(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<HabitResponse>> GetForMonthAsync(int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1);
        var habits = await _context.Habits.AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.Name, x.Emoji, x.CreatedAt,
                Days = x.Completions.Where(c => c.CompletedOn >= start && c.CompletedOn < end)
                    .Select(c => c.CompletedOn.Day).ToArray() })
            .ToListAsync();
        return habits.Select(x => new HabitResponse { Id = x.Id, Name = x.Name, Emoji = x.Emoji,
            CreatedAt = x.CreatedAt, CompletedDays = x.Days }).ToList();
    }

    public async Task<HabitResponse> CreateAsync(HabitCreateRequest request)
    {
        var habit = new Habit { Name = request.Name.Trim(), Emoji = string.IsNullOrWhiteSpace(request.Emoji) ? "✅" : request.Emoji.Trim() };
        _context.Habits.Add(habit);
        await _context.SaveChangesAsync();
        return new HabitResponse { Id = habit.Id, Name = habit.Name, Emoji = habit.Emoji, CreatedAt = habit.CreatedAt };
    }

    public async Task<bool> SetCompletionAsync(int id, HabitCompletionRequest request)
    {
        var completion = await _context.HabitCompletions.FindAsync(id, request.Date);
        if (request.Completed && completion is null)
        {
            if (!await _context.Habits.AnyAsync(x => x.Id == id)) return false;
            _context.HabitCompletions.Add(new HabitCompletion { HabitId = id, CompletedOn = request.Date });
        }
        else if (!request.Completed && completion is not null) _context.HabitCompletions.Remove(completion);
        else if (request.Completed && completion is not null) return true;
        else if (!await _context.Habits.AnyAsync(x => x.Id == id)) return false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var habit = await _context.Habits.FindAsync(id);
        if (habit is null) return false;
        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync();
        return true;
    }
}
