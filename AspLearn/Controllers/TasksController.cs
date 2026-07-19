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
    public IActionResult GetTasks() => Ok(taskService.GetAllTasks());

    [HttpGet("{id}")]
    public IActionResult GetTask([FromRoute] int id)
    {
        var task = taskService.GetTaskById(id);

        if (task is null) return NotFound();

        return Ok(task);
    }

    [HttpGet("filter")]
    public IActionResult GetFilteredTasks([FromQuery] bool? isCompleted, [FromQuery] string? keyword, [FromQuery] TaskPriority? priority) => 
        Ok(taskService.GetFilteredTasks(isCompleted, keyword, priority));

    [HttpPost]
    public IActionResult AddTask([FromBody] CreateTaskDto dto)
    {
        var createdTask = taskService.AddTask(dto);

        return Ok(createdTask);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTask([FromRoute] int id, [FromBody] UpdateTaskDto newTask) =>
        HandleStatus(taskService.UpdateTask(id, newTask));

    [HttpDelete("{id}")]
    public IActionResult DeleteTask([FromRoute] int id) => HandleStatus(taskService.DeleteTask(id));

    private IActionResult HandleStatus(ServiceResult result)
    {
        switch (result)
        {
            case ServiceResult.Ok:
                return Ok();
            case ServiceResult.BadRequest:
                return BadRequest();
            case ServiceResult.NotFound:
                return NotFound();
            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }
}