namespace AspLearn.Models;

public class TodoTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}