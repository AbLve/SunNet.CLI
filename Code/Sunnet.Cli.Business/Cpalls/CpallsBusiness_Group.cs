using System.Runtime.Remoting.Messaging;
using Sunnet.Cli.Business.Cpalls.Group;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using Sunnet.Cli.Core.CAC.Entities;

namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {
        public OperationResult AddCpallsStudentGroup(CpallsStudentGroupEntity entity)
        {
            return _cpallsContract.InsertCpallsStudentGroup(entity);
        }

        public OperationResult UpdateCpallsStudentGroup(CpallsStudentGroupEntity entity)
        {
            return _cpallsContract.UpdateCpallsStudentGroup(entity);
        }

        public bool CheckGroupName(CpallsStudentGroupEntity entity)
        {
            return _cpallsContract.CpallsStudentGroups.Where(r => r.ClassId == entity.ClassId && r.SchoolYear == entity.SchoolYear
                && r.Wave == entity.Wave && r.Language == entity.Language && r.AssessmentId == entity.AssessmentId && r.Name == entity.Name).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="wave">Wave</param>
        /// <param name="year">14-15</param>
        /// <param name="language">AssessmentLanguage</param>
        /// <returns></returns>
        public List<GroupModel> GetCpallsStudentGroupList(int classId, Wave wave, string year, AssessmentLanguage language, int AssessmentId,int userId)
        {
            List<GroupStudentModel> studentList = StudentBusiness.GetGroupStudentList(classId, language);
            IList<MyActivityEntity> activityList = CACBusiness.GetMyActivities(userId);
            List<GroupModel> list = _cpallsContract.CpallsStudentGroups.Where(r => r.ClassId == classId && r.Wave == wave
                && r.SchoolYear == year && r.Language == language && r.AssessmentId == AssessmentId).OrderBy(r => r.Name)
                .Select(r => new GroupModel()
                {
                    ID = r.ID,
                    Name = r.Name,
                    ClassId = r.ClassId,
                    StudentIds = r.StudentIds,
                    Note = r.Note,
                    GroupActivities = r.Activities
                }).ToList();

            foreach (GroupModel group in list)
            {
                group.StudentList = new List<GroupStudentModel>();
                if (group.StudentIds.EndsWith(","))
                    group.StudentIds = group.StudentIds.Substring(group.StudentIds.Length - 1);
                string[] ids = group.StudentIds.Split(',');
                if (ids.Length > 0)
                    group.StudentList = studentList.FindAll(r => ids.Contains(r.ID.ToString()));
                if (group.GroupActivities != null && group.GroupActivities.Count > 0)
                {
                   var myActivityIds = group.GroupActivities.Select(c => c.MyActivityId).ToList();
                    group.MyActivityList = activityList.Where(c => myActivityIds.Contains(c.ID)).ToList();
                }
                else
                {
                    group.MyActivityList = new List<MyActivityEntity>();
                }
            }
            return list;
        }


        public CpallsStudentGroupEntity GetCpallsStudentGroupEntity(int id)
        {
            return _cpallsContract.GetCpallsStudentGroupEntity(id);
        }

        public List<SelectItemModel> GetCpallsStudentItemModelList(int classId, string year, Wave wave, AssessmentLanguage language)
        {
            return _cpallsContract.CpallsStudentGroups.Where(r => r.ClassId == classId && r.Wave == wave && r.Language == language && r.SchoolYear == year)
                .Select(r => new SelectItemModel()
                {
                    ID = r.ID,
                    Name = r.Name
                }).ToList();
        }

        public OperationResult DeleteCpallsStudentGroup(int id)
        {
            return _cpallsContract.DeleteCpallsStudentGroup(id);
        }

        #region MeasureClassGroup

        public OperationResult InsertMeasureClassGroup(MeasureClassGroupEntity entity)
        {
            return _cpallsContract.InsertMeasureClassGroup(entity);
        }

        public OperationResult UpdateMeasureClassGroup(MeasureClassGroupEntity entity)
        {
            return _cpallsContract.UpdateMeasureClassGroup(entity);
        }

        public MeasureClassGroupEntity GetMeasureClassGroup(int measureId, int classId, int year, int wave)
        {
            return
                _cpallsContract.MeasureClassGroups.FirstOrDefault(e =>
                    e.ClassId == classId
                    && e.MeasureId == measureId
                    && e.Year == year
                    && e.Wave == wave);
        }

        #endregion

        public OperationResult SaveGroupActivities(int groupId, IList<int> list,int userId)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
           IList<CustomGroupMyActivityEntity> itemList = new List<CustomGroupMyActivityEntity>();
            foreach (var i in list)
            {
                var item = new CustomGroupMyActivityEntity();
                item.GroupId = groupId;
                item.MyActivityId = i;
                item.CreatedBy = userId;
                item.UpdatedBy = userId;
                itemList.Add(item);
            }
                res = _cpallsContract.DeleteGroupActivity(groupId);
            if (res.ResultType == OperationResultType.Success)
            {
                res = _cpallsContract.InsertGroupActivity(itemList);
            }
            return res;
        }

        public OperationResult DeleteGroupActivities(int activityId,  int userId)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            IList<CustomGroupMyActivityEntity> itemList = new List<CustomGroupMyActivityEntity>();
           
            res = _cpallsContract.DeleteGroupActivity(activityId,userId);
            
            return res;
        }
        public IList<CustomGroupMyActivityEntity> GetGroupActivities(int groupId)
        {
            return _cpallsContract.GroupActivities.Where(c=>c.GroupId == groupId).ToList();
        }
    }
}
