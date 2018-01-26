using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sunnet.Framework.Encrypt
{
    internal class MD5Encrypt : IEncrypt
    {
        public string Encrypt(string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            byte[] output = md5.ComputeHash(bytes);

            return BitConverter.ToString(output);
        }

        public string Decrypt(string source)
        {
            throw new NotImplementedException();
        }
    }
}
