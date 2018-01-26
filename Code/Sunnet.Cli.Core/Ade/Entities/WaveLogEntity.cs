using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core.Cpalls;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Base;


/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 1:01:46
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:01:46
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade.Entities
{


    public class WaveLogEntity : EntityBase<int>
    {
         
        [Required]
        public int UserId { get; set; }
            [Required]
        public Wave WaveValue { get; set; }

        public int AssessmentId { get; set; }
    }
}
