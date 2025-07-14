using Microsoft.EntityFrameworkCore;
using TaskPlatformBE.Data;
using TaskPlatformBE.Models;

namespace TaskPlatformBE.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Retrieves all active users ordered by name
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name
            })
            .ToListAsync();
    }
} 