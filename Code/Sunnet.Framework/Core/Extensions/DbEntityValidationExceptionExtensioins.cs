using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/20 20:10:49
 * Description:		Please input class summary
 * Version History:	Created,2014/8/20 20:10:49
 * 
 * 
 **************************************************************************/
namespace Sunnet.Framework.Core.Extensions
{
    public static class DbEntityValidationExceptionExtensioins
    {
        /// <summary>
        /// 获取数据库验证的所有详细信息.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="separator">分隔符:\r\n | &lt;br&gt;.</param>
        /// <returns></returns>
        public static string GetDetailMessage(this DbEntityValidationException exception, string separator = "\r\n")
        {
            StringBuilder sb = new StringBuilder();
            if (exception.EntityValidationErrors.Any())
            {
                foreach (var validation in exception.EntityValidationErrors)
                {
                    foreach(var err in validation.ValidationErrors)
                        sb.AppendFormat("{0}{1}", err.ErrorMessage, separator);
                }
            }
            return sb.ToString();
        }
    }
}
