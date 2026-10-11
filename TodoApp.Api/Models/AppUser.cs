namespace TodoApp.Api.Models;

public class AppUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Habit> Habits { get; set; } = new List<Habit>();
    public ICollection<TodoItem> Todos { get; set; } = new List<TodoItem>();
}
