using StructureMap;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Classrooms;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Core.BUP.Enums;
using Sunnet.Cli.Core.BUP.Models;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Web;

namespace Sunnet.Cli.Business.BUP
{
    public class BUPProcessBusiness
    {
        List<SelectItemModel> stateList = new List<SelectItemModel>();
        List<SelectItemModel> primaryList = new List<SelectItemModel>();
        List<SelectItemModel> secondaryTitleList = new List<SelectItemModel>();
        List<CountyEntity> countyList = new List<CountyEntity>();
        List<SelectItemModel> titleList = new List<SelectItemModel>();
        List<SchoolTypeEntity> schoolTypeList = new List<SchoolTypeEntity>();

        ISunnetLog _log;
        private readonly MasterDataBusiness _masterDataBusiness;
        private readonly BUPTaskBusiness _bupTaskBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly UserBusiness _userBusiness;
        private readonly IBUPContract _contract;
        private readonly CommunityBusiness _communityBusiness;
        private readonly ClassroomBusiness _classroomBusiness;
        private readonly ClassBusiness _classBusiness;

        public BUPProcessBusiness(EFUnitOfWorkContext unit = null)
        {
            _contract = DomainFacade.CreateBUPService(unit);
            _masterDataBusiness = new MasterDataBusiness(unit);
            _bupTaskBusiness = new BUPTaskBusiness(unit);
            _schoolBusiness = new SchoolBusiness(unit);
            _userBusiness = new UserBusiness(unit);
            _communityBusiness = new CommunityBusiness(unit);
            _classroomBusiness = new ClassroomBusiness(unit);
            _classBusiness = new ClassBusiness(unit);
            _log = ObjectFactory.GetInstance<ISunnetLog>();
        }

        #region Process

