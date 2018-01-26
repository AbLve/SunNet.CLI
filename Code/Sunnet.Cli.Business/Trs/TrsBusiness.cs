using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/12 2015 16:01:43
 * Description:		Please input class summary
 * Version History:	Created,1/12 2015 16:01:43
 * 
 * 
 **************************************************************************/
using LinqKit;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using System.Collections;
using Sunnet.Cli.Business.TRSClasses;
using Sunnet.Cli.Business.Users;

namespace Sunnet.Cli.Business.Trs
{
    public partial class TrsBusiness
    {
        private static TRSClassBusiness trsClassBusiness
        {
            get
            {
                return new TRSClassBusiness();
            }
        }

        private static SchoolBusiness SchoolBusiness
        {
            get { return new SchoolBusiness(); }
        }

        private static UserBusiness UserBusiness
        {
            get { return new UserBusiness(); }
        }

        private ITRSContract _trsContract;
        public TrsBusiness(AdeUnitOfWorkContext unit = null)
        {
            _trsContract = DomainFacade.CreateTRSService(unit);
        }

        public TRSAssessmentEntity GetAssessment(int id)
        {
            return _trsContract.GetAssessment(id);
        }

        public OperationResult UpdateAssessment(TRSAssessmentEntity entity)
        {
            return _trsContract.UpdateAssessment(entity);
        }

        /// <summary>
        /// Gets the assessment model being editing status.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public TrsAssessmentModel GetAssessmentModel(int id, UserBaseEntity user)
        {
            var assessmentModel =
                _trsContract.Assessments.Where(x => x.ID == id)
                .Select(AssessmentEntityToModel).FirstOrDefault();
            if (assessmentModel == null)
            {
                return null;
            }
            int schoolId = assessmentModel.SchoolId;
            var schoolModel = SchoolBusiness.GetTrsSchool(schoolId, user);
            if (schoolModel == null)
                return null;

            var items = assessmentModel.Items.ToList();
            items.AddRange(_trsContract.Items.Where(i => i.IsDeleted == false).Select(ItemEntityToModel));
            assessmentModel.Items = items;
            assessmentModel.School = schoolModel;
            assessmentModel.Classes = trsClassBusiness.GetTrsClasses(user, schoolId);
            assessmentModel.SubCategory = _trsContract.Subcategories.Select(x => new TrsSubcategoryModel()
            {
                Category = x.Category,
                Id = x.ID,
                Name = x.Name,
                Sort = x.Sort,
                Type = x.Type
            }).ToDictionary(x => x.Id, x => x);
            assessmentModel.PrepareWithAccess(user);
            return assessmentModel;
        }

        private static Expression<Func<TRSAssessmentEntity, TrsAssessmentModel>> AssessmentEntityToModel
        {
            get
            {
                return x => new TrsAssessmentModel()
                {
                    Id = x.ID,
                    Star = x.Star,
                    VerifiedStar = x.VerifiedStar,
                    Status = x.Status,
                    Type = x.Type,
                    EventLogType = x.EventLogType,
                    ApproveDate = x.ApproveDate,
                    RecertificatedBy = x.RecertificatedBy,
                    DiscussDate = x.DiscussDate,
                    VisitDate = x.VisitDate,
                    SchoolId = x.SchoolId,
                    TaStatusString = x.TAStatus,
                    UpdatedOn = x.UpdatedOn,
                    CreatedOn = x.CreatedOn,
                    AssessmentClasses = x.AssessmentClasses.Select(r => new TrsAssessmentClassModel
                    {
                        ID = r.ID,
                        AssessmentId = r.AssessmentId,
                        ClassId = r.ClassId,
                        ObservationLength = r.ObservationLength
                    }),
                    Items = x.AssessmentItems.Where(i => i.Item.IsDeleted == false).Select(item => new TrsItemModel()
                    {
                        Id = item.ID,
                        AnswerId = item.AnswerId.Value,
                        Answers = item.Item.ItemAnswers.Select(answer => new TrsAnswerModel()
                        {
                            Id = answer.AnswerId,
                            Text = answer.Answer.Text,
                            Score = answer.Answer.Score
                        }),
                        AnswerText = item.Answer == null ? "" : item.Answer.Text,
                        Category = item.Item.Category,
                        Description = item.Item.Description,
                        KeyBehavior = item.Item.KeyBehavior,
                        Filled = item.AnswerId.Value > 0,
                        Filter = item.Item.Filter,
                        ItemId = item.ItemId,
                        AssessmentItemId = item.ID,
                        IsRequired = item.Item.IsRequired,
                        Item = item.Item.Item,
                        LinkingDocument = item.Item.LinkingDocument,
                        SubCategorySort = item.Item.SubCategory.Sort,
                        Sort = item.Item.Sort,
                        Score = item.Answer == null ? 0 : item.Answer.Score == null ? -1 : item.Answer.Score,
                        ShowByInfants = item.Item.ShowByInfants,
                        ShowByToddlers = item.Item.ShowByToddlers,
                        ShowByPreschool = item.Item.ShowByPreschool,
                        ShowBySchoolAge = item.Item.ShowBySchoolAge,
                        LCSA = item.Item.LCSA,
                        LCCH = item.Item.LCCH,
                        RCCH = item.Item.RCCH,
                        LCAA = item.Item.LCAA,
                        SubCategoryId = item.Item.SubCategoryId,
                        TAItemInstructions = item.Item.TAItemInstructions,
                        TAPlanItem = item.Item.TAPlanItem,
                        TAPlanItemType = item.Item.TAPlanItemType,
                        SyncAnswer = item.Item.SyncAnswer,
                        Text = item.Item.Text,
                        Type = item.Item.Type,
                        ClassId = item.ClassId,
                        Comments = item.Comments,
                        AgeGroup = (int)item.AgeGroup,
                        GroupSize = item.GroupSize,
                        CaregiversNo = item.CaregiversNo
                    })
                };
            }
        }

