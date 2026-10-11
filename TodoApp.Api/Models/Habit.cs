namespace TodoApp.Api.Models;

public class Habit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = "✅";
    public string? OwnerId { get; set; }
    public AppUser? Owner { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<HabitCompletion> Completions { get; set; } = new List<HabitCompletion>();
}
