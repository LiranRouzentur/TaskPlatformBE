using System.ComponentModel.DataAnnotations;

namespace TaskPlatformBE.Models;

/// <summary>
/// View model for the main tasks page
/// </summary>
public class TasksViewModel
{
    public List<TaskDto> Tasks { get; set; } = new();
    public List<TaskTypeDto> TaskTypes { get; set; } = new();
    public List<UserDto> Users { get; set; } = new();
    public int? SelectedUserId { get; set; }
}

/// <summary>
/// DTO for task data to avoid circular references
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public int UserId { get; set; }
    public int Status { get; set; }
    public int? NextAssignedUserId { get; set; }
    public string Requirement { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CustomFields { get; set; }
    public TaskTypeDto? TaskType { get; set; }
    public UserDto? User { get; set; }
    public UserDto? NextAssignedUser { get; set; }
}

/// <summary>
/// DTO for task type data
/// </summary>
public class TaskTypeDto
{
    public int TypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaskStatusItemDto> Statuses { get; set; } = new();
}

/// <summary>
/// DTO for task status item data
/// </summary>
public class TaskStatusItemDto
{
    public int Id { get; set; }
    public int TaskTypeId { get; set; }
    public int StatusId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsFinal { get; set; }
    public string? Requirement { get; set; }
    public string? RequirementDescription { get; set; }
} 