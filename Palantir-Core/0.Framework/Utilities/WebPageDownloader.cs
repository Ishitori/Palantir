namespace Ix.Palantir.Utilities
{
    using System.IO;
    using System.Net;
    using System.Text;

    public class WebPageDownloader : IWebPageDownloader
    {
        public WebPageDownloader()
        {
            this.Encoding = Encoding.UTF8;
            this.AllowAutoRedirect = true;
        }

        public string Cookie
        {
            get; set;
        }
        public Encoding Encoding
        {
            get; set;
        }

        public bool AllowAutoRedirect { get; set; }

        public string DownloadPage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");
            request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.75 Safari/535.7";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.Headers.Set(HttpRequestHeader.AcceptCharset, "windows-1251,utf-8;q=0.7,*;q=0.3");
            request.Headers.Set(HttpRequestHeader.Cookie, this.Cookie);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.AllowAutoRedirect = this.AllowAutoRedirect;

            using (WebResponse response = this.MakeRequest(request))
            {
                Stream pageStream = response.GetResponseStream();
                
                using (var pageContentReader = new StreamReader(pageStream, this.Encoding))
                {
                    string responseUri = response.ResponseUri.AbsoluteUri;
                    string pageHeaders = this.GetPageHeaders(response);
                    string pageContent = pageContentReader.ReadToEnd();

                    return string.Format("Response-Uri: {0}\r\n{1}\r\n\r\n{2}", responseUri, pageHeaders, pageContent);
                }
            }
        }

        public string DownloadPageViaPost(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.75 Safari/535.7";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.Headers.Set(HttpRequestHeader.AcceptCharset, "windows-1251,utf-8;q=0.7,*;q=0.3");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.AllowAutoRedirect = this.AllowAutoRedirect;

            if (!string.IsNullOrWhiteSpace(this.Cookie))
            {
                request.Headers.Set(HttpRequestHeader.Cookie, this.Cookie);
            }

            request.Method = "POST";

            string postString = postData;
            byte[] postBytes = Encoding.UTF8.GetBytes(postString);
            request.ContentLength = postBytes.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();

            using (WebResponse response = request.GetResponse())
            {
                Stream pageStream = response.GetResponseStream();

                using (StreamReader pageContentReader = new StreamReader(pageStream, this.Encoding))
                {
                    string responseUri = response.ResponseUri.AbsoluteUri;
                    string pageHeaders = this.GetPageHeaders(response);
                    string pageContent = pageContentReader.ReadToEnd();

                    return string.Format("Response-Uri: {0}\r\n{1}\r\n\r\n{2}", responseUri, pageHeaders, pageContent);
                }
            }
        }

        private string GetPageHeaders(WebResponse response)
        {
            SeparatedStringBuilder headersBuilder = new SeparatedStringBuilder("\r\n");

            for (int i = 0; i < response.Headers.Count; i++)
            {
                string key = response.Headers.GetKey(i);
                SeparatedStringBuilder values = new SeparatedStringBuilder(",", response.Headers.GetValues(i));
                headersBuilder.AppendFormatWithSeparator("{0}: {1}", key, values.ToString());
            }

            return headersBuilder.ToString();
        }

        private HttpWebResponse MakeRequest(HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    return (HttpWebResponse)we.Response;
                }

                throw;
            }
        }
    }
}
