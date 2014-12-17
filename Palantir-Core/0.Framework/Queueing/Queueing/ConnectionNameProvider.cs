namespace Ix.Palantir.Queueing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ix.Palantir.Queueing.API;

    public class ConnectionNameProvider : IConnectionNameProvider
    {
        private static readonly IList<string> disallowedApplicationNames = new List<string> { "bin", "obj" };

        public string GetConnectionName()
        {
            string machineName = this.GetMachineName();
            string applicationName = this.GetApplicationName();
            string uniquePart = Guid.NewGuid().ToString();

            return string.Format("{0}|{1}|{2}", machineName, applicationName, uniquePart);
        }

        private string GetMachineName()
        {
            return System.Environment.MachineName;
        }
        private string GetApplicationName()
        {
            string curDomainBaseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (curDomainBaseDir.Length > 0)
            {
                curDomainBaseDir = curDomainBaseDir.Remove(curDomainBaseDir.Length - 1);
            }

            int index = curDomainBaseDir.LastIndexOf("\\");

            while (index > -1)
            {
                string nameOption = curDomainBaseDir.Substring(index + 1);

                if (this.IsAllowedName(nameOption))
                {
                    return nameOption;
                }

                curDomainBaseDir = curDomainBaseDir.Remove(index);
                index = curDomainBaseDir.LastIndexOf("\\");
            }

            return curDomainBaseDir;
        }

        private bool IsAllowedName(string nameOption)
        {
            return !disallowedApplicationNames.Any(disallowedName => string.Compare(disallowedName, nameOption, true) == 0);
        }
    }
}