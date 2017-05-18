﻿using AutoMapper;
using uCommunity.CentralFeed.Models;

namespace uCommunity.CentralFeed.AutoMapperProfiles
{
    public class CentralFeedAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<CentralFeedTabModel, CentralFeedTabViewModel>()
                .ForMember(d => d.Url, o => o.MapFrom(el => el.Content.Url));
        }
    }
}