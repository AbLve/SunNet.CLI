using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using Sunnet.Cli.Business.Reports;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.MasterData.Configurations;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework;
using Sunnet.Framework.Excel;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Ade.Entities;
using System.Diagnostics;
using Sunnet.Cli.Core.Cpalls.Models.Report;
using Sunnet.Cli.Business.Common;

namespace Sunnet.Cli.Business.Cpalls
{
    public partial class CpallsBusiness
    {
        public List<CompletionMeasureModel> GetCompletion_Bilingual(int assessmentId, int otherAssessmentId, string schoolYear, List<int> measureIdList, Wave wave
            , ReportModel[] titleArray, DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate, IList<int> classIds, int communityId = 0, int schoolId = 0)
        {
            ///自己没有 Relation 但父Measure 有 Relation 的 父MeasureId
            List<int> otherMeasureId = new List<int>();
            foreach (var item in titleArray.Where(r => r.RelatedId > 0 && r.Children != null && r.Children.Any()))
            {//如果一个子measure 没有 Related ，并且是初始化 状态。那么判断他的父Measure 有没有Related，如果有，且对应的 Measure 的Totale.Goal >-1 那么该Measure 为Locked
                if (item.Children.Count(r => r.RelatedId == 0) > 0)
                {
                    otherMeasureId.Add(item.MeasureId);
                }
            }

            List<CompletionMeasureModel> otherList;

            List<CompletionMeasureModel> pendingList = _cpallsContract.GetCompletionCombinedStudentMeasure(
                communityId, schoolId, assessmentId, otherAssessmentId, wave, measureIdList, otherMeasureId, schoolYear, startDate, endDate, dobStartDate, dobEndDate, classIds, out otherList);
            List<CompletionMeasureModel> otherCompletionMeasure = _cpallsContract.GetCompletionEnglishAndSpanishStudentMeasure(
                communityId, schoolId, assessmentId, otherAssessmentId, wave, measureIdList, schoolYear, startDate, endDate, dobStartDate, dobEndDate, classIds);


            otherList.ForEach(r => r.Status = CpallsStatus.Locked);

            pendingList.AddRange(otherList);

            foreach (var v in titleArray)
            {
                if (v.Children != null && v.Children.Any())
                {
                    foreach (var c in v.Children)
                        otherCompletionMeasure.Where(r => r.RelatedMeasureId == c.MeasureId).ForEach(r => r.MeasureId = c.MeasureId);
                }
                else
                    otherCompletionMeasure.Where(r => r.RelatedMeasureId == v.MeasureId).ForEach(r => r.MeasureId = v.MeasureId);
            }


            pendingList.AddRange(otherCompletionMeasure);

            return pendingList;
        }


        /// <summary>
        /// School下Finished或Unlock的StudentMeasure
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public List<CompletionMeasureModel> GetCompletionBilingualForClass(int assessmentId, int otherAssessmentId, StudentAssessmentLanguage language,
            string schoolYear, List<int> measureIdList, Wave wave, int classId, List<int> studentIdList, DateTime startDate, DateTime endDate)
        {
            IQueryable<StudentMeasureEntity> queryable = null;
            if (!studentIdList.Any())
            {
                return new List<CompletionMeasureModel>();
            }
            //Assessment和Class没有关联关系，只能通过StudentId查找
            queryable = _cpallsContract.Measures.Where(
                    r => r.Measure.Status == EntityStatus.Active && r.Measure.IsDeleted == false
                        && (r.Assessment.AssessmentId == assessmentId || r.Assessment.AssessmentId == otherAssessmentId)
                        && studentIdList.Contains(r.Assessment.StudentId)
                        && (r.Status == CpallsStatus.Finished || r.Status == CpallsStatus.Locked)
                        && r.Assessment.SchoolYear == schoolYear
                        && (measureIdList.Contains(r.MeasureId) || measureIdList.Contains(r.Measure.RelatedMeasureId))
                        && r.Assessment.Wave == wave
                        && r.UpdatedOn >= startDate && r.UpdatedOn < endDate);

            return queryable.Select(r => new CompletionMeasureModel()
            {
                MeasureId = r.MeasureId,
                Status = r.Status,
                Sort = r.Measure.Sort,
                MeasureName = r.Measure.Name,
                ParentId = r.Measure.ParentId,
                Wave = r.Assessment.Wave,
                RelatedMeasureId = r.Measure.RelatedMeasureId,
                StudentId = r.Assessment.StudentId,
                Language = r.Assessment.Assessment.Language,
                Goal = r.Goal
            }).ToList();
        }