        public string ProcessCommunity(DataTable dt, int userId, string originFileName, string filePath,
            UserBaseEntity user, out int identity, BUPProcessType processType = BUPProcessType.Upload)
        {
            identity = 0;
            if (user.Role == Role.Super_admin)
            {
                List<BUPCommunityModel> dataList = new List<BUPCommunityModel>();
                stateList = _masterDataBusiness.GetStateSelectList().ToList();
                primaryList = _masterDataBusiness.GetTitleSelectList(1).ToList();
                secondaryTitleList = _masterDataBusiness.GetTitleSelectList(2).ToList();
                //Community中，FundingId为必填项
                int fundingId = _masterDataBusiness.GetFundingSelectList()
                    .Where(r => r.Name == "Non Applicable").FirstOrDefault() == null ?
                    _masterDataBusiness.GetFundingSelectList().First().ID :
                    _masterDataBusiness.GetFundingSelectList().Where(r => r.Name == "Non Applicable").FirstOrDefault().ID;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HelpSolve help = new HelpSolve(dt.Rows, i);

                    string action = help.NextData().ToLower();
                    string type = help.NextData().ToLower().Trim();

                    if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                    {
                        continue;
                    }

                    BUPCommunityModel model = new BUPCommunityModel();

                    if (action.StartsWith("i"))
                        model.Action = BUPAction.Insert;
                    else if (action.StartsWith("u"))
                        model.Action = BUPAction.Update;
                    else if (action.StartsWith("d"))
                        model.Action = BUPAction.Delete;
                    else
                        return string.Format("#{0}: Action is incorrect.", i + 2);

                    if (type != "community")
                        return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                    model.Name = help.NextData().Trim();
                    if (model.Name == string.Empty)
                        return string.Format("#{0}: Community Name is incorrect.", i + 2);

                    model.EngageID = help.NextData().Trim();
                    if (model.EngageID == string.Empty && (model.Action == BUPAction.Delete || model.Action == BUPAction.Update))
                        return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                    model.InternalID = help.NextData().Trim();

                    model.Address1 = help.NextData().Trim();
                    if (model.Address1 == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Community Physical Address1 is incorrect.", i + 2);

                    model.Address2 = help.NextData().Trim();
                    model.City = help.NextData().Trim();
                    if (model.City == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Community City is incorrect.", i + 2);

                    string state = help.NextData().Trim().ToLower();
                    var stateEntity = stateList.Find(r => r.Name.ToLower() == state);
                    if (stateEntity == null)
                    {
                        if (model.Action == BUPAction.Insert)
                        {
                            return string.Format("#{0}: Community State is incorrect.", i + 2);
                        }
                        else model.State = 0;    //Update时，若获取的值=0，则存储过程不会修改该字段
                    }
                    else
                        model.State = stateEntity.ID;

                    model.Zip = help.NextData().Trim();
                    if (model.Zip == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Community ZIP is incorrect.", i + 2);

                    model.PhoneNumber = help.NextData().Trim();
                    if (model.PhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Community Phone Number is incorrect.", i + 2);

                    string phoneType = help.NextData().Trim().ToLower();
                    switch (phoneType)
                    {
                        case "h":
                            model.PhoneNumberType = (byte)Core.Common.Enums.PhoneType.HomeNumber;
                            break;
                        case "c":
                            model.PhoneNumberType = (byte)Core.Common.Enums.PhoneType.CellNumber;
                            break;
                        case "w":
                            model.PhoneNumberType = (byte)Core.Common.Enums.PhoneType.WorkNumber;
                            break;
                        case "*clear*":
                            model.PhoneNumberType = 127;
                            break;
                        default:
                            model.PhoneNumberType = 0;
                            break;
                    }

                    string primarySalutation = help.NextData().Trim().ToLower();

                    switch (primarySalutation)
                    {
                        case "mr.":
                            model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Mr;
                            break;
                        case "mrs.":
                            model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Mrs;
                            break;
                        case "ms.":
                            model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Ms;
                            break;
                        case "miss.":
                            model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Miss;
                            break;
                        case "dr.":
                            model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Dr;
                            break;
                        case "*clear*":
                            model.PrimarySalutation = 127;
                            break;
                        default:
                            model.PrimarySalutation = 0;
                            break;
                    }

                    model.PrimaryName = help.NextData().Trim();
                    if (model.PrimaryName == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Community Primary Contact Name is incorrect.", i + 2);


                    string primaryTitle = help.NextData().Trim().ToLower();
                    SelectItemModel primaryModel = primaryList.Find(r => r.Name.ToLower() == primaryTitle);
                    if (primaryModel != null)
                        model.PrimaryTitleId = primaryModel.ID;
                    else
                    {
                        if (model.Action == BUPAction.Update && primaryTitle == "*clear*")
                            model.PrimaryTitleId = -1;
                        else
                            model.PrimaryTitleId = 0;
                    }

                    model.PrimaryPhone = help.NextData().Trim();
                    if (model.PrimaryPhone == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Community Primary Contact Phone Number is incorrect.", i + 2);

                    phoneType = help.NextData().Trim().ToLower();
                    switch (phoneType)
                    {
                        case "h":
                            model.PrimaryPhoneType = (byte)Core.Common.Enums.PhoneType.HomeNumber;
                            break;
                        case "c":
                            model.PrimaryPhoneType = (byte)Core.Common.Enums.PhoneType.CellNumber;
                            break;
                        case "w":
                            model.PrimaryPhoneType = (byte)Core.Common.Enums.PhoneType.WorkNumber;
                            break;
                        case "*clear*":
                            model.PhoneNumberType = 127;
                            break;
                        default:
                            model.PrimaryPhoneType = 0;
                            break;
                    }


                    model.PrimaryEmail = help.NextData();
                    if (model.PrimaryEmail == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Community Primary Contact Email Address is incorrect.", i + 2);
                    if (!string.IsNullOrEmpty(model.PrimaryEmail) && !CommonAgent.IsEmail(model.PrimaryEmail))
                        return string.Format("#{0}: Community Primary Contact Email Address is incorrect.", i + 2);

                    string secondarySalutation = help.NextData().Trim().ToLower();

                    switch (secondarySalutation)
                    {
                        case "mr.":
                            model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Mr;
                            break;
                        case "mrs.":
                            model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Mrs;
                            break;
                        case "ms.":
                            model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Ms;
                            break;
                        case "miss.":
                            model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Miss;
                            break;
                        case "dr.":
                            model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Dr;
                            break;
                        case "*clear*":
                            model.SecondarySalutation = 127;
                            break;
                        default:
                            model.SecondarySalutation = 0;
                            break;
                    }

                    model.SecondaryName = help.NextData().Trim();

                    string secondaryTitle = help.NextData().Trim().ToLower();
                    SelectItemModel secondaryTitleModel = secondaryTitleList.Find(r => r.Name.ToLower() == secondaryTitle);
                    if (secondaryTitleModel != null)
                        model.SecondaryTitleId = secondaryTitleModel.ID;
                    else
                    {
                        if (model.Action == BUPAction.Update && secondaryTitle == "*clear*")
                            model.SecondaryTitleId = -1;
                        else
                            model.SecondaryTitleId = 0;
                    }

                    model.SecondaryPhone = help.NextData().Trim();
                    if (model.SecondaryName != string.Empty && model.Action == BUPAction.Insert && model.SecondaryPhone == string.Empty)
                        return string.Format("#{0}: Community Secondary Contact Phone Number is incorrect.", i + 2);


                    phoneType = help.NextData().Trim().ToLower();
                    switch (phoneType)
                    {
                        case "h":
                            model.SecondaryPhoneType = (byte)Core.Common.Enums.PhoneType.HomeNumber;
                            break;
                        case "c":
                            model.SecondaryPhoneType = (byte)Core.Common.Enums.PhoneType.CellNumber;
                            break;
                        case "w":
                            model.SecondaryPhoneType = (byte)Core.Common.Enums.PhoneType.WorkNumber;
                            break;
                        case "*clear*":
                            model.SecondaryPhoneType = 127;
                            break;
                        default:
                            model.SecondaryPhoneType = 0;
                            break;
                    }

                    model.SecondaryEmail = help.NextData().Trim();
                    if (model.SecondaryName != string.Empty && model.Action == BUPAction.Insert && model.SecondaryEmail == string.Empty)
                        return string.Format("#{0}: Community Secondary Contact Email Address is incorrect.", i + 2);
                    if (!string.IsNullOrEmpty(model.SecondaryEmail) && !CommonAgent.IsEmail(model.SecondaryEmail))
                        return string.Format("#{0}: Community Secondary Contact Email Address is incorrect.", i + 2);

                    model.WebAddress = help.NextData().Trim();
                    model.Status = BUPStatus.Queued;
                    model.Remark = string.Empty;
                    model.FundingId = fundingId;
                    model.LineNum = i + 2;
                    dataList.Add(model);
                }

                BUPTaskEntity taskEntity = new BUPTaskEntity();

                taskEntity.Type = BUPType.Community;
                taskEntity.ProcessType = processType;
                taskEntity.Status = BUPStatus.Queued;
                taskEntity.SendInvitation = false;
                taskEntity.Remark = string.Empty;
                taskEntity.RecordCount = dataList.Count;
                taskEntity.FailCount = 0;
                taskEntity.SuccessCount = 0;
                taskEntity.OriginFileName = originFileName;
                taskEntity.FilePath = filePath;
                taskEntity.CreatedBy = userId;
                taskEntity.UpdatedBy = userId;

                OperationResult result = _bupTaskBusiness.Insert(taskEntity);

                if (result.ResultType == OperationResultType.Success)
                {
                    identity = taskEntity.ID;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("BEGIN TRY ;");
                    sb.Append("BEGIN TRANSACTION;");

                    foreach (BUPCommunityModel model in dataList)
                    {
                        sb.Append(";INSERT INTO dbo.BUP_Communities(TaskId, Action,Name, EngageID,InternalID")
                       .Append(", Address1, Address2, City,State, Zip,PhoneNumber")
                       .Append(", PHoneNumberType, PrimarySalutation, PrimaryName, PrimaryTitleId,PrimaryPhone")
                        .Append(", PrimaryPhoneType, PrimaryEmail, SecondarySalutation, SecondaryName,SecondaryTitleId")
                        .Append(", SecondaryPhone, SecondaryPhoneType, SecondaryEmail, WebAddress,Status,Remark, FundingId, LineNum)");

                        sb.Append(" VALUES (")
                            .AppendFormat("{0},{1},'{2}','{3}','{4}'", taskEntity.ID, (byte)model.Action, model.Name.ReplaceSqlChar(),
                            model.EngageID.ReplaceSqlChar(), model.InternalID.ReplaceSqlChar())  //InternalID
                            .AppendFormat(",'{0}','{1}','{2}',{3},'{4}','{5}'", model.Address1.ReplaceSqlChar(), model.Address2.ReplaceSqlChar(),
                            model.City.ReplaceSqlChar(), model.State, model.Zip.ReplaceSqlChar(), model.PhoneNumber.ReplaceSqlChar()) //PhoneNumber
                            .AppendFormat(",{0},{1},'{2}',{3},'{4}'"
                            , (byte)model.PhoneNumberType, (byte)model.PrimarySalutation, model.PrimaryName.ReplaceSqlChar(),
                            model.PrimaryTitleId, model.PrimaryPhone.ReplaceSqlChar())   //PrimaryPhone
                            .AppendFormat(",{0},'{1}',{2},'{3}',{4}"
                            , (byte)model.PrimaryPhoneType, model.PrimaryEmail.ReplaceSqlChar(),
                            (byte)model.SecondarySalutation, model.SecondaryName.ReplaceSqlChar(), model.SecondaryTitleId)//SecondaryTitleId
                              .AppendFormat(",'{0}',{1},'{2}','{3}',{4},'{5}','{6}',{7}"
                              , model.SecondaryPhone.ReplaceSqlChar(), (byte)model.SecondaryPhoneType, model.SecondaryEmail.ReplaceSqlChar(),
                              model.WebAddress.ReplaceSqlChar(), (byte)model.Status,
                              model.Remark, model.FundingId, model.LineNum)
                       .Append(" ) ");
                    }

                    sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                        .Append(" END TRY ")
                        .Append(" BEGIN CATCH ;  ")
                        .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                        .Append(" SELECT ERROR_MESSAGE() ;")
                        .Append(" END CATCH;");

                    string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                    if (message != "1")
                    {
                        taskEntity.Status = BUPStatus.Error;
                        taskEntity.Remark = message;
                        _bupTaskBusiness.Update(taskEntity);
                    }
                }

                return "";
            }
            else
            {
                return "Error: No Access.";
            }
        }

        public string ProcessSchool(DataTable dt, int userId, string originFileName, string filePath,
           UserBaseEntity user, out int identity, BUPProcessType processType = BUPProcessType.Upload, int communityId = 0)
        {
            identity = 0;
            List<BUPSchoolModel> dataList = new List<BUPSchoolModel>();
            stateList = _masterDataBusiness.GetStateSelectList().ToList();
            countyList = _masterDataBusiness.GetAllCounty().ToList();
            titleList = _masterDataBusiness.GetTitleSelectList(3).ToList();
            secondaryTitleList = _masterDataBusiness.GetTitleSelectList(4).ToList();
            schoolTypeList = _schoolBusiness.GetSchoolTypeList();

            List<NameModel> communityLists = _communityBusiness.GetCommunitiesByUser(user);
            //根据CommunitID获取所有PrimaryCommunity为当前Community的School

            List<NameModel> primarySchools = new List<NameModel>();
            if (communityId > 0)
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user, communityId);
            }
            else
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string multError = "";
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPSchoolModel model = new BUPSchoolModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);

                if (type != "school")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData().Trim();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);

                model.CommunityEngageID = help.NextData().Trim();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                var communityEngage = _communityBusiness.GetCommunityByEngageId(model.CommunityEngageID);
                if (communityEngage == null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: Community Engage ID does not match.", i + 2);
                }

                if (!communityLists.Any(e => e.EngageId == model.CommunityEngageID && e.Name == model.CommunityName))
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: You are not assigned to this community or you have no access for this community.", i + 2);
                }

                model.Name = help.NextData().Trim();
                if (model.Name == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);


                model.EngageID = help.NextData().Trim();
                if (model.EngageID == string.Empty && (model.Action == BUPAction.Delete || model.Action == BUPAction.Update))
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                if (model.Action == BUPAction.Update || model.Action == BUPAction.Delete)
                {
                    bool canAccessSchool = CheckIfCanAccessSchool(model.EngageID, primarySchools);
                    if (!canAccessSchool)
                        if (string.IsNullOrEmpty(multError))
                            multError = string.Format("#{0}: You are not assigned to this school or you have no access for this school.", i + 2);
                }

                model.InternalID = help.NextData().Trim();

                model.PhysicalAddress1 = help.NextData().Trim();
                if (model.PhysicalAddress1 == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Physical Address1 is incorrect.", i + 2);


                model.PhysicalAddress2 = help.NextData().Trim();
                model.City = help.NextData().Trim();
                if (model.City == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Physical Address City is incorrect.", i + 2);

                //先读取数据，获取State的值后，再给county赋值
                string county = help.NextData().Trim().ToLower();

                string state = help.NextData().Trim().ToLower();
                var stateEntity = stateList.Find(r => r.Name.ToLower() == state);
                if (stateEntity == null)
                {
                    if (model.Action == BUPAction.Insert)
                        return string.Format("#{0}: School Physical Address State is incorrect.", i + 2);

                    else model.StateId = 0;
                }
                else
                    model.StateId = stateEntity.ID;

                var countyEntity = countyList.Where(r => r.StateId == model.StateId && r.Name.ToLower() == county).FirstOrDefault();
                if (countyEntity == null)
                {
                    if (model.Action == BUPAction.Insert)
                        return string.Format("#{0}: School Physical Address County is incorrect.", i + 2);
                    else model.CountyId = 0;
                }
                else
                    model.CountyId = countyEntity.ID;


                model.Zip = help.NextData().Trim();
                if (model.Zip == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Physical Address ZIP is incorrect.", i + 2);


                model.MailingAddress1 = help.NextData().Trim();
                if (model.MailingAddress1 == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Mailing Address1 is incorrect.", i + 2);


                model.MailingAddress2 = help.NextData().Trim();

                model.MailingCity = help.NextData().Trim();
                if (model.MailingCity == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Mailing Address City is incorrect.", i + 2);

                county = help.NextData().Trim().ToLower();

                state = help.NextData().Trim().ToLower();
                stateEntity = stateList.Find(r => r.Name.ToLower() == state);
                if (stateEntity == null)
                {
                    if (model.Action == BUPAction.Insert)
                        return string.Format("#{0}: School Mailing Address State is incorrect.", i + 2);
                }
                else
                    model.MailingStateId = stateEntity.ID;

                countyEntity = countyList.Find(r => r.StateId == model.MailingStateId && r.Name.ToLower() == county);
                if (countyEntity == null)
                {
                    if (model.Action == BUPAction.Insert)
                        return string.Format("#{0}: School Mailing Address County is incorrect.", i + 2);
                    else
                        model.MailingCountyId = 0;
                }
                else
                    model.MailingCountyId = countyEntity.ID;

                model.MailingZip = help.NextData().Trim();
                if (model.MailingZip == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Mailing Address ZIP is incorrect.", i + 2);


                model.PhoneNumber = help.NextData().Trim();
                if (model.PhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Phone Number is incorrect.", i + 2);


                string phoneType = help.NextData().Trim().ToLower();
                switch (phoneType)
                {
                    case "h":
                        model.PhoneType = (byte)Core.Common.Enums.PhoneType.HomeNumber;
                        break;
                    case "c":
                        model.PhoneType = (byte)Core.Common.Enums.PhoneType.CellNumber;
                        break;
                    case "w":
                        model.PhoneType = (byte)Core.Common.Enums.PhoneType.WorkNumber;
                        break;
                    case "*clear*":
                        model.PhoneType = 127;
                        break;
                    default:
                        model.PhoneType = 0;
                        break;
                }
                SchoolTypeEntity schoolTypeEntity = new SchoolTypeEntity();
                string schoolType = help.NextData().Trim().ToLower();
                switch (schoolType)
                {
                    case "cc":
                        schoolTypeEntity = schoolTypeList.Find(r => r.Name == "Child Care Center-based");
                        if (schoolTypeEntity != null)
                            model.SchoolTypeId = schoolTypeEntity.ID;
                        break;
                    case "fc":
                        schoolTypeEntity = schoolTypeList.Find(r => r.Name == "Family Child Care (FCC)");
                        if (schoolTypeEntity != null)
                            model.SchoolTypeId = schoolTypeEntity.ID;
                        break;
                    case "he":
                        schoolTypeEntity = schoolTypeList.Find(r => r.Name == "Higher Ed");
                        if (schoolTypeEntity != null)
                            model.SchoolTypeId = schoolTypeEntity.ID;
                        break;
                    case "hs":
                        schoolTypeEntity = schoolTypeList.Find(r => r.Name == "Head Start");
                        if (schoolTypeEntity != null)
                            model.SchoolTypeId = schoolTypeEntity.ID;
                        break;
                    case "ps":
                        schoolTypeEntity = schoolTypeList.Find(r => r.Name == "Public School");
                        if (schoolTypeEntity != null)
                            model.SchoolTypeId = schoolTypeEntity.ID;
                        break;
                    case "*clear*":
                        model.SchoolTypeId = -1;
                        break;
                    default:
                        model.SchoolTypeId = 0;
                        break;
                }
                if (model.SchoolTypeId <= 0 && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Type is incorrect.", i + 2);



                int atRisk;
                int.TryParse(help.NextData(), out atRisk);
                model.AtRiskPercent = atRisk;

                int size;
                int.TryParse(help.NextData().Trim(), out size);
                model.SchoolSize = size;

                string primarySalutation = help.NextData().Trim().ToLower();

                switch (primarySalutation)
                {
                    case "mr.":
                        model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Mr;
                        break;
                    case "mrs.":
                        model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Mrs;
                        break;
                    case "ms.":
                        model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Ms;
                        break;
                    case "miss.":
                        model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Miss;
                        break;
                    case "dr.":
                        model.PrimarySalutation = (byte)Core.Common.Enums.UserSalutation.Dr;
                        break;
                    case "*clear*":
                        model.PrimarySalutation = 127;
                        break;
                    default:
                        model.PrimarySalutation = 0;
                        break;
                }

                model.PrimaryName = help.NextData().Trim();
                if (model.PrimaryName == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Primary Contact Name is incorrect.", i + 2);

                string title = help.NextData().Trim().ToLower();
                var titleEntity = titleList.Find(r => r.Name.ToLower() == title);
                if (titleEntity != null)
                    model.PrimaryTitleId = titleEntity.ID;
                else
                    model.PrimaryTitleId = 0;


                model.PrimaryPhone = help.NextData().Trim();
                if (model.PrimaryPhone == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Primary Contact Phone Number is incorrect.", i + 2);

                phoneType = help.NextData().Trim().ToLower();
                switch (phoneType)
                {
                    case "h":
                        model.PrimaryPhoneType = (byte)Core.Common.Enums.PhoneType.HomeNumber;
                        break;
                    case "c":
                        model.PrimaryPhoneType = (byte)Core.Common.Enums.PhoneType.CellNumber;
                        break;
                    case "w":
                        model.PrimaryPhoneType = (byte)Core.Common.Enums.PhoneType.WorkNumber;
                        break;
                    case "*clear*":
                        model.PrimaryPhoneType = 127;
                        break;
                    default:
                        model.PrimaryPhoneType = 0;
                        break;
                }


                model.PrimaryEmail = help.NextData().Trim();
                if (model.PrimaryEmail == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: School Primary Contact Email Address is incorrect.", i + 2);
                if (!string.IsNullOrEmpty(model.PrimaryEmail) && !CommonAgent.IsEmail(model.PrimaryEmail))
                    return string.Format("#{0}: School Primary Contact Email Address is incorrect.", i + 2);


                primarySalutation = help.NextData().Trim().ToLower();

                switch (primarySalutation)
                {
                    case "mr.":
                        model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Mr;
                        break;
                    case "mrs.":
                        model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Mrs;
                        break;
                    case "ms.":
                        model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Ms;
                        break;
                    case "miss.":
                        model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Miss;
                        break;
                    case "dr.":
                        model.SecondarySalutation = (byte)Core.Common.Enums.UserSalutation.Dr;
                        break;
                    case "*clear*":
                        model.SecondarySalutation = 127;
                        break;
                    default:
                        model.SecondarySalutation = 0;
                        break;
                }


                model.SecondaryName = help.NextData().Trim();

                title = help.NextData().Trim().ToLower();
                titleEntity = secondaryTitleList.Find(r => r.Name.ToLower() == title);
                if (titleEntity != null)
                    model.SecondaryTitleId = titleEntity.ID;
                else
                    model.SecondaryTitleId = 0;

                model.SecondaryPhoneNumber = help.NextData().Trim();

                if (model.SecondaryName != string.Empty && model.Action == BUPAction.Insert && model.SecondaryPhoneNumber == string.Empty)
                    return string.Format("#{0}: School Secondary Contact Phone Number is incorrect.", i + 2);


                phoneType = help.NextData().Trim().ToLower();
                switch (phoneType)
                {
                    case "h":
                        model.SecondaryPhoneType = (byte)Core.Common.Enums.PhoneType.HomeNumber;
                        break;
                    case "c":
                        model.SecondaryPhoneType = (byte)Core.Common.Enums.PhoneType.CellNumber;
                        break;
                    case "w":
                        model.SecondaryPhoneType = (byte)Core.Common.Enums.PhoneType.WorkNumber;
                        break;
                    case "*clear*":
                        model.SecondaryPhoneType = 127;
                        break;
                    default:
                        model.SecondaryPhoneType = 0;
                        break;
                }

                model.SecondaryEmail = help.NextData().Trim();

                if (model.SecondaryName != string.Empty && model.Action == BUPAction.Insert && model.SecondaryEmail == string.Empty)
                    return string.Format("#{0}: School Secondary Contact Email Address is incorrect.", i + 2);
                if (!string.IsNullOrEmpty(model.SecondaryEmail) && !CommonAgent.IsEmail(model.SecondaryEmail))
                    return string.Format("#{0}: School Secondary Contact Email Address is incorrect.", i + 2);


                model.Latitude = help.NextData().Trim();
                model.Longitude = help.NextData().Trim();
                if (string.IsNullOrEmpty(multError))
                {
                    model.Status = BUPStatus.Queued;
                    model.Remark = string.Empty;
                }
                else
                {
                    model.Status = BUPStatus.DataError;
                    model.Remark = multError;
                }
                model.LineNum = i + 2;

                dataList.Add(model);
            }

            BUPTaskEntity taskEntity = new BUPTaskEntity();

            taskEntity.Type = BUPType.School;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Queued;
            taskEntity.SendInvitation = false;
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");

                foreach (BUPSchoolModel model in dataList)
                {
                    sb.Append(";INSERT INTO dbo.[BUP_Schools]([TaskId],[Action],[CommunityName],[CommunityEngageID],[Name]")
                   .Append(",[EngageID],[InternalID],[PhysicalAddress1],[PhysicalAddress2],[City]")
                   .Append(",[CountyId],[StateId],[Zip],[MailingAddress1],[MailingAddress2]")
                    .Append(",[MailingCity],[MailingCountyId],[MailingStateId],[MailingZip],[PhoneNumber]")
                    .Append(",[PhoneType],[SchoolTypeId],[AtRiskPercent],[SchoolSize],[PrimarySalutation]")
                    .Append(",[PrimaryName],[PrimaryTitleId],[PrimaryPhone],[PrimaryPhoneType],[PrimaryEmail]")
                    .Append(",[SecondarySalutation],[SecondaryName],[SecondaryTitleId],[SecondaryPhoneNumber],[SecondaryPhoneType]")
                    .Append(",[SecondaryEmail],[Latitude],[Longitude],[Status],[Remark], LineNum)");

                    sb.Append(" VALUES (")
                        .AppendFormat("{0},{1},'{2}','{3}','{4}'", taskEntity.ID, (byte)model.Action,
                        model.CommunityName.ReplaceSqlChar(), model.CommunityEngageID.ReplaceSqlChar(), model.Name.ReplaceSqlChar())  //InternalID
                        .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'", model.EngageID.ReplaceSqlChar(), model.InternalID.ReplaceSqlChar(),
                        model.PhysicalAddress1.ReplaceSqlChar(), model.PhysicalAddress2.ReplaceSqlChar(), model.City.ReplaceSqlChar()) //City
                        .AppendFormat(",{0},{1},'{2}','{3}','{4}'"
                        , model.CountyId, model.StateId, model.Zip.ReplaceSqlChar(),
                        model.MailingAddress1.ReplaceSqlChar(), model.MailingAddress2.ReplaceSqlChar())   //MailingAddress2
                        .AppendFormat(",'{0}',{1},{2},'{3}','{4}'"
                        , model.MailingCity.ReplaceSqlChar(), model.MailingCountyId, model.MailingStateId,
                        model.MailingZip.ReplaceSqlChar(), model.PhoneNumber.ReplaceSqlChar())//PhoneNumber
                          .AppendFormat(",{0},{1},{2},{3},{4}"
                          , model.PhoneType, model.SchoolTypeId, model.AtRiskPercent, model.SchoolSize, model.PrimarySalutation)//PrimarySalutation
                           .AppendFormat(",'{0}',{1},'{2}',{3},'{4}'"
                          , model.PrimaryName.ReplaceSqlChar(), model.PrimaryTitleId, model.PrimaryPhone.ReplaceSqlChar(),
                          model.PrimaryPhoneType, model.PrimaryEmail.ReplaceSqlChar())//PrimaryEmail
                          .AppendFormat(",{0},'{1}',{2},'{3}',{4}"
                               , model.SecondarySalutation, model.SecondaryName.ReplaceSqlChar(),
                               model.SecondaryTitleId, model.SecondaryPhoneNumber.ReplaceSqlChar(), model.SecondaryPhoneType)//SecondaryPhoneType
                               .AppendFormat(",'{0}','{1}','{2}',{3},'{4}',{5}"
                                , model.SecondaryEmail.ReplaceSqlChar(), model.Latitude.ReplaceSqlChar(), model.Longitude.ReplaceSqlChar(),
                                (byte)model.Status, model.Remark, model.LineNum)//SecondaryPhoneType
                   .Append(" ) ");
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }
            return "";
        }

        public string ProcessClassroom(DataTable dt, int userId, string originFileName, string filePath,
            UserBaseEntity user, int communityId, out int identity, BUPProcessType processType = BUPProcessType.Upload)
        {
            identity = 0;
            List<BUPClassroomModel> dataList = new List<BUPClassroomModel>();

            //根据CommunitID获取所有PrimaryCommunity为当前Community的School
            List<NameModel> primarySchools = _schoolBusiness.GetPrimarySchools(user, communityId);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPClassroomModel model = new BUPClassroomModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);

                if (type != "classroom")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData().Trim();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);


                model.CommunityEngageID = help.NextData().Trim();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                if (i == 0)  //判断第一条记录输入的Community和选择的是否相同，之后的数据通过分组判断是否有不同的Community
                {
                    var condition = PredicateHelper.True<CommunityEntity>();
                    condition = condition.And(r => r.Name == model.CommunityName && r.CommunityId == model.CommunityEngageID);
                    if (!_communityBusiness.GetCommunityId(condition, user).Contains(communityId))
                    {
                        return string.Format("#{0}: The Districts that you typed and selected do not match.", i + 2);
                    }
                }

                model.SchoolName = help.NextData().Trim();
                if (model.SchoolName == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);

                model.SchoolEngageID = help.NextData().Trim();
                if (model.SchoolEngageID == string.Empty)
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                bool canAccessSchool = CheckIfCanAccessSchool(model.SchoolName, model.SchoolEngageID, primarySchools);
                if (!canAccessSchool)
                    return string.Format("#{0}: You are not assigned to this school or you have no access for this school.", i + 2);

                model.Name = help.NextData().Trim();

                model.ClassroomEngageId = help.NextData().Trim();
                if (model.ClassroomEngageId == string.Empty && (model.Action == BUPAction.Update || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Classroom Engage ID is incorrect.", i + 2);


                model.ClassroomInternalID = help.NextData().Trim();
                model.Status = BUPStatus.Queued;
                model.Remark = string.Empty;
                model.LineNum = i + 2;

                dataList.Add(model);
            }

            //判断是否是同一个Community
            if (dataList.GroupBy(r => new { r.CommunityName, r.CommunityEngageID }).Count() > 1)
            {
                return string.Format("Different Communities/Districts are found.");
            }


            BUPTaskEntity taskEntity = new BUPTaskEntity();

            taskEntity.Type = BUPType.Classroom;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Queued;
            taskEntity.SendInvitation = false;
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");

                foreach (BUPClassroomModel model in dataList)
                {
                    sb.Append("INSERT INTO dbo.[BUP_Classrooms](")
                        .Append(" [TaskId],[Action],[CommunityName],[CommunityEngageID],[SchoolName]")
                        .Append(" ,[SchoolEngageID],[Name],[ClassroomEngageId],[ClassroomInternalID],[Status],[Remark], LineNum)");

                    sb.Append(" VALUES (")
                        .AppendFormat("{0},{1},'{2}','{3}','{4}'", taskEntity.ID, (byte)model.Action, model.CommunityName.ReplaceSqlChar(),
                        model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())
                        .AppendFormat(",'{0}','{1}','{2}','{3}','{4}','{5}',{6}", model.SchoolEngageID.ReplaceSqlChar(), model.Name.ReplaceSqlChar(),
                        model.ClassroomEngageId.ReplaceSqlChar(), model.ClassroomInternalID.ReplaceSqlChar(),
                        (byte)model.Status, model.Remark, model.LineNum).Append(" ) ");
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }
            return "";
        }

        public string ProcessClass(DataTable dt, int userId, string originFileName, string filePath,
            UserBaseEntity user, out int identity, BUPProcessType processType = BUPProcessType.Upload, int communityId = 0)
        {
            identity = 0;
            List<BUPClassModel> dataList = new List<BUPClassModel>();

            List<NameModel> communityLists = _communityBusiness.GetCommunitiesByUser(user);
            //根据CommunitID获取所有PrimaryCommunity为当前Community的School
            List<NameModel> primarySchools = new List<NameModel>();
            if (communityId > 0)
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user, communityId);
            }
            else
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user);
            }

            List<NameModelWithSchool> classrooms = _classroomBusiness.GetClassroomsBySchools
                (primarySchools.Select(r => r.Name).ToList(), primarySchools.Select(r => r.EngageId).ToList(), user);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string multError = "";
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPClassModel model = new BUPClassModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);

                if (type != "class")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData().Trim();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);

                model.CommunityEngageID = help.NextData().Trim();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                var communityEngage = _communityBusiness.GetCommunityByEngageId(model.CommunityEngageID);
                if (communityEngage == null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: Community Engage ID does not match.", i + 2);
                }

                if (!communityLists.Any(e => e.EngageId == model.CommunityEngageID && e.Name.ToUpper() == model.CommunityName.ToUpper()))
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: You are not assigned to this community or you have no access for this community.", i + 2);
                }

                model.SchoolName = help.NextData().Trim();
                if (model.SchoolName == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);

                model.SchoolEngageID = help.NextData().Trim();
                if (model.SchoolEngageID == string.Empty)
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                var schoolEngage = _schoolBusiness.GetSchool(model.SchoolEngageID);
                if (schoolEngage == null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: School Engage ID does not match.", i + 2);
                }

                bool canAccessSchool = CheckIfCanAccessSchool(model.SchoolName, model.SchoolEngageID, primarySchools);
                if (!canAccessSchool)
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: You are not assigned to this school or you have no access for this school.", i + 2);

                model.Name = help.NextData().Trim();
                if (model.Name == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Class Name is incorrect.", i + 2);

                model.ClassEngageID = help.NextData().Trim();
                if (model.ClassEngageID == string.Empty && (model.Action == BUPAction.Update || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Class Engage ID is incorrect.", i + 2);

                model.ClassInternalID = help.NextData().Trim();
                #region 
                //David 03/17/2017
                if (model.ClassInternalID.Length > 150) {
                    model.ClassInternalID = "Invalid Class_Internal_ID";
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: Invalid Class_Internal_ID.", i + 2);

                }

                # endregion 


                string dayType = help.NextData().Trim().ToLower();
                if (dayType == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Class Day Type is incorrect.", i + 2);
                switch (dayType)
                {
                    case "fd":
                        model.DayType = (byte)DayType.FullDay;
                        break;
                    case "am":
                        model.DayType = (byte)DayType.Am;
                        break;
                    case "pm":
                        model.DayType = (byte)DayType.Pm;
                        break;
                    case "*clear*":
                        model.DayType = 127;
                        break;
                    default:
                        model.DayType = 0;
                        break;
                }

                model.ClassroomName = help.NextData().Trim();
                model.ClassroomEngageID = help.NextData().Trim();

                //check if can access classroom
                if (!string.IsNullOrEmpty(model.ClassroomEngageID))
                {
                    bool canAccessClassroom = CheckIfCanAccessClassroom(model.SchoolName, model.SchoolEngageID,
                        model.ClassroomName, model.ClassroomEngageID, classrooms);
                    if (!canAccessClassroom)
                    {
                        if (string.IsNullOrEmpty(multError))
                            multError = string.Format("#{0}: You have no access for this classroom or it is not under this school.", i + 2);
                    }
                }
                string teacherFirst = help.NextData();
                string teacherLast = help.NextData();
                string teacherEngageId = help.NextData();
                if (!string.IsNullOrEmpty(teacherFirst) || !string.IsNullOrEmpty(teacherLast) ||
                    !string.IsNullOrEmpty(teacherEngageId))
                {
                    var teacher = _userBusiness.GetTeacher(teacherFirst, teacherLast, teacherEngageId);
                    if (teacher != null)
                    {
                        var isExistTeacher = _schoolBusiness.IsExistTeacher(model.SchoolName, model.SchoolEngageID,
                            teacher.UserInfo.ID);
                        if (!isExistTeacher)
                        {
                            if (string.IsNullOrEmpty(multError))
                                multError = string.Format("#{0}: You have no access for this teacher or it is not under this school.", i + 2);
                        }
                        model.HomeroomTeacherFirst = teacherFirst;
                        model.HomeroomTeacherLast = teacherLast;
                        model.HomeroomTeacherEngageId = teacherEngageId;
                    }
                    else
                    {
                        return string.Format("#{0}: Teacher must exist in Engage first. Enter First, Last and Engage ID as in Engage.", i + 2);
                    }
                }
                string teacherStatus = help.NextData();
                switch (teacherStatus.ToLower())
                {
                    case "activate":
                        model.ClassStatus = BUPEntityStatus.Activate;
                        break;
                    case "inactivate":
                        model.ClassStatus = BUPEntityStatus.Inactivate;
                        break;
                    case "delete":
                        model.ClassStatus = BUPEntityStatus.Delete;
                        break;
                    default:
                        model.ClassStatus = 0;
                        break;
                }

                model.Status = BUPStatus.Queued;
                model.Remark = string.Empty;
                if (string.IsNullOrEmpty(multError))
                {
                    model.Status = BUPStatus.Queued;
                    model.Remark = string.Empty;
                }
                else
                {
                    model.Status = BUPStatus.DataError;
                    model.Remark = multError;
                }
                model.LineNum = i + 2;

                dataList.Add(model);
            }

            BUPTaskEntity taskEntity = new BUPTaskEntity();

            taskEntity.Type = BUPType.Class;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Pending;
            taskEntity.SendInvitation = false;
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");

                foreach (BUPClassModel model in dataList)
                {
                    sb.Append(";INSERT INTO dbo.[BUP_Classes](")
                        .Append(" [TaskId],[Action],[CommunityName],[CommunityEngageID],[SchoolName] ")
                        .Append(" ,[SchoolEngageID],[Name],[ClassEngageID],[ClassInternalID],[DayType] ")
                        .Append(" ,[ClassroomName],[ClassroomEngageID],[Status],[Remark],LineNum")
                        .Append(" ,HomeroomTeacherFirst,HomeroomTeacherLast,HomeroomTeacherEngageID,ClassStatus)");

                    sb.Append(" VALUES (")
                        .AppendFormat("{0},{1},'{2}','{3}','{4}'", taskEntity.ID, (byte)model.Action, model.CommunityName.ReplaceSqlChar(),
                        model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())
                        .AppendFormat(",'{0}','{1}','{2}','{3}',{4}", model.SchoolEngageID.ReplaceSqlChar(), model.Name.ReplaceSqlChar(),
                        model.ClassEngageID.ReplaceSqlChar(), model.ClassInternalID.ReplaceSqlChar(), (byte)model.DayType)
                        .AppendFormat(",'{0}','{1}',{2},'{3}',{4}", model.ClassroomName.ReplaceSqlChar(), model.ClassroomEngageID.ReplaceSqlChar(),
                        (byte)model.Status, model.Remark, model.LineNum)
                        .AppendFormat(",'{0}','{1}','{2}',{3}", model.HomeroomTeacherFirst.ReplaceSqlChar(), model.HomeroomTeacherLast.ReplaceSqlChar(),
                        model.HomeroomTeacherEngageId.ReplaceSqlChar(), (byte)model.ClassStatus)
                        .Append(" ) ");
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }
            return "";
        }

        public string ProcessTeacher(DataTable dt, int userId, string invitation, string originFileName, string filePath,
            UserBaseEntity user, out int identity, BUPProcessType processType = BUPProcessType.Upload, int communityId = 0)
        {
            identity = 0;
            List<BUPTeacherModel> dataList = new List<BUPTeacherModel>();

            List<NameModel> communityLists = _communityBusiness.GetCommunitiesByUser(user);
            //根据CommunitID获取所有PrimaryCommunity为当前Community的School
            List<NameModel> primarySchools = new List<NameModel>();

            if (communityId > 0)
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user, communityId);
            }
            else
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user);
            }

            List<NameModelWithSchool> primaryClasses = _classBusiness.GetClassesBySchools(
                primarySchools.Select(r => r.Name).ToList(), primarySchools.Select(r => r.EngageId).ToList(), user);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string multError = "";
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPTeacherModel model = new BUPTeacherModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);

                if (type != "teacher")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);


