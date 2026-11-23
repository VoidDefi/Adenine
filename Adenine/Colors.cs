using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine
{
    internal static class Colors
    {
        public static string Gray => "\x1b[38;2;128;128;128m";

        public static string Green => "\x1b[38;2;0;256;0m";

        public static string DarkGray => "\x1b[38;2;64;64;64m";

        public static string Reset => "\x1b[0m";
    }
}
