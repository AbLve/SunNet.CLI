using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sunnet.Framework.IoCConfiguration
{
    public interface IIoCConfigure
    {
        ISunnetLog Log { get; }
        IFile File { get; }
        IEmailSender EmailSender { get; }
        IEncrypt Encrypt { get; }
    }
}
