using AutoMapper;
using Blogger.Core.Entities;
using Blogger.WebApi.Resources.User;

namespace Blogger.WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SaveUserResource, ApplicationUser>(MemberList.None);
        }
    }
}