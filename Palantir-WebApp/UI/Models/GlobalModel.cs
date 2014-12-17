namespace Ix.Palantir.UI.Models
{
    using System.Configuration;

    public static class GlobalModel
    {
        public static bool IsYandexMetricsEnabled
        {
            get
            {
                var reader = new AppSettingsReader();
                bool isYandexMetricsEnabled = (bool)reader.GetValue("YandexMetricsEnabled", typeof(bool));

                return isYandexMetricsEnabled;
            }
        }
    }
}