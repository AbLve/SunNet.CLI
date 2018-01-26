using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sunnet.Framework.Helpers;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Business.Ade;
using Sunnet.Cli.UIBase.Models;
using Sunnet.Cli.Assessment.Controllers;

namespace Sunnet.Cli.Assessment.Areas.BulkUpload.Controllers
{
    public class PublicController : BaseController
    {
        private readonly AdeBusiness _adeBusiness;

        public PublicController()
        {
            _adeBusiness = new AdeBusiness(AdeUnitWorkContext);
        }

        public string GetAssessmentSelectList()
        {
            var expression = PredicateHelper.True<AssessmentEntity>();
            expression = expression.And(r => r.Locked == false);
            var list = _adeBusiness.GetAssessmentSelectList(expression);
            return JsonHelper.SerializeObject(list);
        }

        public string GetMeasureSelectList(int AssessmentId = 0)
        {
            var expression = PredicateHelper.True<MeasureEntity>();
            expression = expression.And(r => r.Assessment.Locked == false);
            if (AssessmentId > 0)
                expression = expression.And(r => r.AssessmentId == AssessmentId);
            var list = _adeBusiness.GetMeasureSelectList(expression);
            return JsonHelper.SerializeObject(list);
        }
    }
}