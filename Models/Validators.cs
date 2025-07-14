using FluentValidation;
using TaskPlatformBE.Services;

namespace TaskPlatformBE.Models;

/// <summary>
/// Validator for NewTaskRequest
/// </summary>
public class NewTaskRequestValidator : AbstractValidator<NewTaskRequest>
{
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    public NewTaskRequestValidator(ITaskService taskService, IUserService userService)
    {
        _taskService = taskService;
        _userService = userService;

        RuleFor(x => x.TypeId)
            .GreaterThan(0)
            .WithMessage("Task type ID must be greater than 0")
            .MustAsync(async (typeId, cancellation) =>
            {
                var taskTypes = await _taskService.GetTaskTypesAsync();
                return taskTypes.Any(t => t.TypeId == typeId);
            })
            .WithMessage("Invalid task type ID");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0")
            .MustAsync(async (userId, cancellation) =>
            {
                var users = await _userService.GetAllUsersAsync();
                return users.Any(u => u.Id == userId);
            })
            .WithMessage("Invalid user ID");

        RuleFor(x => x.NextAssignedUserId)
            .MustAsync(async (nextUserId, cancellation) =>
            {
                if (!nextUserId.HasValue) return true;
                if (nextUserId.Value <= 0) return false;
                
                var users = await _userService.GetAllUsersAsync();
                return users.Any(u => u.Id == nextUserId.Value);
            })
            .WithMessage("Invalid next assigned user ID");

        RuleFor(x => x.Requirement)
            .MaximumLength(500)
            .WithMessage("Requirement cannot exceed 500 characters");
    }
}

/// <summary>
/// Validator for AdvanceTaskRequest
/// </summary>
public class AdvanceTaskRequestValidator : AbstractValidator<AdvanceTaskRequest>
{
    public AdvanceTaskRequestValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0)
            .WithMessage("Task ID must be greater than 0");

        RuleFor(x => x.Requirement)
            .MaximumLength(500)
            .WithMessage("Requirement cannot exceed 500 characters");
    }
}

/// <summary>
/// Validator for TaskActionRequest
/// </summary>
public class TaskActionRequestValidator : AbstractValidator<TaskActionRequest>
{
    public TaskActionRequestValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0)
            .WithMessage("Task ID must be greater than 0");
    }
} 