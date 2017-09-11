﻿using System;
using System.Collections.Generic;
using System.Linq;
using uIntra.Core.Caching;
using uIntra.Core.Extentions;
using uIntra.Core.Links;
using uIntra.Core.Media;
using uIntra.Core.TypeProviders;
using Umbraco.Core.Models;

namespace uIntra.Core.Activity
{
    public abstract class IntranetActivityService<TActivity> : IIntranetActivityService<TActivity> where TActivity : IIntranetActivity
    {
        public abstract IIntranetType ActivityType { get; }        
        private const string CacheKey = "ActivityCache";
        private string ActivityCacheSuffix => $"{ActivityType.Id}";
        private readonly IIntranetActivityRepository _activityRepository;
        private readonly ICacheService _cache;
        private readonly IActivityTypeProvider _activityTypeProvider;
        private readonly IIntranetMediaService _intranetMediaService;

        protected IntranetActivityService(IIntranetActivityRepository activityRepository,
            ICacheService cache, IActivityTypeProvider activityTypeProvider,IIntranetMediaService intranetMediaService
            )
        {
            _activityRepository = activityRepository;
            _cache = cache;
            _activityTypeProvider = activityTypeProvider;
            _intranetMediaService = intranetMediaService;
        }


        public virtual  ActivityLinks GetCentralFeedLinks(Guid id)
        {
            return new ActivityLinks(
                overview: GetOverviewPage().Url,
                create: GetCreatePage().Url,
                details: GetDetailsPage().Url.AddIdParameter(id),
                edit: GetEditPage().Url.AddIdParameter(id),
                profile: GetProfileLink(id)
            );
        }

        public virtual ActivityLinks GetGroupFeedLinks(Guid id)
        {
            return GetCentralFeedLinks(id);
        }

        protected abstract string GetProfileLink(Guid activityId);

        public TActivity Get(Guid id)
        {
            var cached = GetAll(true).SingleOrDefault(s => s.Id == id);
            return cached;
        }

        public IEnumerable<TActivity> GetManyActual()
        {
            var cached = GetAll(true);
            var actual = cached.Where(s => IsActual(s));
            return actual;
        }

        public IEnumerable<TActivity> GetAll(bool includeHidden = false)
        {            
            if (!_cache.HasValue(CacheKey, ActivityCacheSuffix))
            {
                UpdateCache();
            }
            var cached = GetAllFromCache();
            if (!includeHidden)
            {
                cached = cached.Where(s => !s.IsHidden);
            }
            return cached;
        }

        protected virtual void UpdateCache()
        {
            FillCache();
        }

        private void FillCache()
        {
            var items = GetAllFromSql();
            _cache.Set(CacheKey, items, CacheHelper.GetMidnightUtcDateTimeOffset(), ActivityCacheSuffix);
        }

        public virtual bool IsActual(IIntranetActivity cachedActivity)
        {
            return !cachedActivity.IsHidden;
        }

        public Guid Create(IIntranetActivity activity)
        {
            var newActivity = new IntranetActivityEntity { Type = ActivityType.Id, JsonData = activity.ToJson() };
            _activityRepository.Create(newActivity);

            var newActivityId = newActivity.Id;
            _intranetMediaService.Create(newActivityId, activity.MediaIds.JoinToString());
            UpdateCachedEntity(newActivityId);
            return newActivityId;
        }

        public void Save(IIntranetActivity activity)
        {
            var entity = _activityRepository.Get(activity.Id);
            entity.JsonData = activity.ToJson();
            _activityRepository.Update(entity);
            _intranetMediaService.Update(activity.Id, activity.MediaIds.JoinToString());
            UpdateCachedEntity(activity.Id);
        }

        public void Delete(Guid id)
        {
            _activityRepository.Delete(id);
            _intranetMediaService.Delete(id);
            UpdateCachedEntity(id);
        }

        public bool CanEdit(Guid id)
        {
            var cached = Get(id);
            return CanEdit(cached);
        }

        public abstract bool CanEdit(IIntranetActivity cached);
        public abstract IPublishedContent GetOverviewPage();
        public abstract IPublishedContent GetDetailsPage();
        public abstract IPublishedContent GetCreatePage();
        public abstract IPublishedContent GetEditPage();
        public abstract IPublishedContent GetOverviewPage(IPublishedContent currentPage);
        public abstract IPublishedContent GetDetailsPage(IPublishedContent currentPage);
        public abstract IPublishedContent GetCreatePage(IPublishedContent currentPage);
        public abstract IPublishedContent GetEditPage(IPublishedContent currentPage);

        protected IEnumerable<TActivity> GetAllFromCache()
        {
            var activities = _cache.Get<IList<TActivity>>(CacheKey, ActivityCacheSuffix);
            return activities;
        }

        protected virtual TActivity UpdateCachedEntity(Guid id)
        {
            var activity = GetFromSql(id);
            var cached = GetAll(true);
            var cachedList = (cached as List<TActivity> ?? cached.ToList()).FindAll(s => s.Id != id);

            if (activity != null)
            {
                MapBeforeCache(((IIntranetActivity)activity).ToListOfOne());
                cachedList.Add(activity);
            }

            _cache.Set(CacheKey, cachedList, CacheHelper.GetMidnightUtcDateTimeOffset(), ActivityCacheSuffix);

            return activity;
        }

        private TActivity GetFromSql(Guid id)
        {
            var activityEntity = _activityRepository.Get(id);
            if (activityEntity == null)
            {
                return default(TActivity);
            }

            var activity = MapInternal(activityEntity);
            return activity;
        }

        private IList<TActivity> GetAllFromSql()
        {
            var activities = _activityRepository.GetMany(ActivityType).Select(MapInternal).ToList();
            MapBeforeCache(activities.Select(s => (IIntranetActivity)s).ToList());
            return activities;
        }

        private TActivity MapInternal(IntranetActivityEntity activity)
        {
            var cachedActivity = activity.JsonData.Deserialize<TActivity>();
            cachedActivity.Id = activity.Id;
            cachedActivity.Type = _activityTypeProvider.Get(activity.Type);
            cachedActivity.CreatedDate = activity.CreatedDate;
            cachedActivity.ModifyDate = activity.ModifyDate;
            cachedActivity.IsPinActual = IsPinActual(cachedActivity);
            cachedActivity.MediaIds = _intranetMediaService.GetEntityMedia(cachedActivity.Id);
            return cachedActivity;
        }

        private static bool IsPinActual(IIntranetActivity activity)
        {
            if (!activity.IsPinned) return false;

            if (activity.EndPinDate.HasValue)
            {
                return activity.EndPinDate.Value.ToUniversalTime() > DateTime.UtcNow;
            }

            return true;
        }

        protected abstract void MapBeforeCache(IList<IIntranetActivity> cached);
    }
}