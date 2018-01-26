using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 5:49:36
 * Description:		主类：负责School View，Class View，Student View 的数据展示以及其他相关逻辑
 * Version History:	Created,2014/9/4 5:49:36
 * 
 * 
 **************************************************************************/
using System.Web.Caching;
using LinqKit;
using StructureMap;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Log;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Log;
using Extensions = LinqKit.Extensions;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Students.Enums;

namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {
        #region Private Fields
        private readonly ICpallsContract _cpallsContract;
        private readonly IAdeDataContract _adeData;
        private readonly ISunnetLog _logger;

        /// <summary>
        /// 可能为空，请使用ClassBusiness私有属性
        /// </summary>
        private ClassBusiness _classBusiness;

        /// <summary>
        /// 可能为空，请使用AdeBusiness私有属性
        /// </summary>
        private AdeBusiness _adeBusiness;
        /// <summary>
        /// 可能为空，请使用StudentBusiness私有属性
        /// </summary>
        private StudentBusiness _studentBusiness;
        /// <summary>
        /// 可能为空，请使用SchoolBusiness私有属性
        /// </summary>
        private SchoolBusiness _schoolBusiness;
        /// <summary>
        /// 可能为空，请使用CommunityBusiness私有属性
        /// </summary>
        private CommunityBusiness _communityBusiness;
        /// <summary>
        /// 可能为空，请使用CACBusiness私有属性
        /// </summary>
        private CACBusiness _cacBusiness;
        private OperationLogBusiness _logBusiness;

        private ClassBusiness ClassBusiness
        {
            get { return _classBusiness ?? (_classBusiness = new ClassBusiness()); }
            set { _classBusiness = value; }
        }

        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        private StudentBusiness StudentBusiness
        {
            get { return _studentBusiness ?? (_studentBusiness = new StudentBusiness()); }
            set { _studentBusiness = value; }
        }
        private CACBusiness CACBusiness
        {
            get { return _cacBusiness ?? (_cacBusiness = new CACBusiness()); }
            set { _cacBusiness = value; }
        }

        private SchoolBusiness SchoolBusiness
        {
            get { return _schoolBusiness ?? (_schoolBusiness = new SchoolBusiness()); }
            set { _schoolBusiness = value; }
        }

        private CommunityBusiness CommunityBusiness
        {
            get { return _communityBusiness ?? (_communityBusiness = new CommunityBusiness()); }
            set { _communityBusiness = value; }
        }

        private OperationLogBusiness LogBusiness
        {
            get { return _logBusiness ?? (_logBusiness = new OperationLogBusiness()); }
        }

        #endregion

        #region Selectors
        private static Expression<Func<MeasureEntity, ExecCpallsMeasureModel>> SelectorMeasureEntityToCpallsModel
        {
            get
            {
                return x => new ExecCpallsMeasureModel()
                {
                    ExecId = 0,
                    MeasureId = x.ID,
                    OrderType = x.OrderType,
                    ShowType = x.ItemType,
                    Name = x.Name,
                    InnerTime = x.InnerTime,
                    Timeout = x.Timeout,
                    StartPageHtml = x.StartPageHtml,
                    EndPageHtml = x.EndPageHtml,
                    PauseTime = 0,
                    Status = CpallsStatus.Initialised,
                    Benchmark = 0,
                    TotalScore = x.TotalScore,
                    TotalScored = x.TotalScored,
                    ApplyToWave = x.ApplyToWave,
                    Goal = 0,
                    UpdatedOn = x.UpdatedOn,
                    Comment = string.Empty,
                    Parent = new ExecCpallsParentMeasureModel()
                    {
                        ID = x.Parent.ID,
                        Name = x.Parent.Name,
                        StartPageHtml = x.Parent.StartPageHtml,
                        EndPageHtml = x.Parent.EndPageHtml
                    },
                    IsParent = x.SubMeasures.Any(),
                    Children = x.SubMeasures.Count(y => y.Status == EntityStatus.Active && y.IsDeleted == false),
                    ParentSort = x.ParentId > 1 ? x.Parent.Sort : x.Sort,
                    Sort = x.ParentId > 1 ? x.Parent.Sort * 10 + x.Sort : x.Sort,
                    StopButton = x.StopButton,
                    PreviousButton = x.PreviousButton,
                    NextButton = x.NextButton
                };
            }
        }

        private static Expression<Func<StudentAssessmentEntity, ExecCpallsAssessmentModel>> SelectorAssEntityToModel
        {
            get
            {
                return x => new ExecCpallsAssessmentModel()
                {
                    ExecId = x.ID,
                    AssessmentId = x.AssessmentId,
                    CreatedOn = x.CreatedOn,
                    OrderType = x.Assessment.OrderType,
                    Language = x.Assessment.Language,
                    Name = x.Assessment.Name,
                    SchoolYear = x.SchoolYear,
                    StudentId = x.StudentId,
                    Wave = x.Wave
                };
            }
        }

        #endregion

        #region Others
        private class CompareMeasure : IEqualityComparer<StudentMeasureRecordModel>
        {
            public bool Equals(StudentMeasureRecordModel x, StudentMeasureRecordModel y)
            {
                return x.MeasureId == y.MeasureId
                    && x.StudentId == y.StudentId
                    && x.StudentAssessmentId == y.StudentAssessmentId;
            }

            public int GetHashCode(StudentMeasureRecordModel obj)
            {
                var measure = new { obj.MeasureId, obj.StudentId, obj.StudentAssessmentId };
                return measure.GetHashCode();
            }
        }
        #endregion

        public CpallsBusiness(bool istest)
        {

        }

        public CpallsBusiness(AdeUnitOfWorkContext unit = null)
        {
            _cpallsContract = DomainFacade.CreateCpallsService(unit);
            _adeData = DomainFacade.CreateAdeDataService(unit);

            _logger = ObjectFactory.GetInstance<ISunnetLog>();

            if (unit != null)
                AdeBusiness = new AdeBusiness(unit);
            ClassBusiness = new ClassBusiness();
        }

        /// <summary>
        /// Calculates the benchmark.
        /// </summary>
        /// <param name="cutoffScores">The cutoff scores.</param>
        /// <param name="birthday">The birthday.</param>
        /// <param name="schoolYear">The school year:14-15.</param>
        /// <returns></returns>
        public static decimal CalculateBenchmark(List<CutOffScoreEntity> cutoffScores, DateTime birthday, string schoolYear)
        {
            decimal bentchmark = -1;
            var score = CalculateBenchmarkEntity(cutoffScores, birthday, schoolYear);
            if (score != null)
                bentchmark = score.CutOffScore;
            return bentchmark;
        }
        public static decimal CalculateBenchmark(List<CutOffScoreEntity> cutoffScores, int ageYear, int ageMonth, string schoolYear)
        {
            decimal bentchmark = -1;
            var score = CalculateBenchmarkEntity(cutoffScores, ageYear, ageMonth, schoolYear);
            if (score != null)
                bentchmark = score.CutOffScore;
            return bentchmark;
        }
        /// <summary>
        /// Calculates the benchmark entity.
        /// </summary>
        /// <param name="cutoffScores">The cutoff scores.</param>
        /// <param name="birthday">The birthday.</param>
        /// <param name="schoolYear">The school year:14-15.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">A measure must has at least one Cutoff scores</exception>
        public static CutOffScoreEntity CalculateBenchmarkEntity(List<CutOffScoreEntity> cutoffScores, DateTime birthday, string schoolYear)
        {
            if (cutoffScores == null || cutoffScores.Count == 0)
                throw new Exception("A measure must has at least one Cutoff scores");
            var now = CommonAgent.GetStartDateForAge(schoolYear);
            birthday = birthday.Date;
            DateTime start, end;

            CutOffScoreEntity score = null;
            foreach (var cutoffScore in cutoffScores)
            {
                start = now.AddYears(-cutoffScore.ToYear).AddMonths(-cutoffScore.ToMonth);
                end = now.AddYears(-cutoffScore.FromYear).AddMonths(-cutoffScore.FromMonth);
                if (start < birthday && birthday <= end)
                {
                    score = cutoffScore;
                    break;
                }
            }
            return score;
        }
        public static CutOffScoreEntity CalculateBenchmarkEntity(List<CutOffScoreEntity> cutoffScores, int ageYear, int ageMonth, string schoolYear)
        {
            if (cutoffScores == null || cutoffScores.Count == 0)
                throw new Exception("A measure must has at least one Cutoff scores");
            var now = CommonAgent.GetStartDateForAge(schoolYear);
            CutOffScoreEntity score = null;
            foreach (var cutoffScore in cutoffScores)
            {
                if (ageYear >= cutoffScore.FromYear && ageMonth >= cutoffScore.FromMonth && ageYear <= cutoffScore.ToYear && ageMonth <= cutoffScore.ToMonth)
                {
                    score = cutoffScore;
                    break;
                }
            }
            return score;
        }
        List<MeasureHeaderModel> BuilderHeader(int assessmentId, Wave wave)
        {
            CpallsHeaderModel headerModel = AdeBusiness.GetCpallsHeader(assessmentId, wave);
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

        /// <summary>
        /// 得到Header Measure(实时)
        /// 缓存，由ADE更新维护
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="wave">The wave.</param>
        /// <param name="measures">The measure list.</param>
        /// <param name="parentMeasures">The parent measure list.</param>
        /// <param name="getAllWave">是否获取所有的Wave(否：只获取ApplyToWave包含<paramref name="wave"/>的Measure)</param>
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
                        list = AdeBusiness.GetHeaderMeasures(x => x.AssessmentId == assessmentId && (x.ApplyToWave.Contains(w) || getAllWave));
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

        /// <summary>
        /// Builders the header.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="year">The school year.2014</param>
        /// <param name="wave">The wave.</param>
        /// <param name="measures">The measure list.</param>
        /// <param name="parentMeasures">The parent measure list.</param>
        /// <param name="getAllWave"></param>
        public void BuilderHeader(int assessmentId, int year, Wave wave,
            out List<MeasureHeaderModel> measures, out List<MeasureHeaderModel> parentMeasures, bool getAllWave = false)
        {
            var schoolYear = year.ToSchoolYearString();
            if (CommonAgent.IsCurrentSchoolYear(year))
            {
                BuilderHeaderAlive(assessmentId, wave, out measures, out parentMeasures, getAllWave);
                return;
            }
            List<MeasureHeaderModel> list = new List<MeasureHeaderModel>();
            var w = ((int)wave).ToString();

            int studentAssessmentId = _cpallsContract.Assessments.Where(r => r.AssessmentId == assessmentId
                && r.SchoolYear == schoolYear
                && r.Wave == wave)
            .Select(r => r.ID).FirstOrDefault();

            list = _cpallsContract.Measures.Where(r =>
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

        private List<CpallsSchoolModel> GetCpallsSchool(UserBaseEntity user, List<CpallsSchoolModel> schools, int assessmentId, Wave wave,
            string sort, string order, bool getAll = false)
        {
            IList<int> schoolIds = schools.Select(c => c.ID).ToList();
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolIds.ToList(), user, assessmentId);

            List<SchoolMeasureGoalModel> goalList = _cpallsContract.GetSchoolMeasureGoal(schoolIds.ToList()
                , CommonAgent.SchoolYear, wave, assessmentId, classIds);

            var measureTree = AdeBusiness.GetMeasureTree(assessmentId);
            foreach (CpallsSchoolModel item in schools)
            {
                item.MeasureList = goalList.FindAll(r => r.SchoolId == item.ID);
                measureTree.ForEach(m =>
                {
                    var parentMeasure = item.MeasureList.FirstOrDefault(x => x.MeasureId == m.Key);
                    if (parentMeasure != null)
                    {
                        parentMeasure.Total = item.MeasureList.Where(x => m.Value.Contains(x.MeasureId) && x.TotalScored)
                            .Sum(x => x.AverageOrDefault);
                    }
                });
            }
            return schools;
        }

        /// <summary>
        /// 当前年的school 列表
        /// 缓存，定时过期
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<CpallsSchoolModel> GetCpallsSchool(UserBaseEntity user, Expression<Func<SchoolEntity, bool>> condition, int assessmentId, Wave wave,
            string sort, string order, int first, int count, out int total)
        {
            //var sort2 = sort.Equals("Name", StringComparison.CurrentCultureIgnoreCase) ? "BasicSchool.Name" : sort;
            //var schoolIds = SchoolBusiness.GetSchoolIds(condition, user, sort, order, first, count, out total);
            condition = condition.And(c => c.Status == SchoolStatus.Active);
            List<CpallsSchoolModel> schools = SchoolBusiness.GetSchoolList(condition, user, sort, order, first, count, out total);

            var schoolIds = schools.Select(c => c.ID).ToList();

            var key = string.Format("Assessment_{0}_Wave_{1}_Schools", assessmentId, wave);
            List<CpallsSchoolModel> allSchools = CacheHelper.Get<List<CpallsSchoolModel>>(key);

            if (allSchools == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allSchools = CacheHelper.Get<List<CpallsSchoolModel>>(key);
                    if (allSchools == null)
                    {

                        allSchools = GetCpallsSchool(user, schools, assessmentId, wave, sort, order);
                        CacheHelper.Add(key, allSchools, CacheHelper.DefaultExpiredSeconds);
                    }
                }
            }


            var list = allSchools.Where(x => schoolIds.Contains(x.ID)).DistinctBy(x => x.ID).OrderBy(sort, order).ToList();
            var schoolsNeedtoGetFromDb = schoolIds.Except(list.Select(x => x.ID)).ToList();
            if (schoolsNeedtoGetFromDb.Any())
            {
                lock (CacheHelper.Synchronize)
                {
                    var needFromDbList = SchoolBusiness.GetSchoolList(c => schoolsNeedtoGetFromDb.Contains(c.ID), user, sort, order, 0, int.MaxValue, out total);
                    var extra = GetCpallsSchool(user, needFromDbList, assessmentId, wave, sort, order, false);
                    list.AddRange(extra);
                    IEnumerable<CpallsSchoolModel> needToCache = extra.Except(allSchools, new CompareCpallsSchoolModel());
                    allSchools.AddRange(needToCache);
                }
            }
            return list;
        }

        private class CompareCpallsSchoolModel : IEqualityComparer<CpallsSchoolModel>
        {

            public bool Equals(CpallsSchoolModel x, CpallsSchoolModel y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(CpallsSchoolModel obj)
            {
                return obj.ID.GetHashCode();
            }
        }

        private List<CpallsClassModel> GetCpallsClass(UserBaseEntity user, List<CpallsClassModel> classes, int assessmentId, Wave wave,
            string sort, string order, bool getAll)
        {
            int total;
            //Stopwatch s = new Stopwatch();
            //s.Start();
            //var times = "";
            //List<CpallsClassModel> list = ClassBusiness.GetClassList(x => x.Status == EntityStatus.Active && (classIds.Contains(x.ID)), user,
            //    sort, order, 0, int.MaxValue, out total);
            //s.Stop();
            //times += "    Get Class Time:" + s.Elapsed.TotalMilliseconds;
            //s.Restart();
            List<int> studentIds = new List<int>();
            foreach (CpallsClassModel model in classes)
                studentIds.AddRange(model.StudentIds.ToList());
            //s.Stop();
            //times += "    Get Student ID Time:" + s.Elapsed.TotalMilliseconds;
            //s.Restart();
            // todo:优化效率
            List<StudentMeasureGoalModel> studentList = _cpallsContract.GetStudentMeasureGoal(studentIds, CommonAgent.SchoolYear, wave, assessmentId);
            var measureTree = AdeBusiness.GetMeasureTree(assessmentId);
            //s.Stop();
            //times += "    Get Records Time:" + s.Elapsed.TotalMilliseconds;
            //s.Restart();
            foreach (CpallsClassModel item in classes)
            {
                var classMeasures = studentList.FindAll(r => item.StudentIds.Contains(r.StudentId))
                       .GroupBy(r => r.MeasureId).Select(r => new SchoolMeasureGoalModel()
                       {
                           MeasureId = r.Key,
                           Goal = r.Sum(o => o.Goal),
                           Amount = r.Sum(o => o.Amount),
                           TotalScored = r.First().TotalScored
                       }).ToList();
                measureTree.ForEach(m =>
                {
                    var parentMeasure = classMeasures.Find(x => x.MeasureId == m.Key);
                    if (parentMeasure != null)
                    {
                        parentMeasure.Total =
                            classMeasures.Where(x => m.Value.Contains(x.MeasureId) && x.TotalScored).Sum(x => x.AverageOrDefault);
                    }
                });
                item.MeasureList = classMeasures;
            }
            //s.Stop();
            //times += "    Calc Time:" + s.Elapsed.TotalMilliseconds;
            //_logger.Log(times);
            return classes;
        }

        /// <summary>
        /// 获取当年的class列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="assessmentId"></param>
        /// <param name="sort"></param>
        /// <param name="order"></param>
        /// <param name="first"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<CpallsClassModel> GetCpallsClass(UserBaseEntity user, Expression<Func<ClassEntity, bool>> condition, int assessmentId, Wave wave,
            string sort, string order, int first, int count, out int total)
        {
            //var classesIds = ClassBusiness.GetClassesIds(condition, user, sort, order, first, count, out total);
            //condition = condition.And(c=>c.Status == EntityStatus.Active);

            List<CpallsClassModel> classes = ClassBusiness.GetClassList(condition, user,
              sort, order, first, count, out total);

            var classesIds = classes.Select(c => c.ID).ToList();

            var key = string.Format("Assessment_{0}_Wave_{1}_Classes", assessmentId, wave);
            var allClasses = CacheHelper.Get<List<CpallsClassModel>>(key);
            if (allClasses == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allClasses = CacheHelper.Get<List<CpallsClassModel>>(key);
                    if (allClasses == null)
                    {
                        allClasses = GetCpallsClass(user, classes, assessmentId, wave, sort, order, true);
                        CacheHelper.Add(key, allClasses, CacheHelper.DefaultExpiredSeconds);
                    }
                }
            }
            List<CpallsClassModel> list = allClasses.FindAll(x => classesIds.Contains(x.ID)).OrderBy(sort, order).ToList();
            var classesNeedToGetFromDb = classesIds.Except(list.Select(x => x.ID)).ToList();
            if (classesNeedToGetFromDb.Any())
            {
                lock (CacheHelper.Synchronize)
                {
                    List<CpallsClassModel> needFromDB = ClassBusiness.GetClassList(c => (classesNeedToGetFromDb.Contains(c.ID)), user,
             sort, order, 0, int.MaxValue, out total);
                    var extra = GetCpallsClass(user, needFromDB, assessmentId, wave, sort, order, false);
                    list.AddRange(extra);
                    var needToCache = extra.Except(allClasses, new CompareCpallsClassModel());
                    allClasses.AddRange(needToCache);
                }
            }
            return list;
        }




        private class CompareCpallsClassModel : IEqualityComparer<CpallsClassModel>
        {

            public bool Equals(CpallsClassModel x, CpallsClassModel y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(CpallsClassModel obj)
            {
                return obj.ID.GetHashCode();
            }
        }

        private List<CpallsStudentModel> GetStudentList(UserBaseEntity userInfo, int assessmentId, Wave wave, int year,
            IEnumerable<int> studentIds,
            string sort, string order)
        {
            var schoolYear = year.ToSchoolYearString();
            List<StudentMeasureRecordModel> measureList =
                _cpallsContract.Measures.AsExpandable().Where(r => r.Assessment.AssessmentId == assessmentId
                                                                   && studentIds.Contains(r.Assessment.StudentId)
                                                                   && r.Assessment.Wave == wave
                                                                   && r.Assessment.SchoolYear == schoolYear
                    )
                    .Select(SelectorSMEntityToSMModel).ToList();
            int total;
            List<CpallsStudentModel> studentList = StudentBusiness.GetClassStudents(r => studentIds.Contains(r.ID), userInfo,
                sort, order, 0, int.MaxValue, out total);

            studentList.ForEach(x => x.SchoolYear = year);

            var benchmarks = _adeBusiness.GetBenchmarks(assessmentId);
            var measureTree = AdeBusiness.GetMeasureTree(assessmentId);
            foreach (CpallsStudentModel model in studentList)
            {
                List<StudentMeasureRecordModel> tmpMeasureList = measureList.FindAll(r => r.StudentId == model.ID)
                    .Distinct(new CompareMeasure()).ToList();
                if (tmpMeasureList.Any())
                {
                    model.StudentAssessmentId = tmpMeasureList[0].StudentAssessmentId;
                    tmpMeasureList.ForEach(measure =>
                    {
                        measure.Age = model.Age;
                        measure.BenchmarkColor = benchmarks.FirstOrDefault(e => e.ID == measure.BenchmarkId) == null
                            ? ""
                            : benchmarks.FirstOrDefault(e => e.ID == measure.BenchmarkId).Color;
                    });
                    model.MeasureList.AddRange(tmpMeasureList);

                    CpallsStudentModel model1 = model;
                    measureTree.ForEach(m =>
                    {
                        var parentMeasure = model1.MeasureList.LastOrDefault(x => x.MeasureId == m.Key);
                        if (parentMeasure != null)
                        {
                            parentMeasure.IsTotal = true;
                            parentMeasure.BenchmarkColor = benchmarks.FirstOrDefault(e => e.ID == parentMeasure.BenchmarkId) == null
                            ? ""
                            : benchmarks.FirstOrDefault(e => e.ID == parentMeasure.BenchmarkId).Color;
                        }
                    });
                }
            }

            return studentList;
        }

        private static Expression<Func<StudentMeasureEntity, StudentMeasureRecordModel>> SelectorSMEntityToSMModel
        {
            get
            {
                return r => new StudentMeasureRecordModel
                {
                    ID = r.ID,
                    StudentAssessmentId = r.Assessment.ID,
                    MeasureId = r.MeasureId,
                    MeasureName = r.Measure.Name,
                    Status = r.Status,
                    SchoolYear = r.Assessment.SchoolYear,
                    Wave = r.Assessment.Wave,
                    StudentId = r.Assessment.StudentId,
                    Benchmark = r.Benchmark,
                    TotalScored = r.Measure.TotalScored,
                    Goal = r.Goal,
                    LightColor = r.Measure.LightColor,
                    HasCutOffScores = r.Measure.HasCutOffScores,
                    BOYHasCutOffScores = r.Measure.BOYHasCutOffScores,
                    MOYHasCutOffScores = r.Measure.MOYHasCutOffScores,
                    EOYHasCutOffScores = r.Measure.EOYHasCutOffScores,
                    GroupByParentMeasure = r.Measure.GroupByParentMeasure,
                    PercentileRank = r.PercentileRank,
                    BenchmarkId = r.BenchmarkId,
                    LowerScore = r.LowerScore,
                    HigherScore = r.HigherScore,
                    ShowOnGroup = r.ShowOnGroup
                };
            }
        }

        public List<CpallsStudentModel> GetStudentList(int schoolId, UserBaseEntity userInfo, int assessmentId, Wave wave, int year,
            Expression<Func<StudentEntity, bool>> studentCondition,
            string sort, string order, int first, int count, out int total, int sortMeasureId = 0, bool isDisplayRanks = false)
        {
            if (sortMeasureId != 0)
            {
                return GetSortStudentList(schoolId, userInfo, assessmentId, wave, year, studentCondition, sort, order, first, count, out total, sortMeasureId, isDisplayRanks);
            }
            IEnumerable<int> studentIds;
            studentIds = StudentBusiness.GetStudentId(studentCondition, userInfo, sort, order, first, count, out total).ToList();

            var key = string.Format("Assessment_{0}_Wave_{1}_School_{2}_Students", assessmentId, wave, schoolId);
            var allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
            if (allStudents == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
                    if (allStudents == null)
                    {
                        allStudents = GetStudentList(userInfo, assessmentId, wave, year, studentIds, sort, order);
                        CacheHelper.Add(key, allStudents);
                    }
                }
            }
            var datetime = DateTime.Now.AddSeconds(0 - CacheHelper.Student_View_ExpiredSeconds);
            //去掉缓存过期的学生
            var expiredStudents = allStudents.FindAll(x => x.CachedOn <= datetime);
            expiredStudents.ForEach(stu => allStudents.Remove(stu));

            var list = allStudents.FindAll(x => studentIds.Contains(x.ID)).OrderBy(sort, order).ToList();

            //1、改变了生日的学生；2、改变了CutOffScore的Measure影响的学生；并且在缓存中，需要更新缓存
            //var listIds = list.Select(x => x.ID).ToList();
            //var stuChangedIds = _cpallsContract.Assessments
            //    .Where(x => x.Measures.Any(m => m.BenchmarkChanged == true)
            //        && listIds.Contains(x.StudentId)
            //        && x.AssessmentId == assessmentId
            //        && x.Wave == wave)
            //    .Select(x => x.StudentId).ToList();

            //去掉1、改变了生日的学生；2、改变了CutOffScore的Measure影响的学生；
            //var dobChangedStudents = allStudents.FindAll(x => stuChangedIds.Contains(x.ID));
            //dobChangedStudents.ForEach(stu => allStudents.Remove(stu));

            //list = list.Where(x => !stuChangedIds.Contains(x.ID)).ToList();
            var studentsNeedToGetFromDb = studentIds.Except(list.Select(x => x.ID)).ToList();
            if (studentsNeedToGetFromDb.Any())
            {
                lock (CacheHelper.Synchronize)
                {
                    var extra = GetStudentList(userInfo, assessmentId, wave, year, studentsNeedToGetFromDb.ToList(),
                        sort, order);
                    list.AddRange(extra);
                    var needToCache = extra.Except(allStudents, new CompareCpallsStudentModel());
                    allStudents.AddRange(needToCache);
                }
            }

            //if (stuChangedIds.Any())
            //{
            //    var stuMeasures = _cpallsContract.Measures
            //        .Where(x => stuChangedIds.Contains(x.Assessment.StudentId)
            //            && x.Assessment.AssessmentId == assessmentId
            //            && x.Assessment.Wave == wave
            //            && x.BenchmarkChanged == true).ToList();
            //    stuMeasures.ForEach(x => x.BenchmarkChanged = false);
            //    _cpallsContract.UpdateStudentMeasures(stuMeasures);
            //}
            return list;
        }

        public List<CpallsStudentModel> GetSortStudentList(int schoolId, UserBaseEntity userInfo, int assessmentId, Wave wave, int year,
            Expression<Func<StudentEntity, bool>> studentCondition,
            string sort, string order, int first, int count, out int total, int sortMeasureId, bool isDisplayRank = false)
        {
            IEnumerable<int> studentIds = StudentBusiness.GetStudentIdForCpallsGoalSort(studentCondition, userInfo, out total).ToList();
            var key = string.Format("Assessment_{0}_Wave_{1}_School_{2}_Students", assessmentId, wave, schoolId);
            var allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
            if (allStudents == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
                    if (allStudents == null)
                    {
                        allStudents = GetStudentList(userInfo, assessmentId, wave, year, studentIds, sort, order);
                        CacheHelper.Add(key, allStudents);
                    }
                }
            }
            var list = allStudents.FindAll(x => studentIds.Contains(x.ID)).OrderBy(sort, order).ToList();
            if (sortMeasureId != 0)
            {
                if (isDisplayRank)
                {
                    /*//aaabbbccc: David 排序问题，请处理
                    if (order.ToUpper() == "ASC")
                        list = list.OrderBy(r => r.DicMeasure == null ? 0 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : 0)
                            .ThenBy(r => r.FirstName + r.LastName).ToList();
                    else if (order.ToUpper() == "DESC")
                        list = list.OrderByDescending(r => r.DicMeasure == null ? 0 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : 0)
                            .ThenBy(r => r.FirstName + r.LastName).ToList();
                            */
                    if (order.ToUpper() == "ASC")
                        list = list.OrderBy(r => r.DicMeasure == null ? "N/A" : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : "N/A")
                            .ThenBy(r => r.FirstName + r.LastName).ToList();
                    else if (order.ToUpper() == "DESC")
                        list = list.OrderByDescending(r => r.DicMeasure == null ? "N/A" : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : "N/A")
                            .ThenBy(r => r.FirstName + r.LastName).ToList();
                }
                else
                {
                    if (order.ToUpper() == "ASC")
                        list = list.OrderBy(r => r.DicMeasure == null ? -1 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].Goal : -1)
                            .ThenBy(r => r.FirstName + r.LastName).ToList();
                    else if (order.ToUpper() == "DESC")
                        list = list.OrderByDescending(r => r.DicMeasure == null ? -1 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].Goal : -1)
                            .ThenBy(r => r.FirstName + r.LastName).ToList();
                }
                list = list.Skip(first).Take(count).ToList();
            }
            return list;
        }

        private class CompareCpallsStudentModel : IEqualityComparer<CpallsStudentModel>
        {

            public bool Equals(CpallsStudentModel x, CpallsStudentModel y)
            {
                return x.ID == y.ID;
            }

            public int GetHashCode(CpallsStudentModel obj)
            {
                return obj.ID.GetHashCode();
            }
        }

        public OperationResult InitMeasures(CpallsStudentModel student, Wave w, string schoolYear, int assessmentId,
            IEnumerable<int> measureIds, UserBaseEntity userInfo, out int studentAssessmentId)
        {
            var result = _cpallsContract.InitMeasures(userInfo.ID, assessmentId,
                schoolYear, student.ID, student.BirthDate, w, measureIds);
            studentAssessmentId = result.AppendData.CastTo<int>();
            return result;
        }

        public OperationResult InsertAssessment(CpallsStudentModel student, Wave w, string schoolYear, int assessmentId, IEnumerable<int> measureIds, UserBaseEntity userInfo, out int studentAssessmentId)
        {
            var key = string.Format("Assessment_{0}_Wave_{1}_School_{2}_Students", assessmentId, w, student.SchoolId);
            var allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
            if (allStudents == null)
                return InitMeasures(student, w, schoolYear, assessmentId, measureIds, userInfo, out studentAssessmentId);
            else
            {
                var cachedModel = allStudents.FirstOrDefault(x => x.ID == student.ID);
                if (cachedModel != null)
                {
                    lock (cachedModel)
                    {
                        return InitMeasures(student, w, schoolYear, assessmentId, measureIds, userInfo, out studentAssessmentId);
                    }
                }
                else
                {
                    return InitMeasures(student, w, schoolYear, assessmentId, measureIds, userInfo, out studentAssessmentId);
                }
            }
        }

        /// <summary>
        /// 初始化并锁定Assessment。
        /// 会初始化一个学生ID 为0的学生数据（取消该功能，需要实时读取ADE里面的数据）
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="wave">The wave.</param>
        /// <param name="userInfo">The user information.</param>
        /// <returns></returns>
        public OperationResult InitializeStudentAssessmentDate(int assessmentId, Wave wave, UserBaseEntity userInfo)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            //锁定assessment
            if (result.ResultType == OperationResultType.Success)
                AdeBusiness.LockAssessment(assessmentId);
            return result;
        }


        private class CompareStudentMeasureModel : IEqualityComparer<StudentMeasureRecordModel>
        {

            public bool Equals(StudentMeasureRecordModel x, StudentMeasureRecordModel y)
            {
                return x.MeasureId == y.MeasureId;
            }

            public int GetHashCode(StudentMeasureRecordModel obj)
            {
                return obj.MeasureId;
            }
        }

        private void UpdateCache(int saId, IEnumerable<StudentMeasureEntity> measures)
        {
            UpdateCache(saId, measures.ToArray());
        }

        private void UpdateCache(int saId, params StudentMeasureEntity[] measures)
        {
            if (saId < 1 || measures == null)
                return;
            measures = measures.Where(x => x != null).ToArray();
            if (!measures.Any())
                return;
            var stuAss = _cpallsContract.GetStudentAssessment(saId);
            if (stuAss == null)
                return;
            List<int> schoolIds = SchoolBusiness.GetSchoolsForStudent(stuAss.StudentId);
            List<BenchmarkEntity> benchmarks = _adeBusiness.GetBenchmarks(stuAss.AssessmentId);
            foreach (var schoolId in schoolIds)
            {
                var key = string.Format("Assessment_{0}_Wave_{1}_School_{2}_Students", stuAss.AssessmentId, stuAss.Wave, schoolId);
                var allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
                if (allStudents == null)
                    continue;
                var studentModel = allStudents.Find(x => x.ID == stuAss.StudentId);
                if (studentModel == null)
                    continue;
                if (studentModel.MeasureList == null)
                    studentModel.MeasureList = new List<StudentMeasureRecordModel>();

                var measureTree = new Dictionary<int, IEnumerable<int>>();
                if (measures.Length > 1)
                    measureTree = AdeBusiness.GetMeasureTree(stuAss.AssessmentId);
                else
                    measureTree = AdeBusiness.GetMeasureTree(measures.Select(e => e.MeasureId).ToList());
                var updateMeasureids = measures.ToList().Select(m => m.MeasureId).ToList();
                // 如果更新的Measure 是子级，那么同时更新父级
                measureTree.ForEach(m =>
                {
                    if (m.Value.Intersect(updateMeasureids).Any())
                    {
                        updateMeasureids.Add(m.Key);
                    }
                });
                // 增加新的记录到缓存，更新已存在记录
                var measureModels =
                    _cpallsContract.Measures.Where(
                        x => x.SAId == saId && updateMeasureids.Contains(x.MeasureId))
                        .Select(SelectorSMEntityToSMModel)
                        .ToList();
                measureModels.ForEach(measure =>
                {
                    measure.Age = studentModel.Age;
                });
                measureModels.ForEach(measure =>
                {
                    if (measure.BenchmarkId > 0)
                    {
                        measure.BenchmarkColor = _adeBusiness.GetBenchmark(measure.BenchmarkId) == null
                            ? ""
                            : _adeBusiness.GetBenchmark(measure.BenchmarkId).Color;
                    }
                });
                lock (CacheHelper.Synchronize)
                {
                    if (studentModel.StudentAssessmentId == 0)
                        studentModel.StudentAssessmentId = measureModels.First().StudentAssessmentId;

                    var insertList =
                        measureModels.Where(x => studentModel.MeasureList.Select(m => m.MeasureId).Contains(x.MeasureId) == false)
                            .ToList();

                    var updateList = measureModels.Where(x => insertList.Select(m => m.MeasureId).Contains(x.MeasureId) == false)
                            .ToList();
                    updateList.ForEach(measure =>
                    {
                        if (measure.BenchmarkId > 0)
                        {
                            measure.BenchmarkColor = benchmarks.FirstOrDefault(e => measure.BenchmarkId == e.ID) == null
                                ? ""
                                : benchmarks.FirstOrDefault(e => measure.BenchmarkId == e.ID).Color;
                        }
                    });
                    if (insertList.Any())
                        studentModel.MeasureList.AddRange(insertList);
                    if (updateList.Any())
                        updateList.ForEach(updatedMeasure =>
                        {
                            var originMeasure = studentModel.MeasureList.Find(x => x.MeasureId == updatedMeasure.MeasureId);
                            if (originMeasure != null)
                            {
                                originMeasure.StudentAssessmentId = updatedMeasure.StudentAssessmentId;
                                originMeasure.Status = updatedMeasure.Status;
                                originMeasure.Goal = updatedMeasure.Goal;
                                originMeasure.Benchmark = updatedMeasure.Benchmark;
                                originMeasure.StudentAssessmentId = updatedMeasure.StudentAssessmentId;
                                originMeasure.PercentileRank = updatedMeasure.PercentileRank;
                                originMeasure.BenchmarkId = updatedMeasure.BenchmarkId;
                                originMeasure.LowerScore = updatedMeasure.LowerScore;
                                originMeasure.HigherScore = updatedMeasure.HigherScore;
                                originMeasure.BenchmarkColor = updatedMeasure.BenchmarkColor;
                            }
                        });
                    measureTree.ForEach(m =>
                    {
                        var parentMeasure = studentModel.MeasureList.Find(x => x.MeasureId == m.Key);
                        if (parentMeasure != null)
                        {
                            parentMeasure.IsTotal = true;
                            var scoredSubMeas =
                                studentModel.MeasureList.Where(x => m.Value.Contains(x.MeasureId) && x.TotalScored && x.Status == CpallsStatus.Finished).ToList();
                            parentMeasure.Goal = scoredSubMeas.Any() ? scoredSubMeas.Sum(x => x.GoalForTotal) : (decimal?)null;
                            decimal parentMeasureGoal = 0M;
                            if (parentMeasure.Goal != null)
                                parentMeasureGoal = parentMeasure.Goal.Value;
                            //aaabbbccc: David, 请修正,并检查(decimal)parentMeasure.Goal
                            var parentMeasureEntity =
                                _cpallsContract.Measures.FirstOrDefault(
                                    e => e.MeasureId == parentMeasure.MeasureId && e.SAId == saId);
                            parentMeasure.PercentileRank = _adeBusiness.PercentileRankLookup(parentMeasure.MeasureId,
                                studentModel.BirthDate, parentMeasureGoal, parentMeasureEntity.UpdatedOn);

                        }
                    });
                }
            };
        }

        public static void UpdateMeasureCache(Sunnet.Cli.Business.Ade.Models.MeasureModel measure)
        {
            var maxSchoolId = new SchoolBusiness().GetMaxSchoolId();
            var waves = Wave.BOY.ToList();
            for (int schoolId = 1; schoolId <= maxSchoolId; schoolId++)
            {
                int id = schoolId;
                waves.ForEach(wave =>
                {
                    var key = string.Format("Assessment_{0}_Wave_{1}_School_{2}_Students", measure.AssessmentId, wave,
                        id);
                    var studentModels = CacheHelper.Get<List<CpallsStudentModel>>(key);
                    if (studentModels != null)
                    {
                        studentModels.ForEach(studentModel =>
                        {
                            if (studentModel.MeasureList != null)
                            {
                                var cachedMeasure = studentModel.MeasureList.Find(x => x.MeasureId == measure.ID);
                                if (cachedMeasure != null)
                                {
                                    cachedMeasure.TotalScored = measure.TotalScored;
                                    cachedMeasure.LightColor = measure.LightColor;
                                    cachedMeasure.GroupByParentMeasure = measure.GroupByParentMeasure;
                                }
                            }
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 可删除 ?  
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="studentId"></param>
        public static void UpdateMeasureCache(int schoolId, int studentId)
        {
            var maxAssessmentId = new SchoolBusiness().GetMaxSchoolId();
            var waves = Wave.BOY.ToList();
            for (int assessmentId = 1; schoolId <= maxAssessmentId; schoolId++)
            {
                int id = schoolId;
                waves.ForEach(wave =>
                {
                    var key = string.Format("Assessment_{0}_Wave_{1}_School_{2}_Students", assessmentId, wave,
                        schoolId);
                    var studentModels = CacheHelper.Get<List<CpallsStudentModel>>(key);
                    if (studentModels != null)
                    {

                        var student = studentModels.Find(x => x.ID == studentId);
                        if (student != null)
                        {
                            lock (CacheHelper.Synchronize)
                            {
                                studentModels.Remove(student);
                            }
                        }
                    }
                });
            }
        }


        #region Hide/Display Measures

        public OperationResult SaveUserShownMeasures(UserShownMeasuresEntity entity)
        {
            if (entity.ID <= 0)
            {
                return _cpallsContract.InsertUserShownMeasure(entity);
            }
            else
            {
                return _cpallsContract.UpdateUserShownMeasure(entity);
            }
        }

        public UserShownMeasuresEntity GetUserShownMeasure(int assessmentId, int userId, Wave wave, int year)
        {
            return _cpallsContract.UserShownMeasures.FirstOrDefault(
                  o => o.AssessmentId == assessmentId && o.UserId == userId && o.Wave == wave && o.Year == year);
        }
        public OperationResult DeleteUserShownMeasures(int id)
        {
            return _cpallsContract.DeleteUserShownMeasure(id);
        }

        #endregion

        public List<int> GetExistAssessmentStuIds(int assessmentId, List<int> stuIdList, List<int> measureIdList)
        {
            List<int> existAssessmentStuIds = _cpallsContract.Assessments
                .Where(x => x.AssessmentId == assessmentId
                            && stuIdList.Contains(x.StudentId)
                            && x.Wave != Wave.MOY
                            && x.Measures.Any(m => measureIdList.Contains(m.MeasureId) && m.Goal > -1))
                .Select(x => x.StudentId).Distinct().ToList();
            return existAssessmentStuIds;
        }

        public List<MeasureEntity> GetMeasureByAessIdStuIds(int assessmentId, List<int> stuIds)
        {
            List<MeasureEntity> measures = _cpallsContract.Measures
                 .Where(x => x.Assessment.AssessmentId == assessmentId && stuIds.Contains(x.Assessment.StudentId))
                 .Select(x => x.Measure).ToList();
            return measures;
        }

        public List<StudentMeasureEntity> GetStuMeasureByAessIdStuIds(int assessmentId, List<int> stuIds, List<int> measureIdList)
        {
            List<StudentMeasureEntity> stuMeasures = _cpallsContract.Measures
                 .Where(x => x.Assessment.AssessmentId == assessmentId
                     && stuIds.Contains(x.Assessment.StudentId)
                     && measureIdList.Contains(x.MeasureId))
                 .ToList();
            return stuMeasures;
        }

        public List<StudentItemEntity> GetStuItemsBySMIds(List<int> SMIds, List<int> measureIdList)
        {
            List<StudentItemEntity> stuItems = _cpallsContract.Items
                .Where(x => SMIds.Contains(x.SMId) && x.Status == CpallsStatus.Finished && measureIdList.Contains(x.Measure.MeasureId)).ToList();
            return stuItems;
        }
    }
}
