using AspLearn.Models;
using System.Collections;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public interface ITasksService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync(int userId);
    Task<TaskDto> GetTaskByIdAsync(int userId, int taskId);
    Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(int userId, bool? isCompleted, string? keyword, TaskPriority? priority);
    Task AddTaskAsync(int userId, CreateTaskDto task);
    Task UpdateTaskAsync(int userId, int taskId, UpdateTaskDto newTask);
    Task DeleteTaskAsync(int userId, int taskId);
}