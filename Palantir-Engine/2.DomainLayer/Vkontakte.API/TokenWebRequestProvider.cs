namespace Ix.Palantir.Vkontakte.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Logging;
    using Utilities;

    public class TokenWebRequestProvider : ITokenProvider
    {
        private static readonly string CONST_LoginParameterRegexTemplate = "<input type=\"hidden\" name=\"{0}\" value=\"(.+)\"";
        private static readonly string CONST_ApproveFormUrlRegexTemplate = @"function allow\(.+?location\.href\s*=\s*""(.+?)""";
        private static readonly string CONST_CookieRegexTemplate = @"Set-Cookie:.+?{0}=(.+?);";
        private static readonly string CONST_AuthTokenTemplate = @"Response-Uri:\s+.+#access_token=(.+?)&";
        private static readonly string CONST_RedirectUrl = @"Location:\s+(.+)";

        private readonly string login;
        private readonly string password;
        private readonly WebPageDownloader downloader;
        private readonly ILog logger;

        public TokenWebRequestProvider(string login, string password)
        {
            this.login = login;
            this.password = password;
            this.downloader = new WebPageDownloader();
            this.downloader.Encoding = Encoding.GetEncoding(1251);
            this.logger = LogManager.GetLogger();
        }

        public string GetAccessToken()
        {
            string stringResponse;
            string authenticationFormUrl = this.GetAuthenticationFormUrl();
            IDictionary<string, string> loginParameters = this.CreateLoginParameters();
            IDictionary<string, string> cookieParameters = this.CreateCookieParameters();

            stringResponse = this.downloader.DownloadPage(authenticationFormUrl);
            this.logger.DebugFormat("Downloaded authentication page:\r\n{0}", stringResponse.ToUTF8(this.downloader.Encoding));

            this.FillCookieParameters(cookieParameters, stringResponse);
            this.FillLoginParameters(loginParameters, stringResponse);
            this.logger.DebugFormat("Login parameters are: {0}", loginParameters.ToUrlFormat());

            this.downloader.Cookie = cookieParameters.ToCookieFormat();
            this.downloader.AllowAutoRedirect = false;
            stringResponse = this.SubmitLoginForm(loginParameters);
            this.logger.DebugFormat("Submiting login form returned:\r\n{0}", stringResponse.ToUTF8(this.downloader.Encoding));

            this.FillCookieParameters(cookieParameters, stringResponse);
            string redirectUrl = this.GetRedirectUrlAfterLogin(stringResponse);

            this.downloader.Cookie = cookieParameters.ToCookieFormat();
            this.downloader.AllowAutoRedirect = true;
            stringResponse = this.downloader.DownloadPage(redirectUrl);
            this.logger.DebugFormat("Redirecting to aproving url returned:\r\n{0}", stringResponse.ToUTF8(this.downloader.Encoding));

            string approveUrl = this.GetApproveUrl(stringResponse);
            this.logger.DebugFormat("Fetched approve url: {0}", approveUrl);

            if (!string.IsNullOrWhiteSpace(approveUrl))
            {
                this.FillCookieParameters(cookieParameters, stringResponse);
                this.downloader.Cookie = cookieParameters.ToCookieFormat();
                this.logger.DebugFormat("Cookie parameters are: {0}", cookieParameters.ToCookieFormat());

                stringResponse = this.downloader.DownloadPage(approveUrl + "&notify=1");
                this.logger.DebugFormat("Approve url request returned:\r\n{0}", stringResponse.ToUTF8(this.downloader.Encoding));
            }

            string accessToken = this.GetAccessToken(stringResponse);
            this.logger.DebugFormat("Access token is: {0}", accessToken);

            return accessToken;
        }

        private IDictionary<string, string> CreateLoginParameters()
        {
            IDictionary<string, string> loginParameters = new Dictionary<string, string>();
         
            loginParameters.Add("ip_h", string.Empty);
            loginParameters.Add("_origin", string.Empty);
            loginParameters.Add("expire", string.Empty);
            loginParameters.Add("to", string.Empty);

            return loginParameters;
        }
        private IDictionary<string, string> CreateCookieParameters()
        {
            IDictionary<string, string> cookieParameters = new Dictionary<string, string>();
            cookieParameters.Add("remixdt", "0");
            cookieParameters.Add("remixlang", "0");
            cookieParameters.Add("remixflash", "11.4.31");
            cookieParameters.Add("remixseenads", "2");
            cookieParameters.Add("s", string.Empty);
            cookieParameters.Add("l", string.Empty);
            cookieParameters.Add("p", string.Empty);
            cookieParameters.Add("h", string.Empty);
            cookieParameters.Add("remixsid", string.Empty);

            return cookieParameters;
        }
     
        private void FillLoginParameters(IDictionary<string, string> loginParametersDictionary, string response)
        {
            foreach (var parameterKey in loginParametersDictionary.Keys.ToList())
            {
                string parameterValue = Regex.Match(response, string.Format(CONST_LoginParameterRegexTemplate, parameterKey), RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value;
                loginParametersDictionary[parameterKey] = parameterValue;
            }

            loginParametersDictionary.Add("email", this.login);
            loginParametersDictionary.Add("pass", this.password);
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
        
        private string SubmitLoginForm(IDictionary<string, string> loginParametersDictionary)
        {
            string postData = loginParametersDictionary.ToUrlFormat();
            string result = this.downloader.DownloadPageViaPost("https://login.vk.com/?act=login&soft=1", postData);

            return result;
        }
        
        private string GetAuthenticationFormUrl()
        {
            int scopeCode = 0;

            foreach (int accessRule in Enum.GetValues(typeof(SecurityAccessRule)))
            {
                scopeCode |= accessRule;
            }

            string authUrl = string.Format("http://api.vk.com/oauth/authorize?client_id=2695184&scope={0}&redirect_uri=http://oauth.vk.com/blank.html&display=page&response_type=token", scopeCode);
            return authUrl;
        }
        private string GetApproveUrl(string response)
        {
            string approveUrl = Regex.Match(response, CONST_ApproveFormUrlRegexTemplate, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline).Groups[1].Value;
            return approveUrl;
        }
        private string GetAccessToken(string response)
        {
            string rawAuthToken = Regex.Match(response, CONST_AuthTokenTemplate, RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value;
            string authToken = Regex.Replace(rawAuthToken, "[^a-zA-Z0-9\\-_=&]", string.Empty);
            return authToken;
        }
        private string GetRedirectUrlAfterLogin(string response)
        {
            string redurectUrl = Regex.Match(response, CONST_RedirectUrl, RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value;
            return redurectUrl;
        }
    }
}