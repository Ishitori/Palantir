namespace Ix.Palantir.Utilities
{
    using System.Text;

    public interface IWebPageDownloader
    {
        string Cookie { get; set; }
        Encoding Encoding { get; set; }

        string DownloadPage(string url);
        string DownloadPageViaPost(string url, string postData);
    }
}