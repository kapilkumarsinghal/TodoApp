using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;

namespace TodoApp.Api.Services;

public interface IHabitService
{
    Task<IReadOnlyList<HabitResponse>> GetForMonthAsync(string userId, int year, int month);
    Task<HabitResponse> CreateAsync(string userId, HabitCreateRequest request);
    Task<bool> SetCompletionAsync(string userId, int id, HabitCompletionRequest request);
    Task<bool> DeleteAsync(string userId, int id);
}
