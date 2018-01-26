using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqKit;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Tsds.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Tsds;
using Sunnet.Cli.Core.Tsds.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Business.Tsds.AssessmentXml;
using Sunnet.Framework.Extensions;
using System.Xml;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Communities.Entities;
using System.Text.RegularExpressions;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Tsds.StuParentExtXml;
using System.Web.Mvc;
using Sunnet.Framework.Helpers;

namespace Sunnet.Cli.Business.Tsds
{
    public class TsdsBusiness
    {
        private readonly ITsdsContract _tsdsContract;
        private readonly AdeBusiness _adeBusiness;
        private readonly CpallsBusiness _cpallsBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly StudentBusiness _studentBusiness;
        private readonly CommunityBusiness _communityBusiness;
        public IList<TsdsMapEntity> mapList { get; set; }

        public TsdsBusiness(AdeUnitOfWorkContext unit = null)
        {
            _tsdsContract = DomainFacade.CreateTsdsService(unit);
            _adeBusiness = new AdeBusiness(unit);
            _cpallsBusiness = new CpallsBusiness(unit);
            _schoolBusiness = new SchoolBusiness();
            _studentBusiness = new StudentBusiness();
            _communityBusiness = new CommunityBusiness();

            mapList = _tsdsContract.TsdsMaps.ToList();
        }
        #region David 06/20/2017 Hard code the measeure options
        public List<SelectListItem> GetAvailableMeasures(int assessmentId)
        {
            //Peter 12 / 12 / 2017 Update Measures-List from XML map
            //int sumcount = 0;
            //var xmlData = XmlHelper.ReadXML<SelectListItem>("~/Resources/MeasuresTemp/Measures.xml", "Measures", assessmentId + "", out sumcount);
            //if (sumcount == 0)
            //{
            //    var measureSelectList = new List<SelectListItem>();
            //    var sgroup1 = new SelectListGroup { Name = "9" };
            //    measureSelectList.Add(new SelectListItem { Text = "Emergent Literacy Reading<br>(Rapid Letter Naming)", Value = "69", Group = sgroup1 });
            //    measureSelectList.Add(new SelectListItem { Text = "Emergent Literacy Writing <br /> (Early Writing Skills)", Value = "65", Group = sgroup1 });
            //    measureSelectList.Add(new SelectListItem { Text = "Language and Communication <br /> (Rapid Vocabulary 1)", Value = "70", Group = sgroup1 });
            //    measureSelectList.Add(new SelectListItem { Text = "Language and Communication <br /> (Rapid Vocabulary 3)", Value = "72", Group = sgroup1 });
            //    measureSelectList.Add(new SelectListItem { Text = "Social and Emotional Development <br /> (Social Emotional Behaviors)", Value = "108", Group = sgroup1 });
            //    measureSelectList.Add(new SelectListItem { Text = "Mathematics <br /> (Math)", Value = "130", Group = sgroup1 });
            //    var sgroup2 = new SelectListGroup { Name = "10" };
            //    measureSelectList.Add(new SelectListItem { Text = "Emergent Literacy Reading <br /> (Letras rápidas)", Value = "84", Group = sgroup2 });
            //    measureSelectList.Add(new SelectListItem { Text = "Emergent Literacy Writing <br /> (Escritura temprana)", Value = "80", Group = sgroup2 });
            //    measureSelectList.Add(new SelectListItem { Text = "Language and Communication <br /> (Vocabulario rápido 1)", Value = "85", Group = sgroup2 });
            //    measureSelectList.Add(new SelectListItem { Text = "Language and Communication <br /> (Vocabulario rápido 3)", Value = "87", Group = sgroup2 });
            //    measureSelectList.Add(new SelectListItem { Text = "Social and Emotional Development <br />(Socio-Emocional)", Value = "88", Group = sgroup2 });
            //    measureSelectList.Add(new SelectListItem { Text = "Mathematics <br /> (Matemáticas)", Value = "137", Group = sgroup2 });
            //    xmlData = measureSelectList.Where(t => t.Group.Name.Equals(assessmentId)).ToList();

            //    XmlHelper.InsertXML(measureSelectList, "Measures", "Measures", "~/Resources/MeasuresTemp/");
            //}
            //Peter 12 / 15 / 2017 Update Measures-List from db
            var xmlData = new List<SelectListItem>();
            xmlData = mapList.Where(t => t.AssessmentId == assessmentId && t.MeasureId != null && t.Wave != null)
                             .Select(t => new SelectListItem { Text = string.Format($"{t.AcademicSubject}<br />({(string.IsNullOrEmpty(t.MeasureName) ? "Null" : t.MeasureName)})"), Value = t.MeasureId + "" })
                             .DistinctBy(t => t.Value)
                             .OrderBy(t => int.Parse(t.Value)).ToList();
            return xmlData;
        }
        #endregion

        #region TsdsAssessmentFiles

