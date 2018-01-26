using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.DataProcess.Entities
{
    public class DataStudentEntity : EntityBase<Int64>
    {
        public int GroupId { get; set; }

        public ProcessStatus Status { get; set; }

        public string Remark { get; set; }

        public int CommunityId { get; set; }

        //----------------------------------------

        public string School_TEA_ID { get; set; }

        public string Teacher_Number { get; set; }

        public string Teacher_First_Name { get; set; }

        public string Teacher_Middle_Name { get; set; }

        public string Teacher_Last_Name { get; set; }

        public string Teacher_Previous_Last_Name { get; set; }

        public string Teacher_Phone_Number { get; set; }

        public PhoneType Teacher_Phone_Type { get; set; }

        public string Teacher_Primary_Email { get; set; }

        public string Teacher_Secondary_Email { get; set; }

        public string New_Teacher_Number { get; set; }

        public DayType Student_Class_Day_Type { get; set; }

        public string Previous_Teacher_Number { get; set; }

        public string Previous_Teacher_First_Name { get; set; }

        public string Previous_Teacher_Middle_Name { get; set; }

        public string Previous_Teacher_Last_Name { get; set; }

        public string Student_TSDS_ID { get; set; }

        public string Student_First_Name { get; set; }

        public string Student_Middle_Name { get; set; }

        public string Student_Last_Name { get; set; }

        public DateTime Student_Birth_Date { get; set; }

        public int Student_Gender { get; set; }

        public string Student_Ethnicity { get; set; }

    }

    public enum ProcessStatus : byte
    {
        /// <summary>
        /// 等待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 处理队列中
        /// </summary>
        Queued = 1,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing = 2,

        /// <summary>
        /// 处理过的
        /// </summary>
        Processed = 3,

        /// <summary>
        /// 出错
        /// </summary>
        [Description("Error-Dup")]
        Error_Dup = 4
    }
}
