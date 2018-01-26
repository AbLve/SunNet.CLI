using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/12/1 11:20:00
 * Description:		CecBusiness
 * Version History:	Created,2014/12/1 11:20:00
 * 
 * 
 **************************************************************************/
using LinqKit;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Cec;
using Sunnet.Cli.Core.Cec.Entities;
using Sunnet.Cli.Core.Cec.Interfaces;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Cec.Model;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Cec.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls;
using System.Data.Objects.SqlClient;
using Sunnet.Framework.Core.Extensions;

namespace Sunnet.Cli.Business.Cec
{
    public class CecBusiness
    {
        private readonly ICecContract _cecContract;
        private IAdeDataContract _adeContract;
        private readonly AdeBusiness _adeBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly CpallsBusiness _cpallsBusiness;
        public CecBusiness(AdeUnitOfWorkContext unit = null)
        {
            _adeContract = DomainFacade.CreateAdeDataService(unit);
            _cecContract = DomainFacade.CreateCecServer(unit);
            _adeBusiness = new AdeBusiness(unit);
            _userBusiness = new UserBusiness();
            _cpallsBusiness = new CpallsBusiness(unit);
        }


        private readonly Func<CECTeacherModel, CecSchoolTeacherModel> cecTeacherSelector = x => new CecSchoolTeacherModel()
        {
            CLIFundingId = x.CLIFundingId,
            CoachFirstName = x.CoachFirstName,
            CoachId = x.CoachId,
            CoachLastName = x.CoachLastName,
            FirstName = x.FirstName,
            ID = x.ID,
            LastName = x.LastName,
            Status = x.Status,
            TeacherID = x.TeacherID,
            TeacherType = x.TeacherType,
            UserID = x.UserID,
            YearsInProject = x.YearsInProject,
            YearsInProjectId = x.YearsInProjectId,

            SchoolYear = x.SchoolYear,
            BOY = x.BOY,
            MOY = x.MOY,
            EOY = x.EOY
        };

        public List<CecSchoolTeacherModel> GetCECTeacherList(int assessmentId, int year, int? coachId, List<int> communities, List<int> schoolIds,
            string firstname, string lastname, string teacherId,
            string sort, string order, int first, int count, out int total)
        {
            List<CECTeacherModel> teachers = _cecContract.GetCECTeacherList(assessmentId, year.ToSchoolYearString(), coachId, communities,
                schoolIds, firstname, lastname, teacherId,
                sort, order, first, count, out total);
            if (teachers != null)
            {
                var teacherUserIds = teachers.Select(x => x.UserID).ToArray();
                var teacherCommuntiyNameList = _userBusiness.GetTeacherCommunityName(teacherUserIds);
                var teacherSchoolNameList = _userBusiness.GetTeacherSchoolName(teacherUserIds);

                var teacherWithSch = teachers.Select(cecTeacherSelector).ToList();

                teacherWithSch.ForEach(teacher =>
                {
                    if (teacherCommuntiyNameList.ContainsKey(teacher.UserID))
                        teacher.CommunityNames = teacherCommuntiyNameList[teacher.UserID].Select(r => r.CommunityName).ToList();
                    if (teacherSchoolNameList.ContainsKey(teacher.UserID))
                        teacher.SchoolNames = teacherSchoolNameList[teacher.UserID].Select(r => r.SchoolName).ToList();
                });

                return teacherWithSch;
            }
            return new List<CecSchoolTeacherModel>();
        }

        public List<CecItemModel> GetCecItemModel(int assessmentId, Wave wave)
        {
            string tmpWave = ((byte)wave).ToString();
            return _adeContract.Items.OfType<CecItemEntity>().Where(o => o.Measure.AssessmentId == assessmentId && o.Measure.ApplyToWave.Contains(tmpWave)
                && o.Status == EntityStatus.Active && o.IsDeleted == false)
                .OrderBy("Measure.Sort", "asc").OrderBy("Sort", "asc")
                .Select(o => new CecItemModel
                {
                    ItemId = o.ID,
                    Answer = o.Answers.Select(a => new CecAnswerModel()
                    {
                        AnswerId = a.ID,
                        Score = a.Score,
                        Text = a.Text
                    }),
                    Sort = o.Sort,
                    MeasureName = o.Measure.Name,
                    MeasureId = o.MeasureId,
                    Wave = o.Measure.ApplyToWave,
                    Description = o.TargetText,
                    Area = o.Description,
                    IsRequired = o.IsRequired
                }).ToList();
        }

        public List<CecAnswerModel> GetCecAnswerList(int assessmentId)
        {
            return _adeContract.Answers.Where(r => r.Item.Measure.AssessmentId == assessmentId)
                .Select(r => new CecAnswerModel()
                {
                    AnswerId = r.ID,
                    Score = r.Score
                }).ToList();
        }


        public OperationResult InsertCecHistory(CecHistoryEntity entity)
        {

            //添加之前，计算总分，先获取ItemList
            List<CecItemModel> itemList = GetCecItemModel(entity.AssessmentId, entity.Wave);
            //再获取选择的Answer的得分
            if (entity.CecResults.Any())
            {
                decimal totalScore = 0;
                foreach (var item in entity.CecResults)
                {
                    CecItemModel itemTmp = itemList.FirstOrDefault(o => o.ItemId == item.ItemId);
                    if (itemTmp != null)
                    {
                        CecAnswerModel answerTmp = itemTmp.Answer.FirstOrDefault(o => o.AnswerId == item.AnswerId);
                        if (answerTmp != null)
                        {
                            totalScore += answerTmp.Score;
                        }
                    }
                }
                entity.TotalScore = totalScore;
            }

            return _cecContract.InsertCecHistory(entity);
        }

