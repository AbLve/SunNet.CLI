using Sunnet.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Business.BUP
{
    public class HelpSolve
    {
        private int _index;
        private int _row;
        private DataRowCollection _rows;

        public HelpSolve(DataRowCollection rows, int i)
        {
            _index = 0;
            _row = i;
            _rows = rows;
        }

        public string NextData()
        {
            string v = GetData(_rows[_row][_index]);
            _index++;
            return v;
        }

        private string GetData(object o)
        {
            if (o is DBNull) return string.Empty;
            return o.ToString().Trim();
        }
    }

    public class GetColumnData
    {
        private int _index;
        private DataTable dt;

        public GetColumnData(DataTable dataTabl)
        {
            _index = 0;
            dt = dataTabl;
        }

        public string NextColumn()
        {
            string v = GetData(dt.Columns[_index].ColumnName);
            _index++;
            return v;
        }

        private string GetData(object o)
        {
            if (o is DBNull) return string.Empty;
            return o.ToString().Trim();
        }
    }

}
