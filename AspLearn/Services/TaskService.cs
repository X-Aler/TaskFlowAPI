using AspLearn.Data;
using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AspLearn.Services;

public class TaskService(ITasksRepository repository, ILogger<TaskService> logger) : ITasksService
{
    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(int userId)
    {
        logger.LogInformation("Пользователь {userId} запросил все задачи.", userId);

        var tasks = await repository.GetAllTasksAsync(userId);

        logger.LogInformation("Пользователь {userId} получил все задачи.", userId);

        return tasks.Select(GetDto);
    } 

    public async Task<TaskDto?> GetTaskByIdAsync(int userId, int taskId)
    {
        logger.LogInformation("Пользователь {userId} запросил задачу {taskId}.", userId, taskId); 

        var task = await repository.GetTaskByIdAsync(userId, taskId);

        if (task is null)
        {
            logger.LogWarning("Задача {taskId} пользователя {userId} не найдена.", taskId, userId);
            return null;
        }

        logger.LogInformation("Пользователь {userId} получил задачу {taskId}.", userId, taskId);

        return GetDto(task);
    }

    public async Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int userId, bool? isCompleted, string? keyword, TaskPriority? priority)
    {
        logger.LogInformation("Пользователь {userId} запросил задачи с фильтром.", userId);

        var filteredTasks = await repository.GetFilteredTasksAsync(userId, isCompleted, keyword, priority);

        logger.LogInformation("Пользователь {userId} получил задачи с фильтром.", userId);

        return filteredTasks.Select(GetDto); 
    }

    public async Task<TaskDto> AddTaskAsync(int userId, CreateTaskDto createTask)
    {
        logger.LogInformation("Пользователь {userId} начал добавление задачи {Title}.", userId, createTask.Title);

        var task = new TodoTask()
        {
            IsCompleted = false,
            Title = createTask.Title,
            Description = createTask.Description,
            Priority = createTask.Priority,
            UserId = userId
        };

        await repository.AddTaskAsync(task);

        logger.LogInformation("Пользователь {userId} успешно добавил задачу {Title}.", userId, createTask.Title);

        return GetDto(task);
    }

    public async Task<ServiceResult> UpdateTaskAsync(int userId, int taskId, UpdateTaskDto newTask)
    {
        logger.LogInformation("Пользователь {userId} начал обновление задачи {taskId}.", userId, taskId);

        var task = await repository.GetTaskByIdAsync(userId, taskId);

        if (task is null)
        {
            logger.LogWarning("Задача {taskId} пользователя {userId} не найдена.", taskId, userId);
            return ServiceResult.NotFound;
        }

        task.IsCompleted = newTask.IsCompleted;
        task.Title = newTask.Title;
        task.Description = newTask.Description;

        await repository.UpdateTaskAsync();

        logger.LogInformation("Пользователь {userId} успешно обновил задачу {taskId}.", userId, taskId);

        return ServiceResult.Ok;
    }

    public async Task<ServiceResult> DeleteTaskAsync(int userId, int taskId)
    {
        logger.LogInformation("Пользователь {userId} начал удаление задачи {taskId}.", userId, taskId);

        var task = await repository.GetTaskByIdAsync(userId, taskId);

        if (task is null)
        {
            logger.LogWarning("Задача {taskId} пользователя {userId} не найдена.", taskId, userId);
            return ServiceResult.NotFound;
        }

        await repository.DeleteTaskAsync(task);

        logger.LogInformation("Пользователь {userId} успешно удалил задачу {taskId}.", userId, taskId);

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