using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Framework.SSO
{
    public class CryptoService
    {
        /// <summary>
        /// 加密的密钥，在构造方法里，由外部传入，默认为空，以免误导 12/07/2017 David
        /// </summary>
        string sKey = "";//"22362E7A9285DD53A0BBC2932F9733C505DC04EDBFE00D70";
        string sIV = "";//"1E7FA9231E7FA923";

        byte[] byteKey;
        byte[] byteIV;

        /// <summary>
        /// 加密向量
        /// </summary>
        static byte[] bIV = { 1, 2, 3, 4, 5, 6, 7, 8 };

        public CryptoService()
        { }

        public CryptoService(string key, string IV)
        {
            sKey = key;
            sIV = IV;

            byteKey = CryptoHelper.HexStringToByteArray(sKey);
            byteIV = CryptoHelper.HexStringToByteArray(sIV);
        }



        /// <summary>
        /// 将明文加密，返回密文
        /// </summary>
        /// <param name="Data">要加密的字串</param>
        /// <returns></returns>
        public byte[] Encrypt(string Data)
        {
            try
            {
                byte[] ret;

                using (MemoryStream mStream = new MemoryStream())
                using (CryptoStream cStream = new CryptoStream(mStream,
                    new TripleDESCryptoServiceProvider().CreateEncryptor(byteKey, byteIV),
                    CryptoStreamMode.Write))
                {

                    byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);

                    // Write the byte array to the crypto stream and flush it.
                    cStream.Write(toEncrypt, 0, toEncrypt.Length);
                    cStream.FlushFinalBlock();

                    // Get an array of bytes from the 
                    // MemoryStream that holds the 
                    // encrypted data.
                    ret = mStream.ToArray();

                }

                return ret;
            }
            catch (CryptographicException e)
            {
                //Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                Console.Write(e);
                return null;
            }

        }


        /// <summary>
        /// 将明文加密，返回密文
        /// </summary>
        /// <param name="toEncrypt">明文</param>
        /// <param name="encrypted">密文</param>
        /// <returns></returns>
        public bool Encrypt(byte[] toEncrypt, out byte[] encrypted)
        {
            encrypted = null;
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                using (MemoryStream mStream = new MemoryStream())
                using (CryptoStream cStream = new CryptoStream(mStream,
                    new TripleDESCryptoServiceProvider().CreateEncryptor(byteKey, byteIV),
                    CryptoStreamMode.Write))
                {

                    // Write the byte array to the crypto stream and flush it.
                    cStream.Write(toEncrypt, 0, toEncrypt.Length);
                    cStream.FlushFinalBlock();

                    // Get an array of bytes from the 
                    // MemoryStream that holds the 
                    // encrypted data.
                    encrypted = mStream.ToArray();
                }

                return true;
            }
            catch (CryptographicException e)
            {
                Console.Write(e);
                //Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return false;
            }

        }



        /// <summary>
        /// 将明文加密，返回 Base64 字符串
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string EncryptToString(string Data)
        {
            try
            {
                string base64String = string.Empty;

                using (MemoryStream mStream = new MemoryStream())
                using (CryptoStream cStream = new CryptoStream(mStream,
                    new TripleDESCryptoServiceProvider().CreateEncryptor(byteKey, byteIV),
                    CryptoStreamMode.Write))
                {

                    byte[] toEncrypt = new ASCIIEncoding().GetBytes(Data);

                    cStream.Write(toEncrypt, 0, toEncrypt.Length);
                    cStream.FlushFinalBlock();

                    byte[] ret = mStream.ToArray();

                    base64String = Convert.ToBase64String(ret);
                }

                return base64String;
            }
            catch (CryptographicException e)
            {
                Console.Write(e);
                return null;
            }

        }


        /// <summary>
        /// 将密文解密，返回明文
        /// </summary>
        /// <param name="Data">密文</param>
        /// <returns>明文</returns>
        public bool Decrypt(byte[] Data, out string decrypted)
        {
            decrypted = string.Empty;
            try
            {

                using (MemoryStream msDecrypt = new MemoryStream(Data))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    new TripleDESCryptoServiceProvider().CreateDecryptor(byteKey, byteIV),
                    CryptoStreamMode.Read))
                {

                    byte[] fromEncrypt = new byte[Data.Length];

                    // Read the decrypted data out of the crypto stream
                    // and place it into the temporary buffer.
                    csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                    decrypted = Encoding.UTF8.GetString(fromEncrypt);//new ASCIIEncoding().GetString(fromEncrypt);

                    return true;
                }
            }
            catch (CryptographicException e)
            {
                Console.Write(e);
                return false;
            }
        }

    }
}
