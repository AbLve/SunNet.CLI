using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Export.Enums
{
    /// <summary>
    /// FieldMap表中数据对应各个表的字段，枚举出最大值（例：ID=0~100对应Communities表中字段）
    /// </summary>
    public enum RowID
    {
        Communities = 100,

        Schools = 200,

        Classrooms = 300,

        Classes = 400,

        Students = 500,

        Users = 600,

        CommunityUsers = 700,

        StateWides = 800,

        Principals = 900,

        Teachers = 1000,

        Parents = 1100,

        Auditors = 1200,

        CoordCoachs = 1300,

        VideoCodings = 1400
    }
}
