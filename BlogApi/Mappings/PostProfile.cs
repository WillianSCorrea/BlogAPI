using AutoMapper;
using BlogApi.Models;
using BlogApi.DTOs;

namespace BlogApi.Mappings
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            // De DTO -> Model (para criar/atualizar post)
            CreateMap<PostCreateUpdateDto, Post>();

            // De Model -> DTO (para retornar ao cliente)
            CreateMap<Post, PostResponseDto>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.User.Username);
        }
    }
}
