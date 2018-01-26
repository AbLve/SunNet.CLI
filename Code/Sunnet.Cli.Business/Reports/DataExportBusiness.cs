using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using LinqKit;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Classrooms.Entites;
using Sunnet.Cli.Core.Communities.Entities;
using Sunnet.Cli.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Reports.Entities;
using System.Linq.Expressions;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Framework;
using Sunnet.Framework.Core.Extensions;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Business.Vcw.Models;
using System.IO;

namespace Sunnet.Cli.Business.Reports
{
    public class DataExportBusiness
    {
        private readonly IReportContract _reportService;
        public DataExportBusiness(EFUnitOfWorkContext unit = null)
        {
            _reportService = DomainFacade.CreateReportService(unit);
        }

    }
}
