﻿using AutoMapper;
using uIntra.Core.Activity;
using uIntra.Events;
using uIntra.Search;

namespace Compent.uIntra.Core.Events
{
    public class EventsAutoMapperProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Event, EventExtendedViewModel>()
                  .IncludeBase<EventBase, EventViewModel>()
                  .ForMember(dst => dst.LikesInfo, o => o.MapFrom(el => el))
                  .ForMember(dst => dst.CommentsInfo, o => o.MapFrom(el => el))
                  .ForMember(dst => dst.SubscribeInfo, o => o.MapFrom(el => el));


            Mapper.CreateMap<Event, EventExtendedItemModel>()
                .IncludeBase<EventBase, EventItemViewModel>()
                .ForMember(dst => dst.LikesInfo, o => o.MapFrom(el => el))
                .ForMember(dst => dst.IsSubscribed, o => o.Ignore());

            Mapper.CreateMap<Event, IntranetActivityItemHeaderViewModel>()
                 .IncludeBase<EventBase, IntranetActivityItemHeaderViewModel>();

            Mapper.CreateMap<EventEditModel, Event>()
                .IncludeBase<EventEditModel, EventBase>()
                .ForMember(dst => dst.GroupIds, o => o.Ignore())
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ForMember(dst => dst.IsHidden, o => o.Ignore())
                .ForMember(dst => dst.UmbracoCreatorId, o => o.Ignore())
                .ForMember(dst => dst.CreatedDate, o => o.Ignore())
                .ForMember(dst => dst.ModifyDate, o => o.Ignore())
                .ForMember(dst => dst.Type, o => o.Ignore())
                .ForMember(dst => dst.CanSubscribe, o => o.Ignore())
                .ForMember(dst => dst.MediaIds, o => o.Ignore())
                .ForMember(dst => dst.Type, o => o.Ignore())
                .ForMember(dst => dst.Likes, o => o.Ignore())
                .ForMember(dst => dst.Comments, o => o.Ignore())
                .ForMember(dst => dst.Subscribers, o => o.Ignore());

            Mapper.CreateMap<EventCreateModel, Event>()
                .IncludeBase<EventCreateModel, EventBase>()
                .ForMember(dst => dst.GroupIds, o => o.Ignore())
                .ForMember(dst => dst.Type, o => o.Ignore())
                .ForMember(dst => dst.Likes, o => o.Ignore())
                .ForMember(dst => dst.Comments, o => o.Ignore())
                .ForMember(dst => dst.Subscribers, o => o.Ignore());

            Mapper.CreateMap<Event, SearchableActivity>()
                .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate))
                .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate))
                .ForMember(dst => dst.Url, o => o.Ignore())
                .ForMember(dst => dst.PublishedDate, o => o.Ignore())
                .ForMember(dst => dst.Type, o => o.Ignore())
                .IncludeBase<IntranetActivity, SearchableActivity>();
        }
    }
}