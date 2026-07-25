using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Repositories;

public interface ITasksRepository
{
    Task<IEnumerable<TodoTask>> GetAllTasksAsync(int userId);
    Task<TodoTask?> GetTaskByIdAsync(int userId, int taskId);
    Task<IEnumerable<TodoTask>> GetFilteredTasksAsync(int userId, bool? isCompleted, string? keyword, TaskPriority? priority);
    Task AddTaskAsync(TodoTask task);
    Task UpdateTaskAsync();
    Task DeleteTaskAsync(TodoTask task);
}