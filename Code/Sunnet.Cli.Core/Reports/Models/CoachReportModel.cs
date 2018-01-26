using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Reports.Models
{
    public class CoachReportModel
    {
        public string CommunityName { get; set; }

        public string SchoolName { get; set; }

        public string SchoolType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TeacherFunding { get; set; }

        public string TeacherType { get; set; }

        public string TargetStatus { get; set; }

        public string TeacherCode { get; set; }

        public string PrimaryPhoneNumber { get; set; }

        public string PrimaryEmailAddress { get; set; }

        public string CoachAssignment { get; set; }

        public string CoachModel { get; set; }

        public string EcircleAssignment { get; set; }

        public string YearsInProject { get; set; }

        public decimal NumberofCoachingHoursReceived { get; set; }

        public decimal NumberofRemoteCoachingCyclesReceived { get; set; }

        public string Coach { get; set; }
    }
}
