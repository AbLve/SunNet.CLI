using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		Joe-PC
 * Domain:			Joe-pc
 * CreatedOn:		2016/4/25
 * Description:		
 * Version History:	Created,2016/4/25
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Tsds.Models
{
    public class DownloadListModel
    {
        public int ID { get; set; }

        public string FileName { get; set; }

        public string MisFileName { get; set; }
        public string CommunityName { get; set; }

        public string SchoolName { get; set; }

        public string DownloadedBy { get; set; }

        public DateTime DownloadedOn { get; set; }

        public TsdsStatus Status { get; set; }
    }
}
