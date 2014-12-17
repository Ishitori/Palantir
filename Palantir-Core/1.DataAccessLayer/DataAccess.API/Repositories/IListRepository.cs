namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System;
    using System.Collections.Generic;

    public interface IListRepository
    {
        IList<string> GetPostVkIds(int vkGroupId, DateTime? dateLimit);
        IList<string> GetPostCommentVkIds(int vkGroupId, DateTime? dateLimit);
        IList<string> GetPhotoVkIds(int vkGroupId, DateTime? dateLimit);
        IList<string> GetVideoVkIds(int vkGroupId, DateTime? dateLimit);
        IList<long> GetMemberVkIds(int vkGroupId);
    }
}