using AspLearn.Data;
using AspLearn.Models;
using AspLearn.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AspLearn.Services;

public class TaskService(AppDbContext dbContext) : ITasksService
{
    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await dbContext.Tasks.ToListAsync();

        return tasks.Select(GetDto);
    } 

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
       var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

       if (task is null) return null;

       return GetDto(task);
    }

    public async Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(bool? isCompleted, string? keyword, TaskPriority? priority)
    {
        var filteredTasks = dbContext.Tasks.AsQueryable();

        if (isCompleted.HasValue)
        {
            filteredTasks = filteredTasks.Where(t => t.IsCompleted == isCompleted);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            filteredTasks = filteredTasks.Where(t =>
                t.Title.Contains(keyword) ||
                (t.Description != null && t.Description.Contains(keyword))
            );
        }

        if (priority.HasValue)
        {
            filteredTasks = filteredTasks.Where(t => t.Priority == priority);
        }

        var tasks = await filteredTasks.ToListAsync();

        return tasks.Select(GetDto); 
    }

    public async Task<TaskDto> AddTaskAsync(CreateTaskDto createTask)
    {
        var task = new TodoTask()
        {
            IsCompleted = false,
            Title = createTask.Title,
            Description = createTask.Description,
            Priority = createTask.Priority
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();

        return GetDto(task);
    }

    public async Task<ServiceResult> UpdateTaskAsync(int id, UpdateTaskDto newTask)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;
        if (newTask is null) return ServiceResult.BadRequest;

        task.IsCompleted = newTask.IsCompleted;
        task.Title = newTask.Title;
        task.Description = newTask.Description;

        await dbContext.SaveChangesAsync();

        return ServiceResult.Ok;
    }

    public async Task<ServiceResult> DeleteTaskAsync(int id)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync();

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