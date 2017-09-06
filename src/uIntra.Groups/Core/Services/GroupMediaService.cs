﻿using System;
using System.Linq;
using uIntra.Core.Controls.FileUpload;
using uIntra.Core.Media;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace uIntra.Groups
{
    public class GroupMediaService : IGroupMediaService
    {
        private const string GroupIdPropertyTypeAlias = "GroupId";
        private readonly IMediaService _mediaService;
        private readonly IGroupService _groupService;
        private readonly IMediaHelper _mediaHelper;

        public GroupMediaService(IMediaService mediaService,
            IGroupService groupService, 
            IMediaHelper mediaHelper)
        {
            _mediaService = mediaService;
            _groupService = groupService;
            _mediaHelper = mediaHelper;
        }

        public void GroupTitleChanged(Guid groupId, string newTitle)
        {
            var groupFolder = GetOrCreateGroupMediaFolder(groupId);
            groupFolder.Name = newTitle;
            _mediaService.Save(groupFolder);
        }

        public IMedia CreateGroupMedia(string name, byte[] file, Guid groupId)
        {
            var groupFolder = GetOrCreateGroupMediaFolder(groupId);

            var fileModel = new TempFile
            {
                FileBytes = file,
                FileName = name
            };
            var media = _mediaHelper.CreateMedia(fileModel, groupFolder.Id);
            return media;
        }

        private IMedia GetOrCreateGroupMediaFolder(Guid groupId)
        {
            var medias = _mediaService.GetChildren(-1);
            var groupFolder = medias.FirstOrDefault(s =>
            {
                if (s.HasProperty(GroupIdPropertyTypeAlias))
                {
                    var id = s.GetValue<Guid?>(GroupIdPropertyTypeAlias);
                    return id.HasValue && id.Value == id;
                }
                return false;
            });

            if (groupFolder == null)
            {
                var group = _groupService.Get(groupId);
                groupFolder = _mediaService.CreateMedia(group.Title, -1, "Folder");
                groupFolder.SetValue(GroupIdPropertyTypeAlias, groupId.ToString());
                _mediaService.Save(groupFolder);
            }

            return groupFolder;
        }
    }
}