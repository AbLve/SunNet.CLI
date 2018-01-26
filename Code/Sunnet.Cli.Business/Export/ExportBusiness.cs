using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using LinqKit;

using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Export;
using Sunnet.Cli.Core.Export.Entities;
using Sunnet.Cli.Business.Export.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Framework.Csv;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Reports;
using Sunnet.Cli.Core.Export.Enums;
using Sunnet.Framework.StringZipper;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Business.Users;

namespace Sunnet.Cli.Business.Export
{
    public class ExportBusiness
    {
        private readonly IExportContract _exportContract;
        private readonly CommunityBusiness _communityBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly ClassroomBusiness _classroomBusiness;
        private readonly ClassBusiness _classBusiness;
        private readonly StudentBusiness _studentBusiness;
        private readonly UserBusiness _userBusiness;

        public ExportBusiness(EFUnitOfWorkContext unit = null)
        {
            _exportContract = DomainFacade.CreateExportService(unit);
            _communityBusiness = new CommunityBusiness(unit);
            _schoolBusiness = new SchoolBusiness(unit);
            _classroomBusiness = new ClassroomBusiness(unit);
            _classBusiness = new ClassBusiness(unit);
            _studentBusiness = new StudentBusiness(unit);
            _userBusiness = new UserBusiness(unit);
        }

        #region ReportTemp
        public IEnumerable<ReportTemplateEntity> GetReportTemps()
        {
            return _exportContract.ReportTemplates;
        }

        public List<ReportTemplateWithUserModel> GetReportTempsSelectListOther(string name, UserBaseEntity user)
        {
            return _exportContract.ReportTemplates.AsExpandable()
                .Where(r => r.Name.Contains(name))
                .Where(GetRoleCondition(user))
                .Select(r => new ReportTemplateWithUserModel()
                {
                    ID = r.ID,
                    Name = r.Name,
                    Status = r.Status,
                    CreatedBy = r.CreatedBy,
                    CreatedOn = r.CreatedOn
                }).ToList();
        }

        public IEnumerable<SelectItemModel> GetReportTempsSelectList(UserBaseEntity user)
        {
            return _exportContract.ReportTemplates.AsExpandable()
                .Where(r => r.Status == EntityStatus.Active)
                .Where(GetReportRoleCondition(user))
                .Select(r => new SelectItemModel()
            {
                ID = r.ID,
                Name = r.Name
            });
        }

        public ReportTemplateEntity GetReportTempsById(int tempId)
        {
            return _exportContract.GetTemplate(tempId);
        }

        //是否存在重名
        public bool SearchSameTemps(string name, int id = -1)
        {
            if (_exportContract.ReportTemplates.Where(r => r.ID != id && r.Name == name).Count() > 0)
                return true;
            else
                return false;
        }

        public OperationResult InsertReportTemp(ReportTemplateEntity entity)
        {
            return _exportContract.InsertReportTemplate(entity);
        }

        public OperationResult UpdateReportTemp(ReportTemplateEntity entity)
        {
            return _exportContract.UpdateReportTemplate(entity);
        }

        public OperationResult DeleteReportTemp(int id)
        {
            return _exportContract.DeleteReportTemplate(id);
        }
        #endregion

