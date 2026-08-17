using AspLearn.Data;
using AspLearn.Exceptions;
using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspLearn.Services;

public class TaskService(ITasksRepository repository, IDistributedCache cache, ILogger<TaskService> logger) : ITasksService
{
    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(int userId)
    {
        logger.LogInformation("Пользователь {userId} запросил все задачи.", userId);

        var tasks = await repository.GetAllTasksAsync(userId);

        logger.LogInformation("Пользователь {userId} получил все задачи.", userId);

        return tasks.Select(GetDto);
    } 

    public async Task<TaskDto> GetTaskByIdAsync(int userId, int taskId)
    {
        logger.LogInformation("Пользователь {userId} запросил задачу {taskId}.", userId, taskId);

        var cacheKey = $"user:{userId}:task:{taskId}";

        var cachedJson = await cache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(cachedJson))
        {
            var task = await repository.GetTaskByIdAsync(userId, taskId);

            if (task is null)
            {
                logger.LogWarning("Задача {taskId} пользователя {userId} не найдена.", taskId, userId);
                throw new NotFoundException($"Задача {taskId} пользователя {userId} не найдена.");
            }

            var taskDto = GetDto(task);

            var jsonCache = JsonSerializer.Serialize(taskDto);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await cache.SetStringAsync(cacheKey, jsonCache, cacheOptions);

            logger.LogInformation("Пользователь {userId} получил задачу {taskId} из БД.", userId, taskId);

            return taskDto;
        }

        var cachedDto = JsonSerializer.Deserialize<TaskDto>(cachedJson);

        logger.LogInformation("Пользователь {userId} получил задачу {taskId} из кэша Redis.", userId, taskId);

        return cachedDto;
    }

    public async Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int userId, bool? isCompleted, string? keyword, TaskPriority? priority)
    {
        logger.LogInformation("Пользователь {userId} запросил задачи с фильтром.", userId);

        var filteredTasks = await repository.GetFilteredTasksAsync(userId, isCompleted, keyword, priority);

        logger.LogInformation("Пользователь {userId} получил задачи с фильтром.", userId);

        return filteredTasks.Select(GetDto); 
    }

    public async Task AddTaskAsync(int userId, CreateTaskDto createTask)
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
    }

    public async Task UpdateTaskAsync(int userId, int taskId, UpdateTaskDto newTask)
    {
        logger.LogInformation("Пользователь {userId} начал обновление задачи {taskId}.", userId, taskId);

        var task = await repository.GetTaskByIdAsync(userId, taskId);

        if (task is null)
        {
            logger.LogWarning("Задача {taskId} пользователя {userId} не найдена.", taskId, userId);
            throw new NotFoundException($"Задача {taskId} пользователя {userId} не найдена.");
        }

        task.IsCompleted = newTask.IsCompleted;
        task.Title = newTask.Title;
        task.Description = newTask.Description;

        await repository.UpdateTaskAsync();

        logger.LogInformation("Пользователь {userId} успешно обновил задачу {taskId}.", userId, taskId);

        var cacheKey = $"user:{userId}:task:{taskId}";

        await cache.RemoveAsync(cacheKey);

        logger.LogInformation("Задача {taskId} пользователя {userId} удалена из кэша Reddis", taskId, userId);
    }

    public async Task DeleteTaskAsync(int userId, int taskId)
    {
        logger.LogInformation("Пользователь {userId} начал удаление задачи {taskId}.", userId, taskId);

        var task = await repository.GetTaskByIdAsync(userId, taskId);

        if (task is null)
        {
            logger.LogWarning("Задача {taskId} пользователя {userId} не найдена.", taskId, userId);
            throw new NotFoundException($"Задача {taskId} пользователя {userId} не найдена.");
        }

        await repository.DeleteTaskAsync(task);

        logger.LogInformation("Пользователь {userId} успешно удалил задачу {taskId} из БД.", userId, taskId);

        var cacheKey = $"user:{userId}:task:{taskId}";

        await cache.RemoveAsync(cacheKey);

        logger.LogInformation("Задача {taskId} пользователя {userId} удалена из кэша Reddis", taskId, userId);
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