        public string CreateTsdsAssessmentFile(int assessmentId, string communityNumber, string filePath)
        {
            string fileName = "";
            AssessmentXmlModel assessmentXmlModel = new AssessmentXmlModel();
            assessmentXmlModel.id = assessmentId;
            List<MeasureXmlModel> measureXmlModels = new List<MeasureXmlModel>();
            List<ItemXmlModel> itemXmlModels = new List<ItemXmlModel>();
            AssessmentEntity assessmentEntity = _adeBusiness.GetAssessment(assessmentId);
            foreach (MeasureEntity measure in assessmentEntity.Measures)
            {
                var waves = measure.ApplyToWave.Split(',');
                foreach (var waveStr in waves)
                {
                    int wave = 0;
                    int.TryParse(waveStr, out wave);
                    if (wave != 2)
                    {
                        MeasureXmlModel measureXml = MeasureToXmlModel(measure, wave);
                        if (measureXml != null)
                            measureXmlModels.Add(measureXml);
                    }
                }

                //List<ItemBaseEntity> itemList = measure.Items.Where(o => o.IsDeleted == false).ToList();
                //foreach (ItemBaseEntity item in itemList)
                //{
                //    ItemXmlModel itemXml = new ItemXmlModel();
                //    itemXml.ItemId = item.ID.ToString();//Mandatory
                //    itemXml.IdentificationCode = item.ID.ToString();//Mandatory
                //    itemXml.ItemCategory = measure.Name;//Mandatory
                //    itemXml.MaxRawScore = item.Score.ToString();//Mandatory
                //    itemXml.CorrectResponse = string.Join(",", item.Answers.Select(x => x.ID).ToList());//Mandatory
                //    itemXml.ContentStandardName = "";
                //    itemXml.LearningStandardIdentificationCode = "";
                //    itemXmlModels.Add(itemXml);
                //}
            }
            assessmentXmlModel.Measures = measureXmlModels;
            assessmentXmlModel.Items = itemXmlModels;
            fileName = communityNumber + "_000_" + CommonAgent.SchoolYearMax + "TSDS_" +
                    DateTime.Now.ToString("yyyyMMddhhmm") + "_InterchangeAssessmentMetadata.xml";
            var xmlFile = CreateAssessmentXml(assessmentXmlModel, fileName, filePath);

            //   fileName = assessmentEntity.Name + "-Metadata-" + DateTime.Now.ToString("yyyyMMdd") + ".xml";
            TsdsAssessmentFileEntity entity = new TsdsAssessmentFileEntity();
            entity.AssessmentId = assessmentId;
            entity.Status = (byte)1;
            entity.FileName = fileName;
            entity.FileSize = "";
            entity.DownloadUrl = "";
            entity.CreatedBy = 1;
            _tsdsContract.InsertTsdsAssessmentFile(entity);
            return xmlFile;
        }



        private MeasureXmlModel MeasureToXmlModel(MeasureEntity measure, int wave)
        {

            var map = mapList.FirstOrDefault(c => c.AssessmentId == measure.AssessmentId && c.MeasureId == measure.ID && c.Wave == wave);


            if (map == null)//2017.03.20
            {
                return null;
                // map = mapList.FirstOrDefault(c => c.AssessmentId == measure.AssessmentId && c.Wave == wave);
            }
            //if (map == null)
            //{
            //    //map = new TSDSMap();
            //    //map = GetOtherMap(wave, measure.Assessment.Language);
            //}

            MeasureXmlModel measureXml = new MeasureXmlModel();
            measureXml.AssessmentID = Regex.Replace("_" + map.AssessmentTitle, @"\s", "") +
                                      Regex.Replace("_" + map.AcademicSubject, @"\s", "");
            measureXml.AssessmentTitle = map.AssessmentTitle;//measure.Name;
            measureXml.AssessmentIdentificationSystem = "School";
            measureXml.AssessmentIdentificationID = map.AssessmentIdentificationCode;
            measureXml.AssessmentCategory = "Other"; //measure.Parent.Name;
            measureXml.AcademicSubject = map.AcademicSubject; //"Reading";
            measureXml.GradeLevelAssessed = "Preschool/Prekindergarten";
            measureXml.PerformanceDescription = "ECDS";//
            measureXml.AssessmentReportingMethod = "Raw score";//
            measureXml.ContentStandard = "State Standard";
            measureXml.Version = CommonAgent.SchoolYearMax;//("000000" + measure.ID).Substring(measure.ID.ToString().Length - 1, 6);
            measureXml.RevisionDate = "2016-03-11";//measure.UpdatedOn.ToString("yyyy-MM-dd");
            measureXml.MaxRawScore = "" + (int)measure.TotalScore;
            measureXml.AssessmentPeriodDescription = "";//
            measureXml.AssessmentItemReference = measure.Items.Select(x => x.ID.ToString()).ToList();//
            measureXml.ReportAssessmentType = "ECDS - PK";

            return measureXml;
        }


