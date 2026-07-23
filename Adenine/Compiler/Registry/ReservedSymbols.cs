using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Registry
{
    internal class ReservedSymbols : BaseRegistry
    {
        private readonly static ReservedSymbols Instance = new();

        public static string OpenBrace => "{";

        public static string CloseBrace => "}";

        public static string OpenParenthesis => "(";

        public static string CloseParenthesis => ")";

        public static string More => ">";

        public static string Less => "<";

        public static string MoreOrEqual => ">=";

        public static string LessOrEqual => "<=";

        public static string Equal => "==";

        public static string NotEqual => "!=";

        public static string[] Registry { get; private set; } = [];

        public static void SetupRegistry()
        {
            Instance.CreateRegistry();
        }

        public static bool SymbolExist(string name)
        {
            foreach (var value in Registry)
            {
                if (name == value) return true;
            }

            return false;
        }
    }
}
