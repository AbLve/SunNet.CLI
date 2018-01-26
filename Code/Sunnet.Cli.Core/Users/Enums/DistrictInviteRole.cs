using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-pc
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/11 8:46:44
 * Description:		Please input class summary
 * Version History:	Created,2014/9/11 8:46:44
 * 
 * 
 **************************************************************************/
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Sunnet.Cli.Core.Users.Enums
{
    public enum DistrictInviteRole : byte
    {
        [Description("District Representative")]
        DistrictRepresentative = 1,
        [Description("Community Representative")]
        CommunityRepresentative = 2,
        [Description("Home-base Provider")]
        HomebaseProvider = 3
    }
}
