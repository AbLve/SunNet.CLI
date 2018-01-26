using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataManagement.App;
using Newtonsoft.Json;
using StructureMap;
using Sunnet.Cli.Business.BUP;
using Sunnet.Cli.Business.BUP.Model;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.DataProcess;
using Sunnet.Cli.Business.DataProcess.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.BUP;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.DataProcess;
using Sunnet.Cli.Core.DataProcess.Entities;
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
    public class SingleRosterManager :Base
    {
        private readonly UserBusiness _userBusiness;
        private readonly CommunityBusiness _communityBusiness;
        private readonly SchoolBusiness _schoolBusiness;
        private readonly DataProcessBusiness _processBus;
        private readonly BUPTaskBusiness _bupTaskBusiness;
        private readonly ClassBusiness _classBusiness;
        private readonly StudentBusiness _studentBusiness;

        public SingleRosterManager()
        {
            _userBusiness=new UserBusiness(UnitWorkContext);
            _communityBusiness = new CommunityBusiness();
            _schoolBusiness = new SchoolBusiness();
            _bupTaskBusiness = new BUPTaskBusiness();
            _processBus = new DataProcessBusiness(UnitWorkContext);
            _classBusiness = new ClassBusiness();
            _studentBusiness = new StudentBusiness();
        }

        public bool QueuedSingleRoster()
        {
            try
            {
                LogManager.Info("SingleRosterManager.QueuedSingleRoster() begins to execute");

                var pendingList = _processBus.GetDataGroupListByProcessStatus(ProcessStatus.Queued);

                int listCount = pendingList.Count;
                LogManager.Info("There are " + listCount + " subtasks In this PendingRoster task.");

                for (int i = 0; i < listCount; i++)
                {

                    try
                    {
                        LogManager.Info("#" + (i + 1) + " PendingRoster Sub Task(ID=" + pendingList[i].ID + ") Start.");

                        QueuedSingleRosterByOne(pendingList[i]);

                        LogManager.Info("#" + (i + 1) + " PendingRoster Sub Task(ID=" + pendingList[i].ID + ") End.");
                    }
                    catch (Exception e)
                    {
                        LogManager.Info("#" + (i + 1) + " PendingRoster Sub Task(ID=" + pendingList[i].ID + " CreatedBy=" + pendingList[i].CreatedBy + "), exception：" + e.Message);
                    }
                }

                foreach (var pending in pendingList)
                {
                    QueuedSingleRosterByOne(pending);
                }

                LogManager.Info("SingleRosterManager.QueuedSingleRoster() execution end");
            }
            catch (Exception e)
            {
                LogManager.Info("SingleRosterManager.QueuedSingleRoster() exception:" + e.Message);
                return false;
            }

            return false;
        }

        private bool QueuedSingleRosterByOne(DataGroupEntity groupModel)
        {
            DataProcessBusiness.Start(groupModel.ID,groupModel.CreatedBy);
            return false;
        }
    }
}
