using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Tsds.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Tsds;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Business.Tsds.AssessmentXml;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using System.Xml;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Business.Tsds
{
    public class TsdsBusiness2
    {
        private readonly ITsdsContract _tsdsContract;
        private readonly AdeBusiness _adeBusiness;
        private readonly CpallsBusiness _cpallsBusiness;
        private readonly SchoolBusiness _schoolBusiness;

        public TsdsBusiness2(AdeUnitOfWorkContext unit = null)
        {
            _tsdsContract = DomainFacade.CreateTsdsService(unit);
            _adeBusiness = new AdeBusiness(unit);
            _cpallsBusiness = new CpallsBusiness(unit);
            _schoolBusiness = new SchoolBusiness();
        }

        #region TsdsAssessmentFiles

        public byte[] CreateTsdsAssessmentFile(int assessmentId, UserBaseEntity userInfo, out string fileName)
        {
            AssessmentXmlModel assessmentXmlModel = new AssessmentXmlModel();
            assessmentXmlModel.id = assessmentId;
            List<MeasureXmlModel> measureXmlModels = new List<MeasureXmlModel>();
            List<ItemXmlModel> itemXmlModels = new List<ItemXmlModel>();
            AssessmentEntity assessment = _adeBusiness.GetAssessment(assessmentId);
            foreach (MeasureEntity measure in assessment.Measures)
            {
                MeasureXmlModel measureXml = MeasureToXmlModel(measure);

                measureXmlModels.Add(measureXml);
                foreach (ItemBaseEntity item in measure.Items)
                {
                    ItemXmlModel itemXml = new ItemXmlModel();
                    itemXml.ItemId = item.ID.ToString();
                    itemXml.IdentificationCode = item.ID.ToString();
                    itemXml.ItemCategory = measure.Name;
                    itemXml.MaxRawScore = item.Score.ToString();
                    itemXml.CorrectResponse = string.Join(",", item.Answers.Select(x => x.Value).ToList());
                    itemXml.ContentStandardName = "";
                    itemXml.LearningStandardIdentificationCode = "";
                    itemXmlModels.Add(itemXml);
                }
            }
            assessmentXmlModel.Measures = measureXmlModels;
            assessmentXmlModel.Items = itemXmlModels;
            var xmlFile = CreateAssessmentXml(assessmentXmlModel);
            fileName = "Assessment_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            TsdsAssessmentFileEntity entity = new TsdsAssessmentFileEntity();
            entity.AssessmentId = assessmentId;
            entity.Status = (byte)1;
            entity.FileName = fileName;
            entity.FileSize = "";
            entity.DownloadUrl = "";
            entity.CreatedBy = userInfo.ID;
            _tsdsContract.InsertTsdsAssessmentFile(entity);
            return xmlFile;
        }


        private MeasureXmlModel MeasureToXmlModel(MeasureEntity measure)
        {
            MeasureXmlModel measureXml = new MeasureXmlModel();
            measureXml.AssessmentID = measure.ID.ToString();
            measureXml.AssessmentTitle = measure.Name;
            measureXml.AssessmentIdentificationSystem = "cliengage.org";
            measureXml.AssessmentIdentificationID = measure.ID.ToString();
            measureXml.AssessmentCategory = measure.Parent.Name;
            measureXml.AcademicSubject = measure.Parent.Name;
            measureXml.GradeLevelAssessed = "";
            measureXml.PerformanceDescription = "";
            measureXml.AssessmentReportingMethod = "";
            measureXml.ContentStandard = "";
            measureXml.Version = "";
            measureXml.RevisionDate = measure.UpdatedOn.ToShortDateString();
            measureXml.MaxRawScore = measure.TotalScore.ToString();
            measureXml.AssessmentPeriodDescription = "";
            measureXml.AssessmentItemReference = measure.Items.Select(x => x.ID.ToString()).ToList();
            return measureXml;
        }

        public byte[] CreateAssessmentXml(AssessmentXmlModel model)
        {


            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder xmlAssessmentNode = new StringBuilder();
            xmlAssessmentNode.Append("<?xml version='1.0' encoding='utf-8'?>");
            xmlAssessmentNode.Append("<InterchangeAssessmentMetadata xsi:schemaLocation='http://www.tea.state.tx.us/tsds InterchangeAssessmentMetadata.xsd' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://www.tea.state.tx.us/tsds'>");
            foreach (MeasureXmlModel measure in model.Measures)
            {
                xmlAssessmentNode.AppendFormat("<Assessment id='{0}'>", measure.AssessmentID);

                xmlAssessmentNode.AppendFormat("<AssessmentTitle>{0}</AssessmentTitle>", measure.AssessmentTitle);

                xmlAssessmentNode.AppendFormat("<AssessmentIdentificationCode IdentificationSystem='{0}'>", measure.AssessmentIdentificationSystem);
                xmlAssessmentNode.AppendFormat("<ID>{0}</ID>", measure.AssessmentID);
                xmlAssessmentNode.Append("</AssessmentIdentificationCode>");

                xmlAssessmentNode.AppendFormat("<AssessmentCategory>{0}</AssessmentCategory>", measure.AssessmentCategory);
                xmlAssessmentNode.AppendFormat("<AcademicSubject>{0}</AcademicSubject>", measure.AcademicSubject);
                xmlAssessmentNode.AppendFormat("<GradeLevelAssessed>{0}</GradeLevelAssessed>", measure.GradeLevelAssessed);

                xmlAssessmentNode.Append("<AssessmentPerformanceLevel>");
                xmlAssessmentNode.Append("<PerformanceLevel>");
                xmlAssessmentNode.AppendFormat("<Description>{0}</Description>", measure.PerformanceDescription);
                xmlAssessmentNode.Append("</PerformanceLevel>");
                xmlAssessmentNode.AppendFormat("<AssessmentReportingMethod>{0}</AssessmentReportingMethod>", measure.AssessmentReportingMethod);
                xmlAssessmentNode.Append("</AssessmentPerformanceLevel>");

                xmlAssessmentNode.AppendFormat("<ContentStandard>{0}</ContentStandard>", measure.ContentStandard);
                xmlAssessmentNode.AppendFormat("<Version>{0}</Version>", measure.Version);
                xmlAssessmentNode.AppendFormat("<RevisionDate>{0}</RevisionDate>", measure.RevisionDate);
                xmlAssessmentNode.AppendFormat("<MaxRawScore>{0}</MaxRawScore>", measure.MaxRawScore);

                xmlAssessmentNode.Append("<AssessmentPeriod>");
                xmlAssessmentNode.AppendFormat("<Description>{0}</Description>", measure.AssessmentPeriodDescription);
                xmlAssessmentNode.Append("</AssessmentPeriod>");

                foreach (string itemId in measure.AssessmentItemReference)
                {
                    xmlAssessmentNode.AppendFormat("<AssessmentItemReference ref='{0}'/>", itemId);
                }

                xmlAssessmentNode.Append("</Assessment>");
            }

            foreach (ItemXmlModel item in model.Items)
            {
                xmlAssessmentNode.AppendFormat("<AssessmentItem id='{0}'>", item.ItemId);

                xmlAssessmentNode.AppendFormat("<IdentificationCode>{0}</IdentificationCode>", item.IdentificationCode);
                xmlAssessmentNode.AppendFormat("<ItemCategory>{0}</ItemCategory>", item.ItemCategory);
                xmlAssessmentNode.AppendFormat("<MaxRawScore>{0}</MaxRawScore>", item.MaxRawScore);
                xmlAssessmentNode.AppendFormat("<CorrectResponse>{0}</CorrectResponse>", item.CorrectResponse);

                xmlAssessmentNode.Append("<LearningStandardReference>");
                xmlAssessmentNode.Append("<LearningStandardIdentity>");
                xmlAssessmentNode.AppendFormat("<LearningStandardId ContentStandardName='{0}'>", item.ContentStandardName);
                xmlAssessmentNode.AppendFormat("<IdentificationCode>{0}</IdentificationCode>", item.LearningStandardIdentificationCode);
                xmlAssessmentNode.Append("</LearningStandardId>");
                xmlAssessmentNode.Append("</LearningStandardIdentity>");
                xmlAssessmentNode.Append("</LearningStandardReference>");

                xmlAssessmentNode.Append("</AssessmentItem>");
            }
            xmlAssessmentNode.Append("</InterchangeAssessmentMetadata>");

            xmlDoc.LoadXml(xmlAssessmentNode.ToString());
 
            Stream stream = new MemoryStream();
            xmlDoc.Save(stream);
            byte[] bytes = new byte[(int)stream.Length];
            stream.Seek(0, SeekOrigin.Begin); 
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();

            return bytes;

        }
        #endregion

        #region Tsds Student Assessment xml

        #endregion

        #region TSDS

        public OperationResult InsertTSDS(List<TsdsEntity> tsds)
        {
            return _tsdsContract.InsertTsds(tsds);
        }

        public TsdsEntity GetTsds(int id)
        {
            return _tsdsContract.GetTsds(id);
        }

        public List<DownloadListModel> SearchTsdses(UserBaseEntity user, Expression<Func<TsdsEntity, bool>> condition,
     string sort, string order, int first, int count, out int total)
        {
            var userSchoolList = _schoolBusiness.SearchSchoolIdsByUserIds(user);//TODO:
            var query = _tsdsContract.Tsdses.AsExpandable().Where(condition).Select(o => new DownloadListModel
            {
                ID = o.ID,
                CommunityName = o.Community == null ? "" : o.Community.Name,
                SchoolName = o.SchoolNames,
                FileName = o.FileName,
                DownloadedOn = o.DownloadOn,
                DownloadedBy = o.DownloadUser == null ? "" : o.DownloadUser.FirstName + " " + o.DownloadUser.LastName,
                Status = o.Status
            });
            total = query.Count();
            var queryList = query.OrderBy(sort, order).Skip(first).Take(count);
            var list = queryList.ToList();
            return list;
        }

        public List<SelectItemModel> GetDownLoadUsers()
        {
            return _tsdsContract.Tsdses.Select(c => new SelectItemModel()
            {
                ID = c.DownloadBy,
                Name = c.DownloadUser.FirstName + " " + c.DownloadUser.LastName
            }).Distinct().ToList();
        }

        #endregion
    }
}