        /// <summary>
        /// Gets the new assessment model.
        /// </summary>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public TrsAssessmentModel GetNewAssessmentModel(int schoolId, UserBaseEntity user)
        {
            var schoolModel = SchoolBusiness.GetTrsSchool(schoolId, user);
            if (schoolModel == null)
                return null;
            var assessmentModel = new TrsAssessmentModel()
            {
                SchoolId = schoolId
            };
            var items = assessmentModel.Items.ToList();
            items.AddRange(_trsContract.Items.Where(i => i.IsDeleted == false).Select(ItemEntityToModel));
            assessmentModel.Items = items;
            assessmentModel.School = schoolModel;
            assessmentModel.Classes = trsClassBusiness.GetTrsClasses(user, schoolId);
            assessmentModel.SubCategory = _trsContract.Subcategories.Select(x => new TrsSubcategoryModel()
            {
                Category = x.Category,
                Id = x.ID,
                Name = x.Name,
                Sort = x.Sort,
                Type = x.Type
            }).ToDictionary(x => x.Id, x => x);
            assessmentModel.PrepareWithAccess(user);
            return assessmentModel;
        }

        private static Expression<Func<TRSItemEntity, TrsItemModel>> ItemEntityToModel
        {
            get
            {
                return item => new TrsItemModel()
                {
                    AnswerId = 0,
                    Answers = item.ItemAnswers.Select(answer => new TrsAnswerModel()
                    {
                        Id = answer.AnswerId,
                        Text = answer.Answer.Text,
                        Score = answer.Answer.Score
                    }),
                    AnswerText = "",
                    Category = item.Category,
                    Description = item.Description,
                    KeyBehavior = item.KeyBehavior,
                    Filled = false,
                    Filter = item.Filter,
                    IsRequired = item.IsRequired,
                    Item = item.Item,
                    ItemId = item.ID,
                    AssessmentItemId = 0,
                    LinkingDocument = item.LinkingDocument,
                    Score = -1,
                    ShowByInfants = item.ShowByInfants,
                    ShowByToddlers = item.ShowByToddlers,
                    ShowByPreschool = item.ShowByPreschool,
                    ShowBySchoolAge = item.ShowBySchoolAge,
                    LCSA = item.LCSA,
                    LCCH = item.LCCH,
                    RCCH = item.RCCH,
                    LCAA = item.LCAA,
                    SubCategorySort = item.SubCategory.Sort,
                    Sort = item.Sort,
                    SubCategoryId = item.SubCategoryId,
                    TAItemInstructions = item.TAItemInstructions,
                    TAPlanItem = item.TAPlanItem,
                    TAPlanItemType = item.TAPlanItemType,
                    SyncAnswer = item.SyncAnswer,
                    Text = item.Text,
                    Type = item.Type
                };
            }
        }

        private class CompareTRSAssessmentItem : IEqualityComparer<TRSAssessmentItemEntity>
        {
            public bool Equals(TRSAssessmentItemEntity x, TRSAssessmentItemEntity y)
            {
                return x.ClassId == y.ClassId && x.ItemId == y.ItemId;
            }
            public int GetHashCode(TRSAssessmentItemEntity obj)
            {
                var trsAssessmentItem = new { obj.ClassId, obj.ItemId };
                return trsAssessmentItem.GetHashCode();
            }
        }

