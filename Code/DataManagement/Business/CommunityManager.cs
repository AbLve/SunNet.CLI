using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.DataProcess;
using Sunnet.Cli.Business.DataProcess.Models;
using Sunnet.Cli.Business.MasterData;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.BUP.Entities;
using Sunnet.Cli.Core.BUP.Models;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.DataProcess;
using Sunnet.Cli.Core.DataProcess.Entities;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Students.Enums;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.File;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;

namespace DataManagement.Business
{
    public class CommunityManager : Base
    {
        MasterDataBusiness _masterDataBusiness;
        UserBusiness _userBusiness;
        CommunityBusiness _communityBusiness;
        SchoolBusiness _schoolBusiness;
        BUPTaskBusiness _bupTaskBusiness;
        BUPProcessBusiness _bupProcessBusiness;
        ClassBusiness _classBusiness;
        StudentBusiness _studentBusiness;
        ISunnetLog _log;
        
        public CommunityManager()
        {
            _masterDataBusiness = new MasterDataBusiness(UnitWorkContext);
            _userBusiness =new UserBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness();
            _schoolBusiness = new SchoolBusiness();
            _bupTaskBusiness = new BUPTaskBusiness();
            _bupProcessBusiness=new BUPProcessBusiness();
            _classBusiness = new ClassBusiness();
            _studentBusiness = new StudentBusiness();
            _log = ObjectFactory.GetInstance<ISunnetLog>();
        }
        public bool PendingSchool()
        {
            //获取所有“提交的任务”
            var pendingList = _bupTaskBusiness.GetList(BUPType.Community).Where(c => c.Status == BUPStatus.Pending);
            foreach (var pending in pendingList)
            {
                BUPTaskBusiness.Start(pending.ID, pending.CreatedBy);
            }

            return false;
        }
    }
}
