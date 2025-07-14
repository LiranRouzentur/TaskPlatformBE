using System.ComponentModel.DataAnnotations;

namespace TaskPlatformBE.Models
{
    public class TaskType
    {
        public int TypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // Navigation property for statuses
        public virtual ICollection<TaskStatusItem> Statuses { get; set; } = new List<TaskStatusItem>();
    }
} 