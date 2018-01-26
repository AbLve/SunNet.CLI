using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/11/15 3:15:03
 * Description:		Please input class summary
 * Version History:	Created,2014/11/15 3:15:03
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Cpalls.Models.Report;

namespace Sunnet.Cli.Business.Cpalls
{

    internal interface IGenerator
    {
        void Generate();

    }

    internal interface IReportGenerator : IGenerator
    {
        string Filename { get; }
        Dictionary<object, List<ReportRowModel>> Reports { get; }
    }
}
