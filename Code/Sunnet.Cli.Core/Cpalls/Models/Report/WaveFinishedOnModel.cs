using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		6/22/2015 17:37:46
 * Description:		Please input class summary
 * Version History:	Created,6/22/2015 17:37:46
 *
 *
 **************************************************************************/


namespace Sunnet.Cli.Core.Cpalls.Models.Report
{
    public class WaveFinishedOnModel
    {
        public int CommunityId { get; set; }

        public int SchoolId { get; set; }

        public int ClassId { get; set; }

        public int StudentId { get; set; }

        public Wave Wave { get; set; }

        public DateTime FinishedOn { get; set; }


    }
}
