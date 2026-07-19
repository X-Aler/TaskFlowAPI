using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public class TaskService : ITasksService
{
    private List<TodoTask> tasks = new List<TodoTask>();
    private int id;

    public IEnumerable<TaskDto> GetAllTasks() => tasks.Select(GetDto);

    public TaskDto? GetTaskById(int id)
    {
       var task = tasks.FirstOrDefault(t => t.Id == id);

       if (task is null) return null;

       return GetDto(task);
    }

    public IEnumerable<TaskDto> GetFilteredTasks(bool? isCompleted, string? keyword, TaskPriority? priority)
    {
        var filteredTasks = tasks.AsEnumerable();

        if (isCompleted.HasValue)
        {
            filteredTasks = filteredTasks.Where(t => t.IsCompleted == isCompleted);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            filteredTasks = filteredTasks.Where(t => t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                                     || (t.Description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (priority.HasValue)
        {
            filteredTasks = filteredTasks.Where(t => t.Priority == priority);
        }

        return filteredTasks.Select(GetDto); 
    }

    public TaskDto AddTask(CreateTaskDto createTask)
    {
        var task = new TodoTask()
        {
            Id = ++id,
            IsCompleted = false,
            Title = createTask.Title,
            Description = createTask.Description
        };

        tasks.Add(task);

        return GetDto(task);
    }

    public ServiceResult UpdateTask(int id, UpdateTaskDto newTask)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;
        if (newTask is null) return ServiceResult.BadRequest;

        task.IsCompleted = newTask.IsCompleted;
        task.Title = newTask.Title;
        task.Description = newTask.Description;

        return ServiceResult.Ok;
    }

    public ServiceResult DeleteTask(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null) return ServiceResult.NotFound;

        tasks.Remove(task);

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