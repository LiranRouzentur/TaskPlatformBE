using AutoMapper;
using TaskPlatformBE.Models;

namespace TaskPlatformBE.Services;

/// <summary>
/// AutoMapper profile for TaskPlatform entities
/// </summary>
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // TaskItem mappings
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.NextAssignedUserId, opt => opt.MapFrom(src => src.NextAssignedUserId))
            .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src => src.Requirement))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.CustomFields, opt => opt.MapFrom(src => src.CustomFields))
            .ForMember(dest => dest.TaskType, opt => opt.MapFrom(src => src.TaskType))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.NextAssignedUser, opt => opt.MapFrom(src => src.NextAssignedUser));

        CreateMap<NewTaskRequest, TaskItem>()
            .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.NextAssignedUserId, opt => opt.MapFrom(src => src.NextAssignedUserId))
            .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src => src.Requirement))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // TaskType mappings
        CreateMap<TaskType, TaskTypeDto>()
            .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.TypeId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Statuses, opt => opt.MapFrom(src => src.Statuses));

        // TaskStatusItem mappings
        CreateMap<TaskStatusItem, TaskStatusItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TaskTypeId, opt => opt.MapFrom(src => src.TaskTypeId))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.IsFinal, opt => opt.MapFrom(src => src.IsFinal))
            .ForMember(dest => dest.Requirement, opt => opt.MapFrom(src => src.Requirement))
            .ForMember(dest => dest.RequirementDescription, opt => opt.MapFrom(src => src.RequirementDescription));

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => string.Empty)); // Default empty email
    }
} 