using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Entity.Validation;

using Sunnet.Framework.Core.Base;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Framework.Resources;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Core.Extensions;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/1 1:01:13
 * Description:		Please input class summary
 * Version History:	Created,2014/8/1 1:01:13
 * 
 * 
 **************************************************************************/



namespace Sunnet.Cli.Core
{
    /// <summary>
    /// ����ҵ��ʵ�ֻ���
    /// </summary>
    public abstract class CoreServiceBase
    {
        protected CoreServiceBase()
        {

        }

        protected CoreServiceBase(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt)
        {
            LoggerHelper = log;
            FileHelper = fileHelper;
            EmailSender = emailSender;
            Encrypt = encrypt;
        }

        /// <summary>
        /// ��ȡ������ ������Ԫ�������ڴ���ͬ��ҵ����������
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; set; }

        /// <summary>
        /// ����Դ�ļ�(Sunnet.Cli.UIBase.Information.resx)�л�ȡ��Ӧ��ʾ��Ϣ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetInformation(string key)
        {
            var r = ResourceHelper.GetRM().RmInformation.GetObject(key);
            if (r != null)
                return r.ToString();
            return "";
        }

        protected OperationResult ResultError(Exception exception)
        {
            var result = new OperationResult(OperationResultType.EntityValidationError);
            if (exception is DbEntityValidationException)
            {
                DbEntityValidationException ex = exception as DbEntityValidationException;
                if (ex.InnerException != null)
                {
                    result.Message = ex.InnerException.ToString();
                }
                if (ex.EntityValidationErrors != null && ex.EntityValidationErrors.Any())
                {
                    result.Message = string.Format("EntityValidationErrors:<br>{0}", ex.GetDetailMessage("<br>"));
                }
            }
            else
            {
                result.Message = GetInformation("DbError");
            }           
            LoggerHelper.Debug(exception);
            return result;
        }
        
        protected ISunnetLog LoggerHelper { get; set; }
        protected IFile FileHelper { get; set; }
        protected IEmailSender EmailSender { get; set; }
        protected IEncrypt Encrypt { get; set; }
    }
}
