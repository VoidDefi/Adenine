namespace Adenine.CodeObjects
{
    internal enum ComparisonOperator
    {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual
    }

    internal static class ComparisonOperatorParser
    {
        public static string ToString(ComparisonOperator comparisonOperator)
        {
            string operation = "";

            switch (comparisonOperator)
            {
                case ComparisonOperator.Equal:
                    operation = "==";
                    break;
                case ComparisonOperator.NotEqual:
                    operation = "!=";
                    break;
                case ComparisonOperator.Greater:
                    operation = ">";
                    break;
                case ComparisonOperator.Less:
                    operation = "<";
                    break;
                case ComparisonOperator.GreaterOrEqual:
                    operation = ">=";
                    break;
                case ComparisonOperator.LessOrEqual:
                    operation = "<=";
                    break;
            }

            return operation;
        }

        public static bool TryParse(string? token, out ComparisonOperator? operation)
        {
            switch (token)
            {
                case "==":
                    operation = ComparisonOperator.Equal;
                    break;
                case "!=":
                    operation = ComparisonOperator.NotEqual;
                    break;
                case ">":
                    operation = ComparisonOperator.Greater;
                    break;
                case "<":
                    operation = ComparisonOperator.Less;
                    break;
                case ">=":
                    operation = ComparisonOperator.GreaterOrEqual;
                    break;
                case "<=":
                    operation = ComparisonOperator.LessOrEqual;
                    break;
                default:
                    operation = null;
                    return false;
            }

            return true;
        }
    }
}
