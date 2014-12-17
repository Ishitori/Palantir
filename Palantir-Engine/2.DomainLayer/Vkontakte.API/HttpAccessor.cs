namespace Ix.Palantir.Vkontakte.API
{
    using System.Text;
    using Utilities;

    public class HttpAccessor : IHttpAccessor
    {
        private readonly ICookieProvider cookieProvider;
        private readonly IWebPageDownloader pageDownloader;

        private string accessCookie;

        public HttpAccessor(ICookieProvider cookieProvider, IWebPageDownloader pageDownloader)
        {
            this.cookieProvider = cookieProvider;
            this.pageDownloader = pageDownloader;
            this.pageDownloader.Encoding = Encoding.GetEncoding("windows-1251");
        }

        private string AccessCookie
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.accessCookie))
                {
                    this.accessCookie = this.cookieProvider.GetAccessCookie();
                }

                return this.accessCookie;
            }
        }
        
        public string GetPageByUri(string pageUri)
        {
            this.pageDownloader.Cookie = this.AccessCookie;
            string page = this.pageDownloader.DownloadPage(pageUri);

            return page;
        }
        public string GetPageByUriViaPost(string pageUri, string postData)
        {
            this.pageDownloader.Cookie = this.AccessCookie;
            string page = this.pageDownloader.DownloadPageViaPost(pageUri, postData);

            return page;
        }
    }
}