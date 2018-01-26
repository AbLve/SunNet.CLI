using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh.jsch;

namespace Sunnet.Framework.SFTP
{
    public class SFTPUserInfo : UserInfo
    {
        public SFTPUserInfo(string password)
        {
            _passwd = password;
        }

        string _passwd;
        public string getPassphrase()
        {
            return null;
        }

        public string getPassword()
        {
            return _passwd;
        }

        public bool promptPassphrase(string message)
        {
            return true;
        }

        public bool promptPassword(string message)
        {
            return true;
        }

        public bool promptYesNo(string message)
        {
            return true;
        }

        public void showMessage(string message)
        {
           
        }
    }
}