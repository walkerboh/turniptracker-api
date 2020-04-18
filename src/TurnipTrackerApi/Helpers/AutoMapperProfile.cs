using AutoMapper;
using TurnipTrackerApi.Database.Entities;
using TurnipTrackerApi.Models.Boards;
using TurnipTrackerApi.Models.BoardUsers;
using TurnipTrackerApi.Models.Users;

namespace TurnipTrackerApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BoardCreateModel, Board>().ForMember(dest => dest.UrlName,
                opt => opt.MapFrom(src => src.UrlName.ToLowerInvariant()));
            CreateMap<RegisterModel, RegisteredUser>();
            CreateMap<RegisteredUser, UserModel>();
            CreateMap<Board, BoardModel>();

            CreateMap<BoardUser, BoardUserModel>().ForMember(dest=>dest.UserId, opt => opt.MapFrom(src=>src.RegisteredUserId));
        }
    }
}