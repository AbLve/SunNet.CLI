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
    public class BeechReportModel
    {
        public string CommunityDistrict { get; set; }

        public string SchoolName { get; set; }

        public string SchoolCode { get; set; }

        public string Phone { get; set; }

        public string PhysicalAddress { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public string AdminName { get; set; }

        public string AdminEmail { get; set; }

        public string ChildCareLicense { get; set; }

        public string Language { get; set; }

        public string TeacherName { get; set; }

        public string TeacherCode { get; set; }

        public int Infant { get; set; }

        public int Toddler { get; set; }

        public int Preschool { get; set; }

        public int Other { get; set; }

        public string SchoolNotes { get; set; }

        public string ClassName { get; set; }

        public string ClassCode { get; set; }
    }
}
