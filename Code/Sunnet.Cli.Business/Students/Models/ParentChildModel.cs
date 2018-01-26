using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2014/10/10 12:15:10
 * Description:		Please input class summary
 * Version History:	Created,2014/10/10 12:15:10
 **************************************************************************/
using System.ComponentModel.DataAnnotations;
using Sunnet.Cli.Core.Users.Enums;
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Students.Models
{
    public class ParentChildModel
    {
        [Required]
        [Display(Name="Parent Code")]
        public string ParentId { get; set; }

        [Required]
        [Display(Name = "Child's First Name")]
        public string ChildFirstName { get; set; }

        [Required]
        [Display(Name = "Child's Last Name")]
        public string ChildLastName { get; set; }

        [Required]
        public ParentRelation Relation { get; set; }

        [EensureEmptyIfNull]
        public string RelationOther { get; set; }
    }
}
