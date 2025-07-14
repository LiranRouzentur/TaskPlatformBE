using Microsoft.AspNetCore.Mvc;
using TaskPlatformBE.Models;
using TaskPlatformBE.Services;

namespace TaskPlatformBE.Controllers;

// API controller for task management operations
public class TasksController : BaseApiController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger) : base(logger)
    {
        _taskService = taskService;
    }

    // Health check endpoint
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("Tasks API is working!");
    }

    // Get all tasks
    [HttpGet("get-all-tasks")]
    public async Task<ActionResult<List<TaskItem>>> GetAllTasks([FromQuery] int? userId = null)
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync(userId);
            return Ok(tasks.ToList());
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, "getting all tasks");
        }
    }

    // Get all tasks for a specific user
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<TaskItem>>> GetUserTasks(int userId)
    {
        try
        {
            if (userId <= 0)
                return HandleValidationError("User ID must be greater than 0");

            var tasks = await _taskService.GetAllTasksAsync(userId);
            return Ok(tasks.ToList());
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, $"getting tasks for user {userId}");
        }
    }

    // Create a new task
    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] NewTaskRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return HandleValidationError("Invalid task data");

            var task = await _taskService.CreateTaskAsync(request);
            return CreatedAtAction(nameof(GetAllTasks), new { }, task);
        }
        catch (TaskValidationException ex)
        {
            return HandleBusinessError(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, "creating task");
        }
    }

    // Delete a task by ID
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        try
        {
            if (id <= 0)
                return HandleValidationError("Task ID must be greater than 0");

            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Task with ID {id} not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, $"deleting task {id}");
        }
    }

    // Advance a task to the next status
    [HttpPost("{id}/advance")]
    public async Task<ActionResult<TaskValidation>> AdvanceTask(int id, [FromBody] AdvanceTaskRequest request)
    {
        try
        {
            if (id <= 0)
                return HandleValidationError("Task ID must be greater than 0");

            if (id != request.TaskId)
                return HandleValidationError("ID mismatch");

            var validation = await _taskService.AdvanceTaskAsync(id, request.Requirement ?? string.Empty);
            return Ok(validation);
        }
        catch (TaskValidationException ex)
        {
            return HandleBusinessError(ex.Message);
        }
        catch (TaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, $"advancing task {id}");
        }
    }

    // Reverse a task to the previous status
    [HttpPost("{id}/reverse")]
    public async Task<ActionResult> ReverseTask(int id)
    {
        try
        {
            if (id <= 0)
                return HandleValidationError("Task ID must be greater than 0");

            var reverseResponse = await _taskService.ReverseTaskAsync(id);
            if (!reverseResponse.IsValid)
                return Ok(new { success = false, requirements = new object[0] });

            // Fetch the updated task to get the new status
            var updatedTask = await _taskService.GetTaskByIdAsync(id);
            if (updatedTask == null)
                return Ok(new { success = false, requirements = new object[0] });

            // Fetch the requirement for the current status only
            var requirements = await _taskService.GetTaskStatusRequirementsAsync(id);
            var currentRequirement = requirements
                .Where(r => r.StatusId == updatedTask.Status)
                .Select(r => new {
                    statusId = r.StatusId,
                    requirementValue = r.RequirementValue
                })
                .ToList();

            return Ok(new {
                success = true,
                requirements = currentRequirement
            });
        }
        catch (TaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, $"reversing task {id}");
        }
    }

    // Close a task if it's in a final status
    [HttpPost("{id}/close")]
    public async Task<ActionResult<TaskValidation>> CloseTask(int id)
    {
        try
        {
            if (id <= 0)
                return HandleValidationError("Task ID must be greater than 0");

            var validation = await _taskService.CloseTaskAsync(id);
            return Ok(validation);
        }
        catch (TaskValidationException ex)
        {
            return HandleBusinessError(ex.Message);
        }
        catch (TaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, $"closing task {id}");
        }
    }

    // Get all available task types
    [HttpGet("types")]
    public async Task<ActionResult<List<TaskType>>> GetTaskTypes()
    {
        try
        {
            var taskTypes = await _taskService.GetTaskTypesAsync();
            return Ok(taskTypes.ToList());
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, "getting task types");
        }
    }
} 