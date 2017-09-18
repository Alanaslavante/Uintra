using System;
using System.Collections.Generic;

namespace uIntra.Groups
{
    public interface IGroupActivityService
    {
        void AddRelation(Guid groupId, Guid activityId);
        void RemoveRelation(Guid groupId, Guid activityId);
        Guid GetGroupId(Guid activityId);
    }
}