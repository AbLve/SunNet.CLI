using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Framework.Resources;

namespace Sunnet.Cli.Assessment.Controllers
{
    public class MessagesController : ApiController
    {
        public string Get()
        {
            var messages = new Dictionary<string, object>();
            var successTitles = ResourceHelper.GetRM()
                .GetInformation("JS_Msg_Success").Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var failTitles = ResourceHelper.GetRM()
                .GetInformation("JS_Msg_Fail").Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var messageBox = new List<AjaxMessage>()
            {
                new AjaxMessage("success",successTitles[0], successTitles[1], "success"),
                new AjaxMessage("fail",failTitles[0], failTitles[1], "danger")
            };
#if DEBUG
            messageBox.Add(new AjaxMessage("debug", "Debug", "Source code error.", "danger"));
#endif
            messages.Add("modalMsgs", messageBox);
            var otherMessages = new Dictionary<string, string>();

            otherMessages.Add("Assessment_CutoffScore_order",
                ResourceHelper.GetRM().GetInformation("Assessment_CutoffScore_order"));

            messages.Add("other", otherMessages);
            return JsonHelper.SerializeObject(messages);
        }
    }
}