        public OperationResult SaveAssessment(TRSAssessmentEntity entity, List<TRSAssessmentItemEntity> items,
            UserBaseEntity user, List<TRSAssessmentClassEntity> assessmentClasses, bool hasRetained = false)
        {
            var result = new OperationResult(OperationResultType.Success);
            TRSAssessmentEntity oldEntity = null;
            if (entity.ID > 0)
            {
                oldEntity = _trsContract.GetAssessment(entity.ID);
                if (oldEntity.Status == TRSStatusEnum.Completed)
                {
                    return new OperationResult(OperationResultType.Error, "The assessment has been finalized on " + oldEntity.UpdatedOn);
                }
            }
            if (oldEntity == null)
            {
                oldEntity = _trsContract.NewAssessmentEntity();
                oldEntity.ApproveDate = CommonAgent.MinDate;
                oldEntity.DiscussDate = CommonAgent.MinDate;
                oldEntity.VisitDate = CommonAgent.MinDate;
                oldEntity.RecertificatedBy = CommonAgent.MinDate;
                oldEntity.CreatedBy = user.ID;
                oldEntity.CreatedOn = DateTime.Now;
                oldEntity.Status = TRSStatusEnum.Initialized;
                oldEntity.SchoolId = entity.SchoolId;
                result = _trsContract.InsertAssessment(oldEntity);
                if (result.ResultType != OperationResultType.Success)
                    return result;
            }

            oldEntity.UpdatedBy = user.ID;
            oldEntity.UpdatedOn = DateTime.Now;

            if (entity.DiscussDate > CommonAgent.MinDate)
                oldEntity.DiscussDate = entity.DiscussDate;
            if (entity.VisitDate > CommonAgent.MinDate)
                oldEntity.VisitDate = entity.VisitDate;
            oldEntity.Type = entity.Type;
            oldEntity.Status = entity.Status;
            oldEntity.TAStatus = entity.TAStatus;
            if (oldEntity.Status != TRSStatusEnum.Completed)
            {
                oldEntity.Status = TRSStatusEnum.Saved;
            }

            if (entity.ApproveDate > CommonAgent.MinDate)
                oldEntity.ApproveDate = entity.ApproveDate;
            if (entity.RecertificatedBy > CommonAgent.MinDate)
                oldEntity.RecertificatedBy = entity.RecertificatedBy;

            var savedItems = oldEntity.AssessmentItems.Where(i => i.Item.IsDeleted == false).ToList();
            #region 删除页面上不显示但是已经保存到数据库的TRSAssessmentItems
            if (savedItems.Any())
            {
                var delItems = savedItems.Except(items, new CompareTRSAssessmentItem()).ToList();
                if (delItems.Any())
                {
                    List<int> delItemIds = delItems.Select(x => x.ID).ToList();
                    int count = _trsContract.DelAssessmentItems(delItemIds);
                    if (count <= 0)
                    {
                        return new OperationResult(OperationResultType.Error, "Please refresh the page and try again. ");
                    }
                }
            }
            #endregion
            if (items != null && items.Any())
                items.ForEach(newItem =>
                {
                    newItem.CreatedBy = user.ID;
                    newItem.CreatedOn = DateTime.Now;
                    newItem.UpdatedBy = user.ID;
                    newItem.UpdatedOn = DateTime.Now;
                    newItem.TRSAssessmentId = oldEntity.ID;
                    var oldItem = savedItems.Find(x => x.ItemId == newItem.ItemId && x.ClassId == newItem.ClassId);
                    if (oldItem == null)
                    {
                        oldEntity.AssessmentItems.Add(newItem);
                    }
                    else
                    {
                        oldItem.UpdatedBy = user.ID;
                        oldItem.UpdatedOn = DateTime.Now;
                        oldItem.AnswerId = newItem.AnswerId;
                        oldItem.Comments = newItem.Comments;
                        oldItem.AgeGroup = newItem.AgeGroup;
                        oldItem.GroupSize = newItem.GroupSize;
                        oldItem.CaregiversNo = newItem.CaregiversNo;
                    }
                });

            //先删除AssessmentClasses
            List<TRSAssessmentClassEntity> list_AssessmentClasses = oldEntity.AssessmentClasses.ToList();
            if (list_AssessmentClasses != null && list_AssessmentClasses.Count > 0)
            {
                result = _trsContract.DeleteAssessmentClasses(list_AssessmentClasses, false);
            }
            //再添加AssessmentClasses
            if (assessmentClasses != null && assessmentClasses.Count > 0)
            {
                foreach (TRSAssessmentClassEntity item in assessmentClasses)
                {
                    oldEntity.AssessmentClasses.Add(item);
                }
            }
            result = _trsContract.UpdateAssessment(oldEntity);
            if (result.ResultType == OperationResultType.Success && oldEntity.Status == TRSStatusEnum.Completed)
            {
                result = FinalizeAssessment(oldEntity, user, hasRetained);

            }
            result.AppendData = new
            {
                id = oldEntity.ID,
                status = oldEntity.Status
            };
            return result;
        }

        public OperationResult AddAssessments(List<TRSAssessmentEntity> assessments, bool isSave)
        {
            return _trsContract.InsertAssessment(assessments, isSave);
        }

        public OperationResult UpdateVerifiedStar(int id, TRSStarEnum star, UserBaseEntity user)
        {
            var entity = _trsContract.GetAssessment(id);
            if (entity == null)
                return new OperationResult(OperationResultType.Error, "Assessment is null.");

            entity.VerifiedStar = star;
            //entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = user.ID;
            var result = _trsContract.UpdateAssessment(entity);
            if (result.ResultType == OperationResultType.Success && entity.Type != TrsAssessmentType.AnnualMonitoring)
            {
                result = SchoolBusiness.SaveSchool(entity.SchoolId, star);
            }
            result.AppendData = entity.SchoolId;
            return result;
        }