        /// <summary>
        /// School下Finished或Unlock的StudentMeasure
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="schoolYear"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public List<CompletionMeasureModel> GetCompletionForClass(int assessmentId, StudentAssessmentLanguage language,
            string schoolYear, List<int> measureIdList, Wave wave, int classId, List<int> studentIdList, DateTime startDate, DateTime endDate)
        {
            IQueryable<StudentMeasureEntity> queryable = null;
            if (!studentIdList.Any())
            {
                return new List<CompletionMeasureModel>();
            }
            //Assessment和Class没有关联关系，只能通过StudentId查找
            queryable = _cpallsContract.Measures.Where(
                    r => r.Measure.Status == EntityStatus.Active && r.Measure.IsDeleted == false
                        && r.Assessment.AssessmentId == assessmentId
                        && studentIdList.Contains(r.Assessment.StudentId)
                        && (r.Status == CpallsStatus.Finished || r.Status == CpallsStatus.Locked)
                        && r.Assessment.SchoolYear == schoolYear
                        && measureIdList.Contains(r.MeasureId)
                        && r.Assessment.Wave == wave
                        && r.UpdatedOn >= startDate && r.UpdatedOn < endDate);

            return queryable.Select(r => new CompletionMeasureModel()
            {
                MeasureId = r.MeasureId,
                Status = r.Status,
                Sort = r.Measure.Sort,
                MeasureName = r.Measure.Name,
                ParentId = r.Measure.ParentId,
                Wave = r.Assessment.Wave,
                RelatedMeasureId = r.Measure.RelatedMeasureId,
                StudentId = r.Assessment.StudentId,
                Language = r.Assessment.Assessment.Language,
                Goal = r.Goal
            }).ToList();
        }

        /// <summary>
        /// StudentView，双语是的Completion Report
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="otherAssessmentId"></param>
        /// <param name="language"></param>
        /// <param name="schoolYear"></param>
        /// <param name="measureIdList"></param>
        /// <param name="classId"></param>
        /// <returns></returns>
        public List<CompletionMeasureModel> GetCompletionForClass_Bilingual(int assessmentId, int otherAssessmentId, StudentAssessmentLanguage language,
            string schoolYear, List<int> measureIdList, Wave wave, int classId, ReportModel[] titleArray, List<int> studentIdList, DateTime startDate, DateTime endDate)
        {
            if (!studentIdList.Any())
            {
                return new List<CompletionMeasureModel>();
            }
            //Assessment和Class没有关联关系，只能通过StudentId查找
            List<CompletionMeasureModel> pendingList = _cpallsContract.Measures.Where(
                    r => r.Measure.Status == EntityStatus.Active && r.Measure.IsDeleted == false
                        && (r.Assessment.AssessmentId == assessmentId || r.Assessment.AssessmentId == otherAssessmentId)
                        && studentIdList.Contains(r.Assessment.StudentId)
                        && r.Assessment.SchoolYear == schoolYear
                        && (measureIdList.Contains(r.Measure.ID) || measureIdList.Contains(r.Measure.RelatedMeasureId))
                        && r.Assessment.Wave == wave
                        && r.UpdatedOn >= startDate && r.UpdatedOn < endDate)
                        .Select(r => new CompletionMeasureModel()
                        {
                            MeasureId = r.MeasureId,
                            Status = r.Status,
                            Sort = r.Measure.Sort,
                            MeasureName = r.Measure.Name,
                            ParentId = r.Measure.ParentId,
                            Wave = r.Assessment.Wave,
                            RelatedMeasureId = r.Measure.RelatedMeasureId,
                            StudentId = r.Assessment.StudentId,
                            Language = r.Assessment.Assessment.Language,
                            Goal = r.Goal
                        }).ToList();


            return CompletionBilingual(pendingList, titleArray, wave);
        }


