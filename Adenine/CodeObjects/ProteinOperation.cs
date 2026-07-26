using Adenine.Compiler.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal enum ProteinOperation
    {
        Set,
        Add,
        Subtract,
        Multiply,
        Divide,
        DivideByModule
    }

    internal static class ProteinOperationParser
    {
        public static string ToString(ProteinOperation operation)
        {
            switch (operation)
            {
                case ProteinOperation.Set:
                    return ReservedNames.Set;
                case ProteinOperation.Add:
                    return ReservedNames.Add;
                case ProteinOperation.Subtract:
                    return ReservedNames.Sub;
                case ProteinOperation.Multiply:
                    return ReservedNames.Mul;
                case ProteinOperation.Divide:
                    return ReservedNames.Div;
                case ProteinOperation.DivideByModule:
                    return ReservedNames.Mod;
            }

            return null;
        }

        public static bool TryParse(string token, out ProteinOperation? operation)
        {
            operation = null;

            if (token == ReservedNames.Set)
                operation = ProteinOperation.Set;
            if (token == ReservedNames.Add)
                operation = ProteinOperation.Add;
            if (token == ReservedNames.Sub)
                operation = ProteinOperation.Subtract;
            if (token == ReservedNames.Mul)
                operation = ProteinOperation.Multiply;
            if (token == ReservedNames.Div)
                operation = ProteinOperation.Divide;
            if (token == ReservedNames.Mod)
                operation = ProteinOperation.DivideByModule;

            return operation != null;
        }
    }
}
