using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspLearn.Controllers;

[Authorize]
[ApiController]
[Route("tasks")]
public class TasksController(ITasksService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTasksAsync()
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        return Ok(await taskService.GetAllTasksAsync((int)userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskAsync([FromRoute] int taskId)
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        var task = await taskService.GetTaskByIdAsync((int)userId, taskId);

        if (task is null) return NotFound();

        return Ok(task);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredTasksAsync([FromQuery] bool? isCompleted, [FromQuery] string? keyword,
        [FromQuery] TaskPriority? priority)
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        return Ok(await taskService.GetFilteredTasksAsync((int)userId, isCompleted, keyword, priority));
    }

    [HttpPost]
    public async Task<IActionResult> AddTaskAsync([FromBody] CreateTaskDto dto)
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        var createdTask = await taskService.AddTaskAsync((int)userId, dto);

        return Ok(createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskAsync([FromRoute] int id, [FromBody] UpdateTaskDto newTask)
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        return this.HandleStatus(await taskService.UpdateTaskAsync((int)userId, id, newTask));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskAsync([FromRoute] int id)
    {
        var userId = GetUserId();

        if (userId is null) return Unauthorized();

        return this.HandleStatus(await taskService.DeleteTaskAsync((int)userId, id));
    }

    [HttpGet("whoami")]
    public IActionResult GetUserName()
    {
        var name = User.Identity?.Name;

        var id = User.Claims.FirstOrDefault(u => u.Type == "Id");

        return Ok($"Текущий пользователь: {name}. Id: {id?.Value}");
    }

    public int? GetUserId()
    {
        var userIdString = User.FindFirst("Id")?.Value;

        if (!int.TryParse(userIdString, out var userId))
        {
            return null;
        }

        return userId;
    }
}