        private OperationResult FinalizeAssessment(TRSAssessmentEntity entity, UserBaseEntity user, bool hasRetained = false)
        {
            OperationResult result = null;
            if (entity == null || entity.ID < 0)
            {
                return new OperationResult(OperationResultType.Error, "Assessment is required.");
            }
            if (entity.Status != TRSStatusEnum.Completed)
            {
                return new OperationResult(OperationResultType.Error, "Assessment status is not valid.");
            }
            var model = GetAssessmentModel(entity.ID, user);
            if (model == null) return new OperationResult(OperationResultType.Error, "Something is wrong: assessment model is null.");
            if (hasRetained)
            {
                entity.Stars.ForEach(x =>
                {
                    x.UpdatedOn = DateTime.Now;
                });
            }
            else
            {
                model.UpdateStar();
                entity.Star = model.Star;
                var starOfClass = entity.Stars.ToList();
                model.StarOfCategory.ForEach(categoryStar =>
                {
                    var oldStar = starOfClass.Find(x => x.Category == categoryStar.Key && x.ClassId == 0);
                    if (oldStar != null)
                    {
                        oldStar.Retain = false;
                        oldStar.AutoAssign = false;
                        oldStar.Star = categoryStar.Value;
                        oldStar.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        entity.Stars.Add(new TrsStarEntity()
                        {
                            ClassId = 0,
                            Category = categoryStar.Key,
                            Star = categoryStar.Value,
                            Retain = false,
                            AutoAssign = false
                        });
                    }
                });
            }
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = user.ID;
            result = _trsContract.UpdateAssessment(entity);
            if (result.ResultType == OperationResultType.Success)
                result = SchoolBusiness.SaveSchool(entity.SchoolId, model.TaStatuses.ToList(), entity.RecertificatedBy,
                    entity.ApproveDate, entity.Type, entity.Type != TrsAssessmentType.AnnualMonitoring, entity.Star);
            return result;
        }

        public List<TrsAssessmentListModel> GetAssessments(int schoolId, UserBaseEntity user, TrsSchoolModel schoolModel)
        {
            var condition = PredicateHelper.True<TRSAssessmentEntity>();
            if (schoolModel.Action != "assessment" && schoolModel.Action != "viewAssessment")
            {
                condition = x => x.Status == TRSStatusEnum.Completed;
            }
            if (string.IsNullOrEmpty(schoolModel.Action))
            {
                condition = x => false;
            }
            var assessments = _trsContract.Assessments.AsExpandable().Where(x => x.SchoolId == schoolId).Where(condition)
                .OrderByDescending(x => x.ID)
                .Select(a => new TrsAssessmentListModel()
                {
                    Id = a.ID,
                    ApproveDate = a.ApproveDate,
                    CreatedOn = a.CreatedOn,
                    UpdatedOn = a.UpdatedOn,
                    SchoolId = a.SchoolId,
                    Star = a.Star,
                    VerifiedStar = a.VerifiedStar,
                    Status = a.Status,
                    Type = a.Type,
                    EventLogType = a.EventLogType,
                    ClassIds = a.AssessmentItems.Where(i => i.Item.IsDeleted == false).Select(x => x.ClassId).Distinct()
                }).ToList();

            var schoolIds = assessments.Select(x => x.SchoolId).ToList();
            var classes = trsClassBusiness.GetTrsClasses(user, schoolIds.ToArray());

            assessments.ForEach(a =>
            {
                a.School = schoolModel;
                a.Classes = classes.Where(x => a.ClassIds.Contains(x.Id)).ToList();
                a.UpdateAction(user);
            });
            return assessments;
        }

        public OperationResult Invalidate(int assessmentId, UserBaseEntity user)
        {
            var entity = _trsContract.GetAssessment(assessmentId);
            if (entity == null)
                return new OperationResult(OperationResultType.Success);
            if (entity.Star > 0)
            {
                // 正常的Assessment
                entity.Status = TRSStatusEnum.Saved;
                entity.Star = 0;
                entity.VerifiedStar = 0;
                entity.Stars.ForEach(x =>
                {
                    x.Star = 0;
                    x.Retain = false;
                    x.AutoAssign = false;
                });
                entity.UpdatedBy = user.ID;
                entity.UpdatedOn = DateTime.Now;
            }
            else
            {
                // 由School修改Verified Star产生的记录 直接删除
                entity.IsDeleted = true;
                entity.UpdatedOn = DateTime.Now;
                entity.UpdatedBy = user.ID;
            }
            return _trsContract.UpdateAssessment(entity);
        }

        public OperationResult DeleteAssessment(int assessmentId)
        {
            var assessment = _trsContract.GetAssessment(assessmentId);
            if (assessment == null)
                return new OperationResult(OperationResultType.Success);
            if (assessment.Status == TRSStatusEnum.Completed)
                return new OperationResult(OperationResultType.Error, "Finalized assessment can not be deleted.");
            assessment.IsDeleted = true;
            return _trsContract.UpdateAssessment(assessment);
        }

