using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Business.Classes.Models;
using Sunnet.Cli.Business.Reports.Model;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Reports;
using Sunnet.Cli.Core.Reports.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework.Extensions;
using LinqKit;
using Sunnet.Cli.Business.Export.Models;
using Sunnet.Cli.Core.Users.Enums;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Export.Enums;

namespace Sunnet.Cli.Business.Reports
{
    public partial class ReportBusiness
    {

        public IList<ParentReportEntity> GetParentReportListByStudentId(int studentId)
        {
            IList<ParentReportEntity> list = new List<ParentReportEntity>();
            return _reportService.ParentReports.Where(c => c.StudentId == studentId).ToList();
        }
        public ParentReportEntity GetParentReport(int id)
        {
            return _reportService.ParentReports.FirstOrDefault(c => c.ID == id);
        }
        public List<ParentReportModel> SearchParentReports(DateTime dob,Expression<Func<ParentReportEntity, bool>> condition, string sort, string order, int first, int count, out int total)
        {
            var query = _reportService.ParentReports.Where(condition).Select(c => new ParentReportModel()
            {
                ID = c.ID,
                assessmentId = c.AssessmentId,
                ReportName = c.ReportName.Replace("-", "/"),
                CreatedOn = c.CreatedOn,
                DOB = dob
            }).ToList(); ;
            total = query.Count();
            var list = query.OrderBy(sort, order).Skip(first).Take(count);
            return list.ToList();
        }

        public string GetParentReportName(int studentId, string wave, string assessmentName, int index = 0)
        {
            string name = assessmentName + " (" + wave.ToString()+")";
            if (index > 0)
                name = name + "_" + index;
            try
            {
                if (_reportService.ParentReports.Any(c => c.StudentId == studentId && c.ReportName == name))
                {
                    index++;
                    return GetParentReportName(studentId, wave, assessmentName, index);
                }
                else
                {
                    return name;
                }
            }
            catch (Exception ex)
            {
                
                return "";
            }
           
        }

        public OperationResult InsertParentReport(List<ParentReportEntity> entities)
        {
            return _reportService.InsertParentReportList(entities);
        }
        public OperationResult InsertParentReport(ParentReportEntity entity)
        {
          return _reportService.InsertParentReport(entity);
        }
    }
}