        #region FieldMap
        public IEnumerable<FieldListModel> GetAllFilds(UserBaseEntity user)
        {
            List<FieldListModel> filedList = _exportContract.FieldMaps
                .Select(f => new FieldListModel()
                {
                    ID = f.ID,
                    FieldName = f.FieldName,
                    DisplayName = f.DisplayName,
                    IsDisabled = true
                }).ToList();

            #region 根据角色禁用
            bool checkCommunity = true;
            bool checkSchool = true;
            bool checkClassroom = true;
            bool checkClass = true;
            bool checkStudent = true;

            bool checkUser = true;
            bool checkCommunityUser = true;
            bool checkStateWide = true;
            bool checkPrincipal = true;
            bool checkTeacher = true;
            bool checkParent = true;
            bool checkAuditor = true;
            bool checkCoach = true;
            bool checkVideoCoding = true;
            Role userRole = user.Role;
            switch (userRole)
            {
                case Role.Statewide:
                    checkVideoCoding = false;
                    checkCoach = false;
                    break;
                case Role.Community:
                case Role.District_Community_Delegate:
                case Role.District_Community_Specialist://可查看自己的delegate
                    checkStateWide = false;
                    checkAuditor = false;
                    checkVideoCoding = false;
                    checkCoach = false;
                    break;
                case Role.Community_Specialist_Delegate:
                    checkStateWide = false;
                    checkCommunityUser = false;
                    checkAuditor = false;
                    checkVideoCoding = false;
                    checkCoach = false;
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    checkCommunity = false;

                    checkCommunityUser = false;
                    checkStateWide = false;
                    checkAuditor = false;
                    checkVideoCoding = false;
                    checkCoach = false;
                    break;
                case Role.TRS_Specialist_Delegate:
                case Role.School_Specialist_Delegate:
                    checkCommunity = false;

                    checkCommunityUser = false;
                    checkPrincipal = false;
                    checkStateWide = false;
                    checkAuditor = false;
                    checkVideoCoding = false;
                    checkCoach = false;
                    break;
                case Role.Teacher:
                    checkCommunity = false;
                    checkSchool = false;

                    checkCommunityUser = false;
                    checkStateWide = false;
                    checkPrincipal = false;
                    checkTeacher = false;
                    checkAuditor = false;
                    checkCoach = false;
                    checkVideoCoding = false;
                    break;
                case Role.Parent:
                    checkCommunity = false;
                    checkSchool = false;
                    checkClassroom = false;
                    checkClass = false;
                    checkStudent = false;

                    checkUser = false;
                    checkCommunityUser = false;
                    checkStateWide = false;
                    checkPrincipal = false;
                    checkTeacher = false;
                    checkParent = false;
                    checkAuditor = false;
                    checkCoach = false;
                    checkVideoCoding = false;
                    break;
            }
            #endregion

            #region Object
            CommunityRoleEntity communityRole = _communityBusiness.GetCommunityRoleEntity(userRole);
            if (communityRole != null && checkCommunity)
            {
                List<FieldListModel> commuFieldList = filedList.Where(f => f.ID <= (int)RowID.Communities).ToList();
                CheckFieldIsShow(communityRole, commuFieldList);
            }

            SchoolRoleEntity schoolRole = _schoolBusiness.GetSchoolRoleEntity(userRole);
            if (schoolRole != null && checkSchool)
            {
                List<FieldListModel> schoolFieldList = filedList.Where(f => f.ID > (int)RowID.Communities && f.ID <= (int)RowID.Schools).ToList();
                CheckFieldIsShow(schoolRole, schoolFieldList);
            }

            ClassroomRoleEntity classroomRole = _classroomBusiness.GetClassroomRoleEntity(userRole);
            if (classroomRole != null && checkClassroom)
            {
                List<FieldListModel> classroomFieldList = filedList.Where(f => f.ID > (int)RowID.Schools && f.ID <= (int)RowID.Classrooms).ToList();
                CheckFieldIsShow(classroomRole, classroomFieldList);
            }

            ClassRoleEntity classRole = _classBusiness.GetClassRole(userRole);
            if (classRole != null && checkClass)
            {
                List<FieldListModel> classFieldList = filedList.Where(f => f.ID > (int)RowID.Classrooms && f.ID <= (int)RowID.Classes).ToList();
                CheckFieldIsShow(classRole, classFieldList);
            }

            StudentRoleEntity studentRole = _studentBusiness.GetStudentRoleEntity(userRole);
            if (studentRole != null && checkStudent)
            {
                List<FieldListModel> studentFieldList = filedList.Where(f => f.ID > (int)RowID.Classes && f.ID <= (int)RowID.Students).ToList();
                CheckFieldIsShow(studentRole, studentFieldList);
            }
            #endregion

            #region User
            if (checkUser)
            {
                List<FieldListModel> userBaseFieldList = filedList.Where(f => f.ID > (int)RowID.Students && f.ID <= (int)RowID.Users).ToList();
                ChangeFieldIsDisabled(userBaseFieldList);
            }
            if (checkCommunityUser)
            {
                List<FieldListModel> communityUserFieldList = filedList.Where(f => f.ID > (int)RowID.Users && f.ID <= (int)RowID.CommunityUsers).ToList();
                ChangeFieldIsDisabled(communityUserFieldList);
            }

            if (checkStateWide)
            {
                List<FieldListModel> stateWideFieldList = filedList.Where(f => f.ID > (int)RowID.CommunityUsers && f.ID <= (int)RowID.StateWides).ToList();
                ChangeFieldIsDisabled(stateWideFieldList);
            }

            PrincipalRoleEntity principalRole = _userBusiness.GetPrincipalRoleEntity(userRole);
            if (principalRole != null && checkPrincipal)
            {
                List<FieldListModel> principalFieldList = filedList.Where(f => f.ID > (int)RowID.StateWides && f.ID <= (int)RowID.Principals).ToList();
                CheckFieldIsShow(principalRole, principalFieldList);
            }

            if (checkTeacher)
            {
                List<FieldListModel> teacherFieldList = filedList.Where(f => f.ID > (int)RowID.Principals && f.ID <= (int)RowID.Teachers).ToList();
                ChangeFieldIsDisabled(teacherFieldList);
            }

            if (checkParent)
            {
                List<FieldListModel> parentFieldList = filedList.Where(f => f.ID > (int)RowID.Teachers && f.ID <= (int)RowID.Parents).ToList();
                ChangeFieldIsDisabled(parentFieldList);
            }

            if (checkAuditor)
            {
                List<FieldListModel> auditorFieldList = filedList.Where(f => f.ID > (int)RowID.Parents && f.ID <= (int)RowID.Auditors).ToList();
                ChangeFieldIsDisabled(auditorFieldList);
            }

            CoordCoachRoleEntity coordCoachRole = _userBusiness.GetCoordCoachRoleEntity(userRole);
            if (coordCoachRole != null && checkCoach)
            {
                List<FieldListModel> coordCoachFieldList = filedList.Where(f => f.ID > (int)RowID.Auditors && f.ID <= (int)RowID.CoordCoachs).ToList();
                CheckFieldIsShow(coordCoachRole, coordCoachFieldList);
            }

            if (checkVideoCoding)
            {
                List<FieldListModel> videoCodingFieldList = filedList.Where(f => f.ID > (int)RowID.CoordCoachs && f.ID <= (int)RowID.VideoCodings).ToList();
                ChangeFieldIsDisabled(videoCodingFieldList);
            }
            #endregion

            return filedList;
        }

        private void ChangeFieldIsDisabled(List<FieldListModel> fieldList)
        {
            fieldList.ForEach(f => f.IsDisabled = false);
        }

        private void CheckFieldIsShow(object entity, List<FieldListModel> fieldList)
        {
            PropertyInfo[] propertys = entity.GetType().GetProperties();
            Dictionary<string, string> dicEntity = new Dictionary<string, string>();
            foreach (PropertyInfo prop in propertys)
            {
                dicEntity.Add(prop.Name, prop.GetValue(entity, null).ToString());
            }
            foreach (FieldListModel filed in fieldList)
            {
                if (dicEntity.ContainsKey(filed.FieldName))
                {
                    if (dicEntity[filed.FieldName] != "X")
                        filed.IsDisabled = false;
                }
                else
                    filed.IsDisabled = false;
            }
        }

        /*
        public List<string> GetSqlListByDispalyName(List<string> displayFileds, int communityId)
        {
            List<int> fieldList = _exportContract.FieldMaps
                .Where(f => displayFileds.Contains(f.DisplayName)).Select(f => f.ID).ToList();

            List<int> objectFieldList = fieldList.Where(f => f <= 500).ToList();
            string objectSql = string.Empty;
            if (objectFieldList.Count > 0)
                objectSql = GetObjectExportSql(objectFieldList, communityId);

            List<int> userFieldList = fieldList.Where(f => f > 500).ToList();
            List<string> userSqlList = new List<string>();
            if (userFieldList.Count > 0)
                userSqlList = GetUserExportSqlList(userFieldList);
            if (objectSql.Length > 0)
                userSqlList.Add(objectSql);
            return userSqlList;
        }
        */

        public IEnumerable<FieldMapModel> GetAllFildsForSql()
        {
            return _exportContract.FieldMaps.Select(f => new FieldMapModel()
            {
                ID = f.ID,
                FieldName = f.FieldName,
                DisplayName = f.DisplayName,
                AssociateSql = f.AssociateSql,
                SelectName = f.SelectName
            });
        }
        #endregion