                model.CommunityEngageID = help.NextData();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                var communityEngage = _communityBusiness.GetCommunityByEngageId(model.CommunityEngageID);
                if (communityEngage == null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: Community Engage ID does not match.", i + 2);
                }

                if (!communityLists.Any(e => e.EngageId == model.CommunityEngageID && e.Name.ToUpper() == model.CommunityName.ToUpper()))
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: You are not assigned to this community or you have no access for this community.", i + 2);
                }

                model.SchoolName = help.NextData();
                if (model.SchoolName == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);


                model.SchoolEngageID = help.NextData();
                if (model.SchoolEngageID == string.Empty)
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                var schoolEngage = _schoolBusiness.GetSchool(model.SchoolEngageID);
                if (schoolEngage == null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: School Engage ID does not match.", i + 2);
                }

                bool canAccessSchool = CheckIfCanAccessSchool(model.SchoolName, model.SchoolEngageID, primarySchools);
                if (!canAccessSchool)
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: You are not assigned to this school or you have no access for this school.", i + 2);

                model.FirstName = help.NextData();
                if (model.FirstName == string.Empty)
                    return string.Format("#{0}: Teacher First Name is incorrect.", i + 2);


                model.MiddleName = help.NextData();

                model.LastName = help.NextData();
                if (model.LastName == string.Empty)
                    return string.Format("#{0}: Teacher Last Name is incorrect.", i + 2);


