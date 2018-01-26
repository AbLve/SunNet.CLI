using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2/3 2015 17:02:55
 * Description:		Please input class summary
 * Version History:	Created,2/3 2015 17:02:55
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Reports.Entities
{
    public class SchoolViewEntity : EntityBase<int>
    {

        [Description("School Code")]
        public string SchoolId { get; set; }

        [Description("")]
        public int BasicSchoolId { get; set; }

        [Description("School Name")]
        public string SchoolName { get; set; }

        public int CommunityId { get; set; }

        [Description("Community/District Name")]
        public int CommunityName { get; set; }

        public EntityStatus Status { get; set; }

        [Description("Cohort")]
        public string SchoolYear { get; set; }

        [Description("Physical Address1")]
        public string PhysicalAddress1 { get; set; }

        [Description("Physical Address2")]
        public string PhysicalAddress2 { get; set; }

        [Description("School City")]
        public string City { get; set; }

        public int CountyId { get; set; }

        [Description("School County")]
        public int CountyName { get; set; }

        public int StateId { get; set; }

        [Description("School State")]
        public int StateName { get; set; }

        [Description("School Zip")]
        public string Zip { get; set; }

        [Description("School Phone")]
        public string PhoneNumber { get; set; }

        [Description("Phone# Type")]
        public PhoneType PhoneType { get; set; }

        public int SchoolTypeId { get; set; }

        public int SubTypeId { get; set; }

        [Description("School Type")]
        public string Type { get; set; }

        [Description("School Type")]
        public string SubType { get; set; }

        public int FundingId { get; set; }

        public string Funding { get; set; }

        [Description("Classroom Count")]
        public int CountOfClassroom { get; set; }

        [Description("Teacher Count")]
        public int CountOfTeacher { get; set; }

        [Description("Student Count")]
        public int CountOfStudent { get; set; }

    }
}
