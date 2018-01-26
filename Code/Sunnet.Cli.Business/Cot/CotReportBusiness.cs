/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/30 2015 11:13:41
 * Description:		Please input class summary
 * Version History:	Created,1/30 2015 11:13:41
 * 
 * 
 **************************************************************************/
using LinqKit;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Communities;
using Sunnet.Cli.Business.Cot.Cumulative;
using Sunnet.Cli.Business.Cot.Growth;
using Sunnet.Cli.Business.Cot.Report;
using Sunnet.Cli.Business.Cot.Summary;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Business.Users;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cot;
using Sunnet.Cli.Core.Cot.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MeasureModel = Sunnet.Cli.Business.Cot.Cumulative.MeasureModel;

namespace Sunnet.Cli.Business.Cot
{
    public class CotReportBusiness
    {
        private ICotContract _cotContract;
        private readonly IAdeDataContract _adeData;
        private AdeBusiness _adeBusiness;
        private UserBusiness _userBusiness;
        private SchoolBusiness _schoolBusiness;
        private CommunityBusiness _communityBusiness;
        private CotBusiness _cotBusiness;

        public CotReportBusiness(AdeUnitOfWorkContext unit = null)
        {
            _cotContract = DomainFacade.CreateCotContract(unit);
            _adeData = DomainFacade.CreateAdeDataService(unit);
            _userBusiness = new UserBusiness();
            _schoolBusiness = new SchoolBusiness();
            _adeBusiness = new AdeBusiness(unit);
            _communityBusiness = new CommunityBusiness();
            _cotBusiness = new CotBusiness();
        }