        public OperationResult InsertCecHistory(List<CecHistoryEntity> list)
        {
            return _cecContract.InsertCecHistory(list);
        }

        public List<CecResultModel> GetCecResultModels(Expression<Func<CecResultEntity, bool>> expression)
        {
            return _cecContract.CecResultEntities.AsExpandable().Where(expression).Select(ResultEntityToModel).ToList();
        }

        /// <summary>
        /// 获取满足条件的最新的第一个CecHistoryEntity
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public CecHistoryEntity GetCecHistoryEntity(Expression<Func<CecHistoryEntity, bool>> expression)
        {
            return _cecContract.CecHistoryEntities.Where(expression).OrderByDescending(r => r.AssessmentDate).FirstOrDefault();
        }

        /// <summary>
        /// 判断是否有过历史记录
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="wave"></param>
        /// <param name="teacherId"></param>
        /// <returns>有返回True ,无返回False</returns>
        public bool CheckCecHistory(int assessmentId, Wave wave, int teacherId)
        {
            return _cecContract.CecHistoryEntities.Count(r => r.AssessmentId == assessmentId && r.Wave == wave
                && r.TeacherId == teacherId && r.SchoolYear == CommonAgent.SchoolYear) > 0;
        }

        private Expression<Func<CecResultEntity, CecResultModel>> ResultEntityToModel
        {
            get
            {
                return o => new CecResultModel()
                {
                    ID = o.ID,
                    AssessmentDate = o.CecHistory.AssessmentDate,
                    AnswerId = o.AnswerId,
                    CecHistoryId = o.CecHistoryId,
                    ItemId = o.ItemId,
                    Score = o.Score
                };
            }
        }

        public OperationResult Reset(int assessmentId, int teacherId, Wave wave)
        {
            return _cecContract.Reset(assessmentId, teacherId, wave, CommonAgent.SchoolYear);
        }

        public CecReportModel GetCECReport(int historyId, int assessmentId, Wave wave)
        {
            CecReportModel cecReportModel = new CecReportModel();

            //表头
            List<MeasureHeaderModel> MeasureList;
            List<MeasureHeaderModel> ParentMeasureList;

            _cpallsBusiness.BuilderHeader(assessmentId
                , CommonAgent.Year, (Wave)wave
                , out MeasureList, out ParentMeasureList);

            if (MeasureList != null && MeasureList.Count > 0)
            {
                List<int> tmpMeasureIds = MeasureList.Select(r => r.MeasureId).ToList();
                tmpMeasureIds.AddRange(ParentMeasureList.Select(r => r.MeasureId).ToList());
                int[] ids = tmpMeasureIds.Distinct().ToArray();
                List<AdeLinkEntity> measureLinks = _adeBusiness.GetLinks<MeasureEntity>(ids);

                // ViewBag.HaveMeasure = true;

                foreach (MeasureHeaderModel item in MeasureList)
                {
                    item.Links = new List<AdeLinkEntity>();
                    item.Links.AddRange(measureLinks.FindAll(r => r.HostId == item.MeasureId));
                }

                foreach (MeasureHeaderModel item in ParentMeasureList)
                {
                    item.Links = new List<AdeLinkEntity>();
                    item.Links.AddRange(measureLinks.FindAll(r => r.HostId == item.MeasureId));
                }

                ///获取CEC Items
                List<CecItemModel> itemList = GetCecItemModel(assessmentId, wave);

                //排除 没Items 的Measure
                List<int> measureIds = itemList.GroupBy(r => r.MeasureId).Select(r => r.Key).Distinct().ToList();
                List<MeasureHeaderModel> tmpHeader = MeasureList.Where(r => r.MeasureId != r.ParentId && measureIds.Contains(r.MeasureId)).ToList();
                cecReportModel.MeasureList = tmpHeader;
                cecReportModel.ParentMeasureList = ParentMeasureList.Where(r => measureIds.Contains(r.MeasureId) || tmpHeader.Select(t => t.ParentId).Contains(r.MeasureId)).ToList();

                //获取result 的分数
                List<CecResultModel> resultList = GetCecResultModels(r => r.CecHistoryId == historyId);

                if (resultList != null && resultList.Count > 0)
                {
                    foreach (CecResultModel item in resultList)
                    {
                        CecItemModel cecItem = itemList.Find(r => r.ItemId == item.ItemId);
                        if (cecItem != null)
                        {
                            CecAnswerModel tmpAnser = cecItem.Answer.FirstOrDefault(r => r.AnswerId == item.AnswerId);
                            if (tmpAnser != null)
                                cecItem.Score = tmpAnser.Score;
                        }
                    }
                }

                //获取Items 的Links
                ids = itemList.Select(r => r.ItemId).Distinct().ToArray();
                List<AdeLinkEntity> itemLinks = _adeBusiness.GetLinks<ItemBaseEntity>(ids);

                foreach (CecItemModel item in itemList)
                {
                    item.Links = new List<AdeLinkEntity>();
                    item.Links.AddRange(itemLinks.FindAll(r => r.HostId == item.ItemId));
                }
                cecReportModel.Items = itemList;

                return cecReportModel;
            }
            else
                return null;
        }

        #region VCW调用

        public List<CECTeacherModel_VCW> GetCecReports(int year, int teacherId)
        {
            string schoolYear = year.ToSchoolYearString();
            return _cecContract.CecHistoryEntities.Where(r => r.TeacherId == teacherId && r.SchoolYear == schoolYear)
                .Select(r => new CECTeacherModel_VCW
                {
                    AssessmentId = r.AssessmentId,
                    Wave = r.Wave,
                    AssessmentDate = r.AssessmentDate,
                    TeacherId = r.TeacherId
                }).ToList();
        }

        #endregion
    }
}
