namespace Ix.Palantir.Engine.Configuration
{
    using System.Xml.Serialization;

    [XmlRoot("vk-credentials")]
    public class VkCredentials
    {
        [XmlAttribute("login")]
        public string Login { get; set; }
        [XmlAttribute("password")]
        public string Password { get; set; }
    }
}