using AspLearn.Data;
using AspLearn.Models;
using Microsoft.EntityFrameworkCore;

namespace AspLearn.Repositories;

public class TasksRepository(AppDbContext dbContext) : ITasksRepository
{
    public async Task<IEnumerable<TodoTask>> GetAllTasksAsync(int userId) => await dbContext.Tasks.Where(t => t.UserId == userId).ToListAsync();
    public async Task<TodoTask?> GetTaskByIdAsync(int userId, int taskId) => await dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
    public async Task<IEnumerable<TodoTask>> GetFilteredTasksAsync(int userId, bool? isCompleted, string? keyword, TaskPriority? priority)
    {
        var filteredTasks = dbContext.Tasks.Where(t => t.UserId == userId).AsQueryable();

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

        return await filteredTasks.ToListAsync();
    }
    public async Task AddTaskAsync(TodoTask task)
    {
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync();
    }
    public async Task UpdateTaskAsync() => await dbContext.SaveChangesAsync();
    public async Task DeleteTaskAsync(TodoTask task)
    {
        dbContext.Tasks.Remove(task);

        await dbContext.SaveChangesAsync();
    }
}