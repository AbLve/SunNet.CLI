using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Trs.Enums
{
    public enum TrsAccreditation : byte
    {
        [Description("National Association for the Education of Young Children ( NAEYC )")]
        NAEYC = 1,

        [Description("National Early Childhood Program Accreditation ( NECPA )")]
        NECPA = 2,

        [Description("National Association of Family Child Care ( NAFCC )")]
        NAFCC = 3,

        [Description("U.S. Military")]
        USMilitary = 4,

        [Description("Council of Accreditation - National After School Association ( COA )")]
        COA = 5,

        [Description("National Accreditation Commission for Early Child Care and Education ( NAC )")]
        NAC = 6,

        [Description("Association of Chistion Schools International ( ACSI )")]
        ACSI = 7,

        [Description("AdvancED Quality Early Learning Standards QELS ( QELS )")]
        QELS = 8
    }
}
