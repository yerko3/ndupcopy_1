using System;
using System.IO;
using System.Security.Cryptography;

namespace ndupcopy
{
    public class FileHash
    {
        public string Path { get; private set; }
        public byte[] Hash { get; private set; }

        public FileHash(string path)
        {
            Path = path;
            Hash = CalculateHash(path);
        }
        private static byte[] CalculateHash(string path)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    return sha256.ComputeHash(fs);
                }
            }
        }
        public static string GetFileHash(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(fs);
                    return BitConverter.ToString(hashBytes).Replace("-", "");
                }
            }
        }
        public bool AreHashesEqual(string hash1, string hash2)
        {
            return hash1.Equals(hash2);
        }

        public static byte[] GetCalculateHash(string path)
        {
            return CalculateHash(path);
        }

    }
}
