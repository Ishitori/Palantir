namespace Ix.Palantir.Vkontakte.API
{
    using System.Collections.Generic;

    public interface IVkAccessor
    {
        string ExecuteCall(string apiMethodName, Dictionary<string, string> parameters);
        T ExecuteCall<T>(string apiMethodName, Dictionary<string, string> parameters) where T : class, new();
    }
}