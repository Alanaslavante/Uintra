﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using uIntra.Core.Extentions;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace uIntra.Core.Controls.LightboxGallery
{
    public abstract class LightboxGalleryControllerBase : SurfaceController
    {
        protected virtual string GalleryViewPath { get; } = "~/App_Plugins/Core/Controls/LightBoxGallery/LightboxGallery.cshtml";
        protected virtual string PreviewViewPath { get; } = "~/App_Plugins/Core/Controls/LightBoxGallery/LightboxGalleryPreview.cshtml";

        private readonly UmbracoHelper _umbracoHelper;

        protected LightboxGalleryControllerBase(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        public virtual ActionResult RenderGallery(string mediaIds)
        {
            var model = GetGalleryOverviewModel(mediaIds);
            return View(GalleryViewPath, model);
        }

        public virtual ActionResult Preview(LightboxGalleryPreviewModel model)
        {
            if (model.MediaIds.IsEmpty())
            {
                return new EmptyResult();
            }

            var medias = _umbracoHelper.TypedMedia(model.MediaIds).ToList();
            if (medias.IsEmpty())
            {
                return new EmptyResult();
            }

            var galleryPreviewModel = GetGalleryPreviewModel(model, medias);
            return PartialView(PreviewViewPath, galleryPreviewModel);
        }

        protected virtual LightboxGalleryOverviewModel GetGalleryOverviewModel(string mediaIds)
        {
            var result = new LightboxGalleryOverviewModel();
            if (mediaIds.IsNullOrEmpty())
            {
                return result;
            }

            var ids = mediaIds.ToIntCollection();
            var medias = _umbracoHelper.TypedMedia(ids).ToList();            
            result.GalleryItems = medias.Select(MapToMedia).OrderBy(s => s.Type.Id);

            return result;
        }

        protected virtual LightboxGalleryItemViewModel MapToMedia(IPublishedContent media)
        {
            var result = new LightboxGalleryItemViewModel()
            {
                Id = media.Id,
                Url = media.Url,
                Name = media.GetFileName(),
                Extention = media.GetMediaExtention(),
                Type = media.GetMediaType(),
            };

            if (result.Type.Id == MediaTypeEnum.Image.ToInt())
            {
                result.Height = media.GetPropertyValue<int>(UmbracoAliases.Media.MediaHeight);
                result.Width = media.GetPropertyValue<int>(UmbracoAliases.Media.MediaWidth);
                result.PreviewUrl = media.GetCropUrl(UmbracoAliases.GalleryPreviewImageCrop);
            }
            return result;
        }

        protected virtual LightboxGalleryPreviewViewModel GetGalleryPreviewModel(LightboxGalleryPreviewModel model, IEnumerable<IPublishedContent> medias)
        {
            var galleryPreviewModel = model.Map<LightboxGalleryPreviewViewModel>();
            var galleryViewModelList = medias.Map<List<LightboxGalleryItemViewModel>>();

            galleryPreviewModel.Images = galleryViewModelList.FindAll(m => m.Type.Id == MediaTypeEnum.Image.ToInt());
            galleryPreviewModel.OtherFiles = galleryViewModelList.FindAll(m => m.Type.Id != MediaTypeEnum.Image.ToInt());
            galleryPreviewModel.Images.Skip(model.DisplayedImagesCount).ToList().ForEach(i => i.IsHidden = true);

            return galleryPreviewModel;
        }
    }
}