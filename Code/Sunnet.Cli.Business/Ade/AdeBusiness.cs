
using System.Web.UI.WebControls;
using LinqKit;
using StructureMap;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/12 22:44:49
 * Description:		Please input class summary
 * Version History:	Created,2014/8/12 22:44:49
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Log;
using Sunnet.Framework.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MeasureModel = Sunnet.Cli.Business.Ade.Models.MeasureModel;
using Sunnet.Cli.Business.Permission;
using Sunnet.Cli.Business.Students;

namespace Sunnet.Cli.Business.Ade
{
    public partial class AdeBusiness
    {
        private readonly IAdeContract _adeContract;
        private ISunnetLog logger;
        private UserBusiness _userBusiness;
        private readonly ICpallsContract _cpallsContract;
        private StudentBusiness _studentBusiness;

        private UserBusiness UserBusiness
        {
            get { return _userBusiness ?? (_userBusiness = new UserBusiness()); }
            set { _userBusiness = value; }
        }
        private StudentBusiness StudentBusiness
        {
            get { return _studentBusiness ?? (_studentBusiness = new StudentBusiness()); }
            set { _studentBusiness = value; }
        }

        public AdeBusiness(AdeUnitOfWorkContext unit = null)
        {
            _adeContract = DomainFacade.CreateAdeService(unit);
            logger = ObjectFactory.GetInstance<ISunnetLog>();
            _cpallsContract = DomainFacade.CreateCpallsService(unit);
        }

        public OperationResult SaveAssessment(AssessmentModel model)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            if (_adeContract.Assessments.Any(x => x.ID != model.ID && x.IsDeleted == false
                && x.Label == model.Label))
            {
                result.Message = ResourceHelper.GetRM().GetInformation("Ade_SameAssessmentLabel");
                return result;
            }

            if (_adeContract.Assessments.Any(x => x.ID != model.ID && x.IsDeleted == false
                            && x.Name == model.Name && x.Language == model.Language))
            {
                result.Message = ResourceHelper.GetRM().GetInformation("Ade_AssessmentExsited");
                return result;
            }
            bool isSameLabel = true;
            bool isEdit = true;
            AssessmentEntity entity = null;
            if (model.ID < 1)
            {
                isEdit = false;
                entity = _adeContract.NewAssessmentEntity();

                entity.IsDeleted = false;
                entity.UpdatedBy = model.CreatedBy;
                entity.CreatedBy = model.UpdatedBy;
                entity.Type = model.Type;
                entity.Description = model.Description;
            }
            else
            {
                entity = _adeContract.GetAssessment(model.ID);
                isSameLabel = entity.Label == model.Label;
            }
            entity.Status = model.Status;
            if (!entity.Locked)
            {
                entity.Language = model.Language;
                entity.Label = model.Label;
                entity.Name = model.Name;
                entity.OrderType = model.OrderType;
                entity.TotalScored = model.TotalScored;
                entity.Description = model.Description;
                entity.ParentReportCoverName = model.ParentReportCoverName ?? "";
                entity.ParentReportCoverPath = model.ParentReportCoverPath ?? "";
                entity.DisplayPercentileRank = model.DisplayPercentileRank;
            }
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = model.UpdatedBy;

            if (model.ID < 1)
            {
                result = _adeContract.InsertAssessment(entity);
                if (result.ResultType == OperationResultType.Success)
                    new PermissionBusiness().AddAssessmentPage(entity.ID, model.Label, (int)model.Type);
            }
            else
            {
                result = _adeContract.UpdateAssessment(entity);
                if (result.ResultType == OperationResultType.Success && !isSameLabel)
                    new PermissionBusiness().UpdateAssessmentPage(entity.ID, model.Label, (int)model.Type);
            }

            model.ID = entity.ID;
            model.CreatedOn = entity.CreatedOn;
            model.UpdatedOn = entity.UpdatedOn;
            var userName = UserBusiness.GetUsername(entity.CreatedBy);
            model.CreatedByName = userName != null ? userName.ShowName : "";
            //if (model.CutOffScores.Count > 0)
            //{
            //    if (result.ResultType == OperationResultType.Success)
            //    {
            //        result = _adeContract.DeleteCutOffScore<AssessmentEntity>(model.ID);
            //    }
            //    if (result.ResultType == OperationResultType.Success)
            //    {
            //        model.CutOffScores.ForEach(x => x.HostId = model.ID);
            //        result = _adeContract.InsertCutOffScore<AssessmentEntity>(model.CutOffScores);
            //    }
            //}
            if (model.Benchmarks.Count > 0)
            {
                model.Benchmarks.ForEach(x => x.AssessmentId = model.ID);
                if (isEdit)
                {
                    var modelBenchmarks = model.Benchmarks;
                    var addBenchmarks = modelBenchmarks.Where(x => x.ID == 0).ToList();

                    var updateBenchmarks = modelBenchmarks.Where(x => x.ID != 0).ToList();

                    var currentBenchmarkIds = _adeContract.Benchmarks.Where(x => x.AssessmentId == model.ID).Select(x => x.ID).ToList();
                    var modelBenchmarkIds = updateBenchmarks.Select(x => x.ID).ToList();
                    var deleteBenchmarkIds = currentBenchmarkIds.Except(modelBenchmarkIds).ToList();

                    if (result.ResultType == OperationResultType.Success)
                    {
                        result = _adeContract.InsertBenchmarks(addBenchmarks);
                    }
                    if (result.ResultType == OperationResultType.Success)
                    {
                        result = _adeContract.DeleteBenchmarks(deleteBenchmarkIds);
                    }
                    if (result.ResultType == OperationResultType.Success)
                    {
                        result = _adeContract.UpdateBenchmarks(updateBenchmarks);
                    }
                }
                else
                {
                    result = _adeContract.InsertBenchmarks(model.Benchmarks);
                }
            }
            return result;
        }

        public OperationResult DeleteAssessment(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            var entity = _adeContract.GetAssessment(id);
            if (entity == null)
            {
                return result;
            }
            if (_adeContract.Assessments.Any(x => x.ID == id && x.Locked))
            {
                return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
            }
            if (_adeContract.Measures.Count(x => x.AssessmentId == id && x.IsDeleted == false) > 0)
            {
                result.Message =
                    ResourceHelper.GetRM().GetInformation("Ade_CannotDeleteAssessment");
                result.ResultType = OperationResultType.ParamError;
                return result;
            }
            entity.Status = EntityStatus.Inactive;
            entity.IsDeleted = true;
            result = _adeContract.UpdateAssessment(entity);
            return result;
        }

        public OperationResult ChangeAssessmentStatus(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            var entity = _adeContract.GetAssessment(id);
            if (entity == null)
            {
                return result;
            }
            entity.Status = entity.Status == EntityStatus.Inactive ? EntityStatus.Active : EntityStatus.Inactive;
            if (entity.Status == EntityStatus.Active)
            {
                ClearCahce(id);
            }
            entity.UpdatedOn = DateTime.Now;
            result = _adeContract.UpdateAssessment(entity);
            result.AppendData = entity.Status;
            return result;
        }

