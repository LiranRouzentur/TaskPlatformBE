using System.ComponentModel.DataAnnotations;

namespace TaskPlatformBE.Models
{
    public class CreateTaskRequest
    {
        [Required]
        public int TypeId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public int? NextAssignedUserId { get; set; }
        
        public string Requirement { get; set; } = string.Empty;
    }

    public class UpdateTaskRequest
    {
        public int? Status { get; set; }
        public int? NextAssignedUserId { get; set; }
        public string? Requirement { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for advancing a task via MVC
    /// </summary>
    public class AdvanceTaskRequest
    {
        public int TaskId { get; set; }
        public string? Requirement { get; set; }
    }

    /// <summary>
    /// Request model for task actions (reverse, close, delete) via MVC
    /// </summary>
    public class TaskActionRequest
    {
        public int TaskId { get; set; }
    }

    /// <summary>
    /// Response model for task operations
    /// </summary>
    public class TaskOperationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public TaskDto? Task { get; set; }
        public string? ReversedTaskId { get; set; }
        public string? ReversedRequirement { get; set; }
    }

    /// <summary>
    /// Response model for reverse operations that includes saved requirement value
    /// </summary>
    public class ReverseTaskResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? SavedRequirement { get; set; }
    }
} 