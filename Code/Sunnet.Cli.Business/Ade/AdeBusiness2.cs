using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using LinqKit;
using Sunnet.Cli.Core.Cpalls;

namespace Sunnet.Cli.Business.Ade
{
    public partial class AdeBusiness
    {
        public List<AdeLinkEntity> GetLinks<T>(params int[] ids) where T : IAdeLinkProperties
        {
            var type = typeof(T).ToString();
            return _adeContract.AdeLinks.Where(x => ids.Contains(x.HostId) && x.HostType == type)
                .OrderBy(x => x.Link)
                .ToList();
        }

        public List<AdeLinkEntity> GetLinks<T>(Wave wave, params int[] ids) where T : IAdeLinkProperties
        {
            bool selectWave1 = false;
            bool selectWave2 = false;
            bool selectWave3 = false;
            switch (wave)
            {
                case Wave.BOY:
                    selectWave1 = true;
                    break;
                case Wave.MOY:
                    selectWave2 = true;
                    break;
                case Wave.EOY:
                    selectWave3 = true;
                    break;
            }
            var type = typeof(T).ToString();
            return _adeContract.AdeLinks.Where(x => ids.Contains(x.HostId) && x.HostType == type
                && ((selectWave1 && x.MeasureWave1) || (selectWave2 && x.MeasureWave2) || (selectWave3 && x.MeasureWave3)))
                .OrderBy(x => x.Link)
                .ToList();
        }

        public List<AdeLinkModel> GetLinkModels<T>(params int[] ids) where T : IAdeLinkProperties
        {
            var type = typeof(T).ToString();
            return _adeContract.AdeLinks.Where(x => ids.Contains(x.HostId) && x.HostType == type)
                .OrderBy(x => x.Link).Select(linkEntity => new AdeLinkModel()
                {
                    DisplayText = linkEntity.DisplayText,
                    HostId = linkEntity.HostId,
                    HostType = linkEntity.HostType,
                    ID = linkEntity.ID,
                    Link = linkEntity.Link,
                    LinkType = linkEntity.LinkType
                }).ToList();
        }

        /// <summary>
        /// 获取所有活动的cpalls
        /// </summary>
        /// <returns></returns>
        public List<CpallsAssessmentModel> GetAssessmentCpallsList()
        {
            return _adeContract.Assessments.Where(r => r.Type == AssessmentType.Cpalls && r.Status == EntityStatus.Active && r.IsDeleted == false)
                 .Select(r => new CpallsAssessmentModel()
             {
                 ID = r.ID,
                 Name = r.Name,
                 Language = r.Language,
                 Description = r.Description
             }).ToList();
        }

        /// <summary>
        /// 获取满足条件的Assessment
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<CpallsAssessmentModel> GetAssessmentList(Expression<Func<AssessmentEntity, bool>> expression)
        {
            return _adeContract.Assessments.Where(r => r.Status == EntityStatus.Active && r.IsDeleted == false)
                 .Where(expression)
                 .Select(r => new CpallsAssessmentModel()
                 {
                     ID = r.ID,
                     Name = r.Name,
                     Language = r.Language,
                     Description = r.Description
                 }).ToList();
        }


        /// <summary>
        /// 获取满足条件的Assessment
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public List<CpallsAssessmentModel> GetAssessmentFeatureList(Expression<Func<AssessmentEntity, bool>> expression)
        {
            List<CpallsAssessmentModel> resultList = new List<CpallsAssessmentModel>();
            List<CpallsAssessmentModel> list = _adeContract.Assessments.Where(r => r.Status == EntityStatus.Active && r.IsDeleted == false)
                 .Where(expression)
                 .Select(r => new CpallsAssessmentModel()
                 {
                     ID = r.ID,
                     Name = r.Name,
                     Language = r.Language,
                     Type = r.Type
                 }).ToList();
            var nameList = list.Select(o => o.Name).Distinct().ToList<string>();

            foreach (string name in nameList)
            {

                var firsTFeature = list.OrderBy(o => o.ID).FirstOrDefault(o => o.Name == name);
                if (firsTFeature != null)
                {
                    resultList.Add(firsTFeature);
                }
            }
            return resultList;
        }

        /// <summary>
        /// Gets the active measures for report.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        public List<MeasureReportModel> GetMeasureReport(int assessmentId)
        {

            return _adeContract.Measures.Where(r => r.AssessmentId == assessmentId
                && r.Status == EntityStatus.Active && r.IsDeleted == false)
                .Select(o => new MeasureReportModel()
            {
                ID = o.ID,
                Name = o.Name,
                ShortName = o.ShortName,
                ParentId = o.ParentId,
                Sort = o.Sort,
                ApplyToWave = o.ApplyToWave,
                RelatedMeasureId = o.RelatedMeasureId
            }).ToList();
        }


        /// <summary>
        /// Gets all measures for report.
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <returns></returns>
        public List<MeasureReportModel> GetAllMeasureReport(int assessmentId)
        {


            return _adeContract.Measures.Where(r => r.AssessmentId == assessmentId
                && r.IsDeleted == false)
                .Select(o => new MeasureReportModel()
                {
                    ID = o.ID,
                    Name = o.Name,
                    ShortName = o.ShortName,
                    ParentId = o.ParentId,
                    Sort = o.Sort,
                    ApplyToWave = o.ApplyToWave,
                    RelatedMeasureId = o.RelatedMeasureId
                }).ToList();
        }

        public List<AssessmentModel> GetAssessmentSelectList(Expression<Func<AssessmentEntity, bool>> condition)
        {
            return _adeContract.Assessments.AsExpandable().Where(r => r.IsDeleted == false)
                .Where(condition).Select(r => new AssessmentModel()
                {
                    ID = r.ID,
                    Label = r.Label,
                    Name = r.Name
                }).ToList();
        }

        public List<MeasureReportModel> GetMeasureSelectList(Expression<Func<MeasureEntity, bool>> condition)
        {
            return _adeContract.Measures.AsExpandable().Where(r => r.IsDeleted == false)
                .Where(condition).Select(r => new MeasureReportModel()
                {
                    ID = r.ID,
                    Name = r.Name,
                    ShortName = r.ShortName
                }).ToList();
        }
    }
}
