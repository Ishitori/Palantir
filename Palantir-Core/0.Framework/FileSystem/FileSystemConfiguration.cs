namespace Ix.Palantir.FileSystem
{
    using System.Xml.Serialization;
    using Ix.Palantir.FileSystem.API;

    [XmlRoot("file-system")]
    public class FileSystemConfiguration : IFileSystemConfiguration
    {
        [XmlAttribute("rootDir")]
        public string RootDir { get; set; }
    }
}