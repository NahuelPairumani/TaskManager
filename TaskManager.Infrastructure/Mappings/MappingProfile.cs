using AutoMapper;
using TaskManager.Core.Entities;
using TaskManager.Infrastructure.DTOs;

namespace TaskManager.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskAssignment, TaskAssignmentDto>();
            CreateMap<TaskAssignmentDto, TaskAssignment>();

            CreateMap<TaskEntity, TaskEntityDto>();
            CreateMap<TaskEntityDto, TaskEntity>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
