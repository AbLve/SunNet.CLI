using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		Sam
 * Computer:		SAM-PC
 * Domain:			sam-pc
 * CreatedOn:		2014/11/8 12:10:16
 * Description:		Please input class summary
 * Version History:	Created,2014/11/8 12:10:16
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Reports
{
    public enum DataExportStatus : byte
    {
        /// <summary>
        /// �ȴ�����
        /// </summary>
        Pending = 1,

        /// <summary>
        /// ������
        /// </summary>
        Processing = 2,

        /// <summary>
        /// �������
        /// </summary>
        Processed = 3,

        /// <summary>
        /// ����
        /// </summary>
        Error = 4
    }
}
