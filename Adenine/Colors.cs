using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine
{
    internal static class Colors
    {
        public static string Red => "\x1b[38;2;255;0;0m";

        public static string DarkRed => "\x1b[38;2;128;0;0m";

        public static string Gray => "\x1b[38;2;128;128;128m";

        public static string Green => "\x1b[38;2;0;255;0m";

        public static string DarkGray => "\x1b[38;2;64;64;64m";

        public static string Cyan => "\x1b[38;2;0;256;256m";

        public static string Yellow => "\x1b[38;2;256;256;0m";

        public static string DarkCyan => "\x1b[38;2;0;128;128m";

        public static string Reset => "\x1b[0m";
    }
}
