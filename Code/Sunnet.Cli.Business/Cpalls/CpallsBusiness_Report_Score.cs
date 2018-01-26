using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sunnet.Cli.Business.Cpalls.Growth;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Cpalls.Models.Report;
using LinqKit;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Core.Common.Enums;


namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {
        //School Custom Score Report
        public List<CustomScoreReportModel> GeCommunityScoreReportPdf(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, Wave wave, int year,
              int districtId, List<int> scoreIds, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CustomScoreReportModel> customScoreReports = new List<CustomScoreReportModel>();
            try
            {
                var community = CommunityBusiness.GetCommunity(districtId);
                //查询出Community下所有School，同时查询School下的所有的studentIds
                var schools = SchoolBusiness.GetSchoolList(s => s.Status == SchoolStatus.Active
                && (districtId < 1 || s.CommunitySchoolRelations.Any(com => com.CommunityId == districtId))
                && s.SchoolType.Name.StartsWith("Demo") == false,
                user, language, dobStartDate, dobEndDate);

                var studentIds = new List<int>();
                schools.ForEach(x => studentIds.AddRange(x.StudentIds));

                List<Wave> waveList = new List<Wave>();
                waveList.Add(wave);
                //将school下所有student的finalscore全部查询出来，在后面进行分组处理
                List<ScoreReportModel> scoreReports = _adeBusiness.GetAllFinalResult(studentIds.Distinct().ToList(), assessmentId, waveList,schoolYear,
                    scoreIds, startDate, endDate);
                //下面方法是计算出每个custom score下的maximum score和measure值
                List<ScoreInitModel> scoreInits = _adeBusiness.GetScoreInits(assessmentId, wave, scoreIds);
                foreach (var school in schools)
                {
                    CustomScoreReportModel customScoreReportModel = new CustomScoreReportModel();
                    customScoreReportModel.SchoolId = school.ID;
                    customScoreReportModel.School = school.Name;
                    customScoreReportModel.Community = community.Name;
                    customScoreReportModel.Wave = wave;
                    customScoreReportModel.Language = language;
                    customScoreReportModel.School = school.Name;
                    customScoreReportModel.Year = year;
                    customScoreReportModel.ScoreReports =//
                        scoreReports.Where(e => school.StudentIds.Contains(e.StudentId)
                                                && e.Wave == wave && scoreIds.Contains(e.ScoreId)).ToList();
                    customScoreReports.Add(customScoreReportModel);
                    customScoreReportModel.ScoreInits = scoreInits;
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                return null;
            }
            finally
            {
                GC.Collect();
            }
            return customScoreReports;
        }

        //School Custom Score Report
        public List<CustomScoreReportModel> GetSchoolScoreReportPdf(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, Wave wave, int year,
              int schoolId, List<int> scoreIds, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CustomScoreReportModel> customScoreReports = new List<CustomScoreReportModel>();
            try
            {
                List<CpallsClassModel> classes = null;
                IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
                classes = ClassBusiness.GetClassList(x => classIds.Contains(x.ID) && x.Status == EntityStatus.Active,
                    user, dobStartDate, dobEndDate, language);
                var studentIds = new List<int>();
                classes.ForEach(x => studentIds.AddRange(x.StudentIds));

                List<Wave> waveList = new List<Wave>();
                waveList.Add(wave);
                var school = SchoolBusiness.GetCpallsSchoolModel(schoolId);
                List<ScoreReportModel> scoreReports = _adeBusiness.GetAllFinalResult(studentIds.Distinct().ToList(), assessmentId, waveList, schoolYear,
                    scoreIds, startDate, endDate);
                //下面方法是计算出每个custom score下的maximum score和measure值
                List<ScoreInitModel> scoreInits = _adeBusiness.GetScoreInits(assessmentId, wave, scoreIds);
                foreach (var cpallsClassModel in classes)
                {
                    CustomScoreReportModel customScoreReportModel = new CustomScoreReportModel();
                    customScoreReportModel.ClassId = cpallsClassModel.ID;
                    customScoreReportModel.Community = school.CommunitiesText;
                    customScoreReportModel.Class = cpallsClassModel.Name;
                    customScoreReportModel.Wave = wave;
                    customScoreReportModel.Language = language;
                    customScoreReportModel.School = school.Name;
                    customScoreReportModel.Teacher = string.Join("; ", cpallsClassModel.Teacher);
                    customScoreReportModel.Year = year;
                    customScoreReportModel.ScoreReports =//
                        scoreReports.Where(e => cpallsClassModel.StudentIds.Contains(e.StudentId)
                                                && e.Wave == wave && scoreIds.Contains(e.ScoreId)).ToList();
                    customScoreReportModel.ScoreInits = scoreInits;
                    customScoreReports.Add(customScoreReportModel);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                return null;
            }
            finally
            {
                GC.Collect();
            }
            return customScoreReports;
        }

        // Class Custom Score Report
        public List<CustomScoreReportModel> GetScoreReportPdf(int assessmentId, StudentAssessmentLanguage language, UserBaseEntity user, Wave wave,int year,
              int schoolId, int classId, bool allClasses, List<int> scoreIds, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate)
        {
            var schoolYear = string.Format("{0}-{1}", year.ToString().Substring(2), (year + 1).ToString().Substring(2));
            List<CustomScoreReportModel> customScoreReports = new List<CustomScoreReportModel>();
            try
            {
                List<CpallsClassModel> classes = null;
                IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, user, assessmentId);
                if (allClasses)
                {
                    classes = ClassBusiness.GetClassList(x => classIds.Contains(x.ID) && x.SchoolId == schoolId && x.Status == EntityStatus.Active,
                    user, dobStartDate,dobEndDate, language);
                }
                else
                {
                    classes = ClassBusiness.GetClassList(x => classIds.Contains(x.ID) && x.ID == classId && x.Status == EntityStatus.Active,
                    user, dobStartDate, dobEndDate, language);
                }
                var studentIds = new List<int>();
                classes.ForEach(x => studentIds.AddRange(x.StudentIds));

                List<Wave> waveList = new List<Wave>();
                waveList.Add(wave);
                var school = SchoolBusiness.GetCpallsSchoolModel(schoolId);
                List<ScoreReportModel> scoreReports = _adeBusiness.GetAllFinalResult(studentIds, assessmentId, waveList, schoolYear,
                    scoreIds, startDate, endDate);
                //下面方法是计算出每个custom score下的maximum score和measure值
                List<ScoreInitModel> scoreInits = _adeBusiness.GetScoreInits(assessmentId, wave, scoreIds);
                foreach (var cpallsClassModel in classes)
                {
                    CustomScoreReportModel customScoreReportModel = new CustomScoreReportModel();
                    customScoreReportModel.Community = school.CommunitiesText;
                    customScoreReportModel.Class = cpallsClassModel.Name;
                    customScoreReportModel.Wave = wave;
                    customScoreReportModel.Language = language;
                    customScoreReportModel.School = school.Name;
                    customScoreReportModel.Teacher = string.Join("; ", cpallsClassModel.Teacher); 
                    customScoreReportModel.Year = year;
                    customScoreReportModel.ScoreReports =//
                        scoreReports.Where(e => cpallsClassModel.StudentIds.Contains(e.StudentId)
                                                && e.Wave == wave && scoreIds.Contains(e.ScoreId)).ToList();
                    customScoreReportModel.ScoreInits = scoreInits;
                    customScoreReports.Add(customScoreReportModel);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                return null;
            }
            finally
            {
                GC.Collect();
            }
            return customScoreReports;
        }

    }
}