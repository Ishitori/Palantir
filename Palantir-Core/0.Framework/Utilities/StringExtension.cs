namespace Ix.Palantir.Utilities
{
    using System.Diagnostics.Contracts;
    using System.Text;

    public static class StringExtension
    {
         public static string ToUTF8(this string value, Encoding initialEncoding)
         {
             byte[] initialEncodingBytes = initialEncoding.GetBytes(value);
             byte[] utf8Bytes = Encoding.Convert(initialEncoding, Encoding.UTF8, initialEncodingBytes);
             string message = Encoding.UTF8.GetString(utf8Bytes);

             return message;
         }

        public static string FirstSymbols(this string value, int symbolsCount, string endings = "...")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (value.Length < symbolsCount)
            {
                return value;
            }

            string substring = value.Substring(0, symbolsCount);

            return string.IsNullOrWhiteSpace(endings)
                           ? substring
                           : substring + endings;
        }

        public static string ToUpperFirstLetter(this string value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));
            return char.ToUpper(value[0]) + value.Substring(1);
        }
    }
}