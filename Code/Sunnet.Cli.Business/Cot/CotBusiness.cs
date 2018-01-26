/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/12/17 9:15:39
 * Description:		Please input class summary
 * Version History:	Created,2014/12/17 9:15:39
 * 
 * 
 **************************************************************************/

using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cot.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using Sunnet.Framework.Resources;
using Sunnet.Framework.Core.Extensions;

namespace Sunnet.Cli.Business.Cot
{
    public class CotBusiness
    {
        private ICotContract _cotContract;
        private readonly IAdeDataContract _adeData;
        private AdeBusiness _adeBusiness;
        private UserBusiness _userBusiness;

        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }

        public CotBusiness(AdeUnitOfWorkContext unit = null)
        {
            _cotContract = DomainFacade.CreateCotContract(unit);
            _adeData = DomainFacade.CreateAdeDataService(unit);
            _userBusiness = new UserBusiness();
        }
        private static object _synchronize = new object();

        private readonly Func<CotTeacherModel, CotSchoolTeacherModel> cotTeacherSelector = x => new CotSchoolTeacherModel()
        {
            AssessmentID = x.AssessmentID,
            CLIFundingId = x.CLIFundingId,
            CoachFirstName = x.CoachFirstName,
            CoachId = x.CoachId,
            CoachLastName = x.CoachLastName,
            CotWave1 = x.CotWave1,
            CotWave2 = x.CotWave2,
            CotWaveStatus1 = x.CotWaveStatus1,
            CotWaveStatus2 = x.CotWaveStatus2,
            FirstName = x.FirstName,
            ID = x.ID,
            LastName = x.LastName,
            Records = x.Records,
            SchoolYear = x.SchoolYear,
            Status = x.Status,
            TeacherID = x.TeacherID,
            TeacherType = x.TeacherType,
            UserID = x.UserID,
            YearsInProject = x.YearsInProject,
            YearsInProjectId = x.YearsInProjectId,
        };

        public CotAssessmentModel GetAssessmentFromCache(int assessmentId)
        {
            var key = string.Format("__COT_ASSESSMENT_{0}_", assessmentId);
            var assessment = CacheHelper.Get<CotAssessmentModel>(key);
            if (assessment == null)
            {
                lock (_synchronize)
                {
                    assessment = CacheHelper.Get<CotAssessmentModel>(key);
                    if (assessment == null)
                    {
                        assessment = _adeData.Assessments.Where(x => x.ID == assessmentId).Select(x => new CotAssessmentModel()
                        {
                            AssessmentId = x.ID,
                            Name = x.Name,
                            CreatedOn = x.CreatedOn,
                            Measures = x.Measures.Where(m => m.ParentId == 1).OrderBy(m => m.Sort).Select(m => new CotMeasureModel()
                            {
                                MeasureId = m.ID,
                                Name = m.Name,
                                Items = m.Items.Where(item => item.Status == EntityStatus.Active && item.IsDeleted == false && item.Type == ItemType.Cot)
                                                .OrderBy(item => item.Sort).OfType<CotItemEntity>().Select(item => new CotItemModel()
                                                {
                                                    ID = 0,
                                                    ItemId = item.ID,
                                                    Level = item.Level,
                                                    CircleManual = item.CircleManual,
                                                    CotAssessmentId = 0,
                                                    FullTargetText = item.FullTargetText,
                                                    MentoringGuide = item.MentoringGuide,
                                                    PrekindergartenGuidelines = item.PrekindergartenGuidelines,
                                                    ShortTargetText = item.ShortTargetText,
                                                    CotItemId = item.CotItemId
                                                }),
                                Children = m.SubMeasures.Where(child => child.Status == EntityStatus.Active && child.IsDeleted == false)
                                .OrderBy(child => child.Sort).Select(child => new CotChildMeasureModel()
                                {
                                    MeasureId = child.ID,
                                    Name = child.Name,
                                    Items = child.Items.Where(item => item.Status == EntityStatus.Active && item.IsDeleted == false && item.Type == ItemType.Cot)
                                                .OrderBy(item => item.Sort).OfType<CotItemEntity>().Select(item => new CotItemModel()
                                                {
                                                    ID = 0,
                                                    ItemId = item.ID,
                                                    Level = item.Level,
                                                    CircleManual = item.CircleManual,
                                                    CotAssessmentId = 0,
                                                    FullTargetText = item.FullTargetText,
                                                    MentoringGuide = item.MentoringGuide,
                                                    PrekindergartenGuidelines = item.PrekindergartenGuidelines,
                                                    ShortTargetText = item.ShortTargetText,
                                                    CotItemId = item.CotItemId
                                                })
                                })
                            })
                        }).FirstOrDefault();
                        if (assessment != null)
                        {
                            var itemIds = assessment.Items.Select(x => x.ItemId).ToList();
                            var links = AdeBusiness.GetLinkModels<ItemBaseEntity>(itemIds.ToArray());
                            if (links != null)
                            {
                                assessment.Items.ForEach(item =>
                                {
                                    item.Links = links.Where(x => x.HostId == item.ItemId).ToList();
                                });
                            }

                            var measureIds = new List<int>();
                            assessment.Measures.ForEach(mea =>
                            {
                                measureIds.Add(mea.MeasureId);
                                if (mea.Children != null && mea.Children.Any())
                                    measureIds.AddRange(mea.Children.Select(x => x.MeasureId));
                            });
                            links = AdeBusiness.GetLinkModels<MeasureEntity>(measureIds.ToArray());
                            assessment.Measures.ForEach(mea =>
                            {
                                mea.Links = links.Where(x => x.HostId == mea.MeasureId).ToList();
                                if (mea.Children != null && mea.Children.Any())
                                {
                                    mea.Children.ForEach(child => child.Links = links.Where(x => x.HostId == child.MeasureId).ToList());
                                }
                            });
                        }
                        CacheHelper.Add(key, assessment);
                    }
                }
            }
            var clone = assessment.Clone();
            return clone;
        }

        #region Create Entity
        public CotAssessmentEntity NewCotAssessmentEntity()
        {
            return _cotContract.NewCotAssessmentEntity();
        }
        public CotWaveEntity NewCotWaveEntity()
        {
            return _cotContract.NewCotWaveEntity();
        }

        public CotAssessmentItemEntity NewCotAssessmentItemEntity()
        {
            return _cotContract.NewCotAssessmentItemEntity();
        }

        public CotStgReportEntity NewCotStgReportEntity()
        {
            return _cotContract.NewCotStgReportEntity();
        }
        #endregion

        public List<CotWaveEntity> GetWaves(int assessmentId, int year, int teacherId)
        {
            var schoolYear = year.ToSchoolYearString();
            var existed = _cotContract.Assessments.FirstOrDefault(x =>
                x.AssessmentId == assessmentId
                && x.TeacherId == teacherId
                && x.SchoolYear == schoolYear);
            if (existed == null)
            {
                return new List<CotWaveEntity>();
            }
            return existed.Waves.ToList();

        }

        public CotWaveEntity GetLastCotWave(int assessmentId, int year, int teacherId)
        {
            var schoolYear = year.ToSchoolYearString();
            CotWaveEntity lastCotWave = _cotContract.Waves.OrderByDescending(x => x.CreatedOn).FirstOrDefault(x =>
                x.Assessment.AssessmentId == assessmentId
                && x.Assessment.TeacherId == teacherId
                && x.Assessment.SchoolYear == schoolYear);
            return lastCotWave;
        }

