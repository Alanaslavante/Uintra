﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using uIntra.Core.Exceptions;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using File = System.IO.File;
using static uIntra.Core.Constants.UmbracoAliases.Media;

namespace uIntra.Search
{
    public class DocumentIndexer : IIndexer, IDocumentIndexer
    {
        private readonly IElasticDocumentIndex _documentIndex;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ISearchApplicationSettings _settings;
        private readonly IMediaHelper _mediaHelper;
        private readonly IExceptionLogger _exceptionLogger;
        private readonly IMediaService _mediaService;

        public DocumentIndexer(IElasticDocumentIndex documentIndex,
            UmbracoHelper umbracoHelper,
            ISearchApplicationSettings settings,
            IMediaHelper mediaHelper,
            IExceptionLogger exceptionLogger,
            IMediaService mediaService)
        {
            _documentIndex = documentIndex;
            _umbracoHelper = umbracoHelper;
            _settings = settings;
            _mediaHelper = mediaHelper;
            _exceptionLogger = exceptionLogger;
            _mediaService = mediaService;
        }

        public void FillIndex()
        {
            var documentsToIndex = GetDocumentsForIndexing();
            Index(documentsToIndex);
        }

        private IEnumerable<int> GetDocumentsForIndexing()
        {
            var medias = _umbracoHelper
                .TypedMediaAtRoot()
                .SelectMany(m => m.DescendantsOrSelf());

            var result = medias
                .Where(c => IsAllowedForIndexing(c) && !_mediaHelper.IsMediaDeleted(c))
                .Select(m => m.Id);

            return result.ToList();
        }

        private bool IsAllowedForIndexing(IPublishedContent media)
        {
            return media.GetPropertyValue<bool>(UseInSearchPropertyAlias);
        }

        private bool IsAllowedForIndexing(IMedia media)
        {
            return media.HasProperty(UseInSearchPropertyAlias) && media.GetValue<bool>(UseInSearchPropertyAlias);
        }

        public void Index(int id)
        {
            Index(id.ToEnumerableOfOne());
        }

        public void Index(IEnumerable<int> ids)
        {
            var medias = _mediaService.GetByIds(ids);
            var documents = new List<SearchableDocument>();

            foreach (var media in medias)
            {
                var document = GetSearchableDocument(media.Id);
                if (!document.Any()) continue;

                if (!IsAllowedForIndexing(media))
                {
                    media.SetValue(UseInSearchPropertyAlias, true);
                    _mediaService.Save(media);
                }

                documents.AddRange(document);
            }
            _documentIndex.Index(documents);
        }

        public void DeleteFromIndex(int id)
        {
            DeleteFromIndex(id.ToEnumerableOfOne());
        }

        public void DeleteFromIndex(IEnumerable<int> ids)
        {
            var medias = _mediaService.GetByIds(ids);
            foreach (var media in medias)
            {
                if (IsAllowedForIndexing(media))
                {
                    media.SetValue(UseInSearchPropertyAlias, false);
                    _mediaService.Save(media);
                }
                _documentIndex.Delete(media.Id);
            }
        }

        private IEnumerable<SearchableDocument> GetSearchableDocument(int id)
        {
            var content = _umbracoHelper.TypedMedia(id);
            if (content == null)
            {
                return Enumerable.Empty<SearchableDocument>();
            }

            return GetSearchableDocument(content);
        }

        private IEnumerable<SearchableDocument> GetSearchableDocument(IPublishedContent content)
        {
            var fileName = Path.GetFileName(content.Url);
            var extension = Path.GetExtension(fileName)?.Trim('.');

            bool isFileExtensionAllowedForIndex = _settings.IndexingDocumentTypesKey.Contains(extension, StringComparison.OrdinalIgnoreCase);


            if (!content.Url.IsNullOrEmpty())
            {
                var physicalPath = HostingEnvironment.MapPath(content.Url);

                if (!File.Exists(physicalPath))
                {
                    _exceptionLogger.Log(new FileNotFoundException($"Could not find file \"{physicalPath}\""));
                   return Enumerable.Empty<SearchableDocument>();
                }
                var base64File = isFileExtensionAllowedForIndex ? Convert.ToBase64String(File.ReadAllBytes(physicalPath)) : string.Empty;
                var result = new SearchableDocument
                {
                    Id = content.Id,
                    Title = fileName,
                    Url = content.Url,
                    Data = base64File,
                    Type = SearchableTypeEnum.Document.ToInt()
                };
                 return result.ToEnumerableOfOne();
            }
            return Enumerable.Empty<SearchableDocument>();
        }
    }
}
