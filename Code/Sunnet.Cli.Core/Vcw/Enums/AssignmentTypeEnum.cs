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
    public enum AssignmentTypeEnum : byte
    {
        [Description("Teacher Coaching Assignment")]
        TeacherAssignment = 1,


        [Description("Teacher VIP Assignment")]
        TeacherVIP = 2,


        [Description("Coach Assignment")]
        CoachAssignment = 3
    }


    public enum ChangeStatusEnum
    {
        AddFile = 1,
        DeleteFile = 2,
        UpdateFile = 3
    }
}