        public List<CotWaveEntity> GetLastCotWaves(int assessmentId, int year, List<int> teacherIds)
        {
            var schoolYear = year.ToSchoolYearString();
            List<CotWaveEntity> cotWaves = _cotContract.Waves.Where(x =>
                x.Assessment.AssessmentId == assessmentId
                && teacherIds.Contains(x.Assessment.TeacherId)
                && x.Assessment.SchoolYear == schoolYear).ToList();
            if (cotWaves == null)
            {
                return new List<CotWaveEntity>();
            }
            List<CotWaveEntity> lastTeacherWaves = new List<CotWaveEntity>();
            foreach (int teacherId in teacherIds)
            {
                List<CotWaveEntity> teacherWaves = cotWaves.Where(x => x.Assessment.TeacherId == teacherId).ToList();
                if (teacherWaves.Count == 0)
                    continue;
                lastTeacherWaves.Add(teacherWaves.OrderByDescending(x => x.CreatedOn).FirstOrDefault());
            }
            return lastTeacherWaves;
        }

        public List<CotStgReportModel> GetReports(int assessmentId, int year, int teacherId)
        {
            var schoolYear = year.ToSchoolYearString();
            var existed = _cotContract.Assessments.FirstOrDefault(x =>
                x.AssessmentId == assessmentId
                && x.TeacherId == teacherId
                && x.SchoolYear == schoolYear);
            if (existed == null)
            {
                return new List<CotStgReportModel>();
            }
            return existed.Reports.Where(x => x.Status != CotStgReportStatus.Deleted).OrderByDescending(x => x.CreatedOn).Select(x => new CotStgReportModel()
            {
                ID = x.ID,
                AdditionalComments = x.AdditionalComments,
                CotAssessmentId = x.CotAssessmentId,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                GoalMetDate = x.GoalMetDate,
                GoalSetDate = x.GoalSetDate,
                OnMyOwn = x.OnMyOwn,
                SpentTime = x.SpentTime,
                Status = x.Status,
                UpdatedBy = x.UpdatedBy,
                UpdatedOn = x.UpdatedOn,
                WithSupport = x.WithSupport,
                ShowFullText = x.ShowFullText
            }).OrderByDescending(x => x.CreatedOn).ToList();
        }

        public CotStgReportEntity GetLastReport(int assessmentId, int year, int teacherId)
        {
            var schoolYear = year.ToSchoolYearString();
            CotStgReportEntity lastReportEntity =
                _cotContract.Reports.Where(x => x.Status != CotStgReportStatus.Deleted).OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefault(
                        x => x.Assessment.AssessmentId == assessmentId && x.Assessment.TeacherId == teacherId &&
                             x.Assessment.SchoolYear == schoolYear);
            return lastReportEntity;
        }
        public CotStgReportEntity GetLastReport(int assessmentId, string schoolYear, int teacherId)
        {
            CotStgReportEntity lastReportEntity =
                _cotContract.Reports.Where(x => x.Status != CotStgReportStatus.Deleted).OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefault(
                        x => x.Assessment.AssessmentId == assessmentId && x.Assessment.TeacherId == teacherId &&
                             x.Assessment.SchoolYear == schoolYear);
            return lastReportEntity;
        }

        public List<CotStgReportEntity> GetLastReports(int assessmentId, int year, List<int> teacherIds)
        {
            var schoolYear = year.ToSchoolYearString();
            List<CotStgReportEntity> allTeacherStgReports =
                _cotContract.Reports.Where(x =>
                x.Status != CotStgReportStatus.Deleted
                && x.Assessment.AssessmentId == assessmentId
                && teacherIds.Contains(x.Assessment.TeacherId)
                && x.Assessment.SchoolYear == schoolYear).ToList();
            List<CotStgReportEntity> lastStgReports = new List<CotStgReportEntity>();
            foreach (int teacherId in teacherIds)
            {
                List<CotStgReportEntity> teacherStgReports = allTeacherStgReports.Where(x => x.Assessment.TeacherId == teacherId).ToList();
                if (teacherStgReports.Count == 0)
                    continue;
                lastStgReports.Add(teacherStgReports.OrderByDescending(x => x.CreatedOn).FirstOrDefault());
            }
            return lastStgReports;
        }

        public CotTeacherStatus GetTeacherStatus(int assessmentId, int year, int teacherId)
        {
            var status = new CotTeacherStatus();
            var schoolYear = year.ToSchoolYearString();
            var existed = _cotContract.Assessments.FirstOrDefault(x =>
                   x.AssessmentId == assessmentId
                   && x.TeacherId == teacherId
                   && x.SchoolYear == schoolYear);
            if (year == CommonAgent.Year)
            {
                if (existed == null)
                {
                    status.HasWavesToDo = true;
                    return status;
                }
                status.LastStgReportId = existed.Reports.Where(x => x.Status != CotStgReportStatus.Deleted).OrderByDescending(x => x.CreatedOn).Select(x => x.ID).FirstOrDefault();
                var completed = existed.Waves.Count(x => x.Status > CotWaveStatus.Saved);
                status.HasCotReport = completed > 0;
                status.CotPdfVerisonVisible = completed > 0;
                status.HasWavesToDo = completed < 2;
                status.HasOldData = existed.Waves.Count(x => x.Status == CotWaveStatus.OldData) > 0;
            }
            else
            {
                if (existed != null)
                {
                    var completed = existed.Waves.Count(x => x.Status > CotWaveStatus.Saved);
                    status.CotPdfVerisonVisible = completed > 0;
                    status.StgReportReadOnly = true;
                    status.HasCotReport = completed > 0;
                }
            }
            return status;
        }

        /// <summary>
        /// 做Assessment时，从草稿表[CotAssessmentWaveItem]中获取相关的记录，填充Model
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="year">The year.</param>
        /// <param name="teacherId">The teacher identifier.</param>
        /// <param name="wave">The wave.</param>
        /// <returns></returns>
        public CotAssessmentModel GetAssessmentForWave(int assessmentId, int year, int teacherId, CotWave wave)
        {
            var schoolYear = year.ToSchoolYearString();
            CotAssessmentModel assessment = GetAssessmentFromCache(assessmentId);
            CotAssessmentEntity existed = _cotContract.Assessments.FirstOrDefault(x =>
                x.AssessmentId == assessmentId
                && x.TeacherId == teacherId
                && x.SchoolYear == schoolYear);
            if (existed != null)
            {
                assessment.ID = existed.ID;
                assessment.CreatedBy = existed.CreatedBy;
                assessment.CreatedOn = existed.CreatedOn;
                assessment.SchoolYear = existed.SchoolYear;
                assessment.Status = existed.Status;
                assessment.TeacherId = existed.TeacherId;
                assessment.UpdatedBy = existed.UpdatedBy;
                assessment.UpdatedOn = existed.UpdatedOn;

                assessment.Teacher = _userBusiness.GetTeacherModel(assessment.TeacherId);

                List<CotAssessmentWaveItemEntity> existedItems = existed.WaveItems.Where(x => x.Wave == wave).ToList();
                List<CotItemModel> itemModels = assessment.Items.ToList();
                existedItems.ForEach(existedItem =>
                {
                    var item = itemModels.Find(x => x.ItemId == existedItem.ItemId);
                    if (item != null)
                    {
                        item.BoyObsDate = existedItem.BoyObsDate;
                        item.CotAssessmentId = existedItem.CotAssessmentId;
                        item.CotUpdatedOn = existedItem.CotUpdatedOn;
                        item.CreatedBy = existedItem.CreatedBy;
                        item.CreatedOn = existedItem.CreatedOn;
                        item.GoalMetDate = existedItem.GoalMetDate;
                        item.GoalSetDate = existedItem.GoalSetDate;
                        item.MoyObsDate = existedItem.MoyObsDate;
                        item.NeedSupport = existedItem.NeedSupport;
                        item.UpdatedBy = existedItem.UpdatedBy;
                        item.CreatedOn = existedItem.CreatedOn;
                    }
                });
            }
            else
            {
                assessment.SchoolYear = schoolYear;
                assessment.Status = CotAssessmentStatus.Initialised;
            }
            return assessment;
        }

