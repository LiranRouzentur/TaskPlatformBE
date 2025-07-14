using AutoMapper;
using TaskPlatformBE.Models;

namespace TaskPlatformBE.Services;

/// <summary>
/// Service for mapping between entities and DTOs using AutoMapper
/// </summary>
public class MappingService : IMappingService
{
    private readonly IMapper _mapper;

    public MappingService(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Maps a single TaskItem to TaskDto
    /// </summary>
    public TaskDto MapToTaskDto(TaskItem task)
    {
        return _mapper.Map<TaskDto>(task);
    }

    /// <summary>
    /// Maps a collection of TaskItems to TaskDtos
    /// </summary>
    public List<TaskDto> MapToTaskDtos(IEnumerable<TaskItem> tasks)
    {
        return _mapper.Map<List<TaskDto>>(tasks);
    }

    /// <summary>
    /// Maps a single TaskType to TaskTypeDto
    /// </summary>
    public TaskTypeDto MapToTaskTypeDto(TaskType taskType)
    {
        return _mapper.Map<TaskTypeDto>(taskType);
    }

    /// <summary>
    /// Maps a collection of TaskTypes to TaskTypeDtos
    /// </summary>
    public List<TaskTypeDto> MapToTaskTypeDtos(IEnumerable<TaskType> taskTypes)
    {
        return _mapper.Map<List<TaskTypeDto>>(taskTypes);
    }

    /// <summary>
    /// Maps a single User to UserDto
    /// </summary>
    public UserDto MapToUserDto(User user)
    {
        return _mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Maps a collection of Users to UserDtos
    /// </summary>
    public List<UserDto> MapToUserDtos(IEnumerable<User> users)
    {
        return _mapper.Map<List<UserDto>>(users);
    }

    /// <summary>
    /// Maps a NewTaskRequest to TaskItem
    /// </summary>
    public TaskItem MapToTaskItem(NewTaskRequest request)
    {
        return _mapper.Map<TaskItem>(request);
    }
} 