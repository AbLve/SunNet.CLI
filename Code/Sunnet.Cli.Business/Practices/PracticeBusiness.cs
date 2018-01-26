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
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Core.Ade.Enums;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Log.Entities;

namespace Sunnet.Cli.Business.Practices
{
    public partial class PracticeBusiness
    {
        private readonly IPracticeContract _practiceContract;
        private readonly IAdeContract _adeContract;
        private readonly AdeBusiness _adeBusiness;
        private readonly IAdeDataContract _adeData;
        private readonly ISunnetLog _logger;
        OperationLogBusiness _logBusiness;
        public PracticeBusiness(PracticeUnitOfWorkContext unit = null)
        {
            _practiceContract = DomainFacade.CreatePracticeService(unit);
            _adeData = DomainFacade.CreateAdeDataService();
            _adeBusiness = new AdeBusiness();
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
            _logBusiness = new OperationLogBusiness();
        }
        /// <summary>
        /// 可能为空，请使用CACBusiness私有属性
        /// </summary>
        private CACBusiness _cacBusiness;
        private CACBusiness CACBusiness
        {
            get { return _cacBusiness ?? (_cacBusiness = new CACBusiness()); }
            set { _cacBusiness = value; }
        }

        public OperationResult InsertStudent(DemoStudentEntity entity)
        {
            return _practiceContract.InsertDemoStudent(entity);
        }

        public OperationResult InsertStudentAssessment(UserBaseEntity currentUsr, IList<ExcelModel> excelList,
            AssessmentEntity assessment, List<DemoStudentEntity> studentEntities, string dataFrom = "")
        {
            var res = new OperationResult(OperationResultType.Success);
            List<PracticeStudentAssessmentEntity> saList = new List<PracticeStudentAssessmentEntity>();
            foreach (var demoStudentEntity in studentEntities)
            {
                var time = CommonAgent.GetStartDateForAge(CommonAgent.SchoolYear);
                var studentDob = time.AddYears(-demoStudentEntity.StudentAgeYear).AddMonths(-demoStudentEntity.StudentAgeMonth);

                List<Wave> waveList = excelList.Select(c => c.Wave).Distinct().ToList();
                foreach (var wave in waveList)
                {
                    //Student - Assessment
                    PracticeStudentAssessmentEntity SA = new PracticeStudentAssessmentEntity();
                    SA.AssessmentId = assessment.ID;
                    SA.StudentId = demoStudentEntity.ID;
                    SA.Status = CpallsStatus.Initialised;
                    SA.SchoolYear = CommonAgent.SchoolYear;
                    SA.CreatedBy = 0;
                    SA.UpdatedBy = 0;
                    SA.CreatedOn = DateTime.Now;
                    SA.UpdatedOn = DateTime.Now;
                    SA.Wave = wave;
                    SA.Measures = new List<PracticeStudentMeasureEntity>();
                    // Student - Measure
                    #region Student Measure
                    var excelMeasureNameList = excelList.Where(c => c.Wave == wave && c.ItemValue > -1 && (c.StudentName == demoStudentEntity.StudentName && demoStudentEntity.StudentAgeYear == c.AgeYear
                          && demoStudentEntity.StudentAgeMonth == c.AgeMonth)).Select(c => c.MeaureName).Distinct().ToList();
                    var dbMeasureIds = assessment.Measures.Where(c => excelMeasureNameList.Contains(c.Label)).Select(c => c.ID).ToList();
                    List<CutOffScoreEntity> cutOffScoreEntities = _adeBusiness.GetCutOffScores<MeasureEntity>(dbMeasureIds);
                    foreach (var measureName in excelMeasureNameList)
                    {
                        var dbMeasure = assessment.Measures.FirstOrDefault(c => c.Label == measureName && c.AssessmentId == assessment.ID);
                        if (dbMeasure != null)
                        {

                            var excelStudentItemList =
                             excelList.Where(
                                 c => c.StudentName == demoStudentEntity.StudentName && c.MeaureName == measureName)
                                 .ToList();//列举该measure student做的所有item
                            PracticeStudentMeasureEntity SM = new PracticeStudentMeasureEntity();
                            SM.SAId = SA.ID;
                            SM.MeasureId = dbMeasure.ID;
                            SM.Status = CpallsStatus.Finished;
                            SM.PauseTime = 0;
                            SM.CreatedOn = DateTime.Now;
                            SM.UpdatedOn = DateTime.Now;
                            SM.TotalScore = excelStudentItemList.Sum(c => c.ItemValue);
                            SM.Benchmark = 0.00m;
                            SM.Items = new List<PracticeStudentItemEntity>();

                            //Student Items
                            #region Student Items
                            var excelItemList = excelList.Where(c => c.Wave == wave && c.MeaureName == measureName && (c.StudentName == demoStudentEntity.StudentName && demoStudentEntity.StudentAgeYear == c.AgeYear
                          && demoStudentEntity.StudentAgeMonth == c.AgeMonth)).ToList();
                            var totoalScores = 0.00m;
                            var hasScore = false;
                            foreach (var excelItem in excelItemList)
                            {
                                var dbItem = dbMeasure.Items.FirstOrDefault(c => c.Label == excelItem.ItemLabel && c.MeasureId == dbMeasure.ID);
                                if (dbItem != null)
                                {
                                    PracticeStudentItemEntity item = new PracticeStudentItemEntity();
                                    item.SMId = SM.ID;
                                    item.ItemId = dbItem.ID;
                                    item.Status = CpallsStatus.Finished;
                                    item.IsCorrect = false;//TODO:
                                    item.SelectedAnswers = "";
                                    item.PauseTime = 0;
                                    item.CreatedOn = DateTime.Now;
                                    item.UpdatedOn = DateTime.Now;
                                    item.Goal = excelItem.ItemValue;
                                    item.Scored = dbItem.Scored;
                                    item.Score = dbItem.Score;
                                    item.Details = "";
                                    item.Executed = true;
                                    item.LastItemIndex = 0;
                                    item.ResultIndex = 0;
                                    SM.Items.Add(item);
                                    if (item.Goal > -1)
                                    {
                                        hasScore = true;
                                        totoalScores += item.Goal.Value;
                                    }

                                }
                            }
                            #endregion

                            if (totoalScores > -1 && hasScore)
                            {
                                SM.DataFrom = dataFrom;
                                SM.Goal = totoalScores;
                                decimal benchmark = -1;
                                var currentWave = wave;
                                List<CutOffScoreEntity> cutOffScores = cutOffScoreEntities.Where(c => c.HostId == dbMeasure.ID && c.Wave == currentWave).ToList();
                                if (cutOffScores.Any())
                                    benchmark = CpallsBusiness.CalculateBenchmark(cutOffScores, demoStudentEntity.StudentAgeYear, demoStudentEntity.StudentAgeMonth, CommonAgent.SchoolYear);
                                SM.Benchmark = benchmark;
                                SM.PercentileRank = _adeBusiness.PercentileRankLookup(dbMeasure.ID, studentDob, SM.Goal, SM.UpdatedOn);
                                var cutoffScore = _adeBusiness.GetCutOffScore<MeasureEntity>(dbMeasure.ID, wave, CommonAgent.SchoolYear, studentDob, SM.Goal);
                                if (cutoffScore != null)
                                {
                                    SM.BenchmarkId = cutoffScore.BenchmarkId;
                                    SM.LowerScore = cutoffScore.LowerScore;
                                    SM.HigherScore = cutoffScore.HigherScore;
                                    SM.ShowOnGroup = cutoffScore.ShowOnGroup;
                                }
                                SA.Measures.Add(SM);

                                //创建父measure
                                if (dbMeasure.ParentId > 0)
                                {
                                    var countSubMeasures = dbMeasure.Parent.SubMeasures;
                                    var parentMeasure = new PracticeStudentMeasureEntity();
                                    parentMeasure.MeasureId = dbMeasure.ParentId;
                                    parentMeasure.SAId = SA.ID;
                                    parentMeasure.Status = CpallsStatus.Initialised;
                                    parentMeasure.PauseTime = 0;
                                    parentMeasure.CreatedOn = DateTime.Now;
                                    parentMeasure.UpdatedOn = DateTime.Now;
                                    parentMeasure.TotalScore = excelStudentItemList.Sum(c => c.ItemValue);
                                    parentMeasure.Benchmark = 0.00m;
                                    parentMeasure.PercentileRank = dbMeasure.Parent.PercentileRank ? "N/A" : "-";
                                    parentMeasure.DataFrom = dataFrom;
                                    if (SA.Measures.All(c => c.MeasureId != parentMeasure.MeasureId))
                                        SA.Measures.Add(parentMeasure);
                                    var allSubMeasure = true;
                                    foreach (var item in countSubMeasures)
                                    {
                                        if (SA.Measures.All(C => C.MeasureId != item.ID))
                                        {
                                            allSubMeasure = false;
                                            break;
                                        }
                                    }
                                    if (allSubMeasure)
                                    {
                                        SA.Measures.FirstOrDefault(c => c.MeasureId == dbMeasure.ParentId).Status = CpallsStatus.Finished;
                                    }
                                }


                            }

                        }
                    }
                    #endregion
                    if (SA.Measures.Count > 0)
                    {
                        res = _practiceContract.InsertStudentAssessment(SA);
                        if (res.ResultType == OperationResultType.Error)
                            return res;
                        _practiceContract.RecalculateParentGoal(SA.ID);
                    }
                }
            }
            return res;
        }