        public AssessmentEntity GetAssessment(int id)
        {
            return _adeContract.GetAssessment(id);
        }
        public bool IsAssessmentExist(int id)
        {
            return _adeContract.IsAssessmentExist(id);
        }
        public BaseAssessmentModel GetBaseAssessmentModel(int id)
        {
            var model =
                _adeContract.Assessments.Where(
                    x => x.IsDeleted == false && x.Status == EntityStatus.Active && x.ID == id)
                    .Select(x => new BaseAssessmentModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Language = x.Language,
                        ParentReportCoverPath = x.ParentReportCoverPath,
                        DisplayPercentileRank = x.DisplayPercentileRank
                    }).FirstOrDefault();
            if (model != null)
            {
                var theOther =
                    _adeContract.Assessments.Where(
                        x => x.IsDeleted == false && x.Status == EntityStatus.Active && x.Name == model.Name && x.ID != model.ID)
                        .Select(x => new BaseAssessmentModel()
                        {
                            ID = x.ID,
                            Name = x.Name,
                            Language = x.Language,
                            ParentReportCoverPath = x.ParentReportCoverPath,
                            DisplayPercentileRank = x.DisplayPercentileRank
                        }).FirstOrDefault();
                if (theOther != null)
                {
                    model.TheOtherId = theOther.ID;
                    model.TheOtherLang = theOther.Language;
                }
            }
            return model;
        }


        public BaseAssessmentModel GetBaseAssessmentModelForReport(int id)
        {
            var model =
                _adeContract.Assessments.Where(
                    x => x.IsDeleted == false && x.ID == id)
                    .Select(x => new BaseAssessmentModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Language = x.Language,
                        ParentReportCoverPath = x.ParentReportCoverPath
                    }).FirstOrDefault();
            if (model != null)
            {
                var theOther =
                    _adeContract.Assessments.Where(
                        x => x.IsDeleted == false && x.Name == model.Name && x.ID != model.ID)
                        .Select(x => new BaseAssessmentModel()
                        {
                            ID = x.ID,
                            Name = x.Name,
                            Language = x.Language,
                            ParentReportCoverPath = x.ParentReportCoverPath
                        }).FirstOrDefault();
                if (theOther != null)
                {
                    model.TheOtherId = theOther.ID;
                    model.TheOtherLang = theOther.Language;
                }
            }
            return model;
        }

        /// <summary>
        ///  获取另一个语言版本的Assessment
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Find another language version of assessment ( + assessmentId + ) error, database error.</exception>
        public AssessmentEntity GetTheOtherLanguageAssessment(int assessmentId)
        {
            var target =
                _adeContract.Assessments.FirstOrDefault(assessment => assessment.ID == assessmentId);
            if (target == null)
            {
                throw new Exception("Find another language version of assessment (" + assessmentId + ") error, database error.");
            }
            var theOther =
                _adeContract.Assessments.FirstOrDefault(assessment => assessment.Name == target.Name && assessment.Language != target.Language);
            return theOther;
        }
        public AssessmentEntity GetTheOtherLanguageAssessment(int assessmentId, string name)
        {
            var target =
                _adeContract.Assessments.FirstOrDefault(assessment => assessment.ID == assessmentId);
            if (target == null)
            {
                throw new Exception("Find another language version of assessment (" + assessmentId + ") error, database error.");
            }
            var theOther =
                _adeContract.Assessments.FirstOrDefault(assessment => assessment.Name == name && assessment.ID != assessmentId);
            return theOther;
        }
        public AssessmentModel GetAssessmentModel(int id)
        {
            var model = _adeContract.Assessments.Select(SelectorAssessmentEntityToModel).FirstOrDefault(x => x.ID == id);
            if (model != null)
            {
                model.CutOffScores = this.GetCutOffScores<AssessmentEntity>(id);
                model.Benchmarks = this.GetBenchmarks(id);
            }
            return model;
        }

        public List<SelectItemModel> GetAssessments(Expression<Func<AssessmentEntity, bool>> condition)
        {
            var query = _adeContract.Assessments.Where(x => x.IsDeleted == false)
                .Where(condition).OrderBy(x => x.Label).Select(x => new SelectItemModel()
                {
                    Name = x.Label,
                    ID = x.ID
                });
            return query.ToList();
        }

        public List<AssessmentModel> SearchAssessments(Expression<Func<AssessmentEntity, bool>> condition,
            string sort, string order, int first, int count, out int total)
        {
            var query = _adeContract.Assessments.AsExpandable().Where(x => x.IsDeleted == false)
                .Where(condition).Select(SelectorAssessmentEntityToModel);
            total = query.Count();
            var list = query.OrderBy(sort, order).AsQueryable().Skip(first).Take(count);
            var users = UserBusiness.GetUsernames(list.Select(x => x.CreatedBy));
            var result = list.ToList();
            result.ForEach(x =>
            {
                if (users.Find(y => y.ID == x.CreatedBy) != null)
                    x.CreatedByName = users.Find(y => y.ID == x.CreatedBy).ShowName;
            });
            return result;
        }

        private static Expression<Func<AssessmentEntity, AssessmentModel>> SelectorAssessmentEntityToModel
        {
            get
            {
                return x => new AssessmentModel()
                {
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    ID = x.ID,
                    Label = x.Label,
                    Name = x.Name,
                    OrderType = x.OrderType,
                    Status = x.Status,
                    Language = x.Language,
                    Locked = x.Locked,
                    TotalScored = x.TotalScored,
                    Type = x.Type,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedOn = x.UpdatedOn,
                    Description = x.Description,
                    MeasureCount = x.Measures.Count(y => y.IsDeleted == false),
                    ParentReportCoverName = x.ParentReportCoverName,
                    ParentReportCoverPath = x.ParentReportCoverPath,
                    DisplayPercentileRank = x.DisplayPercentileRank
                };
            }
        }

        public bool CanExecuteAssessment(int id)
        {
            if (id < 1)
                return false;
            return
                _adeContract.Assessments.Any(x => x.ID == id && x.Status == EntityStatus.Active && x.IsDeleted == false);
        }

        public OperationResult LockAssessment(int id)
        {
            var entity = _adeContract.GetAssessment(id);
            if (entity.Locked != true)
            {
                entity.Locked = true;
                entity.UpdatedOn = DateTime.Now;
                return _adeContract.UpdateAssessment(entity);
            }
            return new OperationResult(OperationResultType.Success);
        }

        public OperationResult UnockAssessment(int id, UserBaseEntity user)
        {
            var entity = _adeContract.GetAssessment(id);
            entity.Locked = false;
            entity.Status = EntityStatus.Inactive;
            entity.UpdatedOn = DateTime.Now;
            //logger.Info(string.Format("User: ID,{0};Name:{1} {2};\r\n Action: UnlockAssessment, ID:{3}",
            //    user.ID,
            //    user.FirstName,
            //    user.LastName,
            //    id));
            var result = _adeContract.UpdateAssessment(entity);
            result.AppendData = entity.Status;
            return result;
        }

        public OperationResult UpdateAssessment(AssessmentEntity entity)
        {
            return _adeContract.UpdateAssessment(entity);
        }
        /// <summary>
        /// Gets the cut off scores.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The HostId.</param>
        /// <returns></returns>
        public List<CutOffScoreEntity> GetCutOffScores<T>(int id) where T : ICutOffScoreProperties
        {
            var type = typeof(T).ToString();
            return _adeContract.CutOffScores.Where(x => x.HostId == id && x.HostType == type)
                .OrderBy(x => x.FromYear).ThenBy(x => x.FromMonth).ThenBy(x => x.LowerScore)
                .ToList();
        }

        /// <summary>
        /// Gets the cut off scores.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hostIds">The host ids.</param>
        /// <returns></returns>
        public List<CutOffScoreEntity> GetCutOffScores<T>(IEnumerable<int> hostIds) where T : ICutOffScoreProperties
        {
            var type = typeof(T).ToString();
            return _adeContract.CutOffScores.Where(x => hostIds.Contains(x.HostId) && x.HostType == type)
                .OrderBy(x => x.FromYear).ThenBy(x => x.FromMonth)
                .ToList();
        }

        /// <summary>
        /// Gets the cut off scores.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hostIds">The host ids.</param>
        /// <returns></returns>
        public List<CutOffScoreEntity> GetCutOffScores<T>(IEnumerable<int> hostIds, Wave wave) where T : ICutOffScoreProperties
        {
            var type = typeof(T).ToString();
            return _adeContract.CutOffScores.Where(x => hostIds.Contains(x.HostId) && x.HostType == type && x.Wave == wave)
                .OrderBy(x => x.FromYear).ThenBy(x => x.FromMonth)
                .ToList();
        }

        /// <summary>
        /// Gets the cut off scores.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The HostId.</param>
        /// <param name="wave">The wave.</param>
        /// <returns></returns>
        public List<CutOffScoreEntity> GetCutOffScores<T>(int id, Wave wave) where T : ICutOffScoreProperties
        {
            var type = typeof(T).ToString();
            return _adeContract.CutOffScores.Where(x => x.HostId == id && x.Wave == wave && x.HostType == type)
                .OrderBy(x => x.FromYear).ThenBy(x => x.FromMonth)
                .ToList();
        }

        public CutOffScoreEntity GetCutOffScore<T>(int measureId, Wave wave, string schoolYear,
            DateTime birthday, decimal goal)
            where T : ICutOffScoreProperties
        {
            var type = typeof(T).ToString();
            /// todo:Sam Add Log
         
            //logger.Info(@"C--> AdeBusiness.GetCutOffScore,MeasureId:" + measureId+",Wave:"+wave.ToString()+",SchoolYear:"+schoolYear+",Birthday:"+birthday+",Goal:"+goal+",type:"+type);
            var list = _adeContract.CutOffScores.Where(
                x => x.HostType == type && x.HostId == measureId && x.Wave == wave
                     && x.LowerScore <= goal && x.HigherScore >= goal).ToList();
            //if(list!= null && list.Count>0)
            //logger.Info(@"D--> AdeBusiness.GetCutOffScore,List:" + string.Join(",",list.Select(c => c.BenchmarkId).ToArray()));
            //else
            //    logger.Info(@"D--> AdeBusiness.GetCutOffScore,List:NULL");

            var currentYear = CommonAgent.GetStartDateForAge(schoolYear);
            foreach (var entity in list)
            {
                var endDate = currentYear.AddYears(0 - entity.FromYear);
                endDate = endDate.AddMonths(0 - entity.FromMonth);
                var startDate = currentYear.AddYears(0 - entity.ToYear);
                startDate = startDate.AddMonths(0 - entity.ToMonth);

                //logger.Info(@"E--> AdeBusiness.GetCutOffScore,CutOffScore ID:" + entity.ID+" StartDate:"+ startDate+",EndDate:"+endDate+",birthDate:"+birthday);
                if (birthday > startDate && birthday <= endDate)
                {
                    return entity;
                }
            }
            return null;
        }

        public CutOffScoreEntity GetCutOffScore<T>(List<CutOffScoreEntity> cutoffScoreList, string schoolYear,
            DateTime birthday, decimal goal)
            where T : ICutOffScoreProperties
        {
            var type = typeof(T).ToString();
            //logger.Info(@"C2--> AdeBusiness.GetCutOffScore,cutoffScoreList:" + String.Join(",",cutoffScoreList.Select(c=>c.ID).ToList()) + ",SchoolYear:" + schoolYear + ",Birthday:" + birthday + ",Goal:" + goal + ",type:" + type);

            var list = cutoffScoreList.Where(x => x.LowerScore <= goal && x.HigherScore >= goal).ToList();
            //if (list != null && list.Count > 0)
            //    logger.Info(@"D2--> AdeBusiness.GetCutOffScore,List:" + string.Join(",", list.Select(c => c.BenchmarkId).ToArray()));
            var currentYear = Convert.ToDateTime("20" + schoolYear.Substring(0, 2) + "-9-1");
            foreach (var entity in list)
            {
                var endDate = currentYear.AddYears(0 - entity.FromYear);
                endDate = endDate.AddMonths(0 - entity.FromMonth);
                var startDate = currentYear.AddYears(0 - entity.ToYear);
                startDate = startDate.AddMonths(0 - entity.ToMonth);
                //logger.Info(@"E2--> AdeBusiness.GetCutOffScore,CutOffScore ID:" + entity.ID + " StartDate:" + startDate + ",EndDate:" + endDate + ",birthDate:" + birthday);
                if (birthday > startDate && birthday <= endDate)
                {
                    return entity;
                }
            }
            return null;
        }

        public OperationResult UpdateCutoffScores<T>(int id, List<CutOffScoreEntity> scores)
            where T : ICutOffScoreProperties
        {
            var type = typeof(T);
            if (type == typeof(AssessmentEntity))
            {
                if (_adeContract.Assessments.Any(x => x.ID == id && x.Locked))
                {
                    return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
                }
            }
            if (type == typeof(AssessmentEntity))
            {
                if (_adeContract.Measures.Any(x => x.ID == id && x.Assessment.Locked))
                {
                    return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
                }
            }
            var result = _adeContract.DeleteCutOffScore<T>(id);
            if (result.ResultType != OperationResultType.Success)
                return result;
            scores.ForEach(x => x.HostId = id);
            result = _adeContract.InsertCutOffScore<T>(scores);
            return result;
        }

        public List<AdeLinkEntity> GetLinks<T>(int id) where T : IAdeLinkProperties
        {
            var type = typeof(T).ToString();
            return _adeContract.AdeLinks.Where(x => x.HostId == id && x.HostType == type)
                .OrderBy(x => x.Link)
                .ToList();
        }

        public OperationResult UpdateLinks<T>(int id, IEnumerable<AdeLinkEntity> links)
            where T : IAdeLinkProperties
        {
            var type = typeof(T);
            if (type == typeof(AssessmentEntity))
            {
                if (_adeContract.Assessments.Any(x => x.ID == id && x.Locked))
                {
                    return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
                }
            }
            if (type == typeof(AssessmentEntity))
            {
                if (_adeContract.Measures.Any(x => x.ID == id && x.Assessment.Locked))
                {
                    return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
                }
            }
            var result = _adeContract.DeleteLink<T>(id, links.FirstOrDefault().LinkType);
            if (result.ResultType != OperationResultType.Success)
                return result;
            var targetToAdd = links.Where(x => !string.IsNullOrEmpty(x.Link)).ToList();
            targetToAdd.ForEach(x => x.HostId = id);
            result = _adeContract.InsertLink<T>(targetToAdd);
            return result;
        }

        private static Expression<Func<MeasureEntity, MeasureModel>> SelectorMeasureEntityToModel
        {
            get
            {
                return x => new MeasureModel()
                {
                    AssessmentId = x.AssessmentId,
                    AssessmentLabel = x.AssessmentId > 0 ? x.Assessment.Label : "",
                    AssessmentType = x.Assessment.Type,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    EndPageHtml = x.EndPageHtml,
                    ID = x.ID,
                    InnerTime = x.InnerTime,
                    IsDeleted = x.IsDeleted,
                    ItemCount = x.Items.Count(),
                    ItemType = x.ItemType,
                    Label = x.Label,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    OrderType = x.OrderType,
                    ParentId = x.ParentId,
                    Sort = x.Sort,
                    StartPageHtml = x.StartPageHtml,
                    Status = x.Status,
                    SubMeasureCount = x.SubMeasures.Count(),
                    Timeout = x.Timeout,
                    TotalScored = x.TotalScored,
                    TotalScore = x.TotalScore,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedOn = x.UpdatedOn,
                    Locked = x.Assessment.Locked,
                    ApplyToWaveValues = x.ApplyToWave,
                    RelatedMeasureId = x.RelatedMeasureId,
                    PreviousButton = x.PreviousButton,
                    StopButton = x.StopButton,
                    NextButton = x.NextButton,
                    Note = x.Note,
                    LightColor = x.LightColor,
                    HasCutOffScores = x.HasCutOffScores,
                    BOYHasCutOffScores = x.BOYHasCutOffScores,
                    MOYHasCutOffScores = x.MOYHasCutOffScores,
                    EOYHasCutOffScores = x.EOYHasCutOffScores,
                    GroupByParentMeasure = x.GroupByParentMeasure,
                    ShowLaunchPage = x.ShowLaunchPage,
                    ShowFinalizePage = x.ShowFinalizePage,
                    Description = x.Description,
                    PercentileRank = x.PercentileRank,
                    GroupByLabel = x.GroupByLabel
                };
            }
        }

        public List<MeasureModel> SearchMeasures(int assessmentId, Func<MeasureModel, bool> condition, out int total,
            string sort = "Sort", string order = "asc")
        {
            var query = GetMeasuresForAssessment(assessmentId);
            total = query.Where(x => x.ParentId > 0).Where(condition).Count();
            var list = query.Where(x => x.ParentId > 0).Where(condition).ToList();
            var users = UserBusiness.GetUsernames(list.Select(x => x.CreatedBy));
            var result = list.ToList();
            result.ForEach(x =>
            {
                if (users.Find(y => y.ID == x.CreatedBy) != null)
                    x.CreatedByName = users.Find(y => y.ID == x.CreatedBy).ShowName;
            });
            return result;
        }

        public IEnumerable<SelectItemModel> GetMeasures(int assessmentId)
        {
            var query = _adeContract.Measures.Where(x => (x.AssessmentId == assessmentId
                && x.IsDeleted == false && x.ParentId > 0)
                ).Select(SelectorMeasureEntityToModel).OrderBy("Label", "Asc");
            var list = query.ToList();
            list.ForEach(x =>
            {
                if (x.SubMeasureCount > 0)
                    x.SubMeasures = list.Where(y => y.ParentId == x.ID).AsQueryable().OrderBy("Sort", "Asc").ToList();
                x.Parent = list.Find(y => y.ID == x.ParentId);
            });
            var list2 = list.Where(x => x.ParentId == 1).ToList();
            var listOrdered = new List<MeasureModel>();
            const char separator = (char)0xA0; //(char)0xa0 表示空格
            ResetOrder(list2, listOrdered, string.Empty.PadLeft(4, separator));
            return listOrdered.Where(x => x.ParentId > 0).ToList()
                .Select(x => new SelectItemModel()
                {
                    ID = x.ID,
                    Name = x.NamePrefix + x.Label
                });
        }

        public List<MeasureEntity> GetMeasureEntitiesByIds(List<int> measureIdList)
        {
            return _adeContract.Measures.Where(x => measureIdList.Contains(x.ID)).ToList();
        }

        /// <summary>
        /// 查询Measure的父子结构关系
        /// 缓存，由ADE更新维护
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns></returns>
        public Dictionary<int, IEnumerable<int>> GetMeasureTree(int assessmentId)
        {
            var tree = new Dictionary<int, IEnumerable<int>>();
            if (assessmentId < 1)
                return tree;
            var key = string.Format("Assessment_{0}_Measure_Tree", assessmentId);
            var result = CacheHelper.Get<Dictionary<int, IEnumerable<int>>>(key);
            if (result == null)
            {
                lock (CacheHelper.Synchronize)
                {
                    result = CacheHelper.Get<Dictionary<int, IEnumerable<int>>>(key);
                    if (result == null)
                    {
                        result = _adeContract.Measures.Where(
                            x =>
                                x.AssessmentId == assessmentId && x.SubMeasures.Any() && x.IsDeleted == false &&
                                x.Status == EntityStatus.Active)
                            .ToDictionary(m => m.ID,
                                m => m.SubMeasures.Where(x => x.IsDeleted == false && x.Status == EntityStatus.Active)
                                    .Select(sM => sM.ID));
                        CacheHelper.Add(key, result);
                    }
                }
            }
            return result;
        }

        public Dictionary<int, IEnumerable<int>> GetMeasureTree(List<int> measureIds)
        {
            var tree = new Dictionary<int, IEnumerable<int>>();
            lock (CacheHelper.Synchronize)
            {
                var test = _adeContract.Measures.Where(
                    x =>
                        measureIds.Contains(x.ID) && x.ParentId > 1 && x.IsDeleted == false &&
                        x.Status == EntityStatus.Active).Select(e => e.Parent);
                tree = test.ToDictionary(m => m.ID,
                    m => m.SubMeasures.Where(x => x.IsDeleted == false && x.Status == EntityStatus.Active)
                        .Select(sM => sM.ID));
            }
            return tree;
        }


        /// <summary>
        /// 负载均衡下的实时读取数据
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns></returns>
        public Dictionary<int, IEnumerable<int>> GetMeasureTreeForOffline(int assessmentId)
        {
            var tree = new Dictionary<int, IEnumerable<int>>();
            if (assessmentId < 1)
                return tree;
            var result = _adeContract.Measures.Where(
                              x =>
                                  x.AssessmentId == assessmentId && x.SubMeasures.Any() && x.IsDeleted == false &&
                                  x.Status == EntityStatus.Active)
                              .ToDictionary(m => m.ID,
                                  m => m.SubMeasures.Where(x => x.IsDeleted == false && x.Status == EntityStatus.Active)
                                      .Select(sM => sM.ID));
            return result;
        }


        public List<MeasureHeaderModel> GetHeaderMeasures(Expression<Func<MeasureEntity, bool>> condition)
        {
            return
                _adeContract.Measures.AsExpandable().Where(m => m.IsDeleted == false && m.Status == EntityStatus.Active)
                    .Where(condition)
                    .Select(r => new MeasureHeaderModel()
                    {
                        ID = 0,
                        MeasureId = r.ID,
                        Name = r.Name,
                        ParentId = r.ParentId,
                        ParentMeasureName = r.Parent.Name,
                        TotalScored = r.TotalScored,
                        TotalScore = r.TotalScored ? r.TotalScore : (decimal?)null,
                        Sort = r.Sort,
                        ApplyToWave = r.ApplyToWave,
                        RelatedMeasureId = r.RelatedMeasureId,
                        Note = r.Note,
                        LightColor = r.LightColor,
                        PercentileRank = r.PercentileRank,
                        GroupByLabel = r.GroupByLabel
                    }).ToList();
        }

        public List<ReportModel> GetReportModel(Expression<Func<MeasureEntity, bool>> condition)
        {
            return
                _adeContract.Measures.AsExpandable().Where(m => m.IsDeleted == false && m.Status == EntityStatus.Active)
                    .Where(condition)
                    .Select(r => new ReportModel()
                    {
                        MeasureId = r.ID,
                        MeasureName = r.Name,
                        ParentId = r.ParentId,
                        TotalScored = r.TotalScored,
                        Sort = r.Sort,
                        ApplyToWave = r.ApplyToWave,
                        RelatedId = r.RelatedMeasureId
                    }).OrderBy(r => r.Sort).ToList();
        }

        private void ResetOrder(List<MeasureModel> sourceList, List<MeasureModel> targetList, string separator)
        {
            if (sourceList == null || sourceList.Count == 0 || targetList == null)
                return;
            sourceList.ForEach(x =>
            {
                if (x.ParentId > 1)
                    x.NamePrefix = separator;
                targetList.Add(x);
                if (x.SubMeasures != null && x.SubMeasures.Count > 0)
                {
                    ResetOrder(x.SubMeasures, targetList,
                        x.ParentId > 1 ?
                        (separator + separator) : separator);
                }
            });
        }
        /// <summary>
        /// Gets all the measures for assessment, include root Measure.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        private IEnumerable<MeasureModel> GetMeasuresForAssessment(int assessmentId, string sort = "Sort", string order = "asc")
        {
            // 查询该Assessment下面所有的Measure, 以及根父级类型
            var query = _adeContract.Measures.Where(x => (x.AssessmentId == assessmentId
                && x.IsDeleted == false)
                || x.ParentId == 0
                ).Select(SelectorMeasureEntityToModel).OrderBy(sort, order).ThenBy("Label");
            var list = query.ToList();
            list.ForEach(x =>
            {
                if (x.SubMeasureCount > 0)
                    x.SubMeasures = list.Where(y => y.ParentId == x.ID).AsQueryable().OrderBy(sort, order).ToList();
                x.Parent = list.Find(y => y.ID == x.ParentId);
            });
            var list2 = list.Where(x => x.ParentId == 0).ToList();
            var listOrdered = new List<MeasureModel>();
            const char separator = (char)0xA0; //表示空格
            ResetOrder(list2, listOrdered, string.Empty.PadLeft(4, separator));
            return listOrdered;
        }

        /// <summary>
        /// Gets the parent measures for assessment(New | Edit).
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        public List<MeasureModel> GetParentMeasuresForAssessment(int assessmentId)
        {
            var query = _adeContract.Measures.Where(x => (x.AssessmentId == assessmentId
                && x.IsDeleted == false && !x.Items.Any() && x.ParentId < 2)
                || x.ParentId == 0
                ).Select(SelectorMeasureEntityToModel).OrderBy("Label", "Asc");
            var list = query.ToList();
            list.ForEach(x =>
            {
                if (x.SubMeasureCount > 0)
                    x.SubMeasures = list.Where(y => y.ParentId == x.ID).AsQueryable().OrderBy("Sort", "Asc").ToList();
                x.Parent = list.Find(y => y.ID == x.ParentId);
            });
            var list2 = list.Where(x => x.ParentId == 0).ToList();
            var listOrdered = new List<MeasureModel>();
            const char separator = (char)0xA0;
            ResetOrder(list2, listOrdered, string.Empty.PadLeft(4, separator));
            return listOrdered;
        }

        public List<RelateModel> GetMeasuresForAssessment(bool isParentMeasure, int assessmentId, int relatedMeasureId = 0)
        {
            if (isParentMeasure)
            {
                return _adeContract.Measures.Where(x => x.AssessmentId == assessmentId && x.IsDeleted == false && x.ParentId == 1
                    && ((x.Status == EntityStatus.Active && x.RelatedMeasureId == 0) || x.ID == relatedMeasureId))
                    .OrderBy(x => x.Sort)
                    .Select(r => new RelateModel()
                    {
                        ID = r.ID,
                        ParentId = r.ParentId,
                        Name = r.Name
                    }).ToList();
            }
            else
            {
                return _adeContract.Measures.Where(x => x.AssessmentId == assessmentId && x.IsDeleted == false && x.ParentId > 1
                    && ((x.Parent.IsDeleted == false && x.Status == EntityStatus.Active && x.RelatedMeasureId == 0) || x.ID == relatedMeasureId))
                    .OrderBy(x => x.Sort)
                    .Select(o => new RelateModel()
                    {
                        ID = o.ID,
                        ParentId = o.ParentId,
                        ParentRelationId = o.Parent.RelatedMeasureId,
                        Name = o.Name
                    }).ToList();
            }
        }

        /// <summary>
        /// Gets the parent measures for assessment(New | Edit).
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <returns></returns>
        public List<MeasureModel> GetMeasuresForAdjustOrder(int assessmentId, int parentId)
        {
            var query = GetMeasuresForAssessment(assessmentId).Where(x => x.ParentId == parentId).ToList();
            var users = UserBusiness.GetUsernames(query.Select(x => x.CreatedBy));
            query.ForEach(x =>
            {
                if (users.Find(y => y.ID == x.CreatedBy) != null)
                    x.CreatedByName = users.Find(y => y.ID == x.CreatedBy).ShowName;
            });
            return query;
        }

        public OperationResult SaveMeasure(MeasureModel model)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            if (_adeContract.Assessments.Any(x => x.ID == model.AssessmentId && x.Locked))
            {
                return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
            }
            if (_adeContract.Measures.Any(x => x.ID != model.ID && x.Label == model.Label && x.IsDeleted == false))
            {
                result.Message = ResourceHelper.GetRM().GetInformation("Ade_SameMeasureLabel");
                return result;
            }
            if (model.PercentileRank && !_adeContract.PercentileRanks.Any(x => x.MeasureLabel == model.Label))
            {
                result.Message = ResourceHelper.GetRM().GetInformation("NotPercentileRank").Replace("{0}", model.Label);
                return result;
            }
            MeasureEntity entity = null;
            int oldRelatedMeasureId = 0;
            int oldParentId = 1;
            bool isEdit = false;
            if (model.ID < 1)
            {
                entity = _adeContract.NewMeasureEntity();
                entity.IsDeleted = false;
                entity.UpdatedBy = model.CreatedBy;
                entity.CreatedBy = model.UpdatedBy;
                entity.AssessmentId = model.AssessmentId;
                entity.RelatedMeasureId = model.RelatedMeasureId;

                entity.Sort = _adeContract.Measures.Where(x => x.AssessmentId == model.AssessmentId).Select(x => x.Sort)
                   .OrderByDescending(x => x).FirstOrDefault() + 1;
                entity.Note = "";
                entity.CutOffScoresChanged = false;
            }
            else
            {
                entity = _adeContract.GetMeasure(model.ID);
                model.IsDeleted = entity.IsDeleted;
                model.UpdatedBy = entity.UpdatedBy;
                model.CreatedBy = entity.CreatedBy;
                model.AssessmentId = entity.AssessmentId;
                model.Sort = entity.Sort;

                //record first relatedMeasureId
                isEdit = true;
                oldRelatedMeasureId = entity.RelatedMeasureId;
                oldParentId = entity.ParentId;
                entity.RelatedMeasureId = model.RelatedMeasureId;
                entity.Note = model.Note;
            }
            bool oldCutoffScoreChanged = entity.CutOffScoresChanged;
            entity.Status = model.Status;
            if (entity.Status == EntityStatus.Inactive)
            {
                // todo:更新父级或子级,暂时不做
            }

            entity.ParentId = model.ParentId;
            entity.Label = model.Label;
            entity.Name = model.Name;
            entity.ShortName = model.ShortName;
            if (string.IsNullOrEmpty(entity.ShortName) && !string.IsNullOrEmpty(entity.Name))
                entity.ShortName = string.Join("",
                    entity.Name.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Substring(0, 1)));

            entity.OrderType = model.OrderType;
            entity.ItemType = model.ItemType;
            entity.TotalScored = model.TotalScored;
            entity.InnerTime = model.InnerTime;
            entity.Timeout = model.Timeout;
            entity.ApplyToWave = model.ApplyToWaveValues;
            entity.PreviousButton = model.PreviousButton;
            entity.StopButton = model.StopButton;
            entity.NextButton = model.NextButton;
            entity.LightColor = model.LightColor;
            entity.HasCutOffScores = model.HasCutOffScores;
            entity.BOYHasCutOffScores = model.BOYHasCutOffScores;
            entity.MOYHasCutOffScores = model.MOYHasCutOffScores;
            entity.EOYHasCutOffScores = model.EOYHasCutOffScores;
            entity.GroupByParentMeasure = model.GroupByParentMeasure;
            entity.ShowLaunchPage = model.ShowLaunchPage;
            entity.ShowFinalizePage = model.ShowFinalizePage;
            entity.Description = model.Description;
            if (model.StartPageHtml != null)
                entity.StartPageHtml = model.StartPageHtml;
            if (model.EndPageHtml != null)
                entity.EndPageHtml = model.EndPageHtml;
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = model.UpdatedBy;
            entity.PercentileRank = model.PercentileRank;
            entity.GroupByLabel = model.GroupByLabel;

            //check CutOffScore
            entity.CutOffScoresChanged = false;
            if (isEdit)
            {

                List<CutOffScoreEntity> OldCutOffScores = GetCutOffScores<MeasureEntity>(entity.ID);
                List<CutOffScoreEntity> NewCutOffScores = model.CutOffScores;
                if (OldCutOffScores.Count != NewCutOffScores.Count)
                    entity.CutOffScoresChanged = true;
                else
                {
                    bool isExist = true;
                    foreach (CutOffScoreEntity newModel in NewCutOffScores)
                    {
                        isExist = OldCutOffScores.Exists(r => r.FromYear == newModel.FromYear && r.FromMonth == newModel.FromMonth
                            && r.ToYear == newModel.ToYear && r.ToMonth == newModel.ToMonth
                            && r.Wave == newModel.Wave && r.BenchmarkId == newModel.BenchmarkId
                            && r.LowerScore == newModel.LowerScore && r.HigherScore == newModel.HigherScore
                            && r.ShowOnGroup == newModel.ShowOnGroup);
                        if (isExist == false)
                        {
                            entity.CutOffScoresChanged = true;
                            break;
                        }
                    }
                }
                if (entity.CutOffScoresChanged == false)
                {
                    entity.CutOffScoresChanged = oldCutoffScoreChanged;
                }
            }

            result = model.ID < 1 ?
                _adeContract.InsertMeasure(entity)
                : _adeContract.UpdateMeasure(entity);

            model.ID = entity.ID;

            //delete old relation
            if (isEdit)
            {
                ///更新 relation
                if (oldRelatedMeasureId > 0 && oldRelatedMeasureId != model.RelatedMeasureId)
                    UpdateRelatedMeasure(oldRelatedMeasureId, 0);
            }

            //insert new relation 
            if (oldRelatedMeasureId != model.RelatedMeasureId && model.RelatedMeasureId > 0)
            {
                UpdateRelatedMeasure(model.RelatedMeasureId, model.ID);
            }

            model.CreatedOn = entity.CreatedOn;
            model.UpdatedOn = entity.UpdatedOn;
            model.Status = entity.Status;
            var userName = UserBusiness.GetUsername(entity.CreatedBy);
            model.CreatedByName = userName != null ? userName.ShowName : "";
            if (model.CutOffScores.Count > 0 && (!isEdit || entity.CutOffScoresChanged))
            {
                if (result.ResultType == OperationResultType.Success)
                {
                    result = _adeContract.DeleteCutOffScore<MeasureEntity>(model.ID);
                }
                if (result.ResultType == OperationResultType.Success)
                {
                    model.CutOffScores.ForEach(x => x.HostId = model.ID);
                    result = _adeContract.InsertCutOffScore<MeasureEntity>(model.CutOffScores);
                }
            }
            if (result.ResultType == OperationResultType.Success)
            {
                CpallsBusiness.UpdateMeasureCache(model);
            }
            _adeContract.RecalculateTotalScore();

            return result;
        }

        public OperationResult SaveMeasureNote(MeasureModel model)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            MeasureEntity entity = null;
            entity = _adeContract.GetMeasure(model.ID);
            entity.Note = model.Note;

            result = _adeContract.UpdateMeasure(entity);


            return result;
        }

        /// <summary>
        /// 更新Measure中的RelatedMeasureId字段
        /// </summary>
        /// <param name="measureId">MeasureId</param>
        /// <param name="relatedMeasureId">关联的MeasureId</param>
        private void UpdateRelatedMeasure(int measureId, int relatedMeasureId)
        {
            MeasureEntity relatedMeasure = _adeContract.GetMeasure(measureId);
            relatedMeasure.RelatedMeasureId = relatedMeasureId;
            _adeContract.UpdateMeasure(relatedMeasure);
        }

        public MeasureModel GetMeasureModel(int id)
        {
            var model = _adeContract.Measures.Select(SelectorMeasureEntityToModel).FirstOrDefault(x => x.ID == id);
            if (model != null)
            {
                model.Parent = _adeContract.Measures.Select(SelectorMeasureEntityToModel).FirstOrDefault(x => x.ID == model.ParentId);
                model.CutOffScores = this.GetCutOffScores<MeasureEntity>(id);
                model.Links = this.GetLinks<MeasureEntity>(id);
            }
            return model;
        }

        public int GetParentMeasureId(int measureId)
        {
            MeasureEntity measure = _adeContract.Measures.FirstOrDefault(e => e.ID == measureId && e.ParentId > 1);
            return measure?.ParentId ?? 0;
        }

        /// <summary>
        /// 获取关联Measure的Name
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="relatedMeasureId"></param>
        /// <returns></returns>
        public string GetRelatedMeasureName(int assessmentId, int relatedMeasureId)
        {
            AssessmentEntity otherAssessmentEntity = GetTheOtherLanguageAssessment(assessmentId);
            if (otherAssessmentEntity != null)
            {
                var model = otherAssessmentEntity.Measures.FirstOrDefault(o => o.ID == relatedMeasureId);
                if (model != null)
                {
                    return model.Name;
                }
            }
            return null;
        }

        public OperationResult DeleteMeasure(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            var entity = _adeContract.GetMeasure(id);
            if (entity == null)
            {
                return result;
            }
            if (_adeContract.Measures.Any(x => x.ID == id && x.Assessment.Locked))
            {
                return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
            }
            if (_adeContract.Items.Count(x => x.MeasureId == id && x.IsDeleted == false) > 0)
            {
                result.Message =
                    ResourceHelper.GetRM().GetInformation("Ade_CannotDeleteMeasure");
                result.ResultType = OperationResultType.ParamError;
                return result;
            }
            entity.Status = EntityStatus.Inactive;
            entity.IsDeleted = true;
            result = _adeContract.UpdateMeasure(entity);
            _adeContract.RecalculateTotalScore();

            return result;
        }

        public OperationResult AdjustMeasures(int assessmentId, List<int> measureIds)
        {
            var result = _adeContract.AdjustMeasureOrders(measureIds);
            return result;
        }

        public ItemBaseEntity NewItemEntity(ItemType type)
        {
            return _adeContract.NewItemBaseEntity(type);
        }

        public OperationResult CopyTxkeaExpressive(ItemModel model, TxkeaExpressiveItemModel copyModel)
        {
            OperationResult result = null;
            if (_adeContract.Measures.Any(x => x.ID == model.MeasureId && x.Assessment.Locked))
            {
                return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
            }
            if (_adeContract.Items.Any(x => x.ID != model.ID && x.Label == model.Label && x.IsDeleted == false))
            {
                result = new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_SameItemLabel"));
                return result;
            }
            if (_adeContract.Items.Any(x => x.ID == model.ID && x.Measure.Assessment.Locked))
            {
                result = new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
                return result;
            }
            ItemBaseEntity entity = _adeContract.NewItemBaseEntity(model.Type);
            ((TxkeaExpressiveItemModel)model).Copy((TxkeaExpressiveItemEntity)entity, copyModel);
            entity.Sort = _adeContract.Items.Where(x => x.MeasureId == model.MeasureId).Select(x => x.Sort)
                   .OrderByDescending(x => x).FirstOrDefault() + 1;
            result = _adeContract.InsertItem(entity);
            ((TxkeaExpressiveItemModel)model).UpdateModel((TxkeaExpressiveItemEntity)entity);
            _adeContract.RecalculateTotalScore();
            return result;
        }

        public OperationResult SaveItem(ItemModel model)
        {
            OperationResult result = null;
            if (_adeContract.Measures.Any(x => x.ID == model.MeasureId && x.Assessment.Locked))
            {
                return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
            }
            if (_adeContract.Items.Any(x => x.ID != model.ID && x.Label == model.Label && x.IsDeleted == false))
            {
                result = new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_SameItemLabel"));
                return result;
            }
            if (_adeContract.Items.Any(x => x.ID == model.ID && x.Measure.Assessment.Locked))
            {
                result = new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
                return result;
            }
            ItemBaseEntity entity = model.ID < 1 ?
                _adeContract.NewItemBaseEntity(model.Type) :
                _adeContract.GetItemBase(model.ID);

            model.UpdateEntity(entity);

            if (entity.ID < 1)
            {
                entity.Sort = _adeContract.Items.Where(x => x.MeasureId == model.MeasureId).Select(x => x.Sort)
                    .OrderByDescending(x => x).FirstOrDefault() + 1;
            }
            var userName = UserBusiness.GetUsername(entity.CreatedBy);
            model.CreatedByName = userName != null ? userName.ShowName : "";
            if (model.ID < 1)
            {
                foreach (AnswerEntity item in model.Answers)
                {
                    if (item != null)  //model.Answers实例化时，为List<null>
                    {
                        item.ID = 0;
                        entity.Answers.Add(item);
                    }
                }
                result = _adeContract.InsertItem(entity);
            }
            else
            {
                if (model.Answers != null && model.Answers.Any(x => x != null))
                {
                    var answers = entity.Answers.ToList();
                    List<AnswerEntity> removedAnswers = new List<AnswerEntity>();
                    entity.Answers.ForEach(answer =>
                    {
                        var editedAnswer = model.Answers.Find(a => a.ID == answer.ID);
                        if (editedAnswer != null)
                        {
                            answer.Audio = editedAnswer.Audio;
                            answer.AudioTime = editedAnswer.AudioTime;
                            answer.IsCorrect = editedAnswer.IsCorrect;
                            answer.Maps = editedAnswer.Maps;
                            answer.Picture = editedAnswer.Picture;
                            answer.PictureTime = editedAnswer.PictureTime;
                            answer.Score = editedAnswer.Score;
                            answer.Text = editedAnswer.Text;
                            answer.TextTime = editedAnswer.TextTime;
                            answer.Value = editedAnswer.Value;
                            answer.UpdatedOn = DateTime.Now;
                            answer.ImageType = editedAnswer.ImageType;
                            answer.SequenceNumber = editedAnswer.SequenceNumber;
                            answer.ResponseAudio = editedAnswer.ResponseAudio;
                        }
                        else if (entity.Type == ItemType.TxkeaReceptive)  //TxkeaReceptive类型的Answer数量不固定，可减少
                        {
                            removedAnswers.Add(answer);
                        }
                    });
                    model.Answers.ForEach(answer =>
                    {
                        if (answer.ID < 1)
                        {
                            entity.Answers.Add(answer);
                        }
                    });

                    if (removedAnswers.Count > 0)
                        _adeContract.DeleteAnswers(removedAnswers, false);
                }

                result = _adeContract.UpdateItem(entity);
            }
            if (entity.Type == ItemType.TxkeaExpressive)
                ((TxkeaExpressiveItemModel)model).UpdateModel((TxkeaExpressiveItemEntity)entity);
            if (entity.Type == ItemType.TxkeaReceptive)
                ((TxkeaReceptiveItemModel)model).UpdateModel((TxkeaReceptiveItemEntity)entity);

            _adeContract.RecalculateTotalScore();

            return result;
        }

        public List<ItemListModel> SearchItem(Expression<Func<ItemBaseEntity, bool>> condition, out int total,
            string sort = "Sort", string order = "asc", int first = 0, int count = 10)
        {
            var query = _adeContract.Items.AsExpandable().Where(x => x.IsDeleted == false && x.ID > 0).OfType<ItemBaseEntity>()
                .Where(condition).OrderBy(sort, order).Select(SelectorEntityToItemListModel);
            total = query.Count();
            var result = query.Skip(first).Take(count).ToList();
            var users = UserBusiness.GetUsernames(result.Select(x => x.CreatedBy));
            result.ForEach(x =>
            {
                x.IsShown = false;
                if (users.Find(y => y.ID == x.CreatedBy) != null)
                    x.CreatedByName = users.Find(y => y.ID == x.CreatedBy).ShowName;
                if (x.Type == ItemType.ObservableChoice)
                {
                    ObservableChoiceModel find1 = (ObservableChoiceModel)GetItemModel(x.ID);
                    x.IsShown = find1.IsShown;
                }
                if (x.Type == ItemType.ObservableResponse)
                {
                    ObservableEntryModel find2 = (ObservableEntryModel)GetItemModel(x.ID);
                    x.IsShown = find2.IsShown;
                }
            });
            return result;
        }

        private static Expression<Func<ItemBaseEntity, ItemListModel>> SelectorEntityToItemListModel
        {
            get
            {
                return x => new ItemListModel()
                {
                    ID = x.ID,
                    MeasureId = x.MeasureId,
                    Label = x.Label,
                    Type = x.Type,
                    Description = x.Description,
                    CreatedBy = x.CreatedBy,
                    UpdatedOn = x.UpdatedOn,
                    Status = x.Status,
                    Locked = x.Measure.Assessment.Locked,
                    BranchingScoresLength = x.Status == EntityStatus.Inactive ? 0 : x.BranchingScores.Where(r => r.IsDeleted == false).Count()
                };
            }
        }


        public OperationResult AdjustItems(int assessmentId, int measureId, List<int> itemIds)
        {
            var result = _adeContract.AdjustItemsOrders(itemIds);
            return result;
        }

        public OperationResult DeleteItem(int id)
        {
            var entity = _adeContract.GetItemBase(id);
            if (entity == null)
                return new OperationResult(OperationResultType.Success);
            if (_adeContract.Items.Any(x => x.ID == id && x.Measure.Assessment.Locked))
            {
                return new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_Assessment_Locked"));
            }
            entity.IsDeleted = true;
            entity.Status = EntityStatus.Inactive;
            var result = _adeContract.UpdateItem(entity);
            _adeContract.RecalculateTotalScore();
            return result;
        }

        public ItemListModel GetItemListModel(int id)
        {
            var model = _adeContract.Items.Select(SelectorEntityToItemListModel).FirstOrDefault(x => x.ID == id);
            if (model != null)
            {
                model.Links = this.GetLinks<ItemBaseEntity>(id);
            }
            return model;
        }

        public ItemBaseEntity GetItem(int id)
        {
            return _adeContract.GetItemBase(id);
        }

        public ItemModel GetItemModel(int id)
        {
            var item = _adeContract.GetItemBase(id);
            if (item == null) return null;
            var model = ItemFactory.GetItemModel(item);
            model.Answers = item.Answers.OrderBy(r => r.ID).ToList();
            model.Measure = GetMeasureModel(item.MeasureId);
            return model;
        }

        /// <summary>
        /// 获取同一个Measure 下的，排序在指定序号之后的 item
        /// </summary>
        /// <param name="index"></param>
        /// <param name="measureId"></param>
        /// <returns></returns>
        public List<SelectItemModel> GetSkipItems(int index, int measureId)
        {
            return _adeContract.Items.Where(r => r.Sort > index && r.MeasureId == measureId && r.IsDeleted == false && r.Status == EntityStatus.Active)
                  .Select(r => new SelectItemModel()
                  {
                      ID = r.ID,
                      Name = r.Label
                  }).ToList();
        }

        public IList<TxkeaReceptiveItemEntity> GetTxkeaReceptiveItemsForPlayMeasure(List<int> measureIds)
        {
            return _adeContract.GetTxkeaReceptiveItemsForPlayMeasure(measureIds);
        }
        public IEnumerable<ItemModel> GetItemModels(Expression<Func<ItemBaseEntity, bool>> condition)
        {
            var query = _adeContract.Items.Where(x => x.IsDeleted == false && x.ID > 0)
                .Where(condition).OrderByDescending(x => x.IsPractice).ThenBy(x => x.Sort).ToList();
            var listModels = new List<ItemModel>();
            foreach (var entity in query)
            {
                var model = ItemFactory.GetItemModel(entity);
                listModels.Add(model);
            }
            // query.ForEach(x => listModels.Add(ItemFactory.GetItemModel(x)));
            return listModels;
        }

        public IEnumerable<ItemModel> GetItemModels(List<int> measureIds)
        {
            var query = _adeContract.GetItems(measureIds).OrderByDescending(x => x.IsPractice).ThenBy(x => x.Sort).ToList();
            var listModels = new List<ItemModel>();
            foreach (var entity in query)
            {
                var model = ItemFactory.GetItemModel(entity);
                listModels.Add(model);
            }
            return listModels;
        }


        /// <summary>
        /// 获取Cpalls Header
        /// </summary>
        /// <returns></returns>
        public CpallsHeaderModel GetCpallsHeader(int assessmentId, Wave wave)
        {
            var w = ((int)wave).ToString();
            List<CpallsHeaderModel> list = _adeContract.Assessments.Where(r => r.ID == assessmentId && r.Status == EntityStatus.Active).Select(
                r => new CpallsHeaderModel
                {
                    ID = r.ID,
                    Name = r.Name,
                    Measures = r.Measures.Where(m =>
                        m.Status == EntityStatus.Active
                        && m.IsDeleted == false
                        && m.ApplyToWave.Contains(w))
                    .Select(m => new MeasureHeaderModel()
                    {
                        ID = 0,
                        MeasureId = m.ID,
                        Name = m.Name,
                        ParentId = m.ParentId,
                        ParentMeasureName = m.Parent.Name,
                        Subs = m.SubMeasures.Count(s => s.IsDeleted == false && s.Status == EntityStatus.Active && s.ApplyToWave.Contains(w)),
                        Sort = m.Sort
                    })
                }
                ).OrderByDescending(r => r.ID).Take(1).ToList();
            if (list.Count == 0)
                return null;
            return list[0];
        }

        public void ClearCahce(int assessmentId)
        {
            var keys = new List<string>()
            {
                string.Format("Assessment_{0}_Measure_Tree", assessmentId),
                string.Format("Exec_Assessment_{0}", assessmentId),
                string.Format("Assessment_{0}_Wave_{1}", assessmentId, Wave.BOY),
                string.Format("Assessment_{0}_Wave_{1}", assessmentId, Wave.MOY),
                string.Format("Assessment_{0}_Wave_{1}", assessmentId, Wave.EOY),
                string.Format("Assessment_{0}_Wave_All", assessmentId),
                string.Format("__COT_ASSESSMENT_{0}_",assessmentId)
            };
            keys.ForEach(CacheHelper.Remove);
        }
        public OperationResult ExecuteSql(string sql)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (!string.IsNullOrEmpty(sql))
            {
                result = _adeContract.ExecuteSql(sql);
            }
            else
            {
                result.ResultType = OperationResultType.Error;
            }
            return result;
        }

        #region WaveLog

        public OperationResult InsertWaveLog(Wave wave, int userId, int assessmentId)
        {
            WaveLogEntity waveLogEntity = new WaveLogEntity();
            waveLogEntity.UserId = userId;
            waveLogEntity.WaveValue = wave;
            waveLogEntity.CreatedOn = DateTime.Now;
            waveLogEntity.UpdatedOn = DateTime.Now;
            waveLogEntity.AssessmentId = assessmentId;
            return _adeContract.InsertWaveLog(waveLogEntity);
        }
        public OperationResult SaveWaveLog(Wave wave, int userId, int assessmentId)
        {
            var findItem = _adeContract.GetUserWavelog(userId, assessmentId);
            if (findItem == null)
            {
                return InsertWaveLog(wave, userId, assessmentId);
            }
            else
            {
                findItem.WaveValue = wave;
                findItem.UpdatedOn = DateTime.Now;
                return _adeContract.UpdateWaveLog(findItem);
            }
        }
        public WaveLogEntity GetUserWaveLog(int userId, int assessmentId)
        {
            return _adeContract.GetUserWavelog(userId, assessmentId);
        }
        #endregion

        /// <summary>
        /// Get Measures which cutoffscores has changed.
        /// </summary>
        /// <returns></returns>
        /// Author : Steven
        /// Date   : 27/06/2016
        public List<MeasureEntity> GetCutOffChangedMeasures()
        {
            var measureList = _adeContract.Measures.Where(m => m.CutOffScoresChanged == true);
            return measureList.ToList();
        }
        public List<MeasureEntity> GetCutOffChangedMeasures(int measureId)
        {
            var measureList = _adeContract.Measures.Where(m => m.CutOffScoresChanged == true && m.ID == measureId);
            return measureList.ToList();
        }
        /// <summary>
        /// Update Measure
        /// </summary>
        /// <returns></returns>
        /// Author : Steven
        /// Date   : 27/06/2016
        public OperationResult UpdateMeasure(MeasureEntity measure)
        {
            return _adeContract.UpdateMeasure(measure);
        }

        public int UpdateCutOffScoresChanged(int measureId, bool cutoffScoresChanged)
        {
            return _adeContract.UpdateCutOffScoresChanged(measureId, cutoffScoresChanged);
        }

        #region Benchmark Entity

        public List<BenchmarkEntity> GetBenchmarks(int assessmentId)
        {
            return _adeContract.Benchmarks.Where(x => x.AssessmentId == assessmentId)
                .OrderBy(x => x.CreatedOn).ToList();
        }

        public List<BenchmarkEntity> GetBenchmarksForSelect(int assessmentId)
        {
            return _adeContract.Benchmarks.Where(x => x.AssessmentId == assessmentId && x.LabelText != "" && x.BlackWhite != 0)
                .OrderBy(x => x.CreatedOn).ToList();
        }

        public BenchmarkEntity GetBenchmark(int benchmarkId)
        {
            return _adeContract.Benchmarks.FirstOrDefault(x => x.ID == benchmarkId);
        }

        public IEnumerable<BenchmarkModel> GetIEnumBenchmarks(int assessmentId)
        {
            IEnumerable<BenchmarkModel> benchmarks = _adeContract.Benchmarks
                .Where(x => x.AssessmentId == assessmentId && x.LabelText != "")
                .Select(x => new BenchmarkModel
                {
                    ID = x.ID,
                    AssessmentId = x.AssessmentId,
                    LabelText = x.LabelText,
                    Color = x.Color,
                    BlackWhite = x.BlackWhite
                });
            return benchmarks;
        }

        public Dictionary<int, BenchmarkModel> GetDicBenchmarks(int assessmentId)
        {
            Dictionary<int, BenchmarkModel> dicBenchmarks = new Dictionary<int, BenchmarkModel>();
            List<BenchmarkModel> benchmarks = _adeContract.Benchmarks
                .Where(x => x.AssessmentId == assessmentId)
                .Select(x => new BenchmarkModel
                {
                    ID = x.ID,
                    AssessmentId = x.AssessmentId,
                    LabelText = x.LabelText,
                    Color = x.Color,
                    BlackWhite = x.BlackWhite
                }).ToList();
            benchmarks.ForEach(x => dicBenchmarks.Add(x.ID, x));
            return dicBenchmarks;
        }
        #endregion

        #region Assessment Report

        public List<AssessmentReportEntity> GetAssessmentReports(int assessmentId, ReportTypeEnum reportType = 0)
        {
            return _adeContract.AssessmentReports.Where(e => e.AssessmentId == assessmentId &&
                                                             (e.ReportType == reportType || reportType == 0)).ToList();
        }

        public OperationResult InsertAssessmentReports(List<AssessmentReportEntity> assessmentReports)
        {
            return _adeContract.InsertAssessmentReports(assessmentReports);
        }

        public OperationResult DeleteAssessmentReports(List<AssessmentReportEntity> assessmentReports)
        {
            return _adeContract.DeleteAssessmentReports(assessmentReports);
        }
        #endregion

        #region Assessment Report

        public AssessmentLegendEntity GetAssessmentLegend(int legendId)
        {
            return _adeContract.AssessmentLegends.FirstOrDefault(e => e.ID == legendId);
        }

        public AssessmentLegendEntity GetAssessmentLegend(int assessmentId, LegendTypeEnum legendType = 0)
        {
            return _adeContract.AssessmentLegends.FirstOrDefault(e => e.AssessmentId == assessmentId &&
                                                                      (e.LegendType == legendType || legendType == 0));
        }

        public OperationResult InsertAssessmentLegends(List<AssessmentLegendEntity> assessmentLegends)
        {
            return _adeContract.InsertAssessmentLegends(assessmentLegends);
        }

        public OperationResult InsertAssessmentLegend(AssessmentLegendEntity assessmentLegend)
        {
            return _adeContract.InsertAssessmentLegend(assessmentLegend);
        }

        public OperationResult DeleteAssessmentLegends(List<AssessmentLegendEntity> assessmentLegends)
        {
            return _adeContract.DeleteAssessmentLegends(assessmentLegends);
        }

        public OperationResult UpdateAssessmentLegend(AssessmentLegendEntity assessmentLegend)
        {
            return _adeContract.UpdateAssessmentLegend(assessmentLegend);
        }

        #endregion


    }
}
