using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using StructureMap;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Schools;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Common.Enums;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Business.Cpalls.Group;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.PDF;
using System.Text;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Practices;
using Sunnet.Cli.Business.Practices.Models;
using Sunnet.Cli.Core.Practices.Entites;
using Sunnet.Cli.Practice.Controllers;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.CAC.Entities;

namespace Sunnet.Cli.Practice.Areas.Cpalls.Controllers
{
    public class GroupController : BaseController
    {

        AdeBusiness _adeBusiness;
        private PracticeBusiness _practiceBusiness;
        CACBusiness _cacBus;
        public GroupController()
        {
            _cacBus = new CACBusiness();
            _adeBusiness = new AdeBusiness();
            _practiceBusiness = new PracticeBusiness(PracticeUnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Index(int assessmentId, int year, int wave, AssessmentLanguage language)
        {
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;

            if ((int)UserInfo.Role > (int)Role.Teacher)
                return new RedirectResult("/error/nonauthorized");

            if (year == 0)
            {
                year = CommonAgent.Year;
            }
            if (wave == 0)
                wave = (int)CommonAgent.CurrentWave;
            ViewBag.Year = year.ToSchoolYearString();
            ViewBag.Wave = (Wave)wave;
            ViewBag.WaveString = ((Wave)wave).ToDescription();
            ViewBag.Language = language;

            int total = 0;
            IList<DemoStudentModel> list = new List<DemoStudentModel>();

            Expression<Func<DemoStudentEntity, bool>> studentCondition = PredicateHelper.True<DemoStudentEntity>();
            studentCondition = studentCondition.And(r => r.AssessmentId == assessmentId);
            studentCondition = studentCondition.And(r => r.Status == EntityStatus.Active);
            studentCondition = studentCondition.And(r => r.AssessmentLanguage == (StudentAssessmentLanguage)((byte)language)
                                                         || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);

            list = _practiceBusiness.GetStudentList(UserInfo, assessmentId, studentCondition, (Wave)wave, year, "StudentName", "asc", 0, 1000, out total);

            List<MeasureHeaderModel> MeasureList;
            List<MeasureHeaderModel> ParentMeasureList;
            List<GroupDataModel> listData = new List<GroupDataModel>();
            List<GroupDataModel> listParentData = new List<GroupDataModel>();

            _practiceBusiness.BuilderHeader(assessmentId, year, (Wave)wave, UserInfo.ID, out MeasureList, out ParentMeasureList);

            var benchmarks = _adeBusiness.GetDicBenchmarks(assessmentId);

            if (MeasureList != null && MeasureList.Count > 0)
            {
                List<int> tmpParentMeasureId = ParentMeasureList.FindAll(r => r.Subs > 0).Select(r => r.MeasureId).ToList();
                List<int> tmpParentMeasureNoSubId = ParentMeasureList.Where(r => r.Subs == 0).Select(r => r.MeasureId).ToList();

                foreach (DemoStudentModel student in list)
                {
                    //循环子Measure和没有子Measure的父Measure
                    List<DemoStudentMeasureRecordModel> stuMeasureRecordList = student.MeasureList.FindAll(r =>
                        r.ShowOnGroup == true && r.TotalScored && r.Status == CpallsStatus.Finished
                        && r.Goal >= 0 && r.BenchmarkId > 0
                        && r.Goal <= r.HigherScore && r.Goal >= r.LowerScore);
                    foreach (DemoStudentMeasureRecordModel recordModel in stuMeasureRecordList)
                    {
                        if (recordModel.GroupByParentMeasure
                            && !tmpParentMeasureId.Contains(recordModel.MeasureId)
                            && !tmpParentMeasureNoSubId.Contains(recordModel.MeasureId))
                            continue;
                        if (!benchmarks.ContainsKey(recordModel.BenchmarkId))
                            continue;

                        var tmpMeasure = listData.Find(r => r.MeasureId == recordModel.MeasureId);
                        if (tmpMeasure != null) listData.Remove(tmpMeasure);

                        var benchmark = benchmarks[recordModel.BenchmarkId];
                        listData.Add(BuilderMeasure(tmpMeasure, MeasureList, recordModel, student, benchmark));
                    }

                    /*-----------------------------------------------------------------------------------*/
                    //循环子有子Measure的父Measure
                    stuMeasureRecordList = student.MeasureList.FindAll(r =>
                        tmpParentMeasureId.Contains(r.MeasureId)
                        && r.ShowOnGroup == true && r.TotalScored
                        && r.Goal >= 0 && r.BenchmarkId > 0
                        && r.Goal <= r.HigherScore && r.Goal >= r.LowerScore);
                    foreach (DemoStudentMeasureRecordModel recordModel in stuMeasureRecordList)
                    {
                        if (!benchmarks.ContainsKey(recordModel.BenchmarkId))
                            continue;
                        var tmpMeasure = listParentData.Find(r => r.MeasureId == recordModel.MeasureId);
                        if (tmpMeasure != null) listParentData.Remove(tmpMeasure);
                        var benchmark = benchmarks[recordModel.BenchmarkId];
                        listParentData.Add(BuilderMeasure(tmpMeasure, MeasureList, recordModel, student, benchmark));
                    }
                }

                int[] tmpMeasureId = listData.Select(r => r.MeasureId).ToArray();

                List<AdeLinkEntity> measureLinks = _adeBusiness.GetLinks<MeasureEntity>((Wave)wave, tmpMeasureId);

                foreach (var v in listData)
                {
                    v.Links = new List<AdeLinkEntity>();
                    v.Links.AddRange(measureLinks.FindAll(r => r.HostId == v.MeasureId));
                }

                tmpMeasureId = listParentData.Select(r => r.MeasureId).ToArray();
                measureLinks = _adeBusiness.GetLinks<MeasureEntity>((Wave)wave, tmpMeasureId);

                foreach (var v in listParentData)
                {
                    v.Links = new List<AdeLinkEntity>();
                    v.Links.AddRange(measureLinks.FindAll(r => r.HostId == v.MeasureId));
                }
            }
            listData.AddRange(listParentData);
            foreach (var groupDataModel in listData)
            {
                PracticeMeasureGroupEntity measureGroup =
                    _practiceBusiness.GetMeasureClassGroup(groupDataModel.MeasureId, assessmentId, year, wave);
                groupDataModel.Note = measureGroup != null ? measureGroup.Note : "";
            }
            ViewBag.GroupData = JsonConvert.SerializeObject(listData.OrderBy(r => r.ParentSort).ThenBy(r => r.SubSort));
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        private GroupDataModel BuilderMeasure(GroupDataModel measure, List<MeasureHeaderModel> MeasureList, DemoStudentMeasureRecordModel recordModel, DemoStudentModel student, BenchmarkModel benchmark)
        {
            if (measure == null)
            {
                MeasureHeaderModel tmpMeasureModel = MeasureList.Find(m => m.MeasureId == recordModel.MeasureId);
                measure = new GroupDataModel()
                {
                    MeasureId = recordModel.MeasureId,
                    Name = recordModel.MeasureName,
                    ParentId = tmpMeasureModel.MeasureId == tmpMeasureModel.ParentId ? 1 : tmpMeasureModel.ParentId,
                    ParentName = tmpMeasureModel.MeasureId == tmpMeasureModel.ParentId ? string.Empty : tmpMeasureModel.ParentMeasureName,
                    //StudentList = new List<GroupStudentModel>(),
                    Sort = tmpMeasureModel.Sort,
                    Note = tmpMeasureModel.Note ?? "",
                    GroupByLabel = tmpMeasureModel.GroupByLabel,
                    DicBenchmarkList = new Dictionary<int, GroupBenchamrkModel>()
                };
                List<GroupStudentModel> studentList = new List<GroupStudentModel>();
                studentList.Add(new GroupStudentModel
                {
                    ID = recordModel.StudentId,
                    FirstName = student.StudentName,
                    LastName = "",
                    Color = recordModel.Color,
                    StudentAssessmentId = recordModel.StudentAssessmentId,
                    Goal = recordModel.Goal ?? (decimal)recordModel.Goal,
                    BenchmarkColor = recordModel.BenchmarkColor
                });
                measure.DicBenchmarkList.Add(benchmark.ID, new GroupBenchamrkModel
                    {
                        BenchmarkId = benchmark.ID,
                        LabelText = benchmark.LabelText,
                        Color = benchmark.Color,
                        LowerScore = recordModel.LowerScore,
                        StudentList = studentList
                    });
            }
            else if (!measure.DicBenchmarkList.ContainsKey(benchmark.ID))
            {
                List<GroupStudentModel> studentList = new List<GroupStudentModel>();
                studentList.Add(new GroupStudentModel
                {
                    ID = recordModel.StudentId,
                    FirstName = student.StudentName,
                    LastName = "",
                    Color = recordModel.Color,
                    StudentAssessmentId = recordModel.StudentAssessmentId,
                    Goal = recordModel.Goal ?? (decimal)recordModel.Goal,
                    BenchmarkColor = recordModel.BenchmarkColor
                });
                measure.DicBenchmarkList.Add(benchmark.ID, new GroupBenchamrkModel
                {
                    BenchmarkId = benchmark.ID,
                    LabelText = benchmark.LabelText,
                    Color = benchmark.Color,
                    LowerScore = recordModel.LowerScore,
                    StudentList = studentList
                });
            }
            else
            {
                measure.DicBenchmarkList[benchmark.ID].StudentList.Add(new GroupStudentModel
                {
                    ID = recordModel.StudentId,
                    FirstName = student.StudentName,
                    LastName = "",
                    Color = recordModel.Color,
                    StudentAssessmentId = recordModel.StudentAssessmentId,
                    Goal = recordModel.Goal ?? (decimal)recordModel.Goal,
                    BenchmarkColor = recordModel.BenchmarkColor
                });
            }
            //为了防止缓存，必须重新从数据库再取一次数据
            var findMeasure = _adeBusiness.GetMeasureModel(recordModel.MeasureId);
            if (findMeasure != null)
            {
                measure.Note = findMeasure.Note ?? "";
                if (findMeasure.ParentId > 1)
                {
                    measure.ParentSort = findMeasure.Parent.Sort;
                }
                else
                {
                    measure.ParentSort = measure.Sort;
                }
            }
            return measure;
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult AddGroup(AssessmentLanguage language, Wave wave, string year, int assessmentId)
        {
            ViewBag.Title = "Add Group";

            PracticeStudentGroupEntity entity = new PracticeStudentGroupEntity();
            entity.Language = language;
            entity.Wave = wave;
            entity.SchoolYear = year;
            entity.AssessmentId = assessmentId;

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string AddGroup(PracticeStudentGroupEntity model)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                model.StudentIds = string.Empty;
                model.UpdatedBy = UserInfo.ID;
                model.CreatedBy = UserInfo.ID;
                model.Name = model.Name.Trim();
                model.Note = "";
                bool sameName = _practiceBusiness.CheckGroupName(model);
                if (sameName)
                {
                    response.Success = false;
                    response.Message = GetInformation("SameName");
                }
                else
                {
                    OperationResult result = _practiceBusiness.AddPracticeStudentGroup(model);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Data = new GroupModel()
                    {
                        ID = model.ID,

                        Name = model.Name,
                        Note = "",
                        StudentList = new List<GroupStudentModel>()
                    };
                    response.Message = result.Message;
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult EditGroup(int groupId)
        {
            PracticeStudentGroupEntity entity = _practiceBusiness.GetPracticeStudentGroupEntity(groupId);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string EditGroup(PracticeStudentGroupEntity model)
        {
            var response = new PostFormResponse();
            if (ModelState.IsValid)
            {
                bool sameName = _practiceBusiness.CheckGroupName(model);
                if (sameName)
                {
                    response.Success = false;
                    response.Message = GetInformation("SameName");
                }
                else
                {
                    PracticeStudentGroupEntity entity = _practiceBusiness.GetPracticeStudentGroupEntity(model.ID);
                    entity.Name = model.Name;
                    OperationResult result = _practiceBusiness.UpdatePracticeStudentGroup(entity);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Data = new GroupModel() { ID = model.ID, Name = model.Name };
                    response.Message = result.Message;
                }
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult Student(int groupId, int assessmentId, AssessmentLanguage language)
        {
            ViewBag.Width = 500;
            PracticeStudentGroupEntity entity = _practiceBusiness.GetPracticeStudentGroupEntity(groupId);
            if (entity == null)
                return new RedirectResult("/Error/nonauthorized");
            ViewBag.AssessmentId = assessmentId;
            ViewBag.GroupId = groupId;
            ViewBag.Language = language;
            List<GroupStudentModel> list = _practiceBusiness.GetGroupStudentList(assessmentId, language);
            if (entity.StudentIds != string.Empty)
            {
                string[] ids = entity.StudentIds.Split(',');
                list.FindAll(r => ids.Contains(r.ID.ToString())).ForEach(r => r.Seleted = true);
            }
            ViewBag.List = list;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string AddStudent(string[] student_select, int assessmentId, int groupId, AssessmentLanguage language)
        {
            var response = new PostFormResponse();
            PracticeStudentGroupEntity entity = _practiceBusiness.GetPracticeStudentGroupEntity(groupId);
            IList<MyActivityEntity> activityList = _cacBus.GetMyActivities(UserInfo.ID);
            entity.StudentIds = string.Join(",", student_select);

            OperationResult result = _practiceBusiness.UpdatePracticeStudentGroup(entity);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            if (response.Success)
            {
                GroupModel model = new GroupModel()
                {
                    ID = entity.ID,
                    Name = entity.Name,
                    Note = entity.Note,
                    PracticeGroupActivities = entity.Activities,
                    StudentList = new List<GroupStudentModel>()
                };
                if (model.PracticeGroupActivities != null && model.PracticeGroupActivities.Count > 0)
                {
                    var myActivityIds = model.PracticeGroupActivities.Select(c => c.MyActivityId).ToList();
                    model.MyActivityList = activityList.Where(c => myActivityIds.Contains(c.ID)).ToList();
                }
                else
                {
                    model.MyActivityList = new List<MyActivityEntity>();
                }
                List<GroupStudentModel> list = _practiceBusiness.GetGroupStudentList(assessmentId, language);
                if (entity.StudentIds != string.Empty)
                {
                    string[] ids = entity.StudentIds.Split(',');
                    model.StudentList = list.FindAll(r => ids.Contains(r.ID.ToString()));
                }
                response.Data = model;

            }
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string MoveToGroup(int assessmentId, int studentId, int groupId, AssessmentLanguage language)
        {
            var response = new PostFormResponse();
            IList<MyActivityEntity> activityList = _cacBus.GetMyActivities(UserInfo.ID);
            if (groupId > 0)
            {
                PracticeStudentGroupEntity entity = _practiceBusiness.GetPracticeStudentGroupEntity(groupId);
                if (entity != null)
                {
                    response.Success = true;

                    string[] ids = entity.StudentIds.Split(',');
                    if (ids.Contains(studentId.ToString()) == false) //不包含新增加的 学生
                    {
                        if (entity.StudentIds != string.Empty)
                            entity.StudentIds += "," + studentId.ToString();
                        else
                            entity.StudentIds = studentId.ToString();

                        OperationResult result = _practiceBusiness.UpdatePracticeStudentGroup(entity);
                        response.Success = result.ResultType == OperationResultType.Success;
                        response.Message = result.Message;
                    }
                    if (response.Success)
                    {
                        GroupModel model = new GroupModel()
                        {
                            ID = entity.ID,
                            Name = entity.Name,
                            Note = entity.Note ?? entity.Note,
                            PracticeGroupActivities = entity.Activities,
                            StudentList = new List<GroupStudentModel>()
                        };
                        if (model.PracticeGroupActivities != null && model.PracticeGroupActivities.Count > 0)
                        {
                            var myActivityIds = model.PracticeGroupActivities.Select(c => c.MyActivityId).ToList();
                            model.MyActivityList = activityList.Where(c => myActivityIds.Contains(c.ID)).ToList();
                        }
                        else
                        {
                            model.MyActivityList = new List<MyActivityEntity>();
                        }
                        List<GroupStudentModel> list = _practiceBusiness.GetGroupStudentList(assessmentId, language);
                        if (entity.StudentIds != string.Empty)
                        {
                            ids = entity.StudentIds.Split(',');
                            model.StudentList = list.FindAll(r => ids.Contains(r.ID.ToString()));
                        }
                        response.Data = model;
                    }

                    return JsonHelper.SerializeObject(response);

                }
            }
            response.Success = false;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string DeleteGroup(int id)
        {
            var response = new PostFormResponse();
            OperationResult result = _practiceBusiness.DeletePracticeStudentGroup(id);
            response.Success = result.ResultType == OperationResultType.Success;
            response.Message = result.Message;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string DeleteGroupStudent(int groupId, int studentId)
        {
            var response = new PostFormResponse();

            PracticeStudentGroupEntity entity = _practiceBusiness.GetPracticeStudentGroupEntity(groupId);
            if (entity != null)
            {
                response.Success = true;
                string[] ids = entity.StudentIds.Split(',');
                if (ids.Contains(studentId.ToString()))
                {
                    List<string> newIds = new List<string>();
                    foreach (string s in ids)
                    {
                        if (s != studentId.ToString())
                            newIds.Add(s);
                    }
                    entity.StudentIds = string.Join(",", newIds);
                    OperationResult result = _practiceBusiness.UpdatePracticeStudentGroup(entity);
                    response.Success = result.ResultType == OperationResultType.Success;
                    response.Message = result.Message;
                }
            }
            else
            {
                response.Message = "object is null";
            }
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult MeasureGroupPdf(int assessmentId, int year, int wave, AssessmentLanguage language, bool export = false)
        {
            if ((int)UserInfo.Role > (int)Role.Teacher)
                return new RedirectResult("/error/nonauthorized");
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;
            //验证class

            ViewBag.ShowSchoolback = true;

            if (UserInfo.Role == Role.Teacher)
            {
                ViewBag.ShowSchoolback = false;
            }


            if (year == 0)
            {
                year = CommonAgent.Year;
            }

            if (wave == 0)
                wave = (int)CommonAgent.CurrentWave;

            ViewBag.Year = year.ToSchoolYearString();
            ViewBag.Wave = (Wave)wave;
            ViewBag.WaveString = ((Wave)wave).ToDescription();
            ViewBag.Language = language;


            int total = 0;
            IList<DemoStudentModel> list = new List<DemoStudentModel>();

            string schoolYear = year.ToSchoolYearString();

            Expression<Func<DemoStudentEntity, bool>> studentCondition = PredicateHelper.True<DemoStudentEntity>();
            studentCondition = studentCondition.And(r => r.Status == EntityStatus.Active);
            studentCondition = studentCondition.And(r => r.AssessmentLanguage == (StudentAssessmentLanguage)((byte)language)
                                                         || r.AssessmentLanguage == StudentAssessmentLanguage.Bilingual);

            list = _practiceBusiness.GetStudentList(UserInfo, assessmentId, studentCondition, (Wave)wave, year, "StudentName", "asc", 0, 1000, out total);

            List<MeasureHeaderModel> MeasureList;
            List<MeasureHeaderModel> ParentMeasureList;

            _practiceBusiness.BuilderHeader(assessmentId
                , year, (Wave)wave,UserInfo.ID
                , out MeasureList, out ParentMeasureList);


            List<GroupDataModel> listData = new List<GroupDataModel>();
            List<GroupDataModel> listParentData = new List<GroupDataModel>();
            var benchmarks = _adeBusiness.GetDicBenchmarks(assessmentId);

            if (MeasureList != null && MeasureList.Count > 0)
            {
                List<int> tmpParentMeasureId = ParentMeasureList.FindAll(r => r.Subs > 0).Select(r => r.MeasureId).ToList();
                List<int> tmpParentMeasureNoSubId = ParentMeasureList.Where(r => r.Subs == 0).Select(r => r.MeasureId).ToList();

                foreach (DemoStudentModel student in list)
                {
                    //循环子Measure和没有子Measure的父Measure
                    List<DemoStudentMeasureRecordModel> stuMeasureRecordList = student.MeasureList.FindAll(r =>
                       r.ShowOnGroup == true && r.TotalScored && r.Status == CpallsStatus.Finished
                       && r.Goal >= 0 && r.BenchmarkId > 0
                       && r.Goal <= r.HigherScore && r.Goal >= r.LowerScore);

                    foreach (DemoStudentMeasureRecordModel recordModel in stuMeasureRecordList)
                    {
                        if (recordModel.GroupByParentMeasure
                            && !tmpParentMeasureId.Contains(recordModel.MeasureId)
                            && !tmpParentMeasureNoSubId.Contains(recordModel.MeasureId))
                            continue;
                        if (!benchmarks.ContainsKey(recordModel.BenchmarkId))
                            continue;

                        var tmpMeasure = listData.Find(r => r.MeasureId == recordModel.MeasureId);
                        if (tmpMeasure != null) listData.Remove(tmpMeasure);
                        var benchmark = benchmarks[recordModel.BenchmarkId];
                        listData.Add(BuilderMeasure(tmpMeasure, MeasureList, recordModel, student, benchmark));
                    }

                    //循环子有子Measure的父Measure
                    stuMeasureRecordList = student.MeasureList.FindAll(r =>
                       tmpParentMeasureId.Contains(r.MeasureId)
                       && r.ShowOnGroup == true && r.TotalScored
                       && r.Goal >= 0 && r.BenchmarkId > 0
                       && r.Goal <= r.HigherScore && r.Goal >= r.LowerScore);

                    foreach (DemoStudentMeasureRecordModel recordModel in stuMeasureRecordList)
                    {
                        if (!benchmarks.ContainsKey(recordModel.BenchmarkId))
                            continue;
                        var tmpMeasure = listParentData.Find(r => r.MeasureId == recordModel.MeasureId);
                        if (tmpMeasure != null) listParentData.Remove(tmpMeasure);
                        var benchmark = benchmarks[recordModel.BenchmarkId];
                        listParentData.Add(BuilderMeasure(tmpMeasure, MeasureList, recordModel, student, benchmark));
                    }
                }

                int[] tmpMeasureId = listData.Select(r => r.MeasureId).ToArray();

                List<AdeLinkEntity> measureLinks = _adeBusiness.GetLinks<MeasureEntity>(tmpMeasureId);

                foreach (var v in listData)
                {
                    v.Links = new List<AdeLinkEntity>();
                    v.Links.AddRange(measureLinks.FindAll(r => r.HostId == v.MeasureId));
                }

                tmpMeasureId = listParentData.Select(r => r.MeasureId).ToArray();
                measureLinks = _adeBusiness.GetLinks<MeasureEntity>(tmpMeasureId);

                foreach (var v in listParentData)
                {
                    v.Links = new List<AdeLinkEntity>();
                    v.Links.AddRange(measureLinks.FindAll(r => r.HostId == v.MeasureId));
                }
            }

            listData.AddRange(listParentData);
            ViewBag.List = listData.OrderBy(r => r.ParentSort).ThenBy(r => r.SubSort).ToList();
            //ViewBag.List = assessment.GroupbyParentMeasure ? listParentData.OrderBy(r => r.Sort).ToList() : listData.OrderBy(r => r.Sort).ToList();
            ViewBag.Pdf = false;
            if (export)
            {
                ViewBag.Pdf = true;
                GetPdf(GetViewHtml("MeasureGroupPdf"), "MeasureGroup.pdf");
            }

            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult CustomGroupPdf(int assessmentId, int year, int wave, AssessmentLanguage language, bool export = false)
        {
            if ((int)UserInfo.Role > (int)Role.Teacher)
                return new RedirectResult("/error/nonauthorized");
            var assessment = _adeBusiness.GetBaseAssessmentModel(assessmentId);
            if (assessment == null) return new EmptyResult();
            ViewBag.AssessmentName = assessment.Name;

            //验证class

            ViewBag.ShowSchoolback = true;
            var teachers = new List<string>();

            if (year == 0)
            {
                year = CommonAgent.Year;
            }
            if (wave == 0)
                wave = (int)CommonAgent.CurrentWave;
            ViewBag.Year = year.ToSchoolYearString();
            ViewBag.Wave = (Wave)wave;
            ViewBag.WaveString = ((Wave)wave).ToDescription();
            ViewBag.Language = language;

            List<GroupModel> groupList = _practiceBusiness.GetCpallsStudentGroupList(UserInfo.ID,(Wave)wave, year.ToSchoolYearString(), language, assessmentId);
            ViewBag.List = groupList;
            ViewBag.Pdf = false;
            if (export)
            {
                ViewBag.Pdf = true;
                GetPdf(GetViewHtml("CustomGroupPdf"), "CustomGroup.pdf");
            }

            return View();
        }


        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult Note(int groupId, bool isMeasureGroup, int assessmentId = 0, int year = 0, int wave = 0)
        {
            ViewBag.groupId = groupId;
            ViewBag.assessmentId = assessmentId;
            ViewBag.isMeasure = isMeasureGroup.ToString();
            if (isMeasureGroup)
            {
                ViewBag.year = year;
                ViewBag.wave = wave;
                var entity = _practiceBusiness.GetMeasureClassGroup(groupId, assessmentId, year, wave);
                var model = _adeBusiness.GetMeasureModel(groupId);
                ViewBag.note = entity != null ? entity.Note : "";
                ViewBag.Name = model.Name;
            }
            else
            {
                var entity = _practiceBusiness.GetPracticeStudentGroupEntity(groupId);
                ViewBag.note = entity.Note ?? "";
                ViewBag.Name = entity.Name;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string SaveNote(int groupId, string note, bool isMeasure, int assessmentId = 0, int year = 0, int wave = 0)
        {
            var response = new PostFormResponse();
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (ModelState.IsValid)
            {
                if (isMeasure)// Measure Groups
                {
                    var entity = _practiceBusiness.GetMeasureClassGroup(groupId, assessmentId, year, wave);
                    if (entity != null)
                    {
                        entity.AssessmentId = assessmentId;
                        entity.Note = note.Trim();
                        entity.UpdatedOn = DateTime.Now;
                        entity.UpdatedBy = UserInfo.ID;
                        result = _practiceBusiness.UpdateMeasureClassGroup(entity);
                    }
                    else
                    {
                        PracticeMeasureGroupEntity measureClassGroup = new PracticeMeasureGroupEntity();
                        measureClassGroup.MeasureId = groupId;
                        measureClassGroup.AssessmentId = assessmentId;
                        measureClassGroup.Year = year;
                        measureClassGroup.Wave = wave;
                        measureClassGroup.Note = note.Trim();
                        measureClassGroup.CreatedBy = UserInfo.ID;
                        measureClassGroup.UpdatedBy = UserInfo.ID;
                        measureClassGroup.CreatedOn = DateTime.Now;
                        measureClassGroup.UpdatedOn = DateTime.Now;
                        result = _practiceBusiness.InsertMeasureClassGroup(measureClassGroup);
                    }
                }
                else //Custom Groups
                {
                    PracticeStudentGroupEntity entity = _practiceBusiness.GetPracticeStudentGroupEntity(groupId);
                    entity.Note = note.Trim();
                    result = _practiceBusiness.UpdatePracticeStudentGroup(entity);
                }
                response.Success = result.ResultType == OperationResultType.Success;
                response.Message = result.Message;
            }
            response.ModelState = ModelState;
            return JsonHelper.SerializeObject(response);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult ParentEmailActivities(string studentIds, int measureId, Wave wave, int year)
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public string SendParentEmailActivities(List<string> parentEmails, List<string> activities, List<string> activityNames, List<string> students)
        {
            var response = new PostFormResponse();
            EmailTemplete template = XmlHelper.GetEmailTemp("ParentActivities_Template.xml");
            string activitiesHtml = "";
            for (int i = 0; i < activities.Count; i++)
            {

                activitiesHtml += "<a href='" + activities[i] + "'>" + activityNames[i] + "</a>" + "<br />";
            }
            string emailParent = "";
            string emailBody = template.Body
                   .Replace("{TeacherName}", UserInfo.FirstName + " " + UserInfo.LastName)
                   .Replace("{Activities}", activitiesHtml)
                   .Replace("{MainSiteDomain}", SFConfig.MainSiteDomain)
                   .Replace("{StaticDomain}", SFConfig.StaticDomain);
            template.Subject = template.Subject.Replace("{TeacherName}",
                UserInfo.FirstName + " " + UserInfo.LastName);


            foreach (string parentEmail in students)
            {
                var temp1 = parentEmail.Split(';')[2];
                emailParent += temp1 + ";";
            }
            SendEmail(UserInfo.PrimaryEmailAddress, emailParent, template.Subject, emailBody);
            response.Success = true;

            return JsonHelper.SerializeObject(response);
        }

        public void SendEmail(string to, string subject, string emailBody)
        {
            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            new SendHandler(() => emailSender.SendMail(to, subject, emailBody)).BeginInvoke(null, null);
        }
        public void SendEmail(string to, string bcc, string subject, string emailBody)
        {
            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            new SendHandler(() => emailSender.SendMail(to, bcc, subject, emailBody)).BeginInvoke(null, null);
        }
        private delegate void SendHandler();


        private void GetPdf(string html, string fileName)
        {
            PdfProvider pdfProvider = new PdfProvider(PdfType.Assessment_Portrait);
            pdfProvider.GeneratePDF(html, fileName);
        }

        private string GetViewHtml(string viewName)
        {
            var resultHtml = string.Empty;
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            if (null == result.View)
            {
                throw new InvalidOperationException(FormatErrorMessage(viewName, result.SearchedLocations));
            }
            try
            {
                ViewContext viewContext = new ViewContext(ControllerContext, result.View, this.ViewData, this.TempData, Response.Output);
                var textWriter = new StringWriter();
                result.View.Render(viewContext, textWriter);
                resultHtml = textWriter.ToString();
            }
            finally
            {
                result.ViewEngine.ReleaseView(ControllerContext, result.View);
            }
            return resultHtml;
        }
        public string Search(Wave wave, string year, AssessmentLanguage language, int AssessmentId)
        {
            List<GroupModel> list = _practiceBusiness.GetCpallsStudentGroupList(UserInfo.ID,wave, year, language, AssessmentId);

            var result = new { data = list };
            return JsonHelper.SerializeObject(result);
        }

        private string FormatErrorMessage(string viewName, IEnumerable<string> searchedLocations)
        {
            string format = "The view '{0}' or its master was not found or no view engine supports the searched locations. The following locations were searched:{1}";
            StringBuilder builder = new StringBuilder();
            foreach (string str in searchedLocations)
            {
                builder.AppendLine();
                builder.Append(str);
            }
            return string.Format(CultureInfo.CurrentCulture, format, viewName, builder);
        }

        public ActionResult SelectActivity(int groupId)
        {
            var selectedIds = new List<int>();
            var list = _cacBus.GetMyActivities(UserInfo.ID);
            var listIds = list.Select(c => c.ID).ToList();
            var groupList = _practiceBusiness.GetPracticeGroupActivities(groupId);
            if (groupList.Count > 0)
                selectedIds = groupList.Where(c => listIds.Contains(c.MyActivityId)).Select(c => c.MyActivityId).ToList();
            ViewBag.list = list;
            ViewBag.groupList = selectedIds;
            ViewBag.UserId = UserInfo.ID;
            ViewBag.groupId = groupId;
            return View();
        }
        [HttpPost]
        public string SaveActivities(int groupId, IList<string> chkAcvitity)
        {
            var response = new PostFormResponse();
            IList<int> idList = new List<int>();
            foreach (var chk in chkAcvitity)
            {
                var id = 0;
                int.TryParse(chk, out id);
                if (id > 0)
                    idList.Add(id);
            }
            var res = _practiceBusiness.SavePracticeGroupActivities(groupId, idList, UserInfo.ID);
            response.Success = res.ResultType == OperationResultType.Success;
            response.Message = res.Message;
            return JsonHelper.SerializeObject(response);
        }
    }


}