namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public interface IVideoRepository
    {
        void Save(Video video);
        void Update(Video video);
        void SaveComment(VideoComment comment);
        Video GetVideo(int vkGroupId, string vkId);
        VideoComment GetVideoCommentByVkGroupId(int vkGroupId, string vkId);
        IList<Video> GetVideosByVkGroupId(int vkGroupId);
    }
}