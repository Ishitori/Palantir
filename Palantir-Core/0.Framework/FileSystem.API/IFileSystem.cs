namespace Ix.Palantir.FileSystem.API
{
    using System.IO;

    public interface IFileSystem
    {
        string SaveToFile(string fileName, Stream stream);
        string SaveToFile(string fileName, byte[] bytes);
        Stream LoadFile(string virtualFilePath);
    }
}