        public CotAssessmentModel GetAssessment(int assessmentId, int year, int teacherId)
        {
            var schoolYear = year.ToSchoolYearString();
            CotAssessmentModel assessment = GetAssessmentFromCache(assessmentId);
            CotAssessmentEntity existed = _cotContract.Assessments.FirstOrDefault(x =>
                x.AssessmentId == assessmentId
                && x.TeacherId == teacherId
                && x.SchoolYear == schoolYear);

            if (existed != null)
            {
                assessment.ID = existed.ID;
                assessment.CreatedBy = existed.CreatedBy;
                assessment.CreatedOn = existed.CreatedOn;
                assessment.SchoolYear = existed.SchoolYear;
                assessment.Status = existed.Status;
                assessment.TeacherId = existed.TeacherId;
                assessment.UpdatedBy = existed.UpdatedBy;
                assessment.UpdatedOn = existed.UpdatedOn;

                assessment.Teacher = _userBusiness.GetTeacherModel(assessment.TeacherId);

                List<CotAssessmentItemEntity> existedItems = existed.Items.ToList();
                List<CotItemModel> itemModels = assessment.Items.ToList();
                existedItems.ForEach(existedItem =>
                {
                    var item = itemModels.Find(x => x.ItemId == existedItem.ItemId);
                    if (item != null)
                    {
                        item.BoyObsDate = existedItem.BoyObsDate;
                        item.CotAssessmentId = existedItem.CotAssessmentId;
                        item.CotUpdatedOn = existedItem.CotUpdatedOn;
                        item.CreatedBy = existedItem.CreatedBy;
                        item.CreatedOn = existedItem.CreatedOn;
                        item.GoalMetDate = existedItem.GoalMetDate;
                        item.GoalSetDate = existedItem.GoalSetDate;
                        item.MoyObsDate = existedItem.MoyObsDate;
                        item.WaitingGoalMet = existedItem.WaitingGoalMet;
                        item.NeedSupport = existedItem.NeedSupport;
                        item.UpdatedBy = existedItem.UpdatedBy;
                        item.CreatedOn = existedItem.CreatedOn;
                    }
                });

            }
            else
            {
                assessment.SchoolYear = schoolYear;
                assessment.Status = CotAssessmentStatus.Initialised;
            }

            return assessment;
        }

        public CotAssessmentModel GetAssessmentModelForStg(int reportId)
        {
            CotStgReportEntity reportEntity = _cotContract.GetCotStgReportEntity(reportId);
            if (reportEntity == null || reportEntity.Assessment == null)
                return null;
            CotAssessmentEntity existed = reportEntity.Assessment;
            CotAssessmentModel assessment = GetAssessmentFromCache(existed.AssessmentId);
            assessment.Report = new CotStgReportModel(reportEntity);
            assessment.Report.Groups = GetStgGroupByReportId(reportId);
            assessment.ID = existed.ID;
            assessment.CreatedBy = existed.CreatedBy;
            assessment.CreatedOn = existed.CreatedOn;
            assessment.SchoolYear = existed.SchoolYear;
            assessment.Status = existed.Status;
            assessment.TeacherId = existed.TeacherId;
            assessment.UpdatedBy = existed.UpdatedBy;
            assessment.UpdatedOn = existed.UpdatedOn;

            assessment.Teacher = _userBusiness.GetTeacherModel(assessment.TeacherId);

            List<CotAssessmentItemEntity> items = reportEntity.ReportItems.OrderBy(x => x.Sort).Select(x => x.Item).ToList();
            var cotItemIds = items.Select(x => x.ItemId).ToList();
            assessment.Measures.ForEach(mea =>
            {
                var adeItems = mea.Items;
                var matchedItems = new List<CotItemModel>();
                matchedItems.AddRange(adeItems.Where(x => cotItemIds.Contains(x.ItemId)).ToList());
                mea.Items = matchedItems;
                if (mea.Children != null)
                {
                    mea.Children.ForEach(child =>
                    {
                        adeItems = child.Items;
                        matchedItems = new List<CotItemModel>();
                        matchedItems.AddRange(adeItems.Where(x => cotItemIds.Contains(x.ItemId)).ToList());
                        child.Items = matchedItems;
                    });
                }
            });
            var sort = 0;
            List<CotItemModel> itemModels = assessment.Items.ToList();
            items.ForEach(existedItem =>
            {
                var item = itemModels.Find(x => x.ItemId == existedItem.ItemId);
                if (item != null)
                {
                    item.ID = existedItem.ID;
                    item.BoyObsDate = existedItem.BoyObsDate;
                    item.CotAssessmentId = existedItem.CotAssessmentId;
                    item.CotUpdatedOn = existedItem.CotUpdatedOn;
                    item.CreatedBy = existedItem.CreatedBy;
                    item.CreatedOn = existedItem.CreatedOn;
                    item.GoalMetDate = existedItem.GoalMetDate;
                    item.GoalSetDate = existedItem.GoalSetDate;
                    item.MoyObsDate = existedItem.MoyObsDate;
                    item.NeedSupport = existedItem.NeedSupport;
                    item.UpdatedBy = existedItem.UpdatedBy;
                    item.CreatedOn = existedItem.CreatedOn;
                    item.Sort = sort++;
                    item.GoalMetAble = assessment.Report.Status == CotStgReportStatus.Saved &&
                                       item.GoalMetDate == CommonAgent.MinDate;
                }
            });
            assessment.Measures.ForEach(mea =>
            {
                if (mea.Children != null && mea.Children.Any())
                {
                    mea.Children.ForEach(child =>
                    {
                        if (child.Items != null && child.Items.Any())
                            child.Sort = child.Items.Min(x => x.Sort);
                    });
                    mea.Sort = mea.Children.Min(x => x.Sort);
                }
                else
                {
                    if (mea.Items != null && mea.Items.Any())
                        mea.Sort = mea.Items.Min(x => x.Sort);
                }
            });
            return assessment;
        }

        public OperationResult SaveCotItems(CotWaveEntity wave, List<CotAssessmentItemEntity> items, bool createStgReport = false)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            CotAssessmentEntity assessmentEntity = _cotContract.GetCotAssessmentEntity(wave.CotAssessmentId);


            if (assessmentEntity == null)
            {
                result.Message = "Error: assessment is null.";
                return result;
            }
            List<CotAssessmentItemEntity> itemsExisted = assessmentEntity.Items.ToList();