                model.TeacherEngageID = help.NextData();
                if (model.TeacherEngageID == string.Empty && (model.Action == BUPAction.Update || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Teacher Engage ID is incorrect.", i + 2);


                model.TeacherInternalId = help.NextData();
                if (model.TeacherInternalId == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Teacher_Internal_ID is incorrect.", i + 2);

                model.PrimaryPhoneNumber = help.NextData(); //Teacher_Phone_Number
                //if (model.PrimaryPhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                //    return string.Format("#{0}: Teacher Phone Number is incorrect.", i + 2);


                string phonType = help.NextData().ToLower();

                switch (phonType)
                {
                    case "w":
                    case "work":
                        model.PrimaryNumberType = (byte)PhoneType.WorkNumber;
                        break;
                    case "h":
                    case "home":
                        model.PrimaryNumberType = (byte)PhoneType.HomeNumber;
                        break;
                    case "c":
                    case "cell":
                        model.PrimaryNumberType = (byte)PhoneType.CellNumber;
                        break;
                    case "*clear*":
                        model.PrimaryNumberType = 127;
                        break;
                    default:
                        model.PrimaryNumberType = 0;
                        break;
                }

                model.PrimaryEmailAddress = help.NextData();
                if (model.PrimaryEmailAddress == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Teacher Primary Email is incorrect.", i + 2);
                if (!string.IsNullOrEmpty(model.PrimaryEmailAddress) && !CommonAgent.IsEmail(model.PrimaryEmailAddress))
                    return string.Format("#{0}: Teacher Primary Email is incorrect.", i + 2);

                if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && !string.IsNullOrEmpty(model.PrimaryEmailAddress) &&
                    dataList.Find(r => r.PrimaryEmailAddress == model.PrimaryEmailAddress && r.Action == model.Action &&
                        (!r.PrimaryEmailAddress.Contains("@1.com"))) != null)
                {
                    return string.Format("Duplicate record found on row #{0} with the same action.", i + 2);
                }

                model.SecondaryEmailAddress = help.NextData();
                if (!string.IsNullOrEmpty(model.SecondaryEmailAddress) && !CommonAgent.IsEmail(model.SecondaryEmailAddress))
                    return string.Format("#{0}: Teacher Secondary Email is incorrect.", i + 2);

                model.ClassName = help.NextData();
                model.ClassEngageID = help.NextData();

                //check if can access class
                if (!string.IsNullOrEmpty(model.ClassEngageID))
                {
                    bool canAccessClass = CheckIfCanAccessClass(model.SchoolName, model.SchoolEngageID,
                        model.ClassName, model.ClassEngageID, primaryClasses);
                    if (!canAccessClass)
                    {
                        if (string.IsNullOrEmpty(multError))
                            multError = string.Format("#{0}: You have no access for this class or it is not under this school.", i + 2);
                    }
                }

                model.ClassroomName = help.NextData();
                model.ClassroomEngageID = help.NextData();
                model.Teacher_TSDS_ID = help.NextData();
                string teacherStatus = help.NextData();
                switch (teacherStatus.ToLower())
                {
                    case "activate":
                        model.TeacherStatus = BUPEntityStatus.Activate;
                        break;
                    case "inactivate":
                        model.TeacherStatus = BUPEntityStatus.Inactivate;
                        break;
                    case "delete":
                        model.TeacherStatus = BUPEntityStatus.Delete;
                        break;
                    default:
                        model.TeacherStatus = 0;
                        break;
                }
                if (string.IsNullOrEmpty(multError))
                {
                    model.Status = BUPStatus.Queued;
                    model.Remark = string.Empty;
                }
                else
                {
                    model.Status = BUPStatus.DataError;
                    model.Remark = multError;
                }
                model.LineNum = i + 2;

                dataList.Add(model);
            }

            BUPTaskEntity taskEntity = new BUPTaskEntity();

            taskEntity.Type = BUPType.Teacher;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Queued;
            taskEntity.SendInvitation = (invitation == "1");
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");

                foreach (BUPTeacherModel model in dataList)
                {
                    sb.Append(";INSERT INTO dbo.[BUP_Teachers](")
                        .Append(" [TaskId],[Action],[CommunityName],[CommunityEngageID],[SchoolName]")
                        .Append(" ,[SchoolEngageID],[FirstName],[MiddleName],[LastName],[TeacherEngageID]")
                        .Append(" ,[TeacherInternalId],[PrimaryPhoneNumber],[PrimaryNumberType],[PrimaryEmailAddress],[SecondaryEmailAddress] ")
                        .Append(" ,[ClassName],[ClassEngageID],[ClassroomName],[ClassroomEngageID],[Status] ")
                        .Append(" ,[Remark],LineNum,Teacher_TSDS_ID,TeacherStatus)");

                    sb.Append(" VALUES (")
                        .AppendFormat("{0},{1},'{2}','{3}','{4}'"
                        , taskEntity.ID, (byte)model.Action, model.CommunityName.ReplaceSqlChar(),
                        model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())

                          .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'"
                        , model.SchoolEngageID.ReplaceSqlChar(), model.FirstName.ReplaceSqlChar(),
                        model.MiddleName.ReplaceSqlChar(), model.LastName.ReplaceSqlChar(), model.TeacherEngageID.ReplaceSqlChar())

                        .AppendFormat(",'{0}','{1}',{2},'{3}','{4}'"
                        , model.TeacherInternalId.ReplaceSqlChar(), model.PrimaryPhoneNumber.ReplaceSqlChar(),
                        model.PrimaryNumberType, model.PrimaryEmailAddress.ReplaceSqlChar(), model.SecondaryEmailAddress.ReplaceSqlChar())

                        .AppendFormat(",'{0}','{1}','{2}','{3}',{4}"
                        , model.ClassName.ReplaceSqlChar(), model.ClassEngageID.ReplaceSqlChar(),
                        model.ClassroomName.ReplaceSqlChar(), model.ClassroomEngageID.ReplaceSqlChar(), (byte)model.Status)

                        .AppendFormat(",'{0}',{1},'{2}',{3} ) ", model.Remark, model.LineNum, model.Teacher_TSDS_ID, (byte)model.TeacherStatus);
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }

            return "";
        }

