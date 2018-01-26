using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/9/12 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/9/12 12:15:10
 * 
 * 
 **************************************************************************/
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sunnet.Cli.UIBase.Models
{
    /// <summary>
    /// 页面和菜单的枚举
    /// </summary>
    public enum PagesModel
    {
        /// <summary>
        /// ADE 模块  只有 All 权限
        /// </summary>
        [Description("ADE")]
        ADE = 1000,

        /// <summary>
        /// Assessment 模块 ，不做 action 权限判断 , 只有 All 权限
        /// </summary>
        [Description("Assessment")]
        Assessment = 2000,

        /// <summary>
        ///Assessment => CPALLS+ 模块  
        /// </summary>
        [Description("CPALLS+")]
        CPALLS = 2100,

        /// <summary>
        ///Assessment => CEC 模块
        /// </summary>
        [Description("CEC")]
        CEC = 2200,

        /// <summary>
        ///Assessment => COT 模块
        /// </summary>
        [Description("COT")]
        COT = 2300,

        [Description("Track Child's development")]
        TCD = 2400,

        [Description("TSDS")]
        TSDS = 2500,

        ///// <summary>
        /////Assessment =>Cpalls Offline 模块
        ///// </summary>
        //[Description("Cpalls Offline")]
        //CpallsOffline = 2400,

        ///// <summary>
        /////Assessment =>Cec Offline 模块
        ///// </summary>
        //[Description("Cec Offline")]
        //CecOffline = 2500,

        ///// <summary>
        /////Assessment =>Cot Offline 模块
        ///// </summary>
        //[Description("Cot Offline")]
        //CotOffline = 2600,

        /// <summary>
        /// Administrative
        /// </summary>
        [Description("Administrative")]
        Administrative = 3000,

        [Description("Dashboard")]
        Dashboard = 3100,

        [Description("Community Management")]
        Community_Management = 3200,

        [Description("School Management")]
        School_Management = 3300,

        [Description("Classroom Management")]
        Classroom_Management = 3400,

        [Description("Class Management")]
        Class_Management = 3500,

        [Description("TRS Class Management")]
        TRSClass_Management = 3550,

        [Description("Parent Management")]
        Parent_Management = 3555,

        [Description("Student Management")]
        Student_Management = 3600,

        [Description("User Verification")]
        User_Verification = 3780,

        [Description("Delegate")]
        Delegate = 3790,

        [Description("User Management")]
        User_Management = 3700,

        [Description("CLI User Management")]
        CLI_User_Management = 3800,

        [Description("Permission Management")]
        Permission_Management = 3900,

        [Description("Community")]
        Community = 3710,

        [Description("Community Specialist List")]
        Community_Specialist_List = 3720,

        [Description("Principal Director")]
        Principal_Director = 3730,

        [Description("TRS Specialist")]
        TRS_Specialist = 3740,

        [Description("School Specialist")]
        School_Specialist = 3745,

        [Description("Teacher")]
        Teacher = 3750,

        [Description("Parent")]
        Parent = 3755,

        [Description("Statewide")]
        Statewide = 3760,

        [Description("Auditor")]
        Auditor = 3770,

        [Description("Toolbox")]
        Toolbox = 3905,

        [Description("Data Management")]
        BUP = 3906,

        [Description("Class Roster Management")]
        ClassRoster = 3908,

        [Description("Reports")]
        Reports = 3909,

        [Description("Data Export")]
        DataExport = 3911,

        [Description("CIRCLE Data Export")]
        CIRCLEDataExport = 3912,

        [Description("Beech Reports")]
        BeechReports = 3913,

        [Description("Participation Counts")]
        ParticipationCounts = 3914,

        [Description("Mentor/Coach Report")]
        MentorCoachReport = 3915,

        [Description("Ever Serviced")]
        EverServiced = 3916,

        [Description("Currently Serving")]
        CurrentlyServing = 3917,

        [Description("By ESC Region")]
        ByESCRegion = 3918,

        [Description("PD Reports")]
        PDReports = 3919,

        [Description("Coaching Hours by Communitys")]
        CoachingHoursbyCommunitys = 3920,

        [Description("Teacher Turnover Report")]
        TeacherTurnoverReport = 3921,

        [Description("TSR Media Consent Reports")]
        TSRMediaConsentReports = 3922,

        [Description("Status Tracking")]
        StatusTracking = 3930,

        [Description("Export")]
        Export = 3932,

        [Description("Community/District")]
        BUP_Community = 3940,

        [Description("School")]
        BUP_School = 3941,

        [Description("Classroom")]
        BUP_Classroom = 3942,

        [Description("Class")]
        BUP_Class = 3943,

        [Description("Teacher")]
        BUP_Teacher = 3944,

        [Description("Student")]
        BUP_Student = 3945,

        [Description("Community/District User/Specialist")]
        BUP_CommunityUser = 3946,

        [Description("Principal/School Specialist")]
        BUP_Principal = 3948,

        [Description("Parent")]
        BUP_Parent = 3950,

        [Description("Statewide Agency")]
        BUP_Statewide = 3951,

        [Description("Auditor")]
        BUP_Auditor = 3952,

        [Description("Data Process")]
        DataProcess = 3953,

        [Description("LMS")]
        LMS = 4000,

        [Description("Collaborative Tool")]
        VCW = 5000,

        [Description("Teachers")]
        Teachers = 5100,

        [Description("Teacher VIP Assignments")]
        TeacherVip = 5110,

        [Description("Teacher Coaching Assignments")]
        TeacherAssignment = 5120,

        [Description("Teacher General")]
        TeacherGeneral = 5130,

        [Description("Teacher Summary")]
        TeacherSummary = 5140,

        [Description("COT & CEC Reports")]
        STGReport = 5150,

        [Description("Mentor/Coach and Coordinators")]
        Coach = 5200,

        [Description("Coach Assignments")]
        CoachAssignment = 5210,

        [Description("Coach Send Assignments – VIP/Coaching")]
        CoachTeachers = 5220,

        [Description("Coach General")]
        CoachGeneral = 5230,

        [Description("Coach Summary")]
        CoachSummary = 5240,

        [Description("Project Managers")]
        PM = 5300,

        [Description("Teachers")]
        PMTeachers = 5310,

        [Description("Coaches")]
        PMCoaches = 5320,

        [Description("Summary")]
        PMSummary = 5330,

        [Description("Admin")]
        Admin = 5400,

        [Description("TRS")]
        TRS = 6000,

        [Description("TRS Page")]
        TRSPage = 6100,

        [Description("CAC")]
        CAC = 7000,

        [Description("LDE")]
        LDE = 8000,

        [Description("Item Bulk Upload")]
        BulkUpload = 9000,

        [Description("ParentFeature")]
        ParentFeature = 9500,

        [Description("Track Your child's Development")]
        TrackchildDevelopment = 9503,

        [Description("School Reports")]
        SchoolReport = 9504,

        [Description("My Connections")]
        MyConnection = 9505,

        [Description("Talk with Me")]
        TalkwithMe1 = 9506,

        [Description("Talk with Me")]
        TalkwithMe2 = 9506,

        [Description("Practice")]
        Practice = 9600
    }
}