            List<CotStgReportItemEntity> cotStgReportItemList = new List<CotStgReportItemEntity>();
            if (createStgReport == false)
            {
                List<int> tmpItemIds = items.Select(r => r.ItemId).ToList();
                List<int> editItemIds = itemsExisted.Where(r => tmpItemIds.Contains(r.ItemId)).Select(r => r.ID).ToList();

                if (editItemIds.Count > 0)
                {
                    assessmentEntity.Reports.Where(r => r.Status != CotStgReportStatus.Deleted).ForEach(r =>
                    {
                        var v = r.ReportItems.Where(i => i.GoalMetDate == CommonAgent.MinDate && editItemIds.Contains(i.ItemId));
                        int testv = v.Count();
                        cotStgReportItemList.AddRange(v);
                    });
                }
            }

            items.ForEach(item =>
            {
                var savedItem = itemsExisted.Find(x => x.ItemId == item.ItemId);
                if (savedItem == null)
                {
                    savedItem = NewCotAssessmentItemEntity();
                    savedItem.ItemId = item.ItemId;
                    savedItem.CotAssessmentId = assessmentEntity.ID;
                    savedItem.NeedSupport = item.NeedSupport;
                    savedItem.CotUpdatedOn = item.CotUpdatedOn;

                    savedItem.BoyObsDate = CommonAgent.MinDate;
                    savedItem.MoyObsDate = CommonAgent.MinDate;
                    savedItem.GoalMetDate = CommonAgent.MinDate;
                    savedItem.GoalSetDate = CommonAgent.MinDate;

                    savedItem.CreatedBy = wave.UpdatedBy;
                    savedItem.CreatedOn = DateTime.Now;

                    assessmentEntity.Items.Add(savedItem);
                }
                else
                {
                    savedItem.NeedSupport = item.NeedSupport;
                    savedItem.CotUpdatedOn = item.CotUpdatedOn;
                    savedItem.GoalMetDate = item.GoalMetDate.EnsureMinDate();
                    if (savedItem.GoalMetDate > CommonAgent.MinDate)
                    {
                        savedItem.WaitingGoalMet = false;
                    }

                    if (cotStgReportItemList != null)
                    {
                        cotStgReportItemList.FindAll(r => r.ItemId == savedItem.ID).ForEach(r => { r.GoalMetDate = savedItem.GoalMetDate; });
                    }
                }
                savedItem.UpdatedBy = wave.UpdatedBy;
                savedItem.UpdatedOn = DateTime.Now;
            });
            result = _cotContract.UpdateCotAssessmentEntity(assessmentEntity);



