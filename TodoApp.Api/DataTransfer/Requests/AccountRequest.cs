namespace TodoApp.Api.DataTransfer.Requests;

public class AccountRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
