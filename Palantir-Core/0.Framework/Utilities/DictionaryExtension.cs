namespace Ix.Palantir.Utilities
{
    using System.Collections.Generic;

    public static class DictionaryExtension
    {
        public static string ToUrlFormat(this IDictionary<string, string> dictionary)
        {
            SeparatedStringBuilder stringBuilder = new SeparatedStringBuilder("&");

            foreach (KeyValuePair<string, string> parameter in dictionary)
            {
                stringBuilder.AppendFormatWithSeparator("{0}={1}", parameter.Key, parameter.Value);
            }

            return stringBuilder.ToString();
        }
        public static string ToCookieFormat(this IDictionary<string, string> dictionary)
        {
            SeparatedStringBuilder stringBuilder = new SeparatedStringBuilder("; ");

            foreach (KeyValuePair<string, string> parameter in dictionary)
            {
                if (!string.IsNullOrWhiteSpace(parameter.Value))
                {
                    stringBuilder.AppendFormatWithSeparator("{0}={1}", parameter.Key, parameter.Value);
                }
            }

            return stringBuilder.ToString();
        }
        public static string ToFormattedString(this IDictionary<string, string> dictionary)
        {
            SeparatedStringBuilder stringBuilder = new SeparatedStringBuilder(" AND ");

            foreach (KeyValuePair<string, string> parameter in dictionary)
            {
                if (!string.IsNullOrWhiteSpace(parameter.Value))
                {
                    stringBuilder.AppendFormatWithSeparator("{0} = '{1}'", parameter.Key, parameter.Value);
                }
            }

            return stringBuilder.ToString();
        }
    }
}