using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;

namespace TodoApp.Api.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoResponse>> GetAllAsync();
    Task<TodoResponse?> GetByIdAsync(int id);
    Task<TodoResponse> CreateAsync(TodoCreateRequest request);
    Task<TodoResponse?> UpdateAsync(TodoUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