        public string CreateAssessmentXml(AssessmentXmlModel model, string fileName, string filePath)
        {


            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder xmlAssessmentNode = new StringBuilder();
            xmlAssessmentNode.Append("<?xml version='1.0' encoding='utf-8'?>");
            xmlAssessmentNode.Append("<InterchangeAssessmentMetadata xsi:schemaLocation='http://www.tea.state.tx.us/tsds InterchangeAssessmentMetadata.xsd' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://www.tea.state.tx.us/tsds'>");
            foreach (MeasureXmlModel measure in model.Measures)
            {
                xmlAssessmentNode.AppendFormat("<Assessment id='{0}'>", measure.AssessmentID);

                xmlAssessmentNode.AppendFormat("<AssessmentTitle>{0}</AssessmentTitle>", measure.AssessmentTitle.ReplaceXmlChar());

                xmlAssessmentNode.AppendFormat("<AssessmentIdentificationCode IdentificationSystem='{0}'>", measure.AssessmentIdentificationSystem.ReplaceXmlChar());
                xmlAssessmentNode.AppendFormat("<ID>{0}</ID>", measure.AssessmentIdentificationID);
                xmlAssessmentNode.Append("</AssessmentIdentificationCode>");

                xmlAssessmentNode.AppendFormat("<AssessmentCategory>{0}</AssessmentCategory>", measure.AssessmentCategory.ReplaceXmlChar());
                xmlAssessmentNode.AppendFormat("<AcademicSubject>{0}</AcademicSubject>", measure.AcademicSubject.ReplaceXmlChar());
                xmlAssessmentNode.AppendFormat("<GradeLevelAssessed>{0}</GradeLevelAssessed>", measure.GradeLevelAssessed.ReplaceXmlChar());

                xmlAssessmentNode.Append("<AssessmentPerformanceLevel>");
                xmlAssessmentNode.Append("<PerformanceLevel ref=\"_bf4ed162-1c4c-4496-a195-a970f5d7281\">");//David 05/22/2017

                // xmlAssessmentNode.AppendFormat("<ref>_ecd9107c-c220-4b39-9fb8-3992967c1e23</ref>");
                xmlAssessmentNode.AppendFormat("<Description>{0}</Description>", measure.PerformanceDescription.ReplaceXmlChar());
                xmlAssessmentNode.Append("</PerformanceLevel>");
                xmlAssessmentNode.AppendFormat("<AssessmentReportingMethod>{0}</AssessmentReportingMethod>", measure.AssessmentReportingMethod.ReplaceXmlChar());
                xmlAssessmentNode.Append("</AssessmentPerformanceLevel>");

                xmlAssessmentNode.AppendFormat("<ContentStandard>{0}</ContentStandard>", measure.ContentStandard.ReplaceXmlChar());
                xmlAssessmentNode.AppendFormat("<Version>{0}</Version>", measure.Version.ReplaceXmlChar());
                xmlAssessmentNode.AppendFormat("<RevisionDate>{0}</RevisionDate>", measure.RevisionDate.ReplaceXmlChar());
                xmlAssessmentNode.AppendFormat("<MaxRawScore>{0}</MaxRawScore>", measure.MaxRawScore);
                xmlAssessmentNode.AppendFormat("<TX-ReportAssessmentType>{0}</TX-ReportAssessmentType>", measure.ReportAssessmentType);
                //xmlAssessmentNode.Append("<AssessmentPeriod>");
                //xmlAssessmentNode.AppendFormat("<Description>{0}</Description>", measure.AssessmentPeriodDescription.ReplaceXmlChar());
                //xmlAssessmentNode.Append("</AssessmentPeriod>");

                //foreach (string itemId in measure.AssessmentItemReference)
                //{
                //    xmlAssessmentNode.AppendFormat("<AssessmentItemReference ref='{0}'/>", itemId);
                //}

                xmlAssessmentNode.Append("</Assessment>");
            }

            //foreach (ItemXmlModel item in model.Items)
            //{
            //    xmlAssessmentNode.AppendFormat("<AssessmentItem id='{0}'>", item.ItemId);

            //    xmlAssessmentNode.AppendFormat("<IdentificationCode>{0}</IdentificationCode>", item.IdentificationCode.ReplaceXmlChar());
            //    xmlAssessmentNode.AppendFormat("<ItemCategory>{0}</ItemCategory>", item.ItemCategory.ReplaceXmlChar());
            //    xmlAssessmentNode.AppendFormat("<MaxRawScore>{0}</MaxRawScore>", item.MaxRawScore.ReplaceXmlChar());
            //    xmlAssessmentNode.AppendFormat("<CorrectResponse>{0}</CorrectResponse>", item.CorrectResponse.ReplaceXmlChar());

            //    xmlAssessmentNode.Append("<LearningStandardReference>");
            //    xmlAssessmentNode.Append("<LearningStandardIdentity>");
            //    xmlAssessmentNode.AppendFormat("<LearningStandardId ContentStandardName='{0}'>", item.ContentStandardName.ReplaceXmlChar());
            //    xmlAssessmentNode.AppendFormat("<IdentificationCode>{0}</IdentificationCode>", item.LearningStandardIdentificationCode.ReplaceXmlChar());
            //    xmlAssessmentNode.Append("</LearningStandardId>");
            //    xmlAssessmentNode.Append("</LearningStandardIdentity>");
            //    xmlAssessmentNode.Append("</LearningStandardReference>");

            //    xmlAssessmentNode.Append("</AssessmentItem>");
            //}
            xmlAssessmentNode.Append("<PerformanceLevelDescriptor id=\"_bf4ed162-1c4c-4496-a195-a970f5d7281\">");
            xmlAssessmentNode.Append("<Description>ECDS</Description>");
            xmlAssessmentNode.Append("</PerformanceLevelDescriptor>");
            xmlAssessmentNode.Append("</InterchangeAssessmentMetadata>");

            xmlDoc.LoadXml(xmlAssessmentNode.ToString());


            //  xmlDoc.LoadXml(xmlStuAssessmentNodes.ToString());
            ;
            xmlDoc.Save(filePath + fileName);
            return fileName;


            //Stream stream = new MemoryStream();
            //xmlDoc.Save(stream);
            //byte[] bytes = new byte[(int)stream.Length];
            //stream.Seek(0, SeekOrigin.Begin);
            //stream.Read(bytes, 0, bytes.Length);
            //stream.Close();

            //return bytes;

        }


