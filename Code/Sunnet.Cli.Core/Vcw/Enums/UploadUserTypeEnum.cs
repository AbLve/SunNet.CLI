using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/12/1
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/12/1
 * 
 * 
 **************************************************************************/


namespace Sunnet.Cli.Core.Vcw.Enums
{
    /// <summary>
    /// 用于记录上传人类型
    /// </summary>
    public enum UploadUserTypeEnum : byte
    {
        [Description("Teacher")]
        Teacher = 1,
        [Description("Coach")]
        Coach = 2,
        [Description("PM")]
        PM = 3,
        [Description("Admin")]
        Admin = 4
    }
}