        private List<CompletionMeasureModel> CompletionBilingual(List<CompletionMeasureModel> pendingList, ReportModel[] titleArray, Wave wave)
        {
            List<int> measureHasChilder = titleArray.Where(r => r.Children != null && r.Children.Any()).Select(r => r.MeasureId).ToList();

            List<CompletionMeasureModel> list = new List<CompletionMeasureModel>();

            ///锁定与完成状态的 Measure
            List<CompletionMeasureModel> measureList = pendingList.FindAll(r => !measureHasChilder.Contains(r.MeasureId) && r.Status > CpallsStatus.Paused && r.Wave == r.Wave);

            list.AddRange(measureList.FindAll(r => r.RelatedMeasureId == 0)); //处理没有relation的   
            measureList.RemoveAll(r => r.RelatedMeasureId == 0); //移除 没有relation的   

            List<CompletionMeasureModel> englishFinished = measureList.FindAll(r => r.Language == AssessmentLanguage.English && r.Status == CpallsStatus.Finished);
            list.AddRange(englishFinished); //处理English完成状态的
            measureList.RemoveAll(r => r.Language == AssessmentLanguage.English && r.Status == CpallsStatus.Finished);// 移除 English完成状态的


            englishFinished.ForEach(r =>
            {
                measureList.RemoveAll(m => m.RelatedMeasureId == r.MeasureId && m.StudentId == r.StudentId);
            });

            List<CompletionMeasureModel> spanishFinished = measureList.FindAll(r => r.Status == CpallsStatus.Finished); // 处理 Spanish 完成状态的
            measureList.RemoveAll(r => r.Status == CpallsStatus.Finished); //移除 Spanish 完成状态的
            spanishFinished.ForEach(r => r.MeasureId = r.RelatedMeasureId);
            list.AddRange(spanishFinished);

            //处理Locked状态的 ,如果有相应的 RelateMeasure，那么判断是完成，还是lock，否则忽略
            foreach (CompletionMeasureModel measure in measureList.FindAll(r => r.Language == AssessmentLanguage.English))
            {
                CompletionMeasureModel tmpMeasure = measureList.Find(r => r.StudentId == measure.StudentId
                    && r.MeasureId == measure.RelatedMeasureId);
                if (tmpMeasure != null)
                {
                    list.Add(measure);
                }
            }

            foreach (var item in titleArray.Where(r => r.RelatedId > 0 && r.Children != null && r.Children.Any()))
            {//如果一个子measure 没有 Related ，并且是初始化 状态。那么判断他的父Measure 有没有Related，如果有，且对应的 Measure 的Totale.Goal >-1 那么该Measure 为Locked

                foreach (var Children in item.Children.Where(r => r.RelatedId == 0))//需要进行判断的 子 Measure
                {
                    //需要处理的学生
                    List<int> studentIdList = pendingList.Where(r => r.MeasureId == item.RelatedId && r.Goal > -1).Select(r => r.StudentId).Distinct().ToList();

                    ///已处理过的学生
                    List<int> processedStudentIds = pendingList.Where(r => r.MeasureId == Children.MeasureId && r.Status > CpallsStatus.Finished)
                        .Select(r => r.StudentId).Distinct().ToList();

                    //未处理的学生
                    List<int> otherStudentIds = studentIdList.Except(processedStudentIds).ToList();

                    //是否需要标记为锁定
                    foreach (int tmpStudentId in otherStudentIds)
                    {
                        CompletionMeasureModel tmpModel = new CompletionMeasureModel();
                        tmpModel.MeasureId = Children.MeasureId;
                        tmpModel.Status = CpallsStatus.Locked;
                        tmpModel.Wave = wave;
                        tmpModel.StudentId = tmpStudentId;
                        list.Add(tmpModel);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 返回2014,2015……
        /// </summary>
        /// <returns></returns>
        public List<int> GetSchoolYear()
        {
            List<int> schoolYear = new List<int>();
            DateTime dt = new DateTime(2014, 8, 1);
            schoolYear.Add(dt.Year);
            for (int i = 0; i < 1000; i++)
            {
                dt = dt.AddYears(1);
                if (dt < DateTime.Now)
                    schoolYear.Add(dt.Year);
                else
                    break;
            }
            return schoolYear;
        }

        /// <summary>
        /// 获取名称并排序
        /// </summary>
        /// <param name="waves"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        private ReportModel[] SortMeasure(List<int> measureIds, Wave wave
            , AssessmentEntity assessmentEntity, AssessmentEntity otherAssessmentEnity, StudentAssessmentLanguage studentLanguage)
        {
            List<ReportModel> communityReport = new List<ReportModel>();

            List<ReportModel> englishList = new List<ReportModel>();
            List<ReportModel> spanishList = new List<ReportModel>();
            if (studentLanguage == StudentAssessmentLanguage.Bilingual)
            {
                var language = assessmentEntity.Language;

                int otherAssessmentId = 0;
                if (otherAssessmentEnity != null)
                {
                    otherAssessmentId = otherAssessmentEnity.ID;
                    if (language == AssessmentLanguage.English)
                    {
                        englishList = _adeBusiness.GetReportModel(o => o.AssessmentId == assessmentEntity.ID);
                        spanishList = _adeBusiness.GetReportModel(o => o.AssessmentId == otherAssessmentId);
                    }
                    else
                    {
                        englishList = _adeBusiness.GetReportModel(o => o.AssessmentId == otherAssessmentId);
                        spanishList = _adeBusiness.GetReportModel(o => o.AssessmentId == assessmentEntity.ID);
                    }
                }
                else
                {
                    if (language == AssessmentLanguage.English)
                        englishList = _adeBusiness.GetReportModel(o => o.AssessmentId == assessmentEntity.ID);
                    else
                        spanishList = _adeBusiness.GetReportModel(o => o.AssessmentId == assessmentEntity.ID);
                }
            }
            else
            {
                englishList = _adeBusiness.GetReportModel(o => o.AssessmentId == assessmentEntity.ID);

                foreach (int tmpMeasureId in measureIds)
                {
                    var measure = englishList.Find(r => r.MeasureId == tmpMeasureId);
                    measure.Language = studentLanguage == StudentAssessmentLanguage.English ? AssessmentLanguage.English : AssessmentLanguage.Spanish;

                    if (measure.ParentId == 1)
                    {
                        var tmpParentMeasure = communityReport.FirstOrDefault(o => o.MeasureId == tmpMeasureId);
                        if (tmpParentMeasure == null)
                            communityReport.Add(measure);
                    }
                    else
                    {
                        ///判断有没有父Measure 
                        var measureModelParent = communityReport.FirstOrDefault(o => o.MeasureId == measure.ParentId);
                        if (measureModelParent == null)
                        {
                            measureModelParent = englishList.Find(r => r.MeasureId == measure.ParentId);
                            measureModelParent.Children = new List<ReportModel>();
                            communityReport.Add(measureModelParent);
                        }
                        measureModelParent.Children.Add(measure);
                    }
                }
                return communityReport.Where(o => o.ApplyToWave.Contains(wave.ToDescription())).OrderBy(o => o.Sort).ToArray();
            }


            foreach (int tmpMeasureId in measureIds)
            {
                var measure = englishList.Find(r => r.MeasureId == tmpMeasureId);


                if (measure == null)
                {
                    measure = spanishList.Find(r => r.MeasureId == tmpMeasureId);
                    measure.Language = AssessmentLanguage.Spanish;
                }
                else measure.Language = AssessmentLanguage.English;


                if (measure.Language == AssessmentLanguage.English)
                {
                    if (measure.RelatedId > 0)
                    {
                        var tmpRelate = spanishList.Find(r => r.MeasureId == measure.RelatedId);
                        if (tmpRelate != null)
                            measure.RelatedName = tmpRelate.MeasureName;
                        else
                            measure.RelatedName = "Only in English";
                    }
                    else
                        measure.RelatedName = "Only in English";
                }
                else
                {
                    measure.RelatedName = "Only in Spanish";
                }

                if (measure.ParentId == 1)
                {
                    var tmpParentMeasure = communityReport.FirstOrDefault(o => o.MeasureId == tmpMeasureId);
                    if (tmpParentMeasure == null)
                        communityReport.Add(measure);
                }
                else
                {
                    ///判断有没有父Measure 
                    var measureModelParent = communityReport.FirstOrDefault(o => o.MeasureId == measure.ParentId);
                    if (measureModelParent == null)
                    {
                        if (measure.Language == AssessmentLanguage.English)
                        {
                            measureModelParent = englishList.Find(r => r.MeasureId == measure.ParentId);
                            var tmpRelate = spanishList.Find(r => r.RelatedId == measure.ParentId);
                            if (tmpRelate != null)
                                measureModelParent.RelatedName = tmpRelate.MeasureName;
                            else
                                measureModelParent.RelatedName = "Only in English";
                        }
                        else
                        {
                            measureModelParent = spanishList.Find(r => r.MeasureId == measure.ParentId);
                            measureModelParent.RelatedName = "Only in Spanish";
                        }
                        measureModelParent.Children = new List<ReportModel>();
                        communityReport.Add(measureModelParent);
                    }
                    measureModelParent.Children.Add(measure);
                }
            }

            return communityReport.Where(o => o.ApplyToWave.Contains(wave.ToDescription())).OrderBy(o => o.Sort).ToArray();
        }


        public Dictionary<Wave, ReportModel[]> GetDictionary(List<CompletionMeasureModel> completionMeasure, ReportModel[] titleArray,
           int studentCount, List<int> measureIds, Wave wave, int assessmentId, StudentAssessmentLanguage studentLanguage)
        {
            Dictionary<Wave, ReportModel[]> dictionary = new Dictionary<Wave, ReportModel[]>();

            if (titleArray != null)
            {
                var groups = completionMeasure.GroupBy(r => new { r.MeasureId, r.Status }).Select(r => new { r.Key.MeasureId, r.Key.Status, Num = r.Count() }).ToList();

                foreach (var measure in titleArray)
                {
                    if (measure.Children != null)
                    {
                        foreach (var childMeasure in measure.Children)
                        {
                            var finished = groups.Find(r => r.Status == CpallsStatus.Finished && r.MeasureId == childMeasure.MeasureId);
                            var locked = groups.Find(r => r.Status == CpallsStatus.Locked && r.MeasureId == childMeasure.MeasureId);

                            childMeasure.Completion = finished == null ? 0 : finished.Num; //completionCount;
                            childMeasure.Exclude = locked == null ? 0 : locked.Num; // excludeCount;
                            childMeasure.Incompletion = studentCount - childMeasure.Completion - childMeasure.Exclude;

                            int validStudentTotal = studentCount - childMeasure.Exclude;

                            if (childMeasure.Completion != 0 && studentCount != 0)
                                //12.5四舍五入为13,下面另种方法
                                //childMeasure.CompletionPercent = Convert.ToDouble((completionCount / (decimal)validStudentTotal * 100)
                                //    .ToString("F0"));
                                childMeasure.CompletionPercent = Math.Round(childMeasure.Completion / (decimal)validStudentTotal * 100,
                                        0, MidpointRounding.AwayFromZero);
                            else
                                childMeasure.CompletionPercent = 0;
                        }
                    }
                    else
                    {
                        var finished = groups.Find(r => r.Status == CpallsStatus.Finished && r.MeasureId == measure.MeasureId);
                        var locked = groups.Find(r => r.Status == CpallsStatus.Locked && r.MeasureId == measure.MeasureId);

                        measure.Completion = finished == null ? 0 : finished.Num; //completionCount;
                        measure.Exclude = locked == null ? 0 : locked.Num; // excludeCount;
                        measure.Incompletion = studentCount - measure.Completion - measure.Exclude;

                        int validStudentTotal = studentCount - measure.Exclude;

                        if (measure.Completion != 0 && validStudentTotal != 0)
                            measure.CompletionPercent = Math.Round((measure.Completion / (decimal)validStudentTotal * 100), 0,
                                MidpointRounding.AwayFromZero);
                        else
                            measure.CompletionPercent = 0;
                    }
                }
            }
            dictionary.Add(wave, titleArray);
            return dictionary;
        }

        #region School View
        public ReportList GetReport_Community(int assessmentId, int year, List<int> measureIds, Wave wave, StudentAssessmentLanguage language, int communityId
            , DateTime startDate, DateTime endDate, DateTime dobStartDate, DateTime dobEndDate)
        {
            string schoolYear = year.ToSchoolYearString();
            AssessmentEntity tmpAssessmentEntity = AdeBusiness.GetAssessment(assessmentId);
            AssessmentEntity otherAssessmentEntity = null;
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                otherAssessmentEntity = AdeBusiness.GetTheOtherLanguageAssessment(assessmentId);
                if (otherAssessmentEntity != null)
                {//双语的另一个 Assessment 不是活动的 ,就变为单语言的报表
                    if (otherAssessmentEntity.Status != EntityStatus.Active)
                    {
                        if (otherAssessmentEntity.Language == AssessmentLanguage.Spanish)
                            language = StudentAssessmentLanguage.English;
                        else
                            language = StudentAssessmentLanguage.Spanish;
                        otherAssessmentEntity = null;
                    }
                }
            }
            else
            {
                AssessmentLanguage assessmentLanguage = tmpAssessmentEntity.Language;
                //如果被选语言与assessmentId语言不一样，改变assessmentId的值
                if ((byte)assessmentLanguage != (byte)language)
                    tmpAssessmentEntity = AdeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            }

            ReportModel[] titleArray = SortMeasure(measureIds, wave, tmpAssessmentEntity, otherAssessmentEntity, language);


            List<CompletionMeasureModel> completionMeasure = null;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(communityId, assessmentId);
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                completionMeasure = GetCompletion_Bilingual(assessmentId, otherAssessmentEntity == null ? 0 : otherAssessmentEntity.ID
                    , schoolYear, measureIds, wave, titleArray, startDate, endDate, dobStartDate, dobEndDate, classIds, communityId);

            }
            else
                completionMeasure = _cpallsContract.GetCompletionStudentMeasure(communityId, 0, tmpAssessmentEntity.ID, wave, measureIds, schoolYear, startDate, endDate, dobStartDate, dobEndDate, language, classIds);


            int studentTotal = StudentBusiness.GetStudentCount(communityId, language, classIds, dobStartDate, dobEndDate);
            string name = CommunityBusiness.GetCommunity(communityId).Name;

            var dictionary = GetDictionary(completionMeasure, titleArray, studentTotal, measureIds, wave, tmpAssessmentEntity.ID, language);


            var reportList = new ReportList();
            reportList.Name = name;
            reportList.Num = studentTotal;
            reportList.ModelList = dictionary;
            return reportList;
        }
        #endregion

        #region Class View
        public ReportList GetReport_School(int assessmentId, string schoolYear, List<int> measureIds, Wave wave,
            StudentAssessmentLanguage language, int schoolId, UserBaseEntity userBase, DateTime startDate, DateTime endDate, DateTime dobStart, DateTime dobEnd)
        {
            AssessmentEntity tmpAssessmentEntity = _adeBusiness.GetAssessment(assessmentId);
            AssessmentEntity otherAssessmentEntity = null;
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                otherAssessmentEntity = AdeBusiness.GetTheOtherLanguageAssessment(assessmentId);
                if (otherAssessmentEntity != null)
                {//双语的另一个 Assessment 不是活动的 ,就变为单语言的报表
                    if (otherAssessmentEntity.Status != EntityStatus.Active)
                    {
                        if (otherAssessmentEntity.Language == AssessmentLanguage.Spanish)
                            language = StudentAssessmentLanguage.English;
                        else
                            language = StudentAssessmentLanguage.Spanish;
                        otherAssessmentEntity = null;
                    }
                }
                else
                {
                    if (tmpAssessmentEntity.Language == AssessmentLanguage.Spanish)
                        language = StudentAssessmentLanguage.Spanish;
                    else
                        language = StudentAssessmentLanguage.English;
                }
            }
            else
            {
                AssessmentLanguage assessmentLanguage = tmpAssessmentEntity.Language;
                //如果被选语言与assessmentId语言不一样，改变assessmentId的值
                if ((byte)assessmentLanguage != (byte)language)
                    tmpAssessmentEntity = AdeBusiness.GetTheOtherLanguageAssessment(assessmentId);
            }

            ReportModel[] titleArray = SortMeasure(measureIds, wave, tmpAssessmentEntity, otherAssessmentEntity, language);

            //传入measureIdList，就不用把CpallsStatus为Finished和Locked的所有StudentMeasure拿到本地
            List<CompletionMeasureModel> completionMeasure = null;
            IList<int> classIds = ClassBusiness.GetClassIdsForReport(schoolId, userBase, assessmentId);
            if (language == StudentAssessmentLanguage.Bilingual && otherAssessmentEntity != null)
            {
                completionMeasure = GetCompletion_Bilingual(assessmentId, otherAssessmentEntity.ID, schoolYear, measureIds, wave, titleArray, startDate, endDate, dobStart, dobEnd, classIds, 0, schoolId);
            }
            else
                completionMeasure = _cpallsContract.GetCompletionStudentMeasure(0, schoolId, tmpAssessmentEntity.ID, wave, measureIds, schoolYear, startDate, endDate, dobStart, dobEnd, language, classIds);




            int studentTotal = StudentBusiness.GetStudentCountBySchoolId(schoolId, classIds, language, dobStart, dobEnd);

            string name = SchoolBusiness.GetSchoolEntity(schoolId, userBase).SchoolName;
            var dictionary = GetDictionary(completionMeasure, titleArray, studentTotal, measureIds, wave, assessmentId, language);

            var reportList = new ReportList();
            reportList.Name = name;
            reportList.Num = studentTotal;
            reportList.ModelList = dictionary;
            return reportList;
        }
        #endregion

