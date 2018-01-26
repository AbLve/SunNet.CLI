using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/19 19:59:01
 * Description:		Please input class summary
 * Version History:	Created,2014/11/19 19:59:01
 * 
 * 
 **************************************************************************/
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Cli.Core.Practices;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Log;
using Extensions = LinqKit.Extensions;
using Sunnet.Cli.Business.Practices.Models;

namespace Sunnet.Cli.Business.Practices
{
    public class PracticeOfflineBusiness
    {
        #region Private Fields
        private readonly IPracticeContract _practiceContract;
        private readonly ISunnetLog _logger;
        #endregion

        /// <summary>
        /// 可能为空，请使用StudentBusiness私有属性
        /// </summary>
        private StudentBusiness _studentBusiness;
        private StudentBusiness StudentBusiness
        {
            get { return _studentBusiness ?? (_studentBusiness = new StudentBusiness()); }
            set { _studentBusiness = value; }
        }


        /// <summary>
        /// 可能为空，请使用AdeBusiness私有属性
        /// </summary>
        private AdeBusiness _adeBusiness;

        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public PracticeOfflineBusiness(PracticeUnitOfWorkContext unit = null)
        {
            _practiceContract = DomainFacade.CreatePracticeService(unit);


            _logger = ObjectFactory.GetInstance<ISunnetLog>();

            if (unit != null)
                AdeBusiness = new AdeBusiness();
        }
        public IEnumerable<StudentAssessmentModel> GetStudentsAssessments(UserBaseEntity userInfo, int assessmentId, Wave wave, int year)
        {
            AssessmentEntity assessment = AdeBusiness.GetAssessment(assessmentId);
            var schoolYear = year.ToSchoolYearString();
            if (assessment == null)
            {
                throw new ArgumentNullException("assessmentId");
            }
            int total;
            List<DemoStudentEntity> students = _practiceContract.Students.Where(x =>
                      x.Status == EntityStatus.Active
                    && (x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual || (byte)x.AssessmentLanguage == (byte)assessment.Language)).ToList();
            var studentIds = students.Select(s => s.ID).ToList();
            var w = ((int)wave).ToString();
            var assessments =
                _practiceContract.Assessments.Where(
                    stuAss => stuAss.AssessmentId == assessmentId
                        && stuAss.Wave == wave
                        && stuAss.SchoolYear == schoolYear
                        && studentIds.Contains(stuAss.StudentId)
                    ).Select(GetOfflineAssessmentSelector(w)).ToList();

            var measures =
                assessment.Measures.Where(x => x.Status == EntityStatus.Active && x.IsDeleted == false
                    && x.ApplyToWave.Contains(((int)wave).ToString()))
                    .Select(mea => new StudentMeasureModel()
                    {
                        ID = 0,
                        SAId = 0,
                        MeasureId = mea.ID,
                        Status = CpallsStatus.Initialised,
                        PauseTime = 0,
                        Benchmark = -1,
                        TotalScore = mea.TotalScore,
                        TotalScored = mea.TotalScored,
                        CreatedOn = mea.CreatedOn,
                        UpdatedOn = mea.UpdatedOn,
                        Goal = -1,
                        LightColor = mea.LightColor,
                        Wave = Wave.BOY,
                        BOYHasCutOffScores = mea.BOYHasCutOffScores,
                        MOYHasCutOffScores = mea.MOYHasCutOffScores,
                        EOYHasCutOffScores = mea.EOYHasCutOffScores,
                        Items = new List<StudentItemModel>()
                    }).ToList();

            var measureIds = measures.Select(m => m.MeasureId).Distinct().ToList();

            var cutoffScores = AdeBusiness.GetCutOffScores<MeasureEntity>(measureIds);
            var measureTree = AdeBusiness.GetMeasureTree(assessmentId);
            assessments.AddRange(students.Where(x => assessments.Count(sa => sa.StudentId == x.ID) == 0)
                .Select(stu => new StudentAssessmentModel()
                {
                    AssessmentId = assessmentId,
                    BirthDate = stu.StudentDob,
                    FirstName = stu.StudentName,
                    LastName = "",
                    SchoolYear = schoolYear,
                    StudentId = stu.ID,
                    Wave = wave
                }));
            assessments.ForEach(sa =>
            {
                var student = students.Find(x => x.ID == sa.StudentId);
                if (student != null)
                {
                    sa.FirstName = student.StudentName;
                    sa.LastName = "";
                    sa.BirthDate = student.StudentDob;
                }
                var studentMeasures = sa.Measures.ToList();
                studentMeasures.AddRange(measures.Where(x => studentMeasures.All(m => m.MeasureId != x.MeasureId)).Select(mea => new StudentMeasureModel()
                {
                    ID = 0,
                    SAId = sa.ID,
                    MeasureId = mea.MeasureId,
                    Status = mea.Status,
                    Benchmark = -1,
                    TotalScore = mea.TotalScore,
                    TotalScored = mea.TotalScored,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    Goal = -1,
                    LightColor = mea.LightColor,
                    Wave = mea.Wave,
                    BOYHasCutOffScores = mea.BOYHasCutOffScores,
                    MOYHasCutOffScores = mea.MOYHasCutOffScores,
                    EOYHasCutOffScores = mea.EOYHasCutOffScores,
                    Items = mea.Items
                }));

                studentMeasures.ToList().ForEach(sm =>
                {
                    sm.IsTotal = measureTree.Keys.Contains(sm.MeasureId);
                    if (sm.IsTotal)
                    {
                        sm.Goal = sa.Measures.Any(x => measureTree[sm.MeasureId].Contains(x.MeasureId) && x.TotalScored && x.Status == CpallsStatus.Finished) ?
                            sa.Measures.Where(x => measureTree[sm.MeasureId].Contains(x.MeasureId)
                                && x.TotalScored && x.Status == CpallsStatus.Finished).Sum(m => m.Goal)
                                : -1;
                    }
                    var score = CpallsBusiness.CalculateBenchmarkEntity(
                            cutoffScores.FindAll(x => x.HostId == sm.MeasureId && x.Wave == sa.Wave), sa.BirthDate, year.ToSchoolYearString());
                    if (score != null)
                    {
                        sm.Benchmark = sm.ID == 0 ? score.CutOffScore : sm.Benchmark;
                        sm.AgeGroup = string.Format("{0} year {1} month", score.FromYear, score.FromMonth);
                    }
                    else
                    {
                        sm.AgeGroup = "N/A";
                    }
                });

                sa.Measures = studentMeasures;
            });
            return assessments;
        }


