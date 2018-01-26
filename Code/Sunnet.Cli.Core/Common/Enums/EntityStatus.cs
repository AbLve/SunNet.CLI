using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/12 3:06:31
 * Description:		Please input class summary
 * Version History:	Created,2014/8/12 3:06:31
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Common.Enums
{
    public enum EntityStatus : byte
    {
        Active = 1,
        Inactive = 2
    }
    public enum CommunityNoteStatus : byte
    {
        Active = 1,
        Inactive = 2,
        Replaced = 3
    }

    public enum SchoolStatus : byte
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    public enum TsdsStatus : byte
    {
        Pending = 1,
        Processing = 2,
        Succeed = 3,
        Error = 4
    }
}
