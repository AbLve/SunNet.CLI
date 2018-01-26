using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/11 1:09:31
 * Description:		Please input class summary
 * Version History:	Created,2014/8/11 1:09:31
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Core.Ade.Entities
{
    public class ObservableEntryEntity : ItemBaseEntity
    {
        private ICollection<TypedResponseEntity> _responses;

        [DisplayName("Target Text")]
        [StringLength(1000)]
        public string TargetText { get; set; }
        public bool IsShown { get; set; }
        //public virtual ICollection<TypedResponseEntity> Responses
        //{
        //    get { return _responses ?? (_responses = new List<TypedResponseEntity>()); }
        //    set { _responses = value; }
        //}
    }
}