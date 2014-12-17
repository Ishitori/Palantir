namespace Ix.Palantir.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlRoot("caching")]
    public class CachingConfig
    {
        [XmlAttribute("enabled")]
        public bool Enabled { get; set; }

        [XmlArray("interfaces")]
        [XmlArrayItem("interface", typeof(CachingSettings))]
        public List<CachingSettings> Interfaces { get; set; }

        public bool IsCachingEnabledFor(Type interfaceType)
        {
            if (!this.Enabled)
            {
                return false;
            }

            CachingSettings cachingInfo = this.GetCachingSettings(interfaceType);
            return cachingInfo != null && cachingInfo.Enabled;
        }
        public CachingSettings GetCachingSettings(Type interfaceType)
        {
            string name = interfaceType.Name;
            CachingSettings cachingInfo = this.Interfaces.FirstOrDefault(x => string.Compare(x.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0);
            return cachingInfo;
        }
    }
}