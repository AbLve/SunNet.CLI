using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnet.Cli.Core.Ade
{
    public enum BlackWhiteStyle : byte
    {
        Underline = 1,

        Bold = 2,

        Italics = 3,

        Asterisks = 4,

        [Description("Font Increase")]
        Font_Increase = 5,

        Triangle = 6,

        Rectangle = 7,

        Square = 8,

        Hexagon = 9
    }
}
