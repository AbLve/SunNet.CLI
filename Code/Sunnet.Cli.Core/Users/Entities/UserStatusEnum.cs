using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/8/7 11:52:41
 * Description:		Please input class summary
 * Version History:	Created,2014/8/7 11:52:41
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Users.Entities
{
    public enum UserTitle : byte
    {
        [Description("Mr.")]
        Mr = 1,
        [Description("Mrs.")]
        Mrs = 2,
        [Description("Ms.")]
        Ms = 3,
        [Description("Miss")]
        Miss = 4,
        [Description("Dr.")]
        Dr = 5
    }

    public enum ApplicantStatus : byte
    {
        Pending = 1,
        Invited = 2,
        Verified = 3,
        Denied = 4
    }

    public enum EmailType : byte
    {
        teacherApplicantEmailTemplate = 1,
        teacherAppliantNoSchool = 2
    }
}
