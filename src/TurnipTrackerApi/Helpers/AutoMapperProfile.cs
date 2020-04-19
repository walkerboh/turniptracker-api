using AutoMapper;
using TurnipTallyApi.Database.Entities;
using TurnipTallyApi.Models.Boards;
using TurnipTallyApi.Models.BoardUsers;
using TurnipTallyApi.Models.Prices;
using TurnipTallyApi.Models.Users;

namespace TurnipTallyApi.Helpers
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
            CreateMap<Board, UserBoardsModel>();

            CreateMap<Week, PricesUserModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BoardUser.Name))
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.Records));
            CreateMap<Record, PricesRecordModel>();
        }
    }
}