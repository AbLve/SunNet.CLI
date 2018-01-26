using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/9/5 9:40:00
 * Description:		Create CommunityFeatureModel
 * Version History:	Created,2014/9/5 9:40:00
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Mvc;

namespace Sunnet.Cli.Business.Communities.Models
{
    public class CommunityFeatureModel
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public bool ECircleCli { get; set; }
        public bool ECircleRequest { get; set; }
        [EensureEmptyIfNull]
        [StringLength(140)]
        [DisplayName("eCIRCLE")]
        public string ECircle { get; set; }

        public bool BeechCli { get; set; }
        public bool BeechRequest { get; set; }
        [EensureEmptyIfNull]
        [StringLength(140)]
        [DisplayName("BEECH")]
        public string Beech { get; set; }

        public bool CpallsCli { get; set; }
        public bool CpallsRequest { get; set; }
        [EensureEmptyIfNull]
        [StringLength(140)]
        [DisplayName("CPALLS+")]
        public string Cpalls { get; set; }

        public bool CoachingCli { get; set; }
        public bool CoachingRequest { get; set; }
        [EensureEmptyIfNull]
        [StringLength(140)]
        [DisplayName("Coaching")]
        public string Coaching { get; set; }

        public bool MaterialsCli { get; set; }
        public bool MaterialsRequest { get; set; }
        [EensureEmptyIfNull]
        [StringLength(140)]
        [DisplayName("Materials")]
        public string Materials { get; set; }

        public bool TrainingCli { get; set; }
        public bool TrainingRequest { get; set; }
        [EensureEmptyIfNull]
        [StringLength(140)]
        [DisplayName("2 - Day Training")]
        public string Training { get; set; }
        public bool TexasRisingStar { get; set; }
    }
}
