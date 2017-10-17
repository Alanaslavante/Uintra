﻿using System.Web;
using uIntra.Core;
using uIntra.Core.Extentions;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace uIntra.Navigation
{
    public static class PublishedContentExtentions
    {
        public static bool GetHideInNavigation(this IPublishedContent content)
        {
            var docTypeAliasProvider = HttpContext.Current.GetService<IDocumentTypeAliasProvider>();

            if (content.IsComposedOf(docTypeAliasProvider.GetNavigationComposition()))
            {
                var isHideFromLeftNavigationPropName = content.GetPropertyValue<bool>(NavigationPropertiesConstants.IsHideFromLeftNavigationPropName);

                return isHideFromLeftNavigationPropName;
            }

            return true;
        }

        public static bool IsShowPageInSubNavigation(this IPublishedContent content)
        {
            var docTypeAliasProvider = HttpContext.Current.GetService<IDocumentTypeAliasProvider>();
            
            if (content.IsComposedOf(docTypeAliasProvider.GetNavigationComposition()))
            {
                var isHideFromSubNavigation = content.GetPropertyValue<bool>(NavigationPropertiesConstants.IsHideFromSubNavigationPropName);

                return !isHideFromSubNavigation;
            }

            return true;
        }

        public static string GetNavigationName(this IPublishedContent content)
        {
            var docTypeAliasProvider = HttpContext.Current.GetService<IDocumentTypeAliasProvider>();
            if (content.IsComposedOf(docTypeAliasProvider.GetNavigationComposition()))
            {
                var navigationName = content.GetPropertyValue<string>(NavigationPropertiesConstants.NavigationNamePropName);
                
                return navigationName;
            }

            return string.Empty;
        }

        public static bool IsHeading(this IPublishedContent publishedContent)
        {
            var docTypeAliasProvider = HttpContext.Current.GetService<IDocumentTypeAliasProvider>();
            return publishedContent.DocumentTypeAlias.Equals(docTypeAliasProvider.GetHeading());
        }

        public static bool IsContentPage(this IPublishedContent publishedContent)
        {
            var docTypeAliasProvider = HttpContext.Current.GetService<IDocumentTypeAliasProvider>();
            return publishedContent.DocumentTypeAlias.Equals(docTypeAliasProvider.GetContentPage());
        }
    }
}