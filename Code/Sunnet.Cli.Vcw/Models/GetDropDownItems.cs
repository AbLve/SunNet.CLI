using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Vcw.Models;
using Sunnet.Cli.UIBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/11/20 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/11/20 12:15:10
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;
using System.Web.Mvc;
using System.Linq.Expressions;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Core.Vcw.Entities;
using System.Data.Entity.Core.Objects.SqlClient;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Schools;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Schools.Entities;

namespace Sunnet.Cli.Vcw.Models
{
    public static class GetDropDownItems
    {
        /// <summary>
        /// Coach查找Community,School和Teacher
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public static List<object> GetItems(int userid)
        {
            List<TeacherListModel> teacherList = new UserBusiness().GetAssignedTeachersByCoach(userid).ToList();
            List<SelectListItem> DropdownSchools = new List<SelectListItem>();
            List<SelectListItem> DropdownTeachers = new List<SelectListItem>();
            List<SelectListItem> DropdownCommunities = new UserBusiness().GetAssignedCommunities(userid).ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            foreach (TeacherListModel item in teacherList)
            {
                foreach (SelectItemModel school in item.Schools)
                {
                    if (!DropdownSchools.Select(a => a.Value).Contains(school.ID.ToString()))//去除School重复选项
                    {
                        DropdownSchools.Add(new SelectListItem()
                        {
                            Value = school.ID.ToString(),
                            Text = school.Name
                        });
                    }
                }

                DropdownTeachers.Add(new SelectListItem
                {
                    Text = item.TeacherName,
                    Value = item.TeacherUserId.ToString()
                });
            }
            if (DropdownSchools.Count > 0)
                DropdownSchools = DropdownSchools.OrderBy(r => r.Text).ToList();
            DropdownSchools.AddDefaultItem(ViewTextHelper.DefaultAllText, "-1");
            DropdownTeachers.AddDefaultItem(ViewTextHelper.DefaultAllText, "-1");


            List<object> dropdownData = new List<object> { DropdownTeachers, DropdownSchools, DropdownCommunities };
            return dropdownData;
        }

        /// <summary>
        /// PM查找Community和Coach
        /// </summary>
        /// <param name="pmId"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public static List<object> GetItemsByPM(int pmId)
        {
            List<SelectItemModel> CoachesModels = new UserBusiness().GetCoachByPM(pmId);
            List<SelectListItem> Coaches = new List<SelectListItem>();
            List<SelectListItem> Communities = new List<SelectListItem>();
            Coaches = CoachesModels.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            Communities = new UserBusiness().GetAssignedCommunities(pmId)
                .ToSelectList().AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            List<object> dropdownData = new List<object> { Coaches, Communities };
            return dropdownData;
        }

        /// <summary>
        /// 用于 PM-Teachers 下拉框数据源
        /// </summary>
        /// <param name="pmId"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public static List<object> GetItemsByPM_Teacher(int pmId)
        {
            List<SelectListItem> DropdownSchools = new List<SelectListItem>(); //school下拉框
            List<SelectListItem> DropdownTeachers = new List<SelectListItem>(); //teacher下拉框
            List<SelectItemModel> CoachesModels = new UserBusiness().GetCoachByPM(pmId);
            List<SelectListItem> Coaches = new List<SelectListItem>(); //coach下拉框
            List<SelectListItem> Communities = new List<SelectListItem>(); //community下拉框
            List<int> CoachIds = new List<int>();
            Coaches = CoachesModels.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            Communities = new UserBusiness().GetAssignedCommunities(pmId)
                .ToSelectList().AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            List<TeacherListModel> teacherList = new UserBusiness().GetTeacherListByPm(pmId);
            teacherList = teacherList.DistinctBy(e => e.TeacherUserId).ToList();
            foreach (TeacherListModel item in teacherList)
            {
                foreach (SelectItemModel school in item.Schools)
                {
                    if (!DropdownSchools.Select(a => a.Value).Contains(school.ID.ToString()))//去除School重复选项
                    {
                        DropdownSchools.Add(new SelectListItem()
                        {
                            Value = school.ID.ToString(),
                            Text = school.Name
                        });
                    }
                }

                DropdownTeachers.Add(new SelectListItem
                {
                    Text = item.TeacherName,
                    Value = item.TeacherUserId.ToString()
                });
            }
            DropdownSchools = DropdownSchools.OrderBy(r => r.Text).AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            DropdownTeachers = DropdownTeachers.OrderBy(r => r.Text).AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();
            List<object> dropdownData = new List<object> { Communities, Coaches, DropdownSchools, DropdownTeachers };
            return dropdownData;
        }

        /// <summary>
        /// 筛选File的ID
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static Expression<Func<Vcw_FileEntity, bool>> GetNumberExpression(string number)
        {
            Expression<Func<Vcw_FileEntity, bool>> expression_number = (r => SqlFunctions.Replicate(
                      (ConvertNumber.NumBefore +
                      SqlFunctions.Replicate(ConvertNumber.NumStr, ConvertNumber.NumCount - SqlFunctions.StringConvert((double)r.ID).Trim().Length)
                      + SqlFunctions.StringConvert((double)r.ID).Trim()), 1).Contains(number));
            return expression_number;
        }

        /// <summary>
        /// Coach查找Community,School和Teacher
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public static List<object> GetExternalItems(UserBaseEntity userInfo)
        {
            CommunityBusiness _communityBusiness = new CommunityBusiness();
            List<SelectListItem> DropdownCommunities = _communityBusiness.GetCommunitySelectList(userInfo).ToSelectList()
                .AddDefaultItem(ViewTextHelper.DefaultAllText, "-1").ToList();

            SchoolBusiness _schoolBusiness = new SchoolBusiness();
            Expression<Func<SchoolEntity, bool>> expression = PredicateHelper.True<SchoolEntity>();
            List<SelectListItem> DropdownSchools = _schoolBusiness.GetSchoolsSelectList(userInfo, expression).ToSelectList();

            List<SelectListItem> DropdownTeachers = new List<SelectListItem>();
            DropdownTeachers = new UserBusiness().GetTeacherSelectList(userInfo).ToSelectList();
            
            if (DropdownSchools.Count > 0)
                DropdownSchools = DropdownSchools.OrderBy(r => r.Text).ToList();
            DropdownSchools.AddDefaultItem(ViewTextHelper.DefaultAllText, "-1");

            DropdownTeachers.AddDefaultItem(ViewTextHelper.DefaultAllText, "-1");

            List<object> dropdownData = new List<object> { DropdownTeachers, DropdownSchools, DropdownCommunities };
            return dropdownData;
        }
    }
}