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
using Sunnet.Cli.Business.Trs.Models;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Trs.Enums;

namespace Sunnet.Cli.Business.Trs
{
    public partial class TrsBusiness
    {
        public TrsResultReportModel GetReportAssement(int assementId, UserBaseEntity user)
        {
            TrsResultReportModel model_Assessment = _trsContract.Assessments.Where(a => a.ID == assementId).Select(
                r => new TrsResultReportModel
                {
                    Id = r.ID,
                    SchoolId = r.SchoolId,
                    ApproveDate = r.ApproveDate,
                    RecertificatedBy = r.RecertificatedBy,
                    Star = r.Star,
                    VerifiedStar = r.VerifiedStar,
                    VisitDate = r.VisitDate,
                    DiscussDate = r.DiscussDate,
                    UpdatedOn = r.UpdatedOn,
                    Status = r.Status,
                    Type = r.Type,
                    EventLogType = r.EventLogType,
                    CategoryStars = r.Stars.Select(a => new TrsCategoryStarModel
                    {
                        AssessmentId = a.AssessmentId,
                        Category = a.Category,
                        Star = a.Star,
                        Retain = a.Retain,
                        AutoAssign = a.AutoAssign
                    }),
                    Items = r.AssessmentItems.Where(i => i.Item.IsDeleted == false).Where(a => a.ItemId > 0
                        && a.Answer != null && a.AnswerId.Value > 0)
                    .Select(item => new TrsItemReportModel
                    {
                        Id = item.ID,
                        AnswerId = item.AnswerId.Value,
                        Category = item.Item.Category,
                        ItemId = item.ItemId,
                        AssessmentItemId = item.ID,
                        Item = item.Item.Item,
                        SubCategorySort = item.Item.SubCategory.Sort,
                        Sort = item.Item.Sort,
                        Score = item.Answer.Score,
                        AnswerText = item.Answer.Text,
                        SubCategoryId = item.Item.SubCategoryId,
                        Text = item.Item.Text,
                        Description = item.Item.Description,
                        Type = item.Item.Type,
                        ClassId = item.ClassId,
                        ItemType = item.Item.Type,
                        SubcategoryType = item.Item.SubCategory.Type,
                        LCAA = item.Item.LCAA,
                        LCCH = item.Item.LCCH,
                        LCSA = item.Item.LCSA,
                        RCCH = item.Item.RCCH,
                        ShowByInfants = item.Item.ShowByInfants,
                        ShowByToddlers = item.Item.ShowByToddlers,
                        ShowByPreschool = item.Item.ShowByPreschool,
                        ShowBySchoolAge = item.Item.ShowBySchoolAge,
                        SyncAnswer = item.Item.SyncAnswer,
                        Comments = item.Comments,
                        AgeGroup = item.AgeGroup,
                        GroupSize = item.GroupSize,
                        CaregiversNo = item.CaregiversNo
                    })
                }).FirstOrDefault();

            if (model_Assessment != null)
            {
                if (model_Assessment.SchoolId > 0)
                {
                    model_Assessment.School = SchoolBusiness.GetTrsSchoolReport(user, model_Assessment.SchoolId);

                    string minAgeGroup = "";//最小年龄段
                    string maxAgeGroup = "";//最大年龄段
                    model_Assessment.Classes = trsClassBusiness.GetTrsClassesReport(user, model_Assessment.SchoolId, out minAgeGroup, out maxAgeGroup);
                    model_Assessment.MinAgeGroup = minAgeGroup;
                    model_Assessment.MaxAgeGroup = maxAgeGroup;
                }

                model_Assessment.SubCategory = _trsContract.Subcategories.Select(x => new TrsSubcategoryModel()
                {
                    Category = x.Category,
                    Id = x.ID,
                    Name = x.Name,
                    Sort = x.Sort,
                    Type = x.Type
                }).ToDictionary(x => x.Id, x => x);

                model_Assessment.Prepare(true);
            }
            return model_Assessment;
        }

