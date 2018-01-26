/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason
 * CreatedOn:		2014/11/28 14:53:10
 * Description:		For Cec Answer
 * Version History:	Created,2014/11/28 14:53:10
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Business.Cec.Model
{
    public class CecAnswerModel
    {
        public int AnswerId { get; set; }

        public int Sort { get; set; }

        public string Text { get; set; }

        public decimal Score { get; set; }
        
        /// <summary>
        /// 是否选中状态
        /// </summary>
        public bool Selected { get; set; }

    }
}
