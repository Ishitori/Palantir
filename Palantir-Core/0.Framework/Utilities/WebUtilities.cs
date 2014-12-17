namespace Ix.Palantir.Utilities
{
    using System;
    using System.DirectoryServices;
    using Ix.Palantir.Logging;

    public class WebUtilities : IWebUtilities
    {
        private readonly ILog log;
        private string applicationPool;

        public WebUtilities(ILog log)
        {
            this.applicationPool = string.Empty;
            this.log = log;
        }

        public string GetServerName()
        {
            return Environment.MachineName;
        }

        public string GetApplicationPoolName()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.applicationPool))
                {
                    return this.applicationPool;
                }

                string virtualDirPath = AppDomain.CurrentDomain.FriendlyName;
                virtualDirPath = virtualDirPath.Substring(4);
                int index = virtualDirPath.Length + 1;
                index = virtualDirPath.LastIndexOf("-", index - 1, index - 1, StringComparison.Ordinal);
                index = virtualDirPath.LastIndexOf("-", index - 1, index - 1, StringComparison.Ordinal);
                virtualDirPath = "IIS://localhost/" + virtualDirPath.Remove(index);

                var virtualDirEntry = new DirectoryEntry(virtualDirPath);
                this.applicationPool = virtualDirEntry.Properties["AppPoolId"].Value.ToString();
                return this.applicationPool;
            }
            catch (Exception exc)
            {
                this.log.Error(exc.ToString());
                this.applicationPool = "n/a";
                return this.applicationPool;
            }
        }
    }
}