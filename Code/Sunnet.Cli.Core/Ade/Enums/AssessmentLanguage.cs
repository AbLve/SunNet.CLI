using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/11 3:47:30
 * Description:		Please input class summary
 * Version History:	Created,2014/9/11 3:47:30
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Ade
{
    public static class AssessmentLanguageHelper
    {
        public static string GetButtonText(AssessmentLanguage language)
        {
            return "Click to Assess in " + language.ToDescription();
        }
    }

    public enum AssessmentLanguage : byte
    {
        [Description("English")]
        English = 1,
        [Description("Spanish")]
        Spanish = 2
    }

    public enum StudentAssessmentLanguage : byte
    {
        English = 1,

        Spanish = 2,

        /// <summary>
        /// Both English and Spanish
        /// </summary>
        [Description("En and Sp")]
        Bilingual = 3,

        [Description("Non Applicable")]
        NonApplicable = 4
    }
}
