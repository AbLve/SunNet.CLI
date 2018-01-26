using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/12 3:39:27
 * Description:		Please input class summary
 * Version History:	Created,2014/8/12 3:39:27
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Core.Ade
{
    // 修改时注意修改:Sunnet.Cli.Static\Content\Scripts_cliProject\Module_Cpalls.js
    public enum AnswerType : byte
    {
        /// <summary>
        /// Cot
        /// </summary>
        None = 1,
        /// <summary>
        /// Rapid Expressive
        /// </summary>
        YesNo = 2,
        /// <summary>
        /// PA
        /// </summary>
        PaText = 3,
        /// <summary>
        /// Quadrant,Multiple choices,Receptive with promp,Receptive without prompt
        /// </summary>
        PictureAudio = 4,
        /// <summary>
        /// Cec,Checklist
        /// </summary>
        Cec = 5,

        TypedResponse = 6,

        /// <summary>
        /// Txkea Expressive TypedResponse
        /// </summary>
        TxkeaTypedResponse = 7
    }
}
