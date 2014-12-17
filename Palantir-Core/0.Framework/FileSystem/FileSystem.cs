namespace Ix.Palantir.FileSystem
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Ix.Palantir.FileSystem.API;

    public class FileSystem : IFileSystem
    {
        private static readonly Regex AllowedChars = new Regex(@"[^\d\w\-_\!\. \(\)\[\]\,\'\’\;\@\~\$\{\}\=\`\^\/]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly int userId;
        private readonly IFileSystemConfiguration configuration;

        public FileSystem(int userId, IFileSystemConfiguration configuration)
        {
            this.userId = userId;
            this.configuration = configuration;
        }

        public string SaveToFile(string fileName, Stream stream)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(fileName));
            Contract.Requires(stream != null);

            fileName = this.ToSafeFileName(fileName);
            string virtualFilePath = this.GetVirtualFilePath(fileName);
            string filePath = this.GetFullPhysicalFilePath(virtualFilePath);

            using (StreamReader reader = new StreamReader(stream))
            {
                File.WriteAllText(filePath, reader.ReadToEnd(), Encoding.UTF8);
            }

            return virtualFilePath;
        }
        public string SaveToFile(string fileName, byte[] bytes)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(fileName));
            Contract.Requires(bytes != null);

            fileName = this.ToSafeFileName(fileName);
            string virtualFilePath = this.GetVirtualFilePath(fileName);
            string filePath = this.GetFullPhysicalFilePath(virtualFilePath);

            File.WriteAllBytes(filePath, bytes);

            return virtualFilePath;
        }
        public Stream LoadFile(string virtualFilePath)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(virtualFilePath));
            string filePath = this.GetFullPhysicalFilePath(virtualFilePath);

            return File.OpenRead(filePath);
        }

        private string ToSafeFileName(string fileName)
        {
            string replaced = AllowedChars.Replace(fileName, "_");
            return replaced;
        }

        private string GetVirtualFilePath(string fileName)
        {
            return string.Format("{0}\\{1}", this.userId, fileName);
        }
        private string GetFullPhysicalFilePath(string virtualFilePath)
        {
            string fileSystemRoot = Environment.ExpandEnvironmentVariables(this.configuration.RootDir);
            return Path.Combine(fileSystemRoot, virtualFilePath);
        }
    }
}
