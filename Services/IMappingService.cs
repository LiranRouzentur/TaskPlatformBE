using TaskPlatformBE.Models;

namespace TaskPlatformBE.Services;

/// <summary>
/// Interface for mapping between entities and DTOs
/// </summary>
public interface IMappingService
{
    /// <summary>
    /// Maps a single TaskItem to TaskDto
    /// </summary>
    TaskDto MapToTaskDto(TaskItem task);

    /// <summary>
    /// Maps a collection of TaskItems to TaskDtos
    /// </summary>
    List<TaskDto> MapToTaskDtos(IEnumerable<TaskItem> tasks);

    /// <summary>
    /// Maps a single TaskType to TaskTypeDto
    /// </summary>
    TaskTypeDto MapToTaskTypeDto(TaskType taskType);

    /// <summary>
    /// Maps a collection of TaskTypes to TaskTypeDtos
    /// </summary>
    List<TaskTypeDto> MapToTaskTypeDtos(IEnumerable<TaskType> taskTypes);

    /// <summary>
    /// Maps a single User to UserDto
    /// </summary>
    UserDto MapToUserDto(User user);

    /// <summary>
    /// Maps a collection of Users to UserDtos
    /// </summary>
    List<UserDto> MapToUserDtos(IEnumerable<User> users);

    /// <summary>
    /// Maps a NewTaskRequest to TaskItem
    /// </summary>
    TaskItem MapToTaskItem(NewTaskRequest request);
} 