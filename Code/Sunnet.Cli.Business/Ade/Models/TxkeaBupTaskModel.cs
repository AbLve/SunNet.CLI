using Sunnet.Cli.Core.Ade.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/12/16
 * Description:		
 * Version History:	Created,2015/12/16
 * 
 * 
 **************************************************************************/

namespace Sunnet.Cli.Business.Ade.Models
{
    public class TxkeaBupTaskModel : TxkeaBupTaskEntity
    {
        public string SubmitedTime
        {
            get
            {
                if (CreatedOn != null)
                    return CreatedOn.ToString("MM/dd/yyyy HH:mm");
                else
                    return "";
            }
        }

        public string ProcessedTime
        {
            get
            {
                if (UpdatedOn != null)
                    return UpdatedOn.ToString("MM/dd/yyyy HH:mm");
                else
                    return "";
            }
        }
    }
}