        public int DeleteOfflineAssessment(List<int> assessmentIds)
        {
            return _trsContract.DeleteOfflineAssessment(assessmentIds);
        }

        /// <summary>
        /// School 的 Verified Star Designation发生变更时生成空的Assessment记录
        /// </summary>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="verifiedStar">The verified star.</param>
        /// <param name="approvalDate">The approval date.</param>
        /// <param name="recertificatedBy">The recertificated by.</param>
        /// <returns></returns>
        public OperationResult InsertRecordOfVerifiedStarChanged(int schoolId, TRSStarEnum verifiedStar, DateTime approvalDate,
            DateTime recertificatedBy, EventLogType eventLogType = 0)
        {
            var entity = _trsContract.NewAssessmentEntity();

            entity.ApproveDate = approvalDate;
            entity.DiscussDate = CommonAgent.MinDate;
            entity.VisitDate = CommonAgent.MinDate;
            entity.CreatedBy = 0;
            entity.CreatedOn = DateTime.Now;
            entity.Status = TRSStatusEnum.Completed;
            entity.SchoolId = schoolId;
            entity.UpdatedBy = 0;
            entity.UpdatedOn = DateTime.Now;
            entity.IsDeleted = false;
            entity.RecertificatedBy = recertificatedBy;
            entity.VerifiedStar = verifiedStar;
            entity.EventLogType = eventLogType;

            var result = _trsContract.InsertAssessment(entity);
            return result;
        }

        public OperationResult DeleteAssessmentClasses(IEnumerable<TRSAssessmentClassEntity> entities, bool isSave)
        {
            return _trsContract.DeleteAssessmentClasses(entities, isSave);
        }

        public TrsRecentStarModel GetRecentStarModel(int assessmentId, int schoolId, UserBaseEntity user)
        {
            TrsAssessmentModel model = GetPreviewModel(assessmentId, user);
            TRSAssessmentEntity assessment = _trsContract.Assessments
                .Where(x => x.Status == TRSStatusEnum.Completed
                    && x.SchoolId == schoolId)
                .OrderByDescending(x => x.CreatedOn).FirstOrDefault();

            TrsRecentStarModel recentStarModel = new TrsRecentStarModel();

            recentStarModel.CurrentStars = new Dictionary<TRSCategoryEnum, TRSStarDisplayEnum>();
            recentStarModel.CurrentStars.Add(TRSCategoryEnum.Category1, 0);
            recentStarModel.CurrentStars.Add(TRSCategoryEnum.Category2, 0);
            recentStarModel.CurrentStars.Add(TRSCategoryEnum.Category3, 0);
            recentStarModel.CurrentStars.Add(TRSCategoryEnum.Category4, 0);
            recentStarModel.CurrentStars.Add(TRSCategoryEnum.Category5, 0);

            recentStarModel.RecentStars = new Dictionary<TRSCategoryEnum, TRSStarDisplayEnum>();
            recentStarModel.RecentStars.Add(TRSCategoryEnum.Category1, 0);
            recentStarModel.RecentStars.Add(TRSCategoryEnum.Category2, 0);
            recentStarModel.RecentStars.Add(TRSCategoryEnum.Category3, 0);
            recentStarModel.RecentStars.Add(TRSCategoryEnum.Category4, 0);
            recentStarModel.RecentStars.Add(TRSCategoryEnum.Category5, 0);

            List<TRSCategoryEnum> categoryList = new List<TRSCategoryEnum>();
            categoryList.Add(TRSCategoryEnum.Category1);
            categoryList.Add(TRSCategoryEnum.Category2);
            categoryList.Add(TRSCategoryEnum.Category3);
            categoryList.Add(TRSCategoryEnum.Category4);
            categoryList.Add(TRSCategoryEnum.Category5);

            if (model.StarOfCategory.Count > 0)
            {
                model.StarOfCategory.ForEach(x =>
                {
                    recentStarModel.CurrentStars[x.Key] = (TRSStarDisplayEnum)x.Value;
                });
            }
            categoryList.ForEach(x =>
            {
                // Structural 没有选择N/A的项，没有全部Met，所有Category Star 均为 TRSStarEnum.One，
                // 所以并不是把Category中的Item 全部N/A ,Category Star就会等于0
                if (model.Items.Count(o => o.Category == x && o.Score >= 0) == 0 && recentStarModel.CurrentStars[x] == 0)
                {
                    recentStarModel.CurrentStars[x] = TRSStarDisplayEnum.NA;
                }
            });

            if (assessment == null)
            {
                recentStarModel.HasRecentStar = false;
                recentStarModel.RecentVerifiedStar = 0;
            }
            else
            {
                if (assessment.Stars.Count > 0)
                {
                    assessment.Stars.ForEach(x =>
                    {
                        recentStarModel.RecentStars[x.Category] = (TRSStarDisplayEnum)x.Star;
                    });
                }
                categoryList.ForEach(x =>
                {
                    if (!assessment.AssessmentItems.Any())
                    {
                        recentStarModel.RecentStars[x] = TRSStarDisplayEnum.AutoAssign;
                    }
                    else if (assessment.AssessmentItems.Count(o => o.Item.Category == x && o.AnswerId.Value > 0 && o.Answer.Score >= 0) == 0
                    && recentStarModel.RecentStars[x] == 0)
                    {
                        recentStarModel.RecentStars[x] = TRSStarDisplayEnum.NA;
                    }
                });
                recentStarModel.HasRecentStar = true;
                recentStarModel.RecentVerifiedStar = assessment.VerifiedStar;
            }
            return recentStarModel;
        }

