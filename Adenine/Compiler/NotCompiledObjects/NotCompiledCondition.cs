using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.NotCompiledObjects
{
    internal struct NotCompiledCondition
    {
        public bool IsInverted { get; private set; }

        public Token ProteinName { get; private set; }

        public ComparisonOperator Operator { get; private set; }

        public float? Value { get; private set; }

        public Token? ComparingVariable { get; private set; }

        public LogicOperator LogicOperator { get; private set; }

        public NotCompiledCondition(bool isInverted, Token proteinName, ComparisonOperator comparisonOperator, float value, LogicOperator logicOperator)
        {
            IsInverted = isInverted;
            ProteinName = proteinName;
            Operator = comparisonOperator;
            Value = value;
            ComparingVariable = null;
            LogicOperator = logicOperator;
        }

        public NotCompiledCondition(bool isInverted, Token proteinName, ComparisonOperator comparisonOperator, Token comparingVariable, LogicOperator logicOperator)
        {
            IsInverted = isInverted;
            ProteinName = proteinName;
            Operator = comparisonOperator;
            Value = null;
            ComparingVariable = comparingVariable;
            LogicOperator = logicOperator;
        }

        public override string ToString()
        {
            if (ComparingVariable == null && Value.HasValue)
            {
                return $"{(IsInverted ? "!(" : "")}{ProteinName.Text} " +
                       $"{ComparisonOperatorParser.ToString(Operator)} {Value}{(IsInverted ? ")" : "")}" +
                       $"{(LogicOperator != LogicOperator.None ? " " + LogicOperator.ToString().ToLower() : "")}";
            }

            else if (ComparingVariable != null && Value == null)
            {
                return $"{(IsInverted ? "!(" : "")}{ProteinName.Text} " +
                       $"{ComparisonOperatorParser.ToString(Operator)} {ComparingVariable.Value.Text}{(IsInverted ? ")" : "")}" +
                       $"{(LogicOperator != LogicOperator.None ? " " + LogicOperator.ToString().ToLower() : "")}";
            }

            return "error";
        }
    }
}
