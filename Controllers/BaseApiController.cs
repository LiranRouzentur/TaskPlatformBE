using Microsoft.AspNetCore.Mvc;
using TaskPlatformBE.Models;

namespace TaskPlatformBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger _logger;

    protected BaseApiController(ILogger logger)
    {
        _logger = logger;
    }

    // Handles successful results with null checking
    protected ActionResult<T> HandleResult<T>(T? result) where T : class
    {
        if (result == null)
        {
            _logger.LogWarning("Requested resource not found");
            return NotFound(new { message = "Resource not found" });
        }
        return Ok(result);
    }

    // Handles successful results with custom message
    protected ActionResult<T> HandleResult<T>(T? result, string message) where T : class
    {
        if (result == null)
        {
            _logger.LogWarning("Requested resource not found: {Message}", message);
            return NotFound(new { message });
        }
        return Ok(new { data = result, message });
    }

    // Handles validation errors
    protected ActionResult HandleValidationError(string message)
    {
        _logger.LogWarning("Validation error: {Message}", message);
        return BadRequest(new { message, type = "ValidationError" });
    }

    // Handles business logic errors
    protected ActionResult HandleBusinessError(string message)
    {
        _logger.LogWarning("Business logic error: {Message}", message);
        return BadRequest(new { message, type = "BusinessError" });
    }

    // Handles unexpected errors
    protected ActionResult HandleUnexpectedError(Exception ex, string operation)
    {
        _logger.LogError(ex, "Unexpected error during {Operation}", operation);
        return StatusCode(500, new {
            message = "An unexpected error occurred. Please try again later.",
            type = "InternalError"
        });
    }

    // Standardized success response
    protected ActionResult SuccessResponse(string message, object? data = null)
    {
        return Ok(new {
            success = true,
            message,
            data
        });
    }

    // Standardized error response
    protected ActionResult ErrorResponse(string message, string type = "Error")
    {
        return BadRequest(new {
            success = false,
            message,
            type
        });
    }
} 