        public OperationResult InsertTrsStars(int assessmentId, int category1, int category2, int category3, int category4, int category5,
            int verifiedStar, int[] chk, List<int> autoAssginCategorys, UserBaseEntity user)
        {
            Dictionary<TRSCategoryEnum, TRSStarEnum> StarOfCategory = new Dictionary<TRSCategoryEnum, TRSStarEnum>();
            StarOfCategory.Add(TRSCategoryEnum.Category1, (TRSStarEnum)category1);
            StarOfCategory.Add(TRSCategoryEnum.Category2, (TRSStarEnum)category2);
            StarOfCategory.Add(TRSCategoryEnum.Category3, (TRSStarEnum)category3);
            StarOfCategory.Add(TRSCategoryEnum.Category4, (TRSStarEnum)category4);
            StarOfCategory.Add(TRSCategoryEnum.Category5, (TRSStarEnum)category5);
            TRSStarEnum star = TRSStarEnum.One;
            int numOfFour = StarOfCategory.Count(x => x.Value == TRSStarEnum.Four);
            int numOfTwo = StarOfCategory.Count(x => x.Value == TRSStarEnum.Two);
            if (numOfFour == 4 && numOfTwo == 1)
            {
                star = TRSStarEnum.Three;
            }
            else
            {
                star = StarOfCategory.Any(x => x.Value != 0) ? StarOfCategory.Where(x => x.Value != 0).Min(x => x.Value) : TRSStarEnum.One;
            }

            List<int> retainCategory = new List<int>();
            if (chk != null)
            {
                retainCategory = chk.ToList();
            }
            else
            {
                retainCategory.Add(0);
            }

            TRSAssessmentEntity assessment = GetAssessment(assessmentId);
            var currentStars = assessment.Stars.ToList();
            StarOfCategory.ForEach(categoryStar =>
            {
                var oldStar = currentStars.Find(x => x.Category == categoryStar.Key && x.ClassId == 0);
                if (oldStar != null)
                {
                    if (retainCategory.Contains((int)oldStar.Category))
                        oldStar.Retain = true;
                    else
                        oldStar.Retain = false;
                    if (autoAssginCategorys.Contains((int)oldStar.Category))
                        oldStar.AutoAssign = true;
                    else
                        oldStar.AutoAssign = false;
                    oldStar.Star = categoryStar.Value;
                    oldStar.UpdatedOn = DateTime.Now;
                }
                else
                {
                    assessment.Stars.Add(new TrsStarEntity()
                    {
                        ClassId = 0,
                        Category = categoryStar.Key,
                        Star = categoryStar.Value,
                        Retain = retainCategory.Contains((int)categoryStar.Key) ? true : false,
                        AutoAssign = autoAssginCategorys.Contains((int)categoryStar.Key) ? true : false
                    });
                }
            });
            assessment.UpdatedOn = DateTime.Now;
            assessment.UpdatedBy = user.ID;
            assessment.Star = star;
            assessment.VerifiedStar = verifiedStar == 0 ? assessment.VerifiedStar : (TRSStarEnum)verifiedStar;
            return _trsContract.UpdateAssessment(assessment);
        }

        #region TRS ReScore
        public List<TRSAssessmentEntity> GetAssessmentByCreateDate(DateTime createDate)
        {
            return _trsContract.Assessments.Where(e => e.CreatedOn > createDate && e.IsDeleted == false).ToList();
        }

        public int UpdateAssessmentItemAnswer(int trsAssessmentItemId, int newAnswerId)
        {
            return _trsContract.UpdateItemAnswer(trsAssessmentItemId, newAnswerId);
        }

        public int UpdateTrsAssessmentStar(int trsAssessmentId, byte newStar)
        {
            return _trsContract.UpdateTrsAssessmentStar(trsAssessmentId, newStar);
        }

        public int UpdateTrsStar(int trsStarId, byte newStar)
        {
            return _trsContract.UpdateTrsStar(trsStarId, newStar);
        }

        public TrsStarEntity GetTrsStarByCategory(int trsAssessmentId, TRSCategoryEnum category)
        {
            return _trsContract.Stars.Where(s => s.AssessmentId == trsAssessmentId && s.Category == category).FirstOrDefault();
        }
        #endregion


