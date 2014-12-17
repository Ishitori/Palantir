namespace Ix.Palantir.DataAccess.API.Repositories
{
    using System.Collections.Generic;
    using Ix.Palantir.DomainModel;

    public interface IPhotoRepository
    {
        void Save(Photo photo);
        void UpdatePhoto(Photo savedPhoto);
        Photo GetPhotoByIdInAlbum(int vkGroupId, string vkAlbumId, string vkId);
        IList<string> GetGroupAlbumIds(int id);
    }
}