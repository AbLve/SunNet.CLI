using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Optimization;
using StructureMap;
using Sunnet.Cli.Assessment.Controllers;
using Sunnet.Cli.Assessment.Models;
using Sunnet.Cli.Business;
using Sunnet.Cli.Business.Classes;
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Cpalls;
using Sunnet.Cli.Business.Cpalls.Models;
using Sunnet.Cli.Business.Students;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Cpalls.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.Helpers;
using Sunnet.Framework.Log;
using Sunnet.Framework.Permission;
using Sunnet.Framework.Resources;
using WebGrease.Css.Extensions;
using Newtonsoft.Json;
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.Core.Ade.Enums;
using System.Collections;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Assessment.Areas.Offline.Controllers
{
    public class IndexController : BaseController
    {

        CpallsBusiness _cpallsBusiness;
        ClassBusiness _classBusiness;
        private StudentBusiness _studentBusiness;
        private CpallsOfflineBusiness _offlineBusiness;
        private AdeBusiness _adeBusiness;

        private ISunnetLog logger;
        public IndexController()
        {
            _cpallsBusiness = new CpallsBusiness(AdeUnitWorkContext);
            _offlineBusiness = new CpallsOfflineBusiness(AdeUnitWorkContext);
            _classBusiness = new ClassBusiness();
            _studentBusiness = new StudentBusiness();
            _adeBusiness = new AdeBusiness();
            logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        private void ProcessViewBag(int schoolId, int classId, int assessmentId, int year, Wave wave, string legendUrl)
        {
            Session["AssessmentId"] = assessmentId;
            Session["SchoolId"] = schoolId;
            Session["Year"] = year;
            Session["Wave"] = wave;
            Session["ClassId"] = classId;
            Session["LegendUrl"] = legendUrl;
        }

        private object GetManifestUrl(int assessmentId = 0, int schoolId = 0, int classId = 0, int year = 0, Wave wave = Wave.BOY, string legendUrl = "")
        {
            return Url.Action("Manifest", new
            {
                assessmentId = Session["AssessmentId"] == null ? assessmentId : Session["AssessmentId"],
                schoolId = Session["SchoolId"] == null ? schoolId : Session["SchoolId"],
                year = Session["Year"] == null ? year : Session["Year"],
                wave = Session["Wave"] == null ? wave : Session["Wave"],
                classId = Session["ClassId"] == null ? classId : Session["ClassId"],
                legendUrl = Session["LegendUrl"] == null ? "" : Session["LegendUrl"]
            });
        }

        // GET: Offline/Index
        [CLIUrlAuthorize(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public ActionResult Index(int assessmentId = 0, int schoolId = 0, int classId = 0, int year = 0, Wave wave = Wave.BOY)
        {
            ViewBag.LoginUrl = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.CPALLS_OFFLINE);
            ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, LoginIASID.CPALLS_OFFLINE);
            ViewBag.Manifest = GetManifestUrl(assessmentId, schoolId, classId, year, wave);
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Offline, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Preparing(int assessmentId = 0, AssessmentLanguage language = AssessmentLanguage.English,
            int year = 0, Wave wave = Wave.BOY, int classId = 0)
        {
            if (classId < 1 || assessmentId < 1)
                return View();
            if (year < 1) year = CommonAgent.Year;
            ClassModel class1 = _classBusiness.GetClassForCpalls(classId);

            var assessment = _cpallsBusiness.GetAssessment(assessmentId, wave);
            assessment.InExecDate = (UserInfo.Role == Role.Teacher ? false : assessment.InExecDateTemp);
            //TxkeaReceptive类型并且Image Sequence为Random时，Answer类型为Selectable时需要重新排列（只需变更Image delay 和 audio delay）
            //在线每次做题时都要随机变化，所以不能从缓存中读取，以免缓存后读出的数据都相同
            var allMeaIds = assessment.Measures.Select(x => x.MeasureId).ToList();
            var baseItems = new AdeBusiness().GetItemModels(
                                i => allMeaIds.Contains(i.MeasureId) && i.IsDeleted == false
                                    && i.Status == EntityStatus.Active && i.Type == ItemType.TxkeaReceptive).ToList();

            foreach (ExecCpallsMeasureModel measure in assessment.Measures)
            {
                foreach (ExecCpallsItemModel item in measure.Items)
                {
                    if (item.Type == ItemType.TxkeaReceptive)
                    {
                        ItemModel baseItem = baseItems.Find(r => r.ID == item.ItemId);
                        if (baseItem != null && (TxkeaReceptiveItemModel)baseItem != null
                            && ((TxkeaReceptiveItemModel)baseItem).ImageSequence == OrderType.Random)
                        {
                            if (item.Answers != null && item.Answers.Count > 0
                                && item.Answers.Where(r => r.ImageType == ImageType.Selectable).Count() > 1)
                            {
                                List<AnswerEntity> RandomAnswers = new List<AnswerEntity>();//重新排列后的answer集合
                                List<AnswerEntity> OrderedAnswers = item.Answers.Select(
                                    r => new AnswerEntity() { PictureTime = r.PictureTime, AudioTime = r.AudioTime }).ToList();
                                ArrayList choosedRandomIndex = new ArrayList();

                                List<int> selectableIndex = item.Answers.Select((r, i) => new { r, i }).
                                    Where(r => r.r.ImageType == ImageType.Selectable).Select(r => r.i).ToList();
                                RandomTool.GetRandomNum(selectableIndex);//重新排列Selectable的Index

                                for (int i = 0; i < item.Answers.Count; i++)
                                {
                                    AnswerEntity TargetAnswer = item.Answers[i];
                                    //只有Selectable的才随机显示
                                    if (TargetAnswer.ImageType == ImageType.NonSelectable)
                                    {
                                        RandomAnswers.Add(TargetAnswer);
                                    }
                                    else
                                    {
                                        if (selectableIndex.Count > 0)//保证还有得拿
                                        {
                                            int randomIndexValue = selectableIndex[0];//总拿第一个的值
                                            selectableIndex.RemoveAt(0);//去掉，下次自动拿下一个
                                            item.Answers[randomIndexValue].PictureTime = OrderedAnswers[i].PictureTime;
                                            item.Answers[randomIndexValue].AudioTime = OrderedAnswers[i].AudioTime;
                                            RandomAnswers.Add(item.Answers[randomIndexValue]);
                                        }
                                    }
                                }
                                item.Answers = RandomAnswers;
                            }
                        }
                    }
                }
            }

            assessment.SchoolId = class1.School.ID;
            assessment.SchoolName = class1.School.Name;
            assessment.CommunityName = class1.School.CommunitiesText;
            assessment.SchoolYear = year.ToSchoolYearString();
            assessment.Student = new ExecCpallsStudentModel();
            assessment.Class = new ExecCpallsClassModel()
            {
                ID = classId,
                Name = class1.ClassName,
                Teachers = string.Join(", ", _classBusiness.GetTeachers(classId))
            };

            var legendUi = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.EngageUI);
            string legendUrl = legendUi != null ? SFConfig.StaticDomain + "upload/" + legendUi.ColorFilePath : "";
            assessment.LegendUIFilePath = legendUrl;
            assessment.LegendUIText = legendUi != null ? legendUi.Text : "";
            assessment.LegendUITextPosition = legendUi != null ? legendUi.TextPosition : "";
            var list = _offlineBusiness.GetStudentsAssessmentsForOffline(UserInfo, assessmentId, wave, year, classId);
            dynamic jsonObj = new
            {
                students = list.Select(sa => new
                {
                    studentId = sa.StudentId,
                    firstName = sa.FirstName,
                    lastName = sa.LastName,
                    age = sa.Age,
                    birthday = sa.BirthDate,
                    saId = sa.ID,
                    assessmentId = sa.AssessmentId,
                    changed = false,
                    measures = sa.Measure.ToDictionary(mea => mea.Key, mea => new
                    {
                        smId = mea.Value.ID,
                        saId = mea.Value.SAId,
                        measureId = mea.Value.MeasureId,
                        status = mea.Value.Status,
                        benchmark = mea.Value.Benchmark,
                        ageGroup = mea.Value.AgeGroup,
                        totalScored = mea.Value.TotalScored,
                        goal = mea.Value.Goal,
                        className = mea.Value.Color,
                        isTotal = mea.Value.IsTotal,
                        age = mea.Value.Age,
                        pauseTime = mea.Value.PauseTime,
                        createdOn = mea.Value.CreatedOn,
                        updatedOn = mea.Value.UpdatedOn,
                        changed = false,
                        comment = mea.Value.Comment,
                        lightColor = mea.Value.LightColor,
                        hasCutoffScores = mea.Value.HasCutOffScores,
                        benchmarkId = mea.Value.BenchamrkId,
                        lowerScore = mea.Value.LowerScore,
                        higherScore = mea.Value.HigherScore,
                        benchmarkColor = mea.Value.BenchmarkColor,
                        benchmarkText = mea.Value.BenchmarkText,
                        items = mea.Value.Items.Select(si => new
                        {
                            siId = si.ID,
                            itemId = si.ItemId,
                            type = si.Type,
                            status = si.Status,
                            isCorrect = si.IsCorrect,
                            selectedAnswers = si.SelectedAnswers,
                            pauseTime = si.PauseTime,
                            goal = si.Goal,
                            scored = si.Scored,
                            score = si.Score,
                            details = si.Details,
                            executed = si.Executed,
                            lastItemIndex = si.LastItemIndex,
                            resultIndex = si.ResultIndex,
                            createdOn = si.CreatedOn,
                            updatedOn = si.UpdatedOn
                        })
                    })
                }).OrderBy(x => x.lastName),
                assessment = assessment,
                onlineUrl = string.Format(
            "/Cpalls/Student?schoolId={0}&classId={1}&assessmentId={2}&year={3}&wave={4}", class1.School.ID, classId,
            assessmentId, year, (byte)wave)
            };

            ViewBag.Json = JsonHelper.SerializeObject(jsonObj);

            ViewBag.AssessmentId = assessmentId;
            ViewBag.SchoolId = class1.School.ID;
            ViewBag.ClassId = classId;
            ViewBag.Year = year;
            ViewBag.Wave = wave;

            ProcessViewBag(class1.School.ID, classId, assessmentId, year, wave, legendUrl);

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified)]
        public FileResult Manifest(int assessmentId, int schoolId, int year, Wave wave, int classId, string legendUrl)
        {
            if (assessmentId + schoolId + year + classId < 4)
                return File(new byte[] { }, "text/html");
            ExecCpallsAssessmentModel assessment = _cpallsBusiness.GetAssessmentForOffline(assessmentId, wave);
            if (assessment == null)
                return File(new byte[] { }, "text/html");

            var fileList = new List<string>();

            fileList.AddRange(assessment.Measures.Select(x => x.StartPageHtml));
            fileList.AddRange(assessment.Measures.Select(x => x.EndPageHtml));
            assessment.Measures.ForEach(mea => mea.Items.ForEach(item =>
            {
                var measureName = mea.Name;
                var itemName = item.Label;
                if (item.Props.ContainsKey("PromptAudio")) fileList.Add(item.Props["PromptAudio"] as string);
                if (item.Props.ContainsKey("PromptPicture")) fileList.Add(item.Props["PromptPicture"] as string);
                if (item.Props.ContainsKey("TargetAudio")) fileList.Add(item.Props["TargetAudio"] as string);
                if (item.Props.ContainsKey("BackgroundImage")) fileList.Add(item.Props["BackgroundImage"] as string);
                if (item.Props.ContainsKey("LayoutBackgroundImage")) fileList.Add(item.Props["LayoutBackgroundImage"] as string);
                if (item.Props.ContainsKey("ResponseBackgroundImage")) fileList.Add(item.Props["ResponseBackgroundImage"] as string);
                if (item.Props.ContainsKey("InstructionAudio")) fileList.Add(item.Props["InstructionAudio"] as string);
                if (item.Props.ContainsKey("Responses"))
                {
                    var v = JsonConvert.SerializeObject(item.Props["Responses"]);
                    List<TypedResopnseModel> responseList = JsonConvert.DeserializeObject<List<TypedResopnseModel>>(v);
                    responseList.ForEach(r => fileList.Add(r.Picture));
                }
                if (item.Props.ContainsKey("ImageList"))  // Txkea-expressive item
                {
                    var v = JsonConvert.SerializeObject(item.Props["ImageList"]);
                    List<AnswerEntity> imageList = JsonConvert.DeserializeObject<List<AnswerEntity>>(v);
                    imageList.ForEach(image =>
                        {
                            if (!string.IsNullOrEmpty(image.Audio)) fileList.Add(image.Audio);
                            if (!string.IsNullOrEmpty(image.Picture)) fileList.Add(image.Picture);
                        }
                    );
                }

                item.Answers.ForEach(answer =>
                {
                    if (!string.IsNullOrEmpty(answer.Audio)) fileList.Add(answer.Audio);
                    if (!string.IsNullOrEmpty(answer.Picture)) fileList.Add(answer.Picture);
                    if (!string.IsNullOrEmpty(answer.ResponseAudio)) fileList.Add(answer.ResponseAudio);
                });
            }));
            fileList = fileList.Select(FileHelper.GetPreviewPathofUploadFileSameDomain).ToList();

            ProcessStaticFiles(fileList);
            ProcessStartEndPage(fileList);
            fileList.Add(legendUrl);
            // update cached resources simbal : updated by assessment's updatedOn and per week
            var hash = assessment.CreatedOn.Ticks + DateTime.Now.DayOfYear / 7;

            var cacheList = string.Join("\n", fileList);

            var fallback = "/ /Offline/Index/Offline";
            var network = "*";
            var content = string.Format("CACHE MANIFEST\n# hash:{0}\nCACHE:\n{1}\nFALLBACK:\n{2}\nNETWORK:\n{3}", hash,
                cacheList, fallback, network);
            byte[] by = Encoding.UTF8.GetBytes(content);
            return File(by, "text/cache-manifest");
        }


        private void ProcessStaticFiles(List<string> fileList)
        {
            fileList.Add("/Cpalls/Execute/Go?offline=true");
            fileList.Add("/Cpalls/Execute/ViewOffline");
            //fileList.Add(Url.Action("Manifest"));

            fileList.AddRange(OfflineHelper.GlobalResources);

            var cpallsList = new List<string>()
            {
                "~/scripts/modernizr/offline",
                "~/scripts/jquery/offline",
                "~/scripts/bootstrap/offline",
                "~/scripts/knockout/offline",
                "~/scripts/cli/offline",
                "~/scripts/cpalls/offline",
                "~/scripts/jquery_val/offline",
                "~/scripts/format/offline",
                "~/css/basic/offline",
                "~/css/cpalls/offline",
                "~/css/home"
            };
#if DEBUG
            cpallsList.ForEach(resource =>
            {
                string content = resource.StartsWith("~/scripts")
                    ? Scripts.Render(resource).ToString()
                    : Styles.Render(resource).ToString();
                fileList.AddRange(OfflineHelper.SplitResources(content));
            });
#endif

#if !DEBUG
            fileList.AddRange(cpallsList.Select(resource => resource.Replace("~", "") + "?v=" + BundleConfig.UpdateKey));  
#endif
        }

        private void ProcessStartEndPage(List<string> fileList)
        {
            fileList.AddRange(FileHelper.GetFiles("staticfiles"));
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.Offline, PageId = PagesModel.CPALLS, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public string Sync(int assessmentId, int studentId, int schoolId, string year, Wave wave, string measures, string items)
        {
            //logger.Info("A--> IndexController.Sync AssessmentId:"+assessmentId+",StudentId:"+studentId+",SchoolId:"+schoolId+",Year:"+year+",Wave:"+((int)wave).ToString()+",Measures:"+measures+",Items:"+items);
            ///Sam todo:所有参数，方法名
            var studentMeasures = JsonHelper.DeserializeObject<List<StudentMeasureEntity>>(measures);
            var studentItems = JsonHelper.DeserializeObject<List<StudentItemEntity>>(items);
            var studentAssessmentId = 0;
            var student = _studentBusiness.GetStudentModel(studentId, UserInfo);
            var schoolYear = int.Parse("20" + year.Substring(0, 2));
            var response = new PostFormResponse();
            response.Update(_cpallsBusiness.PlayMeasure(student, assessmentId, studentMeasures.Select(x => x.MeasureId).ToList(),
                 0, schoolYear, wave, UserInfo, out studentAssessmentId));
            if (response.Success && studentAssessmentId > 0)
                response.Update(_cpallsBusiness.SyncMeasures(studentAssessmentId, schoolId, studentMeasures,
                    studentItems, student.BirthDate, year, wave));
            return JsonHelper.SerializeObject(response);
        }

        public string Online()
        {
            var online = new
            {
                online = true,
                date = "",
                logged = UserInfo != null
            };
            return JsonHelper.SerializeObject(online);
        }
        public string Offline()
        {
            return ResourceHelper.GetRM().GetInformation("Offline_Message");
        }

    }
}