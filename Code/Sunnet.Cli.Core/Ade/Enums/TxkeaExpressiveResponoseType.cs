using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade
{
    public  enum TxkeaExpressiveResponoseType:byte
    {

        /// <summary>
        /// Simple Response
        /// </summary>
        [Description("Simple Response")]
        Simple =1,


        /// <summary>
        /// Detailed Response
        /// </summary>
        [Description("Detailed Response")]
        Detailed =2
    }
}
