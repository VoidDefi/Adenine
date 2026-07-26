using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Registry
{
    internal class ReservedNames : BaseRegistry
    {
        private readonly static ReservedNames Instance = new();

        public static string Gen => "gen";

        public static string Condition => "condition";

        public static string Result => "result";

        public static string Exist => "exist";

        public static string And => "and";

        public static string Or => "or";

        public static string Not => "not";

        public static string Set => "set";

        public static string Add => "add";

        public static string Sub => "sub";

        public static string Mul => "mul";

        public static string Div => "div";

        public static string Mod => "mod";

        public static string Action => "action";

        public static string GetLink => "getlink";

        public static string GetGen => "getgen";

        public static string Next => "next";

        public static string[] Registry { get; private set; } = [];

        public static void SetupRegistry()
        {
            Instance.CreateRegistry();
        }

        public static bool NameExist(string name)
        {
            foreach (var value in Registry)
            {
                if (name == value) return true;
            }

            return false;
        }
    }
}
