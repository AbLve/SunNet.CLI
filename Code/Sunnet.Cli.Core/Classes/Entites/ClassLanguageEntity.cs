using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/28 16:49:20
 * Description:		Create ClassLanguages
 * Version History:	Created,2014/8/28 16:49:20
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Core.Base;

namespace Sunnet.Cli.Core.Classes.Entites
{
    public class ClassLanguageEntity:EntityBase<int>
    {
        /// <summary>
        /// 该字段对应Class里的主键(ID)
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// 该字段对应Language里的主键(ID)
        /// </summary>
        public int LanguageId { get; set; }
    }
}
