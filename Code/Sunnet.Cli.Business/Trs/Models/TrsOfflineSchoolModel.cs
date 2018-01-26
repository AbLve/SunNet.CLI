using Sunnet.Cli.Business.Common;
using Sunnet.Cli.Business.Users.Models;
using Sunnet.Cli.Core.Schools.Entities;
using Sunnet.Cli.Core.Schools.Enums;
using Sunnet.Cli.Core.Trs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.Trs.Models
{
    public class TrsOfflineSchoolModel : TrsSchoolModel
    {
        public TrsOfflineSchoolModel()
        {
            AssessmentList = new List<TrsAssessmentModel>();
            NewAssessment = new TrsAssessmentModel();
        }

        public bool IsCommunityTRS { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Owner { get; set; }

        public string FacilityTelephone { get; set; }

        public string SecondaryTelephone { get; set; }

        public string Directors { get; set; }

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

        public string TrsTaStatus { get; set; }

        public string DFPS { get; set; }

        public string MinAgeGroup { get; set; }

        public string MaxAgeGroup { get; set; }

        public Regulating RegulatingEntity { get; set; }

        public List<TrsAssessmentModel> AssessmentList { get; set; }

        public TrsAssessmentModel NewAssessment { get; set; }  //新增时的模板
    }
}
