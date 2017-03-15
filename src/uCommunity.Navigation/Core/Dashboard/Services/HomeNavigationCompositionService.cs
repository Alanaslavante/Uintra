﻿using System.Linq;
using uCommunity.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace uCommunity.Navigation.Core.Dashboard
{
    public class HomeNavigationCompositionService : DocumentTypeService, IHomeNavigationCompositionService
    {
        private readonly NavigationConfiguration _navigationConfiguration;

        public HomeNavigationCompositionService(
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService,
            IConfigurationProvider<NavigationConfiguration> navigationConfigurationProvider
            ) : base(contentTypeService, dataTypeService)
        {
            _navigationConfiguration = navigationConfigurationProvider.GetSettings();
        }

        protected override void CreateTabs(IContentType currentDocumentType, IContentType parentDocumentType)
        {
            if (parentDocumentType == null || !parentDocumentType.CompositionPropertyGroups.Any(x => x.Name.Equals(_navigationConfiguration.NavigationTab)))
            {
                currentDocumentType.AddPropertyGroup(_navigationConfiguration.NavigationTab);
            }
        }

        protected override void AddProperties(IContentType documentType)
        {
            AddProperty(documentType, UmbracoPropertyTypeEnum.TrueFalse, _navigationConfiguration.IsShowInHomeNavigation.Alias,
                _navigationConfiguration.IsShowInHomeNavigation.Name, sortOrder: 3, mandatory: false, tab: _navigationConfiguration.NavigationTab);
        }

        protected override string Alias => _navigationConfiguration.HomeNavigationComposition.Alias;
        protected override string Name => _navigationConfiguration.HomeNavigationComposition.Name;
    }
}
