using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/13 2015 10:15:20
 * Description:		Please input class summary
 * Version History:	Created,1/13 2015 10:15:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Enums;
using Sunnet.Cli.Core.TRSClasses.Enums;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsAgeModel
    {
        public int Id { get; set; }
        public int NumberOfChildren { get; set; }
        public string TypeOfChildren { get; set; }
        public TrsAgeArea AgeArea { get; set; }

    }

    /// <summary>
    /// 用于生成报表时获取最小年龄和最大年龄
    /// </summary>
    public class TrsAgeReportModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int AgeSort { get; set; }

        public TrsAgeArea AgeArea { get; set; }
    }
}