            if (createStgReport && result.ResultType == OperationResultType.Success)
            {
                var existedReports =
                    assessmentEntity.Reports.Where(
                        x => x.Status == CotStgReportStatus.Initialised || x.Status == CotStgReportStatus.Saved).ToList();
                existedReports.ForEach(x =>
                {
                    x.Status = CotStgReportStatus.Completed;
                    x.UpdatedOn = DateTime.Now;
                    x.UpdatedBy = wave.UpdatedBy;
                });

                var report = _cotContract.NewCotStgReportEntity();
                report.AdditionalComments = string.Empty;
                report.CotAssessmentId = assessmentEntity.ID;
                report.CreatedBy = wave.CreatedBy;
                report.CreatedOn = DateTime.Now;
                report.GoalMetDate = CommonAgent.MinDate;
                report.GoalSetDate = DateTime.Now;
                report.OnMyOwn = string.Empty;
                report.SpentTime = string.Empty;
                report.UpdatedBy = wave.CreatedBy;
                report.UpdatedOn = DateTime.Now;
                report.WithSupport = string.Empty;

                var itemIds = items.Select(x => x.ItemId).ToList();
                var itemForReport = assessmentEntity.Items.Where(x => itemIds.Contains(x.ItemId)).ToList();

                var sort = 0;
                itemForReport.ForEach(x =>
                {
                    x.WaitingGoalMet = true;
                    x.GoalMetDate = CommonAgent.MinDate;
                    x.GoalSetDate = DateTime.Now;
                    report.ReportItems.Add(new CotStgReportItemEntity()
                    {
                        ItemId = x.ID,
                        Sort = sort++,
                        GoalMetDate = CommonAgent.MinDate
                    });
                });
                assessmentEntity.Reports.Add(report);

                result = _cotContract.UpdateCotAssessmentEntity(assessmentEntity);
                if (result.ResultType == OperationResultType.Success)
                {
                    result.AppendData = report.ID;
                }
            }
            return result;
        }

        public OperationResult SaveAssessment(CotWaveEntity waveEntity, List<CotAssessmentItemEntity> items, bool isFinalize)
        {
            if (waveEntity == null)
                return new OperationResult(OperationResultType.Error, "Arguments error.");
            var successAction = "none";
            var assessmentEntity = waveEntity.Assessment;
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (assessmentEntity.ID > 0)
            {
                assessmentEntity = _cotContract.GetCotAssessmentEntity(assessmentEntity.ID);
                assessmentEntity.UpdatedBy = assessmentEntity.UpdatedBy;
                assessmentEntity.UpdatedOn = assessmentEntity.UpdatedOn;
            }
            else
            {
                result = _cotContract.InsertCotAssessmentEntity(assessmentEntity);
            }
            if (result.ResultType == OperationResultType.Success)
            {
                var wave = assessmentEntity.Waves.FirstOrDefault(x => x.Wave == waveEntity.Wave);
                if (wave == null)
                {
                    successAction = "refresh";
                    wave = NewCotWaveEntity();
                    wave.CreatedBy = waveEntity.CreatedBy;
                    wave.CreatedOn = waveEntity.CreatedOn;
                    wave.FinalizedOn = CommonAgent.MinDate;
                    assessmentEntity.Waves.Add(wave);
                }

                wave.CotAssessmentId = assessmentEntity.ID;
                wave.SpentTime = waveEntity.SpentTime;
                wave.Status = CotWaveStatus.Saved;
                wave.UpdatedBy = waveEntity.UpdatedBy;
                wave.UpdatedOn = waveEntity.UpdatedOn;
                wave.VisitDate = waveEntity.VisitDate;
                wave.Wave = waveEntity.Wave;

                if (isFinalize)
                {
                    successAction = "gotoTeacher";
                    wave.Status = CotWaveStatus.Finalized;
                    wave.FinalizedOn = DateTime.Now;
                    UpdateAssessmentItem(items, assessmentEntity, wave);
                }
                else
                {
                    UpdateAssessmentWaveItem(items, assessmentEntity, wave);
                }
                result = _cotContract.UpdateCotAssessmentEntity(assessmentEntity);
            }
            result.AppendData = successAction;
            return result;
        }

        private void UpdateAssessmentItem(List<CotAssessmentItemEntity> items, CotAssessmentEntity assessmentEntity, CotWaveEntity wave)
        {
            var savedItems = assessmentEntity.Items.ToList();
            savedItems.ForEach(x =>
            {
                if (wave.Wave == CotWave.BOY && x.MoyObsDate == CommonAgent.MinDate && x.NeedSupport == false)
                {
                    x.WaitingGoalMet = false;
                }
                if (wave.Wave == CotWave.MOY && x.BoyObsDate == CommonAgent.MinDate && x.NeedSupport == false)
                {
                    x.WaitingGoalMet = false;
                }
            });
            items.ForEach(item =>
            {
                var savedItem = savedItems.Find(x => x.ItemId == item.ItemId);
                if (savedItem == null)
                {
                    savedItem = NewCotAssessmentItemEntity();
                    savedItem.ItemId = item.ItemId;
                    savedItem.CotAssessmentId = assessmentEntity.ID;
                    savedItem.NeedSupport = item.NeedSupport;
                    if (wave.Wave == CotWave.BOY)
                        savedItem.BoyObsDate = wave.VisitDate;
                    else
                        savedItem.BoyObsDate = CommonAgent.MinDate;
                    if (wave.Wave == CotWave.MOY)
                        savedItem.MoyObsDate = wave.VisitDate;
                    else
                        savedItem.MoyObsDate = CommonAgent.MinDate;
                    savedItem.GoalMetDate = CommonAgent.MinDate;
                    savedItem.GoalSetDate = CommonAgent.MinDate;
                    savedItem.CotUpdatedOn = CommonAgent.MinDate;

                    savedItem.CreatedBy = wave.UpdatedBy;
                    savedItem.CreatedOn = DateTime.Now;

                    assessmentEntity.Items.Add(savedItem);
                }
                else
                {
                    savedItem.NeedSupport = item.NeedSupport;
                    if (wave.Wave == CotWave.BOY)
                        savedItem.BoyObsDate = wave.VisitDate;
                    if (wave.Wave == CotWave.MOY)
                        savedItem.MoyObsDate = wave.VisitDate;
                }
                savedItem.UpdatedBy = wave.UpdatedBy;
                savedItem.UpdatedOn = DateTime.Now;
            });
        }


        private void UpdateAssessmentWaveItem(List<CotAssessmentItemEntity> items, CotAssessmentEntity assessmentEntity, CotWaveEntity wave)
        {
            var w = wave.Wave;
            var savedItems = assessmentEntity.WaveItems.Where(x => x.Wave == w).ToList();
            items.ForEach(item =>
            {
                var savedItem = savedItems.Find(x => x.ItemId == item.ItemId);
                if (savedItem == null)
                {
                    savedItem = _cotContract.NewCotAssessmentWaveItemEntity();

                    savedItem.Wave = wave.Wave;
                    savedItem.ItemId = item.ItemId;
                    savedItem.CotAssessmentId = assessmentEntity.ID;
                    savedItem.NeedSupport = item.NeedSupport;
                    if (wave.Wave == CotWave.BOY)
                        savedItem.BoyObsDate = wave.VisitDate;
                    else
                        savedItem.BoyObsDate = CommonAgent.MinDate;
                    if (wave.Wave == CotWave.MOY)
                        savedItem.MoyObsDate = wave.VisitDate;
                    else
                        savedItem.MoyObsDate = CommonAgent.MinDate;
                    savedItem.GoalMetDate = CommonAgent.MinDate;
                    savedItem.GoalSetDate = CommonAgent.MinDate;
                    savedItem.CotUpdatedOn = CommonAgent.MinDate;

                    savedItem.CreatedBy = wave.UpdatedBy;
                    savedItem.CreatedOn = DateTime.Now;

                    assessmentEntity.WaveItems.Add(savedItem);
                }
                else
                {
                    savedItem.NeedSupport = item.NeedSupport;
                    if (wave.Wave == CotWave.BOY)
                        savedItem.BoyObsDate = wave.VisitDate;
                    if (wave.Wave == CotWave.MOY)
                        savedItem.MoyObsDate = wave.VisitDate;
                }
                savedItem.UpdatedBy = wave.UpdatedBy;
                savedItem.UpdatedOn = DateTime.Now;
            });
        }

        public OperationResult SaveStgReport(CotStgReportEntity reportEntity, List<int> items)
        {
            var result = new OperationResult(OperationResultType.Success);
            if (reportEntity == null || reportEntity.ID < 1)
            {
                result.Message = "Error: report is null.";
                return result;
            }
            CotStgReportEntity existedEntity = _cotContract.GetCotStgReportEntity(reportEntity.ID);
            if (existedEntity == null)
            {
                result.Message = "Error: report is null.";
                return result;
            }

            var existedItems = existedEntity.ReportItems.Select(x => x.Item).ToList();
            if (existedEntity.Status != CotStgReportStatus.Deleted)
            {
                existedEntity.GoalSetDate = reportEntity.GoalSetDate;
                existedEntity.SpentTime = reportEntity.SpentTime;
                existedEntity.OnMyOwn = reportEntity.OnMyOwn;
                existedEntity.WithSupport = reportEntity.WithSupport;
                existedEntity.AdditionalComments = reportEntity.AdditionalComments;
                existedEntity.Status = CotStgReportStatus.Saved;
                existedEntity.ShowFullText = reportEntity.ShowFullText;
                existedItems.ForEach(x =>
                {
                    //if (x.GoalSetDate > CommonAgent.MinDate == false)//Steven 06/30/2016 ticket 2235
                    //{
                    x.GoalSetDate = reportEntity.GoalSetDate;
                    //}
                    x.WaitingGoalMet = true;
                });
            }
            result = _cotContract.UpdateCotStgReportEntity(existedEntity);
            return result;
        }

        public OperationResult SaveStgReportItemOrders(int id, IEnumerable<int> itemOrders)
        {
            var report = _cotContract.GetCotStgReportEntity(id);
            if (report == null)
                return new OperationResult(OperationResultType.Error, "STG Report is null.");
            var itemIds = itemOrders.ToList();
            var items = report.ReportItems.ToList();
            for (int i = 0; i < itemIds.Count; i++)
            {
                var existedItem = items.Find(x => x.ItemId == itemIds[i]);
                if (existedItem != null)
                {
                    existedItem.Sort = i;
                }
            }
            return _cotContract.UpdateCotStgReportEntity(report);
        }

        public List<int> GetExistedTeachers(int assessmentId, int year)
        {
            var schoolYear = year.ToSchoolYearString();
            return
                _cotContract.Assessments.Where(x => x.AssessmentId == assessmentId && x.SchoolYear == schoolYear)
                    .Select(x => x.TeacherId)
                    .ToList();
        }

        public List<CotSchoolTeacherModel> GetTeachers(int assessmentId, int year, int? coachId, List<int> communities, List<int> schoolIds,
            string firstname, string lastname, string teacherId, bool searchExistingCot,
            string sort, string order, int first, int count, out int total)
        {
            List<CotTeacherModel> teachers =
                _cotContract.GetTeachers(assessmentId, year.ToSchoolYearString(), coachId, communities, schoolIds, firstname, lastname, teacherId,
                searchExistingCot, sort, order, first, count, out total);

            if (teachers != null)
            {
                var teacherUserIds = teachers.Select(x => x.UserID).ToArray();
                var userModels = _userBusiness.GetUserModels(teacherUserIds);

                var teacherWithSch = teachers.Select(cotTeacherSelector).ToList();

                teacherWithSch.ForEach(teacher =>
                {
                    if (userModels.ContainsKey(teacher.UserID))
                    {
                        teacher.Schools = userModels[teacher.UserID].SchoolNames;
                        teacher.Communities = userModels[teacher.UserID].CommunityNames;
                    }
                });

                return teacherWithSch;
            }
            return new List<CotSchoolTeacherModel>();
        }

        public OperationResult ResetStgReport(int assessmentId, int teacherId, int year, UserBaseEntity user)
        {
            var result = new OperationResult(OperationResultType.Success);
            if (year != CommonAgent.Year)
            {
                result.Message = ResourceHelper.GetRM().GetInformation("Can_Not_Reset_Year");
                return result;
            }
            var schoolYear = year.ToSchoolYearString();
            CotStgReportEntity lastReportEntity =
                _cotContract.Reports.Where(x => x.Status != CotStgReportStatus.Deleted).OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefault(
                        x => x.Assessment.AssessmentId == assessmentId && x.Assessment.TeacherId == teacherId &&
                             x.Assessment.SchoolYear == schoolYear);

            if (lastReportEntity != null)
            {
                List<int> updatedItemId = lastReportEntity.ReportItems.Select(r => r.ItemId).ToList();

                List<CotStgReportEntity> reportList = _cotContract.Reports.Where(r => r.Status != CotStgReportStatus.Deleted && r.ID != lastReportEntity.ID
                    && r.Assessment.AssessmentId == assessmentId && r.Assessment.TeacherId == teacherId && r.Assessment.SchoolYear == schoolYear
                    && r.ReportItems.Any(item => updatedItemId.Contains(item.ItemId))).OrderByDescending(r => r.CreatedOn).ToList();


                var items = lastReportEntity.ReportItems.Select(x => x.Item).ToList();
                items.ForEach(item =>
                {
                    item.WaitingGoalMet = false;
                    item.GoalMetDate = CommonAgent.MinDate;
                    item.GoalSetDate = CommonAgent.MinDate;
                    foreach (var reportModel in reportList)
                    {
                        var itemModel = reportModel.ReportItems.Where(r => r.ItemId == item.ID).FirstOrDefault();
                        if (itemModel != null)
                        {
                            item.GoalSetDate = reportModel.GoalSetDate;
                            item.GoalMetDate = itemModel.GoalMetDate;
                            if (item.GoalSetDate > CommonAgent.MinDate && item.GoalMetDate == CommonAgent.MinDate)
                                item.WaitingGoalMet = true;
                            break;
                        }
                    }
                });
                lastReportEntity.Status = CotStgReportStatus.Deleted;
                lastReportEntity.UpdatedBy = user.ID;
                lastReportEntity.UpdatedOn = DateTime.Now;
                result = _cotContract.UpdateCotStgReportEntity(lastReportEntity);
            }
            else
            {
                result.Message = ResourceHelper.GetRM().GetInformation("No_STG_Report");
            }
            return result;
        }

        public OperationResult ResetCot(int assessmentId, int teacherId, int year, CotWave wave, UserBaseEntity user)
        {
            var result = new OperationResult(OperationResultType.Success);
            if (year != CommonAgent.Year)
            {
                result.Message = ResourceHelper.GetRM().GetInformation("Can_Not_Reset_Year");
                return result;
            }
            var schoolYear = year.ToSchoolYearString();
            var assessmentEntity =
                _cotContract.Assessments.FirstOrDefault(x => x.AssessmentId == assessmentId && x.TeacherId == teacherId &&
                                                             x.SchoolYear == schoolYear);
            if (assessmentEntity != null)
            {
                var waveEntity = assessmentEntity.Waves.FirstOrDefault(x => x.Wave == wave);
                if (waveEntity != null)
                {
                    var items = assessmentEntity.Items.ToList();
                    var tmpItems = assessmentEntity.WaveItems.Where(x => x.Wave == wave).ToList();
                    if (wave == CotWave.BOY)
                    {
                        items.Where(x => x.BoyObsDate > CommonAgent.MinDate).ForEach(x => x.NeedSupport = false);
                        items.ForEach(x => x.BoyObsDate = CommonAgent.MinDate);
                        tmpItems.Where(x => x.BoyObsDate > CommonAgent.MinDate).ForEach(x => x.NeedSupport = false);
                        tmpItems.ForEach(x => x.BoyObsDate = CommonAgent.MinDate);
                    }
                    else if (wave == CotWave.MOY)
                    {
                        items.Where(x => x.MoyObsDate > CommonAgent.MinDate).ForEach(x => x.NeedSupport = false);
                        items.ForEach(x => x.MoyObsDate = CommonAgent.MinDate);

                        tmpItems.Where(x => x.MoyObsDate > CommonAgent.MinDate).ForEach(x => x.NeedSupport = false);
                        tmpItems.ForEach(x => x.MoyObsDate = CommonAgent.MinDate);
                    }
                    waveEntity.Status = CotWaveStatus.Initialised;
                    waveEntity.SpentTime = "0.00";
                    waveEntity.UpdatedOn = DateTime.Now;
                    waveEntity.UpdatedBy = user.ID;
                    waveEntity.FinalizedOn = CommonAgent.MinDate;
                    waveEntity.VisitDate = CommonAgent.MinDate;

                    result = _cotContract.UpdateCotAssessmentEntity(assessmentEntity);
                }
                else
                {
                    result.Message = ResourceHelper.GetRM().GetInformation("No_COT_Wave");
                }
            }
            else
            {
                result.Message = ResourceHelper.GetRM().GetInformation("No_Cot_Assessment");
            }
            return result;
        }

        #region Stg Group
        public OperationResult SaveStgGroup(int stgReportId, string groupName, IEnumerable<int> groupItemIds)
        {
            var stgGroup = _cotContract.NewCotStgGroupEntity();
            stgGroup.CotStgReportId = stgReportId;
            stgGroup.GroupName = groupName;
            stgGroup.OnMyOwn = "";
            stgGroup.WithSupport = "";
            stgGroup.IsDelete = false;
            var itemIds = groupItemIds.ToList();
            for (int i = 0; i < itemIds.Count; i++)
            {
                CotStgGroupItemEntity stgGroupItem = _cotContract.NewCotStgGroupItemEntity();
                stgGroupItem.ItemId = itemIds[i];
                stgGroupItem.Sort = i;
                stgGroup.CotStgGroupItems.Add(stgGroupItem);
            }
            return _cotContract.InsertCotStgGroupEntity(stgGroup);
        }

        public List<CotStgGroupModel> GetStgGroupByReportId(int stgReportId)
        {
            List<CotStgGroupItemModel> stgGroupItemModels = _cotContract.GetCotStgGroupItems(stgReportId);
            List<CotStgGroupModel> cotStgGroupModels = _cotContract.CotStgGroups.Where(g => g.IsDelete == false && g.CotStgReportId == stgReportId)
                .Select(g => new CotStgGroupModel
                {
                    ID = g.ID,
                    CotStgReportId = g.CotStgReportId,
                    GroupName = g.GroupName,
                    OnMyOwn = g.OnMyOwn,
                    WithSupport = g.WithSupport
                }).OrderByDescending(g => g.ID).ToList();
            foreach (var model in cotStgGroupModels)
            {
                model.StgGroupItems = stgGroupItemModels.Where(e => e.CotStgGroupId == model.ID).OrderBy(e => e.Sort).ToList();
            }
            return cotStgGroupModels;
        }

        public OperationResult DelStgGroup(int stgGroupId)
        {
            CotStgGroupEntity entity = _cotContract.GetCotStgGroupEntity(stgGroupId);
            entity.IsDelete = true;
            return _cotContract.UpdateCotStgGroupEntity(entity);
        }

        public OperationResult UpdateStgGroup(int stgGroupId, int[] groupItemIds, string onMyOwn, string withSupport)
        {
            var stgGroup = _cotContract.GetCotStgGroupEntity(stgGroupId);
            stgGroup.OnMyOwn = onMyOwn;
            stgGroup.WithSupport = withSupport;
            stgGroup.UpdatedOn = DateTime.Now;
            var response = _cotContract.UpdateCotStgGroupEntity(stgGroup);
            if (response.ResultType == OperationResultType.Success && groupItemIds.Count() > 0)
            {
                response = _cotContract.DeleteCotStgGroupItems(stgGroup.CotStgGroupItems.ToList());
                List<CotStgGroupItemEntity> groupItems = new List<CotStgGroupItemEntity>();
                var itemIds = groupItemIds.ToList();
                for (int i = 0; i < itemIds.Count; i++)
                {
                    CotStgGroupItemEntity stgGroupItem = _cotContract.NewCotStgGroupItemEntity();
                    stgGroupItem.CotStgGroupId = stgGroup.ID;
                    stgGroupItem.ItemId = itemIds[i];
                    stgGroupItem.Sort = i;
                    groupItems.Add(stgGroupItem);
                }
                response = _cotContract.InsertCotStgGroupItems(groupItems);
            }
            return response;
        }

        public OperationResult DelStgGroupItem(int stgGroupId, int stgGroupItemId)
        {
            int id = _cotContract.CotStgGroupItems.Where(e => e.CotStgGroupId == stgGroupId && e.ItemId == stgGroupItemId).FirstOrDefault().ID;
            return _cotContract.DeleteCotStgGroupItem(id);
        }

        #endregion

        #region Offline

        public object GetOfflineTeachers(int assessmentId, List<CotSchoolTeacherModel> teachers)
        {
            if (teachers == null || !teachers.Any())
            {
                throw new Exception("Error: teachers is null.");
            }
            int year = CommonAgent.Year;
            var schoolYear = year.ToSchoolYearString();
            var teacherIds = teachers.Select(x => x.ID).ToList();
            var assessments =
                _cotContract.Assessments.Where(
                    x =>
                        x.AssessmentId == assessmentId && teacherIds.Contains(x.TeacherId) && x.SchoolYear == schoolYear)
                    .ToList();
            var assessment = GetAssessmentFromCache(assessmentId);
            assessment.SchoolYear = schoolYear;

            teachers.ForEach(teacher =>
            {
                teacher.SchoolYear = schoolYear;
                teacher.AssessmentID = assessmentId;
                var assessmentEntities = assessments.Find(x => x.TeacherId == teacher.ID);
                if (assessmentEntities != null)
                {
                    teacher.Records = new
                    {
                        Waves = assessmentEntities.Waves.Select(wave => new
                        {
                            wave.ID,
                            wave.Wave,
                            wave.SpentTime,
                            wave.Status,
                            wave.VisitDate,
                            wave.CotAssessmentId,
                            Items = assessmentEntities.WaveItems.Where(x => x.Wave == wave.Wave).Select(x => x.ItemId)
                        }),
                        Reports = assessmentEntities.Reports.Where(x => x.Status != CotStgReportStatus.Deleted).OrderByDescending(x => x.CreatedOn).Select(report => new
                        {
                            report.ID,
                            report.GoalSetDate,
                            report.GoalMetDate,
                            report.OnMyOwn,
                            report.WithSupport,
                            report.AdditionalComments,
                            report.CotAssessmentId,
                            report.Status,
                            report.SpentTime,
                            Items = report.ReportItems.Select(item => item.Item.ItemId),
                            report.CreatedOn,
                            UpdatedOn = report.UpdatedOn.ToString("MM/dd/yyyy HH:mm:ss"),
                            GroupGoals = report.CotStgGroups.Where(g => g.IsDelete == false).Select(g => new
                            {
                                g.ID,
                                g.CotStgReportId,
                                g.GroupName,
                                g.OnMyOwn,
                                g.WithSupport,
                                Items = g.CotStgGroupItems.Select(item => item.ItemId)
                            })
                        }),
                        TempItems = new
                        {
                            BOY = assessmentEntities.WaveItems.Where(x => x.Wave == CotWave.BOY).Select(item => new
                            {
                                item.ItemId,
                                item.BoyObsDate,
                                item.MoyObsDate,
                                item.NeedSupport
                            }),
                            MOY = assessmentEntities.WaveItems.Where(x => x.Wave == CotWave.MOY).Select(item => new
                            {
                                item.ItemId,
                                item.BoyObsDate,
                                item.MoyObsDate,
                                item.NeedSupport
                            })
                        },
                        Items = assessmentEntities.Items.Select(item => new
                        {
                            item.ID,
                            item.CotAssessmentId,
                            item.ItemId,
                            item.WaitingGoalMet,
                            GoalMetAble = false,
                            item.BoyObsDate,
                            item.MoyObsDate,
                            item.NeedSupport,
                            item.CotUpdatedOn,
                            item.GoalMetDate,
                            item.GoalSetDate,
                            GoalMetDone = item.GoalMetDate > CommonAgent.MinDate
                        })
                    };
                }
            });
            return new
            {
                assessment,
                teachers,
                schoolYear = year
            };
        }

        private DateTime GetMaxDate(params DateTime[] args)
        {
            var max = args.Max();
            if (max < CommonAgent.MinDate.AddDays(1)) max = DateTime.Now;
            if (max > DateTime.Now) max = DateTime.Now;
            return max;
        }

        private DateTime GetMinDate(params DateTime[] args)
        {
            var min = args.Min();
            if (min < CommonAgent.MinDate.AddDays(1)) min = DateTime.Now;
            return min;
        }

        public OperationResult Sync(int teacherId, int assessmentId, List<CotAssessmentItemEntity> items,
            List<CotAssessmentWaveItemEntity> tmpItems, List<CotWaveEntity> waves, List<CotStgReportEntity> reports, UserBaseEntity user)
        {
            var year = CommonAgent.Year;
            var schoolYear = year.ToSchoolYearString();
            var assessmentEntity =
                _cotContract.Assessments.FirstOrDefault(
                    x => x.AssessmentId == assessmentId && x.TeacherId == teacherId && x.SchoolYear == schoolYear);

            var dates = new List<DateTime>();
            dates.AddRange(items.Select(x => x.CreatedOn));
            dates.AddRange(items.Select(x => x.UpdatedOn));
            dates.AddRange(waves.Select(x => x.CreatedOn));
            dates.AddRange(waves.Select(x => x.UpdatedOn));
            dates.AddRange(reports.Select(x => x.CreatedOn));
            dates.AddRange(reports.Select(x => x.UpdatedOn));
            var createdOn = GetMinDate(dates.ToArray());
            var updatedOn = GetMaxDate(dates.ToArray());

            OperationResult result = null;
            if (assessmentEntity == null)
            {
                assessmentEntity = new CotAssessmentEntity()
                {
                    AssessmentId = assessmentId,
                    CreatedBy = user.ID,
                    CreatedOn = createdOn,
                    SchoolYear = schoolYear,
                    Status = CotAssessmentStatus.Initialised,
                    TeacherId = teacherId,
                    UpdatedBy = user.ID,
                    UpdatedOn = updatedOn
                };
                result = _cotContract.InsertCotAssessmentEntity(assessmentEntity);
                if (result.ResultType != OperationResultType.Success)
                    return result;
            }
            assessmentEntity.UpdatedOn = updatedOn;

            var existedItems = assessmentEntity.Items.ToList();
            items.ForEach(item =>
            {
                var existed = existedItems.Find(x => x.ItemId == item.ItemId);
                if (existed == null)
                {
                    existed = new CotAssessmentItemEntity()
                    {
                        CotAssessmentId = assessmentEntity.ID,
                        CreatedBy = user.ID,
                        CreatedOn = GetMaxDate(item.CreatedOn),
                        ItemId = item.ItemId,
                    };
                    assessmentEntity.Items.Add(existed);
                }
                existed.UpdatedBy = user.ID;
                existed.UpdatedOn = GetMaxDate(existed.UpdatedOn, item.UpdatedOn);

                existed.GoalMetDate = item.GoalMetDate.EnsureMinDate();
                if (existed.GoalSetDate > CommonAgent.MinDate == false)
                    existed.GoalSetDate = item.GoalSetDate.EnsureMinDate();
                existed.BoyObsDate = item.BoyObsDate.EnsureMinDate();
                existed.MoyObsDate = item.MoyObsDate.EnsureMinDate();
                existed.CotUpdatedOn = item.CotUpdatedOn.EnsureMinDate();
                existed.NeedSupport = item.NeedSupport;
                existed.WaitingGoalMet = item.WaitingGoalMet;
            });

            result = _cotContract.UpdateCotAssessmentEntity(assessmentEntity);
            if (result.ResultType != OperationResultType.Success)
                return result;

            var existedReports = assessmentEntity.Reports.ToList();
            reports.ForEach(report =>
            {
                var existed = existedReports.Find(x => x.ID == report.ID);
                if (existed == null)
                {
                    existed = new CotStgReportEntity()
                    {
                        CotAssessmentId = assessmentId,
                        CreatedBy = user.ID,
                        CreatedOn = GetMaxDate(report.CreatedOn),
                    };
                    assessmentEntity.Reports.Add(existed);
                }
                existed.UpdatedBy = user.ID;
                existed.UpdatedOn = GetMaxDate(existed.UpdatedOn, report.UpdatedOn);

                List<int> itemIds = report.ReportItems.Select(x => x.ItemId).ToList();
                existed.AdditionalComments = report.AdditionalComments;
                existed.GoalMetDate = report.GoalMetDate.EnsureMinDate();
                existed.GoalSetDate = report.GoalSetDate.EnsureMinDate();
                existed.OnMyOwn = report.OnMyOwn;
                existed.SpentTime = report.SpentTime;
                existed.WithSupport = report.WithSupport;
                existed.ReportItems =
                    assessmentEntity.Items.Where(x => itemIds.Contains(x.ItemId))
                        .ToList().Select(item => new CotStgReportItemEntity()
                        {
                            ItemId = item.ID,
                            GoalMetDate = item.GoalMetDate
                        }).ToList();
            });
            if (assessmentEntity.Reports.Any() && reports.Any())
            {
                assessmentEntity.Reports.ForEach(report => report.Status = CotStgReportStatus.Completed);
                assessmentEntity.Reports.OrderBy(x => x.CreatedOn).Last().Status = reports.Last().Status;
            }

            List<CotWaveEntity> existedWaves = assessmentEntity.Waves.ToList();
            waves.ForEach(wave =>
            {
                CotWaveEntity existed = existedWaves.Find(x => x.Wave == wave.Wave);
                if (existed == null)
                {
                    existed = new CotWaveEntity()
                    {
                        CotAssessmentId = assessmentId,
                        CreatedBy = user.ID,
                        CreatedOn = GetMaxDate(wave.CreatedOn),
                        FinalizedOn = CommonAgent.MinDate,
                        VisitDate = CommonAgent.MinDate,
                        Wave = wave.Wave,
                    };
                    assessmentEntity.Waves.Add(existed);
                }
                existed.UpdatedBy = user.ID;
                existed.UpdatedOn = GetMaxDate(existed.UpdatedOn, wave.UpdatedOn);

                existed.FinalizedOn = wave.FinalizedOn.EnsureMinDate();
                existed.SpentTime = wave.SpentTime;
                existed.Status = wave.Status;
                existed.VisitDate = wave.VisitDate;
            });

            List<CotAssessmentWaveItemEntity> existedTempItems = assessmentEntity.WaveItems.ToList();
            tmpItems.ForEach(item =>
            {
                CotAssessmentWaveItemEntity existed = existedTempItems.Find(x => x.ItemId == item.ItemId);
                if (existed == null)
                {
                    existed = new CotAssessmentWaveItemEntity()
                    {
                        CotAssessmentId = assessmentEntity.ID,
                        CreatedBy = user.ID,
                        CreatedOn = GetMaxDate(item.CreatedOn),
                        ItemId = item.ItemId,
                        Wave = item.Wave
                    };
                    assessmentEntity.WaveItems.Add(existed);
                }
                existed.UpdatedBy = user.ID;
                existed.UpdatedOn = GetMaxDate(item.UpdatedOn, existed.UpdatedOn);

                existed.GoalMetDate = item.GoalMetDate.EnsureMinDate();
                existed.GoalSetDate = item.GoalSetDate.EnsureMinDate();
                existed.BoyObsDate = item.BoyObsDate.EnsureMinDate();
                existed.MoyObsDate = item.MoyObsDate.EnsureMinDate();
                existed.CotUpdatedOn = item.CotUpdatedOn.EnsureMinDate();
                existed.NeedSupport = item.NeedSupport;
            });
            result = _cotContract.UpdateCotAssessmentEntity(assessmentEntity);

            return result;
        }
        #endregion




        #region VCW调用

        public List<CotStgReportModel> GetReports(int year, int teacherId)
        {
            string schoolYear = year.ToSchoolYearString();
            return _cotContract.Reports.Where(r => r.Assessment.TeacherId == teacherId
                && r.Assessment.SchoolYear == schoolYear && r.Status != CotStgReportStatus.Deleted)
                .Select(r => new CotStgReportModel()
                {
                    ID = r.ID,
                    CotAssessmentId = r.CotAssessmentId,
                    CreatedBy = r.CreatedBy,
                    CreatedOn = r.CreatedOn,
                    UpdatedBy = r.UpdatedBy,
                    UpdatedOn = r.UpdatedOn
                }).OrderByDescending(x => x.ID).ToList();
        }

        public List<CotStgReportModel> GetReports(int year, int teacherId, string sort, string order, int first, int count, out int total)
        {
            string schoolYear = year.ToSchoolYearString();
            var Reports = _cotContract.Reports.Where(r => r.Assessment.TeacherId == teacherId
                && r.Assessment.SchoolYear == schoolYear && r.Status != CotStgReportStatus.Deleted)
                .Select(r => new CotStgReportModel()
                {
                    ID = r.ID,
                    CotAssessmentId = r.CotAssessmentId,
                    CreatedBy = r.CreatedBy,
                    CreatedOn = r.CreatedOn,
                    UpdatedBy = r.UpdatedBy,
                    UpdatedOn = r.UpdatedOn
                });
            total = Reports.Count();
            return Reports.OrderBy(sort, order).Skip(first).Take(count).ToList();
        }

        public List<CotAssessmentModel> GetAssessments(int year, int teacherId)
        {
            string schoolYear = year.ToSchoolYearString();
            return _cotContract.Assessments.Where(r => r.TeacherId == teacherId
                && r.SchoolYear == schoolYear)
                .Select(r => new CotAssessmentModel()
                {
                    ID = r.ID,
                    AssessmentId = r.AssessmentId,
                    TeacherId = r.TeacherId,
                    CreatedOn = r.CreatedOn,
                    UpdatedOn = r.UpdatedOn,
                    SchoolYear = r.SchoolYear
                }).ToList();
        }

        #endregion
    }
}
