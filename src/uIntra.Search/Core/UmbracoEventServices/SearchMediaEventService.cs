﻿using System;
using System.Linq;
using uIntra.Core.UmbracoEventServices;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using static uIntra.Core.Constants.UmbracoAliases.Media;

namespace uIntra.Search
{
    public class SearchMediaEventService : IUmbracoMediaEventService
    {
        private readonly IDocumentIndexer _documentIndexer;

        public SearchMediaEventService(IDocumentIndexer documentIndexer)
        {
            _documentIndexer = documentIndexer;
        }

        public void ProcessMediaSaved(IMediaService sender, SaveEventArgs<IMedia> args)
        {
            var actualMedia = args
                .SavedEntities
                .Where(m => !m.IsNewEntity());

            foreach (var media in actualMedia)
            {
                if (IsAllowedForSearch(media))
                    _documentIndexer.Index(media.Id);
                else _documentIndexer.DeleteFromIndex(media.Id);
            }
            //var entities = args
            //    .SavedEntities
            //    .Where(m => !m.IsNewEntity())
            //    .ToLookup(IsAllowedForSearch)
            //    .ToDictionary(g => g.Key, g => g.Select(m => m.Id).ToList());

            //entities[true].ForEach(_documentIndexer.Index);
            //entities[false].ForEach(_documentIndexer.DeleteFromIndex);



        }

        private bool IsAllowedForSearch(IMedia media)
        {
            return media.HasProperty(UseInSearchPropertyAlias) &&
                   media.GetValue<bool>(UseInSearchPropertyAlias);
        }
    }
}
