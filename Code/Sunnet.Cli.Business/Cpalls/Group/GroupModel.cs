using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Practices.Entites;

namespace Sunnet.Cli.Business.Cpalls.Group
{
    public class GroupModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int ClassId { get; set; }

        /// <summary>
        /// 1,2,3,4
        /// </summary>
        public string StudentIds { get; set; }

        /// <summary>
        /// StudentIds 与 StudentList 数据相结合
        /// </summary>
        public List<GroupStudentModel> StudentList { get; set; }
        public ICollection<CustomGroupMyActivityEntity> GroupActivities { get; set; }
        public ICollection<PracticeGroupMyActivityEntity> PracticeGroupActivities { get; set; }
        public IList<MyActivityEntity> MyActivityList { get; set; }
        public string Note { get; set; }
    }

    public class GroupStudentModel
    {
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public bool Seleted { get; set; }

        public string Color { get; set; }

        public int StudentAssessmentId { get; set; }

        public decimal Goal { get; set; }

        public string BenchmarkColor { get; set; }
    }
}

