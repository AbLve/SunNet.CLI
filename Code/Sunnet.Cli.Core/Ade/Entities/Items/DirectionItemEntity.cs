using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/28 1:21:22
 * Description:		Please input class summary
 * Version History:	Created,2014/8/28 1:21:22
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class DirectionItemEntity : ItemBaseEntity
    {

        [EensureEmptyIfNull]
        [DisplayName("Direction Text")]
        public string FullDescription { get; set; }
    }
}
