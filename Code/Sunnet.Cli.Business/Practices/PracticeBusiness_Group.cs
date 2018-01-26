using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Wordprocessing;
/**************************************************************************
 * Developer: 		Damon
 * Computer:		Damon-PC
 * Domain:			Damon-pc
 * CreatedOn:		2014/8/30 14:28:23
 * Description:		Please input class summary
 * Version History:	Created,2014/8/30 14:28:23
 **************************************************************************/
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Students.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Common;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.PDF;
using Sunnet.Cli.Business.Cpalls.Models;
using LinqKit;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Group;
using Sunnet.Cli.Business.Practices.Models;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Practices;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Core.Ade.Enums;
using Sunnet.Cli.Core.CAC.Entities;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Log.Entities;

namespace Sunnet.Cli.Business.Practices
{
    public partial class PracticeBusiness
    {

        #region Groups

        public void BuilderHeader(int assessmentId, int year, Wave wave, int userId, out List<MeasureHeaderModel> measures, out List<MeasureHeaderModel> parentMeasures, bool getAllWave = false)
        {
            var schoolYear = year.ToSchoolYearString();
            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                BuilderHeaderAlive(assessmentId, wave, out measures, out parentMeasures, getAllWave);
                return;
            }
            List<MeasureHeaderModel> list = new List<MeasureHeaderModel>();
            var w = ((int)wave).ToString();

            int studentAssessmentId = _practiceContract.Assessments.Where(r => r.AssessmentId == assessmentId
                && r.SchoolYear == schoolYear
                && r.Wave == wave && (r.CreatedBy == userId || r.CreatedBy == 0))
            .Select(r => r.ID).FirstOrDefault();

            list = _practiceContract.Measures.Where(r =>
                r.SAId == studentAssessmentId &&
                r.Measure.ApplyToWave.Contains(w)
                )
                .Select(r => new MeasureHeaderModel()
                {
                    ID = r.ID,
                    MeasureId = r.MeasureId,
                    Name = r.Measure.Name,
                    ParentId = r.Measure.ParentId,
                    TotalScored = r.TotalScored,
                    TotalScore = r.TotalScored ? r.TotalScore : (decimal?)null,
                    ParentMeasureName = r.Measure.Parent.Name,
                    Sort = r.Measure.Sort,
                    ApplyToWave = r.Measure.ApplyToWave,
                    PercentileRank = r.Measure.PercentileRank,
                    GroupByLabel = r.Measure.GroupByLabel
                }).ToList();
            list.ForEach(r =>
            {
                r.Subs = list.Count(l => l.ParentId == r.MeasureId);
                if (r.Subs > 0) r.Subs++;
            });

            parentMeasures = list.FindAll(r => r.ParentId == 1).OrderBy(r => r.Sort).ToList();
            measures = new List<MeasureHeaderModel>();

            foreach (MeasureHeaderModel parentHeader in parentMeasures)
            {
                if (parentHeader.Subs > 0)
                {
                    measures.AddRange(list.FindAll(r => r.ParentId == parentHeader.MeasureId)
                           .OrderBy(r => r.ParentId)
                           .ThenBy(r => r.Sort));
                    measures.Add(new MeasureHeaderModel()
                    {
                        ID = parentHeader.MeasureId,
                        Name = "Total",
                        MeasureId = parentHeader.MeasureId,
                        ParentId = parentHeader.MeasureId,
                        TotalScore = list.Where(r => r.ParentId == parentHeader.MeasureId)
                           .OrderBy(r => r.ParentId).Sum(x => x.TotalScore),
                        GroupByLabel = parentHeader.GroupByLabel
                    });

                }
                else
                    measures.Add(parentHeader);
            }
        }
        private void BuilderHeaderAlive(int assessmentId, Wave wave,
        out List<MeasureHeaderModel> measures, out List<MeasureHeaderModel> parentMeasures, bool getAllWave = false)
        {
            var w = ((int)wave).ToString();

            var key = string.Format("Assessment_{0}_Wave_{1}", assessmentId, getAllWave ? "All" : wave.ToString());
            var list = CacheHelper.Get<List<MeasureHeaderModel>>(key);
            if (list == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    list = CacheHelper.Get<List<MeasureHeaderModel>>(key);
                    if (list == null)
                    {
                        list = _adeBusiness.GetHeaderMeasures(x => x.AssessmentId == assessmentId && (x.ApplyToWave.Contains(w) || getAllWave));
                        list.ForEach(r => r.Subs = list.Any(l => l.ParentId == r.MeasureId) ? list.Count(l => l.ParentId == r.MeasureId) + 1 : 0);
                        CacheHelper.Add(key, list);
                    }
                }
            }

