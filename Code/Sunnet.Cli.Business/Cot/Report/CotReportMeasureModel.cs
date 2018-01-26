using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/30 2015 10:50:10
 * Description:		Please input class summary
 * Version History:	Created,1/30 2015 10:50:10
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Cot;

namespace Sunnet.Cli.Business.Cot.Report
{
    public class CotReportMeasureModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int CountOfBoy { get; set; }

        public int CountOfMoy { get; set; }

        public int Total { get; set; }

    }
}