        #region Offline

        public List<TrsAssessmentModel> GetAssessmentModelsBySchool(TrsOfflineSchoolModel school, UserBaseEntity user)
        {
            List<TrsAssessmentModel> assessmentModels =
                _trsContract.Assessments.Where(x => x.SchoolId == school.ID && school.IsCommunityTRS
                && !(x.Status == TRSStatusEnum.Completed && (int)x.Star < 1) && x.IsDeleted == false)
                .Select(x => new TrsAssessmentModel
                {
                    Id = x.ID,
                    Star = x.Star,
                    VerifiedStar = x.VerifiedStar,
                    Status = x.Status,
                    Type = x.Type,
                    EventLogType = x.EventLogType,
                    ApproveDate = x.ApproveDate,
                    RecertificatedBy = x.RecertificatedBy,
                    DiscussDate = x.DiscussDate,
                    VisitDate = x.VisitDate,
                    SchoolId = x.SchoolId,
                    TaStatusString = x.TAStatus,
                    UpdatedOn = x.UpdatedOn,
                    CreatedOn = x.CreatedOn,
                    AssessmentClasses = x.AssessmentClasses.Select(r => new TrsAssessmentClassModel
                    {
                        ID = r.ID,
                        AssessmentId = r.AssessmentId,
                        ClassId = r.ClassId,
                        ObservationLength = r.ObservationLength
                    }),
                    Items = x.AssessmentItems.Where(i => i.Item.IsDeleted == false).Select(item => new TrsItemModel
                    {
                        Id = item.ID,
                        AnswerId = item.AnswerId.Value,
                        ItemId = item.ItemId,
                        AssessmentItemId = item.ID,
                        ClassId = item.ClassId,
                        Comments = item.Comments,
                        Category = item.Item.Category,
                        LCAA = item.Item.LCAA,
                        LCCH = item.Item.LCCH,
                        LCSA = item.Item.LCSA,
                        RCCH = item.Item.RCCH,
                        SubCategorySort = item.Item.SubCategory.Sort,
                        Sort = item.Item.Sort,
                        SubCategoryId = item.Item.SubCategoryId,
                        SyncAnswer = item.Item.SyncAnswer,
                        ShowByInfants = item.Item.ShowByInfants,
                        ShowByPreschool = item.Item.ShowByInfants,
                        ShowBySchoolAge = item.Item.ShowBySchoolAge,
                        ShowByToddlers = item.Item.ShowByToddlers
                    })
                }).OrderByDescending(x => x.Id).ToList();
            if (assessmentModels != null && assessmentModels.Count > 0)
            {
                List<TrsItemModel> itemModels = _trsContract.Items.Where(i => i.IsDeleted == false)
                    .Select(item => new TrsItemModel
                    {
                        Id = 0,
                        ItemId = item.ID,
                        Category = item.Category,
                        LCAA = item.LCAA,
                        LCCH = item.LCCH,
                        LCSA = item.LCSA,
                        RCCH = item.RCCH,
                        SubCategorySort = item.SubCategory.Sort,
                        Sort = item.Sort,
                        SubCategoryId = item.SubCategoryId,
                        SyncAnswer = item.SyncAnswer,
                        ShowByInfants = item.ShowByInfants,
                        ShowByPreschool = item.ShowByInfants,
                        ShowBySchoolAge = item.ShowBySchoolAge,
                        ShowByToddlers = item.ShowByToddlers
                    }).ToList();
                Dictionary<int, TrsSubcategoryModel> subCategories = _trsContract.Subcategories.Select(x => new TrsSubcategoryModel()
                {
                    Category = x.Category,
                    Id = x.ID,
                    Name = x.Name,
                    Sort = x.Sort,
                    Type = x.Type
                }).ToDictionary(x => x.Id, x => x);

                foreach (TrsAssessmentModel item in assessmentModels)
                {
                    List<TrsClassModel> classes = new List<TrsClassModel>();
                    foreach (TrsClassModel item_class in school.Classes)
                    {
                        TrsClassModel classModel = new TrsClassModel();
                        classModel.Id = item_class.Id;
                        classModel.Name = item_class.Name;
                        classModel.TypeOfClass = item_class.TypeOfClass;
                        classModel.PlaygroundId = item_class.PlaygroundId;
                        classModel.TrsAssessorId = item_class.TrsAssessorId;
                        classModel.TrsMentorId = item_class.TrsMentorId;
                        classModel.Teachers = item_class.Teachers;
                        classModel.Ages = item_class.Ages;
                        classes.Add(classModel);
                    }
                    var items = item.Items.ToList();
                    items.AddRange(itemModels);
                    item.Items = items;
                    item.School = school;
                    item.Classes = classes;
                    item.SubCategory = subCategories;
                    item.OfflinePrepareWithAccess(user);

                    var list_Classes = item.AssessmentClasses;
                    foreach (var item_class in item.Classes)
                    {
                        var classModel = list_Classes.Where(r => r.ClassId == item_class.Id).FirstOrDefault();
                        if (classModel != null)
                            item_class.ObservationLength = classModel.ObservationLength;
                    }
                }
            }
            return assessmentModels;
        }