        #region GetSQL
        public string GetObjectExportSql(UserBaseEntity user, List<int> checkedFieldList, int communityId)
        {
            List<int> communityFieldId = checkedFieldList.Where(r => r <= (int)RowID.Communities).ToList();
            List<int> schoolFieldId = checkedFieldList.Where(r => r > (int)RowID.Communities && r <= (int)RowID.Schools).ToList();
            List<int> classroomFieldId = checkedFieldList.Where(r => r > (int)RowID.Schools && r <= (int)RowID.Classrooms).ToList();
            List<int> classFieldId = checkedFieldList.Where(r => r > (int)RowID.Classrooms && r <= (int)RowID.Classes).ToList();
            List<int> studentFieldId = checkedFieldList.Where(r => r > (int)RowID.Classes && r <= (int)RowID.Students).ToList();

            List<FieldMapModel> FieldsMaps = GetAllFildsForSql().ToList();

            #region 只选了Community的字段
            if (communityFieldId.Count > 0
                && schoolFieldId.Count == 0
                && classroomFieldId.Count == 0
                && classFieldId.Count == 0
                && studentFieldId.Count == 0)
            {
                List<FieldMapModel> communityFieldsMaps = FieldsMaps.Where(f => communityFieldId.Contains(f.ID)).ToList();
                string exportSqlStr = "SELECT DISTINCT {0} FROM Communities C {1} WHERE C.ID=" + communityId;
                string fieldStrPart = "";
                string leftStrPart = "";
                foreach (FieldMapModel model in communityFieldsMaps)
                {
                    if (model.AssociateSql.Length == 0)
                        fieldStrPart += string.Format("C.{0} [{1}],", model.FieldName, model.DisplayName);
                    else
                    {
                        fieldStrPart += string.Format("{0} [{1}],", model.SelectName, model.DisplayName);
                        leftStrPart += string.Format(" {0} ", model.AssociateSql);
                    }
                }
                fieldStrPart = fieldStrPart.TrimEnd(',');
                exportSqlStr = string.Format(exportSqlStr, fieldStrPart, leftStrPart);
                return exportSqlStr;
            }
            #endregion

            #region Sql拼接
            StringBuilder exportDataSql = new StringBuilder();
            exportDataSql.Append("SELECT DISTINCT {0} ");
            exportDataSql.Append("FROM Communities C ");
            exportDataSql.Append("LEFT JOIN CommunitySchoolRelations CSR ON C.ID=CSR.CommunityId ");
            exportDataSql.Append(" {1} ");
            exportDataSql.Append(" WHERE C.ID=" + communityId);

            string format0 = "";
            string format1 = "";

            //添加查询条件
            Dictionary<string, string> sqlCondition = GetObjectCondition(user);
            StringBuilder sbStrWhere = new StringBuilder();
            if (sqlCondition.ContainsKey("School"))
                sbStrWhere.Append(sqlCondition["School"]);

            if (communityFieldId.Count > 0)
            {
                List<FieldMapModel> communityFieldsMaps = FieldsMaps.Where
                    (f => communityFieldId.Contains(f.ID)).ToList();
                foreach (FieldMapModel model in communityFieldsMaps)
                {
                    if (model.AssociateSql.Length == 0)
                        format0 += string.Format("C.{0} [{1}],", model.FieldName, model.DisplayName);
                    else
                    {
                        format0 += string.Format("{0} [{1}],", model.SelectName, model.DisplayName);
                        format1 += string.Format(" {0} ", model.AssociateSql);
                    }
                }
            }

            if (schoolFieldId.Count > 0)
            {
                format1 += " LEFT JOIN Schools S ON CSR.SchoolId=S.ID ";

                List<FieldMapModel> schoolFieldsMaps = FieldsMaps.Where
                    (f => schoolFieldId.Contains(f.ID)).ToList();
                foreach (FieldMapModel model in schoolFieldsMaps)
                {
                    if (model.AssociateSql.Length == 0)
                        format0 += string.Format("S.{0} [{1}],", model.FieldName, model.DisplayName);
                    else
                    {
                        format0 += string.Format("{0} [{1}],", model.SelectName, model.DisplayName);
                        format1 += " " + model.AssociateSql + " ";
                    }
                }
            }

            if (classroomFieldId.Count > 0)
            {
                if (sqlCondition.ContainsKey("Classroom"))
                    sbStrWhere.Append(sqlCondition["Classroom"]);

                format1 += " LEFT JOIN Classrooms Classroom ON Classroom.SchoolId=CSR.SchoolId ";

                List<FieldMapModel> classroomFieldsMaps = FieldsMaps.Where
                    (f => classroomFieldId.Contains(f.ID)).ToList();
                foreach (FieldMapModel model in classroomFieldsMaps)
                {
                    if (model.AssociateSql.Length == 0)
                        format0 += string.Format("Classroom.{0} [{1}],", model.FieldName, model.DisplayName);
                    else
                    {
                        format0 += string.Format("{0} [{1}],", model.SelectName, model.DisplayName);
                        format1 += string.Format(" {0} ", model.AssociateSql);
                    }
                }
            }

            if (classFieldId.Count > 0)
            {
                if (sqlCondition.ContainsKey("Class"))
                    sbStrWhere.Append(sqlCondition["Class"]);

                format1 += " LEFT JOIN Classes Cla ON Cla.SchoolId=CSR.SchoolId ";

                List<FieldMapModel> classFieldsMaps = FieldsMaps.Where
                    (f => classFieldId.Contains(f.ID)).ToList();

                foreach (FieldMapModel model in classFieldsMaps)
                {
                    
                    if (model.AssociateSql.Length == 0)
                    {
                        //为了处理Class为IsDeleted时，Status显示为Deleted
                        if (model.FieldName == "Status")
                            format0 += string.Format("{0} [{1}],", "(case when Cla.IsDeleted=1 then 3 else Cla.Status end)", model.DisplayName);
                        else
                            format0 += string.Format("Cla.{0} [{1}],", model.FieldName, model.DisplayName);
                    }
                    else
                    {
                        format0 += string.Format("{0} [{1}],", model.SelectName, model.DisplayName);
                        format1 += string.Format(" {0} ", model.AssociateSql);
                    }
                }
            }

            if (studentFieldId.Count > 0)
            {
                if (sqlCondition.ContainsKey("Student"))
                    sbStrWhere.Append(sqlCondition["Student"]);
                if (classFieldId.Count > 0)
                {
                    format1 += " LEFT JOIN StudentClassRelations SCR ON Cla.ID=SCR.ClassId ";
                    format1 += " LEFT JOIN Students Stu ON Stu.ID=SCR.StudentId ";
                }
                else
                {
                    format1 += " LEFT JOIN SchoolStudentRelations SSR ON SSR.SchoolId=CSR.SchoolId ";
                    format1 += " LEFT JOIN Students Stu ON Stu.ID=SSR.StudentId ";
                }

                List<FieldMapModel> studentFieldsMaps = FieldsMaps.Where
                        (f => studentFieldId.Contains(f.ID)).ToList();
                foreach (FieldMapModel model in studentFieldsMaps)
                {
                    if (model.AssociateSql.Length == 0)
                    {
                        //为了处理Class为IsDeleted时，Status显示为Deleted
                        if (model.FieldName == "Status")
                            format0 += string.Format("{0} [{1}],", "(case when Stu.IsDeleted=1 then 3 else Stu.Status end)", model.DisplayName);
                        else
                            format0 += string.Format("Stu.{0} [{1}],", model.FieldName, model.DisplayName);
                    }
                    else
                    {
                        format0 += string.Format("{0} [{1}],", model.SelectName, model.DisplayName);
                        format1 += string.Format(" {0} ", model.AssociateSql);
                    }
                }
            }

            format0 = format0.TrimEnd(',');
            string exportSql = exportDataSql.ToString();
            exportSql = string.Format(exportSql, format0, format1);

            exportSql += sbStrWhere.ToString();
            return exportSql;

            #endregion
        }

