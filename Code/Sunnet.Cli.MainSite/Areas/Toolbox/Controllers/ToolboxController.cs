using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.MainSite.Areas.Toolbox.Models;
using Sunnet.Cli.MainSite.Controllers;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.Core.Vcw.Entities;

namespace Sunnet.Cli.MainSite.Areas.Toolbox.Controllers
{
    public class ToolboxController : BaseController
    {

        #region Private Field
        private readonly MasterDataBusiness _masterDataBusiness = null;
        private readonly SchoolBusiness _schoolBusiness = null;
        private readonly ClassroomBusiness _classroomBusiness = null;
        private readonly ClassBusiness _classBusiness = null;
        private readonly UserBusiness _userBusiness = null;
        private readonly VCW_MasterDataBusiness _vcwmasterDataBusiness = null;
        #endregion

        #region Public Contruction
        public ToolboxController()
        {
            _masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            _schoolBusiness = new SchoolBusiness(UnitWorkContext);
            _classroomBusiness = new ClassroomBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness(UnitWorkContext);
            _userBusiness = new UserBusiness(UnitWorkContext);
            _vcwmasterDataBusiness = new VCW_MasterDataBusiness();
        }
        #endregion

        #region Public Function
        [CLIUrlAuthorize(Account = Authority.Index, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public ActionResult Index(string name)
        {
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Edit, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public ActionResult DropDownList(string name)
        {
            switch (name.Trim().ToLower())
            {
                case "fundings":
                case "languages":
                case "curriculums":
                case "uploadtypes":
                case "sessions":
                case "waves":
                case "contexts":
                case "videolanguages":
                case "assignmentcontents":
                case "videocontents":
                case "coachingstrategies":
                case "selectionlists":
                    break;
                case "trsproviders":
                    ViewBag.ShowName = "TRS Provider";
                    break;
                case "parentagencies":
                    ViewBag.ShowName = "Parent Agency";
                    break;
                case "internetserviceproviders":
                    ViewBag.ShowName = "Internet Service Provider";
                    break;
                case "kits":
                    break;
                case "monitoringtools":
                    ViewBag.ShowName = "Monitoring Tool";
                    break;
                case "yearsinproject":
                    ViewBag.ShowName = "Years In Project";
                    break;
                case "affiliations":
                    break;
                case "professionaldevelopments":
                    ViewBag.ShowName = "Professional Development";
                    break;
                case "certificates":
                    break;
            }
            ViewBag.ddlName = name;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Edit, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public ActionResult DropDownListOther(string name)
        {
            switch (name.Trim().ToLower())
            {
                case "titles":
                    ViewBag.OtherOptions = DesEnum.CommunityDistrict_PrimaryContact.
                       ToSelectList().AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, string.Empty);
                    break;
                case "schooltypes":
                    ViewBag.OtherOptions = ListToDDL(_schoolBusiness.GetSchoolTypeSelectList(0, false));
                    ViewBag.ShowName = "School Types";
                    break;
                case "positions":
                    ViewBag.OtherOptions = GetPositionRole();
                    break;
            }
            ViewBag.ddlName = name;
            return View();
        }

        [CLIUrlAuthorize(Account = Authority.Edit, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public bool IsSameName(string name, string fieldName, int otherId = 0, bool isEdit = false, int fieldId = 0)
        {
            fieldName = fieldName.Trim();
            switch (name.Trim().ToLower())
            {
                #region mainsite
                case "fundings":
                    var fundings = _masterDataBusiness.GetFundingSelectListOther();
                    if (isEdit)
                        return fundings.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return fundings.Any(o => o.Name == fieldName);
                case "languages":
                    var languages = _masterDataBusiness.GetLanguageSelectList(false);
                    if (isEdit)
                        return languages.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return languages.Any(o => o.Name == fieldName);
                case "curriculums":
                    var curriculums = _masterDataBusiness.GetCurriculumSelectList(false);
                    if (isEdit)
                        return curriculums.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return curriculums.Any(o => o.Name == fieldName);
                case "trsproviders":
                    var trsProviders = _schoolBusiness.GetTrsProviderList(false);
                    if (isEdit)
                        return trsProviders.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return trsProviders.Any(o => o.Name == fieldName);

                case "parentagencies":
                    var parentAgencies = _schoolBusiness.GetParentAgencyList(false);
                    if (isEdit)
                        return parentAgencies.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return parentAgencies.Any(o => o.Name == fieldName);
                case "internetserviceproviders":
                    var internetserviceproviders = _schoolBusiness.GetIspList(false);
                    if (isEdit)
                        return internetserviceproviders.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return internetserviceproviders.Any(o => o.Name == fieldName);
                case "kits":
                    var kits = _classroomBusiness.GetKitsSelectList(true, false);
                    if (isEdit)
                        return kits.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return kits.Any(o => o.Name == fieldName);
                case "monitoringtools":
                    var monitoringtools = _classBusiness.GetMonitoringToolSelectList(false);
                    if (isEdit)
                        return monitoringtools.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return monitoringtools.Any(o => o.Name == fieldName);
                case "yearsinproject":
                    var yearsInProject = _userBusiness.GetYearsInProjects(false);
                    if (isEdit)
                        return yearsInProject.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return yearsInProject.Any(o => o.Name == fieldName);
                case "affiliations":
                    var affiliations = _userBusiness.GetAffiliations(false);
                    if (isEdit)
                        return affiliations.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return affiliations.Any(o => o.Name == fieldName);
                case "professionaldevelopments":
                    var professionalDevelopments = _userBusiness.GetPDs(false);
                    if (isEdit)
                        return professionalDevelopments.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return professionalDevelopments.Any(o => o.Name == fieldName);
                case "certificates":
                    var certificates = _userBusiness.GetCertificates(false);
                    if (isEdit)
                        return certificates.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return certificates.Any(o => o.Name == fieldName);
                case "titles":
                    var titles = _masterDataBusiness.GetTitleSelectListOther();
                    if (isEdit)
                        return titles.Any(o => o.Name == fieldName && o.OtherId == otherId && o.ID != fieldId);
                    else
                        return titles.Any(o => o.Name == fieldName && o.OtherId == otherId);
                case "schooltypes":
                    var schoolTypes = _schoolBusiness.GetSchoolTypeSelectListOther();
                    if (isEdit)
                        return schoolTypes.Any(o => (o.Name == fieldName && o.OtherId == otherId && o.ID != fieldId));
                    else
                        return schoolTypes.Any(o => o.Name == fieldName && o.OtherId == otherId);
                case "positions":
                    var positions = _userBusiness.GetPositionsOther();
                    if (isEdit)
                        return positions.Any(o => o.Name == fieldName && o.OtherId == otherId && o.ID != fieldId);
                    else
                        return positions.Any(o => o.Name == fieldName && o.OtherId == otherId);
                #endregion

                #region vcw
                case "uploadtypes":
                    var uploadtypes = _vcwmasterDataBusiness.GetAllUploadTypes();
                    if (isEdit)
                        return uploadtypes.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return uploadtypes.Any(o => o.Name == fieldName);
                case "sessions":
                    var sessions = _vcwmasterDataBusiness.GetAllSessions();
                    if (isEdit)
                        return sessions.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return sessions.Any(o => o.Name == fieldName);
                case "waves":
                    var waves = _vcwmasterDataBusiness.GetAllWaves();
                    if (isEdit)
                        return waves.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return waves.Any(o => o.Name == fieldName);
                case "contexts":
                    var contexts = _vcwmasterDataBusiness.GetAllContext_Datas();
                    if (isEdit)
                        return contexts.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return contexts.Any(o => o.Name == fieldName);
                case "videolanguages":
                    var videolanguages = _vcwmasterDataBusiness.GetAllVideo_Language_Datas();
                    if (isEdit)
                        return videolanguages.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return videolanguages.Any(o => o.Name == fieldName);
                case "assignmentcontents":
                    var assignmentcontents = _vcwmasterDataBusiness.GetAllAssignment_Content_Datas();
                    if (isEdit)
                        return assignmentcontents.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return assignmentcontents.Any(o => o.Name == fieldName);
                case "videocontents":
                    var videocontents = _vcwmasterDataBusiness.GetAllVideo_Content_Datas();
                    if (isEdit)
                        return videocontents.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return videocontents.Any(o => o.Name == fieldName);
                case "coachingstrategies":
                    var coachingstrategies = _vcwmasterDataBusiness.GetAllCoachingStrategy_Datas();
                    if (isEdit)
                        return coachingstrategies.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return coachingstrategies.Any(o => o.Name == fieldName);
                case "selectionlists":
                    var selectionlists = _vcwmasterDataBusiness.GetAllVideo_SelectionList_Datas();
                    if (isEdit)
                        return selectionlists.Any(o => o.Name == fieldName && o.ID != fieldId);
                    else
                        return selectionlists.Any(o => o.Name == fieldName);
                #endregion
            }
            return false;
        }

        [CLIUrlAuthorize(Account = Authority.Edit, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public string Search(string name, int otherId = 0)
        {
            IEnumerable<SelectItemModelOther> list = null;
            switch (name.Trim().ToLower())
            {
                #region mainsite
                case "fundings":
                    list = _masterDataBusiness.GetFundingSelectListOther();
                    break;
                case "languages":
                    list = _masterDataBusiness.GetLanguageSelectListOther();
                    break;
                case "curriculums":
                    list = _masterDataBusiness.GetCurriculumSelectListOther();
                    break;
                case "trsproviders":
                    list = _schoolBusiness.GetTrsProviderListOther();
                    break;
                case "parentagencies":
                    list = _schoolBusiness.GetParentAgencyListOther();
                    break;
                case "internetserviceproviders":
                    list = _schoolBusiness.GetIspListOther();
                    break;
                case "kits":
                    list = _classroomBusiness.GetKitsSelectListOther();
                    break;
                case "monitoringtools":
                    list = _classBusiness.GetMonitoringToolSelectListOther();
                    break;
                case "yearsinproject":
                    list = _userBusiness.GetYearsInProjectsOther();
                    break;
                case "affiliations":
                    list = _userBusiness.GetAffiliationsOther();
                    break;
                case "professionaldevelopments":
                    list = _userBusiness.GetPDsOther();
                    break;
                case "certificates":
                    list = _userBusiness.GetCertificatesOther();
                    break;
                case "titles":
                    list = _masterDataBusiness.GetTitleSelectListOther(otherId);
                    break;
                case "schooltypes":
                    list = _schoolBusiness.GetSchoolTypeSelectListOther(otherId);
                    break;
                case "positions":
                    list = _userBusiness.GetPositionsOther(otherId);
                    break;
                #endregion

                #region vcw
                case "uploadtypes":
                    list = _vcwmasterDataBusiness.GetAllUploadTypes();
                    break;
                case "sessions":
                    list = _vcwmasterDataBusiness.GetAllSessions();
                    break;
                case "waves":
                    list = _vcwmasterDataBusiness.GetAllWaves();
                    break;
                case "contexts":
                    list = _vcwmasterDataBusiness.GetAllContext_Datas();
                    break;
                case "videolanguages":
                    list = _vcwmasterDataBusiness.GetAllVideo_Language_Datas();
                    break;
                case "assignmentcontents":
                    list = _vcwmasterDataBusiness.GetAllAssignment_Content_Datas();
                    break;
                case "videocontents":
                    list = _vcwmasterDataBusiness.GetAllVideo_Content_Datas();
                    break;
                case "coachingstrategies":
                    list = _vcwmasterDataBusiness.GetAllCoachingStrategy_Datas();
                    break;
                case "selectionlists":
                    list = _vcwmasterDataBusiness.GetAllVideo_SelectionList_Datas();
                    break;
                #endregion
                default:
                    list = GetDefaultDDLOther();
                    break;
            }

            list = list ?? new List<SelectItemModelOther>();
            var result = new { total = list.Count(), data = list };
            return JsonHelper.SerializeObject(result);
        }

        [CLIUrlAuthorize(Account = Authority.Edit, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public string Update(string ddlName, int fieldId, string fieldName, sbyte fieldStatus)
        {
            fieldName = fieldName.Trim();
            var response = new PostFormResponse();
            OperationResult result = null;
            switch (ddlName.Trim().ToLower())
            {
                #region mainsite
                case "fundings":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    FundingEntity funding = _masterDataBusiness.GetFunding(fieldId);
                    funding.Name = fieldName;
                    funding.Status = (EntityStatus)fieldStatus;
                    result = _masterDataBusiness.UpdateFunding(funding);
                    break;
                case "languages":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    var language = _masterDataBusiness.GetLanguage(fieldId);
                    language.Language = fieldName;
                    language.Status = (EntityStatus)fieldStatus;
                    result = _masterDataBusiness.UpdateLanguage(language);
                    break;
                case "curriculums":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    var curriculum = _masterDataBusiness.GetCurriculum(fieldId);
                    curriculum.Name = fieldName;
                    curriculum.Status = (EntityStatus)fieldStatus;
                    result = _masterDataBusiness.UpdateCurriculum(curriculum);
                    break;
                case "trsproviders":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    var trsProviders = _schoolBusiness.GetTrsProvider(fieldId);
                    trsProviders.Name = fieldName;
                    trsProviders.Status = (EntityStatus)fieldStatus;
                    result = _schoolBusiness.UpdateTrsProvider(trsProviders);
                    break;
                case "parentagencies":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    var parentAgency = _schoolBusiness.GetParentAgency(fieldId);
                    parentAgency.Name = fieldName;
                    parentAgency.Status = (EntityStatus)fieldStatus;
                    result = _schoolBusiness.UpdateParentAgency(parentAgency);
                    break;
                case "internetserviceproviders":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    var Isp = _schoolBusiness.GetIsp(fieldId);
                    Isp.Name = fieldName;
                    Isp.Status = (EntityStatus)fieldStatus;
                    result = _schoolBusiness.UpdateIsp(Isp);
                    break;
                case "kits":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    KitEntity kit = _classroomBusiness.GetKit(fieldId);
                    kit.Name = fieldName;
                    kit.Status = (EntityStatus)fieldStatus;
                    result = _classroomBusiness.UpdateKit(kit);
                    break;
                case "monitoringtools":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    MonitoringToolEntity monitoringTool = _classBusiness.GetMonitoringTool(fieldId);
                    monitoringTool.Name = fieldName;
                    monitoringTool.Status = (EntityStatus)fieldStatus;
                    result = _classBusiness.UpdateMonitoringTool(monitoringTool);
                    break;
                case "yearsinproject":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    YearsInProjectEntity yearsInProject = _userBusiness.GetYearsInProject(fieldId);
                    yearsInProject.YearsInProject = fieldName;
                    yearsInProject.Status = (EntityStatus)fieldStatus;
                    result = _userBusiness.UpdateYearsInProject(yearsInProject);
                    break;
                case "affiliations":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    AffiliationEntity affiliation = _userBusiness.GetAffiliation(fieldId);
                    affiliation.Affiliation = fieldName;
                    affiliation.Status = (EntityStatus)fieldStatus;
                    result = _userBusiness.UpdateAffiliation(affiliation);
                    break;
                case "professionaldevelopments":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    ProfessionalDevelopmentEntity professionalDevelopment =
                        _userBusiness.GetProfessionalDevelopment(fieldId);
                    professionalDevelopment.ProfessionalDevelopment = fieldName;
                    professionalDevelopment.Status = (EntityStatus)fieldStatus;
                    result = _userBusiness.UpdateProfessionalDevelopment(professionalDevelopment);
                    break;
                case "certificates":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    CertificateEntity certificate = _userBusiness.GetCertificate(fieldId);
                    string oldCertificate = certificate.Certificate;
                    certificate.Certificate = fieldName;
                    certificate.Status = (EntityStatus)fieldStatus;
                    result = _userBusiness.UpdateCertificate(certificate, oldCertificate);
                    break;
                #endregion

                #region vcw

                case "uploadtypes":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    UploadTypeEntity uploadtype = _vcwmasterDataBusiness.GetUploadType(fieldId);
                    uploadtype.Name = fieldName;
                    uploadtype.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateUploadType(uploadtype);
                    break;

                case "sessions":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    SessionEntity session = _vcwmasterDataBusiness.GetSession(fieldId);
                    session.Name = fieldName;
                    session.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateSession(session);
                    break;
                case "waves":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    WaveEntity wave = _vcwmasterDataBusiness.GetWave(fieldId);
                    wave.Name = fieldName;
                    wave.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateWave(wave);
                    break;
                case "contexts":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Context_DataEntity context = _vcwmasterDataBusiness.GetContext_Data(fieldId);
                    context.Name = fieldName;
                    context.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateContext_Data(context);
                    break;
                case "videolanguages":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Video_Language_DataEntity video_language = _vcwmasterDataBusiness.GetVideo_Language_Data(fieldId);
                    video_language.Name = fieldName;
                    video_language.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateVideo_Language_Data(video_language);
                    break;
                case "assignmentcontents":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Assignment_Content_DataEntity assignment_content = _vcwmasterDataBusiness.GetAssignment_Content_Data(fieldId);
                    assignment_content.Name = fieldName;
                    assignment_content.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateAssignment_Content_Data(assignment_content);
                    break;
                case "videocontents":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Video_Content_DataEntity video_content = _vcwmasterDataBusiness.GetVideo_Content_Data(fieldId);
                    video_content.Name = fieldName;
                    video_content.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateVideo_Content_Data(video_content);
                    break;
                case "coachingstrategies":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    CoachingStrategy_DataEntity coachingstrategy = _vcwmasterDataBusiness.GetCoachingStrategy_Data(fieldId);
                    coachingstrategy.Name = fieldName;
                    coachingstrategy.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateCoachingStrategy_Data(coachingstrategy);
                    break;
                case "selectionlists":
                    if (IsSameName(ddlName, fieldName, 0, true, fieldId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Video_SelectionList_DataEntity video_selectionlist = _vcwmasterDataBusiness.GetVideo_SelectionList_Data(fieldId);
                    video_selectionlist.Name = fieldName;
                    video_selectionlist.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.UpdateVideo_SelectionList_Data(video_selectionlist);
                    break;

                #endregion
                default:
                    response.Success = false;
                    break;
            }
            if (result != null)
            {
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
                response.Success = false;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Edit, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public string UpdateOther(string ddlName, int id, string name, int otherId, sbyte status)
        {
            name = name.Trim();
            var response = new PostFormResponse();
            OperationResult result = null;
            switch (ddlName.Trim().ToLower())
            {
                case "titles":
                    if (IsSameName(ddlName, name, otherId, true, id))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    TitleEntity title = _masterDataBusiness.GetTitle(id);
                    title.Name = name;
                    title.model = otherId;
                    title.des = ((DesEnum)otherId).ToDescription();
                    title.Status = (EntityStatus)status;
                    result = _masterDataBusiness.UpdateTitle(title);
                    break;
                case "schooltypes":
                    if (IsSameName(ddlName, name, otherId, true, id))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    SchoolTypeEntity schoolType = _schoolBusiness.GetSchoolType_(id);
                    schoolType.Name = name;
                    schoolType.ParentId = otherId;
                    schoolType.Status = (EntityStatus)status;
                    result = _schoolBusiness.UpdateSchoolType(schoolType);
                    break;
                case "positions":
                    if (IsSameName(ddlName, name, otherId, true, id))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    PositionEntity position = _userBusiness.GetPosition(id);
                    position.Title = name;
                    position.UserType = otherId;
                    position.Status = (EntityStatus)status;
                    result = _userBusiness.UpdatePosition(position);
                    break;

                default:
                    response.Success = false;
                    break;
            }
            if (result != null)
            {
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
                response.Success = false;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorize(Account = Authority.Edit, PageId = PagesModel.Toolbox, Anonymity = Anonymous.Verified)]
        public string Add(string ddlName, string fieldName, sbyte fieldStatus, int otherId = 0)
        {
            fieldName = fieldName.Trim();
            var response = new PostFormResponse();
            OperationResult result = null;
            switch (ddlName.Trim().ToLower())
            {
                #region mainsite
                case "fundings":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var funding = new FundingEntity();
                        funding.Name = fieldName;
                        funding.Status = (EntityStatus)fieldStatus;
                        result = _masterDataBusiness.InsertFunding(funding);
                        result.AppendData = funding.ID;
                    }
                    break;
                case "languages":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        LanguageEntity language = new LanguageEntity();
                        language.Language = fieldName;
                        language.Status = (EntityStatus)fieldStatus;
                        result = _masterDataBusiness.InsertLanguage(language);
                        result.AppendData = language.ID;
                    }
                    break;
                case "curriculums":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var curriculum = new CurriculumEntity();
                        curriculum.Name = fieldName;
                        curriculum.Status = (EntityStatus)fieldStatus;
                        result = _masterDataBusiness.InsertCurriculum(curriculum);
                        result.AppendData = curriculum.ID;
                    }
                    break;
                case "trsproviders":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var trsprovider = new TrsProviderEntity();
                        trsprovider.Name = fieldName;
                        trsprovider.Status = (EntityStatus)fieldStatus;
                        result = _schoolBusiness.InsertTrsProvider(trsprovider);
                        result.AppendData = trsprovider.ID;
                    }
                    break;
                case "parentagencies":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var parentAgency = new ParentAgencyEntity();
                        parentAgency.Name = fieldName;
                        parentAgency.Status = (EntityStatus)fieldStatus;
                        result = _schoolBusiness.InsertParentAgency(parentAgency);
                        result.AppendData = parentAgency.ID;
                    }
                    break;
                case "internetserviceproviders":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var isp = new IspEntity();
                        isp.Name = fieldName;
                        isp.Status = (EntityStatus)fieldStatus;
                        result = _schoolBusiness.InsertIsp(isp);
                        result.AppendData = isp.ID;
                    }
                    break;
                case "kits":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var kit = new KitEntity();
                        kit.Name = fieldName;
                        kit.Status = (EntityStatus)fieldStatus;
                        result = _classroomBusiness.InsertKit(kit);
                        result.AppendData = kit.ID;
                    }
                    break;
                case "monitoringtools":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var monitoringTool = new MonitoringToolEntity();
                        monitoringTool.Name = fieldName;
                        monitoringTool.Status = (EntityStatus)fieldStatus;
                        result = _classBusiness.InsertMonitoringTool(monitoringTool);
                        result.AppendData = monitoringTool.ID;
                    }
                    break;
                case "yearsinproject":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var yearInProject = new YearsInProjectEntity();
                        yearInProject.YearsInProject = fieldName;
                        yearInProject.Status = (EntityStatus)fieldStatus;
                        result = _userBusiness.InsertYearsInProject(yearInProject);
                        result.AppendData = yearInProject.ID;
                    }
                    break;
                case "affiliations":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var affiliation = new AffiliationEntity();
                        affiliation.Affiliation = fieldName;
                        affiliation.Status = (EntityStatus)fieldStatus;
                        result = _userBusiness.InsertAffiliation(affiliation);
                        result.AppendData = affiliation.ID;
                    }
                    break;
                case "professionaldevelopments":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var professionalDevelopment = new ProfessionalDevelopmentEntity();
                        professionalDevelopment.ProfessionalDevelopment = fieldName;
                        professionalDevelopment.Status = (EntityStatus)fieldStatus;
                        result = _userBusiness.InsertProfessionalDevelopment(professionalDevelopment);
                        result.AppendData = professionalDevelopment.ID;
                    }
                    break;
                case "titles":
                    if (IsSameName(ddlName, fieldName, otherId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        TitleEntity title = new TitleEntity();
                        title.Name = fieldName;
                        title.Status = (EntityStatus)fieldStatus;
                        title.model = otherId;
                        title.des = ((DesEnum)otherId).ToDescription();
                        result = _masterDataBusiness.InsertTitle(title);
                        result.AppendData = title.ID;
                    }
                    break;
                case "schooltypes":
                    if (IsSameName(ddlName, fieldName, otherId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var schoolType = new SchoolTypeEntity();
                        schoolType.Name = fieldName;
                        schoolType.ParentId = otherId;
                        schoolType.Status = (EntityStatus)fieldStatus;
                        result = _schoolBusiness.InsertSchoolType(schoolType);
                        result.AppendData = schoolType.ID;
                    }
                    break;
                case "positions":
                    if (IsSameName(ddlName, fieldName, otherId))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var position = new PositionEntity();
                        position.Title = fieldName;
                        position.Status = (EntityStatus)fieldStatus;
                        position.UserType = otherId;
                        result = _userBusiness.InsertPosition(position);
                        result.AppendData = position.ID;
                    }
                    break;
                case "certificates":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var certificate = new CertificateEntity();
                        certificate.Certificate = fieldName;
                        certificate.Status = (EntityStatus)fieldStatus;
                        result = _userBusiness.InsertCertificate(certificate);
                        result.AppendData = certificate.ID;
                    }
                    break;
                #endregion

                #region vcw
                case "uploadtypes":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var uploadtype = new UploadTypeEntity();
                        uploadtype.Name = fieldName;
                        uploadtype.Status = (EntityStatus)fieldStatus;
                        result = _vcwmasterDataBusiness.AddUploadType(uploadtype);
                        result.AppendData = uploadtype.ID;
                    }
                    break;
                case "sessions":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                    }
                    else
                    {
                        var session = new SessionEntity();
                        session.Name = fieldName;
                        session.Status = (EntityStatus)fieldStatus;
                        result = _vcwmasterDataBusiness.AddSession(session);
                        result.AppendData = session.ID;
                    }
                    break;
                case "waves":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    WaveEntity wave = new WaveEntity();
                    wave.Name = fieldName;
                    wave.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.AddWave(wave);
                    result.AppendData = wave.ID;
                    break;
                case "contexts":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Context_DataEntity context = new Context_DataEntity();
                    context.Name = fieldName;
                    context.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.AddContext_Data(context);
                    result.AppendData = context.ID;
                    break;
                case "videolanguages":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Video_Language_DataEntity video_language = new Video_Language_DataEntity();
                    video_language.Name = fieldName;
                    video_language.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.AddVideo_Language_Data(video_language);
                    result.AppendData = video_language.ID;
                    break;
                case "assignmentcontents":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Assignment_Content_DataEntity assignment_content = new Assignment_Content_DataEntity();
                    assignment_content.Name = fieldName;
                    assignment_content.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.AddAssignment_Content_Data(assignment_content);
                    result.AppendData = assignment_content.ID;
                    break;
                case "videocontents":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Video_Content_DataEntity video_content = new Video_Content_DataEntity();
                    video_content.Name = fieldName;
                    video_content.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.AddVideo_Content_Data(video_content);
                    result.AppendData = video_content.ID;
                    break;
                case "coachingstrategies":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    CoachingStrategy_DataEntity coachingstrategy = new CoachingStrategy_DataEntity();
                    coachingstrategy.Name = fieldName;
                    coachingstrategy.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.AddCoachingStrategy_Data(coachingstrategy);
                    result.AppendData = coachingstrategy.ID;
                    break;
                case "selectionlists":
                    if (IsSameName(ddlName, fieldName))
                    {
                        result = new OperationResult(OperationResultType.Error);
                        result.ResultType = OperationResultType.Error;
                        result.Message = CommonAgent.GetInformation("IsSameName");
                        break;
                    }
                    Video_SelectionList_DataEntity video_selectionlist = new Video_SelectionList_DataEntity();
                    video_selectionlist.Name = fieldName;
                    video_selectionlist.Status = (EntityStatus)fieldStatus;
                    result = _vcwmasterDataBusiness.AddVideo_SelectionList_Data(video_selectionlist);
                    result.AppendData = video_selectionlist.ID;
                    break;
                #endregion
                default:
                    response.Success = false;
                    break;
            }
            if (result != null)
            {
                response.Data = result.AppendData;
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            else
                response.Success = false;

            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Administrative, Anonymity = Anonymous.Logined)]
        public string GetSchoolTypeOption()
        {
            var list = ListToDDL(_schoolBusiness.GetSchoolTypeSelectList(0, false));
            return JsonHelper.SerializeObject(list);
        }
        #endregion

        #region Private Function
        private IEnumerable<SelectListItem> ListToDDL(IEnumerable<object> list, string defaultValue = "")
        {
            return
                new SelectList(list, "ID", "Name").AddDefaultItem("None", "0")
                    .AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, string.Empty);
        }

        private IEnumerable<SelectItemModelOther> GetDefaultDDLOther()
        {
            var tmpList = new List<SelectItemModelOther>();
            tmpList.Add(new SelectItemModelOther()
            {
                ID = -1,
                Name = ViewTextHelper.DefaultPleaseSelectText,
                Status = EntityStatus.Active
            });
            return tmpList;
        }

        private List<SelectListItem> GetPositionRole()
        {
            //与Role枚举中数据一一对应
            int[] userType = {101, 105, 110, 115, 125, 130, 133};
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in userType)
            {
                list.Add(new SelectListItem()
                {
                    Text = ((Role)item).ToDescription(),
                    Value = item.ToString()
                });
            }
            list.AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, string.Empty);
            return list;
        }
        #endregion

    }
}