using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler
{
    internal struct NotCompiledCondition
    {
        public bool IsInverted { get; private set; }

        public string ProteinName { get; private set; }

        public ComparisonOperator Operator { get; private set; }

        public float Value { get; private set; }

        public LogicOperator LogicOperator { get; private set; }

        public NotCompiledCondition(bool isInverted, string proteinName, ComparisonOperator comparisonOperator, float value, LogicOperator logicOperator)
        {
            IsInverted = isInverted;
            ProteinName = proteinName;
            Operator = comparisonOperator;
            Value = value;
            LogicOperator = logicOperator;
        }

        public override string ToString()
        {
            return $"{(IsInverted ? "!(" : "")}{ProteinName} " +
                   $"{ComparisonOperatorParser.ToString(Operator)} {Value}{(IsInverted ? ")" : "")}" +
                   $"{(LogicOperator != LogicOperator.None ? " " + LogicOperator.ToString().ToLower() : "")}";
        }
    }
}