        #endregion

        #region InterchangeStudentParentExtension.xml 
        /// <summary>
        /// David 06/13/2017
        /// </summary>
        private StuParentExtXmlModel CreateStuParentExtModel(List<int> stuIds)
        {
            //errorStudentEntities = new List<StudentEntity>();
            StuParentExtXmlModel stuParentExtXml = new StuParentExtXmlModel();
            List<StudentModel> stuModels = new List<StudentModel>();
            List<StudentEntity> studentEntities = _studentBusiness.GetStudentsByIds(stuIds);
            foreach (StudentEntity stuEntity in studentEntities)
            {
                if (IsNumber10(stuEntity.TSDSStudentID))
                {
                    StudentModel studentModel = new StudentModel();
                    studentModel.Id = "_" + stuEntity.TSDSStudentID;
                    studentModel.StudentUniqueStateId = stuEntity.TSDSStudentID;

                    //The 9 digit number can be any number as long as its 9 digits.  Typically it would be the SSN or S Number (Alternate ID)
                    studentModel.StudentIdentificationCode = stuEntity.StudentId.Substring(3, 9);//(9 digits)

                    studentModel.FirstName = stuEntity.FirstName;
                    studentModel.LastSurname = stuEntity.LastName;
                    //studentModel.Sex= stuEntity.Gender
                    studentModel.BirthDate = stuEntity.BirthDate.ToString("yyyy-MM-dd");
                    //please add other infroatmion from here if needed
                    stuModels.Add(studentModel);
                }
                /* else
                 {
                     errorStudentEntities.Add(stuEntity);
                     stuIds.Remove(stuEntity.ID);
                 }*/
            }
            stuParentExtXml.students = stuModels;
            return stuParentExtXml;
        }
        /// <summary>
        /// David 06/13/2017
        /// Same as StudentAssessmentXML students' infromation
        /// </summary>
        public string CreateStudentParentExtensionXml(List<int> stuIdList, string communityNumber, string xmlFilePath)
        {
            //CommunityEntity community = _communityBusiness.GetCommunity(communityId);
            // string communityNumber = community.DistrictNumber;
            StuParentExtXmlModel stuParentExtXmlModel = CreateStuParentExtModel(stuIdList);

            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder xmlStudParentExtNodes = new StringBuilder();
            xmlStudParentExtNodes.Append("<?xml version='1.0' encoding='utf-8'?>");
            xmlStudParentExtNodes.Append("<InterchangeStudentParent xsi:schemaLocation='http://www.tea.state.tx.us/tsds InterchangeStudentParentExtension.xsd' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://www.tea.state.tx.us/tsds'>");
            List<StudentModel> stuModels = stuParentExtXmlModel.students;
            foreach (StudentModel stuModel in stuModels)
            {
                xmlStudParentExtNodes.AppendFormat("<Student id='{0}'>", stuModel.Id);
                xmlStudParentExtNodes.AppendFormat("<StudentUniqueStateId>{0}</StudentUniqueStateId>", stuModel.StudentUniqueStateId);
                xmlStudParentExtNodes.Append("<StudentIdentificationCode IdentificationSystem='CPM'>");
                xmlStudParentExtNodes.AppendFormat("<IdentificationCode>{0}</IdentificationCode>", stuModel.StudentIdentificationCode);
                xmlStudParentExtNodes.Append("</StudentIdentificationCode>");
                xmlStudParentExtNodes.Append("<Name>");
                xmlStudParentExtNodes.AppendFormat("<FirstName>{0}</FirstName>", stuModel.FirstName);
                xmlStudParentExtNodes.AppendFormat("<LastSurname>{0}</LastSurname>", stuModel.LastSurname);
                xmlStudParentExtNodes.Append("</Name>");
                //xmlStudParentExtNodes.AppendFormat("<Sex>{0}</Sex>", stuModel.Sex); Not Required.
                xmlStudParentExtNodes.Append("<BirthData>");
                xmlStudParentExtNodes.AppendFormat("<BirthDate>{0}</BirthDate>", stuModel.BirthDate); //Format:YYYY-MM-DD
                xmlStudParentExtNodes.Append("</BirthData>");

                xmlStudParentExtNodes.Append("<TX-LEAReference>");
                xmlStudParentExtNodes.Append("<EducationalOrgIdentity>");
                xmlStudParentExtNodes.AppendFormat("<StateOrganizationId>{0}</StateOrganizationId>", communityNumber);//6 digits
                xmlStudParentExtNodes.Append("</EducationalOrgIdentity>");
                xmlStudParentExtNodes.Append("</TX-LEAReference>");

                xmlStudParentExtNodes.Append("</Student>");
            }

            xmlStudParentExtNodes.Append("</InterchangeStudentParent>");

            xmlDoc.LoadXml(xmlStudParentExtNodes.ToString());
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmm");
            string xmlFileName = communityNumber + "_000_" + CommonAgent.SchoolYearMax + "TSDS_" + timeStamp + "_InterchangeStudentParentExtension.xml";
            xmlDoc.Save(xmlFilePath + xmlFileName);
            return xmlFileName;
        }
        #endregion


