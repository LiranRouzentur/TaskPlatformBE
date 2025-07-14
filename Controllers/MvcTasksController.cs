using Microsoft.AspNetCore.Mvc;
using TaskPlatformBE.Models;
using TaskPlatformBE.Services;

namespace TaskPlatformBE.Controllers;

/// <summary>
/// MVC controller for task management operations
/// </summary>
public class MvcTasksController : Controller
{
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private readonly IMappingService _mappingService;
    private readonly ILogger<MvcTasksController> _logger;

    public MvcTasksController(
        ITaskService taskService, 
        IUserService userService, 
        IMappingService mappingService,
        ILogger<MvcTasksController> logger)
    {
        _taskService = taskService;
        _userService = userService;
        _mappingService = mappingService;
        _logger = logger;
    }

    // Main page displaying tasks and task management interface
    public async Task<IActionResult> Index(int? userId = null)
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync(userId);
            var taskTypes = await _taskService.GetTaskTypesAsync();
            var users = await _userService.GetAllUsersAsync();

            var viewModel = new TasksViewModel
            {
                Tasks = _mappingService.MapToTaskDtos(tasks),
                TaskTypes = _mappingService.MapToTaskTypeDtos(taskTypes),
                Users = users.ToList(),
                SelectedUserId = userId
            };

            // Handle reversed task requirements for UI state
            ViewData["ReversedTaskId"] = TempData["ReversedTaskId"]?.ToString() ?? "";
            ViewData["ReversedRequirement"] = TempData["ReversedRequirement"]?.ToString() ?? "";

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tasks for MVC view");
            TempData["ErrorMessage"] = "Failed to load tasks. Please try again.";
            return View(new TasksViewModel());
        }
    }

    // Creates a new task via AJAX
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] NewTaskRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new TaskOperationResponse 
                { 
                    Success = false, 
                    Message = "Invalid task data. Please check your input." 
                });
            }

            var task = await _taskService.CreateTaskAsync(request);
            var mappedTask = _mappingService.MapToTaskDto(task);
            return Json(new TaskOperationResponse 
            { 
                Success = true, 
                Message = "Task created successfully!",
                Task = mappedTask
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Task creation validation failed");
            return Json(new TaskOperationResponse 
            { 
                Success = false, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return Json(new TaskOperationResponse 
            { 
                Success = false, 
                Message = "Failed to create task. Please try again." 
            });
        }
    }

    // Advances a task to the next status via AJAX
    [HttpPost]
    public async Task<IActionResult> AdvanceTask([FromBody] AdvanceTaskRequest request)
    {
        try
        {
            var validation = await _taskService.AdvanceTaskAsync(request.TaskId, request.Requirement ?? string.Empty);
            
            if (validation.IsValid)
            {
                var updatedTask = await _taskService.GetTaskByIdAsync(request.TaskId);
                if (updatedTask == null)
                {
                    return Json(new TaskOperationResponse 
                    { 
                        Success = false, 
                        Message = "Task not found after advancement." 
                    });
                }

                var mappedTask = _mappingService.MapToTaskDto(updatedTask);
                
                return Json(new TaskOperationResponse
                { 
                    Success = true, 
                    Message = "Task advanced successfully!",
                    Task = mappedTask
                });
            }
            else
            {
                return Json(new TaskOperationResponse 
                { 
                    Success = false, 
                    Message = validation.Message 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error advancing task {TaskId}", request.TaskId);
            return Json(new TaskOperationResponse 
            { 
                Success = false, 
                Message = "Failed to advance task. Please try again." 
            });
        }
    }

    // Reverses a task to the previous status via AJAX
    [HttpPost]
    public async Task<IActionResult> ReverseTask([FromBody] TaskActionRequest request)
    {
        try
        {
            var currentTask = await _taskService.GetTaskByIdAsync(request.TaskId);
            if (currentTask == null)
            {
                return Json(new TaskOperationResponse 
                { 
                    Success = false, 
                    Message = "Task not found." 
                });
            }

            var reverseResponse = await _taskService.ReverseTaskAsync(request.TaskId);
            
            if (reverseResponse.IsValid)
            {
                var updatedTask = await _taskService.GetTaskByIdAsync(request.TaskId);
                if (updatedTask == null)
                {
                    return Json(new TaskOperationResponse 
                    { 
                        Success = false, 
                        Message = "Task not found after reversal." 
                    });
                }

                return Json(new TaskOperationResponse
                { 
                    Success = true, 
                    Message = reverseResponse.Message,
                    Task = _mappingService.MapToTaskDto(updatedTask),
                    ReversedTaskId = request.TaskId.ToString(),
                    ReversedRequirement = reverseResponse.SavedRequirement
                });
            }
            else
            {
                return Json(new TaskOperationResponse 
                { 
                    Success = false, 
                    Message = reverseResponse.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reversing task {TaskId}", request.TaskId);
            return Json(new TaskOperationResponse 
            { 
                Success = false, 
                Message = "Failed to reverse task. Please try again." 
            });
        }
    }

    // Closes a task via AJAX
    [HttpPost]
    public async Task<IActionResult> CloseTask([FromBody] TaskActionRequest request)
    {
        try
        {
            var validation = await _taskService.CloseTaskAsync(request.TaskId);
            
            if (validation.IsValid)
            {
                var updatedTask = await _taskService.GetTaskByIdAsync(request.TaskId);
                return Json(new TaskOperationResponse 
                { 
                    Success = true, 
                    Message = "Task closed successfully!",
                    Task = updatedTask != null ? _mappingService.MapToTaskDto(updatedTask) : null
                });
            }
            else
            {
                return Json(new TaskOperationResponse 
                { 
                    Success = false, 
                    Message = validation.Message 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing task {TaskId}", request.TaskId);
            return Json(new TaskOperationResponse 
            { 
                Success = false, 
                Message = "Failed to close task. Please try again." 
            });
        }
    }

    // Deletes a task via AJAX
    [HttpPost]
    public async Task<IActionResult> DeleteTask([FromBody] TaskActionRequest request)
    {
        try
        {
            var deleted = await _taskService.DeleteTaskAsync(request.TaskId);
            
            if (deleted)
            {
                return Json(new TaskOperationResponse 
                { 
                    Success = true, 
                    Message = "Task deleted successfully!" 
                });
            }
            else
            {
                return Json(new TaskOperationResponse 
                { 
                    Success = false, 
                    Message = "Task not found." 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", request.TaskId);
            return Json(new TaskOperationResponse 
            { 
                Success = false, 
                Message = "Failed to delete task. Please try again." 
            });
        }
    }
} 