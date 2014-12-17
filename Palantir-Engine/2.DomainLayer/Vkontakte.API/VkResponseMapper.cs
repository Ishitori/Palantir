namespace Ix.Palantir.Vkontakte.API
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public class VkResponseMapper : IVkResponseMapper
    {
        public T MapResponse<T>(string responseString, bool parseSafely = false) where T : new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            try
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
                {
                    T result = (T)serializer.Deserialize(stream);
                    return result;
                }
            }
            catch (Exception exc)
            {
                if (!parseSafely)
                {
                    throw new ArgumentException(string.Format("Unable to parse a response:\r\n{0}", responseString), exc);
                }

                return new T();
            }
        }

        public string MapResponseObject<T>(T responseObject, bool parseSafely = true)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            try
            {
                MemoryStream stream = new MemoryStream();

                using (StreamReader reader = new StreamReader(stream))
                {
                    serializer.Serialize(stream, responseObject);
                    stream.Position = 0;
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception exc)
            {
                if (!parseSafely)
                {
                    throw new ArgumentException(string.Format("Unable to serialize an object:\r\n{0}", responseObject.GetType()), exc);
                }

                return string.Empty;
            }
        }
    }
}