        #region Student View

        public ReportList GetReport_Class(int assessmentId, string schoolYear, int classId, List<int> measureIds, Wave wave,
            StudentAssessmentLanguage language, DateTime startDate, DateTime endDate,
            DateTime dobStart, DateTime dobEnd, ref string teacherName)
        {
            AssessmentEntity tmpAssessmentEntity = _adeBusiness.GetAssessment(assessmentId);
            AssessmentEntity otherAssessmentEntity = null;
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                otherAssessmentEntity = AdeBusiness.GetTheOtherLanguageAssessment(assessmentId);
                if (otherAssessmentEntity != null)
                {//双语的另一个 Assessment 不是活动的 ,就变为单语言的报表
                    if (otherAssessmentEntity.Status != EntityStatus.Active)
                    {
                        if (otherAssessmentEntity.Language == AssessmentLanguage.Spanish)
                            language = StudentAssessmentLanguage.English;
                        else
                            language = StudentAssessmentLanguage.Spanish;
                        otherAssessmentEntity = null;
                    }
                }
            }
            else
            {
                AssessmentLanguage assessmentLanguage = tmpAssessmentEntity.Language;
                //如果被选语言与assessmentId语言不一样，改变assessmentId的值
                if ((byte)assessmentLanguage != (byte)language)
                {
                    tmpAssessmentEntity = AdeBusiness.GetTheOtherLanguageAssessment(assessmentId);
                }
            }

