using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		6/9/2015 16:16:36
 * Description:		Please input class summary
 * Version History:	Created,6/9/2015 16:16:36
 *
 *
 **************************************************************************/
using Sunnet.Cli.Core.Ade;


namespace Sunnet.Cli.Business.Ade.Models
{
    public class TypedResponseOptionModel
    {
        public int Id { get; set; }

        public TypedResponseType Type { get; set; }

        public string Keyword { get; set; }

        public decimal From { get; set; }

        public decimal To { get; set; }

        public decimal Score { get; set; }

        public int ResponseId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