        public IEnumerable<StudentAssessmentModel> GetStudentsAssessmentsForOffline(UserBaseEntity userInfo, int assessmentId, Wave wave, int year)
        {
            AssessmentEntity assessment = AdeBusiness.GetAssessment(assessmentId);
            var schoolYear = year.ToSchoolYearString();
            if (assessment == null)
            {
                throw new ArgumentNullException("assessmentId");
            }
            int total;
            List<PracticeStudentModel> students = _practiceContract.Students.Where(x =>
                x.Status == EntityStatus.Active
                &&
                (x.AssessmentLanguage == StudentAssessmentLanguage.Bilingual ||
                 (byte)x.AssessmentLanguage == (byte)assessment.Language)
                && x.AssessmentId == assessmentId).Select(x => new PracticeStudentModel
                {
                    ID = x.ID,
                    StudentId = x.StudentId,
                    StudentName = x.StudentName,
                    StudentAgeYear = x.StudentAgeYear,
                    StudentAgeMonth = x.StudentAgeMonth,
                    AssessmentLanguage = x.AssessmentLanguage,
                    DataFrom = x.DataFrom,
                    Remark = x.Remark,
                    AssessmentId = x.AssessmentId,
                    Status = x.Status
                }).ToList();

            var studentIds = students.Select(s => s.ID).ToList();
            var w = ((int)wave).ToString();
            var assessments =
                _practiceContract.Assessments.Where(
                    stuAss => stuAss.AssessmentId == assessmentId
                        && stuAss.Wave == wave
                        && stuAss.SchoolYear == schoolYear
                        && studentIds.Contains(stuAss.StudentId)
                        && (stuAss.CreatedBy == userInfo.ID)

                    ).Select(GetOfflineAssessmentSelector(w)).ToList();

            var oldAssessments =
           _practiceContract.Assessments.Where(
               stuAss => stuAss.AssessmentId == assessmentId
                   && stuAss.Wave == wave
                   && stuAss.SchoolYear == schoolYear
                   && studentIds.Contains(stuAss.StudentId)
                   && ( stuAss.CreatedBy == 0)

               ).Select(GetOfflineAssessmentSelector(w)).ToList(); 
            foreach (var model in oldAssessments)
            {
                var findItem =
                    assessments.FirstOrDefault(c => c.AssessmentId == model.AssessmentId && c.StudentId == model.StudentId);
                if (findItem != null)
                {
                    foreach (var oldModel in model.Measures)
                    {
                        var measure = findItem.Measures.FirstOrDefault(c => c.MeasureId == oldModel.MeasureId);
                        if (measure == null)
                        {
                            var list = findItem.Measures.ToList();
                            list.Add(oldModel);
                            findItem.Measures = list;
                        }
                    } 
                } 
            }

            var measures =
                assessment.Measures.Where(x => x.Status == EntityStatus.Active && x.IsDeleted == false
                    && x.ApplyToWave.Contains(((int)wave).ToString()))
                    .Select(mea => new StudentMeasureModel()
                    {
                        ID = 0,
                        SAId = 0,
                        MeasureId = mea.ID,
                        Status = CpallsStatus.Initialised,
                        PauseTime = 0,
                        Benchmark = -1,
                        TotalScore = mea.TotalScore,
                        TotalScored = mea.TotalScored,
                        CreatedOn = mea.CreatedOn,
                        UpdatedOn = mea.UpdatedOn,
                        Goal = -1,
                        LightColor = mea.LightColor,
                        Wave = Wave.BOY,
                        BOYHasCutOffScores = mea.BOYHasCutOffScores,
                        MOYHasCutOffScores = mea.MOYHasCutOffScores,
                        EOYHasCutOffScores = mea.EOYHasCutOffScores,
                        BenchamrkId = 0,
                        LowerScore = -1,
                        HigherScore = -1,
                        DataFrom = "",
                        Items = new List<StudentItemModel>()
                    }).ToList();

            var measureIds = measures.Select(m => m.MeasureId).Distinct().ToList();

            var cutoffScores = AdeBusiness.GetCutOffScores<MeasureEntity>(measureIds);
            var benchmarks = AdeBusiness.GetDicBenchmarks(assessmentId);
            var measureTree = AdeBusiness.GetMeasureTreeForOffline(assessmentId);
            assessments.AddRange(students.Where(x => assessments.Count(sa => sa.StudentId == x.ID) == 0)
                .Select(stu => new StudentAssessmentModel()
                {
                    AssessmentId = assessmentId,
                    BirthDate = stu.StudentDob,
                    FirstName = stu.StudentName,
                    LastName = "",
                    SchoolYear = schoolYear,
                    StudentId = stu.ID,
                    Wave = wave
                }));
            assessments.ForEach(sa =>
            {
                var student = students.Find(x => x.ID == sa.StudentId);
                if (student != null)
                {
                    sa.FirstName = student.StudentName;
                    sa.LastName = "";
                    sa.BirthDate = student.StudentDob;
                }
                var studentMeasures = sa.Measures.ToList();
                studentMeasures.AddRange(measures.Where(x => studentMeasures.All(m => m.MeasureId != x.MeasureId)).Select(mea => new StudentMeasureModel()
                {
                    ID = 0,
                    SAId = sa.ID,
                    MeasureId = mea.MeasureId,
                    Status = mea.Status,
                    Benchmark = -1,
                    TotalScore = mea.TotalScore,
                    TotalScored = mea.TotalScored,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    Goal = -1,
                    LightColor = mea.LightColor,
                    Wave = mea.Wave,
                    BOYHasCutOffScores = mea.BOYHasCutOffScores,
                    MOYHasCutOffScores = mea.MOYHasCutOffScores,
                    EOYHasCutOffScores = mea.EOYHasCutOffScores,
                    DataFrom = mea.DataFrom,
                    Items = mea.Items
                }));

                studentMeasures.ToList().ForEach(sm =>
                {
                    sm.IsTotal = measureTree.Keys.Contains(sm.MeasureId);
                    if (sm.IsTotal)
                    {
                        sm.Goal = sa.Measures.Any(x => measureTree[sm.MeasureId].Contains(x.MeasureId) && x.TotalScored && x.Status == CpallsStatus.Finished) ?
                            sa.Measures.Where(x => measureTree[sm.MeasureId].Contains(x.MeasureId)
                                && x.TotalScored && x.Status == CpallsStatus.Finished).Sum(m => m.Goal)
                                : -1;
                    }
                    if (sm.BenchamrkId > 0 && benchmarks.ContainsKey(sm.BenchamrkId))
                    {
                        sm.BenchmarkColor = benchmarks[sm.BenchamrkId].Color;
                        sm.BenchmarkText = benchmarks[sm.BenchamrkId].LabelText;
                    }
                    else
                    {
                        sm.BenchmarkColor = "";
                        sm.BenchmarkText = "";
                    }
                    var score = _adeBusiness.GetCutOffScore<MeasureEntity>(cutoffScores, sa.SchoolYear, sa.BirthDate, sm.Goal == null ? 0 : sm.Goal.Value);
                    if (score != null && sm.BenchamrkId > 0)
                    {
                        sm.AgeGroup = string.Format("{0} years {1} months to {2} years {3} months",
                     score.FromYear, score.FromMonth, score.ToYear, score.ToMonth);
                    }
                });

                sa.Measures = studentMeasures;
            });
            return assessments;
        }