        public OperationResult InsertStudents(AssessmentEntity findAssessment, AssessmentEntity otherAssessment, IList<ExcelModel> excelList,
            string dataFrom, string fileName, string sourceName, out List<DemoStudentEntity> studentList, out List<DemoStudentEntity> otherStudentList)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);

            studentList = new List<DemoStudentEntity>();
            otherStudentList = new List<DemoStudentEntity>();
            foreach (var excel in excelList)
            {
                var studentAssessmentId = 0; var otherStudentAssessmentId = 0;
                if ((int)excel.AssessmentLanguage == (int)findAssessment.Language)
                    studentAssessmentId = findAssessment.ID;
                else if (otherAssessment != null && (int)excel.AssessmentLanguage == (int)otherAssessment.Language)
                    otherStudentAssessmentId = otherAssessment.ID;
                else if (excel.AssessmentLanguage == StudentAssessmentLanguage.Bilingual)
                {
                    studentAssessmentId = findAssessment.ID;
                    otherStudentAssessmentId = (otherAssessment == null ? 0 : otherAssessment.ID);
                }
                if (studentAssessmentId > 0)
                {
                    if (!studentList.Any(c => c.StudentName == excel.StudentName && c.StudentAgeYear == excel.AgeYear
                      && c.StudentAgeMonth == excel.AgeMonth && c.AssessmentId == studentAssessmentId))
                    {
                        var demoStudent = new DemoStudentEntity
                        {
                            StudentName = excel.StudentName,
                            StudentDob = DateTime.Parse("1753-01-01"),
                            StudentAgeYear = excel.AgeYear,
                            StudentAgeMonth = excel.AgeMonth,
                            AssessmentLanguage = excel.AssessmentLanguage,
                            DataFrom = dataFrom,
                            Remark = "",
                            AssessmentId = studentAssessmentId,
                            Status = EntityStatus.Active,
                            FileName = fileName,
                            Source = sourceName

                        };
                        studentList.Add(demoStudent);
                    }
                }
                if (otherStudentAssessmentId > 0)
                {
                    if (!otherStudentList.Any(c => c.StudentName == excel.StudentName && c.StudentAgeYear == excel.AgeYear
                      && c.StudentAgeMonth == excel.AgeMonth && c.AssessmentId == otherStudentAssessmentId))
                    {
                        var demoStudent = new DemoStudentEntity
                        {
                            StudentName = excel.StudentName,
                            StudentDob = DateTime.Parse("1753-01-01"),
                            StudentAgeYear = excel.AgeYear,
                            StudentAgeMonth = excel.AgeMonth,
                            AssessmentLanguage = excel.AssessmentLanguage,
                            DataFrom = dataFrom,
                            Remark = "",
                            AssessmentId = otherStudentAssessmentId,
                            Status = EntityStatus.Active,
                            FileName = fileName,
                            Source = sourceName
                        };
                        otherStudentList.Add(demoStudent);
                    }
                }
            }
            if (studentList.Count > 0 || otherStudentList.Count > 0)
            {
                res = CleanClassroom(findAssessment.ID);
            }
            if (res.ResultType == OperationResultType.Success)
            {
                res = _practiceContract.InsertDemoStudents(studentList);
                if (res.ResultType == OperationResultType.Success)
                    res = _practiceContract.InsertDemoStudents(otherStudentList);
            }

