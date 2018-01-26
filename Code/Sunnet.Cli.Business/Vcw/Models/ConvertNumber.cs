using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using Sunnet.Cli.Core.Vcw.Entities;
using System.Data.Objects.SqlClient;


namespace Sunnet.Cli.Business.Vcw.Models
{
    /// <summary>
    /// 将ID 转换为固定格式的No.
    /// </summary>
    public class ConvertNumber
    {
        /// <summary>
        /// 将ID 转换为固定格式的No.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ConverIDToNumber(int id)
        {
            return NumBefore + id.ToString().PadLeft(NumCount, NumChar);
        }

        public static string NumBefore { get { return "ECT"; } }

        public static int NumCount { get { return 6; } }

        public static string NumStr { get { return "0"; } }

        public static char NumChar { get { return '0'; } }
    }
}
