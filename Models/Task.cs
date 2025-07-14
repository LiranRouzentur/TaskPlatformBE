using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskPlatformBE.Models;

public class TaskItem
{
    public int Id { get; set; }
    
    [Required]
    public int TypeId { get; set; }

    [Required]
    public int UserId { get; set; }

    public int Status { get; set; } = 1;

    public int? NextAssignedUserId { get; set; }

    [StringLength(500)]
    public string Requirement { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // JSON field for custom data based on task type
    [Column(TypeName = "nvarchar(max)")]
    public string? CustomFields { get; set; }
    
    // Navigation properties
    public virtual TaskType? TaskType { get; set; }
    public virtual User? User { get; set; }
    public virtual User? NextAssignedUser { get; set; }
}

public class TaskStatusRequirement
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string Requirement { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int StatusId { get; set; }
    public string? RequirementValue { get; set; }
    
    // Navigation property
    public virtual TaskItem? Task { get; set; }
}

public class TaskValidation
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class NewTaskRequest
{
    [Required]
    public int TypeId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    public int? NextAssignedUserId { get; set; }
    
    public string Requirement { get; set; } = string.Empty;
}

 
