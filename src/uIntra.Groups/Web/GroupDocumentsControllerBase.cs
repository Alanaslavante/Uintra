﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using uIntra.Core;
using uIntra.Core.Extentions;
using uIntra.Core.User;
using uIntra.Groups.Sql;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace uIntra.Groups.Web
{
    [DisabledGroupActionFilter]
    public abstract class GroupDocumentsControllerBase : SurfaceController
    {
        protected virtual string DocumentsViewPath => "~/App_Plugins/Groups/Documents/GroupDocuments.cshtml";
        protected virtual string GroupDocumentsTableViewPath => "~/App_Plugins/Groups/Documents/GroupDocumentsTable.cshtml";

        private readonly IGroupDocumentsService _groupDocumentsService;
        private readonly IMediaService _mediaService;
        private readonly IIntranetUserService<IIntranetUser> _intranetUserService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IGroupService _groupService;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IGroupMediaService _groupMediaService;

        protected GroupDocumentsControllerBase(IGroupDocumentsService groupDocumentsService,
            IMediaService mediaService,
            IIntranetUserService<IIntranetUser> intranetUserService,
            IGroupMemberService groupMemberService,
            IGroupService groupService,
            UmbracoHelper umbracoHelper,
            IGroupMediaService groupMediaService)
        {
            _groupDocumentsService = groupDocumentsService;
            _mediaService = mediaService;
            _intranetUserService = intranetUserService;
            _groupMemberService = groupMemberService;
            _groupService = groupService;
            _umbracoHelper = umbracoHelper;
            _groupMediaService = groupMediaService;
        }

        public virtual ActionResult Documents(Guid groupId)
        {
            var groupMembers = _groupMemberService.GetGroupMemberByGroup(groupId);
            var currentUserId = _intranetUserService.GetCurrentUserId();
            var model = new GroupDocumentsViewModel
            {
                CanUploadFiles = groupMembers.Any(s => s.MemberId == currentUserId)
            };
            return PartialView(DocumentsViewPath, model);
        }

        [HttpPost]
        public void Upload(Guid groupId)
        {
            var file = Request.Files.Get(0);
            if (file != null && file.ContentLength > 0)
            {
                var bytes = file.InputStream.ToBytes();
                var media = _groupMediaService.CreateGroupMedia(file.FileName, bytes, groupId);

                var groupDocument = new GroupDocument
                {
                    GroupId = groupId,
                    MediaId = media.Id
                };
                _groupDocumentsService.Create(groupDocument);
            }
        }

        public virtual ActionResult DocumentsTable(Guid groupId, GroupDocumentDocumentField column, Direction direction)
        {
            var groupDocumentsList = _groupDocumentsService.GetByGroup(groupId).ToList();
            var mediaIdsList = groupDocumentsList.Select(s => s.MediaId).ToList();
            var medias = _mediaService.GetByIds(mediaIdsList);
            var group = _groupService.Get(groupId);
            var groupMembers = _groupMemberService.GetGroupMemberByGroup(groupId);
            var currentUser = _intranetUserService.GetCurrentUser();
            var docs = medias.Select(s =>
            {
                var intranetCreator = s.GetValue<Guid?>(IntranetConstants.IntranetCreatorId);
                var creator = intranetCreator.HasValue ? _intranetUserService.Get(intranetCreator.Value) : _intranetUserService.Get(s.CreatorId);
                var document = groupDocumentsList.First(f => f.MediaId == s.Id);
                var url = _umbracoHelper.TypedMedia(s.Id).Url;
                var model = new GroupDocumentTableRowViewModel
                {
                    CanDelete = CanDelete(currentUser, group, groupMembers, s),
                    Id = document.Id,
                    CreateDate = s.CreateDate,
                    Name = s.Name,
                    Type = s.GetValue<string>(UmbracoAliases.Media.MediaExtension),
                    CreatorName = creator.DisplayedName,
                    FileUrl = url
                };
                return model;
            });

            docs = Sort(docs, column, direction);

            var viewModel = new GroupDocumentsTableViewModel
            {
                Documents = docs,
                GroupId = groupId,
                Column = column,
                Direction = direction
            };
            return PartialView(GroupDocumentsTableViewPath, viewModel);
        }

        [HttpPost]
        public virtual ActionResult DeleteDocument(Guid groupId, Guid documentId, GroupDocumentDocumentField column, Direction direction)
        {
            var document = _groupDocumentsService.Get(documentId);
            if (document.GroupId != groupId)
            {
                throw new Exception("Can't delete document because it does not belong to this group!");
            }

            var currentUser = _intranetUserService.GetCurrentUser();
            var group = _groupService.Get(groupId);
            var groupMembers = _groupMemberService.GetGroupMemberByGroup(groupId);
            var media = _mediaService.GetById(document.MediaId);
            var canDelete = CanDelete(currentUser, @group, groupMembers, media);
            if (canDelete)
            {
                _mediaService.Delete(media);
                _groupDocumentsService.Delete(document);
            }

            return DocumentsTable(groupId, column, direction);
        }

        protected virtual bool CanDelete(IIntranetUser currentUser, Group group, IEnumerable<GroupMember> groupMembers, IMedia media)
        {
            var mediaCreator = media.GetValue<Guid?>(IntranetConstants.IntranetCreatorId);
            return currentUser.Id == group.CreatorId
                || mediaCreator.HasValue && mediaCreator.Value == currentUser.Id && groupMembers.Any(s => s.MemberId == currentUser.Id);
        }

        protected IEnumerable<GroupDocumentTableRowViewModel> Sort(IEnumerable<GroupDocumentTableRowViewModel> documents, GroupDocumentDocumentField column, Direction direction)
        {
            var keySelector = GetkeySelector(column);

            switch (direction)
            {
                case Direction.Asc:
                    documents = documents.OrderBy(keySelector);
                    break;
                case Direction.Desc:
                    documents = documents.OrderByDescending(keySelector);
                    break;
            }

            return documents;
        }

        protected Func<GroupDocumentTableRowViewModel, object> GetkeySelector(GroupDocumentDocumentField column)
        {
            Func<GroupDocumentTableRowViewModel, object> keySelector;

            switch (column)
            {
                case GroupDocumentDocumentField.Type:
                    keySelector = s => s.Type;
                    break;
                case GroupDocumentDocumentField.Date:
                    keySelector = s => s.CreateDate;
                    break;
                case GroupDocumentDocumentField.Creator:
                    keySelector = s => s.CreatorName;
                    break;
                default:
                    keySelector = s => s.Name;
                    break;
            }
            return keySelector;
        }
    }
}