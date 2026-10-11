namespace TodoApp.Api.DataTransfer.Requests;

public class HabitCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = "✅";
}
