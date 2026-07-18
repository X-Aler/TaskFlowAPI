using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public class TaskService : ITasksService
{
    private List<TodoTask> tasks = new List<TodoTask>();
    private int id;

    public IEnumerable<TaskDto> GetAllTasks() => tasks.Select(t => new TaskDto
    {
        Id = t.Id,
        IsCompleted = t.IsCompleted,
        Title = t.Title
    });

    public TaskDto? GetTaskById(int id)
    {
       var task = tasks.FirstOrDefault(t => t.Id == id);

       if (task is null) return null;

       var dto = new TaskDto
       {
           Id = task.Id,
           IsCompleted = task.IsCompleted,
           Title = task.Title
       };

       return dto;
    }

    public IEnumerable<TaskDto> GetFilteredTasks(bool? isCompleted, string? keyword)
    {
        var filteredTasks = tasks.AsEnumerable();

        if (isCompleted.HasValue)
        {
            filteredTasks = filteredTasks.Where(t => t.IsCompleted == isCompleted);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            filteredTasks = filteredTasks.Where(t => t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return filteredTasks.Select(t => new TaskDto
        {
            Id = t.Id,
            IsCompleted = t.IsCompleted,
            Title = t.Title
        }); ;
    }

    public TaskDto AddTask(CreateTaskDto createTask)
    {
        var task = new TodoTask()
        {
            Id = ++id,
            IsCompleted = false,
            Title = createTask.Title
        };

        tasks.Add(task);

        var dto = new TaskDto
        {
            Id = task.Id,
            IsCompleted = task.IsCompleted,
            Title = task.Title
        };

        return dto;
    }

    public ServiceResult UpdateTask(int id, UpdateTaskDto newTask)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;
        if (newTask is null) return ServiceResult.BadRequest;

        task.IsCompleted = newTask.IsCompleted;
        task.Title = newTask.Title;

        return ServiceResult.Ok;
    }

    public ServiceResult DeleteTask(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;

        tasks.Remove(task);

        return ServiceResult.Ok;
    }
}