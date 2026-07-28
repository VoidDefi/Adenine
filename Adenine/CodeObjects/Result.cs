using Adenine.Compiler.NotCompiledObjects;
using Adenine.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal struct Result
    {
        public ProteinOperation Operation { get; private set; }

        public bool Action { get; private set; }

        public int ProteinIndex { get; private set; }

        public float Value { get; private set; }

        public int InputProtein { get; private set; } = -1;

        public bool UseProteinValue => InputProtein != -1;

        public Result(ProteinOperation operation, bool action, int proteinIndex, float value)
        {
            Operation = operation;
            Action = action;
            ProteinIndex = proteinIndex;
            Value = value;
            InputProtein = -1;
        }

        public Result(ProteinOperation operation, bool action, int proteinIndex, int inputProtein)
        {
            Operation = operation;
            Action = action;
            ProteinIndex = proteinIndex;
            Value = 0;
            InputProtein = inputProtein;
        }

        public override string ToString()
        {
            string operation = ProteinOperationParser.ToString(Operation);
            string action = Action ? "action " : "";

            if (UseProteinValue)
            {
                return $"{operation} {action}p#{ProteinIndex}(p#{InputProtein})";
            }

            return $"{operation} {action}p#{ProteinIndex}({Value})";
        }
    }
}