            return res;
        }

        public IList<DemoStudentEntity> GetStudents(Expression<Func<DemoStudentEntity, bool>> studentCondition, string sort, string order, int first, int count, out int total)
        {
            var query = _practiceContract.Students.AsExpandable().Where(studentCondition);
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }


        public DemoStudentEntity GetStudent(int studentId)
        {
            return _practiceContract.Students.FirstOrDefault(c => c.ID == studentId);

        }
        public DemoStudentModel GetStudentModel(int studentId)
        {
            DemoStudentModel model = new DemoStudentModel();
            var entity = _practiceContract.Students.FirstOrDefault(c => c.ID == studentId);
            if (entity == null)
                return null;
            else
            {
                var time = CommonAgent.GetStartDateForAge(CommonAgent.SchoolYear);
                model.ID = entity.ID;
                model.StudentName = entity.StudentName;
                model.StudentDob = time.AddYears(-entity.StudentAgeYear).AddMonths(-entity.StudentAgeMonth);
                model.StudentAgeYear = entity.StudentAgeYear;
                model.StudentAgeMonth = entity.StudentAgeMonth;
                model.AssessmentLanguage = entity.AssessmentLanguage;
                model.DataFrom = entity.DataFrom;
                model.Remark = entity.Remark;
            }
            return model;
        }

        public List<int> GetStudentIdByAssessmentId(int assessmentId)
        {
            return _practiceContract.Students.Where(c => c.AssessmentId == assessmentId).Select(e => e.ID).ToList();
        }

        public List<int> GetStudentIdByCondition(Expression<Func<DemoStudentEntity, bool>> studentCondition)
        {
            var studentIds = _practiceContract.Students.AsExpandable().Where(studentCondition).Select(e => e.ID).ToList();
            return studentIds;
        }

        public List<int> GetStudentIdByAssessmentId(int assessmentId, StudentAssessmentLanguage language, int fromMonths, int toMonths)
        {

            var query = _practiceContract.Students.Where(c =>
                c.AssessmentId == assessmentId
                && c.Status == EntityStatus.Active
                && (c.StudentAgeYear * 12 + c.StudentAgeMonth) >= fromMonths
                && (c.StudentAgeYear * 12 + c.StudentAgeMonth) <= toMonths);

            if (language == StudentAssessmentLanguage.English)
                query = query.Where(c => c.AssessmentLanguage == StudentAssessmentLanguage.English || c.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);
            else if (language == StudentAssessmentLanguage.Spanish)
                query = query.Where(c => c.AssessmentLanguage == StudentAssessmentLanguage.Spanish || c.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);
            return query.Select(e => e.ID).ToList();
        }


        public IList<DemoStudentModel> GetStudentList(UserBaseEntity userInfo, int assessmentId, Expression<Func<DemoStudentEntity, bool>> studentCondition, Wave wave, int year, string sort, string order, int first, int count, out int total, int sortMeasureId = 0, bool isDisplayRanks = false)
        {
            total = 0;
            if (sortMeasureId != 0)
            {
                return GetSortStudentList(userInfo, assessmentId, wave, year, studentCondition, sort, order, first, count, out total, sortMeasureId, isDisplayRanks);
            }
            IEnumerable<int> studentIds;
            studentIds = GetStudents(studentCondition, sort, order, first, count, out total).Select(c => c.ID).ToList();

            var allStudents = GetStudentList(assessmentId, wave, studentIds, sort, order, userInfo.ID).ToList();
            var list = allStudents.FindAll(x => studentIds.Contains(x.ID)).OrderBy(sort, order).ToList();
            return list;
        }

        #region Assessment Models
        public ExecCpallsAssessmentModel GetAssessment(int execAssessmentId,
          List<int> measureIds, UserBaseEntity loginUser)
        {

            var assessment = _practiceContract.Assessments.Where(x => x.ID == execAssessmentId).Select(SelectorAssEntityToModel).FirstOrDefault();
            if (assessment == null)
                throw new Exception("CPALLS+ data error: Assessment is null.");
            var measureTree = _adeBusiness.GetMeasureTree(assessment.AssessmentId);
            var parentIds = measureIds.Where(measureTree.ContainsKey).ToList();
            if (parentIds.Any())
            {
                measureIds = measureIds.Except(parentIds).ToList();
                parentIds.ForEach(parentId => measureIds.AddRange(measureTree[parentId]));
            }
            var student = GetStudentModel(assessment.StudentId);
            if ((student == null || student.ID == 0) && assessment.StudentId == 0)
            {
                student = new DemoStudentModel();

            }
            if ((student == null || student.ID == 0) && assessment.StudentId > 0)
                throw new Exception("CPALLS+ data error: Student is null.");

            assessment.KeepSelectdMeasureIds = measureIds;
            assessment.Student = new ExecCpallsStudentModel()
            {
                ID = assessment.StudentId,
                Name = student.StudentName,
                Birthday = student.StudentDob
            };

            List<ExecCpallsMeasureModel> measures = GetAssessmentFromCache(assessment.AssessmentId);

            //TxkeaReceptive类型并且Image Sequence为Random时，Answer类型为Selectable时需要重新排列（只需变更Image delay 和 audio delay）
            //在线每次做题时都要随机变化，所以不能从缓存中读取，以免缓存后读出的数据都相同
            var allMeaIds = measures.Select(x => x.MeasureId).ToList();
            var txkeaItems = _adeBusiness.GetTxkeaReceptiveItemsForPlayMeasure(allMeaIds);
            //var baseItems = _adeBusiness.GetItemModels(
            //                    i => allMeaIds.Contains(i.MeasureId) && i.IsDeleted == false
            //                        && i.Status == EntityStatus.Active && i.Type == ItemType.TxkeaReceptive).ToList();

            Random random = new Random();
            foreach (ExecCpallsMeasureModel measure in measures)
            {
                foreach (ExecCpallsItemModel item in measure.Items)
                {
                    if (item.Type == ItemType.TxkeaReceptive)
                    {
                        //ItemModel baseItem = baseItems.Find(r => r.ID == item.ItemId);
                        //if (baseItem != null && (TxkeaReceptiveItemModel)baseItem != null
                        //    && ((TxkeaReceptiveItemModel)baseItem).ImageSequence == OrderType.Random)
                        var dbItem = txkeaItems.FirstOrDefault(c => c.ID == item.ItemId);
                        if (dbItem != null && dbItem.ImageSequence == OrderType.Random)
                        {
                            if (item.Answers != null && item.Answers.Count > 0
                                && item.Answers.Where(r => r.ImageType == ImageType.Selectable).Count() > 1)
                            {
                                List<AnswerEntity> RandomAnswers = new List<AnswerEntity>();//重新排列后的answer集合
                                List<AnswerEntity> OrderedAnswers = item.Answers.Select(
                                    r => new AnswerEntity() { PictureTime = r.PictureTime, AudioTime = r.AudioTime }).ToList();
                                ArrayList choosedRandomIndex = new ArrayList();
                                //NonSelectable时，不进行随机排序，且要保留原位置
                                List<int> NonSelectableIndex = item.Answers.Select((r, i) => new { r, i }).
                                    Where(r => r.r.ImageType == ImageType.NonSelectable).Select(r => r.i).ToList();
                                choosedRandomIndex.AddRange(NonSelectableIndex);
                                List<int> listRandom = new List<int>();
                                for (int i = 0; i < item.Answers.Count; i++)
                                {
                                    listRandom.Add(i);
                                }
                                for (int i = 0; i < item.Answers.Count; i++)
                                {
                                    AnswerEntity TargetAnswer = item.Answers[i];
                                    //只有Selectable的才随机显示
                                    if (TargetAnswer.ImageType == ImageType.NonSelectable)
                                    {
                                        RandomAnswers.Add(TargetAnswer);
                                    }
                                    else
                                    {
                                        int randomIndex = random.Next(0, listRandom.Count);
                                        int randomIndexValue = listRandom[randomIndex];
                                        while (choosedRandomIndex.IndexOf(randomIndexValue) >= 0)
                                        {
                                            randomIndex = random.Next(0, listRandom.Count);
                                            randomIndexValue = listRandom[randomIndex];
                                        }
                                        choosedRandomIndex.Add(randomIndexValue);
                                        listRandom.Remove(randomIndexValue);
                                        item.Answers[randomIndexValue].PictureTime = OrderedAnswers[i].PictureTime;
                                        item.Answers[randomIndexValue].AudioTime = OrderedAnswers[i].AudioTime;
                                        RandomAnswers.Add(item.Answers[randomIndexValue]);
                                    }
                                }
                                item.Answers = RandomAnswers;
                            }
                        }
                    }
                }
            }

            List<ExecCpallsMeasureModel> selectedMeasures = measures.Where(x => measureIds.Contains(x.MeasureId)).ToList();
            var selectedMeaIds = selectedMeasures.Select(x => x.MeasureId).ToList();
            List<MeasureEntity> findMeasures = _adeBusiness.GetMeasureEntitiesByIds(selectedMeaIds);
            var cutoffScores = _adeBusiness.GetCutOffScores<MeasureEntity>(selectedMeaIds);
            List<PracticeStudentMeasureEntity> cpallsMeasures =
               _practiceContract.Measures.Where(
                   sm => sm.SAId == execAssessmentId && selectedMeaIds.Contains(sm.MeasureId)).ToList();
            List<int> selectedSmIds = cpallsMeasures.Select(x => x.ID).ToList();
            List<PracticeStudentItemEntity> cpallsItems = new List<PracticeStudentItemEntity>();
            if (student.ID > 0)
                cpallsItems = _practiceContract.Items.Where(y => selectedSmIds.Contains(y.SMId)).ToList();
            var cpallsItemsIds = cpallsItems.Select(x => x.ItemId);
            var studentMeasures = new List<ExecCpallsMeasureModel>();
            foreach (var mea in selectedMeasures)
            {
                var m = mea.Clone();
                var sm = cpallsMeasures.Find(x => x.MeasureId == mea.MeasureId);
                if (sm != null)
                {
                    m.ExecId = sm.ID;
                    m.PauseTime = sm.PauseTime;
                    m.Goal = sm.Goal;
                    m.Status = sm.Status;
                    m.UpdatedOn = sm.UpdatedOn;
                    m.Benchmark = sm.Benchmark;
                    m.Comment = sm.Comment;
                    m.BenchmarkId = sm.BenchmarkId;
                    m.HigherScore = sm.HigherScore;
                    m.LowerScore = sm.LowerScore;

                    var startDate = CommonAgent.GetStartDateForAge(assessment.SchoolYear);
                    int month; int day;
                    CommonAgent.CalculatingAge(startDate.Year, student.StudentDob, out month, out day);
                    var stuAge = ((double)(month * 10 / 12) / 10);
                    var score = mea.CutOffScores.Where(c =>
                        assessment.Wave == c.Wave
                        && stuAge >= c.FromAge && stuAge < c.ToAge
                        && sm.Goal >= c.LowerScore && sm.Goal <= c.HigherScore).FirstOrDefault();
                    if (score != null
                        && score.BenchmarkId == sm.BenchmarkId
                        && score.LowerScore == sm.LowerScore && score.HigherScore == sm.HigherScore)
                    {
                        m.AgeGroup = string.Format("{0} years {1} months to {2} years {3} months",
                     score.FromYear, score.FromMonth, score.ToYear, score.ToMonth);
                        m.BenchmarkText = score.BenchmarkLabel;
                    }
                }
                var findMeasure = findMeasures.FirstOrDefault(c => c.ID == m.MeasureId);
                if (findMeasure != null)
                {
                    m.ShowLaunchPage = findMeasure.ShowLaunchPage;
                    m.ShowFinalizePage = findMeasure.ShowFinalizePage;
                }

                if (student.ID > 0)
                    m.Items = m.Items.Where(x => cpallsItemsIds.Contains(x.ItemId));
                else
                    m.Items = m.Items;
                int count1 = m.Items.Count();
                m.Items.ForEach(item =>
                {
                    var itemEntity = cpallsItems.FirstOrDefault(y => y.ItemId == item.ItemId);
                    if (itemEntity != null)
                    {
                        item.ExecId = itemEntity.ID;
                        item.PauseTime = itemEntity.PauseTime;
                        item.Goal = itemEntity.Goal;
                        item.IsCorrect = itemEntity.IsCorrect;
                        item.SelectedAnswers =
                            itemEntity.SelectedAnswers.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse).ToList();
                        item.Status = itemEntity.Status;
                        item.Details = itemEntity.Details;
                        item.Executed = itemEntity.Executed;
                        item.LastItemIndex = itemEntity.LastItemIndex;
                        //根据最新的设置重新排序
                        item.ResultIndex = item.ResultIndex > 0 ? item.ResultIndex : itemEntity.ResultIndex;
                    }
                });

                studentMeasures.Add(m);
            }

            studentMeasures.ForEach(mea =>
            {
                if (mea.IsParent)
                {
                    mea.Children = studentMeasures.Count(x => x.Parent.ID == mea.MeasureId);
                }
            });
            assessment.Measures = studentMeasures;
            //  _logger.Info("final: Measures:{0},Items:{1}", selectedMeasures.Count, selectedMeasures.Sum(x => x.Items.Count()));
            return assessment;
        }
        private List<ExecCpallsMeasureModel> GetAssessmentFromCache(int assessmentId, Wave wave = 0)
        {
            var key = string.Format("Exec_PracticeAssessment_{0}", assessmentId);
            var measures = CacheHelper.Get<List<ExecCpallsMeasureModel>>(key);
            if (measures == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    measures = CacheHelper.Get<List<ExecCpallsMeasureModel>>(key);
                    if (measures == null)
                    {
                        var benchmarks = _adeBusiness.GetDicBenchmarks(assessmentId);
                        measures =
                            _adeData.Measures.Where(
                                x =>
                                    x.AssessmentId == assessmentId && x.IsDeleted == false &&
                                    x.Status == EntityStatus.Active)
                                .Select(SelectorMeasureEntityToCpallsModel)
                                .OrderBy(x => x.ParentSort)
                                .ThenBy(x => x.Sort)
                                .ToList();

                        var allMeaIds = measures.Select(x => x.MeasureId).ToList();

                        //var allItems = _adeBusiness.GetItemModels(
                        //        i => allMeaIds.Contains(i.MeasureId) && i.IsDeleted == false && i.Status == EntityStatus.Active)
                        //        .Select(x => x.ExecCpallsItemModel).ToList();
                        var allItems = _adeBusiness.GetItemModels(allMeaIds).Select(x => x.ExecCpallsItemModel).ToList();

                        var itemIds = allItems.Select(i => i.ItemId).ToList();
                        var itemsLinks = _adeBusiness.GetLinks<ItemBaseEntity>(itemIds.ToArray());
                        var measuresLinks = _adeBusiness.GetLinks<MeasureEntity>(allMeaIds.ToArray());
                        List<CutOffScoreEntity> measuresCutOffScores = new List<CutOffScoreEntity>();
                        if (wave == 0)
                        {
                            measuresCutOffScores = _adeBusiness.GetCutOffScores<MeasureEntity>(allMeaIds);
                        }
                        else
                        {
                            measuresCutOffScores = _adeBusiness.GetCutOffScores<MeasureEntity>(allMeaIds, wave);
                        }
                        var measuresCutOffScoreModels = measuresCutOffScores.Select(x => new ExecCpallsCutOffScoreModel
                        {
                            ID = x.ID,
                            MeasureId = x.HostId,
                            FromYear = x.FromYear,
                            FromMonth = x.FromMonth,
                            ToYear = x.ToYear,
                            ToMonth = x.ToMonth,
                            Wave = x.Wave,
                            BenchmarkId = x.BenchmarkId,
                            LowerScore = x.LowerScore,
                            HigherScore = x.HigherScore
                        }).ToList();

                        measures.ForEach(mea =>
                        {
                            mea.UpdatedOn = DateTime.Now;
                            mea.Items = allItems.FindAll(x => x.MeasureId == mea.MeasureId);
                            //生成结果页面中显示的索引（Branching Skip时需要特殊处理）
                            int resultIndex = 0;
                            foreach (ExecCpallsItemModel item in mea.Items)
                            {
                                if (item.Type != ItemType.Direction && !item.IsPractice)  //Direction 类型和Practice Item不在结果中显示
                                {
                                    resultIndex++;
                                    item.ResultIndex = resultIndex;
                                }
                                item.Links.AddRange(
                                        itemsLinks.Where(l => l.HostId == item.ItemId).Select(l => l.Link).ToList());
                            }

                            mea.Links.AddRange(measuresLinks.FindAll(m => m.HostId == mea.MeasureId).Select(l => l.Link));
                            mea.CutOffScores = measuresCutOffScoreModels.FindAll(x => x.MeasureId == mea.MeasureId && x.BenchmarkId != 0);
                            foreach (ExecCpallsCutOffScoreModel cutOffScore in mea.CutOffScores)
                            {
                                int benchmarkId = cutOffScore.BenchmarkId;
                                if (benchmarks.ContainsKey(benchmarkId))
                                {
                                    cutOffScore.BenchmarkLabel = benchmarks[benchmarkId].LabelText;
                                    cutOffScore.BenchmarkColor = benchmarks[benchmarkId].Color;
                                    cutOffScore.BenchmarkBW = benchmarks[benchmarkId].BlackWhite;
                                }
                                else
                                {
                                    cutOffScore.BenchmarkLabel = "";
                                    cutOffScore.BenchmarkColor = "";
                                    cutOffScore.BenchmarkBW = 0;
                                }
                            }
                        });

                        CacheHelper.Add(key, measures);
                        //  _logger.Info("Cached Key:" + key);
                    }
                }
            }
            //  _logger.Info("Data from Cache:{0},Measures:{1},Items:{2}", key, measures.Count, measures.Sum(x => x.Items.Count()));
            return measures;
        }
        #endregion

        #region Student Assessment
        public OperationResult InsertAssessment(DemoStudentModel student, Wave w, string schoolYear, int assessmentId, IEnumerable<int> measureIds, UserBaseEntity userInfo, out int studentAssessmentId)
        {
            var key = string.Format("Assessment_{0}_Wave_{1}_{2}_Students", assessmentId, w, userInfo.ID);
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

        #endregion

        #region Student Measure
        public List<DemoStudentModel> GetSortStudentList(UserBaseEntity userInfo, int assessmentId, Wave wave, int year,
          Expression<Func<DemoStudentEntity, bool>> studentCondition,
          string sort, string order, int first, int count, out int total, int sortMeasureId, bool isDisplayRank = false)
        {
            IEnumerable<int> studentIds = GetStudents(studentCondition, sort, order, 0, int.MaxValue, out total).Select(c => c.ID).ToList();
            var key = string.Format("Assessment_{0}_Wave_{1}_Students", assessmentId, wave);
            var allStudents = CacheHelper.Get<IList<DemoStudentModel>>(key);
            if (allStudents == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    allStudents = CacheHelper.Get<List<DemoStudentModel>>(key);
                    if (allStudents == null)
                    {
                        allStudents = GetStudentList(assessmentId, wave, studentIds, sort, order, userInfo.ID);
                        CacheHelper.Add(key, allStudents);
                    }
                }
            }
            var list = allStudents.Where(x => studentIds.Contains(x.ID)).OrderBy(sort, order).ToList();
            if (sortMeasureId != 0)
            {
                if (isDisplayRank)
                {
                    /* aaabbbccc: David 排序问题，请处理
                    if (order.ToUpper() == "ASC")
                        list = list.OrderBy(r => r.DicMeasure == null ? 0 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : 0)
                            .ThenBy(r => r.StudentName).ToList();
                    else if (order.ToUpper() == "DESC")
                        list = list.OrderByDescending(r => r.DicMeasure == null ? 0 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : 0)
                            .ThenBy(r => r.StudentName).ToList();
                            */
                    if (order.ToUpper() == "ASC")
                        list = list.OrderBy(r => r.DicMeasure == null ? "N/A" : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : "N/A")
                            .ThenBy(r => r.StudentName).ToList();
                    else if (order.ToUpper() == "DESC")
                        list = list.OrderByDescending(r => r.DicMeasure == null ? "N/A" : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].PercentileRank : "N/A")
                            .ThenBy(r => r.StudentName).ToList();
                }
                else
                {
                    if (order.ToUpper() == "ASC")
                        list = list.OrderBy(r => r.DicMeasure == null ? -1 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].Goal : -1)
                            .ThenBy(r => r.StudentName).ToList();
                    else if (order.ToUpper() == "DESC")
                        list = list.OrderByDescending(r => r.DicMeasure == null ? -1 : r.DicMeasure.ContainsKey(sortMeasureId) ? r.DicMeasure[sortMeasureId].Goal : -1)
                            .ThenBy(r => r.StudentName).ToList();
                }
                list = list.Skip(first).Take(count).ToList();
            }
            return list;
        }
        private IList<DemoStudentModel> GetStudentList(int assessmentId, Wave wave, IEnumerable<int> studentIds, string sort, string order, int userId)
        {
            List<DemoStudentMeasureRecordModel> measureList =
                _practiceContract.Measures.AsExpandable().Where(r => r.Assessment.AssessmentId == assessmentId
                                                                   && studentIds.Contains(r.Assessment.StudentId)
                                                                   && r.Assessment.Wave == wave
                                                                   && (r.Assessment.CreatedBy == userId || r.Assessment.CreatedBy == 0)
                    )
                    .Select(SelectorSMEntityToSMModel).ToList();

            int total;
            IList<DemoStudentModel> studentList = GetStudents(r => studentIds.Contains(r.ID), sort, order, 0, int.MaxValue, out total)
                .Select(c => new DemoStudentModel()
                {
                    ID = c.ID,
                    StudentName = c.StudentName,
                    StudentAgeYear = c.StudentAgeYear,
                    StudentAgeMonth = c.StudentAgeMonth,
                    AssessmentLanguage = c.AssessmentLanguage,
                    DataFrom = c.DataFrom,
                    Remark = c.Remark,
                    AssessmentId = c.AssessmentId
                }).ToList();

            var benchmarks = _adeBusiness.GetBenchmarks(assessmentId);
            var measureTree = _adeBusiness.GetMeasureTree(assessmentId);
            foreach (DemoStudentModel model in studentList)
            {

                List<DemoStudentMeasureRecordModel> tmpMeasureList = measureList.FindAll(r => r.StudentId == model.ID).Distinct(new ComparePracticeMeasure()).ToList();
                if (tmpMeasureList.Any())
                {
                    model.StudentAssessmentId = tmpMeasureList[0].StudentAssessmentId;
                    tmpMeasureList.ForEach(measure =>
                    {
                        measure.Age = model.StudentAgeYear;
                        measure.BenchmarkColor = benchmarks.FirstOrDefault(e => e.ID == measure.BenchmarkId) == null
                            ? ""
                            : benchmarks.FirstOrDefault(e => e.ID == measure.BenchmarkId).Color;
                    });
                    model.MeasureList.AddRange(tmpMeasureList);

                    DemoStudentModel model1 = model;
                    measureTree.ForEach(m =>
                    {
                        var parentMeasure = model1.MeasureList.LastOrDefault(x => x.MeasureId == m.Key);
                        if (parentMeasure != null)
                        {

                            parentMeasure.IsTotal = true;
                        }
                    });
                }
            }
            return studentList;
        }
        public OperationResult InitMeasures(DemoStudentModel student, Wave w, string schoolYear, int assessmentId,
          IEnumerable<int> measureIds, UserBaseEntity userInfo, out int studentAssessmentId)
        {
            var result = _practiceContract.InitMeasures(userInfo.ID, assessmentId,
                schoolYear, student.ID, student.StudentDob, w, measureIds);
            studentAssessmentId = result.AppendData.CastTo<int>();
            return result;
        }

        public OperationResult CheckStudentMeasure(UserBaseEntity userInfo, int assessmentId, int studentId, string schoolYear, int year, Wave wave, List<int> measureIds)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            var studentIds = new List<int>() { studentId };
            var studentAssessment = _practiceContract.Assessments.FirstOrDefault(r => r.AssessmentId == assessmentId
                                                                   && r.StudentId == studentId
                                                                   && r.SchoolYear == schoolYear
                                                                   && r.Wave == wave
                                                                   && r.CreatedBy == userInfo.ID);
            if (studentAssessment != null)
            {
                var dbList = _practiceContract.Measures.Where(c => c.SAId == studentAssessment.ID && measureIds.Contains(c.MeasureId)).ToList();
                var key = string.Format("Assessment_{0}_Wave_{1}_{2}_Students", assessmentId, wave, userInfo.ID);
                var allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
                if (allStudents != null)
                {
                    var cacheStudent = allStudents.FirstOrDefault(c => c.ID == studentId);
                    if (cacheStudent != null)
                    {
                        var cacheMeasures = cacheStudent.MeasureList;
                        foreach (var dbMeasure in dbList)
                        {
                            var findItem = cacheMeasures.FirstOrDefault(c => c.MeasureId == dbMeasure.MeasureId);
                            if (findItem == null)
                            {
                                res.ResultType = OperationResultType.Error;
                                res.Message = "This measure was launched before. Click OK and try again."; //"Data has changed, please click again.";
                                //UpdateCache(studentAssessment.ID, dbList);
                                ///TOTO:
                                break;
                            }
                            else if (findItem.Status != dbMeasure.Status || (dbMeasure.Goal != ((findItem.Goal == -1) ? -1 : findItem.Goal) && findItem.Goal != null))
                            {
                                res.ResultType = OperationResultType.Error;
                                res.Message = "This measure was launched before. Click OK and try again.";
                                //UpdateCache(studentAssessment.ID, dbList);
                                break;
                            }
                        }

                    }
                }
            }

            return res;
        }


        public OperationResult CancelMeasure(int measureId, int execAssessmentId, UserBaseEntity user, string ip)
        {
            var result = new OperationResult(OperationResultType.Success);
            var measureEntity = _practiceContract.Measures.FirstOrDefault(x => x.SAId == execAssessmentId && x.MeasureId == measureId);
            if (measureEntity == null)
            {
                throw new Exception("CPALLS+ data error: Measure is null.");
            }
            if (measureEntity.Status == CpallsStatus.Initialised)
                return result;
            measureEntity.Status = CpallsStatus.Initialised;
            measureEntity.Goal = -1;
            measureEntity.PauseTime = 0;
            measureEntity.UpdatedOn = DateTime.Now;
            measureEntity.Comment = string.Empty;
            measureEntity.PercentileRank = _adeBusiness.GetMeasureModel(measureId).PercentileRank ? "N/A" : "-";
            measureEntity.BenchmarkId = 0;
            measureEntity.LowerScore = -1;
            measureEntity.HigherScore = -1;
            measureEntity.ShowOnGroup = false;

            var items = measureEntity.Items.ToList();
            items.ForEach(x =>
            {
                x.SelectedAnswers = "";
                x.Goal = 0;
                x.PauseTime = 0;
                x.IsCorrect = false;
                x.UpdatedOn = DateTime.Now;
                x.Details = string.Empty;
                x.Status = CpallsStatus.Initialised;
                x.LastItemIndex = 0;
                x.ResultIndex = 0;
                x.Executed = true;
            });
            result = _practiceContract.UpdateStudentItems(items);
            if (result.ResultType != OperationResultType.Success)
                return result;
            result = _practiceContract.UpdateStudentMeasure(measureEntity);

            _practiceContract.RecalculateParentGoal(execAssessmentId, measureEntity.Measure.ParentId);
            if (result.ResultType == OperationResultType.Success)
            {
                UpdateCache(execAssessmentId, new List<PracticeStudentMeasureEntity>() { measureEntity });
                _logBusiness.InsertLog(OperationEnum.Delete, "CIRCLE Invalidate History",
                    string.Format("Operator:{0} {1},StudentAssessmentId:{2},MeasureId:{3}", user.FirstName, user.LastName, execAssessmentId, measureId)
                    , ip, user);
            }
            return result;
        }
        public OperationResult RefreshClassroom(int assessmentId, Wave wave, int userId)
        {
            var result = new OperationResult(OperationResultType.Success);
            result = _practiceContract.RefreshClassroom(assessmentId, wave, userId);
            DeleteCache(assessmentId);
            return result;
        }
        public OperationResult CleanClassroom(int assessmentId)
        {
            var result = new OperationResult(OperationResultType.Success);
            result = _practiceContract.CleanClassroom(assessmentId);
            DeleteCache(assessmentId);
            return result;
        }
        public OperationResult PauseMeasures(int execAssessmentId, List<PracticeStudentMeasureEntity> measures,
            List<PracticeStudentItemEntity> items, DateTime studentBirthday, string schoolYear, Wave wave)
        {
            var result = new OperationResult(OperationResultType.Success);
            if (execAssessmentId < 1)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, execAssessmentId:" + execAssessmentId);
                throw new Exception("Assessment data error.");
            }
            if (items == null || items.Count < 1)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, items is null");
                throw new Exception("Assessment data: items error.");
            }
            if (measures == null || measures.Count < 1)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, measures is null");
                throw new Exception("Assessment data: measures error.");
            }
            List<int> measureIds = measures.Select(x => x.MeasureId).ToList();
            List<PracticeStudentMeasureEntity> measureEntities = new List<PracticeStudentMeasureEntity>();
            var measureEntitiesTmp =
                _practiceContract.Measures.Where(m => m.SAId == execAssessmentId
                                                               && measureIds.Contains(m.MeasureId)).ToList();
            //_cpallsContract.Measures 查询出来的数据，顺序会被打乱，if (i == measureEntities.Count - 1) 的判断就会出现错误，所以要重新安装之前的顺序排序
            foreach (var meaIndex in measureIds)
            {
                var findItem = measureEntitiesTmp.FirstOrDefault(c => c.MeasureId == meaIndex);
                if (findItem != null)
                    measureEntities.Add(findItem);
            }


            var itemIds = items.Select(i => i.ItemId).ToList();
            var itemEntities = _practiceContract.Items.Where(x =>
               x.Measure.Assessment.ID == execAssessmentId
               && itemIds.Contains(x.ItemId)).ToList();

            itemEntities.ForEach(x =>
            {
                var goalItem = items.Find(y => y.ItemId == x.ItemId);
                if (goalItem != null)
                {
                    x.Status = CpallsStatus.Finished;
                    x.IsCorrect = goalItem.IsCorrect;
                    x.Goal = goalItem.Goal;
                    x.SelectedAnswers = goalItem.SelectedAnswers;
                    x.PauseTime = goalItem.PauseTime;
                    x.Details = goalItem.Details;
                }
            });
            for (int i = 0; i < measureEntities.Count; i++)
            {
                var measure = measureEntities[i];
                var measurePause = measures.Find(m => m.MeasureId == measure.MeasureId);
                if (measurePause != null)
                {
                    if (i == measureEntities.Count - 1)
                    {
                        measure.Status = CpallsStatus.Paused;
                        measure.PauseTime = measurePause.PauseTime;
                        itemEntities.Find(x => x.ItemId == items.Last().ItemId).Status = CpallsStatus.Paused;
                        itemEntities.Find(x => x.ItemId == items.Last().ItemId).PauseTime = items.Last().PauseTime;
                    }
                    else
                    {
                        measure.Comment = measurePause.Comment;
                        measure.Status = CpallsStatus.Finished;
                        measure.PauseTime = 0;
                        measure.Goal =
                            itemEntities.Where(item => item.SMId == measure.ID && item.Scored && item.Goal != null)
                                .Select(item => item.Goal.Value)
                                .Sum();
                        if (measure.Goal == 0)
                            // override -1 score to 0,if the value is same as its old value,EF will not update the prop.
                            measure.Goal = 0.0001m;
                        measure.PercentileRank = _adeBusiness.PercentileRankLookup(measure.MeasureId, studentBirthday, measure.Goal, measure.UpdatedOn);
                        _logger.Info(@"Online.B.Pause--> CpallsBusiness.UpdateItems,MeasureId:" + measure.MeasureId + ",Wave:" + wave + ",schoolYear:" + schoolYear + ",Birthday:" + studentBirthday + ",Goal:" + measure.Goal);
                        var cutoffScore = _adeBusiness.GetCutOffScore<MeasureEntity>(measure.MeasureId, wave, schoolYear,
                            studentBirthday, measure.Goal);
                        if (cutoffScore == null)
                            _logger.Info(@"Online.Pause--> CpallsBusiness.UpdateItems,cutoffScore:NULL");
                        if (cutoffScore != null)
                        {
                            _logger.Info(@"Online.F.Pause--> CpallsBusiness.UpdateItems,cutoffScore BenchmarkId:" + cutoffScore.BenchmarkId + ",LowerScore:" + cutoffScore.LowerScore +
                                ",HigherScore:" + cutoffScore.HigherScore + ",ShowOnGroup:" + cutoffScore.ShowOnGroup);

                            measure.BenchmarkId = cutoffScore.BenchmarkId;
                            measure.LowerScore = cutoffScore.LowerScore;
                            measure.HigherScore = cutoffScore.HigherScore;
                            measure.ShowOnGroup = cutoffScore.ShowOnGroup;
                        }
                    }
                }
                else
                {
                    result.Message = "Sync failed, measure required.";
                    result.ResultType = OperationResultType.Error;
                    return result;
                }
            }
            result = _practiceContract.UpdateStudentMeasures(measureEntities);
            if (result.ResultType != OperationResultType.Success)
                return result;
            result = _practiceContract.UpdateStudentItems(itemEntities);

            int parentMeasureId = 0;
            if (measures.Count == 1)
            {
                parentMeasureId = _adeBusiness.GetParentMeasureId(measures.FirstOrDefault().MeasureId);
            }
            _practiceContract.RecalculateParentGoal(execAssessmentId, parentMeasureId);
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(execAssessmentId, measureEntities);
            return result;
        }


        public PracticeStudentMeasureModel GetStudentMeasureModel(int id)
        {
            return
                _practiceContract.Measures.Where(x => x.ID == id)
                    .Select(studentMeasureEntityToModel)
                    .FirstOrDefault();
        }

        public OperationResult UpdateItems(int execAssessmentId,
           List<PracticeStudentMeasureEntity> studentMeasures, List<PracticeStudentItemEntity> items,
           DateTime studentBirthday, string schoolYear, Wave wave)
        {
            var result = new OperationResult(OperationResultType.Success);
            if (execAssessmentId < 1)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, execAssessmentId:" +
                             execAssessmentId);
                throw new Exception("Assessment data error.");
            }
            if (items == null)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, items is null");
                throw new Exception("Assessment data: items error.");
            }
            var itemIds = items.Select(i => i.ItemId).ToList();
            var itemEntities = _practiceContract.Items.Where(x =>
                x.Measure.Assessment.ID == execAssessmentId
                && itemIds.Contains(x.ItemId)).ToList();
            itemEntities.ForEach(x =>
            {
                var goalItem = items.Find(y => y.ItemId == x.ItemId);
                if (goalItem != null)
                {
                    x.Status = CpallsStatus.Finished;
                    x.IsCorrect = goalItem.IsCorrect;
                    x.Goal = goalItem.Goal;
                    x.PauseTime = goalItem.PauseTime;
                    x.SelectedAnswers = goalItem.SelectedAnswers;
                    x.Details = goalItem.Details;
                    x.Executed = goalItem.Executed;
                    x.LastItemIndex = goalItem.LastItemIndex;
                    x.ResultIndex = goalItem.ResultIndex;
                    x.UpdatedOn = DateTime.Now;
                }
            });
            result = _practiceContract.UpdateStudentItems(itemEntities);

            if (result.ResultType != OperationResultType.Success)
                return result;

            var measures = itemEntities.Select(x => x.Measure).Distinct().ToList();
            measures.ForEach(m =>
            {
                var stuMea = studentMeasures.Find(x => x.MeasureId == m.MeasureId);
                if (stuMea != null)
                    m.Comment = stuMea.Comment;
                m.Status = CpallsStatus.Finished;
                m.Goal = itemEntities.Where(i => i.SMId == m.ID && i.Scored && i.Goal != null).Select(i => i.Goal.Value).Sum();
                if (m.Goal == 0)
                    // override -1 score to 0,if the value is same as its old value,EF will not update the prop.
                    m.Goal = 0.0001m;
                m.TotalScore = stuMea.TotalScore;
                m.UpdatedOn = DateTime.Now;
                m.PercentileRank = _adeBusiness.PercentileRankLookup(m.MeasureId, studentBirthday, m.Goal, m.UpdatedOn);
                var cutoffScore = _adeBusiness.GetCutOffScore<MeasureEntity>(m.MeasureId, wave, schoolYear,
                    studentBirthday, m.Goal);
                if (cutoffScore != null)
                {
                    m.BenchmarkId = cutoffScore.BenchmarkId;
                    m.LowerScore = cutoffScore.LowerScore;
                    m.HigherScore = cutoffScore.HigherScore;
                    m.ShowOnGroup = cutoffScore.ShowOnGroup;
                }
            });
            result = _practiceContract.UpdateStudentMeasures(measures);

            int parentMeasureId = 0;
            if (measures.Count == 1)
            {
                parentMeasureId = _adeBusiness.GetParentMeasureId(studentMeasures.FirstOrDefault().MeasureId);
            }
            _practiceContract.RecalculateParentGoal(execAssessmentId, parentMeasureId);
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(execAssessmentId, measures);
            return result;
        }

        Expression<Func<PracticeStudentMeasureEntity, PracticeStudentMeasureModel>> studentMeasureEntityToModel = x => new PracticeStudentMeasureModel()
        {
            ID = x.ID,
            SAId = x.SAId,
            MeasureId = x.MeasureId,
            Status = x.Status,
            PauseTime = x.PauseTime,
            Benchmark = x.Benchmark,
            TotalScore = x.TotalScore,
            TotalScored = x.TotalScored,
            Goal = x.Goal,
            Comment = x.Comment,
            StudentId = x.Assessment.StudentId,
            LightColor = x.Measure.LightColor,
            Wave = x.Assessment.Wave,
            BOYHasCutOffScores = x.Measure.BOYHasCutOffScores,
            MOYHasCutOffScores = x.Measure.MOYHasCutOffScores,
            EOYHasCutOffScores = x.Measure.EOYHasCutOffScores
        };

        public OperationResult LockMeasure(DemoStudentModel student, int measureId, int assessmentId, int studentAssessmentId,
            Wave wave, int year, UserBaseEntity UserInfo, CpallsStatus status)
        {
            var result = new OperationResult(OperationResultType.Error);
            int stuAssId = 0;
            EnsureCpallsMeasure(studentAssessmentId, assessmentId, wave, year, student, measureId, UserInfo, out stuAssId);
            //if (stuAssId < 1)
            //    _logger.Info("stuassID:" + stuAssId);
            PracticeStudentMeasureEntity entity = _practiceContract.Measures.FirstOrDefault(r => r.MeasureId == measureId && r.Assessment.ID == stuAssId);
            if (entity != null)
            {
                var _m = entity.Measure;
                if (status == CpallsStatus.Initialised)
                    entity.Status = Core.Cpalls.CpallsStatus.Initialised;
                else if (status == CpallsStatus.Locked)
                    entity.Status = Core.Cpalls.CpallsStatus.Locked;
                else
                {
                    result.Message = "Illegal operation.";
                    return result;
                }
                result = _practiceContract.UpdateStudentMeasure(entity);
            }
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(stuAssId, entity);
            return result;
        }

        private static Expression<Func<PracticeStudentMeasureEntity, DemoStudentMeasureRecordModel>> SelectorSMEntityToSMModel
        {
            get
            {
                return r => new DemoStudentMeasureRecordModel
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
                    ShowOnGroup = r.ShowOnGroup,
                    DataFrom = r.DataFrom
                };
            }
        }
        #endregion

        #region Student Measure Comments
        public OperationResult UpdateMeasureComment(int id, string comment)
        {
            var measure = _practiceContract.GetStudentMeasure(id);
            //measure.UpdatedOn = DateTime.Now;
            measure.Comment = comment;

            return _practiceContract.UpdateStudentMeasure(measure);
        }
        #endregion

        #region Others
        private class ComparePracticeMeasure : IEqualityComparer<DemoStudentMeasureRecordModel>
        {
            public bool Equals(DemoStudentMeasureRecordModel x, DemoStudentMeasureRecordModel y)
            {
                return x.MeasureId == y.MeasureId
                    && x.StudentId == y.StudentId
                    && x.StudentAssessmentId == y.StudentAssessmentId;
            }

            public int GetHashCode(DemoStudentMeasureRecordModel obj)
            {
                var measure = new { obj.MeasureId, obj.StudentId, obj.StudentAssessmentId };
                return measure.GetHashCode();
            }
        }
        #endregion

        #region Play Measure
        public OperationResult PlayMeasure(DemoStudentModel student, int assessmentId, IEnumerable<int> measureIds, int studentAssessmentId, int year, Wave wave,
         UserBaseEntity UserInfo, out int newStudentAssessmentId)
        {
            var finalId = 0;
            EnsureCpallsMeasure(studentAssessmentId, assessmentId, wave, year, student, measureIds.ToList(), UserInfo, out finalId);
            newStudentAssessmentId = finalId;
            return new OperationResult(OperationResultType.Success);
        }
        private void EnsureCpallsMeasure(int stuAssId, int assessmentId, Wave wave, int year, DemoStudentModel student,
          int measureId, UserBaseEntity UserInfo, out int stuAssIdAdded)
        {
            EnsureCpallsMeasure(stuAssId, assessmentId, wave, year, student, new List<int> { measureId }, UserInfo,
                out stuAssIdAdded);
        }
        private void EnsureCpallsMeasure(int stuAssId, int assessmentId, Wave wave, int year, DemoStudentModel student,
            List<int> measureIds, UserBaseEntity UserInfo, out int stuAssIdAdded)
        {
            stuAssIdAdded = 0;
            var schoolYear = year.ToSchoolYearString();
            if (stuAssId < 1)
            {

                stuAssId = _practiceContract.Assessments.Where(r => r.AssessmentId == assessmentId
                                                                  && r.StudentId == student.ID
                                                                  && r.SchoolYear == schoolYear
                                                                  && r.Wave == wave
                                                                  && r.CreatedBy == UserInfo.ID)
                    .Select(r => r.ID).FirstOrDefault();
            }
            stuAssIdAdded = stuAssId;
            if (_practiceContract.Measures.Count(x => (x.SAId == stuAssId && measureIds.Contains(x.MeasureId))) < measureIds.Count)
            {
                InsertAssessment(student, wave, schoolYear, assessmentId, measureIds, UserInfo, out stuAssIdAdded);
                return;
            }
        }
        #endregion

        #region Offline
        public ExecCpallsAssessmentModel GetAssessment(int assessmentId, Wave wave, List<int> measureIds = null)
        {
            var assessment = _adeBusiness.GetAssessment(assessmentId);
            if (assessment == null)
                return null;
            var execAssessment = new ExecCpallsAssessmentModel()
            {
                AssessmentId = assessmentId,
                CreatedOn = assessment.UpdatedOn,
                ExecId = 0,
                Name = assessment.Name,
                OrderType = assessment.OrderType,
                Language = assessment.Language,
                Wave = wave,
            };

            List<ExecCpallsMeasureModel> measures = GetAssessmentFromCache(assessmentId, wave);
            if (measureIds != null)
            {
                execAssessment.Measures = measures.FindAll(x => measureIds.Contains(x.MeasureId)
                    && x.ApplyToWave.Contains(((int)wave).ToString()))
                    .OrderBy(x => x.ParentSort).ThenBy(x => x.Sort).ToList();
            }
            else
            {
                execAssessment.Measures = measures.FindAll(x => x.ApplyToWave.Contains(((int)wave).ToString()))
                    .OrderBy(x => x.ParentSort).ThenBy(x => x.Sort).ToList();
            }
            execAssessment.Measures.ForEach(mea =>
            {
                if (mea.IsParent)
                {
                    mea.Children = execAssessment.Measures.Count(x => x.Parent.ID == mea.MeasureId);
                    mea.TotalScore = execAssessment.Measures.Where(x => x.Parent.ID == mea.MeasureId && x.TotalScored).Sum(x => x.TotalScore);
                }
            });
            return execAssessment;
        }

        public OperationResult SyncMeasures(int execAssessmentId, List<PracticeStudentMeasureEntity> measures,
        List<PracticeStudentItemEntity> items, DateTime studentBirthday, string schoolYear, Wave wave)
        {
            var result = new OperationResult(OperationResultType.Success);
            if (execAssessmentId < 1)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, execAssessmentId:" + execAssessmentId);
                throw new Exception("Assessment data error.");
            }
            if (items == null)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, items is null");
                throw new Exception("Assessment data: items error.");
            }
            if (measures == null || measures.Count < 1)
            {
                _logger.Debug(@"Cpalls error:sunnet.cli.business\cpalls\cpallsbusiness.cs, measures is null");
                throw new Exception("Assessment data: measures error.");
            }
            List<int> measureIds = measures.Select(x => x.MeasureId).ToList();
            var measureEntities =
                _practiceContract.Measures.Where(m => m.SAId == execAssessmentId
                                                               && measureIds.Contains(m.MeasureId)).ToList();


            var itemIds = items.Select(i => i.ItemId).ToList();
            var itemEntities = _practiceContract.Items.Where(x =>
               x.Measure.Assessment.ID == execAssessmentId
               && itemIds.Contains(x.ItemId)).ToList();

            itemEntities.ForEach(itemEntity =>
            {
                var goalItem = items.Find(y => y.ItemId == itemEntity.ItemId);
                var measureModel = measures.Find(m => m.MeasureId == measureEntities.First(x => x.ID == itemEntity.SMId).MeasureId);
                if (goalItem != null && measureModel != null)
                {
                    if (measureModel.Status == CpallsStatus.Initialised)
                    {
                        itemEntity.Status = CpallsStatus.Initialised;
                        itemEntity.IsCorrect = false;
                        itemEntity.Goal = 0;
                        itemEntity.SelectedAnswers = "";
                        itemEntity.PauseTime = 0;
                        itemEntity.Details = string.Empty;
                        itemEntity.Executed = true;
                        itemEntity.LastItemIndex = 0;
                    }
                    else
                    {
                        itemEntity.Status = goalItem.PauseTime > 0 ? CpallsStatus.Finished : CpallsStatus.Initialised;
                        itemEntity.IsCorrect = goalItem.IsCorrect;
                        itemEntity.Goal = goalItem.Goal;
                        itemEntity.SelectedAnswers = goalItem.SelectedAnswers;
                        itemEntity.PauseTime = goalItem.PauseTime;
                        itemEntity.UpdatedOn = goalItem.CreatedOn;
                        itemEntity.Details = goalItem.Details;
                        itemEntity.Executed = goalItem.Executed;
                        itemEntity.LastItemIndex = goalItem.LastItemIndex;
                        itemEntity.ResultIndex = itemEntity.ResultIndex > 0 ? itemEntity.ResultIndex : goalItem.ResultIndex;


                    }
                }
                else
                {
                    itemEntity.Status = CpallsStatus.Initialised;
                    itemEntity.IsCorrect = false;
                    itemEntity.Goal = 0;
                    itemEntity.SelectedAnswers = "";
                    itemEntity.PauseTime = 0;
                    itemEntity.Executed = true;
                    itemEntity.LastItemIndex = 0;
                    itemEntity.ResultIndex = 0;
                }
            });
            for (int i = 0; i < measureEntities.Count; i++)
            {
                var measure = measureEntities[i];
                var measureModel = measures.Find(m => m.MeasureId == measure.MeasureId);
                if (measureModel == null) continue;

                measure.Comment = measureModel.Comment;
                measure.UpdatedOn = measureModel.UpdatedOn;
                if (measure.CreatedOn > measure.UpdatedOn)
                    measure.CreatedOn = measure.UpdatedOn;

                measure.Status = measureModel.Status;
                measure.PauseTime = measureModel.Status == CpallsStatus.Finished ? 0 : measureModel.PauseTime;
                if (measure.Status == CpallsStatus.Finished)
                    measure.Goal = itemEntities.Where(item => item.SMId == measure.ID && item.Scored && item.Goal != null).Select(item => item.Goal.Value).Sum();

                if (measure.Goal == 0) // override -1 score to 0,if the value is same as its old value,EF will not update the prop.
                    measure.Goal = 0.0001m;
                measure.TotalScore = measureModel.TotalScore;
                measure.PercentileRank = _adeBusiness.PercentileRankLookup(measure.MeasureId, studentBirthday,
                    measure.Goal, measure.UpdatedOn);
                var cutoffScore = _adeBusiness.GetCutOffScore<MeasureEntity>(measure.MeasureId, wave, schoolYear,
                    studentBirthday, measure.Goal);
                if (cutoffScore != null)
                {
                    measure.BenchmarkId = cutoffScore.BenchmarkId;
                    measure.LowerScore = cutoffScore.LowerScore;
                    measure.HigherScore = cutoffScore.HigherScore;
                    measure.ShowOnGroup = cutoffScore.ShowOnGroup;
                }
            }


            result = _practiceContract.UpdateStudentMeasures(measureEntities);
            if (result.ResultType != OperationResultType.Success)
                return result;
            result = _practiceContract.UpdateStudentItems(itemEntities);
            _practiceContract.RecalculateParentGoal(execAssessmentId);
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(execAssessmentId, measureEntities);
            return result;
        }
        #region 为offline 准备的实时数据
        public ExecCpallsAssessmentModel GetAssessmentForOffline(int assessmentId, Wave wave, List<int> measureIds = null)
        {
            var assessment = _adeBusiness.GetAssessment(assessmentId);
            if (assessment == null)
                return null;
            var execAssessment = new ExecCpallsAssessmentModel()
            {
                AssessmentId = assessmentId,
                CreatedOn = assessment.UpdatedOn,
                ExecId = 0,
                Name = assessment.Name,
                OrderType = assessment.OrderType,
                Language = assessment.Language,
                Wave = wave,
            };

            List<ExecCpallsMeasureModel> measures = GetAssessmentFromdb(assessmentId);
            if (measureIds != null)
            {
                execAssessment.Measures = measures.FindAll(x => measureIds.Contains(x.MeasureId)
                    && x.ApplyToWave.Contains(((int)wave).ToString()))
                    .OrderBy(x => x.ParentSort).ThenBy(x => x.Sort).ToList();
            }
            else
            {
                execAssessment.Measures = measures.FindAll(x => x.ApplyToWave.Contains(((int)wave).ToString()))
                    .OrderBy(x => x.ParentSort).ThenBy(x => x.Sort).ToList();
            }
            execAssessment.Measures.ForEach(mea =>
            {
                if (mea.IsParent)
                {
                    mea.Children = execAssessment.Measures.Count(x => x.Parent.ID == mea.MeasureId);
                    mea.TotalScore = execAssessment.Measures.Where(x => x.Parent.ID == mea.MeasureId && x.TotalScored).Sum(x => x.TotalScore);
                }
            });
            return execAssessment;
        }
        private List<ExecCpallsMeasureModel> GetAssessmentFromdb(int assessmentId)
        {
            var measures = new List<ExecCpallsMeasureModel>();
            measures =
                _adeData.Measures.Where(
                    x =>
                        x.AssessmentId == assessmentId && x.IsDeleted == false &&
                        x.Status == EntityStatus.Active)
                    .Select(SelectorMeasureEntityToCpallsModel)
                    .OrderBy(x => x.ParentSort)
                    .ThenBy(x => x.Sort)
                    .ToList();

            var allMeaIds = measures.Select(x => x.MeasureId).ToList();

            var allItems = _adeBusiness.GetItemModels(
                    i => allMeaIds.Contains(i.MeasureId) && i.IsDeleted == false && i.Status == EntityStatus.Active)
                    .Select(x => x.ExecCpallsItemModel).ToList();

            var itemIds = allItems.Select(i => i.ItemId).ToList();
            var itemsLinks = _adeBusiness.GetLinks<ItemBaseEntity>(itemIds.ToArray());
            var measuresLinks = _adeBusiness.GetLinks<MeasureEntity>(allMeaIds.ToArray());
            measures.ForEach(mea =>
            {
                mea.UpdatedOn = DateTime.Now;
                mea.Items = allItems.FindAll(x => x.MeasureId == mea.MeasureId);
                //生成结果页面中显示的索引（Branching Skip时需要特殊处理）
                int resultIndex = 0;
                foreach (ExecCpallsItemModel item in mea.Items)
                {
                    if (item.Type != ItemType.Direction && !item.IsPractice)  //Direction 类型和Practice Item不在结果中显示
                    {
                        resultIndex++;
                        item.ResultIndex = resultIndex;
                    }
                    item.Links.AddRange(
                            itemsLinks.Where(l => l.HostId == item.ItemId).Select(l => l.Link).ToList());
                }

                mea.Links.AddRange(measuresLinks.FindAll(m => m.HostId == mea.MeasureId).Select(l => l.Link));
            });

            //   _logger.Info("Data from Db: Measures:{1},Items:{2}", measures.Count, measures.Sum(x => x.Items.Count()));
            return measures;
        }
        #endregion
        #endregion

        #region 静态方法
        private static Expression<Func<PracticeStudentAssessmentEntity, ExecCpallsAssessmentModel>> SelectorAssEntityToModel
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


        private void UpdateCache(int saId, IEnumerable<PracticeStudentMeasureEntity> measures)
        {
            UpdateCache(saId, measures.ToArray());
        }

        private void DeleteCache(int assessmentId)
        {
            var key1 = string.Format("Assessment_{0}_Wave_{1}_Students", assessmentId, Wave.BOY);
            CacheHelper.Remove(key1);
            var key2 = string.Format("Assessment_{0}_Wave_{1}_Students", assessmentId, Wave.MOY);
            CacheHelper.Remove(key2);
            var key3 = string.Format("Assessment_{0}_Wave_{1}_Students", assessmentId, Wave.EOY);
            CacheHelper.Remove(key3);
        }

        private void UpdateCache(int saId, params PracticeStudentMeasureEntity[] measures)
        {
            if (saId < 1 || measures == null)
                return;
            measures = measures.Where(x => x != null).ToArray();
            if (!measures.Any())
                return;
            var stuAss = _practiceContract.GetStudentAssessment(saId);
            if (stuAss == null)
                return;
            List<BenchmarkEntity> benchmarks = _adeBusiness.GetBenchmarks(stuAss.AssessmentId);

            var key = string.Format("Assessment_{0}_Wave_{1}_Students", stuAss.AssessmentId, stuAss.Wave);
            var allStudents = CacheHelper.Get<List<DemoStudentModel>>(key);
            if (allStudents == null)
                return;
            var studentModel = allStudents.Find(x => x.ID == stuAss.StudentId);
            if (studentModel == null)
                return;
            if (studentModel.MeasureList == null)
                studentModel.MeasureList = new List<DemoStudentMeasureRecordModel>();

            var measureTree = new Dictionary<int, IEnumerable<int>>();
            if (measures.Length > 1)
                measureTree = new AdeBusiness().GetMeasureTree(stuAss.AssessmentId);
            else
                measureTree = new AdeBusiness().GetMeasureTree(measures.Select(e => e.MeasureId).ToList());
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
                _practiceContract.Measures.Where(
                    x => x.SAId == saId && updateMeasureids.Contains(x.MeasureId))
                    .Select(SelectorSMEntityToSMModel)
                    .ToList();
            measureModels.ForEach(measure =>
            {
                measure.Age = (double)studentModel.StudentAgeYear;
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
                        //aaabbbccc 重新计算获得Student的DOB,同时查询出parentMeasure的UpdatedOn
                        var time = CommonAgent.GetStartDateForAge(CommonAgent.SchoolYear);
                        DateTime studentDOB = time.AddYears(-studentModel.StudentAgeYear).AddMonths(-studentModel.StudentAgeMonth);
                        var parentMeasureEntity =
                                _practiceContract.Measures.FirstOrDefault(
                                    e => e.MeasureId == parentMeasure.MeasureId && e.SAId == saId);
                        parentMeasure.PercentileRank = _adeBusiness.PercentileRankLookup(parentMeasure.MeasureId, studentDOB,
                            parentMeasureGoal, parentMeasureEntity.UpdatedOn);
                    }
                });
            }

        }

        public static void UpdateMeasureCache(Sunnet.Cli.Business.Ade.Models.MeasureModel measure)
        {
            var waves = Wave.BOY.ToList();
            waves.ForEach(wave =>
            {
                var key = string.Format("Assessment_{0}_Wave_{1}_Students", measure.AssessmentId, wave);
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

        #endregion
    }
}
