namespace AspLearn.Models.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
}