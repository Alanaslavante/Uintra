﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using uIntra.Core.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace uIntra.Core.Installer
{
    public class InstallationStepsHelper
    {
        public static ContentType GetBasePageWithGridBase(string basePageTypeAlias)
        {
            var contentService = ApplicationContext.Current.Services.ContentTypeService;
            var fileService = ApplicationContext.Current.Services.FileService;

            var basePageWithGrid = contentService.GetContentType(basePageTypeAlias);
            var basePageWithGridBase = new ContentType(basePageWithGrid.Id);

            basePageWithGridBase.AddContentType(basePageWithGrid);
            basePageWithGridBase.SetDefaultTemplate(fileService.GetTemplate(CoreInstallationConstants.DocumentTypeAliases.GridPageLayoutTemplateAlias));

            return basePageWithGridBase;
        }

        public static IContentType CreatePageDocTypeWithBaseGrid(BasePageWithDefaultGridCreateModel model)
        {
            if (!ValidateCreationModel(model))
            {
                return null;
            }
            var contentService = ApplicationContext.Current.Services.ContentTypeService;

            var page = contentService.GetContentType(model.Alias);
            if (page != null) return null;

            page = GetBasePageWithGridBase(CoreInstallationConstants.DocumentTypeAliases.BasePageWithGrid);

            page.Name = model.Name;
            page.Alias = model.Alias;
            page.Icon = model.Icon;

            contentService.Save(page);
            if (model.ParentAlias.IsNotNullOrEmpty())
            {
                AddAllowedChildNode(model.ParentAlias, model.Alias);
            }

            return page;
        }

        public static void AddAllowedChildNode(string parentDocumentTypeAlias, string childDocumentTypeAlias)
        {
            var contentService = ApplicationContext.Current.Services.ContentTypeService;
            var parentNodeDataType = contentService.GetContentType(parentDocumentTypeAlias);
            var childNodeDataType = contentService.GetContentType(childDocumentTypeAlias);
            var allowedChildren = parentNodeDataType.AllowedContentTypes.ToList();
            var isChildAlready = allowedChildren.Any(c => c.Id.Value == childNodeDataType.Id);
            if (isChildAlready)
            {
                return;
            }

            allowedChildren.Add(new ContentTypeSort(childNodeDataType.Id, 1));
            parentNodeDataType.AllowedContentTypes = allowedChildren;

            contentService.Save(parentNodeDataType);
        }

        private static bool ValidateCreationModel(BasePageWithDefaultGridCreateModel model)
        {
            var context = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(model, context, validationResults, true);
        }

        public static string GetEmbeddedResourceValue(string embeddedResourceName, Assembly sourceAssembly = null)
        {
            var assembly = sourceAssembly != null ? sourceAssembly : Assembly.GetCallingAssembly();
            string json;
            using (Stream stream = assembly.GetManifestResourceStream(embeddedResourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource {embeddedResourceName} doesn't exist.");
                }
                using (TextReader reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
            }

            return json;
        }

        public static void InheritCompositionForPage(string pageTypeAlias, string compositionTypeAlias)
        {
            var contentService = ApplicationContext.Current.Services.ContentTypeService;

            var page = contentService.GetContentType(pageTypeAlias);
            var composition = contentService.GetContentType(compositionTypeAlias);
            if (page == null || composition == null) return;

            if (page.ContentTypeCompositionExists(composition.Alias)) return;

            page.AddContentType(composition);
            contentService.Save(page);
        }

        public static void CreateTrueFalseDataType(string name)
        {
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

            var dataType = dataTypeService.GetDataTypeDefinitionByName(name);
            if (dataType == null)
            {
                dataType = new DataTypeDefinition("Umbraco.TrueFalse")
                {
                    Name = name
                };

                dataTypeService.Save(dataType);
            }
        }

        public static void CreateGrid(string dataTypeName, string gridEmbeddedResourceFileName, Assembly sourceAssembly = null)
        {
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            var defaultGridDataType = dataTypeService.GetDataTypeDefinitionByName(dataTypeName);
            if (defaultGridDataType != null) return;

            var gridJson = GetEmbeddedResourceValue(gridEmbeddedResourceFileName, sourceAssembly);

            var jsonPrevalues = JObject.Parse(gridJson);
            var preValueItemsAlias = CoreInstallationConstants.DataTypePropertyPreValues.DefaultGridItems;
            var preValueRteAlias = CoreInstallationConstants.DataTypePropertyPreValues.DefaultGridRte;
            defaultGridDataType = new DataTypeDefinition(-1, "Umbraco.Grid")
            {
                Name = dataTypeName
            };
            var preValues = new Dictionary<string, PreValue>
            {
                { preValueItemsAlias, new PreValue(jsonPrevalues.Property(preValueItemsAlias).Value.ToString())},
                { preValueRteAlias, new PreValue(jsonPrevalues.Property(preValueRteAlias).Value.ToString())}
            };
            dataTypeService.SaveDataTypeAndPreValues(defaultGridDataType, preValues);
        }

        public static PropertyType GetGridPropertyType(string gridTypeName)
        {
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            var defaultGridDataType = dataTypeService.GetDataTypeDefinitionByName(gridTypeName);
            var gridProperty = new PropertyType(defaultGridDataType)
            {
                Name = CoreInstallationConstants.DataTypePropertyNames.Grid,
                Alias = CoreInstallationConstants.DataTypePropertyAliases.Grid
            };

            return gridProperty;
        }
    }
}
