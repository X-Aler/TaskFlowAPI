using AspLearn.Data;
using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AspLearn.Services;

public class TaskService(ITasksRepository repository) : ITasksService
{
    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(int userId)
    {
        var tasks = await repository.GetAllTasksAsync(userId);

        return tasks.Select(GetDto);
    } 

    public async Task<TaskDto?> GetTaskByIdAsync(int userId, int taskId)
    {
       var task = await repository.GetTaskByIdAsync(userId, taskId);

       return task is null ? null : GetDto(task);
    }

    public async Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int userId, bool? isCompleted, string? keyword, TaskPriority? priority)
    {
        var filteredTasks = await repository.GetFilteredTasksAsync(userId, isCompleted, keyword, priority);

        return filteredTasks.Select(GetDto); 
    }

    public async Task<TaskDto> AddTaskAsync(int userId, CreateTaskDto createTask)
    {
        var task = new TodoTask()
        {
            IsCompleted = false,
            Title = createTask.Title,
            Description = createTask.Description,
            Priority = createTask.Priority,
            UserId = userId
        };

        await repository.AddTaskAsync(task);

        return GetDto(task);
    }

    public async Task<ServiceResult> UpdateTaskAsync(int userId, int taskId, UpdateTaskDto newTask)
    {
        var task = await repository.GetTaskByIdAsync(userId, taskId);

        if (task is null) return ServiceResult.NotFound;
        if (newTask is null) return ServiceResult.BadRequest;

        task.IsCompleted = newTask.IsCompleted;
        task.Title = newTask.Title;
        task.Description = newTask.Description;

        await repository.UpdateTaskAsync();

        return ServiceResult.Ok;
    }

    public async Task<ServiceResult> DeleteTaskAsync(int userId, int taskId)
    {
        var task = await repository.GetTaskByIdAsync(userId, taskId);

        if (task is null) return ServiceResult.NotFound;

        await repository.DeleteTaskAsync(task);

        return ServiceResult.Ok;
    }

    private TaskDto GetDto(TodoTask task) => new TaskDto
    {
        Id = task.Id,
        IsCompleted = task.IsCompleted,
        Title = task.Title,
        Description = task.Description,
        Priority = task.Priority
    };
}