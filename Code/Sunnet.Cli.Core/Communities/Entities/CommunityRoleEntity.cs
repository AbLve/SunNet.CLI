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
 * CreatedOn:		2014/8/18 16:27:20
 * Description:		Create CommunitiesRspt
 * Version History:	Created,2014/8/18 16:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Classes.Entites;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Students.Entities;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.Mvc;
using Sunnet.Cli.Core.Users.Entities;

namespace Sunnet.Cli.Core.Communities.Entities
{
    public class CommunityRoleEntity : EntityBase<int>
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string CommunityId { get; set; }
        public string BasicCommunityId { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string SchoolYear { get; set; }
        public string FundingId { get; set; }
        public string ECircle { get; set; }
        public string ECircleRequest { get; set; }
        public string Beech { get; set; }
        public string BeechRequest { get; set; }
        public string Cpalls { get; set; }
        public string CpallsRequest { get; set; }
        public string Coaching { get; set; }
        public string CoachingRequest { get; set; }
        public string Materials { get; set; }
        public string MaterialsRequest { get; set; }
        public string Training { get; set; }
        public string TrainingRequest { get; set; }
        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string City { get; set; }
        public string CountyId { get; set; }
        public string StateId { get; set; }
        public string Zip { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberType { get; set; }
        public string PrimarySalutation { get; set; }
        public string PrimaryName { get; set; }
        public string PrimaryTitleId { get; set; }
        public string PrimaryPhone { get; set; }
        public string PrimaryPhoneType { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondarySalutation { get; set; }
        public string SecondaryName { get; set; }
        public string SecondaryTitleId { get; set; }
        public string SecondaryPhone { get; set; }
        public string SecondaryPhoneType { get; set; }
        public string SecondaryEmail { get; set; }
        public string WebAddress { get; set; }
        public string MouStatus { get; set; }
        public string MouDocument { get; set; }
        public string Notes { get; set; }
        public string ECircleCli { get; set; }
        public string BeechCli { get; set; }
        public string CpallsCli { get; set; }
        public string CoachingCli { get; set; }
        public string MaterialsCli { get; set; }
        public string TrainingCli { get; set; }
        public string TexasRisingStar { get; set; }
    }

}
