namespace Ix.Palantir.Utilities
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Hasher
    {
        public static string EncodeToHexStr(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            foreach (byte b in hash)
            {
                result.Append(b.ToString("X2"));
            }
            return result.ToString();
        }
        public string ComputeSaltedHash(string input, int salt)
        {
            // Create Byte array of input string
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] secretBytes = encoder.GetBytes(input);

            // Create a new salt
            byte[] saltBytes = new byte[4];
            saltBytes[0] = (byte)(salt >> 24);
            saltBytes[1] = (byte)(salt >> 16);
            saltBytes[2] = (byte)(salt >> 8);
            saltBytes[3] = (byte)salt;

            // append the two arrays
            byte[] toHash = new byte[secretBytes.Length + saltBytes.Length];
            Array.Copy(secretBytes, 0, toHash, 0, secretBytes.Length);
            Array.Copy(saltBytes, 0, toHash, secretBytes.Length, saltBytes.Length);

            SHA1 sha1 = new SHA1Managed();
            byte[] computedHash = sha1.ComputeHash(toHash);

            return EncodeToHexStr(computedHash);
        }
    }
}