using AspLearn.Models;
using System.Collections;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public interface ITasksService
{
    Task<IEnumerable<TaskDto>> GetAllTasksAsync();
    Task<TaskDto?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TaskDto>> GetFilteredTasksAsync(bool? isCompleted, string? keyword, TaskPriority? priority);
    Task<TaskDto> AddTaskAsync(CreateTaskDto task);
    Task<ServiceResult> UpdateTaskAsync(int id, UpdateTaskDto newTask);
    Task<ServiceResult> DeleteTaskAsync(int id);
}