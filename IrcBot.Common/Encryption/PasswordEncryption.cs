using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IrcBot.Common.Encryption
{
    public static class PasswordEncryption
    {
        private const string EncryptionKey = "3/j.~1DE8|hk296}z4|9aKC't|:7/fFH";

        private static readonly byte[] EncryptionSalt =
        {
            0x21, 0x46, 0x65, 0x72, 0x6e,
            0x20, 0x53, 0x6f, 0x66, 0x74,
            0x77, 0x61, 0x72, 0x65, 0x21
        };

        public static string Encrypt(string clearText)
        {
            var bytes = Encoding.Unicode.GetBytes(clearText);

            using (var encryptor = Aes.Create())
            {
                if (encryptor == null)
                {
                    throw new InvalidOperationException();
                }

                var pdb = new Rfc2898DeriveBytes(EncryptionKey, EncryptionSalt);

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.Close();
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            var bytes = Convert.FromBase64String(cipherText);

            using (var encryptor = Aes.Create())
            {
                if (encryptor == null)
                {
                    throw new InvalidOperationException();
                }

                var pdb = new Rfc2898DeriveBytes(EncryptionKey, EncryptionSalt);

                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.Close();
                    }

                    return Encoding.Unicode.GetString(memoryStream.ToArray());
                }
            }
        }
    }
}
