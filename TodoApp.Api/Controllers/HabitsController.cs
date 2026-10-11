using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DataTransfer.Requests;
using TodoApp.Api.DataTransfer.Responses;
using TodoApp.Api.Services;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly IHabitService _service;
    public HabitsController(IHabitService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HabitResponse>>> Get([FromQuery] int year, [FromQuery] int month)
    {
        if (year is < 2000 or > 2200 || month is < 1 or > 12) return BadRequest("Provide a valid year and month.");
        return Ok(await _service.GetForMonthAsync(year, month));
    }

    [HttpPost]
    public async Task<ActionResult<HabitResponse>> Create([FromBody] HabitCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Habit name is required.");
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { year = DateTime.UtcNow.Year, month = DateTime.UtcNow.Month }, result);
    }

    [HttpPut("{id:int}/completion")]
    public async Task<IActionResult> SetCompletion(int id, [FromBody] HabitCompletionRequest request)
    {
        var exists = await _service.SetCompletionAsync(id, request);
        return exists ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
