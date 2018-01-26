using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/10/09 8:43:20
 * Description:		Create SelectItemModelOther
 * Version History:	Created,2014/10/09 8:43:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Common.Enum
{
    public class SelectItemModelOther
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public EntityStatus Status { get; set; }

        public string Other { get; set; }

        public int OtherId { get; set; }

        public bool Selected { get; set; }
    }
}
