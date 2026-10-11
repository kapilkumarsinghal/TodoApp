using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;
using TodoApp.Api.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoResponse>>> GetAll()
    {
        var items = await _todoService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoResponse>> GetById(int id)
    {
        var item = await _todoService.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> Create([FromBody] TodoCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required.");
        }

        if (request.StartAt.HasValue != request.EndAt.HasValue
            || (request.StartAt.HasValue && request.EndAt <= request.StartAt))
        {
            return BadRequest("End time must be after start time.");
        }

        if (!ValidReminder(request.ReminderMinutesBefore) || !ValidRepeat(request.RepeatEveryHours))
            return BadRequest("Reminder must be 5, 10, 15, or 30 minutes; repeat must be every 1 or 2 hours.");

        var item = await _todoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoResponse>> Update(int id, [FromBody] TodoUpdateRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required.");
        }

        if (request.StartAt.HasValue != request.EndAt.HasValue
            || (request.StartAt.HasValue && request.EndAt <= request.StartAt))
        {
            return BadRequest("End time must be after start time.");
        }

        if (!ValidReminder(request.ReminderMinutesBefore) || !ValidRepeat(request.RepeatEveryHours))
            return BadRequest("Reminder must be 5, 10, 15, or 30 minutes; repeat must be every 1 or 2 hours.");

        var item = await _todoService.UpdateAsync(request);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _todoService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private static bool ValidReminder(int? value) => value is null or 5 or 10 or 15 or 30;
    private static bool ValidRepeat(int? value) => value is null or 1 or 2;
}