            parentMeasures = list.FindAll(r => r.ParentId == 1).OrderBy(r => r.Sort).ToList();
            measures = new List<MeasureHeaderModel>();

            foreach (MeasureHeaderModel parentHeader in parentMeasures)
            {
                if (parentHeader.Subs > 0)
                {
                    var children = list.FindAll(r => r.ParentId == parentHeader.MeasureId)
                        .OrderBy(r => r.Sort);
                    if (children != null && children.Any())
                        children.First().IsFirstOfParent = true;
                    measures.AddRange(children);
                    measures.Add(new MeasureHeaderModel()
                    {
                        ID = parentHeader.MeasureId,
                        Name = "Total",
                        MeasureId = parentHeader.MeasureId,
                        ParentId = parentHeader.MeasureId,
                        ParentMeasureName = parentHeader.Name,
                        TotalScore = list.Where(r => r.ParentId == parentHeader.MeasureId)
                           .OrderBy(r => r.ParentId).Sum(x => x.TotalScore),
                        IsLastOfParent = true,
                        Sort = parentHeader.Sort,
                        LightColor = parentHeader.LightColor,
                        PercentileRank = parentHeader.PercentileRank,
                        GroupByLabel = parentHeader.GroupByLabel
                    });
                }
                else
                    measures.Add(parentHeader);
            }
        }

        List<MeasureHeaderModel> BuilderHeader(int assessmentId, Wave wave)
        {
            CpallsHeaderModel headerModel = _adeBusiness.GetCpallsHeader(assessmentId, wave);
            List<MeasureHeaderModel> MeasureList = new List<MeasureHeaderModel>();
            if (headerModel != null)
            {
                MeasureList = headerModel.Measures.ToList();

                foreach (MeasureHeaderModel tmpItem in MeasureList.FindAll(r => r.ParentId == 1))
                {
                    tmpItem.Subs = MeasureList.Count(r => r.ParentId == tmpItem.MeasureId);
                }
            }
            return MeasureList;
        }

        public PracticeMeasureGroupEntity GetMeasureClassGroup(int measureId, int assessmentId, int year, int wave)
        {
            return
                _practiceContract.MeasureGroups.FirstOrDefault(e =>
                    e.AssessmentId == assessmentId
                    && e.MeasureId == measureId
                    && e.Year == year
                    && e.Wave == wave);
        }

        public OperationResult AddPracticeStudentGroup(PracticeStudentGroupEntity entity)
        {
            return _practiceContract.InsertCpallsStudentGroup(entity);
        }

        public OperationResult UpdatePracticeStudentGroup(PracticeStudentGroupEntity entity)
        {
            return _practiceContract.UpdateCpallsStudentGroup(entity);
        }

        public PracticeStudentGroupEntity GetPracticeStudentGroupEntity(int id)
        {
            return _practiceContract.GetCpallsStudentGroupEntity(id);
        }
        public List<GroupModel> GetCpallsStudentGroupList(int userId, Wave wave, string year, AssessmentLanguage language, int assessmentId)
        {
            List<GroupStudentModel> studentList = GetGroupStudentList(assessmentId, language);
            IList<MyActivityEntity> activityList = CACBusiness.GetMyActivities(userId);

            List<GroupModel> list = _practiceContract.StudentGroups.Where(r => r.Wave == wave
                && r.SchoolYear == year && r.Language == language && r.AssessmentId == assessmentId && r.CreatedBy == userId).OrderBy(r => r.Name)
                .Select(r => new GroupModel()
                {
                    ID = r.ID,
                    Name = r.Name,
                    StudentIds = r.StudentIds,
                    Note = r.Note,
                    PracticeGroupActivities = r.Activities
                }).ToList();

            foreach (GroupModel group in list)
            {
                group.StudentList = new List<GroupStudentModel>();
                if (group.StudentIds.EndsWith(","))
                    group.StudentIds = group.StudentIds.Substring(group.StudentIds.Length - 1);
                string[] ids = group.StudentIds.Split(',');
                if (ids.Length > 0)
                    group.StudentList = studentList.FindAll(r => ids.Contains(r.ID.ToString()));
                if (group.PracticeGroupActivities != null && group.PracticeGroupActivities.Count > 0)
                {
                    var myActivityIds = group.PracticeGroupActivities.Select(c => c.MyActivityId).ToList();
                    group.MyActivityList = activityList.Where(c => myActivityIds.Contains(c.ID)).ToList();
                }
                else
                {
                    group.MyActivityList = new List<MyActivityEntity>();
                }
            }
            return list;
        }

        public OperationResult DeletePracticeStudentGroup(int id)
        {
            return _practiceContract.DeleteCpallsStudentGroup(id);
        }
        public bool CheckGroupName(PracticeStudentGroupEntity entity)
        {
            return _practiceContract.StudentGroups.Any(r => r.SchoolYear == entity.SchoolYear
                && r.Wave == entity.Wave && r.Language == entity.Language && r.AssessmentId == entity.AssessmentId && r.Name == entity.Name);
        }

        public List<GroupStudentModel> GetGroupStudentList(int assessmentId, AssessmentLanguage language)
        {
            var query = _practiceContract.Students.Where(r => r.AssessmentId == assessmentId && r.Status == EntityStatus.Active);
            if (language == AssessmentLanguage.English)
                query = query.Where(r => r.AssessmentLanguage == StudentAssessmentLanguage.English || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);
            else
                query = query.Where(r => r.AssessmentLanguage == StudentAssessmentLanguage.Spanish || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);

            return query.Select(r => new GroupStudentModel
            {
                ID = r.ID,
                FirstName = r.StudentName,
                LastName = ""
            }).OrderBy(r => r.FirstName).ToList();
        }
        #endregion
        #region MeasureClassGroup

        public OperationResult InsertMeasureClassGroup(PracticeMeasureGroupEntity entity)
        {
            return _practiceContract.InsertPracticeMeasureGroup(entity);
        }

        public OperationResult UpdateMeasureClassGroup(PracticeMeasureGroupEntity entity)
        {
            return _practiceContract.UpdatePracticeMeasureGroup(entity);
        }

        public PracticeMeasureGroupEntity GetMeasureClassGroup(int measureId, int year, int wave)
        {
            return
                _practiceContract.PracticeMeasureGroups.FirstOrDefault(e =>
                     e.MeasureId == measureId
                    && e.Year == year
                    && e.Wave == wave);
        }

        #endregion


        #region Practice Group Activity
        public OperationResult SavePracticeGroupActivities(int groupId, IList<int> list, int userId)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            IList<PracticeGroupMyActivityEntity> itemList = new List<PracticeGroupMyActivityEntity>();
            foreach (var i in list)
            {
                var item = new PracticeGroupMyActivityEntity();
                item.GroupId = groupId;
                item.MyActivityId = i;
                item.CreatedBy = userId;
                item.UpdatedBy = userId;
                itemList.Add(item);
            }
            res = _practiceContract.DeletePracticeGroupActivity(groupId);
            if (res.ResultType == OperationResultType.Success)
            {
                res = _practiceContract.InsertPracticeGroupActivity(itemList);
            }
            return res;
        }

        public OperationResult DeletePracticeGroupActivities(int activityId, int userId)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            IList<PracticeGroupMyActivityEntity> itemList = new List<PracticeGroupMyActivityEntity>();

            res = _practiceContract.DeletePracticeGroupActivity(activityId, userId);

            return res;
        }
        public IList<PracticeGroupMyActivityEntity> GetPracticeGroupActivities(int groupId)
        {
            return _practiceContract.PracticeGroupActivities.Where(c => c.GroupId == groupId).ToList();
        }
        #endregion
    }
}