        #region Tsds Student Assessment xml
        public List<TsdsEntity> GetPendingTsds()
        {
            return _tsdsContract.Tsdses.Where(x => x.Status == TsdsStatus.Pending).ToList();
        }

        public List<TsdsEntity> GetPendingTsds(List<int> TsdsIDs)
        {
            return _tsdsContract.Tsdses.Where(x => x.Status == TsdsStatus.Pending && TsdsIDs.Contains(x.ID)).ToList();
        }

        //判断是否为10位数字
        private bool IsNumber10(string tsds)
        {
            return Regex.IsMatch(tsds, @"^\d{10}$");
        }

        public StuAssessmentXmlModel CreateStuAssessmentModel(int assessmentId, List<int> stuIds, List<int> measureIds, out List<StudentEntity> errorStudentEntities)
        {
            errorStudentEntities = new List<StudentEntity>();
            StuAssessmentXmlModel stuAssessmentXml = new StuAssessmentXmlModel();

            List<StudentReferenceModel> stuReferenceModels = new List<StudentReferenceModel>();
            List<StudentEntity> studentEntities = _studentBusiness.GetStudentsByIds(stuIds);

            foreach (StudentEntity stuEntity in studentEntities)
            {
                if (IsNumber10(stuEntity.TSDSStudentID))
                {
                    //Peter 12/13/2017 If 2 or more Students have the same TSDS ID should be included on the ID report
                    if (studentEntities.Count(t => t.TSDSStudentID == stuEntity.TSDSStudentID) > 2)
                    {
                        errorStudentEntities.Add(stuEntity);
                        stuIds.Remove(stuEntity.ID);
                    }
                    else
                    {
                        StudentReferenceModel stuReferenceModel = new StudentReferenceModel();
                        stuReferenceModel.ReferenceId = "_" + stuEntity.TSDSStudentID;
                        stuReferenceModel.StudentUniqueStateId = stuEntity.TSDSStudentID;
                        stuReferenceModel.FirstName = stuEntity.FirstName;
                        stuReferenceModel.LastSurname = stuEntity.LastName;
                        stuReferenceModel.BirthDate = stuEntity.BirthDate.ToString("yyyy-MM-dd");
                        stuReferenceModels.Add(stuReferenceModel);
                    }
                }
                else
                {
                    errorStudentEntities.Add(stuEntity);
                    stuIds.Remove(stuEntity.ID);
                }

            }

            List<AssessmentReferenceModel> assessmentReferenceModels = new List<AssessmentReferenceModel>();
            //List<MeasureEntity> measures = _cpallsBusiness.GetMeasureByAessIdStuIds(assessmentId, stuIds);
            List<MeasureEntity> measures = _adeBusiness.GetMeasureEntitiesByIds(measureIds);
            foreach (MeasureEntity measure in measures)
            {
                var waves = measure.ApplyToWave.Split(',');
                foreach (var waveStr in waves)
                {
                    int wave = 0;
                    int.TryParse(waveStr, out wave);
                    if (wave != 2)
                    {
                        var map = mapList.FirstOrDefault(c => c.AssessmentId == measure.AssessmentId && c.MeasureId == measure.ID && c.Wave == wave);
                        if (map == null)
                        {
                            continue; //map = mapList.FirstOrDefault(c => c.AssessmentId == measure.AssessmentId && c.Wave == wave);
                        }
                        //if (map == null)
                        //{
                        //    map = new TSDSMap();
                        //    map = GetOtherMap(wave, measure.Assessment.Language);
                        //}

                        AssessmentReferenceModel assessmentReference = new AssessmentReferenceModel();
                        assessmentReference.ReferenceId = Regex.Replace("_" + map.AssessmentTitle, @"\s", "") +
                                                          Regex.Replace("_" + map.AcademicSubject, @"\s", "");
                        assessmentReference.AssessmentTitle = map.AssessmentTitle;//measure.Name;
                        assessmentReference.AssessmentCategory = measure.ParentId == 0 ? measure.Name.ReplaceXmlChar() : measure.Parent.Name.ReplaceXmlChar();
                        assessmentReference.AcademicSubject = map.AcademicSubject;// "Reading";
                        assessmentReference.GradeLevelAssessed = "Preschool/Prekindergarten";
                        assessmentReference.Version = CommonAgent.SchoolYearMax;//("000000" + measure.ID).Substring(measure.ID.ToString().Length - 1, 6);
                        assessmentReferenceModels.Add(assessmentReference);
                    }
                }
            }

            List<StudentAssessmentModel> stuAssessmentXmlModels = new List<StudentAssessmentModel>();
            List<StudentMeasureEntity> stuMeasures = _cpallsBusiness.GetStuMeasureByAessIdStuIds(assessmentId, stuIds, measureIds);
            foreach (StudentMeasureEntity stuMeasure in stuMeasures)
            {
                var assessemntId = stuMeasure.Assessment.AssessmentId;
                var measureId = stuMeasure.MeasureId;
                var wave = (int)stuMeasure.Assessment.Wave;
                if (wave != 2)
                {
                    var map =
                        mapList.FirstOrDefault(
                            c => c.AssessmentId == assessemntId && c.MeasureId == measureId && c.Wave == wave);
                    #region David  06/20/2017 Changed it;
                    if (map == null)
                        continue;
                    /* 
                   if (map == null)
                   {
                       map = mapList.FirstOrDefault(c => c.AssessmentId == assessemntId && c.Wave == wave);
                   }
                   if (map == null)
                   {
                       map = new TSDSMap();
                       map = GetOtherMap(wave, stuMeasure.Assessment.Assessment.Language);
                   } */
                    #endregion End for map
                    //StudentEntity student = _studentBusiness.GetStudent(stuMeasure.Assessment.StudentId);
                    StudentEntity student = studentEntities.FirstOrDefault(x => x.ID == stuMeasure.Assessment.StudentId);

                    StudentAssessmentModel stuAssessmentModel = new StudentAssessmentModel();
                    //stuAssessmentModel.id = stuMeasure.ID.ToString();
                    stuAssessmentModel.id = "_" + student.TSDSStudentID +
                                            Regex.Replace("_" + map.AssessmentTitle, @"\s", "") +
                                            Regex.Replace("_" + map.AcademicSubject, @"\s", ""); //David 12/15/2016
                    stuAssessmentModel.AdministrationDate = stuMeasure.UpdatedOn.ToString("yyyy-MM-dd");
                    stuAssessmentModel.AdministrationLanguage = "";
                    stuAssessmentModel.AssessmentReportingMethod = "Raw score";
                    stuAssessmentModel.Result = stuMeasure.Goal.ToString();
                    stuAssessmentModel.GradeLevelWhenAssessed = "";
                    stuAssessmentModel.Description = "";
                    stuAssessmentModel.PerformanceLevelMet = "";
                    stuAssessmentModel.StudentUniqueStateId = student.TSDSStudentID;
                    //stuAssessmentModel.AssessmentReference = stuMeasure.MeasureId.ToString();
                    stuAssessmentModel.AssessmentReference = Regex.Replace("_" + map.AssessmentTitle, @"\s", "") +
                                            Regex.Replace("_" + map.AcademicSubject, @"\s", "");
                    //David 12/15/2016
                    stuAssessmentXmlModels.Add(stuAssessmentModel);
                }
            }




            List<StudentAssessmentItemModel> stuItemXmlModels = new List<StudentAssessmentItemModel>();
            //List<int> SMIds = stuMeasures.Select(x => x.ID).ToList();
            //List<StudentItemEntity> stuItems = _cpallsBusiness.GetStuItemsBySMIds(SMIds, measureIds);
            //foreach (StudentItemEntity item in stuItems)
            //{
            //    StudentAssessmentItemModel stuItemModel = new StudentAssessmentItemModel();
            //    stuItemModel.AssessmentResponse = "";
            //    stuItemModel.AssessmentItemResult = item.IsCorrect ? "Correct" : "Incorrect";
            //    stuItemModel.RawScoreResult = item.Goal.ToString();
            //    stuItemModel.StudentTestAssessmentReference = "";
            //    stuItemModel.AssessmentItemIdentificationCode = item.ItemId.ToString();
            //    stuItemXmlModels.Add(stuItemModel);
            //}

            stuAssessmentXml.students = stuReferenceModels;
            stuAssessmentXml.assessments = assessmentReferenceModels;
            stuAssessmentXml.studentAssessments = stuAssessmentXmlModels;
            stuAssessmentXml.items = stuItemXmlModels;
            return stuAssessmentXml;
        }
        public TsdsMapEntity GetOtherMap(int wave, Core.Ade.AssessmentLanguage language)
        {
            var map = new TsdsMapEntity();
            if (wave == 2)
            {
                map.AssessmentIdentificationCode = "CIRCLE English";
                map.AcademicSubject = "Composite";
                if (language == Core.Ade.AssessmentLanguage.English)
                {
                    map.AssessmentTitle = "MOY PK CIRCLE Assessment English";
                }
                else
                {
                    map.AssessmentIdentificationCode = "CIRCLE Spanish";
                    map.AssessmentTitle = "MOY PK CIRCLE Assessment Spanish";
                }
            }
            return map;
        }

