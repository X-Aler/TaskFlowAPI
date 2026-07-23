using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspLearn.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController(ITasksService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTasksAsync() => Ok(await taskService.GetAllTasksAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskAsync([FromRoute] int id)
    {
        var task = await taskService.GetTaskByIdAsync(id);

        if (task is null) return NotFound();

        return Ok(task);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredTasksAsync([FromQuery] bool? isCompleted, [FromQuery] string? keyword, [FromQuery] TaskPriority? priority) => 
        Ok(await taskService.GetFilteredTasksAsync(isCompleted, keyword, priority));

    [HttpPost]
    public async Task<IActionResult> AddTaskAsync([FromBody] CreateTaskDto dto)
    {
        var createdTask = await taskService.AddTaskAsync(dto);

        return Ok(createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskAsync([FromRoute] int id, [FromBody] UpdateTaskDto newTask) =>
        this.HandleStatus(await taskService.UpdateTaskAsync(id, newTask));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskAsync([FromRoute] int id) => this.HandleStatus(await taskService.DeleteTaskAsync(id));

}