        public TrsResultReportModel GetCommentReportAssement(int assementId, UserBaseEntity user)
        {
            var trsAssessment = _trsContract.GetAssessment(assementId);
            List<int> itemIds = trsAssessment.AssessmentItems
                .Where(i => i.Item.IsDeleted == false && i.ItemId > 0 && i.Comments != "")
                .Select(i => i.ItemId).Distinct().ToList();
            TrsResultReportModel model_Assessment = _trsContract.Assessments.Where(a => a.ID == assementId).Select(
                r => new TrsResultReportModel
                {
                    Id = r.ID,
                    SchoolId = r.SchoolId,
                    ApproveDate = r.ApproveDate,
                    RecertificatedBy = r.RecertificatedBy,
                    Star = r.Star,
                    VerifiedStar = r.VerifiedStar,
                    VisitDate = r.VisitDate,
                    DiscussDate = r.DiscussDate,
                    UpdatedOn = r.UpdatedOn,
                    Status = r.Status,
                    Type = r.Type,
                    EventLogType = r.EventLogType,
                    CategoryStars = r.Stars.Select(a => new TrsCategoryStarModel
                    {
                        AssessmentId = a.AssessmentId,
                        Category = a.Category,
                        Star = a.Star,
                        Retain = a.Retain,
                        AutoAssign = a.AutoAssign
                    }),
                    Items = r.AssessmentItems.Where(i => itemIds.Contains(i.ItemId)).Where(a => a.ItemId > 0
                        && a.Answer != null && a.AnswerId.Value > 0)
                    .Select(item => new TrsItemReportModel
                    {
                        Id = item.ID,
                        AnswerId = item.AnswerId.Value,
                        Category = item.Item.Category,
                        ItemId = item.ItemId,
                        AssessmentItemId = item.ID,
                        Item = item.Item.Item,
                        SubCategorySort = item.Item.SubCategory.Sort,
                        Sort = item.Item.Sort,
                        Score = item.Answer.Score,
                        AnswerText = item.Answer.Text,
                        SubCategoryId = item.Item.SubCategoryId,
                        Text = item.Item.Text,
                        Description = item.Item.Description,
                        Type = item.Item.Type,
                        ClassId = item.ClassId,
                        ItemType = item.Item.Type,
                        SubcategoryType = item.Item.SubCategory.Type,
                        LCAA = item.Item.LCAA,
                        LCCH = item.Item.LCCH,
                        LCSA = item.Item.LCSA,
                        RCCH = item.Item.RCCH,
                        ShowByInfants = item.Item.ShowByInfants,
                        ShowByToddlers = item.Item.ShowByToddlers,
                        ShowByPreschool = item.Item.ShowByPreschool,
                        ShowBySchoolAge = item.Item.ShowBySchoolAge,
                        SyncAnswer = item.Item.SyncAnswer,
                        Comments = item.Comments,
                        AgeGroup = item.AgeGroup,
                        GroupSize = item.GroupSize,
                        CaregiversNo = item.CaregiversNo
                    })
                }).FirstOrDefault();

            if (model_Assessment != null)
            {
                if (model_Assessment.SchoolId > 0)
                {
                    model_Assessment.School = SchoolBusiness.GetTrsSchoolReport(user, model_Assessment.SchoolId);

                    string minAgeGroup = "";//最小年龄段
                    string maxAgeGroup = "";//最大年龄段
                    model_Assessment.Classes = trsClassBusiness.GetTrsClassesReport(user, model_Assessment.SchoolId, out minAgeGroup, out maxAgeGroup);
                    model_Assessment.MinAgeGroup = minAgeGroup;
                    model_Assessment.MaxAgeGroup = maxAgeGroup;
                }

                model_Assessment.SubCategory = _trsContract.Subcategories.Select(x => new TrsSubcategoryModel()
                {
                    Category = x.Category,
                    Id = x.ID,
                    Name = x.Name,
                    Sort = x.Sort,
                    Type = x.Type
                }).ToDictionary(x => x.Id, x => x);

                model_Assessment.Prepare(true);
            }
            return model_Assessment;
        }
    }
}
