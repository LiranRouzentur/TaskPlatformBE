using TaskPlatformBE.Models;

namespace TaskPlatformBE.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
} 