using AspLearn.Models;
using System.Collections;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public interface ITasksService
{
    IEnumerable<TaskDto> GetAllTasks();
    TaskDto? GetTaskById(int id);
    IEnumerable<TaskDto> GetFilteredTasks(bool? isCompleted, string? keyword);
    TaskDto AddTask(CreateTaskDto task);
    ServiceResult UpdateTask(int id, UpdateTaskDto newTask);
    ServiceResult DeleteTask(int id);
}