using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.NotCompiledObjects
{
    internal struct NotCompiledResult
    {
        public ProteinOperation Operation { get; private set; }

        public bool Action { get; private set; }

        public Token ProteinName { get; private set; }

        public float? Value { get; private set; } = null;

        public Token? InputName { get; private set; } = null;

        public bool GetValueFrom { get; private set; } = false;

        public NameTranslateMode? TranslateMode { get; private set; } = null;

        public NotCompiledResult(ProteinOperation operation, bool action, Token proteinName, float value)
        {
            Operation = operation;
            Action = action;
            ProteinName = proteinName;
            Value = value;
            InputName = null;
            TranslateMode = null;
            GetValueFrom = false;
        }

        public NotCompiledResult(ProteinOperation operation, bool action, Token proteinName, Token inputName, NameTranslateMode? translateMode, bool getValueFrom)
        {
            Operation = operation;
            Action = action;
            ProteinName = proteinName;
            Value = null;
            InputName = inputName;
            TranslateMode = translateMode;
            GetValueFrom = getValueFrom;
        }

        public override string ToString()
        {
            string getValueFrom = GetValueFrom ? " valuefrom" : "";

            if (InputName == null && TranslateMode == null && Value != null && !GetValueFrom)
            {
                string action = Action ? "action " : "";
                string operation = ProteinOperationParser.ToString(Operation);

                return $"{operation} {action}{ProteinName.Text}({Value})";
            }

            else if (Value == null && InputName != null)
            {
                string action = Action ? "action " : "";
                string mode = TranslateMode != null ? " " + TranslateMode.Value.ToString().ToLower() : "";

                string operation = ProteinOperationParser.ToString(Operation);

                if (getValueFrom != "" && mode != "")
                {
                    return "error";
                }

                return $"{operation} {action}{ProteinName.Text}({InputName.Value.Text}{mode}{getValueFrom})";
            }

            return "error";
        }
    }
}
