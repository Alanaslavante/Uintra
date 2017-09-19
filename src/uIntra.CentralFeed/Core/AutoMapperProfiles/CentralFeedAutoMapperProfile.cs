﻿using AutoMapper;

namespace uIntra.CentralFeed
{
    public class CentralFeedAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<FeedSettings, FeedTabSettings>();
            Mapper.CreateMap<FeedTabModel, FeedTabViewModel>();
        }
    }
}
