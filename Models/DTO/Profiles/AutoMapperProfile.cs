using AutoMapper;
using blogsite.Models.DTO.RequestDTO;
using blogsite.Models.DTO.ResponseDTO;

namespace blogsite.Models.DTO.Profiles;

	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		
		{
			// From DB => Client
			CreateMap<User, UserResponseDTO>();
			CreateMap<Posts, PostResponseDTO>();
			
			// From Client => DB
			CreateMap<PostRequestDTO, Posts>();
			CreateMap<UserRequestDTO, User>();
			
		}
	}
