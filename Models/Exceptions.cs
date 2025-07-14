namespace TaskPlatformBE.Models;

/// <summary>
/// Base exception for the TaskPlatform application
/// </summary>
public class TaskPlatformException : Exception
{
    public TaskPlatformException(string message) : base(message) { }
    public TaskPlatformException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a task is not found
/// </summary>
public class TaskNotFoundException : TaskPlatformException
{
    public int TaskId { get; }
    
    public TaskNotFoundException(int taskId) 
        : base($"Task with ID {taskId} was not found.")
    {
        TaskId = taskId;
    }
}

/// <summary>
/// Exception thrown when task validation fails
/// </summary>
public class TaskValidationException : TaskPlatformException
{
    public TaskValidationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when workflow rules are violated
/// </summary>
public class WorkflowViolationException : TaskPlatformException
{
    public WorkflowViolationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when a user is not found
/// </summary>
public class UserNotFoundException : TaskPlatformException
{
    public int UserId { get; }
    
    public UserNotFoundException(int userId) 
        : base($"User with ID {userId} was not found.")
    {
        UserId = userId;
    }
} 