using Sunnet.Framework.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.DataProcess.Entities
{
    public class DataGroupEntity : EntityBase<int>
    {
        public int RecordCount { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public string FilePath { get; set; }

        public string OriginFileName { get; set; }

        public bool SendInvitation { get; set; }

        public ProcessStatus Status { get; set; }

        public int SchoolTotal { get; set; }

        public int SchoolFail { get; set; }

        public int SchoolSuccess { get; set; }

        public int TeacherTotal { get; set; }

        public int TeacherFail { get; set; }

        public int TeacherSuccess { get; set; }

        public int StudentTotal { get; set; }

        public int StudentFail { get; set; }

        public int StudentSuccess { get; set; }

        public string Remark { get; set; }

        public int CommunityId { get; set; }

        public bool CreateClassroom { get; set; }
    }
}