        public List<string> GetUserExportSqlList(List<int> UserFieldList, UserBaseEntity user, int communityId)
        {
            List<string> exportSqlList = new List<string>();
            List<int> baseUserField = UserFieldList.Where(r => r > (int)RowID.Students && r <= (int)RowID.Users).ToList();
            List<FieldMapModel> FieldsMaps = GetAllFildsForSql().Where(x => x.ID > 500).ToList();

            string baseSelectField = "";
            string baseJoin = " Users U ";
            //if (baseUserField.Count > 0)
            //{
            List<FieldMapModel> baseUserModels = FieldsMaps.Where
                               (f => baseUserField.Contains(f.ID)).ToList();
            //因为order by Role 所以必选Role
            if (baseUserModels.Where(f => f.FieldName == "Role").Count() == 0)
            {
                baseUserModels.Add(new FieldMapModel
                {
                    ID = 502,
                    FieldName = "Role",
                    DisplayName = "Role",
                    AssociateSql = ""
                });
            }
            foreach (FieldMapModel model in baseUserModels)
            {
                if (model.AssociateSql.Length == 0)
                {
                    //为了处理Class为IsDeleted时，Status显示为Deleted
                    if (model.FieldName == "Status")
                        baseSelectField += string.Format("{0} [{1}],", "(case when U.IsDeleted=1 then 3 else U.Status end)", model.DisplayName);
                    else
                        baseSelectField += string.Format("U.{0} [{1}],", model.FieldName, model.DisplayName);
                }
                else
                {
                    baseSelectField += string.Format("{0} [{1}],", model.SelectName, model.DisplayName);
                    baseJoin += string.Format(" {0} ", model.AssociateSql);
                }
            }
            //}
            #region 过滤数据条件
            Dictionary<string, string> userSqlCondition = GetUserSqlCondition(user);
            string communityUserCondition = userSqlCondition.ContainsKey("CommunityUser") ?
                userSqlCondition["CommunityUser"] : "";

            string stateWideCondition = userSqlCondition.ContainsKey("StateWide") ?
                userSqlCondition["StateWide"] : "";

            string principalCondition = userSqlCondition.ContainsKey("Principal") ?
                userSqlCondition["Principal"] : "";

            string teacherCondition = userSqlCondition.ContainsKey("Teacher") ?
                userSqlCondition["Teacher"] : "";

            string parentCondition = userSqlCondition.ContainsKey("Parent") ?
                userSqlCondition["Parent"] : "";

            string auditorCondition = userSqlCondition.ContainsKey("Auditor") ?
                userSqlCondition["Auditor"] : "";

            string coachCondition = userSqlCondition.ContainsKey("Coach") ?
                userSqlCondition["Coach"] : "";

            string videoCodingCondition = userSqlCondition.ContainsKey("VideoCoding") ?
                userSqlCondition["VideoCoding"] : "";

            string userCondition = userSqlCondition.ContainsKey("User") ? userSqlCondition["User"] : "";
            #endregion
            //只选择了Users
            if (UserFieldList.Where(r => r > 600).Count() == 0)
            {
                baseSelectField = baseSelectField.TrimEnd(',');

                StringBuilder sbFormat2 = new StringBuilder();
                sbFormat2.Append(" AND (");
                //CommunityUsers,Teachers
                sbFormat2.AppendFormat(" U.ID IN (SELECT UserId FROM UserComSchRelations WHERE CommunityId={0} )", communityId);
                //Community Delegate
                sbFormat2.AppendFormat(" OR U.ID IN ( SELECT UserId FROM CommunityUsers WHERE ParentId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId = {0}))", communityId);
                //Principal Delegate
                sbFormat2.Append(" OR U.ID IN(");
                sbFormat2.Append(" SELECT UserId FROM Principals WHERE ParentId IN (");
                sbFormat2.Append(" SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(");
                sbFormat2.AppendFormat(" SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId={0} )))", communityId);
                //Parents
                sbFormat2.Append(" OR U.ID IN (");
                sbFormat2.Append(" SELECT UserId FROM Parents WHERE ID IN (");
                sbFormat2.Append(" SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN (");
                sbFormat2.Append(" SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN (");
                sbFormat2.AppendFormat(" SELECT  SchoolId FROM    CommunitySchoolRelations WHERE   CommunityId={0}))))", communityId);
                //Principal
                sbFormat2.Append(" OR U.ID IN (");
                sbFormat2.Append(" SELECT UserId FROM Principals WHERE UserId IN (");
                sbFormat2.Append(" SELECT UserId FROM UserComSchRelations WHERE SchoolId IN (");
                sbFormat2.AppendFormat(" SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId={0})))", communityId);
                sbFormat2.Append(")");
                //内部用户
                if (user.Role <= Role.Mentor_coach)
                    sbFormat2.Append(" OR(U.[Role] <=" + (int)Role.Mentor_coach + ")");
                string exportDataSql = string.Format("SELECT DISTINCT {0} FROM {1} Where 1=1 {2}", baseSelectField, baseJoin, sbFormat2);
                exportDataSql += userCondition;
                exportSqlList.Add(exportDataSql);
                return exportSqlList;
            }

            List<int> communityUserField = UserFieldList.Where(r => r > (int)RowID.Users && r <= (int)RowID.CommunityUsers).ToList();
            List<int> stateWideUserField = UserFieldList.Where(r => r > (int)RowID.CommunityUsers && r <= (int)RowID.StateWides).ToList();
            List<int> principalUserField = UserFieldList.Where(r => r > (int)RowID.StateWides && r <= (int)RowID.Principals).ToList();
            List<int> teacherUserField = UserFieldList.Where(r => r > (int)RowID.Principals && r <= (int)RowID.Teachers).ToList();
            List<int> parentsUserField = UserFieldList.Where(r => r > (int)RowID.Teachers && r <= (int)RowID.Parents).ToList();
            List<int> auditorUserField = UserFieldList.Where(r => r > (int)RowID.Parents && r <= (int)RowID.Auditors).ToList();
            List<int> coordCoachUserField = UserFieldList.Where(r => r > (int)RowID.Auditors && r <= (int)RowID.CoordCoachs).ToList();
            List<int> videoCodingUserField = UserFieldList.Where(r => r > (int)RowID.CoordCoachs && r <= (int)RowID.VideoCodings).ToList();

            if (communityUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    communityUserField, "CommunityUsers", "CU", communityUserCondition, communityId, true);
                exportSqlList.Add(exportDataSql);
            }
            if (stateWideUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    stateWideUserField, "StateWides", "SW", stateWideCondition, communityId);
                exportSqlList.Add(exportDataSql);
            }
            if (principalUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    principalUserField, "Principals", "Principals", principalCondition, communityId, true);
                exportSqlList.Add(exportDataSql);
            }
            if (teacherUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    teacherUserField, "Teachers", "Teachers", teacherCondition, communityId);
                exportSqlList.Add(exportDataSql);
            }
            if (parentsUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    parentsUserField, "Parents", "Parents", parentCondition, communityId);
                exportSqlList.Add(exportDataSql);
            }
            if (auditorUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    auditorUserField, "Auditors", "Auditors", auditorCondition);
                exportSqlList.Add(exportDataSql);
            }
            if (coordCoachUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    coordCoachUserField, "CoordCoachs", "CC", coachCondition);
                exportSqlList.Add(exportDataSql);
            }
            if (videoCodingUserField.Count > 0)
            {
                string exportDataSql = GetAnUserSql(FieldsMaps, baseSelectField, baseJoin,
                    videoCodingUserField, "VideoCodings", "VC", videoCodingCondition);
                exportSqlList.Add(exportDataSql);
            }
            return exportSqlList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FieldsMaps">字段信息</param>
        /// <param name="baseSelectField">基础表选择字段</param>
        /// <param name="baseJoin">基础连接表</param>
        /// <param name="userFields">选择的表字段</param>
        /// <param name="tableName">表名称</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="strWhere">查询条件</param>
        /// <returns></returns>
        private string GetAnUserSql(List<FieldMapModel> FieldsMaps, string baseSelectField, string baseJoin,
            List<int> userFields, string tableName, string tableAlias, string strWhere, int communityId = 0, bool hasParentField = false)
        {
            string exportDataSql = "SELECT DISTINCT {0} FROM {1} WHERE 1=1 {2}";
            StringBuilder sbFormat0 = new StringBuilder();
            StringBuilder sbFormat1 = new StringBuilder();
            StringBuilder sbFormat2 = new StringBuilder();

            sbFormat0.Append(baseSelectField);
            sbFormat2.Append(strWhere);

            //选择的community下的Users
            if (communityId != 0)
            {

                switch (tableAlias)
                {
                    case "CU":
                    case "SW":
                    case "Teachers":
                        sbFormat1.Append(" UserComSchRelations UCSR, ");
                        sbFormat2.AppendFormat(" AND UCSR.CommunityId={0} ", communityId);
                        if (hasParentField)
                            sbFormat2.Append(" AND ( " + tableAlias + ".UserId = UCSR.UserId OR " + tableAlias + ".ParentId = UCSR.UserId) ");
                        else
                            sbFormat2.Append(" AND " + tableAlias + ".UserId = UCSR.UserId ");
                        break;
                    case "Principals":
                        sbFormat1.Append(" UserComSchRelations UCSR, ");
                        sbFormat2.AppendFormat(" AND UCSR.SchoolId IN (SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId={0}) ", communityId);
                        sbFormat2.Append(" AND ( " + tableAlias + ".UserId = UCSR.UserId OR " + tableAlias + ".ParentId = UCSR.UserId) ");
                        break;
                    case "Parents":
                        sbFormat2.Append(" AND Parents.ID IN( ");
                        sbFormat2.Append(" SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN( ");
                        sbFormat2.Append(" SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN( ");
                        sbFormat2.AppendFormat(" SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId={0}))) ", communityId);
                        break;
                }
            }
            sbFormat2.Append(" ORDER BY U.Role ");

            sbFormat1.Append(baseJoin + " INNER JOIN " + tableName + "  " +
               tableAlias + " ON U.ID=" + tableAlias + ".UserId ");

            List<FieldMapModel> communityUserModels = FieldsMaps.Where
                (f => userFields.Contains(f.ID)).ToList();
            foreach (FieldMapModel model in communityUserModels)
            {
                if (model.AssociateSql.Length == 0)
                    sbFormat0.AppendFormat(tableAlias + ".{0} [{1}],", model.FieldName, model.DisplayName);
                else
                {
                    sbFormat0.AppendFormat("{0} [{1}],", model.SelectName, model.DisplayName);
                    sbFormat1.AppendFormat(" {0} ", model.AssociateSql);
                }
            }
            exportDataSql = string.Format(exportDataSql, sbFormat0.ToString().TrimEnd(','), sbFormat1, sbFormat2);
            return exportDataSql;
        }

        private Dictionary<string, string> GetObjectCondition(UserBaseEntity user)
        {
            Dictionary<string, string> objectSqlCondition = new Dictionary<string, string>();
            int userId = user.ID;
            string schoolCondition = string.Empty;
            string classroomCondition = string.Empty;
            string classCondition = string.Empty;
            string studentCondition = string.Empty;
            switch (user.Role)
            {
                case Role.Principal:
                case Role.School_Specialist:
                case Role.TRS_Specialist:
                    schoolCondition = " AND CSR.SchoolID IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + ")";
                    objectSqlCondition.Add("School", schoolCondition);
                    break;
                case Role.Principal_Delegate:
                case Role.School_Specialist_Delegate:
                case Role.TRS_Specialist_Delegate:
                    schoolCondition = " AND CSR.SchoolID IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + "))";
                    objectSqlCondition.Add("School", schoolCondition);
                    break;
                case Role.Teacher:
                    classroomCondition = " AND Classroom.ID IN(SELECT ClassroomId FROM ClassroomClassRelations WHERE ClassId IN(SELECT ID FROM Classes WHERE ID IN(SELECT ClassId FROM TeacherClassRelations WHERE TeacherId IN(SELECT ID FROM Teachers WHERE UserId=" + userId + "))))";
                    objectSqlCondition.Add("Classroom", classroomCondition);

                    classCondition = " AND Cla.ID IN(SELECT ClassId FROM TeacherClassRelations WHERE TeacherId IN(SELECT ID FROM Teachers WHERE UserId=" + userId + "))";
                    objectSqlCondition.Add("Class", classCondition);

                    studentCondition = " AND Stu.ID IN(SELECT StudentId FROM StudentClassRelations WHERE ClassId IN(SELECT ClassId FROM TeacherClassRelations WHERE TeacherId IN(SELECT ID FROM Teachers WHERE UserId=" + userId + ")))";
                    objectSqlCondition.Add("Student", studentCondition);
                    break;
            }
            return objectSqlCondition;
        }

        private Dictionary<string, string> GetUserSqlCondition(UserBaseEntity user)
        {

            Role userRole = user.Role;
            Dictionary<string, string> sqlCondition = new Dictionary<string, string>();
            int userId = user.ID;
            string communityUserCondition = string.Empty;
            string principalCondition = string.Empty;
            string teacherCondition = string.Empty;
            string parentCondition = string.Empty;
            string auditorCondition = string.Empty;
            StringBuilder userCondition = new StringBuilder();
            switch (userRole)
            {
                #region Statewide/105
                case Role.Statewide:
                    communityUserCondition = " AND CU.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    communityUserCondition += " AND (U.[Role] IN(" + (int)Role.Community + "," + (int)Role.District_Community_Specialist + ")) ";
                    sqlCondition.Add("CommunityUser", communityUserCondition);

                    principalCondition = " AND Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))";
                    principalCondition += " AND (U.[Role] IN(" + (int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ")) ";
                    sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))))";
                    sqlCondition.Add("Parent", parentCondition);

                    auditorCondition = " AND Auditors.UserId IN (SELECT UserId FROM Auditors)";
                    sqlCondition.Add("Auditors", auditorCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")) ");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))) ");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")) ");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Parents WHERE ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM StateWides)");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Auditors)");
                    userCondition.Append(") ");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.Community + "," + (int)Role.District_Community_Specialist + ",");
                    userCondition.Append((int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent + ",");
                    userCondition.Append((int)Role.Statewide + ",");
                    userCondition.Append((int)Role.Auditor);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region Community/110
                case Role.Community:
                    communityUserCondition = " AND (CU.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    //加上自己的代理
                    communityUserCondition += " OR CU.UserId IN (SELECT UserId FROM CommunityUsers WHERE ParentId=" + userId + "))";
                    communityUserCondition += " AND (U.[Role] IN(" + (int)Role.Community + "," + (int)Role.District_Community_Specialist + ", " + (int)Role.District_Community_Delegate + ")) ";
                    sqlCondition.Add("CommunityUser", communityUserCondition);

                    principalCondition = " AND Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))";
                    principalCondition += " AND (U.[Role] IN(" + (int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ")) ";
                    sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM CommunityUsers WHERE ParentId=" + userId + ")");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM Parents WHERE ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.Community + "," + (int)Role.District_Community_Specialist + "," + (int)Role.District_Community_Delegate + ",");
                    userCondition.Append((int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region District_Community_Specialist/115
                case Role.District_Community_Specialist:
                    communityUserCondition = " AND CU.ParentId=" + userId;//代理
                    sqlCondition.Add("CommunityUser", communityUserCondition);

                    principalCondition = " AND Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))";
                    principalCondition += " AND (U.[Role] IN(" + (int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ")) ";
                    sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN (SELECT UserId FROM CommunityUsers WHERE ParentId=" + userId + ")");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + "))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId=" + userId + ")))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.Community_Specialist_Delegate + ",");
                    userCondition.Append((int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region District_Community_Delegate/120
                case Role.District_Community_Delegate:
                    communityUserCondition = " AND CU.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))";
                    communityUserCondition += " AND U.[Role]=" + (int)Role.District_Community_Specialist;//此角色只能看到District_Community_Specialist
                    sqlCondition.Add("CommunityUser", communityUserCondition);

                    principalCondition = " AND Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + "))))";
                    principalCondition += " AND (U.[Role] IN(" + (int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ")) ";
                    sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN (SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + "))))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))");
                    userCondition.Append(" OR U.ID IN (SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + "))))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.District_Community_Specialist + ",");
                    userCondition.Append((int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region Community_Specialist_Delegate/140
                case Role.Community_Specialist_Delegate:
                    principalCondition = " AND Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + "))))";
                    principalCondition += " AND (U.[Role] IN(" + (int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ")) ";
                    sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + "))))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM UserComSchRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + ")))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM CommunitySchoolRelations WHERE CommunityId IN(SELECT CommunityId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM CommunityUsers WHERE UserId=" + userId + "))))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region Principal/125
                case Role.Principal:
                    principalCondition = " AND (Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    //加上自己的代理
                    principalCondition += " OR Principals.UserId IN(SELECT UserId FROM Principals WHERE ParentId=" + userId + ")) ";
                    principalCondition += " AND (U.[Role] IN(" + (int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + "," + (int)Role.Principal_Delegate + ")) ";
                    sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + ")))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Principals WHERE ParentId=" + userId + ") ");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.Principal + "," + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + "," + (int)Role.Principal_Delegate + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region Principal_Delegate/135
                case Role.Principal_Delegate:
                    principalCondition = " AND Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + ")))";
                    principalCondition += " AND (U.[Role] IN(" + (int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ")";
                    sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + "))) ";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN (SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + "))))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + ")))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + "))) ");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN (SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + ")))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.School_Specialist + "," + (int)Role.TRS_Specialist + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region TRS_Specialist School_Specialist
                case Role.TRS_Specialist:
                case Role.School_Specialist:
                    principalCondition = " AND Principals.ParentId=" + userId;
                    sqlCondition.Add("Principal", principalCondition);

                    //teacher条件同principal
                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + ")))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM Principals WHERE ParentId =" + userId + ")");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId=" + userId + "))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.TRS_Specialist_Delegate + "," + (int)Role.School_Specialist_Delegate + ",");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region School_Specialist_Delegate TRS_Specialist_Delegate
                case Role.TRS_Specialist_Delegate:
                    //principalCondition = " AND Principals.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + ")))";
                    //principalCondition += " AND U.[Role]=" + (int)Role.School_Specialist;
                    //sqlCondition.Add("Principal", principalCondition);

                    teacherCondition = " AND Teachers.UserId IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + "))) ";
                    sqlCondition.Add("Teacher", teacherCondition);

                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + "))))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM UserComSchRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + "))) ");
                    userCondition.Append(" OR U.ID IN(SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM SchoolStudentRelations WHERE SchoolId IN(SELECT SchoolId FROM UserComSchRelations WHERE UserId IN(SELECT ParentId FROM Principals WHERE UserId=" + userId + ")))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role] IN(");
                    userCondition.Append((int)Role.Teacher + "," + (int)Role.Parent);
                    userCondition.Append("))");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion

                #region Teacher
                case Role.Teacher:
                    parentCondition = " AND Parents.ID IN(SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM StudentClassRelations WHERE ClassId IN(SELECT ClassId FROM TeacherClassRelations WHERE TeacherId IN(SELECT ID FROM Teachers WHERE UserId=" + userId + "))))";
                    sqlCondition.Add("Parent", parentCondition);

                    userCondition.Append(" AND(");
                    userCondition.Append(" U.ID IN(SELECT UserId FROM Parents WHERE ID IN (SELECT ParentId FROM ParentStudentRelations WHERE StudentId IN(SELECT StudentId FROM StudentClassRelations WHERE ClassId IN(SELECT ClassId FROM TeacherClassRelations WHERE TeacherId IN(SELECT ID FROM Teachers WHERE UserId=" + userId + ")))))");
                    userCondition.Append(")");
                    userCondition.Append(" AND (U.[Role]=" + (int)Role.Parent + ")");
                    sqlCondition.Add("User", userCondition.ToString());
                    break;
                #endregion
            }
            return sqlCondition;
        }

        #endregion

        #region ExportInfo
        public OperationResult InsertExportInfo(ExportInfoEntity entity)
        {
            return _exportContract.InsertExportInfo(entity);
        }

        public OperationResult InsertExportInfoList(List<ExportInfoEntity> entities)
        {
            return _exportContract.InsertExportInfoList(entities);
        }

        public ExportInfoEntity GetExportInfosById(int id)
        {
            return _exportContract.GetExportInfo(id);
        }

        public List<ExportInfoEntity> GetNotProcessExportInfos()
        {
            DateTime todayStart = DateTime.Now.Date;
            DateTime todayEnd = DateTime.Now.AddDays(1).Date.AddSeconds(-1);

            return _exportContract.ExportInfos.Where(x => x.Status < ExportInfoStatus.Sent
                && x.ExcuteTime >= todayStart
                && x.ExcuteTime <= todayEnd).ToList();
        }

        public OperationResult UpdateExportInfos(List<ExportInfoEntity> entities)
        {
            return _exportContract.UpdateExportInfos(entities);
        }

        public ExportInfoEntity ExportInfoModelToEntity(string exportSql, string groupName, string ftpPassword,
            ExportInfoModel model, UserBaseEntity userInfo)
        {
            ExportInfoEntity entity = new ExportInfoEntity();
            entity.CreaterMail = userInfo.PrimaryEmailAddress;
            entity.CreaterFirstName = userInfo.FirstName;
            entity.CreaterLastName = userInfo.LastName;
            entity.ExecuteSQL = exportSql;
            entity.FileName = "";
            entity.DownloadUrl = "";
            entity.FileUrl = "";
            entity.Status = ExportInfoStatus.Save;
            entity.FileType = model.FileType;
            entity.CreatedBy = userInfo.ID;
            entity.GroupName = groupName;

            entity.ReceiveFileBy = model.ReceiveFileBy;
            entity.FtpHostIp = model.FtpHostIp == null ? "" : model.FtpHostIp;
            entity.FtpPort = model.FtpPort;
            entity.FtpEnableSsl = model.FtpEnableSsl;
            entity.FtpFilePath = model.FtpFilePath == null ? "/" : model.FtpFilePath;
            entity.FtpUserName = model.FtpUserName == null ? "" : model.FtpUserName;
            entity.FtpPassword = ftpPassword;
            return entity;
        }

        public DataSet GetQueryDataBySql(string sqlStr)
        {
            return _exportContract.ExecuteExportSql(sqlStr);
        }

        public void ExportToCsv(ExportFileType fileType, DataTable dt, string fileFullPath)
        {
            CsvFileWriter csvWriter = new CsvFileWriter(fileFullPath);
            CsvRow csvTitle = new CsvRow();
            foreach (DataColumn dc in dt.Columns)
            {
                csvTitle.Add(dc.ColumnName);
            }
            switch (fileType)
            {
                case ExportFileType.Comma:
                    csvWriter.WriteRow(csvTitle);
                    break;
                case ExportFileType.Tab:
                    csvWriter.WriteRowSeparateByTab(csvTitle);
                    break;
                case ExportFileType.Pipe:
                    csvWriter.WriteRowSeparateByPipe(csvTitle);
                    break;
            }


            //枚举
            IDictionary<string, string> objectEunm = GetObjectEnum();
            foreach (DataRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(dr[0].ToString()))//过滤掉空行
                    continue;
                CsvRow csvRow = new CsvRow();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dr[i].ToString().Contains("\r\n"))
                        dr[i] = dr[i].ToString().Replace("\r\n", " ");

                    if (objectEunm.Keys.Contains(dt.Columns[i].ColumnName) && dr[i].ToString() != "")
                    {
                        Assembly assem = Assembly.Load("Sunnet.Cli.Core");
                        Type enumType = assem.GetType
                            ("Sunnet.Cli.Core." + objectEunm[dt.Columns[i].ColumnName]);
                        string enumValue = Enum.GetName(enumType, dr[i]);
                        enumValue = enumValue == null ? "" : enumValue;
                        csvRow.Add(enumValue);
                    }
                    else
                    {
                        if (dr[i].ToString() == "1753.1.1 0:00:00")
                            csvRow.Add("");
                        else
                            csvRow.Add(dr[i].ToString());
                    }
                }
                switch (fileType)
                {
                    case ExportFileType.Comma:
                        csvWriter.WriteRow(csvRow);
                        break;
                    case ExportFileType.Tab:
                        csvWriter.WriteRowSeparateByTab(csvRow);
                        break;
                    case ExportFileType.Pipe:
                        csvWriter.WriteRowSeparateByPipe(csvRow);
                        break;
                }
            }
            csvWriter.Close();
        }

        #endregion

        #region Enum Fields
        public IDictionary<string, string> GetObjectEnum()
        {
            IDictionary<string, string> objectEunm = new Dictionary<string, string>() { };

            #region Community
            objectEunm.Add(new KeyValuePair<string, string>("Community Status", "Common.Enums.EntityStatus"));
            objectEunm.Add(new KeyValuePair<string, string>("Community PhoneNumber Type", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>("Community Primary Phone Type", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("Community Second Contact Phone Type", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("Community Primary Contact Salutation", "Common.Enums.UserSalutation"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("Community Second Contact Salutation", "Common.Enums.UserSalutation"));
            #endregion

            #region School
            objectEunm.Add(new KeyValuePair<string, string>("School Status", "Common.Enums.SchoolStatus"));
            objectEunm.Add(new KeyValuePair<string, string>("School PhoneType", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("School Primary Contact Salutation", "Common.Enums.UserSalutation"));
            objectEunm.Add(new KeyValuePair<string, string>("School Primary Contact Phone Type", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("School Second Contact Salutation", "Common.Enums.UserSalutation"));
            objectEunm.Add(new KeyValuePair<string, string>("School Second Contact Phone Type", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>("School Internet Speed", "Common.Enums.InternetSpeed"));
            objectEunm.Add(new KeyValuePair<string, string>("School Internet Type", "Common.Enums.InternetType"));
            objectEunm.Add(new KeyValuePair<string, string>("School Wireless Type", "Common.Enums.WirelessType"));
            objectEunm.Add(new KeyValuePair<string, string>("School Facility Type", "Schools.Enums.FacilityType"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("SchoolRegulatingEntity", "Schools.Entities.Regulating"));
            objectEunm.Add(new KeyValuePair<string, string>("SchoolVSDesignation", "Trs.TRSStarEnum"));
            objectEunm.Add(new KeyValuePair<string, string>("SchoolStarStatus", "Trs.TRSStarEnum"));
            #endregion

            #region Classroom
            objectEunm.Add(new KeyValuePair<string, string>("Classroom Status", "Common.Enums.EntityStatus"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("Classroom Intervention Status", "Classrooms.Enums.InterventionStatus"));
            objectEunm.Add(new KeyValuePair<string, string>("Classroom Internet Speed", "Common.Enums.InternetSpeed"));
            objectEunm.Add(new KeyValuePair<string, string>("Classroom Internet Type", "Common.Enums.InternetType"));
            objectEunm.Add(new KeyValuePair<string, string>("ClassroomWirelessType", "Common.Enums.WireLessType"));
            #endregion

            #region Class
            objectEunm.Add(new KeyValuePair<string, string>("Class Status", "Common.Enums.ExportEntityStatus"));
            objectEunm.Add(new KeyValuePair<string, string>("Class Day Type", "Classes.Enums.DayType"));
            objectEunm.Add(new KeyValuePair<string, string>("Class Used Equipment", "Classes.Enums.EquipmentType"));
            objectEunm.Add(new KeyValuePair<string, string>("Class Type", "Classes.Enums.ClassType"));
            //objectEunm.Add(new KeyValuePair<string, string>("Class level", "Classes.Enums.Classlevel"));
            objectEunm.Add(new KeyValuePair<string, string>("ClassTypeOfClass", "Classes.Enums.TRSClassofType"));
            #endregion

            #region Student
            objectEunm.Add(new KeyValuePair<string, string>("Student Status", "Common.Enums.ExportEntityStatus"));
            objectEunm.Add(new KeyValuePair<string, string>("Student Gender", "Common.Enums.Gender"));
            objectEunm.Add(new KeyValuePair<string, string>("Student Ethnicity", "Users.Enums.Ethnicity"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("StudentIsMediaRelease", "Students.Enums.MediaRelease"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("Student GradeLevel", "Students.Enums.StudentGradeLevel"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("Student Assessment Language", "Ade.StudentAssessmentLanguage"));
            #endregion

            #region Users
            objectEunm.Add(new KeyValuePair<string, string>("Role", "Users.Enums.Role"));
            objectEunm.Add(new KeyValuePair<string, string>("Status", "Common.Enums.ExportEntityStatus"));
            objectEunm.Add(new KeyValuePair<string, string>("Primary Number Type", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>("Secondary Number Type", "Common.Enums.PhoneType"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("Invitation Email", "Users.Entities.InvitationEmailEnum"));
            #endregion

            #region Teachers CoordCoachs Principals CommunityUsers
            objectEunm.Add(new KeyValuePair<string, string>("Gender", "Common.Enums.Gender"));
            #endregion

            #region Teachers CoordCoachs Principals
            objectEunm.Add(new KeyValuePair<string, string>("Ethnicity", "Users.Enums.Ethnicity"));
            #endregion

            #region Teachers CoordCoachs CommunityUsers
            objectEunm.Add(new KeyValuePair<string, string>("Education Level", "Users.Enums.EducationLevel"));
            #endregion

            #region Teachers
            objectEunm.Add(new KeyValuePair<string, string>("Teacher Type", "Users.Enums.TeacherType"));
            objectEunm.Add(new KeyValuePair<string, string>("Coach Assignment Way", "Users.Enums.AssignmentType"));
            objectEunm.Add(new KeyValuePair<string, string>
                ("ECIRCLE Assignment Way", "Users.Enums.AssignmentType"));
            objectEunm.Add(new KeyValuePair<string, string>("Employed By", "Users.Enums.EmployedBy"));
            objectEunm.Add(new KeyValuePair<string, string>("Media Release", "Students.Enums.MediaRelease"));
            #endregion

            #region CoordCoachs
            objectEunm.Add(new KeyValuePair<string, string>("Coaching Type", "Users.Enums.AssignmentType"));
            objectEunm.Add(new KeyValuePair<string, string>("Funded Through", "Users.Enums.FundedThrough"));
            #endregion
            return objectEunm;
        }
        #endregion

        #region GetNewRole
        private Role GetNewRole(UserBaseEntity user)
        {
            Role role = user.Role;
            Role newRole = role;
            switch (role)
            {
                case Role.District_Community_Delegate:
                    newRole = Role.Community;
                    break;
                case Role.Principal_Delegate:
                    newRole = Role.Principal;
                    break;
                case Role.TRS_Specialist_Delegate:
                    newRole = Role.TRS_Specialist;
                    break;
                case Role.School_Specialist_Delegate:
                    newRole = Role.School_Specialist;
                    break;
                case Role.Community_Specialist_Delegate:
                    newRole = Role.District_Community_Specialist;
                    break;
                default:
                    newRole = role;
                    break;
            }
            return newRole;
        }
        #endregion


        private Expression<Func<ReportTemplateEntity, bool>> GetRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<ReportTemplateEntity, bool>> condition = o => true;
            if (userInfo.Role != Role.Super_admin) //管理员可看所有的，其他角色只能看自己创建的
                condition = PredicateBuilder.And(condition, r => r.CreatedBy == userInfo.ID);
            return condition;
        }

        private Expression<Func<ReportTemplateEntity, bool>> GetReportRoleCondition(UserBaseEntity userInfo)
        {
            Expression<Func<ReportTemplateEntity, bool>> condition = o => true;
            condition = PredicateBuilder.And(condition, r => r.CreatedBy == userInfo.ID);
            return condition;
        }

        public OperationResult UpdateStatus(List<ExportInfoEntity> exportInfos, ExportInfoStatus status)
        {
            exportInfos.ForEach(e =>
            {
                e.Status = status;
                e.UpdatedOn = DateTime.Now;
            });
            return _exportContract.UpdateExportInfos(exportInfos);
        }
    }
}
