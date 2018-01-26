using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/5/4
 * Description:		Please input class summary
 * Version History:	Created,2015/5/4
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Vcw;
using Sunnet.Cli.UIBase.Filters;
using Sunnet.Framework.Permission;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Business.Vcw.Models;

namespace Sunnet.Cli.Vcw.Controllers
{
    public class FeedbackController : BaseController
    {
        VcwBusiness _vcwBusiness;
        public FeedbackController()
        {
            _vcwBusiness = new VcwBusiness(VcwUnitWorkContext);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public ActionResult FileFeedback(int id)
        {
            ViewBag.Title = "Feedback";
            FeedbackModel model = _vcwBusiness.GetFileFeedbackModel(id);
            return View(model);
        }

        [CLIUrlAuthorizeAttribute(Account = Authority.All, PageId = PagesModel.VCW, Anonymity = Anonymous.Verified)]
        public ActionResult AssignmentFeedback(int id)
        {
            ViewBag.Title = "Feedback";
            FeedbackModel model = _vcwBusiness.GetAssignmentFeedbackModel(id);
            return View(model);
        }
    }
}