        private List<CotMeasureModel> CotMeasureModels(int assessmentId, IEnumerable<int> measureIds)
        {
            Expression<Func<MeasureEntity, bool>> condition =
                x => x.AssessmentId == assessmentId && x.Status == EntityStatus.Active && x.IsDeleted == false;
            if (measureIds != null)
                condition = condition.And(x => measureIds.Contains(x.ID));
            List<CotMeasureModel> measures =
                _adeData.Measures.AsExpandable().Where(condition)
                    .Select(x => new CotMeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        ParentId = x.ParentId,
                        Sort = x.Sort,
                        Items = x.Items.Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false)
                            .Select(i => i.ID),
                        ItemsOfSubMeasure =
                            x.SubMeasures.Select(
                                sub =>
                                    sub.Items.Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false)
                                        .Select(i => i.ID))
                    }).ToList();
            return measures;
        }

        private List<TeacherModel> GetTeachers(int teacherId, string schoolIds, int districtId, int type, UserBaseEntity user)
        {
            List<TeacherModel> teachers = null;
            var choosedTeacher = _userBusiness.GetTeacherModel(teacherId);
            if (type == (int)ObservedReportType.SingleTeacher)
            {
                teachers = new List<TeacherModel>();
                teachers.Add(choosedTeacher);
            }
            else if (type == (int)ObservedReportType.AssignedTeachers)
            {
                var isSchoolLevel = user.Role == Role.TRS_Specialist
                                    || user.Role == Role.TRS_Specialist_Delegate
                                    || user.Role == Role.School_Specialist
                                    || user.Role == Role.School_Specialist_Delegate
                                    || user.Role == Role.Principal
                                    || user.Role == Role.Principal_Delegate;

                var isCommunityLevel = user.Role == Role.District_Community_Specialist
                                       || user.Role == Role.Community_Specialist_Delegate
                                       || user.Role == Role.Community
                                       || user.Role == Role.District_Community_Delegate;
                var isCliLevel = user.Role == Role.Coordinator
                                     || user.Role == Role.Intervention_manager
                                     || user.Role == Role.Intervention_support_personnel
                                     || user.Role == Role.Content_personnel;
                // 如果当前用户是Mentor,则导出分配给Mentor的Teachers
                Expression<Func<TeacherEntity, bool>> condition = x => false;
                if (user.Role == Role.Mentor_coach)
                {
                    teachers = _userBusiness.GetTeacherModels(x => x.CoachId == user.ID);
                }
                else if (user.Role == Role.Super_admin)
                {
                    if (!string.IsNullOrEmpty(schoolIds))
                    {
                        List<int> schoolIdList = schoolIds.Split(',').Select(x => int.Parse(x)).ToList();
                        condition = x => x.UserInfo.UserCommunitySchools.Any
                            (u => schoolIdList.Contains(u.SchoolId) && u.SchoolId > 0);
                        teachers = _userBusiness.GetTeacherModels(condition);
                    }
                }
                else if (isSchoolLevel || isCommunityLevel || isCliLevel)
                {
                    // 否则按角色导出相应范围的Teachers
                    if (!string.IsNullOrEmpty(schoolIds))
                    {
                        List<int> schoolIdList = schoolIds.Split(',').Select(x => int.Parse(x)).ToList();
                        var schoolIdsByUser = _schoolBusiness.GetSchoolIds(user);
                        condition = x => x.UserInfo.UserCommunitySchools.Any
                            (u => schoolIdList.Contains(u.SchoolId) && schoolIdsByUser.Contains(u.SchoolId) && u.SchoolId > 0);
                        teachers = _userBusiness.GetTeacherModels(condition);
                    }
                }
                else
                {
                    // 其他用户不能导出任何Teachers
                }
            }
            else
            {
                throw new Exception("Please call another teachers query method.");
            }
            return teachers;
        }

        private List<TeacherModel> GetTeachers(int teacherId, int schoolId, int districtId, int type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels, List<AssignmentType> eCircles, UserBaseEntity user)
        {
            coachModels = coachModels ?? new List<AssignmentType>();
            eCircles = eCircles ?? new List<AssignmentType>();

            List<TeacherModel> teachers = null;
            Expression<Func<TeacherEntity, bool>> condition = x => false;
            var isSchoolLevel = user.Role == Role.TRS_Specialist
                                    || user.Role == Role.TRS_Specialist_Delegate
                                    || user.Role == Role.School_Specialist
                                    || user.Role == Role.School_Specialist_Delegate
                                    || user.Role == Role.Principal
                                    || user.Role == Role.Principal_Delegate;

            var isCommunityLevel = user.Role == Role.District_Community_Specialist
                                       || user.Role == Role.Community_Specialist_Delegate
                                       || user.Role == Role.Community
                                       || user.Role == Role.District_Community_Delegate;

            var isCliLevel = user.Role == Role.Coordinator
                                 || user.Role == Role.Intervention_manager
                                 || user.Role == Role.Intervention_support_personnel
                                 || user.Role == Role.Content_personnel;
            var isAdmin = user.Role == Role.Super_admin;
            if (type == (int)ObservedReportType.AssignedTeachersAverage)
            {
                // 如果当前用户是Mentor,则导出分配给Mentor的Teachers
                if (user.Role == Role.Mentor_coach)
                {
                    condition = x => x.CoachId == user.ID && x.UserInfo.UserCommunitySchools.Any(y => y.SchoolId == schoolId && y.SchoolId > 0);
                }
                else if (isSchoolLevel || isCommunityLevel || isCliLevel || isAdmin)
                {
                    // 否则按角色导出相应范围的Teachers
                    condition = x => x.UserInfo.UserCommunitySchools.Any(y => y.SchoolId == schoolId && y.SchoolId > 0);
                }
                else
                {
                    // 其他用户不能导出任何Teachers
                    condition = x => false;
                }

                if (yearsInProject != null && yearsInProject.Count > 0)
                {
                    var years = yearsInProject.Keys.ToList();
                    condition = condition.And(x => years.Contains(x.YearsInProjectId));
                }
                if (coachModels != null && coachModels.Count > 0)
                    condition = condition.And(x => coachModels.Contains(x.CoachAssignmentWay));
                if (eCircles != null && eCircles.Count > 0)
                    condition = condition.And(x => eCircles.Contains(x.ECIRCLEAssignmentWay));

                teachers = _userBusiness.GetTeacherModels(condition);
            }
            else if (type == (int)ObservedReportType.SchoolAverage)
            {
                if (user.Role == Role.Mentor_coach)
                {
                    condition = x => x.CoachId == user.ID && x.UserInfo.UserCommunitySchools.Any(y => y.SchoolId == schoolId && y.SchoolId > 0);
                }
                else if (isSchoolLevel || isCommunityLevel || isCliLevel || isAdmin)
                {
                    condition = x => x.UserInfo.UserCommunitySchools.Any(y => y.SchoolId == schoolId && y.SchoolId > 0);
                }
                else
                {
                    condition = x => false;
                }

                if (yearsInProject != null && yearsInProject.Count > 0)
                {
                    var years = yearsInProject.Keys.ToList();
                    condition = condition.And(x => years.Contains(x.YearsInProjectId));
                }
                if (coachModels != null && coachModels.Count > 0)
                    condition = condition.And(x => coachModels.Contains(x.CoachAssignmentWay));
                if (eCircles != null && eCircles.Count > 0)
                    condition = condition.And(x => eCircles.Contains(x.ECIRCLEAssignmentWay));

                teachers = _userBusiness.GetTeacherModels(condition);
            }
            else if (type == (int)ObservedReportType.AllSchoolAverage)
            {
                var choosedTeacher = _userBusiness.GetTeacherModel(teacherId);
                List<int> teacherCommunityIds = choosedTeacher.Communities.Select(c => c.ID).ToList();
                if (user.Role == Role.Mentor_coach)
                {
                    condition = x => x.CoachId == user.ID;
                }
                else if (isSchoolLevel)
                {
                    var schoolIds = _schoolBusiness.GetSchoolIds(user);
                    condition = x => x.UserInfo.UserCommunitySchools.Any(u => schoolIds.Contains(u.SchoolId) && u.SchoolId > 0);
                }
                else if (isCommunityLevel)
                {
                    List<int> communityIds = _communityBusiness.GetCommunityId(c => true, user);
                    List<int> Ids = communityIds.Intersect(teacherCommunityIds).ToList();
                    condition = x => x.UserInfo.UserCommunitySchools.Any(u => Ids.Contains(u.CommunityId));
                }
                else if (isCliLevel || isAdmin)
                {
                    condition = x => x.UserInfo.UserCommunitySchools.Any(u => teacherCommunityIds.Contains(u.CommunityId));
                }
                else
                {
                    condition = x => false;
                }

                if (yearsInProject != null && yearsInProject.Count > 0)
                {
                    var years = yearsInProject.Keys.ToList();
                    condition = condition.And(x => years.Contains(x.YearsInProjectId));
                }
                if (coachModels != null && coachModels.Count > 0)
                    condition = condition.And(x => coachModels.Contains(x.CoachAssignmentWay));
                if (eCircles != null && eCircles.Count > 0)
                    condition = condition.And(x => eCircles.Contains(x.ECIRCLEAssignmentWay));

                teachers = _userBusiness.GetTeacherModels(condition);
            }
            else if (type == (int)ObservedReportType.DistrictAverage)
            {
                if (user.Role == Role.Mentor_coach)
                {
                    condition = x => x.CoachId == user.ID;
                }
                else if (isSchoolLevel || isCommunityLevel || isCliLevel || isAdmin)
                {
                    condition = x => x.UserInfo.UserCommunitySchools.Any(u => u.CommunityId == districtId);
                }
                else
                {
                    condition = x => false;
                }

                if (yearsInProject != null && yearsInProject.Count > 0)
                {
                    var years = yearsInProject.Keys.ToList();
                    condition = condition.And(x => years.Contains(x.YearsInProjectId));
                }
                if (coachModels != null && coachModels.Count > 0)
                    condition = condition.And(x => coachModels.Contains(x.CoachAssignmentWay));
                if (eCircles != null && eCircles.Count > 0)
                    condition = condition.And(x => eCircles.Contains(x.ECIRCLEAssignmentWay));

                teachers = _userBusiness.GetTeacherModels(condition);
            }
            else
            {
                throw new Exception("Please call another teachers query method.");
            }
            return teachers;
        }

        public List<TeacherModel> GetTeacherList(int teacherId, string schoolIds, int districtId, int type, UserBaseEntity user)
        {
            return GetTeachers(teacherId, schoolIds, districtId, type, user);
        }

        #region Cot Report
        public List<CotReportModel> GetCotReports(int assessmentId, int teacherId, string schoolIds, int districtId, int year,
            IEnumerable<int> measureIds, ObservedReportType type, UserBaseEntity user)
        {
            return GetCotReports(assessmentId, teacherId, schoolIds, districtId, year,
                measureIds, type, null, null, null, user);
        }

        public List<CotReportModel> GetMutiTeacherReports(int assessmentId, int teacherId, string schoolIds, int districtId, int year,
            IEnumerable<int> measureIds, ObservedReportType type, UserBaseEntity user, IEnumerable<int> teacherIds)
        {
            var schoolYear = year.ToSchoolYearString();
            List<TeacherModel> teachers = null;
            Expression<Func<TeacherEntity, bool>> condition = x => true;
            condition = condition.And(x => teacherIds.Contains(x.ID));
            teachers = _userBusiness.GetTeacherModels(condition);

            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();
            if (completedTeachers == null || !completedTeachers.Any())
                return null;
            var measures = CotMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => x.BoyObsDate > CommonAgent.MinDate || x.MoyObsDate > CommonAgent.MinDate)
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的
            var reports = new List<CotReportModel>();
            GetTeacherReports(assessmentId, year, type, teachers, completedTeachers, items, measures, reports);
            return reports;
        }

        public List<CotReportModel> GetCotReports(int assessmentId, int teacherId, string schoolIds, int districtId, int year,
            IEnumerable<int> measureIds, ObservedReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels, List<AssignmentType> eCircles, UserBaseEntity user)
        {
            var schoolYear = year.ToSchoolYearString();

            List<TeacherModel> teachers = null;
            if (yearsInProject == null)
                teachers = GetTeachers(teacherId, schoolIds, districtId, (int)type, user);
            else
            {
                string[] schoolIdArr = schoolIds.Split(',');
                int schoolId = 0;
                if (schoolIdArr.Count() > 0)
                    schoolId = int.Parse(schoolIdArr[0]);
                teachers = GetTeachers(teacherId, schoolId, districtId, (int)type, yearsInProject, coachModels, eCircles, user);
            }
            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();
            if (completedTeachers == null || !completedTeachers.Any())
                return null;
            var measures = CotMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => x.BoyObsDate > CommonAgent.MinDate || x.MoyObsDate > CommonAgent.MinDate)
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的
            var reports = new List<CotReportModel>();
            if (type == ObservedReportType.SingleTeacher ||
                type == ObservedReportType.AssignedTeachers)
            {
                GetTeacherReports(assessmentId, year, type, teachers, completedTeachers, items, measures, reports);
            }
            else if (type == ObservedReportType.AllSchoolAverage)
            {
                //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
                string mentor = "";
                List<int> teacherIds = teachers.Where(x => completedTeachers.Contains(x.Id)).Select(x => x.Id).ToList();
                List<CotStgReportEntity> cotStgReports = _cotBusiness.GetLastReports(assessmentId, year, teacherIds);
                int cotStgReportsCount = cotStgReports.Count;
                if (cotStgReportsCount == 0)
                {
                    List<CotWaveEntity> cotWaves = _cotBusiness.GetLastCotWaves(assessmentId, year, teacherIds);
                    int cotWaveCount = cotWaves.Count;
                    if (cotWaveCount == 0)
                        mentor = "N/A";
                    else if (cotWaveCount == 1)
                        mentor = _userBusiness.GetUserBaseModel(cotWaves.First().CreatedBy).FullName;
                    else
                        mentor = "Various";
                }
                else if (cotStgReportsCount == 1)
                    mentor = _userBusiness.GetUserBaseModel(cotStgReports.First().CreatedBy).FullName;
                else
                    mentor = "Various";

                TeacherModel choseTeacher = _userBusiness.GetTeacherModel(teacherId);
                List<int> communityIds = choseTeacher.Communities.Select(c => c.ID).Distinct().ToList();
                GetSchoolsAverageReports(districtId, year, type, user, items, teachers, mentor, completedTeachers, measures, reports, communityIds);
            }
            else if (type == ObservedReportType.AssignedTeachersAverage
                || type == ObservedReportType.SchoolAverage
                || type == ObservedReportType.DistrictAverage)
            {
                //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
                string mentor = "";
                List<int> teacherIds = teachers.Where(x => completedTeachers.Contains(x.Id)).Select(x => x.Id).ToList();
                List<CotStgReportEntity> cotStgReports = _cotBusiness.GetLastReports(assessmentId, year, teacherIds);
                int cotStgReportsCount = cotStgReports.Count;
                if (cotStgReportsCount == 0)
                {
                    List<CotWaveEntity> cotWaves = _cotBusiness.GetLastCotWaves(assessmentId, year, teacherIds);
                    int cotWaveCount = cotWaves.Count;
                    if (cotWaveCount == 0)
                        mentor = "N/A";
                    else if (cotWaveCount == 1)
                        mentor = _userBusiness.GetUserBaseModel(cotWaves.First().CreatedBy).FullName;
                    else
                        mentor = "Various";
                }
                else if (cotStgReportsCount == 1)
                    mentor = _userBusiness.GetUserBaseModel(cotStgReports.First().CreatedBy).FullName;
                else
                    mentor = "Various";

                GetAverageReport(year, type, teachers, mentor, items, completedTeachers, measures, reports);
            }
            reports.ForEach(report =>
            {
                report.YearsInProject = yearsInProject != null ? yearsInProject.Values.ToList() : new List<string>();
                report.CoachModels = coachModels;
                report.ECircles = eCircles;
            });
            return reports;
        }

        private void GetSchoolsAverageReports(int districtId, int year, ObservedReportType type, UserBaseEntity user, List<CotItemModel> items,
            List<TeacherModel> teachers, string mentor, List<int> completedTeachers, List<CotMeasureModel> measures, List<CotReportModel> reports, List<int> communityIds)
        {
            SchoolEntity schoolEntity = null;
            var teacherModel = teachers.First();
            var schools = teachers.Select(x => x.Schools).Aggregate(new List<BasicSchoolModel>(), (first, next) =>
            {
                first.AddRange(next);
                return first;
            }).DistinctBy(x => x.ID).ToList();
            if (schools.Any())
            {
                schools.ForEach(school =>
                {
                    //只留当前选中Teacher所在Community，排除其他Community
                    schoolEntity = _schoolBusiness.GetSchool(school.ID);
                    if (!schoolEntity.CommunitySchoolRelations.Any(csr => communityIds.Contains(csr.CommunityId)))
                        return;
                    items.ForEach(item =>
                    {
                        var teacher = teachers.Find(t => t.Id == item.TeacherId && t.Schools.Any(s => s.ID == school.ID));
                        item.SchoolId = teacher != null ? school.ID : 0;
                    });

                    var teachersOfSchool =
                        teachers.Where(t => t.Schools.Any(s => s.ID == school.ID) && completedTeachers.Contains(t.Id))
                            .ToList();
                    var itemsOfSchool = items.Where(x => x.SchoolId == school.ID).ToList();
                    var teacherCount = teachersOfSchool.Count;
                    if (!teachersOfSchool.Any()) return;

                    var report = new CotReportModel()
                    {
                        Community = school.CommunitiesText,
                        Mentor = mentor,
                        School = school.Name,
                        SchoolYear = year,
                        Teacher = teacherCount.ToString(),
                        Type = type
                    };

                    report.BoyDate = itemsOfSchool.Any(i => i.BoyDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.BoyDate > CommonAgent.MinDate).Min(x => x.BoyDate)
                        : CommonAgent.MinDate;
                    report.MoyDate = itemsOfSchool.Any(i => i.MoyDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.MoyDate > CommonAgent.MinDate).Max(x => x.MoyDate)
                        : CommonAgent.MinDate;
                    var measuresCountByTeacher = from item in itemsOfSchool
                                                 group item by new { item.MeasureId, item.TeacherId }
                                                     into g1
                                                 select new CotReportMeasureModel()
                                                 {
                                                     Id = g1.Key.MeasureId,
                                                     CountOfBoy = g1.Count(i => i.BoyDate > CommonAgent.MinDate),
                                                     CountOfMoy = g1.Count(i => i.MoyDate > CommonAgent.MinDate)
                                                 };
                    var measuresCount = from measure in measuresCountByTeacher
                                        group measure by measure.Id
                                            into g1
                                        select new CotReportMeasureModel()
                                        {
                                            Id = g1.Key,
                                            CountOfBoy = g1.Sum(x => x.CountOfBoy) / teacherCount,
                                            CountOfMoy = g1.Sum(x => x.CountOfMoy) / teacherCount
                                        };
                    var results = measuresCount.ToList();
                    var ms = measures.Select(x => new CotReportMeasureModel()
                    {
                        Id = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        Total = x.ItemCount
                    }).ToList();
                    ms.ForEach(mea =>
                    {
                        var average = results.Find(x => x.Id == mea.Id);
                        if (average != null)
                        {
                            mea.CountOfBoy = average.CountOfBoy;
                            mea.CountOfMoy = average.CountOfMoy;
                        }
                    });
                    report.Measures = ms;

                    reports.Add(report);
                });
            }
        }

        /// <summary>
        /// Get average report: Single School | Community .
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="type">The type.</param>
        /// <param name="teachers">The teachers.</param>
        /// <param name="items">The items.</param>
        /// <param name="completedTeachers">The completed teachers.</param>
        /// <param name="measures">The measures.</param>
        /// <param name="reports">The reports.</param>
        private void GetAverageReport(int year, ObservedReportType type, List<TeacherModel> teachers, string mentor, List<CotItemModel> items, List<int> completedTeachers,
            List<CotMeasureModel> measures, List<CotReportModel> reports)
        {
            teachers = teachers.Where(x => completedTeachers.Contains(x.Id)).ToList();
            var teacher = teachers.First();
            var teacherCount = teachers.Count;
            var schools = new List<BasicSchoolModel>();
            teachers.ForEach(t => schools.AddRange(t.Schools));
            schools = schools.DistinctBy(s => s.ID).ToList();
            var schoolCountStr = type == ObservedReportType.SchoolAverage
                ? teacher.SchoolNameText
                : schools.Count.ToString();
            var report = new CotReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = schoolCountStr,
                SchoolYear = year,
                Teacher = teacherCount.ToString(),
                Type = type
            };
            report.BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate) ?
                items.Where(i => i.BoyDate > CommonAgent.MinDate).Min(x => x.BoyDate)
                : CommonAgent.MinDate;
            report.MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate) ?
                items.Where(i => i.MoyDate > CommonAgent.MinDate).Max(x => x.MoyDate)
                : CommonAgent.MinDate;

            var measuresCountByTeacher = from item in items
                                         group item by new { item.MeasureId, item.TeacherId }
                                             into g1
                                         select new CotReportMeasureModel()
                                         {
                                             Id = g1.Key.MeasureId,
                                             CountOfBoy = g1.Count(i => i.BoyDate > CommonAgent.MinDate),
                                             CountOfMoy = g1.Count(i => i.MoyDate > CommonAgent.MinDate)
                                         };
            var measuresCount = from measure in measuresCountByTeacher
                                group measure by measure.Id
                                    into g1
                                select new CotReportMeasureModel()
                                {
                                    Id = g1.Key,
                                    CountOfBoy = g1.Sum(x => x.CountOfBoy) / teacherCount,
                                    CountOfMoy = g1.Sum(x => x.CountOfMoy) / teacherCount
                                };
            var results = measuresCount.ToList();
            var ms = measures.Select(x => new CotReportMeasureModel()
            {
                Id = x.ID,
                Name = x.Name,
                ShortName = x.ShortName,
                Total = x.ItemCount
            }).ToList();
            ms.ForEach(mea =>
            {
                var average = results.Find(x => x.Id == mea.Id);
                if (average != null)
                {
                    mea.CountOfBoy = average.CountOfBoy;
                    mea.CountOfMoy = average.CountOfMoy;
                }
            });
            report.Measures = ms;
            reports.Add(report);
        }

        private void GetTeacherReports(int assessmentId, int year, ObservedReportType type, List<TeacherModel> teachers, List<int> completedTeachers, List<CotItemModel> items,
            List<CotMeasureModel> measures, List<CotReportModel> reports)
        {
            string observerName = "N/A";
            teachers.ForEach(teacher =>
            {
                if (completedTeachers.Contains(teacher.Id))
                {
                    //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
                    CotStgReportEntity cotStgReport = _cotBusiness.GetLastReport(assessmentId, year, teacher.Id);
                    if (cotStgReport != null)
                    {
                        var observer = _userBusiness.GetUserBaseModel(cotStgReport.CreatedBy);
                        observerName = observer.FullName;
                    }
                    else
                    {
                        CotWaveEntity cotWave = _cotBusiness.GetLastCotWave(assessmentId, year, teacher.Id);
                        if (cotWave != null)
                        {
                            var observer = _userBusiness.GetUserBaseModel(cotWave.CreatedBy);
                            observerName = observer.FullName;
                        }
                        else
                            observerName = "N/A";
                    }
                    var report = new CotReportModel()
                    {
                        Community = teacher.CommunityNameText,
                        Mentor = observerName,
                        School = teacher.SchoolNameText,
                        SchoolYear = year,
                        Teacher = teacher.FullName,
                        Type = type
                    };
                    report.BoyDate =
                        items.Any(i => i.TeacherId == teacher.Id && i.BoyDate > CommonAgent.MinDate)
                            ? items.Where(i => i.TeacherId == teacher.Id && i.BoyDate > CommonAgent.MinDate)
                                .Min(x => x.BoyDate)
                            : CommonAgent.MinDate;
                    report.MoyDate =
                        items.Any(i => i.TeacherId == teacher.Id && i.MoyDate > CommonAgent.MinDate)
                            ? items.Where(i => i.TeacherId == teacher.Id && i.MoyDate > CommonAgent.MinDate)
                                .Max(x => x.MoyDate)
                            : CommonAgent.MinDate;
                    report.Measures = measures.Select(x => new CotReportMeasureModel()
                    {
                        Id = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        CountOfBoy = items.Where(i => x.Items.Contains(i.Id)).
                            Count(i => i.TeacherId == teacher.Id && i.BoyDate > CommonAgent.MinDate),
                        CountOfMoy = items.Where(i => x.Items.Contains(i.Id)).
                            Count(i => i.TeacherId == teacher.Id && i.MoyDate > CommonAgent.MinDate),
                        Total = x.ItemCount
                    }).ToList();
                    reports.Add(report);
                }
            });
        }

        private IQueryable<int> Assessments(int assessmentId, int teacherId, List<int> teacherIds, ObservedReportType type, string schoolYear)
        {
            Expression<Func<CotAssessmentEntity, bool>> condition = x => x.AssessmentId == assessmentId;

            switch (type)
            {
                case ObservedReportType.SingleTeacher:
                    condition = condition.And(x => x.TeacherId == teacherId);
                    break;
                case ObservedReportType.AssignedTeachers:
                case ObservedReportType.AssignedTeachersAverage:
                case ObservedReportType.SchoolAverage:
                case ObservedReportType.AllSchoolAverage:
                case ObservedReportType.DistrictAverage:
                    condition = condition.And(x => teacherIds.Contains(x.TeacherId));
                    break;
                default:
                    break;
            }
            condition = condition.And(x => x.SchoolYear == schoolYear);
            var assessments = _cotContract.Assessments.AsExpandable().Where(condition).Select(x => x.ID);
            return assessments;
        }
        #endregion

        #region Cumulative

        private List<MeasureModel> CotCumulativeMeasureModels(int assessmentId, IEnumerable<int> measureIds)
        {
            Expression<Func<MeasureEntity, bool>> condition =
                x => x.AssessmentId == assessmentId
                    && x.Status == EntityStatus.Active
                    && x.IsDeleted == false
                    && measureIds.Contains(x.ID);
            List<MeasureModel> measures =
                _adeData.Measures.AsExpandable().Where(condition)
                    .Select(x => new MeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        ParentId = x.ParentId,
                        Sort = x.Sort,
                        Items = x.Items.Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false).OfType<CotItemEntity>()
                            .Select(i => new AdeCotItemModel()
                            {
                                CotItemNo = i.CotItemId,
                                Count = 0,
                                Id = i.ID,
                                Level = i.Level,
                                Description = i.Description
                            }),
                        ItemsOfSubMeasure = x.SubMeasures.Select(sub => sub.Items.OfType<CotItemEntity>()
                            .Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false)
                                        .Select(i => new AdeCotItemModel()
                                        {
                                            CotItemNo = i.CotItemId,
                                            Count = 0,
                                            Id = i.ID,
                                            Level = i.Level,
                                            Description = i.Description
                                        }))
                    }).ToList();
            return measures;
        }
        private IQueryable<int> Assessments(int assessmentId, int teacherId, List<int> teacherIds,
            CumulativeReportType type, string schoolYear)
        {
            Expression<Func<CotAssessmentEntity, bool>> condition = x => x.AssessmentId == assessmentId;

            switch (type)
            {
                case CumulativeReportType.SingleTeacher:
                    condition = condition.And(x => x.TeacherId == teacherId);
                    break;
                case CumulativeReportType.AssignedTeachers:
                case CumulativeReportType.AssignedTeachersCumulative:
                case CumulativeReportType.SchoolCumulative:
                case CumulativeReportType.DistrictCumulative:
                    condition = condition.And(x => teacherIds.Contains(x.TeacherId));
                    break;
                default:
                    break;
            }
            condition = condition.And(x => x.SchoolYear == schoolYear);
            var assessments = _cotContract.Assessments.AsExpandable().Where(condition).Select(x => x.ID);
            return assessments;
        }

        /// <summary>
        /// 单个Teacher的Cumulative报表
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="teacherId">The teacher identifier.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="districtId">The district identifier.</param>
        /// <param name="year">The year.</param>
        /// <param name="measureIds">The measure ids.</param>
        /// <param name="type">The type.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public List<ReportModel> GetCumulativeReports(int assessmentId, int teacherId, string schoolIds, int districtId, int year,
            IEnumerable<int> measureIds, CumulativeReportType type, UserBaseEntity user, IEnumerable<int> teacherIds)
        {
            var schoolYear = year.ToSchoolYearString();
            List<TeacherModel> teachers = null;
            if (teacherIds.Any())
            {
                Expression<Func<TeacherEntity, bool>> condition = x => true;
                condition = condition.And(x => teacherIds.Contains(x.ID));
                teachers = _userBusiness.GetTeacherModels(condition);
            }
            else
            {
                teachers = GetTeachers(teacherId, schoolIds, districtId, (int)type, user);
            }
            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();

            var measures = CotCumulativeMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => (x.BoyObsDate > CommonAgent.MinDate
                        || x.MoyObsDate > CommonAgent.MinDate
                        || x.CotUpdatedOn > CommonAgent.MinDate
                        || x.GoalMetDate > CommonAgent.MinDate
                        || x.GoalSetDate > CommonAgent.MinDate
                        )
                        && (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)
                        ))
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        CotUpdatedOn = x.CotUpdatedOn,
                        MetDate = x.GoalMetDate,
                        SetDate = x.GoalSetDate,
                        NeedSupport = x.NeedSupport,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的

            var itemIds = _cotContract.Items
                    .Where(x => (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)))
                    .Select(x => x.ItemId).Distinct();
            var allLinks = _adeBusiness.GetLinkModels<ItemBaseEntity>(itemIds.ToArray());
            if (allLinks == null)
            {
                allLinks = new List<Ade.Models.AdeLinkModel>();
            }

            var reports = new List<ReportModel>();
            string observerName = "N/A";
            if (type == CumulativeReportType.SingleTeacher || type == CumulativeReportType.AssignedTeachers)
            {
                teachers.ForEach(teacher =>
                {
                    if (completedTeachers.Contains(teacher.Id))
                    {
                        //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
                        CotStgReportEntity cotStgReport = _cotBusiness.GetLastReport(assessmentId, year, teacher.Id);
                        if (cotStgReport != null)
                        {
                            var observer = _userBusiness.GetUserBaseModel(cotStgReport.CreatedBy);
                            observerName = observer.FullName;
                        }
                        else
                        {
                            CotWaveEntity cotWave = _cotBusiness.GetLastCotWave(assessmentId, year, teacher.Id);
                            if (cotWave != null)
                            {
                                var observer = _userBusiness.GetUserBaseModel(cotWave.CreatedBy);
                                observerName = observer.FullName;
                            }
                            else
                                observerName = "N/A";
                        }
                        var report = new ReportModel()
                        {
                            Community = teacher.CommunityNameText,
                            Mentor = observerName,
                            School = teacher.SchoolNameText,
                            SchoolYear = year,
                            Teacher = teacher.FullName,
                            Type = type
                        };
                        var itemsOfTeacher = items.Where(x => x.TeacherId == teacher.Id).ToList();
                        report.BoyDate = itemsOfTeacher.Any(i => i.BoyDate > CommonAgent.MinDate)
                               ? itemsOfTeacher.Where(i => i.BoyDate > CommonAgent.MinDate)
                                   .Min(x => x.BoyDate)
                               : CommonAgent.MinDate;
                        report.MoyDate = itemsOfTeacher.Any(i => i.MoyDate > CommonAgent.MinDate)
                                ? itemsOfTeacher.Where(i => i.MoyDate > CommonAgent.MinDate)
                                    .Max(x => x.MoyDate)
                                : CommonAgent.MinDate;
                        report.MetDate = itemsOfTeacher.Any(i => i.MetDate > CommonAgent.MinDate)
                                ? itemsOfTeacher.Where(i => i.MetDate > CommonAgent.MinDate)
                                    .Max(x => x.MetDate)
                                : CommonAgent.MinDate;
                        report.Measures = measures.Where(x => x.Items.Any()).Select(x => new MeasureModel()
                        {
                            ID = x.ID,
                            Name = x.Name,
                            ParentId = x.ParentId,
                            ShortName = x.ShortName,
                            Sort = x.Sort,
                            Items = x.Items.Select(i => new AdeCotItemModel()
                            {
                                CotItemNo = i.CotItemNo,
                                Count = itemsOfTeacher.Count(it => it.Id == i.Id),
                                Id = i.Id,
                                Level = i.Level,
                                Description = i.Description,
                                SetDate = itemsOfTeacher.Where(s => s.Id == i.Id).Any() ? itemsOfTeacher.Where(s => s.Id == i.Id).First().SetDate : CommonAgent.MinDate,
                                IsFillColor = itemsOfTeacher.Where(s => s.Id == i.Id).Any() ? itemsOfTeacher.Where(s => s.Id == i.Id).First().IsFillColor : false,
                                NeedSupport = itemsOfTeacher.Where(s => s.Id == i.Id).Any() ? itemsOfTeacher.Where(s => s.Id == i.Id).First().NeedSupport : false,
                                Links = allLinks.Where(l => l.HostId == i.Id).ToList()
                            }).OrderBy(i => i.Level)
                        });

                        reports.Add(report);
                    }
                });
            }
            return reports;
        }

        /// <summary>
        /// Teachers的组合Cumulative报表
        /// </summary>
        /// <param name="assessmentId">The assessment identifier.</param>
        /// <param name="teacherId">The teacher identifier.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="districtId">The district identifier.</param>
        /// <param name="year">The year.</param>
        /// <param name="measureIds">The measure ids.</param>
        /// <param name="type">The type.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public List<CumulativeReportModel> GetCumulative2Reports(int assessmentId, int teacherId, int schoolId, int districtId, int year,
           IEnumerable<int> measureIds, CumulativeReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels, List<AssignmentType> eCircles, UserBaseEntity user)
        {
            var schoolYear = year.ToSchoolYearString();
            List<TeacherModel> teachers = GetTeachers(teacherId, schoolId, districtId, (int)type, yearsInProject, coachModels, eCircles, user);

            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();
            if (!completedTeachers.Any() || !completedTeachers.Any(x => teachers.Any(t => t.Id == x)))
                return null;
            teachers = teachers.Where(x => completedTeachers.Contains(x.Id)).ToList();

            var measures = CotCumulativeMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => (x.BoyObsDate > CommonAgent.MinDate
                        || x.MoyObsDate > CommonAgent.MinDate
                        || x.CotUpdatedOn > CommonAgent.MinDate
                        || x.GoalMetDate > CommonAgent.MinDate
                        )
                        && (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId))
                        )
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        MetDate = x.GoalMetDate,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的

            var reports = new List<CumulativeReportModel>();
            //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
            string mentor = "";
            List<int> teacherIds = teachers.Where(x => completedTeachers.Contains(x.Id)).Select(x => x.Id).ToList();
            List<CotStgReportEntity> cotStgReports = _cotBusiness.GetLastReports(assessmentId, year, teacherIds);
            int cotStgReportsCount = cotStgReports.Count;
            if (cotStgReportsCount == 0)
            {
                List<CotWaveEntity> cotWaves = _cotBusiness.GetLastCotWaves(assessmentId, year, teacherIds);
                int cotWaveCount = cotWaves.Count;
                if (cotWaveCount == 0)
                    mentor = "N/A";
                else if (cotWaveCount == 1)
                    mentor = _userBusiness.GetUserBaseModel(cotWaves.First().CreatedBy).FullName;
                else
                    mentor = "Various";
            }
            else if (cotStgReportsCount == 1)
                mentor = _userBusiness.GetUserBaseModel(cotStgReports.First().CreatedBy).FullName;
            else
                mentor = "Various";

            if (type == CumulativeReportType.AssignedTeachersCumulative)
            {
                GetAssignedTeachersCumulativeReports(year, type, yearsInProject, coachModels, eCircles, teachers, mentor, completedTeachers, items, measures, reports);
            }
            else if (type == CumulativeReportType.SchoolCumulative || type == CumulativeReportType.AllAssignedSchoolCumulative)
            {
                TeacherModel choseTeacher = _userBusiness.GetTeacherModel(teacherId);
                List<int> communityIds = choseTeacher.Communities.Select(c => c.ID).Distinct().ToList();
                if (!GetSchoolCumulativeReports(year, type, yearsInProject, coachModels, eCircles, teachers, mentor, completedTeachers, items, measures, reports, communityIds, schoolId))
                    return null;
            }
            else if (type == CumulativeReportType.DistrictCumulative)
            {
                var teacher = teachers.First();
                var schools = teachers.Select(x => x.Schools).Aggregate(new List<BasicSchoolModel>(), (first, next) =>
                {
                    first.AddRange(next);
                    return first;
                }).DistinctBy(x => x.ID).ToList();
                var school = schools.Count().ToString();

                var report = new CumulativeReportModel()
                {
                    Community = teacher.CommunityNameText,
                    Mentor = mentor,
                    School = school,
                    SchoolYear = year,
                    Teacher = teacher.FullName,
                    Type = type,
                    YearsInProject = yearsInProject.Values.ToList(),
                    CoachModels = coachModels,
                    ECircles = eCircles,
                    Teachers = teachers.Count(),
                    BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                        ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                            .Min(x => x.BoyDate)
                        : CommonAgent.MinDate,
                    MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                        ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                            .Max(x => x.MoyDate)
                        : CommonAgent.MinDate,
                    MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                        ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                            .Max(x => x.MetDate)
                        : CommonAgent.MinDate,
                    Measures = measures.Where(x => x.Items.Any()).Select(x => new MeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        ShortName = x.ShortName,
                        Sort = x.Sort,
                        Items = x.Items.Select(i => new AdeCotItemModel()
                        {
                            CotItemNo = i.CotItemNo,
                            Count = items.Count(it => it.Id == i.Id),
                            Id = i.Id,
                            Level = i.Level,
                            Description = i.Description
                        }).OrderBy(i => i.Level)
                    })
                };
                reports.Add(report);
            }
            return reports;
        }

        private bool GetSchoolCumulativeReports(int year, CumulativeReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, List<TeacherModel> teachers, string mentor, List<int> completedTeachers, List<CotItemModel> items,
            List<MeasureModel> measures, List<CumulativeReportModel> reports, List<int> communityIds, int schoolId = 0)
        {
            SchoolEntity schoolEnity = null;
            var schools = new List<BasicSchoolModel>();
            if (type == CumulativeReportType.AllAssignedSchoolCumulative)
                teachers.ForEach(tea => schools.AddRange(tea.Schools));
            else
                teachers.ForEach(tea => schools.AddRange(tea.Schools.Where(s => s.ID == schoolId)));
            schools = schools.DistinctBy(x => x.ID).ToList();
            if (!schools.Any())
                return false;
            schools.ForEach(school =>
            {
                //只留当前选中Teacher所在Community，排除其他Community
                schoolEnity = _schoolBusiness.GetSchool(school.ID);
                if (!schoolEnity.CommunitySchoolRelations.Any(csr => communityIds.Contains(csr.CommunityId)))
                    return;

                var totalTeachers = teachers.Where(t => t.Schools.Any(s => s.ID == school.ID) && completedTeachers.Contains(t.Id)).ToList();
                var teacher = totalTeachers.First();
                var schoolItems = items.Where(x => totalTeachers.Any(tea => tea.Id == x.TeacherId)).ToList();
                var report = new CumulativeReportModel
                {
                    Community = school.CommunitiesText,
                    Mentor = mentor,
                    School = school.Name,
                    SchoolYear = year,
                    Teacher = teacher.FullName,
                    Type = type,
                    YearsInProject = yearsInProject.Values.ToList(),
                    CoachModels = coachModels,
                    ECircles = eCircles,
                    Teachers = totalTeachers.Count(),
                    BoyDate = schoolItems.Any(i => i.BoyDate > CommonAgent.MinDate)
                        ? schoolItems.Where(i => i.BoyDate > CommonAgent.MinDate)
                            .Min(x => x.BoyDate)
                        : CommonAgent.MinDate,
                    MoyDate = schoolItems.Any(i => i.MoyDate > CommonAgent.MinDate)
                        ? schoolItems.Where(i => i.MoyDate > CommonAgent.MinDate)
                            .Max(x => x.MoyDate)
                        : CommonAgent.MinDate,
                    MetDate = schoolItems.Any(i => i.MetDate > CommonAgent.MinDate)
                        ? schoolItems.Where(i => i.MetDate > CommonAgent.MinDate)
                            .Max(x => x.MetDate)
                        : CommonAgent.MinDate,
                    Measures = measures.Where(x => x.Items.Any()).Select(x => new MeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        ShortName = x.ShortName,
                        Sort = x.Sort,
                        Items = x.Items.Select(i => new AdeCotItemModel()
                        {
                            CotItemNo = i.CotItemNo,
                            Count = schoolItems.Count(it => it.Id == i.Id),
                            Id = i.Id,
                            Level = i.Level,
                            Description = i.Description
                        }).OrderBy(i => i.Level)
                    })
                };

                reports.Add(report);
            });
            return true;
        }

        private void GetAssignedTeachersCumulativeReports(int year, CumulativeReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, List<TeacherModel> teachers, string mentor, List<int> completedTeachers, List<CotItemModel> items, List<MeasureModel> measures, List<CumulativeReportModel> reports)
        {
            var totalTeachers = teachers.Where(x => completedTeachers.Contains(x.Id)).ToList();
            var teacher = totalTeachers.First();
            var schools = totalTeachers.Select(x => x.Schools).Aggregate(new List<BasicSchoolModel>(), (first, next) =>
            {
                first.AddRange(next);
                return first;
            }).DistinctBy(x => x.ID).ToList();

            var report = new CumulativeReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = schools.Count.ToString(),
                SchoolYear = year,
                Teacher = teacher.FullName,
                Type = type,
                YearsInProject = yearsInProject.Values.ToList(),
                CoachModels = coachModels,
                ECircles = eCircles,
                Teachers = totalTeachers.Count()
            };
            report.BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                    .Min(x => x.BoyDate)
                : CommonAgent.MinDate;
            report.MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                    .Max(x => x.MoyDate)
                : CommonAgent.MinDate;
            report.MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                    .Max(x => x.MetDate)
                : CommonAgent.MinDate;
            report.Measures = measures.Where(x => x.Items.Any()).Select(x => new MeasureModel()
            {
                ID = x.ID,
                Name = x.Name,
                ParentId = x.ParentId,
                ShortName = x.ShortName,
                Sort = x.Sort,
                Items = x.Items.Select(i => new AdeCotItemModel()
                {
                    CotItemNo = i.CotItemNo,
                    Count = items.Count(it => it.Id == i.Id),
                    Id = i.Id,
                    Level = i.Level,
                    Description = i.Description
                }).OrderBy(i => i.Level)
            });

            reports.Add(report);
        }

        #endregion

        #region Growth

        public IEnumerable<Growth.MeasureModel> CotGrowthMeasureModels(int assessmentId,
            IEnumerable<int> measureIds)
        {
            Expression<Func<MeasureEntity, bool>> condition =
                x => x.AssessmentId == assessmentId
                    && x.Status == EntityStatus.Active
                    && x.IsDeleted == false
                    && measureIds.Contains(x.ID);
            List<Growth.MeasureModel> measures =
                _adeData.Measures.AsExpandable().Where(condition).OrderBy(x => x.Sort)
                    .Select(x => new Growth.MeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        ItemCount = x.Items.Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false).OfType<CotItemEntity>().Count(),
                        SubItemCount = x.SubMeasures.Any() ?
                        x.SubMeasures.Sum(sub => sub.Items.OfType<CotItemEntity>()
                            .Count(i => i.Status == EntityStatus.Active && i.IsDeleted == false)) : 0,
                        Items = x.Items.Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false).OfType<CotItemEntity>()
                            .Select(i => new GrowthItemModel()
                            {
                                CotItemNo = i.CotItemId,
                                Count = 0,
                                Id = i.ID,
                                Level = i.Level,
                                Description = i.Description
                            }),
                        ItemsOfSubMeasure = x.SubMeasures.Select(sub => sub.Items.OfType<CotItemEntity>()
                            .Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false)
                                        .Select(i => new GrowthItemModel()
                                        {
                                            CotItemNo = i.CotItemId,
                                            Count = 0,
                                            Id = i.ID,
                                            Level = i.Level,
                                            Description = i.Description
                                        }))
                    }).ToList();
            return measures;
        }

        private IQueryable<int> Assessments(int assessmentId, int teacherId, List<int> teacherIds,
            GrowthReportType type, string schoolYear)
        {
            Expression<Func<CotAssessmentEntity, bool>> condition = x => x.AssessmentId == assessmentId;

            switch (type)
            {
                case GrowthReportType.SingleTeacher:
                    condition = condition.And(x => x.TeacherId == teacherId);
                    break;
                case GrowthReportType.AssignedTeachers:
                case GrowthReportType.AssignedTeachersAverage:
                case GrowthReportType.SchoolAverage:
                case GrowthReportType.AllAssignedSchoolAverage:
                case GrowthReportType.DistrictAverage:
                    condition = condition.And(x => teacherIds.Contains(x.TeacherId));
                    break;
                default:
                    break;
            }
            condition = condition.And(x => x.SchoolYear == schoolYear);
            var assessments = _cotContract.Assessments.AsExpandable().Where(condition).Select(x => x.ID);
            return assessments;
        }

        private static List<DateTime> GetGrowthMonthes(int year)
        {
            var list = new List<DateTime>();
            DateTime start, end;
            start = CommonAgent.GetStartDateOfYear(year);
            //end = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);  //David 12/20/2016
            end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);  //David 12/20/2016   redo Ticket 2176 include current Month
            for (DateTime i = start; i <= end; i = i.AddMonths(1))
            {
                list.Add(new DateTime(i.Year, i.Month, DateTime.DaysInMonth(i.Year, i.Month), 23, 59, 59, 999));
            }
            return list;

        }

        public List<GrowthReportModel> GetGrowthReports(int assessmentId, int teacherId, string schoolId, int districtId, int year,
            IEnumerable<int> measureIds, GrowthReportType type, UserBaseEntity user, IEnumerable<int> teacherIds)
        {
            var schoolYear = year.ToSchoolYearString();
            DateTime startDate = CommonAgent.GetStartDateOfYear(year);
            List<DateTime> growthMontheList = GetGrowthMonthes(year);
            List<TeacherModel> teachers = null;
            if (teacherIds.Any())
            {
                Expression<Func<TeacherEntity, bool>> condition = x => true;
                condition = condition.And(x => teacherIds.Contains(x.ID));
                teachers = _userBusiness.GetTeacherModels(condition);
            }
            else
            {
                teachers = GetTeachers(teacherId, schoolId, districtId, (int)type, user);
            }
            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();

            var measures = CotGrowthMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => (x.BoyObsDate > CommonAgent.MinDate
                        || x.MoyObsDate > CommonAgent.MinDate
                        || x.CotUpdatedOn > CommonAgent.MinDate
                        || x.GoalMetDate > CommonAgent.MinDate
                        )
                        && (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)))
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        MetDate = x.GoalMetDate,
                        CotUpdatedOn = x.CotUpdatedOn,
                        SetDate = x.GoalSetDate,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的

            var itemIds = _cotContract.Items
                    .Where(x => (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)))
                    .Select(x => x.ItemId).Distinct();
            var allLinks = _adeBusiness.GetLinkModels<ItemBaseEntity>(itemIds.ToArray());
            if (allLinks == null)
            {
                allLinks = new List<Ade.Models.AdeLinkModel>();
            }
            var reports = new List<GrowthReportModel>();
            string observerName = "N/A";
            if (type == GrowthReportType.SingleTeacher || type == GrowthReportType.AssignedTeachers)
            {
                teachers.ForEach(teacher =>
                {
                    if (completedTeachers.Contains(teacher.Id))
                    {
                        //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
                        CotStgReportEntity cotStgReport = _cotBusiness.GetLastReport(assessmentId, year, teacher.Id);
                        if (cotStgReport != null)
                        {
                            var observer = _userBusiness.GetUserBaseModel(cotStgReport.CreatedBy);
                            observerName = observer.FullName;
                        }
                        else
                        {
                            CotWaveEntity cotWave = _cotBusiness.GetLastCotWave(assessmentId, year, teacher.Id);
                            if (cotWave != null)
                            {
                                var observer = _userBusiness.GetUserBaseModel(cotWave.CreatedBy);
                                observerName = observer.FullName;
                            }
                            else
                                observerName = "N/A";
                        }
                        var report = new GrowthReportModel()
                        {
                            Community = teacher.CommunityNameText,
                            Mentor = observerName,
                            School = teacher.SchoolNameText,
                            SchoolYear = year,
                            Teacher = teacher.FullName,
                            Type = type
                        };
                        report.CoachModels.Add(teacher.CoachingModel);
                        report.ECircles.Add(teacher.ECircle);
                        report.YearsInProject.Add(_userBusiness.GetYearsInProjectText(teacher.YearsInProjectId));

                        var itemsOfTeacher = items.Where(x => x.TeacherId == teacher.Id).ToList();
                        report.BoyDate = itemsOfTeacher.Any(i => i.BoyDate > CommonAgent.MinDate)
                               ? itemsOfTeacher.Where(i => i.BoyDate > CommonAgent.MinDate)
                                   .Min(x => x.BoyDate)
                               : CommonAgent.MinDate;
                        report.MoyDate = itemsOfTeacher.Any(i => i.MoyDate > CommonAgent.MinDate)
                                ? itemsOfTeacher.Where(i => i.MoyDate > CommonAgent.MinDate)
                                    .Max(x => x.MoyDate)
                                : CommonAgent.MinDate;
                        report.MetDate = itemsOfTeacher.Any(i => i.MetDate > CommonAgent.MinDate)
                                ? itemsOfTeacher.Where(i => i.MetDate > CommonAgent.MinDate)
                                    .Max(x => x.MetDate)
                                : CommonAgent.MinDate;


                        report.Measures = measures.Select(x => new Growth.MeasureModel()
                        {
                            ID = x.ID,
                            Name = x.Name,
                            ShortName = x.ShortName,
                            ItemCount = x.ItemCount,
                            CountOfMonth = growthMontheList.ToDictionary(
                            date => date.ToString("MMM\nyyyy"),
                            date => itemsOfTeacher.Where(i => i.MeasureId == x.ID)
                                .Count(i => startDate <= i.CompareDate && i.CompareDate <= date)),
                            Items = x.Items.Select(i => new GrowthItemModel()
                            {
                                CotItemNo = i.CotItemNo,
                                Count = itemsOfTeacher.Count(it => it.Id == i.Id),
                                Id = i.Id,
                                Level = i.Level,
                                Description = i.Description,
                                SetDate = itemsOfTeacher.Where(s => s.Id == i.Id).Any() ? itemsOfTeacher.Where(s => s.Id == i.Id).First().SetDate : CommonAgent.MinDate,
                                MetDate = itemsOfTeacher.Where(s => s.Id == i.Id).Any() ? itemsOfTeacher.Where(s => s.Id == i.Id).First().MetDate : CommonAgent.MinDate,
                                Links = allLinks.Where(l => l.HostId == i.Id).ToList()
                            }).OrderBy(i => i.Level)
                        });
                        reports.Add(report);
                    }
                });
            }
            return reports;
        }

        public List<GrowthReportModel> GetGrowthReports(int assessmentId, int teacherId, int schoolId, int districtId, int year,
           IEnumerable<int> measureIds, GrowthReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels, List<AssignmentType> eCircles, UserBaseEntity user)
        {
            var schoolYear = year.ToSchoolYearString();
            var teachers = GetTeachers(teacherId, schoolId, districtId, (int)type, yearsInProject, coachModels, eCircles, user);
            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();
            if (completedTeachers == null || !completedTeachers.Any())
                return null;
            teachers = teachers.Where(x => completedTeachers.Contains(x.Id)).ToList();

            var measures = CotGrowthMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => (x.BoyObsDate > CommonAgent.MinDate
                        || x.MoyObsDate > CommonAgent.MinDate
                        || x.CotUpdatedOn > CommonAgent.MinDate
                        || x.GoalMetDate > CommonAgent.MinDate
                        )
                        && (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)))
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        MetDate = x.GoalMetDate,
                        CotUpdatedOn = x.CotUpdatedOn,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的

            var reports = new List<GrowthReportModel>();
            var teacher = teachers.First();
            var mentor = "";
            //var mentorCount = teachers.Where(x => x.CoachId > 0).Select(x => x.CoachId).Distinct().Count();
            //if (mentorCount == 0) mentor = "N/A";
            //else if (mentorCount == 1) mentor = teachers.First(x => x.CoachId > 0).Coach.FullName;
            //else mentor = "Various";

            //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
            List<int> teacherIds = teachers.Select(x => x.Id).ToList();
            List<CotStgReportEntity> cotStgReports = _cotBusiness.GetLastReports(assessmentId, year, teacherIds);
            int cotStgReportsCount = cotStgReports.Count;
            if (cotStgReportsCount == 0)
            {
                List<CotWaveEntity> cotWaves = _cotBusiness.GetLastCotWaves(assessmentId, year, teacherIds);
                int cotWaveCount = cotWaves.Count;
                if (cotWaveCount == 0)
                    mentor = "N/A";
                else if (cotWaveCount == 1)
                    mentor = _userBusiness.GetUserBaseModel(cotWaves.First().CreatedBy).FullName;
                else
                    mentor = "Various";
            }
            else if (cotStgReportsCount == 1)
                mentor = _userBusiness.GetUserBaseModel(cotStgReports.First().CreatedBy).FullName;
            else
                mentor = "Various";

            var schools = teachers.Select(x => x.Schools).Aggregate(new List<BasicSchoolModel>(), (first, next) =>
            {
                first.AddRange(next);
                return first;
            }).DistinctBy(x => x.ID).ToList();
            var schoolCount = schools.Count();
            if (type == GrowthReportType.AssignedTeachersAverage)
            {
                GetAssignedTeachersGrowthReport(year, type, yearsInProject, coachModels, eCircles, teacher, mentor, schoolCount, teachers, items, measures, reports);
            }
            else if (type == GrowthReportType.SchoolAverage)
            {
                GetSchoolAverageGrowthReport(year, type, yearsInProject, coachModels, eCircles, teacher, mentor, teachers, items, measures, reports);
            }
            else if (type == GrowthReportType.AllAssignedSchoolAverage)
            {
                TeacherModel choseTeacher = _userBusiness.GetTeacherModel(teacherId);
                List<int> communityIds = choseTeacher.Communities.Select(c => c.ID).Distinct().ToList();
                GetAllAssignedSchoolAverageGrowthReport(year, type, yearsInProject, coachModels, eCircles, teachers, items, measures, reports, communityIds);
            }
            else if (type == GrowthReportType.DistrictAverage)
            {
                GeDistrictAverageGrwothReport(year, type, yearsInProject, coachModels, eCircles, teacher, mentor, schoolCount, teachers, items, measures, reports);
            }
            return reports;
        }

        private static void GeDistrictAverageGrwothReport(int year, GrowthReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, TeacherModel teacher, string mentor, int schoolCount, List<TeacherModel> teachers, List<CotItemModel> items, IEnumerable<Growth.MeasureModel> measures,
            List<GrowthReportModel> reports)
        {
            DateTime startDate = CommonAgent.GetStartDateOfYear(year);
            List<DateTime> growthMontheList = GetGrowthMonthes(year);
            var report = new GrowthReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = schoolCount.ToString(),
                SchoolYear = year,
                Teacher = teachers.Count.ToString(),
                Type = type,
                YearsInProject = yearsInProject.Values.ToList(),
                CoachModels = coachModels,
                ECircles = eCircles,
                BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                        .Min(x => x.BoyDate)
                    : CommonAgent.MinDate,
                MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                        .Max(x => x.MoyDate)
                    : CommonAgent.MinDate,
                MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                        .Max(x => x.MetDate)
                    : CommonAgent.MinDate,
                Measures = measures.Select(x => new Growth.MeasureModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    ItemCount = x.ItemCount
                }).ToList()
            };
            report.Measures.ForEach(mea => growthMontheList.ForEach(month =>
            {
                var itemOfSectionMonth =
                    items.Where(
                        x =>
                            x.MeasureId == mea.ID && startDate <= x.CompareDate && x.CompareDate <= month).ToList();
                var ts = itemOfSectionMonth.Select(x => x.TeacherId).Distinct().Count();
                mea.CountOfMonth.Add(month.ToString("MMM\nyyyy"), ts > 0 ? itemOfSectionMonth.Count() / ts : 0);
            }));
            reports.Add(report);
        }

        private void GetAllAssignedSchoolAverageGrowthReport(int year, GrowthReportType type, Dictionary<int, string> yearsInProject,
            List<AssignmentType> coachModels, List<AssignmentType> eCircles, List<TeacherModel> teachers, List<CotItemModel> items,
            IEnumerable<Growth.MeasureModel> measures, List<GrowthReportModel> reports, List<int> communityIds)
        {
            DateTime startDate = CommonAgent.GetStartDateOfYear(year);
            List<DateTime> growthMontheList = GetGrowthMonthes(year);
            string mentor;
            var schools = new List<BasicSchoolModel>();
            teachers.ForEach(tea => schools.AddRange(tea.Schools));
            schools = schools.DistinctBy(x => x.ID).ToList();
            SchoolEntity schoolEntity = null;
            schools.ForEach(school =>
            {
                //只留当前选中Teacher所在Community，排除其他Community
                schoolEntity = _schoolBusiness.GetSchool(school.ID);
                if (!schoolEntity.CommunitySchoolRelations.Any(csr => communityIds.Contains(csr.CommunityId)))
                    return;

                var teachersOfSchool = teachers.Where(t => t.Schools.Any(s => s.ID == school.ID)).ToList();
                var teacherOfSchool = teachersOfSchool.First();
                var mentorCount = teachersOfSchool.Where(x => x.CoachId > 0).Select(x => x.CoachId).Distinct().Count();
                if (mentorCount == 0) mentor = "N/A";
                else if (mentorCount == 1) mentor = teachersOfSchool.First(x => x.CoachId > 0).Coach.FullName;
                else mentor = "Various";
                var itemsOfSchool = items.Where(x => teachersOfSchool.Any(t => t.Id == x.TeacherId)).ToList();
                var report = new GrowthReportModel()
                {
                    Community = school.CommunitiesText,
                    Mentor = mentor,
                    School = teacherOfSchool.SchoolNameText,
                    SchoolYear = year,
                    Teacher = teachersOfSchool.Count.ToString(),
                    Type = type,
                    YearsInProject = yearsInProject.Values.ToList(),
                    CoachModels = coachModels,
                    ECircles = eCircles,
                    BoyDate = itemsOfSchool.Any(i => i.BoyDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.BoyDate > CommonAgent.MinDate)
                            .Min(x => x.BoyDate)
                        : CommonAgent.MinDate,
                    MoyDate = itemsOfSchool.Any(i => i.MoyDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.MoyDate > CommonAgent.MinDate)
                            .Max(x => x.MoyDate)
                        : CommonAgent.MinDate,
                    MetDate = itemsOfSchool.Any(i => i.MetDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.MetDate > CommonAgent.MinDate)
                            .Max(x => x.MetDate)
                        : CommonAgent.MinDate,
                    Measures = measures.Select(x => new Growth.MeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        ItemCount = x.ItemCount
                    }).ToList()
                };
                report.Measures.ForEach(mea => growthMontheList.ForEach(month =>
                {
                    var itemOfSectionMonth =
                        itemsOfSchool.Where(
                            x =>
                                x.MeasureId == mea.ID && startDate <= x.CompareDate && x.CompareDate <= month).ToList();
                    var ts = itemOfSectionMonth.Select(x => x.TeacherId).Distinct().Count();
                    mea.CountOfMonth.Add(month.ToString("MMM\nyyyy"), ts > 0 ? itemOfSectionMonth.Count() / ts : 0);
                }));
                reports.Add(report);
            });
        }

        private void GetSchoolAverageGrowthReport(int year, GrowthReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, TeacherModel teacher, string mentor, List<TeacherModel> teachers, List<CotItemModel> items, IEnumerable<Growth.MeasureModel> measures, List<GrowthReportModel> reports)
        {
            DateTime startDate = CommonAgent.GetStartDateOfYear(year);
            List<DateTime> growthMontheList = GetGrowthMonthes(year);
            var report = new GrowthReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = teacher.SchoolNameText,
                SchoolYear = year,
                Teacher = teachers.Count.ToString(),
                Type = type,
                YearsInProject = yearsInProject.Values.ToList(),
                CoachModels = coachModels,
                ECircles = eCircles,
                BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                        .Min(x => x.BoyDate)
                    : CommonAgent.MinDate,
                MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                        .Max(x => x.MoyDate)
                    : CommonAgent.MinDate,
                MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                        .Max(x => x.MetDate)
                    : CommonAgent.MinDate,
                Measures = measures.Select(x => new Growth.MeasureModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    ItemCount = x.ItemCount
                }).ToList()
            };
            report.Measures.ForEach(mea => growthMontheList.ForEach(month =>
            {
                var itemOfSectionMonth =
                    items.Where(
                        x =>
                            x.MeasureId == mea.ID && startDate <= x.CompareDate && x.CompareDate <= month).ToList();
                var ts = itemOfSectionMonth.Select(x => x.TeacherId).Distinct().Count();
                mea.CountOfMonth.Add(month.ToString("MMM\nyyyy"), ts > 0 ? itemOfSectionMonth.Count() / ts : 0);
            }));
            reports.Add(report);
        }

        private void GetAssignedTeachersGrowthReport(int year, GrowthReportType type, Dictionary<int, string> yearsInProject,
            List<AssignmentType> coachModels, List<AssignmentType> eCircles, TeacherModel teacher, string mentor, int schoolCount, List<TeacherModel> teachers, List<CotItemModel> items,
            IEnumerable<Growth.MeasureModel> measures, List<GrowthReportModel> reports)
        {
            DateTime startDate = CommonAgent.GetStartDateOfYear(year);
            List<DateTime> growthMontheList = GetGrowthMonthes(year);
            var report = new GrowthReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = schoolCount.ToString(),
                SchoolYear = year,
                Teacher = teachers.Count.ToString(),
                Type = type,
                YearsInProject = yearsInProject.Values.ToList(),
                CoachModels = coachModels,
                ECircles = eCircles,
                BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                        .Min(x => x.BoyDate)
                    : CommonAgent.MinDate,
                MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                        .Max(x => x.MoyDate)
                    : CommonAgent.MinDate,
                MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                        .Max(x => x.MetDate)
                    : CommonAgent.MinDate,
                Measures = measures.Select(x => new Growth.MeasureModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    ItemCount = x.ItemCount
                }).ToList()
            };
            report.Measures.ForEach(mea => growthMontheList.ForEach(month =>
            {
                var itemOfSectionMonth =
                    items.Where(
                        x =>
                            x.MeasureId == mea.ID && startDate <= x.CompareDate && x.CompareDate <= month).ToList();
                var ts = itemOfSectionMonth.Select(x => x.TeacherId).Distinct().Count();
                mea.CountOfMonth.Add(month.ToString("MMM\nyyyy"), ts > 0 ? itemOfSectionMonth.Count() / ts : 0);
            }));
            reports.Add(report);
        }

        #endregion

        #region Summary

        private IQueryable<int> Assessments(int assessmentId, int teacherId, List<int> teacherIds,
            SummaryReportType type, string schoolYear)
        {
            Expression<Func<CotAssessmentEntity, bool>> condition = x => x.AssessmentId == assessmentId;

            switch (type)
            {
                case SummaryReportType.SingleTeacher:
                    condition = condition.And(x => x.TeacherId == teacherId);
                    break;
                case SummaryReportType.AssignedTeachers:
                case SummaryReportType.AssignedTeachersAverage:
                case SummaryReportType.SchoolAverage:
                case SummaryReportType.AllAssignedSchoolAverage:
                case SummaryReportType.DistrictAverage:
                    condition = condition.And(x => teacherIds.Contains(x.TeacherId));
                    break;
                default:
                    break;
            }
            condition = condition.And(x => x.SchoolYear == schoolYear);
            var assessments = _cotContract.Assessments.AsExpandable().Where(condition).Select(x => x.ID);
            return assessments;
        }

        public IEnumerable<SummaryMeasureModel> CotSummaryMeasureModels(int assessmentId,
           IEnumerable<int> measureIds)
        {
            Expression<Func<MeasureEntity, bool>> condition =
                x => x.AssessmentId == assessmentId
                    && x.Status == EntityStatus.Active
                    && x.IsDeleted == false
                    && measureIds.Contains(x.ID);
            List<SummaryMeasureModel> measures =
                _adeData.Measures.AsExpandable().Where(condition).OrderBy(x => x.Sort)
                    .Select(x => new SummaryMeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        ItemCount = x.Items.Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false).OfType<CotItemEntity>().Count(),
                        SubItemCount = x.SubMeasures.Any() ?
                        x.SubMeasures.Sum(sub => sub.Items.OfType<CotItemEntity>()
                            .Count(i => i.Status == EntityStatus.Active && i.IsDeleted == false)) : 0,
                        Items = x.Items.Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false).OfType<CotItemEntity>()
                            .Select(i => new SummaryItemModel()
                            {
                                CotItemNo = i.CotItemId,
                                Count = 0,
                                Id = i.ID,
                                Level = i.Level,
                                Description = i.Description
                            }),
                        ItemsOfSubMeasure = x.SubMeasures.Select(sub => sub.Items.OfType<CotItemEntity>()
                            .Where(i => i.Status == EntityStatus.Active && i.IsDeleted == false)
                                        .Select(i => new SummaryItemModel()
                                        {
                                            CotItemNo = i.CotItemId,
                                            Count = 0,
                                            Id = i.ID,
                                            Level = i.Level,
                                            Description = i.Description
                                        }))
                    }).ToList();
            return measures;
        }

        public List<SummaryReportModel> GetSummaryReports(int assessmentId, int teacherId, string schoolId, int districtId, int year,
            IEnumerable<int> measureIds, SummaryReportType type, UserBaseEntity user, IEnumerable<int> teacherIds)
        {
            var schoolYear = year.ToSchoolYearString();
            DateTime startDate = CommonAgent.GetStartDateOfYear(year);
            List<DateTime> growthMontheList = GetGrowthMonthes(year);
            List<TeacherModel> teachers = null;
            if (teacherIds.Any())
            {
                Expression<Func<TeacherEntity, bool>> condition = x => true;
                condition = condition.And(x => teacherIds.Contains(x.ID));
                teachers = _userBusiness.GetTeacherModels(condition);
            }
            else
            {
                teachers = GetTeachers(teacherId, schoolId, districtId, (int)type, user);
            }
            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();

            var measures = CotSummaryMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => (x.BoyObsDate > CommonAgent.MinDate
                        || x.MoyObsDate > CommonAgent.MinDate
                        || x.CotUpdatedOn > CommonAgent.MinDate
                        || x.GoalMetDate > CommonAgent.MinDate
                        )
                        && (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)))
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        MetDate = x.GoalMetDate,
                        CotUpdatedOn = x.CotUpdatedOn,
                        SetDate = x.GoalSetDate,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的

            var itemIds = _cotContract.Items
                    .Where(x => (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)))
                    .Select(x => x.ItemId).Distinct();
            var allLinks = _adeBusiness.GetLinkModels<ItemBaseEntity>(itemIds.ToArray());
            if (allLinks == null)
            {
                allLinks = new List<Ade.Models.AdeLinkModel>();
            }
            var reports = new List<SummaryReportModel>();
            string observerName = "N/A";
            if (type == SummaryReportType.SingleTeacher || type == SummaryReportType.AssignedTeachers)
            {
                teachers.ForEach(teacher =>
                {
                    if (completedTeachers.Contains(teacher.Id))
                    {
                        //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
                        CotStgReportEntity cotStgReport = _cotBusiness.GetLastReport(assessmentId, year, teacher.Id);
                        if (cotStgReport != null)
                        {
                            var observer = _userBusiness.GetUserBaseModel(cotStgReport.CreatedBy);
                            observerName = observer.FullName;
                        }
                        else
                        {
                            CotWaveEntity cotWave = _cotBusiness.GetLastCotWave(assessmentId, year, teacher.Id);
                            if (cotWave != null)
                            {
                                var observer = _userBusiness.GetUserBaseModel(cotWave.CreatedBy);
                                observerName = observer.FullName;
                            }
                            else
                                observerName = "N/A";
                        }

                        var report = new SummaryReportModel()
                        {
                            Community = teacher.CommunityNameText,
                            Mentor = observerName,
                            School = teacher.SchoolNameText,
                            SchoolYear = year,
                            Teacher = teacher.FullName,
                            Type = type
                        };
                        report.CoachModels.Add(teacher.CoachingModel);
                        report.ECircles.Add(teacher.ECircle);
                        report.YearsInProject.Add(_userBusiness.GetYearsInProjectText(teacher.YearsInProjectId));

                        var itemsOfTeacher = items.Where(x => x.TeacherId == teacher.Id).ToList();
                        report.BoyDate = itemsOfTeacher.Any(i => i.BoyDate > CommonAgent.MinDate)
                               ? itemsOfTeacher.Where(i => i.BoyDate > CommonAgent.MinDate)
                                   .Min(x => x.BoyDate)
                               : CommonAgent.MinDate;
                        report.MoyDate = itemsOfTeacher.Any(i => i.MoyDate > CommonAgent.MinDate)
                                ? itemsOfTeacher.Where(i => i.MoyDate > CommonAgent.MinDate)
                                    .Max(x => x.MoyDate)
                                : CommonAgent.MinDate;
                        report.MetDate = itemsOfTeacher.Any(i => i.MetDate > CommonAgent.MinDate)
                                ? itemsOfTeacher.Where(i => i.MetDate > CommonAgent.MinDate)
                                    .Max(x => x.MetDate)
                                : CommonAgent.MinDate;

                        report.Measures = measures.Select(x => new SummaryMeasureModel()
                        {
                            ID = x.ID,
                            Name = x.Name,
                            ShortName = x.ShortName,
                            ItemCount = x.ItemCount,
                            FillColorCount = itemsOfTeacher.Count(i => i.MeasureId == x.ID),
                            CountOfMonth = growthMontheList.ToDictionary(
                            date => date.ToString("MMM\nyyyy"),
                            date => itemsOfTeacher.Where(i => i.MeasureId == x.ID)
                                .Count(i => startDate <= i.CompareDate && i.CompareDate <= date)),
                            Items = x.Items.Select(i => new SummaryItemModel()
                            {
                                CotItemNo = i.CotItemNo,
                                Count = itemsOfTeacher.Count(it => it.Id == i.Id),
                                Id = i.Id,
                                Level = i.Level,
                                Description = i.Description,
                                SetDate = itemsOfTeacher.Where(s => s.Id == i.Id).Any() ? itemsOfTeacher.Where(s => s.Id == i.Id).First().SetDate : CommonAgent.MinDate,
                                MetDate = itemsOfTeacher.Where(s => s.Id == i.Id).Any() ? itemsOfTeacher.Where(s => s.Id == i.Id).First().MetDate : CommonAgent.MinDate,
                                Links = allLinks.Where(l => l.HostId == i.Id).ToList()
                            }).OrderBy(i => i.Level)
                        });
                        reports.Add(report);
                    }
                });
            }
            return reports;
        }

        public List<SummaryReportModel> GetSummaryReports(int assessmentId, int teacherId, int schoolId, int districtId, int year,
           IEnumerable<int> measureIds, SummaryReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels, List<AssignmentType> eCircles, UserBaseEntity user)
        {
            var schoolYear = year.ToSchoolYearString();
            var teachers = GetTeachers(teacherId, schoolId, districtId, (int)type, yearsInProject, coachModels, eCircles, user);
            if (teachers == null || !teachers.Any())
                return null;
            var tIds = teachers.Select(t => t.Id).ToList();
            var completedTeachers =
                _cotContract.Assessments.Where(x =>
                    x.AssessmentId == assessmentId
                    && x.SchoolYear == schoolYear
                    && tIds.Contains(x.TeacherId)
                    && x.Waves.Count(w => w.Status >= CotWaveStatus.Finalized || w.Status == CotWaveStatus.OldData) >= 1)
                    .Select(x => x.TeacherId).ToList();
            if (completedTeachers == null || !completedTeachers.Any())
                return null;
            teachers = teachers.Where(x => completedTeachers.Contains(x.Id)).ToList();

            var measures = CotSummaryMeasureModels(assessmentId, measureIds);

            var assessments = Assessments(assessmentId, teacherId, completedTeachers, type, schoolYear);
            var items =
                _cotContract.Items.Where(x => assessments.Contains(x.CotAssessmentId))
                    .Where(x => (x.BoyObsDate > CommonAgent.MinDate
                        || x.MoyObsDate > CommonAgent.MinDate
                        || x.CotUpdatedOn > CommonAgent.MinDate
                        || x.GoalMetDate > CommonAgent.MinDate
                        )
                        && (measureIds.Contains(x.Item.MeasureId) || measureIds.Contains(x.Item.Measure.ParentId)))
                    .Select(x => new CotItemModel()
                    {
                        Id = x.ItemId,
                        MeasureId = x.Item.Measure.ParentId > 1 ? x.Item.Measure.ParentId : x.Item.MeasureId,
                        BoyDate = x.BoyObsDate,
                        MoyDate = x.MoyObsDate,
                        MetDate = x.GoalMetDate,
                        CotUpdatedOn = x.CotUpdatedOn,
                        TeacherId = x.Assessment.TeacherId
                    }).ToList(); // Item 的 MeasureId 必须使用顶级MeasureId,因为是按顶级MeasureId 统计的

            var reports = new List<SummaryReportModel>();
            var teacher = teachers.First();
            var mentor = "";
            //var mentorCount = teachers.Where(x => x.CoachId > 0).Select(x => x.CoachId).Distinct().Count();
            //if (mentorCount == 0) mentor = "N/A";
            //else if (mentorCount == 1) mentor = teachers.First(x => x.CoachId > 0).Coach.FullName;
            //else mentor = "Various";

            //On the Observer field, display the first and last name of the User who created the last Short Term Goal report,or the last COT assessment
            List<int> teacherIds = teachers.Select(x => x.Id).ToList();
            List<CotStgReportEntity> cotStgReports = _cotBusiness.GetLastReports(assessmentId, year, teacherIds);
            int cotStgReportsCount = cotStgReports.Count;
            if (cotStgReportsCount == 0)
            {
                List<CotWaveEntity> cotWaves = _cotBusiness.GetLastCotWaves(assessmentId, year, teacherIds);
                int cotWaveCount = cotWaves.Count;
                if (cotWaveCount == 0)
                    mentor = "N/A";
                else if (cotWaveCount == 1)
                    mentor = _userBusiness.GetUserBaseModel(cotWaves.First().CreatedBy).FullName;
                else
                    mentor = "Various";
            }
            else if (cotStgReportsCount == 1)
                mentor = _userBusiness.GetUserBaseModel(cotStgReports.First().CreatedBy).FullName;
            else
                mentor = "Various";

            var schools = teachers.Select(x => x.Schools).Aggregate(new List<BasicSchoolModel>(), (first, next) =>
            {
                first.AddRange(next);
                return first;
            }).DistinctBy(x => x.ID).ToList();
            var schoolCount = schools.Count();
            if (type == SummaryReportType.AssignedTeachersAverage)
            {
                GetAssignedTeachersSummaryReport(year, type, yearsInProject, coachModels, eCircles, teacher, mentor, schoolCount, teachers, items, measures, reports);
            }
            else if (type == SummaryReportType.SchoolAverage)
            {
                GetSchoolAverageSummaryReport(year, type, yearsInProject, coachModels, eCircles, teacher, mentor, teachers, items, measures, reports);
            }
            else if (type == SummaryReportType.AllAssignedSchoolAverage)
            {
                TeacherModel choseTeacher = _userBusiness.GetTeacherModel(teacherId);
                List<int> communityIds = choseTeacher.Communities.Select(c => c.ID).Distinct().ToList();
                GetAllAssignedSchoolAverageSummaryReport(year, type, yearsInProject, coachModels, eCircles, teachers, items, measures, reports, communityIds);
            }
            else if (type == SummaryReportType.DistrictAverage)
            {
                GeDistrictAverageSummaryReport(year, type, yearsInProject, coachModels, eCircles, teacher, mentor, schoolCount, teachers, items, measures, reports);
            }
            return reports;
        }

        private static void GeDistrictAverageSummaryReport(int year, SummaryReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, TeacherModel teacher, string mentor, int schoolCount, List<TeacherModel> teachers, List<CotItemModel> items, IEnumerable<SummaryMeasureModel> measures,
            List<SummaryReportModel> reports)
        {
            var report = new SummaryReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = schoolCount.ToString(),
                SchoolYear = year,
                Teacher = teachers.Count.ToString(),
                Type = type,
                YearsInProject = yearsInProject.Values.ToList(),
                CoachModels = coachModels,
                ECircles = eCircles,
                BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                        .Min(x => x.BoyDate)
                    : CommonAgent.MinDate,
                MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                        .Max(x => x.MoyDate)
                    : CommonAgent.MinDate,
                MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                        .Max(x => x.MetDate)
                    : CommonAgent.MinDate,
                Measures = measures.Select(x => new SummaryMeasureModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    ItemCount = x.ItemCount
                }).ToList()
            };
            report.Measures.ForEach(mea =>
            {
                var measureItems = items.Where(x => x.MeasureId == mea.ID).ToList();
                var itemTeacherCount = measureItems.Select(x => x.TeacherId).Distinct().Count();
                mea.FillColorCount = itemTeacherCount > 0 ? measureItems.Count / itemTeacherCount : 0;
            });
            reports.Add(report);
        }

        private void GetAllAssignedSchoolAverageSummaryReport(int year, SummaryReportType type, Dictionary<int, string> yearsInProject,
            List<AssignmentType> coachModels, List<AssignmentType> eCircles, List<TeacherModel> teachers, List<CotItemModel> items,
            IEnumerable<SummaryMeasureModel> measures, List<SummaryReportModel> reports, List<int> communityIds)
        {
            string mentor;
            var schools = new List<BasicSchoolModel>();
            teachers.ForEach(tea => schools.AddRange(tea.Schools));
            schools = schools.DistinctBy(x => x.ID).ToList();
            SchoolEntity schoolEntity = null;
            schools.ForEach(school =>
            {
                //只留当前选中Teacher所在Community，排除其他Community
                schoolEntity = _schoolBusiness.GetSchool(school.ID);
                if (!schoolEntity.CommunitySchoolRelations.Any(csr => communityIds.Contains(csr.CommunityId)))
                    return;

                var teachersOfSchool = teachers.Where(t => t.Schools.Any(s => s.ID == school.ID)).ToList();
                var teacherOfSchool = teachersOfSchool.First();
                var mentorCount = teachersOfSchool.Where(x => x.CoachId > 0).Select(x => x.CoachId).Distinct().Count();
                if (mentorCount == 0) mentor = "N/A";
                else if (mentorCount == 1) mentor = teachersOfSchool.First(x => x.CoachId > 0).Coach.FullName;
                else mentor = "Various";
                var itemsOfSchool = items.Where(x => teachersOfSchool.Any(t => t.Id == x.TeacherId)).ToList();
                var report = new SummaryReportModel()
                {
                    Community = school.CommunitiesText,
                    Mentor = mentor,
                    School = teacherOfSchool.SchoolNameText,
                    SchoolYear = year,
                    Teacher = teachersOfSchool.Count.ToString(),
                    Type = type,
                    YearsInProject = yearsInProject.Values.ToList(),
                    CoachModels = coachModels,
                    ECircles = eCircles,
                    BoyDate = itemsOfSchool.Any(i => i.BoyDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.BoyDate > CommonAgent.MinDate)
                            .Min(x => x.BoyDate)
                        : CommonAgent.MinDate,
                    MoyDate = itemsOfSchool.Any(i => i.MoyDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.MoyDate > CommonAgent.MinDate)
                            .Max(x => x.MoyDate)
                        : CommonAgent.MinDate,
                    MetDate = itemsOfSchool.Any(i => i.MetDate > CommonAgent.MinDate)
                        ? itemsOfSchool.Where(i => i.MetDate > CommonAgent.MinDate)
                            .Max(x => x.MetDate)
                        : CommonAgent.MinDate,
                    Measures = measures.Select(x => new SummaryMeasureModel()
                    {
                        ID = x.ID,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        ItemCount = x.ItemCount
                    }).ToList()
                };
                report.Measures.ForEach(mea =>
                {
                    var measureItems = items.Where(x => x.MeasureId == mea.ID).ToList();
                    var itemTeacherCount = measureItems.Select(x => x.TeacherId).Distinct().Count();
                    mea.FillColorCount = itemTeacherCount > 0 ? measureItems.Count / itemTeacherCount : 0;
                });
                reports.Add(report);
            });
        }

        private void GetSchoolAverageSummaryReport(int year, SummaryReportType type, Dictionary<int, string> yearsInProject, List<AssignmentType> coachModels,
            List<AssignmentType> eCircles, TeacherModel teacher, string mentor, List<TeacherModel> teachers, List<CotItemModel> items,
            IEnumerable<SummaryMeasureModel> measures, List<SummaryReportModel> reports)
        {
            var report = new SummaryReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = teacher.SchoolNameText,
                SchoolYear = year,
                Teacher = teachers.Count.ToString(),
                Type = type,
                YearsInProject = yearsInProject.Values.ToList(),
                CoachModels = coachModels,
                ECircles = eCircles,
                BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                        .Min(x => x.BoyDate)
                    : CommonAgent.MinDate,
                MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                        .Max(x => x.MoyDate)
                    : CommonAgent.MinDate,
                MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                        .Max(x => x.MetDate)
                    : CommonAgent.MinDate,
                Measures = measures.Select(x => new SummaryMeasureModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    ItemCount = x.ItemCount
                }).ToList()
            };
            report.Measures.ForEach(mea =>
            {
                var measureItems = items.Where(x => x.MeasureId == mea.ID).ToList();
                var itemTeacherCount = measureItems.Select(x => x.TeacherId).Distinct().Count();
                mea.FillColorCount = itemTeacherCount > 0 ? measureItems.Count / itemTeacherCount : 0;
            });
            reports.Add(report);
        }

        private void GetAssignedTeachersSummaryReport(int year, SummaryReportType type, Dictionary<int, string> yearsInProject,
            List<AssignmentType> coachModels, List<AssignmentType> eCircles, TeacherModel teacher, string mentor, int schoolCount, List<TeacherModel> teachers, List<CotItemModel> items,
            IEnumerable<SummaryMeasureModel> measures, List<SummaryReportModel> reports)
        {
            var report = new SummaryReportModel()
            {
                Community = teacher.CommunityNameText,
                Mentor = mentor,
                School = schoolCount.ToString(),
                SchoolYear = year,
                Teacher = teachers.Count.ToString(),
                Type = type,
                YearsInProject = yearsInProject.Values.ToList(),
                CoachModels = coachModels,
                ECircles = eCircles,
                BoyDate = items.Any(i => i.BoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.BoyDate > CommonAgent.MinDate)
                        .Min(x => x.BoyDate)
                    : CommonAgent.MinDate,
                MoyDate = items.Any(i => i.MoyDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MoyDate > CommonAgent.MinDate)
                        .Max(x => x.MoyDate)
                    : CommonAgent.MinDate,
                MetDate = items.Any(i => i.MetDate > CommonAgent.MinDate)
                    ? items.Where(i => i.MetDate > CommonAgent.MinDate)
                        .Max(x => x.MetDate)
                    : CommonAgent.MinDate,
                Measures = measures.Select(x => new SummaryMeasureModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    ItemCount = x.ItemCount
                }).ToList()
            };
            report.Measures.ForEach(mea =>
            {
                var measureItems = items.Where(x => x.MeasureId == mea.ID).ToList();
                var itemTeacherCount = measureItems.Select(x => x.TeacherId).Distinct().Count();
                mea.FillColorCount = itemTeacherCount > 0 ? measureItems.Count / itemTeacherCount : 0;
                //  GrowthMonthes.ForEach(month =>
                //{
                //    var itemOfSectionMonth =
                //        items.Where(
                //            x =>
                //                x.MeasureId == mea.ID && Growth_StartDate <= x.CompareDate && x.CompareDate <= month).ToList();
                //    var ts = itemOfSectionMonth.Select(x => x.TeacherId).Distinct().Count();
                //    mea.CountOfMonth.Add(month.ToString("MMM\nyyyy"), ts > 0 ? itemOfSectionMonth.Count() / ts : 0);
                //});
            });
            reports.Add(report);
        }
        #endregion
    }
}
