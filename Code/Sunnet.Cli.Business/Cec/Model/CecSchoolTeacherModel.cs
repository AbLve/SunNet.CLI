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
 * CreatedOn:		7/3/2015 09:40:14
 * Description:		Please input class summary
 * Version History:	Created,7/3/2015 09:40:14
 *
 *
 **************************************************************************/
using Sunnet.Cli.Business.Schools.Models;
using Sunnet.Cli.Core.Cec.Models;


namespace Sunnet.Cli.Business.Cec.Model
{
    public class CecSchoolTeacherModel : CECTeacherModel
    {

        public List<string> SchoolNames { get; set; }

        [DisplayName("School Names")]
        public string SchoolsText
        {
            get
            {
                if (SchoolNames == null)
                    return string.Empty;
                return string.Join(", ", SchoolNames);
            }
        }

        public List<string> CommunityNames { get; set; }
        [DisplayName("Community/District")]
        public string CommunitiesText
        {
            get
            {
                if (CommunityNames == null)
                    return string.Empty;
                return string.Join(", ", CommunityNames);
            }
        }
    }
}
