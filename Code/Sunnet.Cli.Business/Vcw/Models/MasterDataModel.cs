using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/10/21
 * Description:		Create AuthorityEntity
 * Version History:	Created,2014/10/21
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Common.Enums;

namespace Sunnet.Cli.Business.Vcw.Models
{
    /// <summary>
    /// 该Model用于MasterData
    /// </summary>
    public class MasterDataModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public EntityStatus Status { get; set; }
    }
}