            ReportModel[] titleArray = SortMeasure(measureIds, wave, tmpAssessmentEntity, otherAssessmentEntity, language);

            List<int> studentIds = new List<int>();
            List<CompletionMeasureModel> completionMeasure = new List<CompletionMeasureModel>();
            int studentTotal = 0;
            if (language == StudentAssessmentLanguage.Bilingual)
            {
                studentIds = StudentBusiness.GetStudentIdsByDOB(classId, StudentAssessmentLanguage.Bilingual, dobStart, dobEnd);
                studentTotal = studentIds.Count();

                if (otherAssessmentEntity != null)
                {
                    ///双语的学生统计
                    completionMeasure = GetCompletionForClass_Bilingual(assessmentId, otherAssessmentEntity.ID, language,
                        schoolYear, measureIds, wave, classId, titleArray, studentIds, startDate, endDate);
                }

                ///只有英语的学生
                List<int> englishStudentIds = StudentBusiness.GetStudentIdsByClassId(classId, StudentAssessmentLanguage.English);

                ///只有西班牙语的学生
                List<int> spanishStudentIds = StudentBusiness.GetStudentIdsByClassId(classId, StudentAssessmentLanguage.Spanish);
                spanishStudentIds.AddRange(englishStudentIds);

                if (spanishStudentIds.Count > 0)
                {
                    List<CompletionMeasureModel> otherCompletionMeasure = GetCompletionBilingualForClass(
                        tmpAssessmentEntity.ID, otherAssessmentEntity == null ? 0 : otherAssessmentEntity.ID
                        , language, schoolYear, measureIds, wave, classId, spanishStudentIds, startDate, endDate);

                    foreach (var v in titleArray)
                    {
                        if (v.Children != null && v.Children.Any())
                        {
                            foreach (var c in v.Children)
                                otherCompletionMeasure.Where(r => r.RelatedMeasureId == c.MeasureId).ForEach(r => r.MeasureId = c.MeasureId);
                        }
                        else
                            otherCompletionMeasure.Where(r => r.RelatedMeasureId == v.MeasureId).ForEach(r => r.MeasureId = v.MeasureId);
                    }

                    completionMeasure.AddRange(otherCompletionMeasure);
                    studentTotal += spanishStudentIds.Count;
                }
            }
            else
            {
                studentIds = StudentBusiness.GetStudentIdListByClassIdList(classId, language, dobStart, dobEnd);

                completionMeasure = GetCompletionForClass(tmpAssessmentEntity.ID, language, schoolYear, measureIds, wave, classId, studentIds, startDate, endDate);
                studentTotal = studentIds.Count();
            }

