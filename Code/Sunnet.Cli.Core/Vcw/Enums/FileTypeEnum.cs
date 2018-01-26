using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/21
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/10/21
 * 
 * 
 **************************************************************************/
using System.ComponentModel;

namespace Sunnet.Cli.Core.Vcw.Enums
{
    public enum FileTypeEnum : byte
    {
        [Description("Teacher VIP")]
        TeacherVIP = 1,

        [Description("Teacher General")]
        TeacherGeneral = 2,

        [Description("Teacher Assignment")]
        TeacherAssignment = 3,

        [Description("Coach General")]
        CoachGeneral = 4,

        [Description("Coach Assignment")]
        CoachAssignment = 5
    }
}
