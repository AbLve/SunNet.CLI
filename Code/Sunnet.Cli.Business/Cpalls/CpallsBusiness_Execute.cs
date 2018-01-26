using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/4 5:49:36
 * Description:		Partial 1：Student View以及执行Assessment相关的逻辑
 * Version History:	Created,2014/9/4 5:49:36
 * 
 * 
 **************************************************************************/
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Cpalls.Models;
using Sunnet.Cli.Core.Log.Entities;
using Sunnet.Cli.Core.Students.Entities;
using LinqKit;
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Ade.Enums;
using System.Collections;
using System.Threading;

namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {
        public ExecCpallsAssessmentModel GetAssessment(int assessmentId, Wave wave, List<int> measureIds = null)
        {
            var assessment = AdeBusiness.GetAssessment(assessmentId);
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

        #region 为offline 准备的实时数据
        public ExecCpallsAssessmentModel GetAssessmentForOffline(int assessmentId, Wave wave, List<int> measureIds = null)
        {
            var assessment = AdeBusiness.GetAssessment(assessmentId);
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

            var allItems = AdeBusiness.GetItemModels(
                    i => allMeaIds.Contains(i.MeasureId) && i.IsDeleted == false && i.Status == EntityStatus.Active)
                    .Select(x => x.ExecCpallsItemModel).ToList();

            var itemIds = allItems.Select(i => i.ItemId).ToList();
            var itemsLinks = AdeBusiness.GetLinks<ItemBaseEntity>(itemIds.ToArray());
            var measuresLinks = AdeBusiness.GetLinks<MeasureEntity>(allMeaIds.ToArray());
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

            //_logger.Info("Data from Db: Measures:{1},Items:{2}", measures.Count, measures.Sum(x => x.Items.Count()));
            return measures;
        }
        #endregion

        private List<ExecCpallsMeasureModel> GetAssessmentFromCache(int assessmentId, Wave wave = 0)
        {
            var key = string.Format("Exec_Assessment_{0}", assessmentId);
            var measures = CacheHelper.Get<List<ExecCpallsMeasureModel>>(key);
            if (measures == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    measures = CacheHelper.Get<List<ExecCpallsMeasureModel>>(key);
                    if (measures == null)
                    {
                        var benchmarks = AdeBusiness.GetDicBenchmarks(assessmentId);
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

                        //var allItems = AdeBusiness.GetItemModels(
                        //        i => allMeaIds.Contains(i.MeasureId) && i.IsDeleted == false && i.Status == EntityStatus.Active)
                        //        .Select(x => x.ExecCpallsItemModel).ToList();
                        var allItems = AdeBusiness.GetItemModels(allMeaIds).Select(x => x.ExecCpallsItemModel).ToList();

                        var itemIds = allItems.Select(i => i.ItemId).ToList();
                        var itemsLinks = AdeBusiness.GetLinks<ItemBaseEntity>(itemIds.ToArray());
                        var measuresLinks = AdeBusiness.GetLinks<MeasureEntity>(allMeaIds.ToArray());
                        List<CutOffScoreEntity> measuresCutOffScores = new List<CutOffScoreEntity>();
                        if (wave == 0)
                        {
                            measuresCutOffScores = AdeBusiness.GetCutOffScores<MeasureEntity>(allMeaIds);
                        }
                        else
                        {
                            measuresCutOffScores = AdeBusiness.GetCutOffScores<MeasureEntity>(allMeaIds, wave);
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
                        //_logger.Info("Cached Key:" + key);
                    }
                }
            }
            // _logger.Info("Data from Cache:{0},Measures:{1},Items:{2}", key, measures.Count, measures.Sum(x => x.Items.Count()));
            return measures;
        }

        /// <summary>
        /// 执行Assessment
        /// 缓存,永远有效,需要维护
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns></returns>
        public ExecCpallsAssessmentModel GetAssessment(int execAssessmentId,
           List<int> measureIds, int classId, UserBaseEntity loginUser)
        {

            var assessment = _cpallsContract.Assessments.Where(x => x.ID == execAssessmentId)
                .Select(SelectorAssEntityToModel).FirstOrDefault();
            if (assessment == null)
                throw new Exception("CPALLS+ data error: Assessment is null.");
            var measureTree = _adeBusiness.GetMeasureTree(assessment.AssessmentId);
            var parentIds = measureIds.Where(measureTree.ContainsKey).ToList();
            if (parentIds.Any())
            {
                measureIds = measureIds.Except(parentIds).ToList();
                parentIds.ForEach(parentId => measureIds.AddRange(measureTree[parentId]));
            }
            var student = StudentBusiness.GetStudentModel(assessment.StudentId, loginUser);
            if ((student == null || student.ID == 0) && assessment.StudentId == 0)
            {
                student = new CpallsStudentModel();
                student.Schools = new List<BasicSchoolModel>();
            }
            if ((student == null || student.ID == 0) && assessment.StudentId > 0)
                throw new Exception("CPALLS+ data error: Student is null.");
            var classEntity = classId < 1 ? new ClassEntity() : ClassBusiness.GetClass(classId);

            assessment.CommunityName = student.CommunitiesText;
            assessment.SchoolName = student.SchoolName;
            assessment.SchoolId = student.Schools.Select(x => x.ID).FirstOrDefault();
            assessment.KeepSelectdMeasureIds = measureIds;
            assessment.Class = new ExecCpallsClassModel()
            {
                ID = classId,
                Name = classEntity.Name,
                HomeroomTeacher = classEntity.LeadTeacherId > 0
                ? classEntity.LeadTeacher.UserInfo.FirstName + " " + classEntity.LeadTeacher.UserInfo.LastName
                : ""
            };
            assessment.Student = new ExecCpallsStudentModel()
            {
                ID = assessment.StudentId,
                Name = student.FirstName + " " + student.LastName,
                Birthday = student.BirthDate,
                Schools = student.Schools
            };

            List<ExecCpallsMeasureModel> measures = GetAssessmentFromCache(assessment.AssessmentId);

            //TxkeaReceptive类型并且Image Sequence为Random时，Answer类型为Selectable时需要重新排列（只需变更Image delay 和 audio delay）
            //在线每次做题时都要随机变化，所以不能从缓存中读取，以免缓存后读出的数据都相同
            var allMeaIds = measures.Select(x => x.MeasureId).ToList();
            //var baseItems = AdeBusiness.GetItemModels(
            //                   i => allMeaIds.Contains(i.MeasureId) && i.IsDeleted == false
            //                        && i.Status == EntityStatus.Active && i.Type == ItemType.TxkeaReceptive).ToList();
            var txkeaItems = AdeBusiness.GetTxkeaReceptiveItemsForPlayMeasure(allMeaIds);
            Random random = new Random();
            foreach (ExecCpallsMeasureModel measure in measures)
            {
                foreach (ExecCpallsItemModel item in measure.Items)
                {
                    if (item.Type == ItemType.TxkeaReceptive)
                    {

                        //  ItemModel baseItem = baseItems.Find(r => r.ID == item.ItemId);
                        //if (baseItem != null && (TxkeaReceptiveItemModel)baseItem != null
                        // && ((TxkeaReceptiveItemModel)baseItem).ImageSequence == OrderType.Random)
                        var dbItem = txkeaItems.FirstOrDefault(c => c.ID == item.ItemId);
                        if (dbItem != null && dbItem.ImageSequence == OrderType.Random)
                        {
                            if (item.Answers != null && item.Answers.Count > 0
                                && item.Answers.Where(r => r.ImageType == ImageType.Selectable).Count() > 1)
                            {
                                List<AnswerEntity> RandomAnswers = new List<AnswerEntity>();//重新排列后的answer集合
                                List<AnswerEntity> OrderedAnswers = item.Answers.Select(
                                    r => new AnswerEntity() { PictureTime = r.PictureTime, AudioTime = r.AudioTime }).ToList();

                                List<int> selectableIndex = item.Answers.Select((r, i) => new { r, i }).
                                    Where(r => r.r.ImageType == ImageType.Selectable).Select(r => r.i).ToList();
                                RandomTool.GetRandomNum(selectableIndex);//重新排列Selectable的Index

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
                                        if (selectableIndex.Count > 0)//保证还有得拿
                                        {
                                            int randomIndexValue = selectableIndex[0];//总拿第一个的值
                                            selectableIndex.RemoveAt(0);//去掉，下次自动拿下一个
                                            item.Answers[randomIndexValue].PictureTime = OrderedAnswers[i].PictureTime;
                                            item.Answers[randomIndexValue].AudioTime = OrderedAnswers[i].AudioTime;
                                            RandomAnswers.Add(item.Answers[randomIndexValue]);
                                        }
                                    }
                                }

                                /* David 07/20/2017
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
                                 */

                                item.Answers = RandomAnswers;
                            }
                        }
                    }
                }
            }

            List<ExecCpallsMeasureModel> selectedMeasures = measures.Where(x => measureIds.Contains(x.MeasureId)).ToList();
            var selectedMeaIds = selectedMeasures.Select(x => x.MeasureId).ToList();

            List<MeasureEntity> findMeasures = AdeBusiness.GetMeasureEntitiesByIds(selectedMeaIds);

            //    var cutoffScores = AdeBusiness.GetCutOffScores<MeasureEntity>(selectedMeaIds);

            List<StudentMeasureEntity> cpallsMeasures =
               _cpallsContract.Measures.Where(
                   sm => sm.SAId == execAssessmentId && selectedMeaIds.Contains(sm.MeasureId)).ToList();

            List<int> selectedSmIds = cpallsMeasures.Select(x => x.ID).ToList();
            List<StudentItemEntity> cpallsItems = new List<StudentItemEntity>();
            if (student.ID > 0)
                cpallsItems = _cpallsContract.Items.Where(y => selectedSmIds.Contains(y.SMId)).ToList();


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
                    CommonAgent.CalculatingAge(startDate.Year, student.BirthDate, out month, out day);
                    var stuAge = ((double)(month * 10 / 12.00) / 10.00);
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
            // _logger.Info("final: Measures:{0},Items:{1}", selectedMeasures.Count, selectedMeasures.Sum(x => x.Items.Count()));
            return assessment;
        }

        public OperationResult CancelMeasure(int measureId, int execAssessmentId, int schoolId, UserBaseEntity user, string ip)
        {
            var result = new OperationResult(OperationResultType.Success);
            var measureEntity = _cpallsContract.Measures.FirstOrDefault(x => x.SAId == execAssessmentId && x.MeasureId == measureId);
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
                x.Goal = null;
                x.PauseTime = 0;
                x.IsCorrect = false;
                x.UpdatedOn = DateTime.Now;
                x.Details = string.Empty;
                x.Status = CpallsStatus.Initialised;
                x.LastItemIndex = 0;
                x.ResultIndex = 0;
                x.Executed = true;
            });
            result = _cpallsContract.UpdateStudentItems(items);
            if (result.ResultType != OperationResultType.Success)
                return result;
            result = _cpallsContract.UpdateStudentMeasure(measureEntity);

            _cpallsContract.RecalculateParentGoal(execAssessmentId, measureEntity.Measure.ParentId);
            if (result.ResultType == OperationResultType.Success)
            {
                UpdateCache(execAssessmentId, new List<StudentMeasureEntity>() { measureEntity });
                LogBusiness.InsertLog(OperationEnum.Delete, "CIRCLE Invalidate History",
                    string.Format("Operator:{0} {1},StudentAssessmentId:{2},MeasureId:{3}", user.FirstName, user.LastName, execAssessmentId, measureId)
                    , ip, user);
            }
            return result;
        }

        public OperationResult PauseMeasures(int execAssessmentId, int schoolId, List<StudentMeasureEntity> measures,
            List<StudentItemEntity> items, DateTime studentBirthday, string schoolYear, Wave wave)
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
            List<StudentMeasureEntity> measureEntities = new List<StudentMeasureEntity>();
            var measureEntitiesTmp =
                _cpallsContract.Measures.Where(m => m.SAId == execAssessmentId
                                                               && measureIds.Contains(m.MeasureId)).ToList();
            //_cpallsContract.Measures 查询出来的数据，顺序会被打乱，if (i == measureEntities.Count - 1) 的判断就会出现错误，所以要重新安装之前的顺序排序
            foreach (var meaIndex in measureIds)
            {
                var findItem = measureEntitiesTmp.FirstOrDefault(c => c.MeasureId == meaIndex);
                if (findItem != null)
                    measureEntities.Add(findItem);
            }


            var itemIds = items.Select(i => i.ItemId).ToList();
            var itemEntities = _cpallsContract.Items.Where(x =>
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
                            itemEntities.Where(item => item.SMId == measure.ID && item.Scored && item.Goal != null)//todo:Goal
                                .Select(item => item.Goal.Value)
                                .Sum();
                        if (measure.Goal == 0)
                            // override -1 score to 0,if the value is same as its old value,EF will not update the prop.
                            measure.Goal = 0.0001m;
                        measure.PercentileRank = _adeBusiness.PercentileRankLookup(measure.MeasureId, studentBirthday, measure.Goal, measure.UpdatedOn);
                        //_logger.Info(@"Online.B.Pause--> CpallsBusiness.UpdateItems,MeasureId:" + measure.MeasureId + ",Wave:" + wave + ",schoolYear:" + schoolYear + ",Birthday:" + studentBirthday + ",Goal:" + measure.Goal);
                        var cutoffScore = _adeBusiness.GetCutOffScore<MeasureEntity>(measure.MeasureId, wave, schoolYear,
                            studentBirthday, measure.Goal);
                        //if (cutoffScore == null)
                        //    _logger.Info(@"Online.Pause--> CpallsBusiness.UpdateItems,cutoffScore:NULL");
                        if (cutoffScore != null)
                        {
                            //_logger.Info(@"Online.F.Pause--> CpallsBusiness.UpdateItems,cutoffScore BenchmarkId:" + cutoffScore.BenchmarkId + ",LowerScore:" + cutoffScore.LowerScore +
                            //    ",HigherScore:" + cutoffScore.HigherScore + ",ShowOnGroup:" + cutoffScore.ShowOnGroup);

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
            result = _cpallsContract.UpdateStudentMeasures(measureEntities);
            if (result.ResultType != OperationResultType.Success)
                return result;
            result = _cpallsContract.UpdateStudentItems(itemEntities);

            int parentMeasureId = 0;
            if (measures.Count == 1)
            {
                parentMeasureId = _adeBusiness.GetParentMeasureId(measures.FirstOrDefault().MeasureId);
            }
            _cpallsContract.RecalculateParentGoal(execAssessmentId, parentMeasureId);
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(execAssessmentId, measureEntities);
            return result;
        }

        public OperationResult UpdateItems(int execAssessmentId, int schoolId,
            List<StudentMeasureEntity> studentMeasures, List<StudentItemEntity> items,
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
            var itemEntities = _cpallsContract.Items.Where(x =>
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
            result = _cpallsContract.UpdateStudentItems(itemEntities);

            if (result.ResultType != OperationResultType.Success)
                return result;

            var measures = itemEntities.Select(x => x.Measure).Distinct().ToList();
            measures.ForEach(m =>
            {
                var stuMea = studentMeasures.Find(x => x.MeasureId == m.MeasureId);
                if (stuMea != null)
                    m.Comment = stuMea.Comment;
                m.Status = CpallsStatus.Finished;
                m.Goal = itemEntities.Where(i => i.SMId == m.ID && i.Scored && i.Goal != null).Select(i => i.Goal.Value).Sum();//todo:Goal
                if (m.Goal == 0)
                    // override -1 score to 0,if the value is same as its old value,EF will not update the prop.
                    m.Goal = 0.0001m;
                m.TotalScore = stuMea.TotalScore;
                m.UpdatedOn = DateTime.Now;
                m.PercentileRank = _adeBusiness.PercentileRankLookup(m.MeasureId, studentBirthday, m.Goal, m.UpdatedOn);
                //_logger.Info(@"Online.B--> CpallsBusiness.UpdateItems,MeasureId:" + m.MeasureId + ",Wave:" + wave + ",schoolYear:" + schoolYear + ",Birthday:" + studentBirthday + ",Goal:" + m.Goal);
                var cutoffScore = _adeBusiness.GetCutOffScore<MeasureEntity>(m.MeasureId, wave, schoolYear,
                    studentBirthday, m.Goal);
                //if (cutoffScore == null)
                //    _logger.Info(@"Online.F--> CpallsBusiness.UpdateItems,cutoffScore:NULL");
                if (cutoffScore != null)
                {
                    //_logger.Info(@"Online.F--> CpallsBusiness.UpdateItems,cutoffScore BenchmarkId:" + cutoffScore.BenchmarkId + ",LowerScore:" + cutoffScore.LowerScore +
                    //    ",HigherScore:" + cutoffScore.HigherScore + ",ShowOnGroup:" + cutoffScore.ShowOnGroup);

                    m.BenchmarkId = cutoffScore.BenchmarkId;
                    m.LowerScore = cutoffScore.LowerScore;
                    m.HigherScore = cutoffScore.HigherScore;
                    m.ShowOnGroup = cutoffScore.ShowOnGroup;
                }
            });
            result = _cpallsContract.UpdateStudentMeasures(measures);
            int parentMeasureId = 0;
            if (studentMeasures.Count == 1)
            {
                parentMeasureId = _adeBusiness.GetParentMeasureId(studentMeasures.FirstOrDefault().MeasureId);
            }
            _cpallsContract.RecalculateParentGoal(execAssessmentId, parentMeasureId);
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(execAssessmentId, measures);
            return result;
        }

        public OperationResult SyncMeasures(int execAssessmentId, int schoolId, List<StudentMeasureEntity> measures,
            List<StudentItemEntity> items, DateTime studentBirthday, string schoolYear, Wave wave)
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
                _cpallsContract.Measures.Where(m => m.SAId == execAssessmentId
                                                               && measureIds.Contains(m.MeasureId)).ToList();


            var itemIds = items.Select(i => i.ItemId).ToList();
            var itemEntities = _cpallsContract.Items.Where(x =>
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
                        itemEntity.Goal = null;
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
                    itemEntity.Goal = null;
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
                    measure.Goal = itemEntities.Where(item => item.SMId == measure.ID && item.Scored && item.Goal != null).Select(item => item.Goal.Value).Sum();//todo:Goal

                if (measure.Goal == 0) // override -1 score to 0,if the value is same as its old value,EF will not update the prop.
                    measure.Goal = 0.0001m;
                measure.TotalScore = measureModel.TotalScore;
                measure.PercentileRank = _adeBusiness.PercentileRankLookup(measure.MeasureId, studentBirthday,
                    measure.Goal, measure.UpdatedOn);
                //_logger.Info(@"B--> CpallsBusiness.SyncMeasures,MeasureId:" + measure.MeasureId + ",Wave:" + wave + ",schoolYear:" + schoolYear + ",Birthday:" + studentBirthday + ",Goal:" + measure.Goal);
                var cutoffScore = _adeBusiness.GetCutOffScore<MeasureEntity>(measure.MeasureId, wave, schoolYear,
                    studentBirthday, measure.Goal);
                //if (cutoffScore == null)
                //    _logger.Info(@"F--> CpallsBusiness.SyncMeasures,cutoffScore:NULL");
                if (cutoffScore != null)
                {
                    //_logger.Info(@"F--> CpallsBusiness.SyncMeasures,cutoffScore BenchmarkId:" + cutoffScore.BenchmarkId + ",LowerScore:" + cutoffScore.LowerScore +
                    //    ",HigherScore:" + cutoffScore.HigherScore + ",ShowOnGroup:" + cutoffScore.ShowOnGroup);
                    measure.BenchmarkId = cutoffScore.BenchmarkId;
                    measure.LowerScore = cutoffScore.LowerScore;
                    measure.HigherScore = cutoffScore.HigherScore;
                    measure.ShowOnGroup = cutoffScore.ShowOnGroup;
                }
            }


            result = _cpallsContract.UpdateStudentMeasures(measureEntities);
            if (result.ResultType != OperationResultType.Success)
                return result;
            result = _cpallsContract.UpdateStudentItems(itemEntities);
            _cpallsContract.RecalculateParentGoal(execAssessmentId);
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(execAssessmentId, measureEntities);
            return result;
        }
        public ExecCpallsAssessmentModel GetAssessmentForPreview(int itemId, int measureId = 0)
        {
            var execAssessment = new ExecCpallsAssessmentModel()
            {
                Wave = Wave.BOY,
                SchoolYear = CommonAgent.SchoolYear,
                OrderType = OrderType.Sequenced
            };
            execAssessment.Measures = new List<ExecCpallsMeasureModel>();
            var execMeasure = GetExecCpallsMeasureModel(measureId);
            //var execMeasure = new ExecCpallsMeasureModel();

            //execMeasure.ExecId = 0;
            //execMeasure.MeasureId = x.MeasureId;
            //execMeasure.OrderType = x.OrderType;
            //execMeasure.ShowType = x.ShowType;
            //execMeasure.Name = x.Name;
            //execMeasure.InnerTime = x.InnerTime;
            //execMeasure.Timeout = x.Timeout;
            //execMeasure.StartPageHtml = x.StartPageHtml;
            //execMeasure.EndPageHtml = x.EndPageHtml;
            //execMeasure.PauseTime = 0;
            //execMeasure.Status = CpallsStatus.Initialised;
            //execMeasure.Benchmark = 0;
            //execMeasure.TotalScore = x.TotalScore;
            //execMeasure.TotalScored = x.TotalScored;
            //execMeasure.ApplyToWave = x.ApplyToWave;
            //execMeasure.Goal = 0;
            //execMeasure.UpdatedOn = x.UpdatedOn;
            //execMeasure.Comment = string.Empty;
            //execMeasure.Parent =  x.Parent;
            //execMeasure.IsParent = x.IsParent;
            //execMeasure.Children = x.Children;
            //execMeasure.ParentSort = x.ParentSort;
            //execMeasure.Sort = x.Sort;
            //execMeasure.StopButton = x.StopButton;
            //execMeasure.PreviousButton = x.PreviousButton;
            //execMeasure.NextButton = x.NextButton;

            List<ItemModel> listItems = new List<ItemModel>();
            if (execMeasure.MeasureId > 0 && execMeasure.ShowType == ItemShowType.List)
            {
                // List 类型预览时显示所有的Item
                listItems.AddRange(AdeBusiness.GetItemModels(M => M.MeasureId == measureId));

            }
            else
            {
                listItems.Add(AdeBusiness.GetItemModel(itemId));
            }
            var list = listItems.Select(M => M.ExecCpallsItemModel).ToList();
            for (int index = 0; index < list.Count; index++)
            {
                var item = list[index];

                ItemModel baseItem = listItems.Find(r => r.ID == item.ItemId);
                if (baseItem.Type != ItemType.TxkeaReceptive)
                {
                    continue;
                }
                if (baseItem != null && (TxkeaReceptiveItemModel)baseItem != null
                    && ((TxkeaReceptiveItemModel)baseItem).ImageSequence == OrderType.Random)
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
                                Random random = new Random();
                                int randomIndex = random.Next(0, item.Answers.Count);
                                while (choosedRandomIndex.IndexOf(randomIndex) >= 0)
                                {
                                    random = new Random();
                                    randomIndex = random.Next(0, item.Answers.Count);
                                }
                                choosedRandomIndex.Add(randomIndex);
                                item.Answers[randomIndex].PictureTime = OrderedAnswers[i].PictureTime;
                                item.Answers[randomIndex].AudioTime = OrderedAnswers[i].AudioTime;
                                RandomAnswers.Add(item.Answers[randomIndex]);
                            }
                        }
                        item.Answers = RandomAnswers;
                    }
                }


            }
            execMeasure.Items = list;
            execAssessment.Measures.Add(execMeasure);
            return execAssessment;
        }

        private ExecCpallsMeasureModel GetExecCpallsMeasureModel(int measureId)
        {
            var execMeasure = new ExecCpallsMeasureModel();
            if (measureId < 1)
            {
                execMeasure.Name = "Unknown";
                execMeasure.OrderType = OrderType.Sequenced;
                execMeasure.ShowType = ItemShowType.List;
            }
            else
            {
                var measure = AdeBusiness.GetMeasureModel(measureId);
                if (measure != null)
                {
                    execMeasure.EndPageHtml = measure.EndPageHtml;
                    execMeasure.InnerTime = measure.InnerTime;
                    execMeasure.MeasureId = measureId;
                    execMeasure.Name = measure.Name;
                    execMeasure.OrderType = measure.OrderType;
                    execMeasure.ShowType = measure.ItemType;
                    execMeasure.StartPageHtml = measure.StartPageHtml;
                    execMeasure.Timeout = measure.Timeout;
                    execMeasure.Links = measure.Links.Select(x => x.Link).ToList();
                    execMeasure.PreviousButton = measure.PreviousButton;
                    execMeasure.NextButton = measure.NextButton;
                    execMeasure.StopButton = measure.StopButton;
                }
            }
            return execMeasure;
        }


        public OperationResult CheckStudentMeasure(UserBaseEntity userInfo, int assessmentId, int studentId, int schoolId, string schoolYear, int year, Wave wave, List<int> measureIds)
        {
            OperationResult res = new OperationResult(OperationResultType.Success);
            var studentIds = new List<int>() { studentId };
            var studentAssessment = _cpallsContract.Assessments.FirstOrDefault(r => r.AssessmentId == assessmentId
                                                                   && r.StudentId == studentId
                                                                   && r.SchoolYear == schoolYear
                                                                   && r.Wave == wave);
            //var saId = _cpallsContract.GetStudentAssessmentIdForPlayMeasure(assessmentId, studentId, schoolYear,
            //    (int) wave);
            if (studentAssessment != null)
            {
                var saId = studentAssessment.ID;
                var dbList = _cpallsContract.Measures.Where(c => c.SAId == saId && measureIds.Contains(c.MeasureId)).ToList();
                var key = string.Format("Assessment_{0}_Wave_{1}_School_{2}_Students", assessmentId, wave, schoolId);
                var allStudents = CacheHelper.Get<List<CpallsStudentModel>>(key);
                if (allStudents != null)
                {
                    var cacheStudent = allStudents.FirstOrDefault(c => c.ID == studentId);
                    if (cacheStudent != null)
                    {
                        var cacheMeasures = cacheStudent.MeasureList;
                        foreach (var dbMeasure in dbList)
                        {
                            // if (studentAssessment.CreatedBy != userInfo.ID) David 12/15/2017 why??? should not have this condiction..
                            {
                                var findItem = cacheMeasures.FirstOrDefault(c => c.MeasureId == dbMeasure.MeasureId);
                                if (findItem == null)
                                {
                                    res.ResultType = OperationResultType.Error;
                                    res.Message = "This measure was launched before. Click OK and try again.";//"Data has changed, please click again.";
                                    UpdateCache(saId, dbList);
                                    break;
                                }
                                else if (findItem.Status != dbMeasure.Status || (dbMeasure.Goal != ((findItem.Goal == -1) ? -1 : findItem.Goal) && findItem.Goal != null))
                                {
                                    res.ResultType = OperationResultType.Error;
                                    res.Message = "This measure was launched before. Click OK and try again.";
                                    UpdateCache(saId, dbList);
                                    break;
                                }
                            }
                        }

                    }
                }
            }

            return res;
        }

        public OperationResult PlayMeasure(CpallsStudentModel student, int assessmentId, IEnumerable<int> measureIds, int studentAssessmentId, int year, Wave wave,
            UserBaseEntity UserInfo, out int newStudentAssessmentId)
        {
            var finalId = 0;
            EnsureCpallsMeasure(studentAssessmentId, assessmentId, wave, year, student, measureIds.ToList(), UserInfo, out finalId);
            newStudentAssessmentId = finalId;
            return new OperationResult(OperationResultType.Success);
        }

        private void EnsureCpallsMeasure(int stuAssId, int assessmentId, Wave wave, int year, CpallsStudentModel student,
            int measureId, UserBaseEntity UserInfo, out int stuAssIdAdded)
        {
            EnsureCpallsMeasure(stuAssId, assessmentId, wave, year, student, new List<int> { measureId }, UserInfo,
                out stuAssIdAdded);
        }
        private void EnsureCpallsMeasure(int stuAssId, int assessmentId, Wave wave, int year, CpallsStudentModel student,
            List<int> measureIds, UserBaseEntity UserInfo, out int stuAssIdAdded)
        {
            stuAssIdAdded = 0;
            var schoolYear = year.ToSchoolYearString();
            if (stuAssId < 1)
            {
                stuAssId = _cpallsContract.GetStudentAssessmentIdForPlayMeasure(assessmentId, student.ID, schoolYear,
                    (int)wave);
                //stuAssId = _cpallsContract.Assessments.Where(r => r.AssessmentId == assessmentId
                //                                                  && r.StudentId == student.ID
                //                                                  && r.SchoolYear == schoolYear
                //                                                  && r.Wave == wave)
                //    .Select(r => r.ID).FirstOrDefault();
            }
            stuAssIdAdded = stuAssId;
            if (_cpallsContract.Measures.Count(x => (x.SAId == stuAssId && measureIds.Contains(x.MeasureId))) < measureIds.Count)
            {
                InsertAssessment(student, wave, schoolYear, assessmentId, measureIds, UserInfo, out stuAssIdAdded);
                return;
            }
        }

        public OperationResult LockMeasure(CpallsStudentModel student, int measureId, int assessmentId, int studentAssessmentId, int schoolId,
            Wave wave, int year, UserBaseEntity UserInfo, CpallsStatus status)
        {
            var result = new OperationResult(OperationResultType.Error);
            int stuAssId = 0;
            EnsureCpallsMeasure(studentAssessmentId, assessmentId, wave, year, student, measureId, UserInfo, out stuAssId);
            //if (stuAssId < 1)
            //    _logger.Info("stuassID:" + stuAssId);

            StudentMeasureEntity entity = _cpallsContract.Measures.FirstOrDefault(r => r.MeasureId == measureId
                                                                                       && r.Assessment.ID == stuAssId);
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
                result = _cpallsContract.UpdateStudentMeasure(entity);
            }
            if (result.ResultType == OperationResultType.Success)
                UpdateCache(stuAssId, entity);
            return result;
        }

        Expression<Func<StudentMeasureEntity, StudentMeasureModel>> studentMeasureEntityToModel = x => new StudentMeasureModel()
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
            EOYHasCutOffScores = x.Measure.EOYHasCutOffScores,
            BenchamrkId = x.BenchmarkId,
            LowerScore = x.LowerScore,
            HigherScore = x.HigherScore
        };

        public StudentMeasureModel GetStudentMeasureModel(int id)
        {
            return
                _cpallsContract.Measures.Where(x => x.ID == id)
                    .Select(studentMeasureEntityToModel)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Updates the measure comment of student.
        /// </summary>
        /// <param name="id">The identifier of Student Measure.</param>
        /// <param name="comment">The new comment.</param>
        /// <returns></returns>
        /// Author : JackZhang
        /// Date   : 7/8/2015 11:59:42
        public OperationResult UpdateMeasureComment(int id, string comment)
        {
            var measure = _cpallsContract.GetStudentMeasure(id);
            //measure.UpdatedOn = DateTime.Now;
            measure.Comment = comment;

            return _cpallsContract.UpdateStudentMeasure(measure);
        }

        public OperationResult UpdateStudentMeasure(StudentMeasureEntity entity)
        {
            return _cpallsContract.UpdateStudentMeasure(entity);
        }

        public int UpdateStudentMeasureBenchmark(int studentMeasureId, int benchmarkId, decimal lowerScore, decimal higherScore, bool ShowOnGroup, bool benchmarkChanged)
        {
            return _cpallsContract.UpdateBenchmark(studentMeasureId, benchmarkId, lowerScore, higherScore, ShowOnGroup, benchmarkChanged);
        }

        public int UpdateStudentMeasurePercentileRank(int studentMeasureId, string percentileRank)
        {
            return _cpallsContract.UpdatePercentileRank(studentMeasureId, percentileRank);
        }

        public int UpdateBenchmarkChangedToFalse(int measureId)
        {
            return _cpallsContract.UpdateBenchmarkChangedToFalse(measureId);
        }

        public OperationResult UpdateStudentMeasures(List<StudentMeasureEntity> entities)
        {
            return _cpallsContract.UpdateStudentMeasures(entities);
        }

        public List<StudentMeasureEntity> GetStuMeasuresByStuId(int stuId)
        {
            List<StudentMeasureEntity> entities = _cpallsContract.Measures
                .Where(m => m.Assessment.StudentId == stuId
               ).ToList();
            return entities;
        }

        //David 05/30/2017 Apply to all studentMeasure data exclude locked data
        public List<StudentMeasureEntity> GetStuMeasuresByMeasureId(int measureId)
        {
            List<StudentMeasureEntity> entities = _cpallsContract.Measures
                .Where(m => m.MeasureId == measureId
                    && m.Status != CpallsStatus.Locked).ToList();
            return entities;
        }
        public List<StudentMeasureEntity> GetStuMeasuresByMeasureId(int measureId, DateTime measureUpdatedOn)
        {
            List<StudentMeasureEntity> entities = _cpallsContract.Measures
                .Where(m => m.MeasureId == measureId
                    && m.Status != CpallsStatus.Locked
                    && m.UpdatedOn < measureUpdatedOn).ToList();
            return entities;
        }
        //David 05/30/2017 Only Resore the data after Last Run Time
        public List<StudentMeasureEntity> GetStuMeasuresAfterLastRunTime(int measureId, DateTime lastRunTime)
        {
            List<StudentMeasureEntity> entities = _cpallsContract.Measures
                .Where(m => m.MeasureId == measureId
                    && m.Status != CpallsStatus.Locked
                    && m.UpdatedOn >= lastRunTime).ToList();
            return entities;
        }

        public List<StudentMeasureModel> GetStuMeasuresForParentReports(int measureId, Wave wave, List<int> studentIds)
        {
            List<StudentMeasureModel> entities = _cpallsContract.Measures.Where(m => m.MeasureId == measureId && m.Assessment.Wave == wave && studentIds.Contains(m.Assessment.StudentId))
                 .Select(studentMeasureEntityToModel).ToList();
            return entities;
        }
    }
}