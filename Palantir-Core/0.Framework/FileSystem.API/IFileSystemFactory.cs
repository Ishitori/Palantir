namespace Ix.Palantir.FileSystem.API
{
    public interface IFileSystemFactory
    {
        IFileSystem CreateFileSystem(int userId);
    }
}