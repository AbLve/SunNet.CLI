using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using StructureMap;
using Sunnet.Cli.Business.Cpalls.Growth;
using Sunnet.Cli.MainSite.Areas.Demo.Models;
using Sunnet.Cli.UIBase;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Extensions;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.MainSite.Areas.Demo.Controllers
{
    public enum TestEnum
    {
        /// <summary>
        /// ttttttttttt
        /// </summary>
        [Description("Hello!")]
        [Display(Name = "Hello!")]
        Hello = 0,
        [Description(" world.")]
        [Display(Name = " world.")]
        World = 1
    }

    public class TestClass
    {
        public int ID { get; set; }
        [Required]
        public TestEnum TE { get; set; }

        [EensureEmptyIfNull]
        public string TestNull { get; set; }

        public bool Selected { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }
    }

    public class DemoController : Controller
    {
        //
        // GET: /Demo/Demo/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Slide()
        {
            return View();
        }

        public ActionResult Twitter()
        {
            //#if !DEBUG
            //            var client = new HttpClient();
            //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", ConfigurationManager.AppSettings["TwitterAuthorization"]);
            //            client.BaseAddress = new Uri("https://api.twitter.com/1.1/");
            //            var response = await client.GetAsync("statuses/user_timeline.json?screen_name=twitterapi&count=1");  // Blocking call!
            //            var tweets = await response.Content.ReadAsAsync<IEnumerable<tweet>>();
            //            ViewBag.Tweets = tweets;
            //#endif
            return View();
        }

        public ActionResult Enum()
        {
            ViewBag.ItemsOptions = TestEnum.Hello.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.Json = JsonHelper.SerializeObject(new TestClass());
            return View(new TestClass() { TE = (TestEnum)1 });
        }
        [HttpPost]
        public ActionResult Enum(TestClass testModel)
        {
            ViewBag.Items = TestEnum.Hello.ToSelectList().AddDefaultItem(ViewTextHelper.DefaultPleaseSelectText, "");
            ViewBag.Json = JsonHelper.SerializeObject(new TestClass() { TE = TestEnum.World });

            return View(testModel);
        }
        public ActionResult Confirm()
        {
            return View();
        }

        public ActionResult Email()
        {
            ViewBag.Result = "就绪";
            return View();
        }
        [HttpPost]
        public ActionResult Email(string to, string subject, string body, bool isAsync)
        {
            var emailSender = ObjectFactory.GetInstance<IEmailSender>();
            if (isAsync)
            {
                new SendHandler(() => emailSender.SendMail(to, subject, body))
                    .BeginInvoke(null, null);
                new SendHandler(() => emailSender.SendMail(to, "Jack", subject, body, ""))
                    .BeginInvoke(null, null);
                ViewBag.Result = "异步发送";
            }
            else
            {
                var result = emailSender.SendMail(to, "Jack", subject, body, "").ToString();
                result += emailSender.SendMail(to, subject, body);
                ViewBag.Result = result;
            }
            return View();
        }

        private delegate void SendHandler();

        public ActionResult TestNull()
        {
            var test = new TestClass();
            ViewBag.isNull = test.TestNull == null;
            return View(test);
        }
        [HttpPost]
        public ActionResult TestNull(TestClass test)
        {
            ViewBag.isNull = test.TestNull == null;
            return View(test);
        }

        public string GetDataItems(string keyword)
        {
            var list = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                list.Add(new { Text = string.Format("Item + abcd+ {0}", i), Value = i.ToString(), Other1 = "Other1", Other2 = "Other2" });
            }
            for (int i = 8; i < 15; i++)
            {
                list.Add(new { Text = string.Format("NewItem + abcd+ {0}", i), Value = i.ToString(), Other1 = "Other1" });
            }
            return JsonHelper.SerializeObject(list);
        }

        public ActionResult AutoComplete()
        {
            return View(new Products() { CreatedOnDateTime = DateTime.Now });
        }

        [HttpPost]
        public string AutoComplete(Products p)
        {
            return "{\"success\":false}";
        }

        public ActionResult Layout()
        {
            return View();
        }

        public ActionResult Format()
        {
            return View();
        }

        public ActionResult Validate()
        {
            return View();
        }


        public ActionResult Files()
        {
            ViewBag.Files = FileHelper.GetFiles("staticfiles");
            return View();
        }
        [HttpPost]
        public ActionResult Files(string folder)
        {
            ViewBag.Files = FileHelper.GetFiles(folder);
            return View();
        }

        public string SplitPdf()
        {
            var file = @"F:\Users\JackZhang\Desktop\Class_Growth_Report.pdf";
            return "";
        }


    }

}