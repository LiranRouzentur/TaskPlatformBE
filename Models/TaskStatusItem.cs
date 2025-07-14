using System.ComponentModel.DataAnnotations;

namespace TaskPlatformBE.Models
{
    public class TaskStatusItem
    {
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsFinal { get; set; }
        public string? Requirement { get; set; }
        public int Id { get; set; }
        public int TaskTypeId { get; set; }
        public string? RequirementDescription { get; set; }
        
        // Navigation property
        public virtual TaskType? TaskType { get; set; }
    }
} 