        private Expression<Func<PracticeStudentAssessmentEntity, StudentAssessmentModel>> GetOfflineAssessmentSelector(string w)
        {
            return stuAss => new StudentAssessmentModel()
            {
                ID = stuAss.ID,
                StudentId = stuAss.StudentId,
                AssessmentId = stuAss.AssessmentId,
                Status = stuAss.Status,
                SchoolYear = stuAss.SchoolYear,
                Wave = stuAss.Wave,
                TotalGoal = stuAss.TotalGoal,
                CreatedBy = stuAss.CreatedBy,
                UpdatedBy = stuAss.UpdatedBy,
                CreatedOn = stuAss.CreatedOn,
                UpdatedOn = stuAss.UpdatedOn,
                Measures = stuAss.Measures.Where(x => x.Measure.ApplyToWave.Contains(w))
                    .Select(stuMea => new StudentMeasureModel()
                    {
                        ID = stuMea.ID,
                        SAId = stuMea.SAId,
                        MeasureId = stuMea.MeasureId,
                        Status = stuMea.Status,
                        PauseTime = stuMea.PauseTime,
                        Benchmark = stuMea.Benchmark,
                        TotalScore = stuMea.Measure.TotalScore,
                        TotalScored = stuMea.Measure.TotalScored,
                        CreatedOn = stuMea.CreatedOn,
                        UpdatedOn = stuMea.UpdatedOn,
                        Comment = stuMea.Comment,
                        Goal = stuMea.Goal,
                        LightColor = stuMea.Measure.LightColor,
                        Wave = stuMea.Assessment.Wave,
                        BOYHasCutOffScores = stuMea.Measure.BOYHasCutOffScores,
                        MOYHasCutOffScores = stuMea.Measure.MOYHasCutOffScores,
                        EOYHasCutOffScores = stuMea.Measure.EOYHasCutOffScores,
                        BenchamrkId = stuMea.BenchmarkId,
                        LowerScore = stuMea.LowerScore,
                        HigherScore = stuMea.HigherScore,
                        DataFrom = stuMea.DataFrom,
                        Items = stuMea.Items.Select(si => new StudentItemModel()
                        {
                            ID = si.ID,
                            SMId = si.SMId,
                            ItemId = si.ItemId,
                            Status = si.Status,
                            IsCorrect = si.IsCorrect,
                            SelectedAnswers = si.SelectedAnswers,
                            PauseTime = si.PauseTime,
                            Goal = si.Goal,
                            Score = si.Item.Score,
                            Scored = si.Item.Scored,
                            CreatedOn = si.CreatedOn,
                            UpdatedOn = si.UpdatedOn,
                            Details = si.Details,
                            Executed = si.Executed,
                            LastItemIndex = si.LastItemIndex,
                            ResultIndex = si.ResultIndex,
                            Type = si.Item.Type
                        })
                    })
            };
        }
    }
}
