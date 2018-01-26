using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Sunnet.Cli.Vcw.Models
{
    public enum LeftmenuModel
    {
        Dashboard = 0,

        [Description("Summary of All Files")]
        Teacher_Summary = 1,

        [Description("My VIP Assignments")]
        Teacher_VIP = 2,

        [Description("My General Files")]
        Teacher_General = 3,

        [Description("My Coaching Assignments")]
        Teacher_Assignment = 4,

        [Description("My COT & CEC Reports")]
        STG_Reports = 5,


        [Description("Summary of All")]
        Coach_Summary = 20,

        [Description("My General Files")]
        Coach_General = 21,

        [Description("My Teachers")]
        Coach_Teachers = 22,

        [Description("My Assignments")]
        Coach_Assignment = 23,



        [Description("Summary of All")]
        PM_Summary = 40,

        [Description("My Teachers")]
        PM_Teachers = 41,

        [Description("My Coaches")]
        PM_Coaches = 42,


        [Description("Summary of All")]
        Admin_Summary = 60,

        [Description("Teachers")]
        Admin_Teachers = 62,

        [Description("Coaches")]
        Admin_Coaches = 64
    }
}