using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.MainSite.Areas.Toolbox.Models
{
    public enum DesEnum
    {
        [Description("Community/District Primary Contact")]
        CommunityDistrict_PrimaryContact = 1,
        [Description("Community/District Secondary Contact")]
        CommunityDistrict_SecondaryContact,
        [Description("School Primary Contact")]
        School_PrimaryContact,
        [Description("School Secondary Contact")]
        School_SecondaryContact,
        //[Description("Community/District User Groups")]
        //CommunityDistrict_UserGroups,
        //[Description("Community/District Specialist User Groups ")]
        //CommunityDistrict_SpecialistUserGroups,
        //[Description("Principal/Director User Groups")]
        //PrincipalDirector_UserGroups,
        //[Description("School Specialist User Groups")]
        //School_SpecialistUserGroups
    }
}