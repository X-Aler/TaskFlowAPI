using System.ComponentModel.DataAnnotations;

namespace AspLearn.Models.DTOs;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Длинна поля {0} должна быть от {2} до {1} символов.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, MinimumLength = 10, ErrorMessage = "Длинна поля {0} должна быть от {2} до {1} символов.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Поле {0} обязательно для заполнения")]
    public TaskPriority Priority { get; set; }
}