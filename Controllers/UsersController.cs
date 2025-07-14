using Microsoft.AspNetCore.Mvc;
using TaskPlatformBE.Models;
using TaskPlatformBE.Services;

namespace TaskPlatformBE.Controllers;

// API controller for user management operations
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService, ILogger<UsersController> logger) : base(logger)
    {
        _userService = userService;
    }

    // Get all users
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users.ToList());
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(ex, "getting all users");
        }
    }
} 