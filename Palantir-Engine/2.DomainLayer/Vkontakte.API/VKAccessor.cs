namespace Ix.Palantir.Vkontakte.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Ix.Palantir.Exceptions;
    using Ix.Palantir.Logging;
    using Utilities;

    public class VkAccessor : IVkAccessor
    {
        public static readonly string CONST_ApiUrl = "https://api.vk.com/method";

        private static readonly VkTimeQuantinizer Quantinizer;
        private static readonly int CONST_MaxHeartBeatTimeout = 10000;

        private readonly string accessToken;
        private readonly IVkResponseMapper responseMapper;

        static VkAccessor()
        {
            Quantinizer = new VkTimeQuantinizer();
            Quantinizer.StartHeartBeat();
        }
        public VkAccessor(string accessToken, IVkResponseMapper responseMapper)
        {
            this.accessToken = accessToken;
            this.responseMapper = responseMapper;
        }

        public string ExecuteCall(string apiMethodName, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(apiMethodName))
            {
                throw new ArgumentException("Name of API method should be provided", "apiMethodName");
            }

            if (!Quantinizer.StartVkRequest(CONST_MaxHeartBeatTimeout))
            {
                LogManager.GetLogger("VkAccessor").WarnFormat("Heartbeat signal haven't been received in {0} ms. Something is wrong with heartbeat thread", CONST_MaxHeartBeatTimeout);
            }

            try
            {
                string response = this.DoRequest(apiMethodName, parameters);
                return response;
            }
            finally
            {
                Quantinizer.FinishVkRequest();
            }
        }

        public T ExecuteCall<T>(string apiMethodName, Dictionary<string, string> parameters) where T : class, new()
        {
            string callResult = this.ExecuteCall(apiMethodName, parameters);

            if (string.IsNullOrWhiteSpace(callResult))
            {
                return null;
            }

            return this.responseMapper.MapResponse<T>(callResult);
        }

        private string DoRequest(string apiMethodName, Dictionary<string, string> parameters)
        {
            string requestUri = this.GetRequestUrl(apiMethodName, parameters);
            var webRequest = WebRequest.Create(requestUri);

            try
            {
                var webResponse = (HttpWebResponse)webRequest.GetResponse();
                var resStream = webResponse.GetResponseStream();

                if (resStream == null || webResponse.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (SocketException exc)
            {
                throw new PalantirException("Exception while connecting to VKontakte.API", exc);
            }
        }
        private string GetRequestUrl(string apiMethodName, Dictionary<string, string> parameters)
        {
            SeparatedStringBuilder requestUrlBuilder = new SeparatedStringBuilder("&", string.Format("{0}/{1}?", CONST_ApiUrl, apiMethodName));
            parameters.Add("access_token", this.accessToken);

            foreach (var parameter in parameters)
            {
                requestUrlBuilder.AppendFormatWithSeparator("{0}={1}", parameter.Key, parameter.Value);
            }

            return requestUrlBuilder.ToString();
        }
    }
}