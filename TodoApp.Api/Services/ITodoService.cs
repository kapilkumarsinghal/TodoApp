using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;

namespace TodoApp.Api.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoResponse>> GetAllAsync(string userId);
    Task<TodoResponse?> GetByIdAsync(string userId, int id);
    Task<TodoResponse> CreateAsync(string userId, TodoCreateRequest request);
    Task<TodoResponse?> UpdateAsync(string userId, TodoUpdateRequest request);
    Task<bool> DeleteAsync(string userId, int id);
}
