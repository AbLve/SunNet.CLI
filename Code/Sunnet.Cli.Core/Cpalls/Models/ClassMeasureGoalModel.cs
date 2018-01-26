using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/9/26 2:32:37
 * Description:		Please input class summary
 * Version History:	Created,2014/9/26 2:32:37
 * 
 * 
 **************************************************************************/
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Core.Cpalls.Models
{
    public class ClassMeasureGoalModel
    {
        private decimal _goal;

        public int SchoolId { get; set; }

        public int MeasureId { get; set; }

        /// <summary>
        /// 总分.
        /// </summary> 
        public decimal Goal
        {
            get
            {
                if (_goal < 0)
                    return 0;
                return _goal;
            }
            set { _goal = value; }
        }

        public int Amount { get; set; }

        public string Average
        {
            get
            {
                if (Amount == 0)
                    return "-";
                return (Goal / Amount).ToPrecisionString(2);
                
            }
        }
    }
}
