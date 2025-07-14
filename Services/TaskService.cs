using Microsoft.EntityFrameworkCore;
using TaskPlatformBE.Data;
using TaskPlatformBE.Models;

namespace TaskPlatformBE.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ApplicationDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Retrieves all tasks with navigation properties, ordered by creation date
    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(int? userId = null)
    {
        IQueryable<TaskItem> query = _context.Tasks
            .Include(t => t.TaskType)
            .Include(t => t.User)
            .Include(t => t.NextAssignedUser);
            
        if (userId.HasValue)
        {
            query = query.Where(t => t.UserId == userId.Value);
        }
        
        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    // Retrieves a specific task by ID with navigation properties
    public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
    {
        return await _context.Tasks
            .Include(t => t.TaskType)
                .ThenInclude(tt => tt.Statuses)
            .Include(t => t.User)
            .Include(t => t.NextAssignedUser)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    // Creates a new task with initial status and random next user assignment
    public async Task<TaskItem> CreateTaskAsync(NewTaskRequest request)
    {
        // Get the task type to determine the initial status
        var taskType = await GetTaskTypeAsync(request.TypeId);
        if (taskType == null)
            throw new InvalidOperationException($"Task type {request.TypeId} not found");

        var initialStatus = taskType.Statuses?.Min(s => s.StatusId) ?? 1;
        
        // Ensure next assigned user is different from assigned user
        var nextAssignedUserId = request.NextAssignedUserId;
        
        // If next assigned user is the same as assigned user, or not provided, assign a different random user
        if (!nextAssignedUserId.HasValue || nextAssignedUserId.Value == request.UserId)
        {
            var randomUser = await _context.Users
                .Where(u => u.Id != request.UserId) // Exclude the assigned user
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync();
            
            nextAssignedUserId = randomUser?.Id;
            
            // If no other users exist, we can't create the task
            if (!nextAssignedUserId.HasValue)
            {
                throw new InvalidOperationException("Cannot create task: no other users available for next assignment");
            }
        }
        
        var task = new TaskItem
        {
            TypeId = request.TypeId,
            UserId = request.UserId,
            Status = initialStatus,
            NextAssignedUserId = nextAssignedUserId,
            Requirement = request.Requirement,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Reload the task with navigation properties
        var createdTask = await _context.Tasks
            .Include(t => t.TaskType)
                .ThenInclude(tt => tt.Statuses)
            .Include(t => t.User)
            .Include(t => t.NextAssignedUser)
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        return createdTask ?? task;
    }

    // Deletes a task by ID if it exists
    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }

    // Saves or updates requirement value for a specific task status
    public async Task SaveOrUpdateStatusRequirementAsync(int taskId, int statusId, string value)
    {
        _logger.LogInformation("Saving requirement for task {TaskId} status {StatusId} with value: '{Value}'", 
            taskId, statusId, value);
        
        var existing = await _context.TaskStatusRequirements
            .FirstOrDefaultAsync(r => r.TaskId == taskId && r.StatusId == statusId);

        if (existing == null)
        {
            _logger.LogInformation("Creating new requirement record for task {TaskId} status {StatusId}", taskId, statusId);
            _context.TaskStatusRequirements.Add(new TaskStatusRequirement
            {
                TaskId = taskId,
                StatusId = statusId,
                RequirementValue = value,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            _logger.LogInformation("Updating existing requirement record for task {TaskId} status {StatusId}. Old value: '{OldValue}', New value: '{NewValue}'", 
                taskId, statusId, existing.RequirementValue, value);
            existing.RequirementValue = value;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        
        var result = await _context.SaveChangesAsync();
        _logger.LogInformation("SaveChanges result: {Result} rows affected for task {TaskId} status {StatusId}", 
            result, taskId, statusId);
    }

    // Advances task to next status with requirement validation and saving
    public async Task<TaskValidation> AdvanceTaskAsync(int taskId, string requirement)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
            return new TaskValidation { IsValid = false, Message = "Task not found" };

        if (task.Status == 0) // Closed task
            return new TaskValidation { IsValid = false, Message = "Task is closed and cannot be modified" };

        _logger.LogInformation("Advancing task {TaskId} from status {CurrentStatus} to {NextStatus} with requirement: '{Requirement}'", 
            taskId, task.Status, task.Status + 1, requirement);

        // Validate advancement (sequential progression only)
        var validation = await ValidateAdvancementAsync(task.TypeId, task.Status, requirement);
        if (validation.IsValid)
        {
            _logger.LogInformation("Validation passed for task {TaskId}. Saving requirement for current status {CurrentStatus}", 
                taskId, task.Status);
            
            // Save requirement data for the current status BEFORE advancing
            await SaveOrUpdateStatusRequirementAsync(task.Id, task.Status, requirement);

            // Get next status (sequential progression)
            var nextStatus = task.Status + 1;
            var nextStatusItem = await GetTaskStatusAsync(task.TypeId, nextStatus);
            
            if (nextStatusItem == null)
            {
                _logger.LogWarning("Cannot advance task {TaskId} beyond final status", taskId);
                return new TaskValidation { IsValid = false, Message = "Cannot advance beyond final status" };
            }

            task.Status = nextStatus;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Task {TaskId} advanced successfully to status {NewStatus}", taskId, nextStatus);
            
            // Verify the requirement was saved
            var savedRequirement = await _context.TaskStatusRequirements
                .Where(r => r.TaskId == taskId && r.StatusId == task.Status - 1) // The status we just saved for
                .Select(r => r.RequirementValue)
                .FirstOrDefaultAsync();
            
            _logger.LogInformation("Verification: Saved requirement for task {TaskId} status {Status}: '{SavedRequirement}'", 
                taskId, task.Status - 1, savedRequirement ?? "null");
        }
        else
        {
            _logger.LogWarning("Task {TaskId} advancement validation failed: {Message}", taskId, validation.Message);
        }

        return validation;
    }

    public async Task<ReverseTaskResponse> ReverseTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
        {
            _logger.LogWarning("Reverse failed: Task {TaskId} not found", taskId);
            return new ReverseTaskResponse { IsValid = false, Message = "Task not found" };
        }

        if (task.Status == 0) // Closed task
        {
            return new ReverseTaskResponse { IsValid = false, Message = "Cannot reverse a closed task" };
        }

        // Validate reversal (backward moves always allowed)
        var validation = await ValidateReversalAsync(task.TypeId, task.Status);
        if (validation.IsValid)
        {
            var previousStatus = task.Status - 1;
            if (previousStatus < 1)
            {
                return new ReverseTaskResponse { IsValid = false, Message = "Cannot reverse below status 1" };
            }

            // Get the saved requirement value for the previous status
            var savedRequirement = await _context.TaskStatusRequirements
                .Where(r => r.TaskId == taskId && r.StatusId == previousStatus)
                .Select(r => r.RequirementValue)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Reversing task {TaskId} from status {CurrentStatus} to {PreviousStatus}, saved requirement: {SavedRequirement}", 
                taskId, task.Status, previousStatus, savedRequirement ?? "null");

            task.Status = previousStatus;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ReverseTaskResponse 
            { 
                IsValid = true, 
                Message = "Task reversed successfully",
                SavedRequirement = savedRequirement
            };
        }

        return new ReverseTaskResponse 
        { 
            IsValid = false, 
            Message = validation.Message 
        };
    }

    public async Task<TaskValidation> CloseTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null)
            return new TaskValidation { IsValid = false, Message = "Task not found" };

        if (task.Status == 0) // Already closed
            return new TaskValidation { IsValid = false, Message = "Task is already closed" };

        // Validate closure (only from final status)
        var validation = await ValidateClosureAsync(task.TypeId, task.Status);
        if (validation.IsValid)
        {
            task.Status = 0; // Closed
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        return validation;
    }

    // Retrieves all task types with their statuses
    public async Task<IEnumerable<TaskType>> GetTaskTypesAsync()
    {
        return await _context.TaskTypes
            .Include(t => t.Statuses)
            .OrderBy(t => t.TypeId)
            .ToListAsync();
    }

    // Retrieves task type by ID with statuses
    private async Task<TaskType?> GetTaskTypeAsync(int typeId)
    {
        return await _context.TaskTypes
            .Include(t => t.Statuses)
            .FirstOrDefaultAsync(t => t.TypeId == typeId);
    }

    // Retrieves specific task status by type and status ID
    private async Task<TaskStatusItem?> GetTaskStatusAsync(int typeId, int statusId)
    {
        return await _context.TaskStatusItems
            .FirstOrDefaultAsync(s => s.TypeId == typeId && s.StatusId == statusId);
    }

    // Checks if task can be closed from current status
    private async Task<bool> CanCloseTaskAsync(int typeId, int statusId)
    {
        var status = await GetTaskStatusAsync(typeId, statusId);
        return status?.IsFinal ?? false;
    }

    // Retrieves all saved requirements for a specific task
    public async Task<List<TaskStatusRequirement>> GetTaskStatusRequirementsAsync(int taskId)
    {
        return await _context.TaskStatusRequirements
            .Where(r => r.TaskId == taskId)
            .OrderBy(r => r.StatusId)
            .ToListAsync();
    }

    // New workflow validation methods
    public async Task<TaskValidation> ValidateAdvancementAsync(int typeId, int currentStatus, string requirement)
    {
        var nextStatus = currentStatus + 1;
        var nextStatusItem = await GetTaskStatusAsync(typeId, nextStatus);
        
        if (nextStatusItem == null)
        {
            return new TaskValidation { IsValid = false, Message = "Cannot advance beyond final status" };
        }

        // Check if requirement is provided when needed
        if (!string.IsNullOrEmpty(nextStatusItem.Requirement) && string.IsNullOrWhiteSpace(requirement))
        {
            var requirementMessage = nextStatusItem.RequirementDescription ?? nextStatusItem.Requirement;
            return new TaskValidation { IsValid = false, Message = $"Requirement required: {requirementMessage}" };
        }

        // Type-specific validation
        if (typeId == 1) // Procurement
        {
            if (nextStatus == 2 && !string.IsNullOrWhiteSpace(requirement))
            {
                // Check for 2 price quotes (comma-separated)
                var quotes = requirement.Split(',').Select(q => q.Trim()).Where(q => !string.IsNullOrEmpty(q)).ToList();
                if (quotes.Count < 2)
                {
                    return new TaskValidation { IsValid = false, Message = "Please provide at least 2 price quotes separated by commas" };
                }
            }
        }

        return new TaskValidation { IsValid = true, Message = "Advancement validated successfully" };
    }

    public async Task<TaskValidation> ValidateReversalAsync(int typeId, int currentStatus)
    {
        if (currentStatus <= 1)
        {
            return new TaskValidation { IsValid = false, Message = "Cannot reverse below status 1" };
        }

        return new TaskValidation { IsValid = true, Message = "Reversal validated successfully" };
    }

    public async Task<TaskValidation> ValidateClosureAsync(int typeId, int currentStatus)
    {
        var currentStatusItem = await GetTaskStatusAsync(typeId, currentStatus);
        
        if (currentStatusItem == null)
        {
            return new TaskValidation { IsValid = false, Message = "Invalid status for closure" };
        }

        if (!currentStatusItem.IsFinal)
        {
            return new TaskValidation { IsValid = false, Message = "Task can only be closed from final status" };
        }

        return new TaskValidation { IsValid = true, Message = "Closure validated successfully" };
    }

    public bool IsSequentialAdvancementAsync(int typeId, int currentStatus, int targetStatus)
    {
        return targetStatus == currentStatus + 1;
    }
} 