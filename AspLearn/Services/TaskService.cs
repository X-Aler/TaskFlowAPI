using AspLearn.Data;
using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public class TaskService(AppDbContext dbContext) : ITasksService
{
    public IEnumerable<TaskDto> GetAllTasks() => dbContext.Tasks.Select(GetDto).ToList();

    public TaskDto? GetTaskById(int id)
    {
       var task = dbContext.Tasks.FirstOrDefault(t => t.Id == id);

       if (task is null) return null;

       return GetDto(task);
    }

    public IEnumerable<TaskDto> GetFilteredTasks(bool? isCompleted, string? keyword, TaskPriority? priority)
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

        return filteredTasks.Select(GetDto).ToList(); 
    }

    public TaskDto AddTask(CreateTaskDto createTask)
    {
        var task = new TodoTask()
        {
            IsCompleted = false,
            Title = createTask.Title,
            Description = createTask.Description,
            Priority = createTask.Priority
        };

        dbContext.Tasks.Add(task);
        dbContext.SaveChanges();

        return GetDto(task);
    }

    public ServiceResult UpdateTask(int id, UpdateTaskDto newTask)
    {
        var task = dbContext.Tasks.FirstOrDefault(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;
        if (newTask is null) return ServiceResult.BadRequest;

        task.IsCompleted = newTask.IsCompleted;
        task.Title = newTask.Title;
        task.Description = newTask.Description;
        dbContext.SaveChanges();

        return ServiceResult.Ok;
    }

    public ServiceResult DeleteTask(int id)
    {
        var task = dbContext.Tasks.FirstOrDefault(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;

        dbContext.Tasks.Remove(task);
        dbContext.SaveChanges();

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