        public TrsAssessmentModel GetNewOfflineAssessmentModel(TrsOfflineSchoolModel school, UserBaseEntity user)
        {
            TrsAssessmentModel assessmentModel = new TrsAssessmentModel();
            if (school.IsCommunityTRS)
            {
                List<TrsItemModel> itemModels = _trsContract.Items.Where(i => i.IsDeleted == false)
                    .Select(item => new TrsItemModel
                    {
                        Id = 0,
                        ItemId = item.ID,
                        Category = item.Category,
                        LCAA = item.LCAA,
                        LCCH = item.LCCH,
                        LCSA = item.LCSA,
                        RCCH = item.RCCH,
                        SubCategorySort = item.SubCategory.Sort,
                        Sort = item.Sort,
                        SubCategoryId = item.SubCategoryId,
                        SyncAnswer = item.SyncAnswer,
                        ShowByInfants = item.ShowByInfants,
                        ShowByPreschool = item.ShowByInfants,
                        ShowBySchoolAge = item.ShowBySchoolAge,
                        ShowByToddlers = item.ShowByToddlers
                    }).ToList();
                Dictionary<int, TrsSubcategoryModel> subCategories = _trsContract.Subcategories.Select(x => new TrsSubcategoryModel()
                {
                    Category = x.Category,
                    Id = x.ID,
                    Name = x.Name,
                    Sort = x.Sort,
                    Type = x.Type
                }).ToDictionary(x => x.Id, x => x);

                assessmentModel.Items = itemModels;
                assessmentModel.School = school;
                assessmentModel.Classes = trsClassBusiness.GetTrsClasses(user, school.ID);
                assessmentModel.SubCategory = subCategories;
                assessmentModel.OfflinePrepareWithAccess(user);
                assessmentModel.UpdateAction(user);
            }
            return assessmentModel;
        }


        public object GetOfflineAssessment()
        {
            var Assessment = new
            {
                Categories = TRSCategoryEnum.Category1.ToList(),
                SubCategories = _trsContract.Subcategories.Select(x => new
                {
                    Category = x.Category,
                    Id = x.ID,
                    Name = x.Name,
                    Sort = x.Sort,
                    Type = x.Type
                }),
                Items = _trsContract.Items.Where(x => x.IsDeleted == false).Select(item => new
                {
                    AnswerId = 0,
                    Answers = item.ItemAnswers.Select(answer => new TrsAnswerModel()
                    {
                        Id = answer.AnswerId,
                        Text = answer.Answer.Text,
                        Score = answer.Answer.Score
                    }),
                    AnswerText = "",
                    Category = item.Category,
                    Description = item.Description,
                    KeyBehavior = item.KeyBehavior,
                    Filled = false,
                    Filter = item.Filter,
                    IsRequired = item.IsRequired,
                    Item = item.Item,
                    ItemId = item.ID,
                    AssessmentItemId = 0,
                    LinkingDocument = item.LinkingDocument,
                    Score = -1,
                    ShowByInfants = item.ShowByInfants,
                    ShowByToddlers = item.ShowByToddlers,
                    ShowByPreschool = item.ShowByPreschool,
                    ShowBySchoolAge = item.ShowBySchoolAge,
                    LCSA = item.LCSA,
                    LCCH = item.LCCH,
                    RCCH = item.RCCH,
                    LCAA = item.LCAA,
                    SubCategorySort = item.SubCategory.Sort,
                    Sort = item.Sort,
                    SubCategoryId = item.SubCategoryId,
                    SubCategoryType = item.SubCategory.Type,
                    TAItemInstructions = item.TAItemInstructions,
                    TAPlanItem = item.TAPlanItem,
                    TAPlanItemType = item.TAPlanItemType,
                    SyncAnswer = item.SyncAnswer,
                    Text = item.Text,
                    Type = item.Type
                }),
                Answers = _trsContract.Answers.Select(answer => new
                {
                    Id = answer.ID,
                    Text = answer.Text,
                    Score = answer.Score
                })
            };
            return Assessment;
        }


        public List<TrsAssessmentModel> GetLatestAssessmentBySchools(List<int> schoolIds)
        {
            if (schoolIds != null && schoolIds.Count > 0)
            {
                List<TrsAssessmentModel> list = _trsContract.Assessments.Where(r => r.Status == TRSStatusEnum.Completed && r.IsDeleted == false)
               .Select(r => new TrsAssessmentModel
               {
                   Id = r.ID,
                   SchoolId = r.SchoolId,
                   Star = r.Star,
                   VerifiedStar = r.VerifiedStar,
                   RecertificatedBy = r.RecertificatedBy
               }).ToList();
                return list;
            }
            else
            {
                return new List<TrsAssessmentModel>();
            }
        }


        #endregion
    }
}
