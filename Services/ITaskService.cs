using TaskPlatformBE.Models;

namespace TaskPlatformBE.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync(int? userId = null);
        Task<TaskItem?> GetTaskByIdAsync(int taskId);
        Task<TaskItem> CreateTaskAsync(NewTaskRequest request);
        Task<bool> DeleteTaskAsync(int id);
        Task<TaskValidation> AdvanceTaskAsync(int taskId, string requirement);
        Task<ReverseTaskResponse> ReverseTaskAsync(int taskId);
        Task<TaskValidation> CloseTaskAsync(int taskId);
        Task<IEnumerable<TaskType>> GetTaskTypesAsync();
        Task<List<TaskStatusRequirement>> GetTaskStatusRequirementsAsync(int taskId);
        
        // New workflow validation methods
        Task<TaskValidation> ValidateAdvancementAsync(int typeId, int currentStatus, string requirement);
        Task<TaskValidation> ValidateReversalAsync(int typeId, int currentStatus);
        Task<TaskValidation> ValidateClosureAsync(int typeId, int currentStatus);
        bool IsSequentialAdvancementAsync(int typeId, int currentStatus, int targetStatus);
    }
} 