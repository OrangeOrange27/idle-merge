using System;
using System.IO;
using System.Security.Cryptography;

namespace Utils.Cryptography
{
    public class AesTool
    {
        private const int AesKeySize = 32;

        public static byte[] Encrypt(byte[] key, byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(data)} cannot be empty");
            }

            if (key == null || key.Length != AesKeySize)
            {
                throw new ArgumentException($"{nameof(key)} must be length of {AesKeySize}");
            }

            using var aes = new AesCryptoServiceProvider();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();
            var iv = aes.IV;
            using var encrypter = aes.CreateEncryptor(aes.Key, iv);
            using var cipherStream = new MemoryStream();
            using (var tCryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
            using (var tBinaryWriter = new BinaryWriter(tCryptoStream))
            {
                // prepend IV to data
                cipherStream.Write(iv);
                tBinaryWriter.Write(data);
                tCryptoStream.FlushFinalBlock();
            }
            var cipherBytes = cipherStream.ToArray();

            return cipherBytes;
        }

        public static byte[] Decrypt(byte[] key, byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(data)} cannot be empty");
            }

            if (key == null || key.Length != AesKeySize)
            {
                throw new ArgumentException($"{nameof(key)} must be length of {AesKeySize}");
            }

            using var aes = new AesCryptoServiceProvider();
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            // get first KeySize bytes of IV and use it to decrypt
            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(data, 0, iv, 0, iv.Length);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, iv), CryptoStreamMode.Write))
            using (var binaryWriter = new BinaryWriter(cs))
            {
                // decrypt cipher text from data, starting just past the IV
                binaryWriter.Write(
                    data,
                    iv.Length,
                    data.Length - iv.Length
                );
            }

            var dataBytes = ms.ToArray();

            return dataBytes;
        }
    }
}