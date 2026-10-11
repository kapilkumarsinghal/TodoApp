namespace TodoApp.Api.Models;

public class HabitCompletion
{
    public int HabitId { get; set; }
    public Habit Habit { get; set; } = null!;
    public DateOnly CompletedOn { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
