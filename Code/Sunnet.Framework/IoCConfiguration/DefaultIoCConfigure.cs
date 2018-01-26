using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Framework.StringZipper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.IoCConfiguration
{
    public class DefaultIoCConfigure : IIoCConfigure
    {
        public virtual ISunnetLog Log
        {
            get
            {
                return new log4netProvider();
            }
        }

        public virtual IFile File
        {
            get { return new RealFileSystem(this.Log); }
        }

        public virtual IEmailSender EmailSender
        {
            get { return new SmtpClientEmailSender(this.Log); }
        }

        public virtual IEncrypt Encrypt
        {
            get { return new DESEncrypt(); }
        }
    }
}
