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

        return Ok(await taskService.GetAllTasksAsync(userId));
    }

    [HttpGet("{taskId:int}")]
    public async Task<IActionResult> GetTaskAsync([FromRoute] int taskId)
    {
        var userId = GetUserId();

        var task = await taskService.GetTaskByIdAsync(userId, taskId);

        return Ok(task);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredTasksAsync([FromQuery] bool? isCompleted, [FromQuery] string? keyword,
        [FromQuery] TaskPriority? priority)
    {
        var userId = GetUserId();

        return Ok(await taskService.GetFilteredTasksAsync(userId, isCompleted, keyword, priority));
    }

    [HttpPost]
    public async Task<IActionResult> AddTaskAsync([FromBody] CreateTaskDto dto)
    {
        var userId = GetUserId();

        await taskService.AddTaskAsync(userId, dto);

        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTaskAsync([FromRoute] int id, [FromBody] UpdateTaskDto newTask)
    {
        var userId = GetUserId();

        await taskService.UpdateTaskAsync(userId, id, newTask);

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTaskAsync([FromRoute] int id)
    {
        var userId = GetUserId();

        await taskService.DeleteTaskAsync(userId, id);

        return Ok();
    }

    [HttpGet("whoami")]
    public IActionResult GetUserName()
    {
        var name = User.Identity?.Name;

        var id = User.Claims.FirstOrDefault(u => u.Type == "Id");

        return Ok($"Текущий пользователь: {name}. Id: {id?.Value}");
    }

    private int GetUserId()
    {
        var userIdString = User.FindFirst("Id")?.Value;

        return int.Parse(userIdString!);
    }
}