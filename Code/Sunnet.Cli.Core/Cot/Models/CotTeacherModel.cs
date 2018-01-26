/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		2014/12/23 18:17:27
 * Description:		Please input class summary
 * Version History:	Created,2014/12/23 18:17:27
 * 
 * 
 **************************************************************************/

using System;
using System.ComponentModel;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Users.Enums;

namespace Sunnet.Cli.Core.Cot.Models
{
    public class CotTeacherModel : AssessmentTeacherModel
    {

        [DisplayName("COT BOY")]
        public DateTime? CotWave1 { get; set; }

        [DisplayName("COT MOY")]
        public DateTime? CotWave2 { get; set; }

        public CotWaveStatus? CotWaveStatus1 { get; set; }

        public CotWaveStatus? CotWaveStatus2 { get; set; }

        public bool HasOldData
        {
            get
            {
                if (CotWaveStatus1 != null)
                {
                    if (CotWaveStatus1 == CotWaveStatus.OldData)
                    {
                        return true;
                    }
                }
                else if (CotWaveStatus2 != null)
                {
                    if (CotWaveStatus2 == CotWaveStatus.OldData)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public int? AssessmentID { get; set; }

        [DisplayName("School Year")]
        public string SchoolYear { get; set; }

        public object Records { get; set; }
    }
}