        public string ProcessStudent(DataTable dt, int userId, string originFileName, string filePath, UserBaseEntity user,
            out int identity, BUPProcessType processType = BUPProcessType.Upload, int communityId = 0)
        {
            identity = 0;
            List<BUPStudentModel> dataList = new List<BUPStudentModel>();

            List<NameModel> communityLists = _communityBusiness.GetCommunitiesByUser(user);

            //根据CommunitID获取所有PrimaryCommunity为当前Community的School
            List<NameModel> primarySchools = new List<NameModel>();

            if (communityId > 0)
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user, communityId);
            }
            else
            {
                primarySchools = _schoolBusiness.GetPrimarySchools(user);
            }


            List<NameModelWithSchool> primaryClasses = _classBusiness.GetClassesBySchools(
                primarySchools.Select(r => r.Name).ToList(), primarySchools.Select(r => r.EngageId).ToList(), user);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string multError = "";
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPStudentModel model = new BUPStudentModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);

                if (type != "student")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);

                model.CommunityEngageID = help.NextData();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                var communityEngage = _communityBusiness.GetCommunityByEngageId(model.CommunityEngageID);
                if (communityEngage == null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: Community Engage ID does not match.", i + 2);
                }

                if (!communityLists.Any(e => e.EngageId == model.CommunityEngageID && e.Name.ToUpper() == model.CommunityName.ToUpper()))
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: You are not assigned to this community.", i + 2);
                }

                model.SchoolName = help.NextData();
                if (model.SchoolName == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);

                model.SchoolEngageID = help.NextData();
                if (model.SchoolEngageID == string.Empty)
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                var schoolEngage = _schoolBusiness.GetSchool(model.SchoolEngageID);
                if (schoolEngage == null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: School Engage ID does not match.", i + 2);
                }

                bool canAccessSchool = CheckIfCanAccessSchool(model.SchoolName, model.SchoolEngageID, primarySchools);
                if (!canAccessSchool)
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("#{0}: You are not assigned to this school or you have no access for this school.", i + 2);

                model.FirstName = help.NextData();
                if (model.FirstName == string.Empty)
                    return string.Format("#{0}: Student_First_Name is incorrect.", i + 2);

                model.MiddleName = help.NextData();

                model.LastName = help.NextData();
                if (model.LastName == string.Empty)
                    return string.Format("#{0}: Student_Last_Name is incorrect.", i + 2);

                model.StudentEngageId = help.NextData();
                if (model.StudentEngageId == string.Empty && (model.Action == BUPAction.Update || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Student_Engage_ID is incorrect.", i + 2);

                model.LocalStudentID = help.NextData();
                if (model.LocalStudentID == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Student_Internal_ID is incorrect.", i + 2);

                DateTime date;
                if (DateTime.TryParse(help.NextData(), out date))
                {
                    //系统中日期小于1900-1-1时，页面显示为空    和   为了防止只输入时间时，读取为当前日期
                    if (date >= DateTime.Parse("1900-1-1") && date.Date < DateTime.Now.Date)
                        model.BirthDate = date;
                    else
                        return string.Format("#{0}: Student_Birth_Date is incorrect.", i + 2);
                }
                else
                {
                    if (model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Student_Birth_Date is incorrect.", i + 2);
                    else
                        model.BirthDate = CommonAgent.MinDate;
                }

                if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && model.BirthDate != CommonAgent.MinDate &&
                    dataList.Find(r => r.FirstName == model.FirstName && r.LastName == model.LastName
                        && r.BirthDate == model.BirthDate && r.Action == model.Action) != null)
                {
                    if (string.IsNullOrEmpty(multError))
                        multError = string.Format("Duplicate record found on row #{0} with the same action.", i + 2);
                }

                string gender = help.NextData().Trim().ToLower();
                if (gender != "f" && gender != "m" && gender != "male" && gender != "female")
                {
                    model.Gender = 0;
                    if (model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Student_Gender is incorrect.", i + 2);
                }
                else
                {
                    if (gender == "m" || gender == "male")
                        model.Gender = (byte)1;
                    else if (gender == "f" || gender == "female")
                        model.Gender = (byte)2;
                }

                string ethnicity = help.NextData().ToLower();
                switch (ethnicity)
                {
                    case "african american":
                        model.Ethnicity = (byte)Ethnicity.African_American;
                        break;
                    case "alaskan":
                        model.Ethnicity = (byte)Ethnicity.Alaskan;
                        break;
                    case "native american":
                        model.Ethnicity = (byte)Ethnicity.Native_American;
                        break;
                    case "indian":
                        model.Ethnicity = (byte)Ethnicity.Indian;
                        break;
                    case "asian":
                        model.Ethnicity = (byte)Ethnicity.Asian;
                        break;
                    case "caucasian":
                    case "white":
                        model.Ethnicity = (byte)Ethnicity.White;
                        break;
                    case "hispanic":
                        model.Ethnicity = (byte)Ethnicity.Hispanic;
                        break;
                    case "multiracial":
                        model.Ethnicity = (byte)Ethnicity.Multiracial;
                        break;
                    case "other":
                        model.Ethnicity = (byte)Ethnicity.Other;
                        break;
                    case "*clear*":
                        model.Ethnicity = (byte)127;
                        break;
                    default:
                        model.Ethnicity = 0;
                        break;
                }

                model.TSDSStudentID = help.NextData();

                string classLevel = help.NextData().ToLower();
                if (classLevel == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Student_Class_Level is incorrect.", i + 2);
                switch (classLevel)
                {
                    case "pk":
                        model.GradeLevel = (byte)StudentGradeLevel.Prek;
                        break;
                    case "k":
                        model.GradeLevel = (byte)StudentGradeLevel.K;
                        break;
                    case "1":
                        model.GradeLevel = (byte)StudentGradeLevel.One_grade;
                        break;
                    case "2":
                        model.GradeLevel = (byte)StudentGradeLevel.Two_grade;
                        break;
                    case "*clear*":
                        model.GradeLevel = (byte)127;
                        break;
                    default:
                        model.GradeLevel = 0;
                        break;
                }

                model.ClassName = help.NextData();
                model.ClassEngageID = help.NextData();

                //check if can access class
                if (!string.IsNullOrEmpty(model.ClassEngageID))
                {
                    bool canAccessClass = CheckIfCanAccessClass(model.SchoolName, model.SchoolEngageID,
                        model.ClassName, model.ClassEngageID, primaryClasses);
                    if (!canAccessClass)
                    {
                        if (string.IsNullOrEmpty(multError))
                            multError = string.Format("#{0}: Class info does not match with School or Community.", i + 2);
                    }
                }

                model.ClassroomName = help.NextData();
                model.ClassroomEngageId = help.NextData();
                string assessmentLangage = help.NextData();
                if ((string.IsNullOrEmpty(assessmentLangage) ||
                     (assessmentLangage.ToLower() != "english" && assessmentLangage.ToLower() != "spanish" &&
                      assessmentLangage.ToLower() != "both"))
                    && model.Action == BUPAction.Insert)
                {
                    return string.Format("#{0}: Assessment_Language is incorrect.", i + 2);
                }
                switch (assessmentLangage.ToLower())
                {
                    case "english":
                        model.AssessmentLanguage = StudentAssessmentLanguage.English;
                        break;
                    case "spanish":
                        model.AssessmentLanguage = StudentAssessmentLanguage.Spanish;
                        break;
                    case "both":
                    case "":
                        model.AssessmentLanguage = StudentAssessmentLanguage.Bilingual;
                        break;
                }
                string studentStatus = help.NextData();
                switch (studentStatus.ToLower())
                {
                    case "activate":
                        model.StudentStatus = BUPEntityStatus.Activate;
                        break;
                    case "inactivate":
                        model.StudentStatus = BUPEntityStatus.Inactivate;
                        break;
                    case "delete":
                        model.StudentStatus = BUPEntityStatus.Delete;
                        break;
                    default:
                        model.StudentStatus = 0;
                        break;
                }
                if (string.IsNullOrEmpty(multError))
                {
                    model.Status = BUPStatus.Queued;
                    model.Remark = string.Empty;
                }
                else
                {
                    model.Status = BUPStatus.DataError;
                    model.Remark = multError;
                }
                model.LineNum = i + 2;

                dataList.Add(model);
            }

            BUPTaskEntity taskEntity = new BUPTaskEntity();

            taskEntity.Type = BUPType.Student;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Queued;
            taskEntity.SendInvitation = false;
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");

                foreach (BUPStudentModel model in dataList)
                {
                    sb.Append(";INSERT INTO dbo.[BUP_Students](")
                        .Append(" [TaskId],[Action],[CommunityName],[CommunityEngageID],[SchoolName]")
                        .Append(" ,[SchoolEngageID],[FirstName],[MiddleName],[LastName],[StudentEngageId]")
                        .Append(" ,[LocalStudentID],[BirthDate],[Gender],[Ethnicity],[TSDSStudentID] ")
                        .Append(" ,[GradeLevel],[ClassName],[ClassEngageID],[ClassroomName],[ClassroomEngageId] ")
                        .Append(" ,[Status],[Remark],LineNum,AssessmentLanguage,StudentStatus)");

                    sb.Append(" VALUES (")
                        .AppendFormat("{0},{1},'{2}','{3}','{4}'"
                            , taskEntity.ID, (byte)model.Action, model.CommunityName.ReplaceSqlChar(),
                            model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())

                        .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'"
                            , model.SchoolEngageID.ReplaceSqlChar(), model.FirstName.ReplaceSqlChar(),
                            model.MiddleName.ReplaceSqlChar(), model.LastName.ReplaceSqlChar(),
                            model.StudentEngageId.ReplaceSqlChar())

                        .AppendFormat(",'{0}','{1}',{2},{3},'{4}'"
                            , model.LocalStudentID.ReplaceSqlChar(), model.BirthDate, model.Gender, model.Ethnicity,
                            model.TSDSStudentID.ReplaceSqlChar())

                        .AppendFormat(",{0},'{1}','{2}','{3}','{4}'"
                            , model.GradeLevel, model.ClassName.ReplaceSqlChar(),
                            model.ClassEngageID.ReplaceSqlChar(), model.ClassroomName.ReplaceSqlChar(),
                            model.ClassroomEngageId.ReplaceSqlChar())

                        .AppendFormat(",{0},'{1}',{2},{3},{4} ) ", (byte)model.Status, model.Remark, model.LineNum,
                            (byte)model.AssessmentLanguage, (byte)model.StudentStatus);
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }

            return "";
        }

        public string ProcessCommunityUser(DataTable dt, int userId, bool isSpecialist, string invitation, string originFileName, string filePath,
            UserBaseEntity user, int communityId, out int identity, BUPProcessType processType = BUPProcessType.Upload)
        {
            identity = 0;
            string objName = isSpecialist ? "Specialist" : "User";
            List<BUPCommunityUserModel> dataList = new List<BUPCommunityUserModel>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPCommunityUserModel model = new BUPCommunityUserModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);


                if (!isSpecialist && type != "community/district user")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);
                if (isSpecialist && type != "community/district specialist")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);

                model.CommunityEngageID = help.NextData();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                if (i == 0)  //判断第一条记录输入的Community和选择的是否相同，之后的数据通过分组判断是否有不同的Community
                {
                    var condition = PredicateHelper.True<CommunityEntity>();
                    condition = condition.And(r => r.Name == model.CommunityName && r.CommunityId == model.CommunityEngageID);
                    if (!_communityBusiness.GetCommunityId(condition, user).Contains(communityId))
                    {
                        return string.Format("#{0}: The Districts that you typed and selected do not match.", i + 2);
                    }
                }

                model.SchoolName = help.NextData();
                if (model.SchoolName == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);

                model.SchoolEngageId = help.NextData();
                if (model.SchoolEngageId == string.Empty)
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                model.FirstName = help.NextData();
                if (model.FirstName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Community District {1} First Name is incorrect.", i + 2, objName);

                model.MiddleName = help.NextData();

                model.LastName = help.NextData();
                if (model.LastName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Community District {1} Last Name is incorrect.", i + 2, objName);

                model.EngageId = help.NextData();
                if (model.Action == BUPAction.Insert)
                    model.EngageId = _userBusiness.CommunityDelegateCode();
                else if (model.EngageId == string.Empty)
                    return string.Format("#{0}: Community District {1} Engage ID is incorrect.", i + 2, objName);

                model.InternalID = help.NextData();

                model.PrimaryPhoneNumber = help.NextData(); // Phone Number
                if (model.PrimaryPhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Community District {1} Phone Number is incorrect.", i + 2, objName);

                string phonType = help.NextData();

                switch (phonType)
                {
                    case "w":
                        model.PrimaryNumberType = (byte)PhoneType.WorkNumber;
                        break;
                    case "h":
                        model.PrimaryNumberType = (byte)PhoneType.HomeNumber;
                        break;
                    case "c":
                        model.PrimaryNumberType = (byte)PhoneType.CellNumber;
                        break;
                    case "*clear*":
                        model.PrimaryNumberType = 127;
                        break;
                    default:
                        model.PrimaryNumberType = 0;
                        break;
                }

                model.PrimaryEmailAddress = help.NextData();
                if (model.PrimaryEmailAddress == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Community District {1} Primary Email is incorrect.", i + 2, objName);
                if (!string.IsNullOrEmpty(model.PrimaryEmailAddress) && !CommonAgent.IsEmail(model.PrimaryEmailAddress))
                    return string.Format("#{0}: Community District {1} Primary Email is incorrect.", i + 2, objName);

                if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && !string.IsNullOrEmpty(model.PrimaryEmailAddress) &&
                    dataList.Find(r => r.FirstName == model.FirstName && r.LastName == model.LastName
                        && r.PrimaryEmailAddress == model.PrimaryEmailAddress && r.Action == model.Action) != null)
                {
                    return string.Format("Duplicate record found on row #{0} with the same action.", i + 2);
                }

                model.SecondaryEmailAddress = help.NextData();
                if (!string.IsNullOrEmpty(model.SecondaryEmailAddress) && !CommonAgent.IsEmail(model.SecondaryEmailAddress))
                    return string.Format("#{0}: Community District {1} Secondary Email is incorrect.", i + 2, objName);

                model.Status = BUPStatus.Queued;
                model.Remark = string.Empty;
                if (isSpecialist)
                    model.Role = Role.District_Community_Specialist;
                else
                    model.Role = Role.Community;
                model.LineNum = i + 2;

                dataList.Add(model);
            }

            //判断是否是同一个Community
            if (dataList.GroupBy(r => new { r.CommunityName, r.CommunityEngageID }).Count() > 1)
            {
                return string.Format("Different Communities/Districts are found.");
            }

            BUPTaskEntity taskEntity = new BUPTaskEntity();
            if (isSpecialist)
                taskEntity.Type = BUPType.CommunitySpecialist;
            else
                taskEntity.Type = BUPType.CommunityUser;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Queued;
            taskEntity.SendInvitation = (invitation == "1");
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");

                foreach (BUPCommunityUserModel model in dataList)
                {
                    sb.Append(";INSERT INTO dbo.[BUP_CommunityUsers](")
                        .Append(" [TaskId],[Action],[Role],[CommunityName],[CommunityEngageID],[SchoolName]")
                        .Append(" ,[SchoolEngageID],[FirstName],[MiddleName],[LastName],[EngageID]")
                        .Append(" ,[InternalID],[PrimaryPhoneNumber],[PrimaryNumberType],[PrimaryEmailAddress],[SecondaryEmailAddress] ")
                        .Append(" ,[Status],[Remark],LineNum)");

                    sb.Append(" VALUES (")
                        .AppendFormat("{0},{1},{2},'{3}','{4}','{5}'"
                        , taskEntity.ID, (byte)model.Action, (byte)model.Role,
                        model.CommunityName.ReplaceSqlChar(), model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())

                          .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'"
                        , model.SchoolEngageId.ReplaceSqlChar(), model.FirstName.ReplaceSqlChar(),
                        model.MiddleName.ReplaceSqlChar(), model.LastName.ReplaceSqlChar(), model.EngageId.ReplaceSqlChar())

                            .AppendFormat(",'{0}','{1}',{2},'{3}','{4}'"
                            , model.InternalID.ReplaceSqlChar(), model.PrimaryPhoneNumber.ReplaceSqlChar(),
                            model.PrimaryNumberType, model.PrimaryEmailAddress.ReplaceSqlChar(), model.SecondaryEmailAddress.ReplaceSqlChar())

                        .AppendFormat(",{0},'{1}',{2} ) ", (byte)model.Status, model.Remark, model.LineNum);
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }

            return "";
        }

        public string ProcessPrincipal(DataTable dt, int userId, bool isSpecialist, string originFileName, string filePath,
            string invitation, UserBaseEntity user, int communityId, out int identity, BUPProcessType processType = BUPProcessType.Upload)
        {
            identity = 0;
            string objName = isSpecialist ? "School Specialist" : "Principal/Director";
            List<BUPPrincipalModel> dataList = new List<BUPPrincipalModel>();

            //根据CommunitID获取所有PrimaryCommunity为当前Community的School
            List<NameModel> primarySchools = _schoolBusiness.GetPrimarySchools(user, communityId);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPPrincipalModel model = new BUPPrincipalModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);


                if (!isSpecialist && type != "principal/director")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);
                if (isSpecialist && type != "school specialist")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);

                model.CommunityEngageID = help.NextData();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                if (i == 0)  //判断第一条记录输入的Community和选择的是否相同，之后的数据通过分组判断是否有不同的Community
                {
                    var condition = PredicateHelper.True<CommunityEntity>();
                    condition = condition.And(r => r.Name == model.CommunityName && r.CommunityId == model.CommunityEngageID);
                    if (!_communityBusiness.GetCommunityId(condition, user).Contains(communityId))
                    {
                        return string.Format("#{0}: The Districts that you typed and selected do not match.", i + 2);
                    }
                }

                model.SchoolName = help.NextData();
                if (model.SchoolName == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);

                model.SchoolEngageId = help.NextData();
                if (model.SchoolEngageId == string.Empty)
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                bool canAccessSchool = CheckIfCanAccessSchool(model.SchoolName, model.SchoolEngageId, primarySchools);
                if (!canAccessSchool)
                    return string.Format("#{0}: You are not assigned to this school or you have no access for this school.", i + 2);

                model.FirstName = help.NextData();
                if (model.FirstName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: {1} First Name is incorrect.", i + 2, objName);

                model.MiddleName = help.NextData();

                model.LastName = help.NextData();
                if (model.LastName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: {1} Last Name is incorrect.", i + 2, objName);

                model.EngageId = help.NextData();
                if (model.Action == BUPAction.Insert)
                    model.EngageId = _userBusiness.PrincipalCode();
                else if (model.EngageId == string.Empty)
                    return string.Format("#{0}: {1} Engage ID is incorrect.", i + 2, objName);

                model.InternalID = help.NextData();

                model.PrimaryPhoneNumber = help.NextData(); // Phone Number
                if (model.PrimaryPhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: {1} Phone Number is incorrect.", i + 2, objName);

                string phonType = help.NextData();

                switch (phonType)
                {
                    case "w":
                        model.PrimaryNumberType = (byte)PhoneType.WorkNumber;
                        break;
                    case "h":
                        model.PrimaryNumberType = (byte)PhoneType.HomeNumber;
                        break;
                    case "c":
                        model.PrimaryNumberType = (byte)PhoneType.CellNumber;
                        break;
                    case "*clear*":
                        model.PrimaryNumberType = 127;
                        break;
                    default:
                        model.PrimaryNumberType = 0;
                        break;
                }

                model.PrimaryEmailAddress = help.NextData();
                if (model.PrimaryEmailAddress == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: {1} Primary Email is incorrect.", i + 2, objName);
                if (!string.IsNullOrEmpty(model.PrimaryEmailAddress) && !CommonAgent.IsEmail(model.PrimaryEmailAddress))
                    return string.Format("#{0}: {1} Primary Email is incorrect.", i + 2, objName);

                if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && !string.IsNullOrEmpty(model.PrimaryEmailAddress) &&
                    dataList.Find(r => r.FirstName == model.FirstName && r.LastName == model.LastName
                        && r.PrimaryEmailAddress == model.PrimaryEmailAddress && r.Action == model.Action) != null)
                {
                    return string.Format("Duplicate record found on row #{0} with the same action.", i + 2);
                }

                model.SecondaryEmailAddress = help.NextData();
                if (!string.IsNullOrEmpty(model.SecondaryEmailAddress) && !CommonAgent.IsEmail(model.SecondaryEmailAddress))
                    return string.Format("#{0}: {1} Secondary Email is incorrect.", i + 2, objName);

                model.Status = BUPStatus.Queued;
                model.Remark = string.Empty;
                if (isSpecialist)
                    model.Role = Role.School_Specialist;
                else
                    model.Role = Role.Principal;
                model.LineNum = i + 2;

                dataList.Add(model);
            }

            //判断是否是同一个Community
            if (dataList.GroupBy(r => new { r.CommunityName, r.CommunityEngageID }).Count() > 1)
            {
                return string.Format("Different Communities/Districts are found.");
            }

            BUPTaskEntity taskEntity = new BUPTaskEntity();

            taskEntity.Type = isSpecialist ? BUPType.SchoolSpecialist : BUPType.Principal;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Queued;
            taskEntity.SendInvitation = (invitation == "1");
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");

                foreach (BUPPrincipalModel model in dataList)
                {
                    sb.Append(";INSERT INTO dbo.[BUP_Principals](")
                        .Append(" [TaskId],[Action],[Role],[CommunityName],[CommunityEngageID],[SchoolName]")
                        .Append(" ,[SchoolEngageID],[FirstName],[MiddleName],[LastName],[EngageID]")
                        .Append(" ,[InternalID],[PrimaryPhoneNumber],[PrimaryNumberType],[PrimaryEmailAddress],[SecondaryEmailAddress] ")
                        .Append(" ,[Status],[Remark],LineNum)");

                    sb.Append(" VALUES (")
                        .AppendFormat("{0},{1},{2},'{3}','{4}','{5}'"
                        , taskEntity.ID, (byte)model.Action, (byte)model.Role,
                        model.CommunityName.ReplaceSqlChar(), model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())

                          .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'"
                        , model.SchoolEngageId.ReplaceSqlChar(), model.FirstName.ReplaceSqlChar(),
                        model.MiddleName.ReplaceSqlChar(), model.LastName.ReplaceSqlChar(), model.EngageId.ReplaceSqlChar())

                            .AppendFormat(",'{0}','{1}',{2},'{3}','{4}'"
                            , model.InternalID.ReplaceSqlChar(), model.PrimaryPhoneNumber.ReplaceSqlChar(), model.PrimaryNumberType,
                            model.PrimaryEmailAddress.ReplaceSqlChar(), model.SecondaryEmailAddress.ReplaceSqlChar())

                        .AppendFormat(",{0},'{1}',{2} ) ", (byte)model.Status, model.Remark, model.LineNum);
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }
            return "";
        }

        public string ProcessParent(DataTable dt, int userId, string originFileName, string filePath,
            out int identity, BUPProcessType processType = BUPProcessType.Upload)
        {
            identity = 0;
            List<BUPParentModel> dataList = new List<BUPParentModel>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HelpSolve help = new HelpSolve(dt.Rows, i);

                string action = help.NextData().ToLower();
                string type = help.NextData().ToLower().Trim();

                if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                {
                    continue;
                }

                BUPParentModel model = new BUPParentModel();

                if (action.StartsWith("i"))
                    model.Action = BUPAction.Insert;
                else if (action.StartsWith("u"))
                    model.Action = BUPAction.Update;
                else if (action.StartsWith("d"))
                    model.Action = BUPAction.Delete;
                else
                    return string.Format("#{0}: Action is incorrect.", i + 2);

                if (type != "parent")
                    return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                model.CommunityName = help.NextData();
                if (model.CommunityName == string.Empty)
                    return string.Format("#{0}: Community Name is incorrect.", i + 2);

                model.CommunityEngageID = help.NextData();
                if (model.CommunityEngageID == string.Empty)
                    return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                model.SchoolName = help.NextData();
                if (model.SchoolName == string.Empty)
                    return string.Format("#{0}: School Name is incorrect.", i + 2);

                model.SchoolEngageID = help.NextData();
                if (model.SchoolEngageID == string.Empty)
                    return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                model.FirstName = help.NextData();
                if (model.FirstName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Parent_First_Name is incorrect.", i + 2);

                model.MiddleName = help.NextData();

                model.LastName = help.NextData();
                if (model.LastName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Parent_Last_Name is incorrect.", i + 2);

                model.EngageId = help.NextData();
                if (model.EngageId == string.Empty && (model.Action == BUPAction.Update || model.Action == BUPAction.Delete))
                    return string.Format("#{0}: Parent_Engage_ID is incorrect.", i + 2);

                model.InternalID = help.NextData();

                model.PhoneNumber = help.NextData();
                if (model.PhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Parent_Phone_Number is incorrect.", i + 2);

                string phonType = help.NextData().ToLower();
                switch (phonType)
                {
                    case "w":
                        model.PhoneType = (byte)PhoneType.WorkNumber;
                        break;
                    case "h":
                        model.PhoneType = (byte)PhoneType.HomeNumber;
                        break;
                    case "c":
                        model.PhoneType = (byte)PhoneType.CellNumber;
                        break;
                    case "*clear*":
                        model.PhoneType = 127;
                        break;
                    default:
                        model.PhoneType = 0;
                        break;
                }

                model.PrimaryEmail = help.NextData();
                if (model.PrimaryEmail == string.Empty && model.Action == BUPAction.Insert)
                    return string.Format("#{0}: Parent_Primary_Email is incorrect.", i + 2);
                if (!string.IsNullOrEmpty(model.PrimaryEmail) && !CommonAgent.IsEmail(model.PrimaryEmail))
                    return string.Format("#{0}: Parent_Primary_Email is incorrect.", i + 2);

                if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && !string.IsNullOrEmpty(model.PrimaryEmail) &&
                        dataList.Find(r => r.FirstName == model.FirstName && r.LastName == model.LastName
                            && r.PrimaryEmail == model.PrimaryEmail && r.Action == model.Action) != null)
                {
                    return string.Format("Duplicate record found on row #{0} with the same action.", i + 2);
                }

                model.SecondEmail = help.NextData();
                if (!string.IsNullOrEmpty(model.SecondEmail) && !CommonAgent.IsEmail(model.SecondEmail))
                    return string.Format("#{0}: Parent_Secondary_Email is incorrect.", i + 2);

                model.Status = BUPStatus.Queued;
                model.Remark = string.Empty;
                model.LineNum = i + 2;
                dataList.Add(model);
            }


            BUPTaskEntity taskEntity = new BUPTaskEntity();

            taskEntity.Type = BUPType.Parent;
            taskEntity.ProcessType = processType;
            taskEntity.Status = BUPStatus.Queued;
            taskEntity.SendInvitation = false;
            taskEntity.Remark = string.Empty;
            taskEntity.RecordCount = dataList.Count;
            taskEntity.FailCount = 0;
            taskEntity.SuccessCount = 0;
            taskEntity.OriginFileName = originFileName;
            taskEntity.FilePath = filePath;
            taskEntity.CreatedBy = userId;
            taskEntity.UpdatedBy = userId;

            OperationResult result = _bupTaskBusiness.Insert(taskEntity);

            if (result.ResultType == OperationResultType.Success)
            {
                identity = taskEntity.ID;
                StringBuilder sb = new StringBuilder();
                sb.Append("BEGIN TRY ;");
                sb.Append("BEGIN TRANSACTION;");
                sb.Append("INSERT INTO dbo.[BUP_Parents](")
                  .Append(" [TaskId],  [Action], [CommunityName], [CommunityEngageID], [SchoolName]")
                  .Append(" ,[SchoolEngageID], [FirstName], [MiddleName], [LastName], [EngageID]")
                  .Append(" ,[InternalID], [PhoneNumber], [PhoneType], [PrimaryEmail], [SecondEmail] ")
                  .Append(" ,[Status],[Remark],LineNum)");
                for (int i = 0; i < dataList.Count; i++)
                {
                    BUPParentModel model = dataList[i];
                    if (i > 0)
                        sb.Append(" UNION ALL ");

                    sb.Append(" SELECT ")
                        .AppendFormat("{0},{1},'{2}','{3}','{4}'"
                            , taskEntity.ID, (byte)model.Action, model.CommunityName.ReplaceSqlChar(),
                            model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())
                        .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'"
                            , model.SchoolEngageID.ReplaceSqlChar(), model.FirstName.ReplaceSqlChar(),
                            model.MiddleName.ReplaceSqlChar(), model.LastName.ReplaceSqlChar(), model.EngageId.ReplaceSqlChar())

                        .AppendFormat(",'{0}','{1}',{2},'{3}','{4}'"
                            , model.InternalID.ReplaceSqlChar(), model.PhoneNumber.ReplaceSqlChar(), model.PhoneType,
                            model.PrimaryEmail.ReplaceSqlChar(), model.SecondEmail.ReplaceSqlChar())

                       .AppendFormat(",{0},'{1}',{2} ", (byte)model.Status, model.Remark, model.LineNum);
                }

                sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                    .Append(" END TRY ")
                    .Append(" BEGIN CATCH ;  ")
                    .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                    .Append(" SELECT ERROR_MESSAGE() ;")
                    .Append(" END CATCH;");

                string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                if (message != "1")
                {
                    taskEntity.Status = BUPStatus.Error;
                    taskEntity.Remark = message;
                    _bupTaskBusiness.Update(taskEntity);
                }
            }

            return "";
        }

        public string ProcessStateWide(DataTable dt, int userId, string invitation, string originFileName, string filePath,
           UserBaseEntity user, out int identity, BUPProcessType processType = BUPProcessType.Upload)
        {
            identity = 0;
            if (user.Role == Role.Super_admin)
            {
                List<BUPStatewideModel> dataList = new List<BUPStatewideModel>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HelpSolve help = new HelpSolve(dt.Rows, i);

                    string action = help.NextData().ToLower();
                    string type = help.NextData().ToLower().Trim();

                    if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                    {
                        continue;
                    }

                    BUPStatewideModel model = new BUPStatewideModel();

                    if (action.StartsWith("i"))
                        model.Action = BUPAction.Insert;
                    else if (action.StartsWith("u"))
                        model.Action = BUPAction.Update;
                    else if (action.StartsWith("d"))
                        model.Action = BUPAction.Delete;
                    else
                        return string.Format("#{0}: Action is incorrect.", i + 2);

                    if (type != "statewide agency")
                        return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                    model.CommunityName = help.NextData();
                    if (model.CommunityName == string.Empty)
                        return string.Format("#{0}: Community Name is incorrect.", i + 2);

                    model.CommunityEngageID = help.NextData();
                    if (model.CommunityEngageID == string.Empty)
                        return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                    model.SchoolName = help.NextData();
                    if (model.SchoolName == string.Empty)
                        return string.Format("#{0}: School Name is incorrect.", i + 2);

                    model.SchoolEngageID = help.NextData();
                    if (model.SchoolEngageID == string.Empty)
                        return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                    model.FirstName = help.NextData();
                    if (model.FirstName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                        return string.Format("#{0}: Statewide_Agency_First_Name is incorrect.", i + 2);

                    model.MiddleName = help.NextData();

                    model.LastName = help.NextData();
                    if (model.LastName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                        return string.Format("#{0}: Statewide_Agency_Last_Name is incorrect.", i + 2);

                    model.EngageId = help.NextData();
                    if (model.Action == BUPAction.Insert)
                        model.EngageId = _userBusiness.StatewideCode();
                    else if (model.EngageId == string.Empty)
                        return string.Format("#{0}: Statewide_Agency_Engage_ID is incorrect.", i + 2);

                    model.InternalID = help.NextData();

                    model.PhoneNumber = help.NextData();
                    if (model.PhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Statewide_Agency_Phone_Number is incorrect.", i + 2);

                    string phonType = help.NextData().ToLower();
                    switch (phonType)
                    {
                        case "w":
                            model.PhoneType = (byte)PhoneType.WorkNumber;
                            break;
                        case "h":
                            model.PhoneType = (byte)PhoneType.HomeNumber;
                            break;
                        case "c":
                            model.PhoneType = (byte)PhoneType.CellNumber;
                            break;
                        case "*clear*":
                            model.PhoneType = 127;
                            break;
                        default:
                            model.PhoneType = 0;
                            break;
                    }

                    model.PrimaryEmail = help.NextData();
                    if (model.PrimaryEmail == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Statewide_Agency_Primary_Email is incorrect.", i + 2);
                    if (!string.IsNullOrEmpty(model.PrimaryEmail) && !CommonAgent.IsEmail(model.PrimaryEmail))
                        return string.Format("#{0}: Statewide_Agency_Primary_Email is incorrect.", i + 2);

                    if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && !string.IsNullOrEmpty(model.PrimaryEmail) &&
                        dataList.Find(r => r.FirstName == model.FirstName && r.LastName == model.LastName
                            && r.PrimaryEmail == model.PrimaryEmail && r.Action == model.Action) != null)
                    {
                        return string.Format("Duplicate record found on row #{0} with the same action.", i + 2);
                    }

                    model.SecondEmail = help.NextData();
                    if (!string.IsNullOrEmpty(model.SecondEmail) && !CommonAgent.IsEmail(model.SecondEmail))
                        return string.Format("#{0}: Statewide_Agency_Secondary_Email is incorrect.", i + 2);

                    model.Status = BUPStatus.Queued;
                    model.Remark = string.Empty;
                    model.LineNum = i + 2;
                    dataList.Add(model);
                }

                BUPTaskEntity taskEntity = new BUPTaskEntity();

                taskEntity.Type = BUPType.StatewideAgency;
                taskEntity.ProcessType = processType;
                taskEntity.Status = BUPStatus.Queued;
                taskEntity.SendInvitation = (invitation == "1");
                taskEntity.Remark = string.Empty;
                taskEntity.RecordCount = dataList.Count;
                taskEntity.FailCount = 0;
                taskEntity.SuccessCount = 0;
                taskEntity.OriginFileName = originFileName;
                taskEntity.FilePath = filePath;
                taskEntity.CreatedBy = userId;
                taskEntity.UpdatedBy = userId;

                OperationResult result = _bupTaskBusiness.Insert(taskEntity);

                if (result.ResultType == OperationResultType.Success)
                {
                    identity = taskEntity.ID;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("BEGIN TRY ;");
                    sb.Append("BEGIN TRANSACTION;");
                    sb.Append("INSERT INTO dbo.[BUP_Statewides](")
                      .Append(" [TaskId],  [Action], [CommunityName], [CommunityEngageID], [SchoolName]")
                      .Append(" ,[SchoolEngageID], [FirstName], [MiddleName], [LastName], [EngageID]")
                      .Append(" ,[InternalID], [PhoneNumber], [PhoneType], [PrimaryEmail], [SecondEmail] ")
                      .Append(" ,[Status],[Remark],LineNum)");
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        BUPStatewideModel model = dataList[i];
                        if (i > 0)
                            sb.Append(" UNION ALL ");

                        sb.Append(" SELECT ")
                            .AppendFormat("{0},{1},'{2}','{3}','{4}'"
                                , taskEntity.ID, (byte)model.Action, model.CommunityName.ReplaceSqlChar(),
                                model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())
                            .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'"
                                , model.SchoolEngageID.ReplaceSqlChar(), model.FirstName.ReplaceSqlChar(),
                                model.MiddleName.ReplaceSqlChar(), model.LastName.ReplaceSqlChar(), model.EngageId.ReplaceSqlChar())

                            .AppendFormat(",'{0}','{1}',{2},'{3}','{4}'"
                                , model.InternalID.ReplaceSqlChar(), model.PhoneNumber.ReplaceSqlChar(), model.PhoneType,
                                model.PrimaryEmail.ReplaceSqlChar(), model.SecondEmail.ReplaceSqlChar())

                           .AppendFormat(",{0},'{1}',{2} ", (byte)model.Status, model.Remark, model.LineNum);
                    }

                    sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                        .Append(" END TRY ")
                        .Append(" BEGIN CATCH ;  ")
                        .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                        .Append(" SELECT ERROR_MESSAGE() ;")
                        .Append(" END CATCH;");

                    string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                    if (message != "1")
                    {
                        taskEntity.Status = BUPStatus.Error;
                        taskEntity.Remark = message;
                        _bupTaskBusiness.Update(taskEntity);
                    }
                }
                return "";
            }
            else
            {
                return "Error: No Access.";
            }
        }

        public string ProcessAuditor(DataTable dt, int userId, string invitation, string originFileName, string filePath,
            UserBaseEntity user, out int identity, BUPProcessType processType = BUPProcessType.Upload)
        {
            identity = 0;
            if (user.Role == Role.Super_admin)
            {
                List<BUPAuditorModel> dataList = new List<BUPAuditorModel>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HelpSolve help = new HelpSolve(dt.Rows, i);

                    string action = help.NextData().ToLower();
                    string type = help.NextData().ToLower().Trim();

                    if (string.IsNullOrEmpty(action) && string.IsNullOrEmpty(type))
                    {
                        continue;
                    }

                    BUPAuditorModel model = new BUPAuditorModel();

                    if (action.StartsWith("i"))
                        model.Action = BUPAction.Insert;
                    else if (action.StartsWith("u"))
                        model.Action = BUPAction.Update;
                    else if (action.StartsWith("d"))
                        model.Action = BUPAction.Delete;
                    else
                        return string.Format("#{0}: Action is incorrect.", i + 2);

                    if (type != "auditor")
                        return string.Format("#{0}: Transaction_Type is incorrect.", i + 2);

                    model.CommunityName = help.NextData();
                    if (model.CommunityName == string.Empty)
                        return string.Format("#{0}: Community Name is incorrect.", i + 2);

                    model.CommunityEngageID = help.NextData();
                    if (model.CommunityEngageID == string.Empty)
                        return string.Format("#{0}: Community Engage ID is incorrect.", i + 2);

                    model.SchoolName = help.NextData();
                    if (model.SchoolName == string.Empty)
                        return string.Format("#{0}: School Name is incorrect.", i + 2);

                    model.SchoolEngageID = help.NextData();
                    if (model.SchoolEngageID == string.Empty)
                        return string.Format("#{0}: School Engage ID is incorrect.", i + 2);

                    model.FirstName = help.NextData();
                    if (model.FirstName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                        return string.Format("#{0}: Auditor_First_Name is incorrect.", i + 2);

                    model.MiddleName = help.NextData();

                    model.LastName = help.NextData();
                    if (model.LastName == string.Empty && (model.Action == BUPAction.Insert || model.Action == BUPAction.Delete))
                        return string.Format("#{0}: Auditor_Last_Name is incorrect.", i + 2);

                    model.EngageId = help.NextData();
                    if (model.Action == BUPAction.Insert)
                        model.EngageId = _userBusiness.AuditorCode();
                    else if (model.EngageId == string.Empty)
                        return string.Format("#{0}: Auditor_Engage_ID is incorrect.", i + 2);

                    model.InternalID = help.NextData();

                    model.PhoneNumber = help.NextData();
                    if (model.PhoneNumber == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Auditor_Phone_Number is incorrect.", i + 2);

                    string phonType = help.NextData().ToLower();
                    switch (phonType)
                    {
                        case "w":
                            model.PhoneType = (byte)PhoneType.WorkNumber;
                            break;
                        case "h":
                            model.PhoneType = (byte)PhoneType.HomeNumber;
                            break;
                        case "c":
                            model.PhoneType = (byte)PhoneType.CellNumber;
                            break;
                        case "*clear*":
                            model.PhoneType = 127;
                            break;
                        default:
                            model.PhoneType = 0;
                            break;
                    }

                    model.PrimaryEmail = help.NextData();
                    if (model.PrimaryEmail == string.Empty && model.Action == BUPAction.Insert)
                        return string.Format("#{0}: Auditor_Primary_Email is incorrect.", i + 2);
                    if (!string.IsNullOrEmpty(model.PrimaryEmail) && !CommonAgent.IsEmail(model.PrimaryEmail))
                        return string.Format("#{0}: Auditor_Primary_Email is incorrect.", i + 2);

                    if (!string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && !string.IsNullOrEmpty(model.PrimaryEmail) &&
                        dataList.Find(r => r.FirstName == model.FirstName && r.LastName == model.LastName
                            && r.PrimaryEmail == model.PrimaryEmail && r.Action == model.Action) != null)
                    {
                        return string.Format("Duplicate record found on row #{0} with the same action.", i + 2);
                    }

                    model.SecondEmail = help.NextData();
                    if (!string.IsNullOrEmpty(model.SecondEmail) && !CommonAgent.IsEmail(model.SecondEmail))
                        return string.Format("#{0}: Auditor_Secondary_Email is incorrect.", i + 2);

                    model.Status = BUPStatus.Queued;
                    model.Remark = string.Empty;
                    model.LineNum = i + 2;
                    dataList.Add(model);
                }

                BUPTaskEntity taskEntity = new BUPTaskEntity();

                taskEntity.Type = BUPType.Auditor;
                taskEntity.ProcessType = processType;
                taskEntity.Status = BUPStatus.Queued;
                taskEntity.SendInvitation = (invitation == "1");
                taskEntity.Remark = string.Empty;
                taskEntity.RecordCount = dataList.Count;
                taskEntity.FailCount = 0;
                taskEntity.SuccessCount = 0;
                taskEntity.OriginFileName = originFileName;
                taskEntity.FilePath = filePath;
                taskEntity.CreatedBy = userId;
                taskEntity.UpdatedBy = userId;


                OperationResult result = _bupTaskBusiness.Insert(taskEntity);

                if (result.ResultType == OperationResultType.Success)
                {
                    identity = taskEntity.ID;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("BEGIN TRY ;");
                    sb.Append("BEGIN TRANSACTION;");
                    sb.Append("INSERT INTO dbo.[BUP_Auditors](")
                      .Append(" [TaskId],  [Action], [CommunityName], [CommunityEngageID], [SchoolName]")
                      .Append(" ,[SchoolEngageID], [FirstName], [MiddleName], [LastName], [EngageID]")
                      .Append(" ,[InternalID], [PhoneNumber], [PhoneType], [PrimaryEmail], [SecondEmail] ")
                      .Append(" ,[Status],[Remark],LineNum)");
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        BUPAuditorModel model = dataList[i];
                        if (i > 0)
                            sb.Append(" UNION ALL ");

                        sb.Append(" SELECT ")
                            .AppendFormat("{0},{1},'{2}','{3}','{4}'"
                                , taskEntity.ID, (byte)model.Action, model.CommunityName.ReplaceSqlChar(),
                                model.CommunityEngageID.ReplaceSqlChar(), model.SchoolName.ReplaceSqlChar())
                            .AppendFormat(",'{0}','{1}','{2}','{3}','{4}'"
                                , model.SchoolEngageID.ReplaceSqlChar(), model.FirstName.ReplaceSqlChar(),
                                model.MiddleName.ReplaceSqlChar(), model.LastName.ReplaceSqlChar(), model.EngageId.ReplaceSqlChar())

                            .AppendFormat(",'{0}','{1}',{2},'{3}','{4}'"
                                , model.InternalID.ReplaceSqlChar(), model.PhoneNumber.ReplaceSqlChar(), model.PhoneType,
                                model.PrimaryEmail.ReplaceSqlChar(), model.SecondEmail.ReplaceSqlChar())

                           .AppendFormat(",{0},'{1}',{2} ", (byte)model.Status, model.Remark, model.LineNum);
                    }

                    sb.Append(";SELECT '1' ;COMMIT TRANSACTION;   ")
                        .Append(" END TRY ")
                        .Append(" BEGIN CATCH ;  ")
                        .Append("IF @@TRANCOUNT > 0  BEGIN ROLLBACK TRANSACTION; END")
                        .Append(" SELECT ERROR_MESSAGE() ;")
                        .Append(" END CATCH;");

                    string message = _bupTaskBusiness.ExecuteCommunitySql(sb.ToString());
                    if (message != "1")
                    {
                        taskEntity.Status = BUPStatus.Error;
                        taskEntity.Remark = message;
                        _bupTaskBusiness.Update(taskEntity);
                    }
                }
                return "";
            }
            else
            {
                return "Error: No Access.";
            }
        }

        #endregion



        #region InvalidateHeads

        public bool ValidateCommunityExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "Community_Physical_Address1", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Physical_Address2", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_City", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_State", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_ZIP", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false  //J

                   || string.Equals(help.NextColumn(), "Community_Phone_Number_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Primary_Contact_Salutation", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Primary_Contact_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Primary_Contact_Title", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Primary_Contact_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false  //K

                   || string.Equals(help.NextColumn(), "Community_Primary_Contact_Phone_Number_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Primary_Contact_Email_Address", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Secondary_Contact_Salutation", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Secondary_Contact_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Secondary_Contact_Title", StringComparison.CurrentCultureIgnoreCase) == false  //T

                   || string.Equals(help.NextColumn(), "Community_Secondary_Contact_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Secondary_Contact_Phone_Number_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Secondary_Contact_Email_Address", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Web_Address", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateSchoolExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Physical_Address1", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Physical_Address2", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Physical_Address_City", StringComparison.CurrentCultureIgnoreCase) == false  //I

                   || string.Equals(help.NextColumn(), "School_Physical_Address_County", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Physical_Address_State", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Physical_Address_ZIP", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Mailing_Address1", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Mailing_Address2", StringComparison.CurrentCultureIgnoreCase) == false  //N

                   || string.Equals(help.NextColumn(), "School_Mailing_Address_City", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Mailing_Address_County", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Mailing_Address_State", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Mailing_Address_ZIP", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false  //S

                   || string.Equals(help.NextColumn(), "School_Phone_Number_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Percent_At_Risk", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Size", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Primary_Contact_Salutation", StringComparison.CurrentCultureIgnoreCase) == false //X

                   || string.Equals(help.NextColumn(), "School_Primary_Contact_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Primary_Contact_Title", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Primary_Contact_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Primary_Contact_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Primary_Contact_Email_Address", StringComparison.CurrentCultureIgnoreCase) == false  //S

                   || string.Equals(help.NextColumn(), "School_Secondary_Contact_Salutation", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Secondary_Contact_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Secondary_Contact_Title", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Secondary_Contact_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Secondary_Contact_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false //AH


                   || string.Equals(help.NextColumn(), "School_Secondary_Contact_Email_Address", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Latitude", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Longitude", StringComparison.CurrentCultureIgnoreCase) == false; //AH
        }

        private bool ValidateClassroomExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateClassExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Day_Type", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Classroom_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Homeroom_Teacher_First", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Homeroom_Teacher_Last", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Homeroom_Teacher_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Status", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateTeacherExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Teacher_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Name", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Class_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_TSDS_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Teacher_Status", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateStudentExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Student_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Birth_Date", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Gender", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Ethnicity", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_TSDS_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Class_Level", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Class_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Class_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Classroom_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Assessment_Language", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Student_Status", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateCommunityUserExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Community_District_User_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_User_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateCommunitySpecialistExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_District_Specialist_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidatePrincipalExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Principal_Director_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Principal_Director_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateSchoolSpecialistExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "School_Specialist_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Specialist_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateParentExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Parent_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Parent_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateStateWideExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Statewide_Agency_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Statewide_Agency_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        private bool ValidateAuditorExcel(DataTable dt)
        {
            GetColumnData help = new GetColumnData(dt);
            return string.Equals(help.NextColumn(), "Action", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Transaction_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Community_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "School_Name", StringComparison.CurrentCultureIgnoreCase) == false //D

                   || string.Equals(help.NextColumn(), "School_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_First_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_Middle_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_Last_Name", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_Engage_ID", StringComparison.CurrentCultureIgnoreCase) == false

                   || string.Equals(help.NextColumn(), "Auditor_Internal_ID", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_Phone_Number", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_Phone_Type", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_Primary_Email", StringComparison.CurrentCultureIgnoreCase) == false
                   || string.Equals(help.NextColumn(), "Auditor_Secondary_Email", StringComparison.CurrentCultureIgnoreCase) == false;
        }

        #endregion


        #region InvalidateFormat
        public string InvalidateFile(HttpPostedFileBase postFileBase)
        {
            if (postFileBase == null || postFileBase.ContentLength == 0)
                return "Please select a valid file.";

            string fileType = string.Empty;

            if (postFileBase.ContentLength == 0)
                return "Please select a valid file.";

            string[] name = postFileBase.FileName.Split('.');
            fileType = name[name.Length - 1];
            if (string.IsNullOrEmpty(fileType) || (fileType.ToLower() != "xls" && fileType.ToLower() != "xlsx"))
                return "Please select a valid excel.";

            return "";
        }

        public string InvalidateFile(string uploadPath, int count, BUPType type, out DataTable dt)
        {
            dt = new DataTable();
            string strCNN = string.Empty;

            strCNN = "Provider=Microsoft.ACE.OleDb.12.0;Data Source = " + uploadPath + ";Extended Properties = 'Excel 12.0;HDR=Yes;IMEX=1;'";

            OleDbConnection cnn = new OleDbConnection(strCNN);
            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                cnn.Dispose();
                _log.Debug(ex);

                return ex.Message;
            }

            DataTable schemaTable = cnn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

            string tableName = string.Empty;
            foreach (DataRow dr in schemaTable.Rows)
            {
                tableName = dr[2].ToString().Trim();
                if (tableName.IndexOf("_FilterDatabase") < 0 && tableName.IndexOf("Instructions") < 0)//
                    break;
            }
            if (tableName.StartsWith("'"))
            {
                tableName = tableName.Replace("'", "");
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
                tableName = "'" + tableName + "'";
            }
            else
            {
                tableName = tableName.EndsWith("$") == false ? tableName + "$" : tableName;
            }

            string strSQL = " SELECT * FROM [" + tableName + "]";

            OleDbDataAdapter cmd = new OleDbDataAdapter(strSQL, cnn);
            cmd.Fill(dt);
            cnn.Close();
            if (dt == null || dt.Rows.Count < 1 || dt.Columns.Count < count)
            {
                return "The template is incorrect.";
            }

            try
            {
                switch (type)
                {
                    case BUPType.Community:
                        if (ValidateCommunityExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.School:
                        if (ValidateSchoolExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.Classroom:
                        if (ValidateClassroomExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.Class:
                        if (ValidateClassExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.Teacher:
                        if (ValidateTeacherExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.Student:
                        if (ValidateStudentExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.CommunityUser:
                        if (ValidateCommunityUserExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.CommunitySpecialist:
                        if (ValidateCommunitySpecialistExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.Principal:
                        if (ValidatePrincipalExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.SchoolSpecialist:
                        if (ValidateSchoolSpecialistExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.Parent:
                        if (ValidateParentExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.StatewideAgency:
                        if (ValidateStateWideExcel(dt))
                            return "The template is incorrect.";
                        break;
                    case BUPType.Auditor:
                        if (ValidateAuditorExcel(dt))
                            return "The template is incorrect.";
                        break;
                    default:
                        return "Can not find action type.";
                }

            }
            catch (Exception ex)
            {
                _log.Debug(ex);
                return ex.Message;
            }

            return "";
        }
        #endregion

        private bool CheckIfCanAccessSchool(string schoolName, string schoolEngageId, List<NameModel> schools)
        {
            return schools.Count > 0 && schools.Find(r => r.EngageId == schoolEngageId && r.Name.ToUpper() == schoolName.ToUpper()) != null;
        }

        private bool CheckIfCanAccessSchool(string schoolEngageId, List<NameModel> schools)
        {
            return schools.Count > 0 && schools.Find(r => r.EngageId == schoolEngageId) != null;
        }


        private bool CheckIfCanAccessClassroom(string schoolName, string schoolEngageId,
            string classroomName, string classroomEngageId, List<NameModelWithSchool> classrooms)
        {
            bool canAccess = classrooms.Count > 0 &&
                classrooms.Find(r => r.SchoolName == schoolName && r.SchoolEngageId == schoolEngageId &&
                    r.EngageId == classroomEngageId && r.Name == classroomName) != null;
            return canAccess;
        }

        private bool CheckIfCanAccessClass(string schoolName, string schoolEngageId,
            string className, string classEngageId, List<NameModelWithSchool> classes)
        {
            bool canAccess = classes.Count > 0 &&
                classes.Find(r => r.SchoolName == schoolName && r.SchoolEngageId == schoolEngageId &&
                r.EngageId == classEngageId && r.Name == className) != null;
            return canAccess;
        }
    }
}