            string name = ClassBusiness.GetClass(classId).Name;


            List<string> teacherNameList = new List<string>();
            ClassBusiness.GetClass(classId).Teachers.Where(e => e.UserInfo.IsDeleted == false).ForEach(o => teacherNameList.Add(o.UserInfo.FirstName + " " + o.UserInfo.LastName));
            teacherName = string.Join("; ", teacherNameList);

            var dictionary = GetDictionary(completionMeasure, titleArray, studentTotal, measureIds, wave, assessmentId, language);

            var reportList = new ReportList();
            reportList.Name = name;
            reportList.Num = studentTotal;
            reportList.ModelList = dictionary;
            return reportList;
        }

        #endregion
    }

    public class MeasureModel
    {
        public Wave Wave { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public CpallsStatus Status { get; set; }
        public int Sort { get; set; }
        public int ParentId { get; set; }
        public int ParentRelationId { get; set; }
        public List<MeasureModel> Children { get; set; }

        #region 下面4个字段，Completion Bilingual
        public int RelatedMeasureId { get; set; }
        public int StudentId { get; set; }
        public AssessmentLanguage Language { get; set; }
        public string RelatedName { get; set; }
        #endregion

        #region 下面3个字段，会在Comparison报表时用到
        public string SchoolYear { get; set; }
        /// <summary>
        /// 该StudentMeasure得分
        /// </summary>
        public decimal Goal { get; set; }
        /// <summary>
        /// 是否计分
        /// </summary>
        public bool TotalScored { get; set; }
        #endregion
    }

    public class ReportModel
    {
        /// <summary>
        /// 1,2,3
        /// </summary>
        public string ApplyToWave { get; set; }
        public int MeasureId { get; set; }
        public string MeasureName { get; set; }
        public int ParentId { get; set; }
        public int Sort { get; set; }
        public List<ReportModel> Children { get; set; }
        public AssessmentLanguage Language { get; set; }
        /// <summary>
        /// 是否计分
        /// </summary>
        public bool TotalScored { get; set; }

        //For Pdf
        public int Completion { get; set; }
        public int Incompletion { get; set; }
        public decimal CompletionPercent { get; set; }
        public int Exclude { get; set; }
        public string RelatedName { get; set; }
        public int RelatedId { get; set; }
        //For Comparison Pdf
        public List<string> SchoolYear { get; set; }
        public List<string> Average { get; set; }
    }

    public class ReportList
    {
        /// <summary>
        /// 选中Wave下面的完成情况
        /// </summary>
        public Dictionary<Wave, ReportModel[]> ModelList { get; set; }
        /// <summary>
        /// 学生数量
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// Community或School名称
        /// </summary>
        public string Name { get; set; }
    }
}

