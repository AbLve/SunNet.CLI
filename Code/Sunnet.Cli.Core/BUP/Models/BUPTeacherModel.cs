using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.BUP.Models
{
   public class BUPTeacherModel
    {
        public int ID { get; set; }

        public int TaskId { get; set; }

        public BUPAction Action { get; set; }

        public string CommunityName { get; set; }

        public string CommunityEngageID { get; set; }

        public string SchoolName { get; set; }

        public string SchoolEngageID { get; set; }

       /// <summary>
       /// Users表
       /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public string TeacherEngageID { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public string TeacherInternalId { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public string PrimaryPhoneNumber { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public byte PrimaryNumberType { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public string PrimaryEmailAddress { get; set; }

        /// <summary>
        /// Users表
        /// </summary>
        public string SecondaryEmailAddress { get; set; }

        public string ClassName { get; set; }

        public string ClassEngageID { get; set; }

        public string ClassroomName { get; set; }

        public string ClassroomEngageID { get; set; }

        public string ClassroomInternalID { get; set; }

        public BUPStatus Status { get; set; }

        public string Remark { get; set; }

        public int LineNum { get; set; }

       public string Teacher_TSDS_ID { get; set; }

       public BUPEntityStatus TeacherStatus { get; set; }
    }
}
