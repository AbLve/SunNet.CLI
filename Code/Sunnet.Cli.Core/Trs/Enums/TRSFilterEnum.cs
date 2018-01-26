using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs
{
    public enum TRSFilterEnum : byte
    {
        /// <summary>
        /// 针对所有School，所有Classroom全部显示
        /// </summary>
        None = 1,

        // ReSharper disable once InconsistentNaming
        School_Type_LC_AA = 10,
        // ReSharper disable once InconsistentNaming
        School_Type_LC_CH = 11,
        // ReSharper disable once InconsistentNaming
        School_Type_LC_SA = 12,

        // ReSharper disable once InconsistentNaming
        School_SchoolAgeOnly = 20,
        // ReSharper disable once InconsistentNaming
        School_Not_SchoolAgeOnly = 21,

        // ReSharper disable once InconsistentNaming
        Classroom_00_11_Ms = 31,
        // ReSharper disable once InconsistentNaming
        Classroom_12_17_Ms = 32,
        // ReSharper disable once InconsistentNaming
        Classroom_18_23_Ms = 33,
        // ReSharper disable once InconsistentNaming
        Classroom_Age_2_Ys = 34,
        // ReSharper disable once InconsistentNaming
        Classroom_Age_3_Ys = 35,
        // ReSharper disable once InconsistentNaming
        Classroom_Age_4_Ys = 36,
        // ReSharper disable once InconsistentNaming
        Classroom_Age_5_Ys = 37,
        // ReSharper disable once InconsistentNaming
        Classroom_Age_6_8_Ys = 38,
        // ReSharper disable once InconsistentNaming
        Classroom_Age_9_12_Ys = 39,

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Category3, Cagegory4
        /// </summary>
        Classroom_Age_Area = 50

    }
}

