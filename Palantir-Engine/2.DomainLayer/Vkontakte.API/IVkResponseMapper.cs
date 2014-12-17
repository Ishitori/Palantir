namespace Ix.Palantir.Vkontakte.API
{
    public interface IVkResponseMapper
    {
        T MapResponse<T>(string responseString, bool parseSafely = false) where T : new();
        string MapResponseObject<T>(T responseObject, bool parseSafely = true);
    }
}