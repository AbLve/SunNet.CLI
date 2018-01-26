using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Reports.Models
{


    /// <summary>
    /// CircleDataExport StudentModel
    /// </summary>
    public class CircleDataExportStudentModel
    {
        public CircleDataExportStudentModel()
        {
            IClassContract classService= DomainFacade.CreateClassService();
            ClassLevels = classService.ClasseLevels.ToList();
        }

        public int CommunityId { get; set; }

        public string CommunityName { get; set; }

        public string CommunityIdentity { get; set; }

        public string DistrictNumber { get; set; }

        public int SchoolId { get; set; }

        public string SchoolName { get; set; }

        public string SchoolIdentity { get; set; }

        public string SchoolType { get; set; }

        public string SchoolNumber { get; set; }

        public EntityStatus SchoolStatus { get; set; }

        public int ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public EntityStatus StudentStatus { get; set; }

        public string StudentIdentity { get; set; }

        public string LocalStudentID { get; set; }

        public string TSDSStudentID { get; set; }

        public StudentAssessmentLanguage AssessmengLanguage { get; set; }

        public Gender StudentGender { get; set; }

        public Ethnicity StudentEthnicity { get; set; }

        public string StudentEthnicityOther { get; set; }

        private string _teachers;
        private List<TeacherModel> teachers = new List<TeacherModel>();


        public static IList<ClassLevelEntity> ClassLevels { get; set; }
        

        /// <summary>
        /// Teacher(s);
        /// Teacher: FirstName,LastName,TeacherId,TeacherNumber,PrimaryEmailAddress,IsHomeroom,TeacherTSDSID;
        /// </summary>
        public string Teachers
        {
            get { return _teachers; }
            set
            {
                _teachers = value;
                if (!string.IsNullOrEmpty(_teachers))
                {
                    var teas = _teachers.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (teas.Count > 0)
                    {
                        teas.ForEach(teaStr => teachers.Add(ParseTeacher(teaStr)));
                    }
                }
            }
        }

        public string HomeroomTeacher
        {
            get
            {
                var ts = teachers.Where(x => x.IsHomeroom).ToList();
                var strTeachers = "";
                ts.ForEach(t => strTeachers += string.Format("{0} {1}; ", t.FirstName, t.LastName));
                strTeachers = strTeachers.Trim();
                if (strTeachers.EndsWith(";"))
                    strTeachers = strTeachers.Remove(strTeachers.Length - 1);
                return strTeachers;
            }
        }

        public string TeacherId
        {
            get { return string.Join("; ", teachers.Select(x => x.TeacherId)); }
        }

        public string TeacherNumber
        {
            get { return string.Join("; ", teachers.Where(x=>x.TeacherNumber != "").Select(x => x.TeacherNumber)); }
        }

        public string TeacherPrimaryEmailAddress
        {
            get { return string.Join("; ", teachers.Select(x => x.PrimaryEmailAddress)); }
        }

        public string TeacherTSDSID
        {
            get { return string.Join("; ", teachers.Where(e => e.TeacherTSDSID != "").Select(x => x.TeacherTSDSID)); }
        }

        static TeacherModel ParseTeacher(string teacherStr)
        {
            var teacher = new TeacherModel();
            var props = teacherStr.Split("|".ToCharArray());
            teacher.FirstName = props.Length >= 1 ? props[0] : "";
            teacher.LastName = props.Length >= 2 ? props[1] : "";
            teacher.TeacherId = props.Length >= 3 ? props[2] : "";
            teacher.TeacherNumber = props.Length >= 4 ? props[3] : "";
            teacher.PrimaryEmailAddress = props.Length >= 5 ? props[4] : "";
            teacher.IsHomeroom = props.Length >= 6 && props[5] == "1";
            teacher.TeacherTSDSID= props.Length >= 7 ? props[6] : "";
            return teacher;
        }

        internal struct TeacherModel
        {
            internal string FirstName;
            internal string LastName;
            internal string TeacherId;
            internal string TeacherNumber;
            internal string PrimaryEmailAddress;
            internal bool IsHomeroom;
            internal string TeacherTSDSID;
        }

        private string _classes;
        private List<ClassModel> classes = new List<ClassModel>();
        /// <summary>
        /// ClassName,ClassId,Status,DayType,Classlevel;
        /// </summary>
        public string Classes
        {
            get { return _classes; }
            set
            {
                _classes = value;
                classes = new List<ClassModel>();
                if (!string.IsNullOrEmpty(_classes))
                {
                    var clses = _classes.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (clses.Count > 0)
                    {
                     
                        clses.ForEach(classStr => classes.Add(ParseClass(classStr)));
                    }
                }

            }
        }

        public string ClassName
        {
            get { return string.Join("; ", classes.Select(x => x.ClassName).Distinct()); }
        }

        public string ClassId
        {
            get { return string.Join("; ", classes.Select(x => x.ClassId).Distinct()); }
        }

        public string ClassStatus
        {
            get { return string.Join("; ", classes.Select(x => x.Status.ToDescription()).Distinct()); }
        }

        public string DayType
        {
            get { return string.Join("; ", classes.Select(x => x.DayType.ToDescription()).Distinct()); }
        }

        public string Classlevel
        {
            get { return string.Join("; ", classes.Select(x => x.ClassLevel).Distinct()); }
        }
        public string ClassInternalID
        {
            get
            {
                return string.Join("; ",
                    classes.Where(e => e.ClassInternalID != "").Select(x => x.ClassInternalID).Distinct());
            }
        }

        static ClassModel ParseClass(string classStr)
        {
            var classModel = new ClassModel();
            var props = classStr.Split('|');
            classModel.ClassName = props.Length >= 1 ? props[0] : "";
            classModel.ClassId = props.Length >= 2 ? props[1] : "";
            classModel.Status = (EntityStatus)int.Parse(props.Length >= 3 ? props[2] : "0");
            classModel.DayType = (DayType)(int.Parse(props.Length >= 4 ? props[3] : "0"));

            var level = ClassLevels.FirstOrDefault(c => c.ID == int.Parse(props.Length >= 5 ? props[4] : "0"));
            classModel.ClassLevel = (level==null?"":level.Name);
            classModel.ClassInternalID= props.Length >= 6 ? props[5] : "";
            return classModel;
        }

        internal struct ClassModel
        {
            internal string ClassName;
            internal string ClassId;
            internal EntityStatus Status;
            internal DayType DayType;
            internal string ClassLevel;
            internal string ClassInternalID;
        }

        public List<CircleDataExportStudentMeasureModel> Measures { get; set; }

        public StudentGradeLevel GradeLevel { get; set; }

    }
}
