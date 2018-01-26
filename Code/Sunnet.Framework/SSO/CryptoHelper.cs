using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Framework.SSO
{
    public class CryptoHelper
    {
        /// <summary>
        /// 复合 Hash：string --> byte[] --> hashed byte[] --> base64 string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ComputeHashString(string s)
        {
            return ToBase64String(ComputeHash(ConvertStringToByteArray(s)));
        }


        public static byte[] ComputeHash(byte[] buf)
        {
            //return ((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")).ComputeHash(buf);
            return SHA1.Create().ComputeHash(buf);

        }

        /// <summary>
        /// //System.Convert.ToBase64String
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static string ToBase64String(byte[] buf)
        {
            return System.Convert.ToBase64String(buf);
        }


        public static byte[] FromBase64String(string s)
        {
            return System.Convert.FromBase64String(s);
        }

        /// <summary>
        /// //Encoding.UTF8.GetBytes(s)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToByteArray(String s)
        {
            return Encoding.UTF8.GetBytes(s);//gb2312
        }


        public static string ConvertByteArrayToString(byte[] buf)
        {
            //return System.Text.Encoding.GetEncoding("utf-8").GetString(buf);

            return Encoding.UTF8.GetString(buf);
        }


        /// <summary>
        /// 字节数组转换为十六进制字符串
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] buf)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buf.Length; i++)
            {
                sb.Append(buf[i].ToString("X").Length == 2 ? buf[i].ToString("X") : "0" + buf[i].ToString("X"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 十六进制字符串转换为字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            Byte[] buf = new byte[s.Length / 2];
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(Char2Hex(s.Substring(i * 2, 1)) * 0x10 + Char2Hex(s.Substring(i * 2 + 1, 1)));
            }
            return buf;
        }


        private static byte Char2Hex(string chr)
        {
            switch (chr)
            {
                case "0":
                    return 0x00;
                case "1":
                    return 0x01;
                case "2":
                    return 0x02;
                case "3":
                    return 0x03;
                case "4":
                    return 0x04;
                case "5":
                    return 0x05;
                case "6":
                    return 0x06;
                case "7":
                    return 0x07;
                case "8":
                    return 0x08;
                case "9":
                    return 0x09;
                case "A":
                    return 0x0a;
                case "B":
                    return 0x0b;
                case "C":
                    return 0x0c;
                case "D":
                    return 0x0d;
                case "E":
                    return 0x0e;
                case "F":
                    return 0x0f;
            }
            return 0x00;
        }
    }
}