        public string CreatStuAssessmentXml(StuAssessmentXmlModel stuAssessmentXmlModel, string communityNumber, int assessmentId, string xmlFilePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder xmlStuAssessmentNodes = new StringBuilder();
            xmlStuAssessmentNodes.Append("<?xml version='1.0' encoding='utf-8'?>");
            xmlStuAssessmentNodes.Append("<InterchangeStudentAssessment xsi:schemaLocation='http://www.tea.state.tx.us/tsds InterchangeStudentAssessment.xsd' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://www.tea.state.tx.us/tsds'>");

            List<StudentReferenceModel> stuReferenceModels = stuAssessmentXmlModel.students;
            foreach (StudentReferenceModel stuModel in stuReferenceModels)
            {
                xmlStuAssessmentNodes.AppendFormat("<StudentReference id='{0}'>", stuModel.ReferenceId);
                xmlStuAssessmentNodes.Append("<StudentIdentity>");
                xmlStuAssessmentNodes.AppendFormat("<StudentUniqueStateId>{0}</StudentUniqueStateId>", stuModel.StudentUniqueStateId);
                //xmlStuAssessmentNodes.Append("<Name>");
                //xmlStuAssessmentNodes.AppendFormat("<FirstName>{0}</FirstName>", stuModel.FirstName.ReplaceXmlChar());
                //xmlStuAssessmentNodes.AppendFormat("<LastSurname>{0}</LastSurname>", stuModel.LastSurname.ReplaceXmlChar());
                //xmlStuAssessmentNodes.Append("</Name>");
                xmlStuAssessmentNodes.AppendFormat("<BirthDate>{0}</BirthDate>", stuModel.BirthDate);
                xmlStuAssessmentNodes.Append("</StudentIdentity>");
                xmlStuAssessmentNodes.Append("</StudentReference>");
            }

            List<AssessmentReferenceModel> assessmentReferenceModels = stuAssessmentXmlModel.assessments;
            foreach (AssessmentReferenceModel measureModel in assessmentReferenceModels)
            {
                xmlStuAssessmentNodes.AppendFormat("<AssessmentReference id='{0}'>", measureModel.ReferenceId);
                xmlStuAssessmentNodes.Append("<AssessmentIdentity>");
                xmlStuAssessmentNodes.AppendFormat("<AssessmentTitle>{0}</AssessmentTitle>", measureModel.AssessmentTitle.ReplaceXmlChar());
                //xmlStuAssessmentNodes.AppendFormat("<AssessmentCategory>{0}</AssessmentCategory>", measureModel.AssessmentCategory.ReplaceXmlChar());
                xmlStuAssessmentNodes.AppendFormat("<AcademicSubject>{0}</AcademicSubject>", measureModel.AcademicSubject.ReplaceXmlChar());
                xmlStuAssessmentNodes.AppendFormat("<GradeLevelAssessed>{0}</GradeLevelAssessed>", measureModel.GradeLevelAssessed.ReplaceXmlChar());
                xmlStuAssessmentNodes.AppendFormat("<Version>{0}</Version>", measureModel.Version.ReplaceXmlChar());
                xmlStuAssessmentNodes.Append("</AssessmentIdentity>");
                xmlStuAssessmentNodes.Append("</AssessmentReference>");
            }

            List<StudentAssessmentModel> stuAssessmentModels = stuAssessmentXmlModel.studentAssessments;
            foreach (StudentAssessmentModel stuAssessModel in stuAssessmentModels)
            {
                xmlStuAssessmentNodes.AppendFormat("<StudentAssessment id='{0}'>", stuAssessModel.id);
                xmlStuAssessmentNodes.AppendFormat("<AdministrationDate>{0}</AdministrationDate>", stuAssessModel.AdministrationDate);
                //xmlStuAssessmentNodes.AppendFormat("<AdministrationLanguage>{0}</AdministrationLanguage>", stuAssessModel.AdministrationLanguage);
                xmlStuAssessmentNodes.AppendFormat("<ScoreResults AssessmentReportingMethod='{0}'>", stuAssessModel.AssessmentReportingMethod.ReplaceXmlChar());
                xmlStuAssessmentNodes.AppendFormat("<Result>{0}</Result>", stuAssessModel.Result);
                xmlStuAssessmentNodes.Append("</ScoreResults>");
                //xmlStuAssessmentNodes.AppendFormat("<GradeLevelWhenAssessed>{0}</GradeLevelWhenAssessed>", stuAssessModel.GradeLevelWhenAssessed.ReplaceXmlChar());

                //xmlStuAssessmentNodes.Append("<PerformanceLevels>");
                //xmlStuAssessmentNodes.AppendFormat("<Description>{0}</Description>", stuAssessModel.Description.ReplaceXmlChar());
                //xmlStuAssessmentNodes.AppendFormat("<PerformanceLevelMet>{0}</PerformanceLevelMet>", stuAssessModel.PerformanceLevelMet.ReplaceXmlChar());
                //xmlStuAssessmentNodes.Append("</PerformanceLevels>");

                xmlStuAssessmentNodes.Append("<StudentReference>");
                xmlStuAssessmentNodes.Append("<StudentIdentity>");
                xmlStuAssessmentNodes.AppendFormat("<StudentUniqueStateId>{0}</StudentUniqueStateId>", stuAssessModel.StudentUniqueStateId);
                xmlStuAssessmentNodes.Append("</StudentIdentity>");
                xmlStuAssessmentNodes.Append("</StudentReference>");

                xmlStuAssessmentNodes.AppendFormat("<AssessmentReference ref='{0}'/>", stuAssessModel.AssessmentReference.ReplaceXmlChar());
                xmlStuAssessmentNodes.Append("</StudentAssessment>");
            }

            //List<StudentAssessmentItemModel> stuItems = stuAssessmentXmlModel.items;
            //foreach (StudentAssessmentItemModel stuItem in stuItems)
            //{
            //    xmlStuAssessmentNodes.Append("<StudentAssessmentItem>");
            //    xmlStuAssessmentNodes.AppendFormat("<AssessmentResponse>{0}</AssessmentResponse>", stuItem.AssessmentResponse.ReplaceXmlChar());
            //    xmlStuAssessmentNodes.AppendFormat("<AssessmentItemResult>{0}</AssessmentItemResult>", stuItem.AssessmentItemResult.ReplaceXmlChar());
            //    xmlStuAssessmentNodes.AppendFormat("<RawScoreResult>{0}</RawScoreResult>", stuItem.RawScoreResult.ReplaceXmlChar());
            //    xmlStuAssessmentNodes.AppendFormat("<StudentTestAssessmentReference ref='{0}'/>", stuItem.StudentTestAssessmentReference.ReplaceXmlChar());
            //    xmlStuAssessmentNodes.Append("<AssessmentItemReference>");
            //    xmlStuAssessmentNodes.Append("<AssessmentItemIdentity>");
            //    xmlStuAssessmentNodes.AppendFormat("<AssessmentItemIdentificationCode>{0}</AssessmentItemIdentificationCode>", stuItem.AssessmentItemIdentificationCode.ReplaceXmlChar());
            //    xmlStuAssessmentNodes.Append("</AssessmentItemIdentity>");
            //    xmlStuAssessmentNodes.Append("</AssessmentItemReference>");
            //    xmlStuAssessmentNodes.Append("</StudentAssessmentItem>");
            //}

            xmlStuAssessmentNodes.Append("</InterchangeStudentAssessment>");

            xmlDoc.LoadXml(xmlStuAssessmentNodes.ToString());
            // AssessmentEntity assessmentEntity = _adeBusiness.GetAssessment(assessmentId);
            //string schoolNumber = "000";
            //string schoolYear = "0000TSDS";
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmm");
            string xmlFileName = communityNumber + "_000_" + CommonAgent.SchoolYearMax + "TSDS_" + timeStamp + "_InterchangeStudentAssessment.xml";
            xmlDoc.Save(xmlFilePath + xmlFileName);
            return xmlFileName;
        }
        #endregion

