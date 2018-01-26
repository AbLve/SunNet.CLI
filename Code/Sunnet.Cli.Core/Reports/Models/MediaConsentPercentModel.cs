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
    public class MediaConsentPercentModel
    {
        public string CommunityName { get; set; }

        public string SchoolName { get; set; }

        public string SchoolCode { get; set; }

        public string TeacherName { get; set; }

        public string TeacherCode { get; set; }

        public string CoachingModel { get; set; }

        public string YearsinProject { get; set; }

        public string CoachName { get; set; }

        public MediaRelease TeacherMediaRelease { get; set; }

        public string ClassName { get; set; }

        public string ClassCode { get; set; }

        public int MediaReleaseYes { get; set; }

        public int MediaReleaseNo { get; set; }

        public int MediaReleaseRefused { get; set; }

        public int TotalStudentsinClass { get; set; }

        public string PercentofConsented
        {
            get
            {
                return (TotalStudentsinClass == 0 || (MediaReleaseYes + MediaReleaseRefused) == 0)
                    ? "0%"
                    : Convert.ToDecimal((Convert.ToDecimal(MediaReleaseYes) +
                                         Convert.ToDecimal(MediaReleaseRefused))*100/
                                        Convert.ToDecimal(TotalStudentsinClass)).ToString("#0.0") + "%";
            }
        }
    }
}
