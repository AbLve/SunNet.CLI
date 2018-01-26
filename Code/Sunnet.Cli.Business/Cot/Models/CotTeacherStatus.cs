using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2014/12/19 18:48:10
 * Description:		Please input class summary
 * Version History:	Created,2014/12/19 18:48:10
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Cot.Models
{
    public class CotTeacherStatus
    {

        /// <summary>
        /// 是否有未完成的Wave: BOY,MOY.
        /// </summary>
        public bool HasWavesToDo { get; set; }
        /// <summary>
        /// 是否可以编辑STGReport
        /// </summary>
        public bool HasLastStgReport
        {
            get { return LastStgReportId > 0; }
        }
        public int LastStgReportId { get; set; }
        /// <summary>
        /// 是否有已经Finalize的Wave
        /// </summary>
        public bool HasCotReport { get; set; }
        /// <summary>
        /// 是否显示CotPdfVerison链接
        /// </summary>
        public bool CotPdfVerisonVisible { get; set; }

        public bool ResetShortTermGoalsVisible { get; set; }

        /// <summary>
        /// 是否显示Stg Reports
        /// </summary>
        public bool StgReportReadOnly { get; set; }

        public bool HasOldData { get; set; }
    }
}
