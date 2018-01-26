using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/15 2015 9:02:35
 * Description:		Please input class summary
 * Version History:	Created,1/15 2015 9:02:35
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Trs.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsSubcategoryModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TRSCategoryEnum Category { get; set; }

        public TRSItemType Type { get; set; }

        public int Sort { get; set; }
    }
}
