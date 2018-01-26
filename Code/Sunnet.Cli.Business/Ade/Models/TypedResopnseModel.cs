using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		6/9/2015 16:15:59
 * Description:		Please input class summary
 * Version History:	Created,6/9/2015 16:15:59
 *
 *
 **************************************************************************/
using Sunnet.Cli.Core.Ade;


namespace Sunnet.Cli.Business.Ade.Models
{
    public class TypedResopnseModel
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public bool Required { get; set; }

        public TypedResponseType Type { get; set; }

        public int Length { get; set; }

        public string Text { get; set; }

        public string Picture { get; set; }

        public int TextTimeIn { get; set; }

        public int PictureTimeIn { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<TypedResponseOptionModel> Options { get; set; }


        /// <summary>
        /// 该Response的分数
        /// </summary> 
        /// Author : JackZhang
        /// Date   : 7/6/2015 11:27:55
        public decimal Score
        {
            get
            {
                return Options != null && Options.Any() ? Options.Max(x => x.Score) : 0;
            }
        }
    }
}
