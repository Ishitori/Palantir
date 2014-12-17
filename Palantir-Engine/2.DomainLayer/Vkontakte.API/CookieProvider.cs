namespace Ix.Palantir.Vkontakte.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Ix.Palantir.Utilities;

    public class CookieProvider : ICookieProvider
    {
        private const string CONST_SsIdRegexTemplate = @"sid=([a-z0-9]+); exp";
        private static readonly string CONST_RedirectUrl = @"Location:\s+(.+)";
        private static readonly string CONST_CookieRegexTemplate = @"Set-Cookie:.+?{0}=(.+?);";

        private readonly string login;
        private readonly string password;
        private readonly WebPageDownloader downloader;

        public CookieProvider(string login, string password)
        {
            this.login = login;
            this.password = password;
            this.downloader = new WebPageDownloader();
            this.downloader.Encoding = Encoding.GetEncoding(1251);
        }

        public string GetAccessCookie()
        {
            string stringResponse;
            string authenticationFormUrl = this.GetAuthenticationFormUrl();
            IDictionary<string, string> loginParameters = this.CreateLoginParameters();
            IDictionary<string, string> cookieParameters = this.CreateCookieParameters();

            this.downloader.AllowAutoRedirect = false;
            this.downloader.Cookie = cookieParameters.ToCookieFormat();
            stringResponse = this.downloader.DownloadPageViaPost(authenticationFormUrl, loginParameters.ToUrlFormat());

            this.FillCookieParameters(cookieParameters, stringResponse);
            var confirmLoginUrl = this.GetRedirectUrl(stringResponse);
            this.downloader.Cookie = cookieParameters.ToCookieFormat();
            stringResponse = this.downloader.DownloadPage(confirmLoginUrl);

            this.FillCookieParameters(cookieParameters, stringResponse);
            return string.Format("remixdt=-28800; remixlang=0; remixexp=1; remixsid={0}; audio_vol=100; remixseenads=1; remixflash=11.7.700;", cookieParameters["remixsid"]);
        }

        private string GetAuthenticationFormUrl()
        {
            string authUrl = string.Format("https://login.vk.com/?act=login");
            return authUrl;
        }
        private string GetRedirectUrl(string response)
        {
            string redurectUrl = Regex.Match(response, CONST_RedirectUrl, RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value;
            return redurectUrl;
        }

        private IDictionary<string, string> CreateLoginParameters()
        {
            IDictionary<string, string> loginParameters = new Dictionary<string, string>();

            loginParameters.Add("captcha_sid", string.Empty);
            loginParameters.Add("captcha_key", string.Empty);
            loginParameters.Add("role", "al_frame");
            loginParameters.Add("act", "login");
            loginParameters.Add("_origin", "http://vk.com");
            loginParameters.Add("email", this.login);
            loginParameters.Add("pass", this.password);
            loginParameters.Add("expire", string.Empty);
            //// loginParameters.Add("ip_h", "64e455216f5f33349a");

            return loginParameters;
        }
        private IDictionary<string, string> CreateCookieParameters()
        {
            IDictionary<string, string> cookieParameters = new Dictionary<string, string>();
            cookieParameters.Add("remixdt", "-28800");
            cookieParameters.Add("remixlang", "0");
            cookieParameters.Add("remixflash", "11.4.31");
            cookieParameters.Add("remixseenads", "2");
            cookieParameters.Add("s", string.Empty);
            cookieParameters.Add("l", string.Empty);
            cookieParameters.Add("p", string.Empty);
            cookieParameters.Add("h", string.Empty);
            cookieParameters.Add("remixsid", string.Empty);
            cookieParameters.Add("remixreg_sid", string.Empty);
            cookieParameters.Add("remixapi_sid", string.Empty);
            cookieParameters.Add("remixrec_sid", string.Empty);

            return cookieParameters;
        }
        private void FillCookieParameters(IDictionary<string, string> cookieParameters, string response)
        {
            foreach (var cookieParameter in cookieParameters.Keys.ToList())
            {
                string parameterValue = Regex.Match(response, string.Format(CONST_CookieRegexTemplate, cookieParameter), RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value;

                if (!string.IsNullOrWhiteSpace(parameterValue))
                {
                    cookieParameters[cookieParameter] = parameterValue;
                }
            }
        }
    }
}