        #region TSDS

        public OperationResult InsertTSDS(List<TsdsEntity> tsds)
        {
            return _tsdsContract.InsertTsds(tsds);
        }


        public OperationResult InsertTSDS(TsdsEntity tsds)
        {
            if (tsds.ID <= 0)
                return _tsdsContract.InsertTsds(tsds);
            else
                return _tsdsContract.UpdateTsds(tsds);
        }

        public TsdsEntity GetTsds(int id)
        {
            return _tsdsContract.GetTsds(id);
        }

        public OperationResult UpdateTsds(TsdsEntity tsds)
        {
            return _tsdsContract.UpdateTsds(tsds);
        }

        public List<DownloadListModel> SearchTsdses(UserBaseEntity user, Expression<Func<TsdsEntity, bool>> condition,
   string sort, string order, int first, int count, out int total)
        {
            var query = _tsdsContract.Tsdses.AsExpandable().Where(condition);
            if (user.Role == Role.Community || user.Role == Role.District_Community_Specialist
                || user.Role == Role.District_Community_Delegate || user.Role == Role.Community_Specialist_Delegate)
            {
                var userCommunityList = _communityBusiness.GetCommunityIdsByUser(user);
                query = query.Where(o => userCommunityList.Contains(o.CommunityId));
            }
            else if (user.Role != Role.Super_admin)
            {
                var userSchoolList = _schoolBusiness.SearchSchoolIdsByUserIds(user).Select(s => s.ToString()).ToList();
                query = query.Where(o => userSchoolList.Any(s => o.SchoolIds.IndexOf(s) >= 0));
            }
            var modelQuery = query.Select(o => new DownloadListModel
            {
                ID = o.ID,
                CommunityName = o.Community == null ? "" : o.Community.Name,
                SchoolName = o.SchoolNames,
                FileName = o.FileName,
                MisFileName = o.ErrorFileName == null ? "" : o.ErrorFileName,
                DownloadedOn = o.DownloadOn,
                DownloadedBy = o.DownloadUser == null ? "" : o.DownloadUser.FirstName + " " + o.DownloadUser.LastName,
                Status = o.Status
            });
            total = modelQuery.Count();
            var queryList = modelQuery.OrderBy(sort, order).Skip(first).Take(count);
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