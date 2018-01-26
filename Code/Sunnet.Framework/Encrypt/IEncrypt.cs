using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.Encrypt
{
    public interface IEncrypt
    {
        /// <summary>
        /// 加密
        /// </summary>
        string Encrypt(string source);

        /// <summary>
        /// 解密
        /// </summary>
        string Decrypt(string source);
    }


}
