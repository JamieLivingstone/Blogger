using AutoMapper;
using Blogger.Core.Entities;
using Blogger.WebApi.Resources.Profile;
using Blogger.WebApi.Resources.User;

namespace Blogger.WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SaveUserResource, ApplicationUser>(MemberList.None);
            CreateMap<ApplicationUser, UserResource>(MemberList.None);
            CreateMap<ApplicationUser, ProfileResource>(MemberList.None);
        }
    }
}