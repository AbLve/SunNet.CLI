using System.Linq.Expressions;
using Sunnet.Cli.Business.Trs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/1/15 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2015/1/15 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Business.Trs
{
    public partial class TrsBusiness
    {
        private static bool ShowNAItem = false;
        private static Expression<Func<TRSAssessmentEntity, TrsAssessmentReportModel>> AssessmentEntityToReportModel
        {
            get
            {
                return x => new TrsAssessmentReportModel()
                {
                    Id = x.ID,
                    Star = x.Star,
                    VerifiedStar = x.VerifiedStar,
                    Status = x.Status,
                    Type = x.Type,
                    EventLogType = x.EventLogType,
                    ApproveDate = x.ApproveDate,
                    DiscussDate = x.DiscussDate,
                    VisitDate = x.VisitDate,
                    SchoolId = x.SchoolId,
                    TaStatusString = x.TAStatus,
                    UpdatedOn = x.UpdatedOn,

                    Items = x.AssessmentItems.Where(i => i.AnswerId > 0 &&
                        i.Answer != null && i.AnswerId.Value > 0 && (i.Answer.Score >= 0 || ShowNAItem))
                        .Select(item => new TrsItemModel()
                        {
                            Id = item.ID,
                            AnswerId = item.AnswerId.Value,
                            AnswerText = item.Answer.Text,
                            Category = item.Item.Category,
                            Description = item.Item.Description,
                            KeyBehavior = item.Item.KeyBehavior,
                            Filled = true,
                            Filter = item.Item.Filter,
                            ItemId = item.ItemId,
                            AssessmentItemId = item.ID,
                            IsRequired = item.Item.IsRequired,
                            Item = item.Item.Item,
                            LinkingDocument = item.Item.LinkingDocument,
                            SubCategorySort = item.Item.SubCategory.Sort,
                            Sort = item.Item.Sort,
                            Score = item.Answer.Score,
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
        /// 是否可以下载报表.
        /// 如果报表不可用将返回0, 否则返回Assessment的Update时间(即上次报表的生成时间, 用于输出已存在报表)
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public long ReportAvailable(int id)
        {
            return _trsContract.Assessments.Where(x => x.ID == id && x.Status == TRSStatusEnum.Completed)
                .Select(x => x.UpdatedOn).FirstOrDefault().Ticks;
        }

        public TrsAssessmentReportModel GetReportModel(int id, UserBaseEntity user, bool showNAItem = false)
        {
            ShowNAItem = showNAItem;
            var assessmentModel =
                _trsContract.Assessments.Where(x => x.ID == id)
                .Select(AssessmentEntityToReportModel).FirstOrDefault();
            if (assessmentModel == null)
            {
                return null;
            }
            var schoolModel = SchoolBusiness.GetTrsSchool(assessmentModel.SchoolId, user);
            if (schoolModel == null)
                return null;
            assessmentModel.School = schoolModel;
            assessmentModel.Classes = trsClassBusiness.GetTrsClasses(user, assessmentModel.SchoolId);
            assessmentModel.SubCategory = _trsContract.Subcategories.Select(x => new TrsSubcategoryModel()
            {
                Category = x.Category,
                Id = x.ID,
                Name = x.Name,
                Sort = x.Sort,
                Type = x.Type
            }).ToDictionary(x => x.Id, x => x);
            assessmentModel.StarOfCategory = _trsContract.Stars.Where(
                s => s.AssessmentId == assessmentModel.Id && s.ClassId == 0).ToList()
                .ToDictionary(s => s.Category, s => s.Star);
            assessmentModel.Prepare(showNAItem);
            return assessmentModel;
        }

        public TrsAssessmentModel GetPreviewModel(int id, UserBaseEntity user, bool calcStar = true)
        {
            var model = GetAssessmentModel(id, user);
            if (model == null) return null;
            if (calcStar)
                model.UpdateStar();
            return model;
        }

    }
}
