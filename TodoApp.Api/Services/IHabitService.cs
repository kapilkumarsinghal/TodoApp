using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;

namespace TodoApp.Api.Services;

public interface IHabitService
{
    Task<IReadOnlyList<HabitResponse>> GetForMonthAsync(int year, int month);
    Task<HabitResponse> CreateAsync(HabitCreateRequest request);
    Task<bool> SetCompletionAsync(int id, HabitCompletionRequest request);
    Task<bool> DeleteAsync(int id);
}
