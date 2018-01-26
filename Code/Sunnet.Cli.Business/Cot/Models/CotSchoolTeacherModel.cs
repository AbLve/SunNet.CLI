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
 * CreatedOn:		7/2/2015 16:16:16
 * Description:		Please input class summary
 * Version History:	Created,7/2/2015 16:16:16
 *
 *
 **************************************************************************/
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Cot.Models;


namespace Sunnet.Cli.Business.Cot.Models
{
    public class CotSchoolTeacherModel : CotTeacherModel
    {
        public CotSchoolTeacherModel()
        {
        }

        public IEnumerable<string> Schools { get; set; }

        [DisplayName("School Names")]
        public string SchoolsText
        {
            get
            {
                if (Schools == null)
                    return string.Empty;
                return string.Join(", ", Schools);
            }
        }

        [DisplayName("Community/District")]
        public IEnumerable<string> Communities { get; set; }

        [DisplayName("Community/District")]
        public string CommunitiesText
        {
            get
            {
                return Communities == null ? string.Empty : string.Join(", ", Communities);
            }
        }
    }
}
