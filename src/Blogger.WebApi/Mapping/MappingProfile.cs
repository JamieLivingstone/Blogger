using System;
using AutoMapper;
using Blogger.Core.Entities;
using Blogger.Infrastructure;
using Blogger.WebApi.Resources.Article;
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

            CreateMap<SaveArticleResource, Article>()
                .ForMember(a => a.Slug, opt => opt.MapFrom(ar => ar.Title.GenerateSlug()));
        }
    }
}