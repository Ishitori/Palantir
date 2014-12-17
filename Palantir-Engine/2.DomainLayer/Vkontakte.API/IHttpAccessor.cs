namespace Ix.Palantir.Vkontakte.API
{
    public interface IHttpAccessor
    {
        string GetPageByUri(string pageUri);
        string GetPageByUriViaPost(string pageUri, string postData);
    }
}