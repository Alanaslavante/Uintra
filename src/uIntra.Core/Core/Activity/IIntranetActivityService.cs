using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace uIntra.Core.Activity
{
    public interface IIntranetActivityService<out TActivity> : IIntranetActivityService where TActivity : IIntranetActivity
    {
        IntranetActivityTypeEnum ActivityType { get; }

        TActivity Get(Guid id);
        IEnumerable<TActivity> GetManyActual();
        IEnumerable<TActivity> GetAll(bool includeHidden = false);
        bool IsActual(IIntranetActivity cachedActivity);        
        Guid Create(IIntranetActivity activity);
        void Save(IIntranetActivity activity);
        bool CanEdit(IIntranetActivity cached);

    }

    public interface IIntranetActivityService
    {
        void Delete(Guid id);
        bool CanEdit(Guid id);

        [Obsolete("Use overloading method instead")]
        IPublishedContent GetOverviewPage();
        [Obsolete("Use overloading method instead")]
        IPublishedContent GetDetailsPage();
        [Obsolete("Use overloading method instead")]
        IPublishedContent GetCreatePage();
        [Obsolete("Use overloading method instead")]
        IPublishedContent GetEditPage();

        IPublishedContent GetOverviewPage(IPublishedContent currentPage);
        IPublishedContent GetDetailsPage(IPublishedContent currentPage);
        IPublishedContent GetCreatePage(IPublishedContent currentPage);
        IPublishedContent GetEditPage(IPublishedContent currentPage);
    }

}