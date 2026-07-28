using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal struct Condition
    {
        public int ProteinIndex { get; private set; }

        public ComparisonOperator Operator { get; private set; }

        public float Value { get; private set; }

        public int ComparingProtein { get; private set; } = -1;

        public LogicOperator LogicOperator { get; private set; }

        public bool UseProteinValue => ComparingProtein != -1;

        public Condition(int proteinIndex, ComparisonOperator comparisonOperator, float value, LogicOperator logicOperator)
        {
            ProteinIndex = proteinIndex;
            Operator = comparisonOperator;
            Value = value;
            ComparingProtein = -1;
            LogicOperator = logicOperator;
        }

        public Condition(int proteinIndex, ComparisonOperator comparisonOperator, int comparingProtein, LogicOperator logicOperator)
        {
            ProteinIndex = proteinIndex;
            Operator = comparisonOperator;
            Value = 0;
            ComparingProtein = comparingProtein;
            LogicOperator = logicOperator;
        }

        public override string ToString()
        {
            string logic = LogicOperator != LogicOperator.None ? " " + LogicOperator.ToString().ToLower() : "";

            if (UseProteinValue)
            {
                return $"p#{ProteinIndex} {ComparisonOperatorParser.ToString(Operator)} p#{ComparingProtein}{logic}";
            }

            return $"p#{ProteinIndex} {ComparisonOperatorParser.ToString(Operator)} {Value}{logic}";
        }
    }
}
