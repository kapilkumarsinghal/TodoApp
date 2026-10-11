using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;

namespace TodoApp.Api.Services;

public interface IAccountService
{
    Task<AccountResponse?> RegisterAsync(AccountRequest request);
    Task<AccountResponse?> LoginAsync(AccountRequest request);
}
