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
using Sunnet.Cli.Business.Practices;
using Sunnet.Cli.Core.Practices.Entities;
using Sunnet.Cli.Practice.Controllers;
using Sunnet.Cli.Practice.Models;

namespace Sunnet.Cli.Practice.Areas.Offline.Controllers
{
    public class IndexController : BaseController
    {

        private PracticeOfflineBusiness _offlineBusiness;
        private AdeBusiness _adeBusiness;
        private PracticeBusiness _practiceBusiness;
        private ISunnetLog logger;
        public IndexController()
        {

            _offlineBusiness = new PracticeOfflineBusiness(PracticeUnitWorkContext);
            _practiceBusiness = new PracticeBusiness(PracticeUnitWorkContext);
            _adeBusiness = new AdeBusiness();
            logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        private void ProcessViewBag(int assessmentId, int year, Wave wave, string legendUrl)
        {
            Session["AssessmentId"] = assessmentId;
            Session["Year"] = year;
            Session["Wave"] = wave;
            Session["LegendUrl"] = legendUrl;
        }

        private object GetManifestUrl(int assessmentId = 0, int year = 0, Wave wave = Wave.BOY, string legendUrl = "")
        {
            return Url.Action("Manifest", new
            {
                assessmentId = Session["AssessmentId"] == null ? assessmentId : Session["AssessmentId"],
                year = Session["Year"] == null ? year : Session["Year"],
                wave = Session["Wave"] == null ? wave : Session["Wave"],
                legendUrl = Session["LegendUrl"] == null ? "" : Session["LegendUrl"]
            });
        }

        // GET: Offline/Index
        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public ActionResult Index(int assessmentId = 0, int year = 0, Wave wave = Wave.BOY)
        {
            ViewBag.LoginUrl = BuilderLoginUrl(LoginUserType.GOOGLEACCOUNT, LoginIASID.CPALLS_OFFLINE);
            ViewBag.CLIUserLogin = BuilderLoginUrl(LoginUserType.UTACCESSMANAGER, LoginIASID.CPALLS_OFFLINE);
            ViewBag.Manifest = GetManifestUrl(assessmentId, year, wave);
            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.AssessmentPracticeArea, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified, Parameter = "assessmentId")]
        public ActionResult Preparing(int assessmentId = 0, AssessmentLanguage language = AssessmentLanguage.English,
            int year = 0, Wave wave = Wave.BOY)
        {
            if (assessmentId < 1)
                return View();
            if (year < 1) year = CommonAgent.Year;
            var assessment = _practiceBusiness.GetAssessment(assessmentId, wave);

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
                                //NonSelectable时，不进行随机排序，且要保留原位置
                                List<int> NonSelectableIndex = item.Answers.Select((r, i) => new { r, i }).
                                    Where(r => r.r.ImageType == ImageType.NonSelectable).Select(r => r.i).ToList();
                                choosedRandomIndex.AddRange(NonSelectableIndex);
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
                                        Random random = new Random();
                                        int randomIndex = random.Next(0, item.Answers.Count);
                                        while (choosedRandomIndex.IndexOf(randomIndex) >= 0)
                                        {
                                            randomIndex = random.Next(0, item.Answers.Count);
                                        }
                                        choosedRandomIndex.Add(randomIndex);
                                        item.Answers[randomIndex].PictureTime = OrderedAnswers[i].PictureTime;
                                        item.Answers[randomIndex].AudioTime = OrderedAnswers[i].AudioTime;
                                        RandomAnswers.Add(item.Answers[randomIndex]);
                                    }
                                }
                                item.Answers = RandomAnswers;
                            }
                        }
                    }
                }
            }

            assessment.SchoolId = 0;
            assessment.SchoolName = "";
            assessment.CommunityName = "";
            assessment.SchoolYear = year.ToSchoolYearString();
            assessment.Student = new ExecCpallsStudentModel();

            var legendUi = _adeBusiness.GetAssessmentLegend(assessmentId, LegendTypeEnum.EngageUI);
            string legendUrl = legendUi != null ? SFConfig.StaticDomain + "upload/" + legendUi.ColorFilePath : "";
            assessment.LegendUIFilePath = legendUrl;
            assessment.LegendUIText = legendUi != null ? legendUi.Text : "";
            assessment.LegendUITextPosition = legendUi != null ? legendUi.TextPosition : "";
            var list = _offlineBusiness.GetStudentsAssessmentsForOffline(UserInfo, assessmentId, wave, year);
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
                        dataFrom = mea.Value.DataFrom,
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
            "/Cpalls/Student?assessmentId={0}&year={1}&wave={2}",
            assessmentId, year, (byte)wave)
            };

            ViewBag.Json = JsonHelper.SerializeObject(jsonObj);

            ViewBag.AssessmentId = assessmentId;
            ViewBag.SchoolId = 0;
            ViewBag.ClassId = 0;
            ViewBag.Year = year;
            ViewBag.Wave = wave;

            ProcessViewBag(assessmentId, year, wave, legendUrl);

            return View();
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.Practice, Anonymity = Anonymous.Verified)]
        public FileResult Manifest(int assessmentId, int year, Wave wave, string legendUrl)
        {
            if (assessmentId + year < 2)
                return File(new byte[] { }, "text/html");
            ExecCpallsAssessmentModel assessment = _practiceBusiness.GetAssessmentForOffline(assessmentId, wave);
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
                "~/scripts/modernizr/practiceoffline",
                "~/scripts/jquery/practiceoffline",
                "~/scripts/bootstrap/practiceoffline",
                "~/scripts/knockout/practiceoffline",
                "~/scripts/cli/practiceoffline",
                "~/scripts/cpalls/practiceoffline",
                "~/scripts/jquery_val/practiceoffline",
                "~/scripts/format/practiceoffline",
                "~/css/basic/practiceoffline",
                "~/css/cpalls/practiceoffline"
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
        public string Sync(int assessmentId, int studentId, string year, Wave wave, string measures, string items)
        {
            var studentMeasures = JsonHelper.DeserializeObject<List<PracticeStudentMeasureEntity>>(measures);
            var studentItems = JsonHelper.DeserializeObject<List<PracticeStudentItemEntity>>(items);
            var studentAssessmentId = 0;
            var student = _practiceBusiness.GetStudentModel(studentId);
            var schoolYear = int.Parse("20" + year.Substring(0, 2));
            var response = new PostFormResponse();
            response.Update(_practiceBusiness.PlayMeasure(student, assessmentId, studentMeasures.Select(x => x.MeasureId).ToList(),
                 0, schoolYear, wave, UserInfo, out studentAssessmentId));
            if (response.Success && studentAssessmentId > 0)
                response.Update(_practiceBusiness.SyncMeasures(studentAssessmentId, studentMeasures,
                    studentItems, student.StudentDob, year, wave));
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