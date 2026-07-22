using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Repositories;

public interface ITasksRepository
{
    Task<IEnumerable<TodoTask>> GetAllTasksAsync();
    Task<TodoTask?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TodoTask>> GetFilteredTasksAsync(bool? isCompleted, string? keyword, TaskPriority? priority);
    Task AddTaskAsync(TodoTask task);
    Task UpdateTaskAsync();
    Task DeleteTaskAsync(TodoTask task);
}