using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		1/12 2015 10:09:20
 * Description:		Please input class summary
 * Version History:	Created,1/12 2015 10:09:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.Trs;
using Sunnet.Cli.Core.Users.Entities;
using Sunnet.Cli.Core.Users.Enums;
using Newtonsoft.Json;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Business.Users;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsSchoolModel
    {
        public int ID { get; set; }

        internal IEnumerable<int> CommunityIds { get; set; }

        [DisplayName("LWDB")]
        public IEnumerable<string> Communities { get; set; }

        [DisplayName("LWDB")]
        public string CommunityNames
        {
            get
            {
                return string.Join(", ", Communities);
            }
        }

        [DisplayName("School")]
        public string Name { get; set; }

        [DisplayName("DFPS License #/Operator #")]
        public string DfpsNumber { get; set; }

        public UserBaseModel Assessor { get; set; }

        [DisplayName("Director")]
        public IEnumerable<UserBaseModel> Principals { get; set; }

        public IEnumerable<string> NamesOfPrincipals
        {
            get
            {
                if (Principals == null) return null;
                return Principals.Select(x => x.FullName);
            }
        }

        [DisplayName("Calculated Star")]
        public TRSStarEnum StarStatus { get; set; }

        public DateTime StarDate { get; set; }

        [DisplayName("Verified Star")]
        public TRSStarEnum VerifiedStar { get; set; }

        public DateTime TrsLastStatusChange { get; set; }

        [DisplayName("Star Designation Date")]
        public DateTime StarDesignationDate { get; set; }

        [DisplayName("Recertification By")]
        public DateTime RecertificationBy { get; set; }

        public SchoolStatus Status { get; set; }

        public FacilityType FacilityType { get; set; }

        public List<TrsClassModel> Classes { get; set; }

        [DisplayName("Action Required")]
        public DateTime ActionRequired { get; set; }

        /// <summary>
        /// 判断Items对School是否可见.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        internal bool ShouldVisible(TrsItemModel model)
        {
            var result = false;
            switch (this.FacilityType)
            {
                case FacilityType.LCAA:
                    result = model.LCAA != TRSFacilityEnum.N_A;
                    break;
                case FacilityType.LCCH:
                    result = model.LCCH != TRSFacilityEnum.N_A;
                    break;
                case FacilityType.LCSA:
                    result = model.LCSA != TRSFacilityEnum.N_A;
                    break;
                case FacilityType.RCCH:
                    result = model.RCCH != TRSFacilityEnum.N_A;
                    break;
                default:
                    break;
            }
            return result;
        }

        public void UpdateAction(UserBaseEntity user)
        {
            switch (user.Role)
            {
                case Role.Super_admin:
                    Action = this.Assessor != null ? "assessment" : "edit";
                    break;
                case Role.Statewide:
                    Action = "edit";
                    break;
                case Role.Principal:
                case Role.Principal_Delegate:
                    Action = this.Principals.Any(x => x.UserId > 0 && (x.UserId == user.ID || x.UserId == user.Principal.ParentId)) ? "view" : "";
                    break;
                case Role.TRS_Specialist:
                case Role.TRS_Specialist_Delegate:
                    //For a school, allow all TRS Specialist users under that school to view all TRS reports under that school. 
                    if (user.Role == Role.TRS_Specialist)
                        Action = user.UserCommunitySchools.Select(r => r.SchoolId).Contains(this.ID) ? "viewAssessment" : Action;
                    if (user.Role == Role.TRS_Specialist_Delegate)
                    {
                        UserBaseEntity parentUser = new UserBusiness().GetUser(user.Principal.ParentId);
                        Action = parentUser.UserCommunitySchools.Select(r => r.SchoolId).Contains(this.ID) ? "viewAssessment" : Action;
                    }

                    //Class的TtsAssessor可以做自己的Class,TrsMentor可以查看
                    if (this.Classes != null)
                    {
                        Action = this.Classes.Any(x => x.TrsAssessorId > 0 &&
                            (x.TrsAssessorId == user.ID || x.TrsAssessorId == user.Principal.ParentId)) ? "assessment" : Action;
                    }
                    Action = this.Assessor != null && this.Assessor.UserId > 0
                        && (this.Assessor.UserId == user.ID || this.Assessor.UserId == user.Principal.ParentId) ? "assessment" : Action;
                    break;
                case Role.Community:
                case Role.District_Community_Specialist:
                    Action = user.CommunityUser != null && user.UserCommunitySchools.Any(e => this.CommunityIds.Contains(e.CommunityId)) ? "edit" : "";
                    break;
                case Role.District_Community_Delegate:
                case Role.Community_Specialist_Delegate:
                    UserBaseEntity parentComUser = new UserBusiness().GetUser(user.CommunityUser.ParentId);
                    Action = parentComUser.UserCommunitySchools.Any(e => this.CommunityIds.Contains(e.CommunityId)) ? "edit" : "";
                    break;
                default: break;
            }
        }
        /// <summary>
        /// 客户端显示时提供操作依据,
        /// view:只能看
        /// edit:可编辑
        /// assessment:可以做Assessment
        /// </summary>
        public string Action { get; set; }
    }

    public class TrsSchoolReportModel
    {
        public int SchoolId { get; set; }

        public string SchoolName { get; set; }

        internal IEnumerable<int> CommunityIds { get; set; }

        public IEnumerable<string> Communities { get; set; }

        public string CommunityNames
        {
            get
            {
                return string.Join(", ", Communities);
            }
        }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string FacilityTelephone { get; set; }

        public string SecondaryTelephone { get; set; }

        public string Owner { get; set; }

        public bool NAEYC { get; set; }

        public bool CANASA { get; set; }

        public bool NECPA { get; set; }

        public bool NACECCE { get; set; }

        public bool NAFCC { get; set; }

        public bool ACSI { get; set; }

        public string NationalAccreditation
        {
            get
            {
                string value = "";
                if (NAEYC)
                    value += "National Association for the Education of Young Children (NAEYC);";
                if (CANASA)
                    value += "Commission on Accreditation - National After School Association;";
                if (NECPA)
                    value += "National Early Childhood Program Accreditation(NECPA);";
                if (NACECCE)
                    value += "National Accreditation Commission for Early Child Care and Educationa;";
                if (NAFCC)
                    value += "National Association of Family Child Care (NAFCC);";
                if (ACSI)
                    value += "Association of Chistion Schools International (ACSI);";
                if (value.EndsWith(";"))
                    value = value.Remove(value.Length - 1);
                return value;
            }
        }

        public string PrimaryName { get; set; }

        public string Directors
        {
            get { return PrimaryName; }
        }

        public Regulating RegulatingEntity { get; set; }

        public FacilityType FacilityType { get; set; }

        public string Assessor { get; set; }

        public string DFPS { get; set; }


        /// <summary>
        /// 判断Items对School是否可见.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        internal bool ShouldVisible(TrsItemReportModel model)
        {
            var result = false;
            switch (this.FacilityType)
            {
                case FacilityType.LCAA:
                    result = model.LCAA != TRSFacilityEnum.N_A;
                    break;
                case FacilityType.LCCH:
                    result = model.LCCH != TRSFacilityEnum.N_A;
                    break;
                case FacilityType.LCSA:
                    result = model.LCSA != TRSFacilityEnum.N_A;
                    break;
                case FacilityType.RCCH:
                    result = model.RCCH != TRSFacilityEnum.N_A;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
