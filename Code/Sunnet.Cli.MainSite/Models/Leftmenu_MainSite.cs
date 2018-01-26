using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/15 14:15:20
 * Description:		Create Leftmenu_MainSite
 * Version History:	Created,2014/9/15 14:15:20
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.MainSite.Models
{
    public enum Leftmenu_MainSite
    {     
        [Description("Community Management")]
        Community,

        [Description("School Management")]
        School,

        [Description("Classroom Management")]
        Classroom,

        [Description("Class Management")]
        Class,

        [Description("TRS Class Management")]
        TRSClass,

        [Description("Parent Management")]
        ParentManagement,

        [Description("Student Management")]
        Student,

        [Description("User Management")]
        User,

        [Description("CLI User Management")]
        CLIUser,

        [Description("Permission Management")]
        Permission,

        [Description("My Profile")]
        My_Profile,

        [Description("My District/Community")]
        My_DistrictCommunity,

        [Description("My School")]
        My_School,

        [Description("My Children")]
        My_Children,

        [Description("Toolbox")]
        Toolbox,

        [Description("BUP")]
        BUP,

        [Description("Reports")]
        Reports,

        [Description("Class Roster Management")]
        Roster,

        [Description("Status Tracking")]
        StatusTracking,

        [Description("Objects Export")]
        Export,
        [Description("Parent")]
        Parent,
        [Description("Fun Activies")]
        FunActivites,
        [Description("Track Child's Development")]
        Observable,
        [Description("School Reports")]
        SchoolReport,
        [Description("My Connections")]
        MyConnections
    }
}