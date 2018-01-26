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

using System.Linq.Expressions;
using System.Runtime.InteropServices;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cot.Models;
using Sunnet.Cli.Business.Observable.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Cot.Models;
using Sunnet.Cli.Core.Observable;
using Sunnet.Cli.Core.Observable.Entities;
using Sunnet.Cli.Core.Observable.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Core.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using Sunnet.Framework.Resources;
using Sunnet.Framework.Core.Extensions;

namespace Sunnet.Cli.Business.Cot
{
    public class ObservableBusiness
    {
        private IObservableContract _observableService;
        private readonly IAdeDataContract _adeData;
        public ObservableBusiness(AdeUnitOfWorkContext unit = null)
        {
            _observableService = DomainFacade.CreateObservableAssessmentService(unit);
            _adeData = DomainFacade.CreateAdeDataService(unit);
        }
        private AdeBusiness _adeBusiness;
        private UserBusiness _userBusiness;

        private AdeBusiness AdeBusiness
        {
            get { return _adeBusiness ?? (_adeBusiness = new AdeBusiness()); }
            set { _adeBusiness = value; }
        }
        private static object _synchronize = new object();


        public ObservableAssessmentModel GetAssessmentFromCache(int assessmentId)
        {
            var key = string.Format("__OBSERVABLE_ASSESSMENT_{0}_", assessmentId);
            var assessment = CacheHelper.Get<ObservableAssessmentModel>(key);
            if (assessment == null)
            {
                lock (_synchronize)
                {
                    assessment = CacheHelper.Get<ObservableAssessmentModel>(key);
                    if (assessment == null)
                    {
                        assessment = _adeData.Assessments.Where(x => x.ID == assessmentId).Select(x => new ObservableAssessmentModel()
                        {
                            AssessmentId = x.ID,
                            Name = x.Name,
                            CreatedOn = x.CreatedOn,
                            Measures = x.Measures.Where(m => m.ParentId == 1).OrderBy(m => m.Sort).Select(m => new ObservableMeasureModel()
                            {
                                MeasureId = m.ID,
                                Name = m.Name,
                                Items = m.Items.Where(item => item.Status == EntityStatus.Active && item.IsDeleted == false && (item.Type == ItemType.ObservableChoice || item.Type == ItemType.ObservableResponse))
                                                .OrderBy(item => item.Sort).Select(item => new ObservableItemModel()
                                                {
                                                    ID = 0,
                                                    ItemId = item.ID,
                                                    Name = item.Description,
                                                    Answers = item.Answers,
                                                    AnswerType = item.AnswerType,
                                                    IsMultiChoice = false,// item.IsMultiChoice,
                                                    Type = item.Type
                                                }),

                                Children = m.SubMeasures.Where(child => child.Status == EntityStatus.Active && child.IsDeleted == false)
                                .OrderBy(child => child.Sort).Select(child => new ObservableChildMeasureModel()
                                {
                                    MeasureId = child.ID,
                                    Name = child.Name,
                                    Items = child.Items.Where(item => item.Status == EntityStatus.Active && item.IsDeleted == false && (item.Type == ItemType.ObservableChoice || item.Type == ItemType.ObservableResponse))
                                                .OrderBy(item => item.Sort).Select(item => new ObservableItemModel()
                                                {
                                                    ID = 0,
                                                    ItemId = item.ID,
                                                    Name = item.Description,
                                                    Answers = item.Answers,
                                                    AnswerType = item.AnswerType,
                                                    IsMultiChoice = false,//, item.IsMultiChoice,
                                                    Type = item.Type

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
                                    if (item.Type == ItemType.ObservableChoice)
                                    {
                                        var find = _adeBusiness.GetItemModel(item.ItemId) as ObservableChoiceModel;
                                        item.IsMultiChoice = find.IsMultiChoice;
                                        item.IsShown = find.IsShown;
                                        item.FullTargetText = find.TargetText;
                                    }
                                    else if (item.Type == ItemType.ObservableResponse)
                                    {
                                        var find = _adeBusiness.GetItemModel(item.ItemId) as ObservableEntryModel;
                                        item.IsShown = find.IsShown;
                                        item.FullTargetText = find.TargetText;
                                    }
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

                            var observItems = _adeBusiness.GetObserveChoiceItems(itemIds);
                            foreach (var observableMeasureModel in assessment.Measures)
                            {
                                foreach (var observableItemModel in observableMeasureModel.Items)
                                {
                                    var observItem = observItems.FirstOrDefault(e => e.ID == observableItemModel.ItemId);
                                    if (observItem != null && observItem.Type == ItemType.ObservableChoice)
                                    {
                                        observableItemModel.IsRequired = observItem.IsRequired;
                                    }

                                }
                            }
                        }
                        CacheHelper.Add(key, assessment);
                    }
                }
            }
            var clone = assessment.Clone();
            return clone;
        }

        #region Create Entity
        public ObservableAssessmentEntity NewObservableAssessmentEntity()
        {
            return _observableService.NewObservableAssessmentEntity();
        }

        public ObservableAssessmentItemEntity NewObservableAssessmentItemEntity()
        {
            return _observableService.NewObservableAssessmentItemEntity();
        }

        public ObservableItemsHistoryEntity NewObservableItemsHistoryEntity()
        {
            return _observableService.NewObservableItemsHistoryEntity();
        }
        #endregion

        public OperationResult SaveObservableAssessment(ObservableAssessmentEntity entity)
        {
            if (entity.ID <= 0)
                return _observableService.InsertObservableAssessmentEntity(entity);
            else
            {
                return _observableService.UpdateObservableAssessmentEntity(entity);
            }
        }

        public OperationResult SaveAssessmentItems(ObservableAssessmentItemEntity item)
        {
            var res = new OperationResult(OperationResultType.Success);
            if (item.ID > 0)
            {
                res = _observableService.UpdateObservableAssessmentItemEntity(item);
            }
            else
            {
                res = _observableService.InsertObservableAssessmentItemEntity(item);
            }
            if (res.ResultType == OperationResultType.Success)
            {
                var history = new ObservableItemsHistoryEntity();
                history.CreatedBy = item.CreatedBy;
                history.CreatedOn = item.CreatedOn;
                history.UpdatedBy = item.UpdatedBy;
                history.UpdatedOn = item.UpdatedOn;
                history.ItemId = item.ItemId;
                history.Response = item.Response;
                history.ObservableAssessmentId = item.ObservableAssessmentId;
                res = _observableService.InsertObservableItemsHistoryEntity(history);

            }
            return res;
        }

        public List<ObservableAssessmentItemEntity> GetAssessmentItemEntities(int observableId, int assessmentId, int studentId, int childId)
        {
            if (observableId > 0)
                return _observableService.ObservableAssessmentItems.Where(c => c.ObservableAssessmentId == observableId && c.ObservableAssessment.AssessmentId == assessmentId
                    && ((studentId != 0 && c.ObservableAssessment.StudentId == studentId) ||
                    (childId != 0 && c.ObservableAssessment.ChildId == childId))
                    ).ToList();
            else
            {
                return _observableService.ObservableAssessmentItems.Where(c => c.ObservableAssessment.AssessmentId == assessmentId
              && ((studentId != 0 && c.ObservableAssessment.StudentId == studentId) ||
                    (childId != 0 && c.ObservableAssessment.ChildId == childId))
              ).ToList();
            }
        }

        public ObservableAssessmentEntity GetAssessmentModel(int id)
        {
            return _observableService.GetObservableAssessmentEntity(id);
        }

        public string GetAssessmentReportName(int studentId, DateTime date, string assessmentName, int index = 0)
        {
            string name = assessmentName + "_" + date.ToString("MMddyy");
            if (index > 0)
                name = name + "_" + index;
            if (_observableService.ObservableAssessments.Any(c => c.StudentId == studentId && c.ReportName == name))
            {
                index++;
                return GetAssessmentReportName(studentId, date, assessmentName, index);
            }
            else
            {
                return name;
            }
        }

        public List<ObservableReportModel> GetAssessmentRports(int assessmentId, int studentId, int status, DateTime? reportBegin, DateTime? reportEnd)
        {

            if (status == (int)ReportStatus.Showfirst)
            {
                return _observableService.ObservableAssessments.Where(c => c.AssessmentId == assessmentId && c.StudentId == studentId &&
                   c.ReportUrl != null && c.ReportUrl != "" && (reportBegin == null || c.CreatedOn >= reportBegin.Value)
                   && (reportEnd == null || c.CreatedOn <= reportEnd.Value)).Select(c => new ObservableReportModel()
                   {
                       ID = c.ID,
                       Name = c.ReportName.Replace("-", "/"),

                       CreateOn = c.CreatedOn
                   }).OrderByDescending(c => c.CreateOn).OrderByDescending(c => c.Name).Skip(0).Take(1).ToList();
            }
            else if (status == (int)ReportStatus.Showmostrecent)
            {
                return _observableService.ObservableAssessments.Where(c => c.AssessmentId == assessmentId && c.StudentId == studentId &&
                  c.ReportUrl != null && c.ReportUrl != "" && (reportBegin == null || c.CreatedOn >= reportBegin.Value)
                   && (reportEnd == null || c.CreatedOn <= reportEnd.Value)).Select(c => new ObservableReportModel()
                  {
                      ID = c.ID,
                      Name = c.ReportName.Replace("-", "/"),

                      CreateOn = c.CreatedOn
                  }).OrderByDescending(c => c.CreateOn).OrderByDescending(c => c.Name).Skip(0).Take(3).ToList();
            }
            else
            {
                return _observableService.ObservableAssessments.Where(c => c.AssessmentId == assessmentId && c.StudentId == studentId &&
            c.ReportUrl != null && c.ReportUrl != "" && (reportBegin == null || c.CreatedOn >= reportBegin.Value)
                   && (reportEnd == null || c.CreatedOn <= reportEnd.Value)).Select(c => new ObservableReportModel()
            {
                ID = c.ID,
                Name = c.ReportName.Replace("-", "/"),

                CreateOn = c.CreatedOn
            }).OrderByDescending(c => c.CreateOn).OrderByDescending(c => c.Name).ToList();
            }

        }

        public List<int> GetAssessmentReportstudentIds(int assessmentId, int status, DateTime? reportBegin, DateTime? reportEnd)
        {
            return _observableService.ObservableAssessments.Where(c => c.AssessmentId == assessmentId &&
                c.ReportUrl != null && c.ReportUrl != "" && (reportBegin == null || c.CreatedOn >= reportBegin.Value)
                && (reportEnd == null || c.CreatedOn <= reportEnd.Value)).Select(c => c.StudentId).ToList();
        }
        public OperationResult DeleteReport(int ID)
        {
            var res = new OperationResult(OperationResultType.Success);
            var findItem = _observableService.GetObservableAssessmentEntity(ID);
            if (findItem != null)
            {
                findItem.ReportUrl = "";
                findItem.ReportName = "";
                res = _observableService.UpdateObservableAssessmentEntity(findItem);
            }
            return res;
        }

        #region Get list by page
        public IList<ObservableReportModel> SearchObervableReports(int studentId, int childId, DateTime dob, string sort, string order, int first, int count, out int total)
        {
            var studentAssessmentList =
                _observableService.ObservableAssessments.Where(c =>
                    ((studentId != 0 && c.StudentId == studentId) || (childId != 0 && c.ChildId == childId))
                    && c.ReportUrl != null && c.ReportUrl != "")
                 .Select(c => new ObservableReportModel()
                {
                    ID = c.ID,
                    assessmentId = c.AssessmentId,
                    Name = c.ReportName.Replace("-", "/"),
                    CreateOn = c.CreatedOn,
                    DOB = dob
                }).ToList();
            total = 0;
            IList<ObservableReportModel> list = new List<ObservableReportModel>();
            if (studentAssessmentList != null)
            {
                var query = studentAssessmentList.Where(a => a.ID == (studentAssessmentList
                    .Where(b => b.assessmentId == a.assessmentId)
                    .OrderByDescending(n => n.CreateOn)
                    .ThenByDescending(n => n.ID)
                    .FirstOrDefault().ID
                    ));

                total = query.Count();
                list = query.OrderBy(c => c.Name).Skip(first).Take(count).ToList();
            }
            return list;

        }

        public IList<ObservableReportModel> SearchObervableReports2(int studentId, int childId, DateTime dob, string sort, string order, int first, int count, out int total)
        {
            var query =
                _observableService.ObservableAssessments.Where(c =>
                    ((studentId != 0 && c.StudentId == studentId) || (childId != 0 && c.ChildId == childId))
                    && c.ReportUrl != null && c.ReportUrl != "")
                 .Select(c => new ObservableReportModel()
                 {
                     ID = c.ID,
                     assessmentId = c.AssessmentId,
                     Name = c.ReportName.Replace("-", "/"),
                     CreateOn = c.CreatedOn,
                     DOB = dob
                 }).OrderByDescending(n => n.CreateOn)
                    .ThenByDescending(n => n.ID);
            total = 0;
            total = query.Count();
            IList<ObservableReportModel> list = new List<ObservableReportModel>();
            list = query.Skip(first).Take(count).ToList();
            return list;
        }



        public IList<ObservableReportModel> SearchObervableReportHistory(int assessmentId, int studentId, int childId, DateTime dob, string sort, string order, int first, int count, out int total)
        {
            var query =
                  _observableService.ObservableAssessments.Where(c =>
                      ((c.StudentId != 0 && c.StudentId == studentId) || (c.ChildId != 0 && c.ChildId == childId))
                      && c.AssessmentId == assessmentId && c.ReportUrl != null && c.ReportUrl != "")
                   .Select(c => new ObservableReportModel()
                   {
                       ID = c.ID,
                       assessmentId = c.AssessmentId,
                       Name = c.ReportName.Replace("-", "/"),
                       CreateOn = c.CreatedOn,
                       DOB = dob
                   }).ToList();
            total = query.Count();
            var list = query.OrderBy(c => c.Name).Skip(first).Take(count).ToList();
            return list;

        }
        #endregion
    }
}
