using Adenine.CodeObjects;
using Adenine.Compiler.Errors;
using Adenine.Compiler.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler
{
    internal static class AdenineCompiler
    {
        public static List<Error> Compile(string code)
        {
            List<Error> errors = new();

            List<Line> lines = SplitLines(code);
            List<Token> tokens = SplitTokens(lines);

            List<TokenTreeObject> tokenTree = CreateTokensTree(tokens);

            Logging.SaveTokenTree(tokenTree);

            CheckTree(tokenTree, out List<Error> treeErrors);
            errors = errors.Concat(treeErrors).ToList();

            if (errors.Count > 0) return errors;

            DNA dna = AssemblyDNA(tokenTree, out List<Error> assemblyErrors);
            errors = errors.Concat(assemblyErrors).ToList();

            return errors;
        }

        #region Code Splits

        private static List<Line> SplitLines(string code)
        {
            string[] rawLines = (code + "\r").Split("\n");
            List<Line> lines = new List<Line>();

            for (int i = 0; i < rawLines.Length; i++)
            {
                lines.Add(new Line(rawLines[i], i + 1));
            }

            return lines;
        }

        private static List<Token> SplitTokens(List<Line> lines)
        {
            List<Token> tokens = new List<Token>();

            foreach (Line line in lines)
            {
                StringBuilder stringBuilder = new();

                int commentIndex = line.Text.IndexOf("//");

                int processTo = line.Text.Length;

                if (commentIndex > -1)
                    processTo = commentIndex + 1;

                for (int i = 0; i < processTo; i++)
                {
                    char symbol = line.Text[i];

                    if (IsNotSpace(symbol) && i < processTo - 1)
                    {
                        stringBuilder.Append(symbol);
                    }

                    else if (stringBuilder.Length > 0)
                    {
                        tokens.Add(new Token(stringBuilder.ToString(), line.Number));
                        stringBuilder.Clear();
                    }
                }
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                Token token = tokens[i];

                if (token.Text.Length > 1)
                {
                    for (int j = 0; j < token.Text.Length; j++)
                    {
                        char symbol = token.Text[j];

                        if (symbol == '{' || symbol == '}' ||
                            symbol == '(' || symbol == ')' /*|| symbol == '\''*/)
                        {
                            string split = token.Text.Remove(0, j + 1);

                            token.Text = token.Text.Remove(j, token.Text.Length - j);
                            tokens[i] = token;
                            tokens.Insert(i + 1, new Token(symbol.ToString(), token.LineNumber));
                            i++;

                            if (split.Length > 0)
                            {
                                tokens.Insert(i + 1, new Token(split, token.LineNumber));
                            }                        
                        }
                    }
                }
            }

            for (int i = 0; i < tokens.Count; i++)
            {
                if (string.IsNullOrEmpty(tokens[i].Text))
                {
                    tokens.RemoveAt(i);
                }
            }

            return tokens;
        }

        private static List<TokenTreeObject> CreateTokensTree(List<Token> tokens)
        {
            List<TokenTreeObject> tokenTree = new();

            List<TokenTreeObject> currentBranch = tokenTree;

            int braceCount = 0;
            //int bracketCount = 0;

            Stack<List<TokenTreeObject>> stack = new();
            //stack.Push(currentBranch);

            foreach (Token token in tokens)
            {
                if (token.Text == "{" || token.Text == "(")
                {
                    stack.Push(currentBranch);
                    currentBranch = currentBranch[currentBranch.Count - 1].Branch;
                    braceCount++;
                }

                currentBranch.Add(new(token));

                if (token.Text == "}" || token.Text == ")")
                {
                    if (braceCount <= 0) { }

                    currentBranch = stack.Pop();
                    braceCount--;
                }
            }

            return tokenTree;
        }

        #endregion

        private static void CheckTree(List<TokenTreeObject> tokenTree, out List<Error> errors)
        {
            errors = new();

            List<TokenTreeObject> currentBranch = tokenTree;
            Stack<(List<TokenTreeObject> branch, int lastIndex)> stack = new();

            int branchDeep = 0;

            int startIndex = 0;
            bool needExit = false;

            while (!needExit)
            {
                if (startIndex >= currentBranch.Count)
                    break;

                if (currentBranch.Count == 1)
                {
                    errors.Add(new NotEndBracketError(currentBranch[0].Token.LineNumber));
                }

                else if (currentBranch.Count >= 2)
                {
                    Token start = currentBranch[0].Token;
                    Token end = currentBranch[currentBranch.Count - 1].Token;

                    if ((start.Text == "{" && end.Text != "}") ||
                        (start.Text == "(" && end.Text != ")")) 
                    {
                        errors.Add(new WrongBracketError(end.LineNumber));
                    }
                }

                for (int i = startIndex; i < currentBranch.Count; i++)
                {
                    startIndex = 0;

                    var treeToken = currentBranch[i];
                    Token token = treeToken.Token;

                    if (token.Text == ReservedNames.Gen)
                    {
                        if (branchDeep != 0)
                        {
                            errors.Add(new NotAvailableInContextError(token.LineNumber));
                        }
                    }

                    else if (token.Text == ReservedNames.Condition ||
                             token.Text == ReservedNames.Result)
                    {
                        if (branchDeep != 1)
                        {
                            errors.Add(new NotAvailableInContextError(token.LineNumber));
                        }
                    }

                    //Check, all why not equals gen, condition, result
                    else if (ReservedNames.NameExist(token.Text)) 
                    {
                        if (branchDeep != 2)
                        {
                            errors.Add(new NotAvailableInContextError(token.LineNumber));
                        }
                    }

                    if (treeToken.Branch.Count > 0)
                    {
                        stack.Push((currentBranch, i));
                        currentBranch = treeToken.Branch;
                        branchDeep++;

                        break;
                    }

                    else if (i == currentBranch.Count - 1)
                    {
                        if (stack.Count <= 0)
                            needExit = true;

                        else
                        {
                            var oldBranch = stack.Pop();
                            currentBranch = oldBranch.branch;
                            startIndex = oldBranch.lastIndex + 1;
                            branchDeep--;
                        }

                        break;
                    }
                }
            }

            return;
        }

        private static DNA AssemblyDNA(List<TokenTreeObject> tokenTree, out List<Error> errors)
        {
            errors = new();

            List<Token> genNames = new();
            List<List<Token>> genConditions = new();
            List<List<Token>> genResults = new();

            List<TokenTreeObject> currentBranch = tokenTree;
            Stack<(List<TokenTreeObject> branch, int lastIndex)> stack = new();

            int branchDeep = 0;

            int startIndex = 0;
            bool needExit = false;

            bool genDefine = false;

            bool conditionDefine = false;
            bool resultDefine = false;

            bool conditionHasEnd = false;
            bool resultHasEnd = false;

            while (!needExit)
            {
                if (startIndex >= currentBranch.Count)
                    break;

                bool genDefineStart = false;

                for (int i = startIndex; i < currentBranch.Count; i++)
                {
                    startIndex = 0;

                    var treeToken = currentBranch[i];
                    Token token = treeToken.Token;

                    if (genDefineStart)
                    {
                        if (!ReservedNames.NameExist(token.Text) && !ReservedSymbols.SymbolExist(token.Text))
                        {
                            if (treeToken.Branch.Count >= 2)
                            {
                                genNames.Add(token);
                                genDefine = true;
                            }

                            else
                            {
                                errors.Add(new NotDefinedBodyError(token.LineNumber));
                            }
                        }

                        else
                        {
                            errors.Add(new ReservedNameError(token.LineNumber));
                        }
                    }

                    if (conditionDefine)
                    {
                        genConditions[genConditions.Count - 1].Add(token);
                    }

                    if (resultDefine)
                    {
                        genResults[genResults.Count - 1].Add(token);
                    }

                    if (token.Text == ReservedNames.Gen)
                    {
                        if (!genDefineStart)
                            genDefineStart = true;
                    }

                    if (token.Text == ReservedNames.Condition)
                    {
                        if (!conditionDefine)
                        {
                            conditionDefine = true;
                            genConditions.Add(new());
                        }
                    }

                    if (token.Text == ReservedNames.Result)
                    {
                        if (!resultDefine)
                        {
                            resultDefine = true;
                            genResults.Add(new());
                        }
                    }

                    if (treeToken.Branch.Count > 0)
                    {
                        stack.Push((currentBranch, i));
                        currentBranch = treeToken.Branch;
                        branchDeep++;

                        break;
                    }

                    else if (i == currentBranch.Count - 1)
                    {
                        if (stack.Count <= 0)
                            needExit = true;

                        else
                        {
                            var oldBranch = stack.Pop();
                            currentBranch = oldBranch.branch;
                            startIndex = oldBranch.lastIndex + 1;
                            branchDeep--;

                            if (genDefine && branchDeep <= 0)
                            {
                                genDefine = false;

                                if (!conditionHasEnd)
                                {
                                    errors.Add(new NotDefineConditionError(token.LineNumber));
                                }

                                if (!resultHasEnd)
                                {
                                    errors.Add(new NotDefineResultError(token.LineNumber));
                                }

                                conditionHasEnd = false;
                                resultHasEnd = false;
                            }

                            if (conditionDefine && branchDeep <= 1)
                            {
                                conditionDefine = false;
                                conditionHasEnd = true;
                            }

                            if (resultDefine && branchDeep <= 1)
                            {
                                resultDefine = false;
                                resultHasEnd = true;
                            }
                        }

                        break;
                    }
                }
            }

            if (errors.Count > 0) return null;

            List<Gen> genes = ProcessGens(genNames, genConditions, genResults, out int proteinCount, out List<Error> processErrors);
            errors = errors.Concat(processErrors).ToList();

            return null;
        }

        private static List<Gen> ProcessGens(List<Token> genNames, List<List<Token>> genConditions, List<List<Token>> genResults, out int proteinCounts, out List<Error> errors)
        {
            List<Token> notRealizedProteinNames = new();
            List<Token> proteinNames = new();

            Dictionary<string, List<NotCompiledCondition>> conditions = new();

            errors = new();

            for (int i = 0; i < genConditions.Count; i++)
            {
                List<Token> tokens = genConditions[i];

                conditions.Add(genNames[i].Text, new());

                if (tokens[0].Text == "{" && tokens[tokens.Count - 1].Text == "}")
                {
                    bool invert = false;
                    bool exist = false;

                    string protein = null;

                    ComparisonOperator? operation = null;
                    float? value = null;

                    LogicOperator logicOperator = LogicOperator.None;

                    for (int j = 1; j < tokens.Count - 1; j++)
                    {
                        Token token = tokens[j];
                        if (token.Text == ReservedNames.Not)
                        {
                            if (exist || protein != null || invert)
                            {
                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                            }

                            else invert = true;
                        }

                        else if (token.Text == ReservedNames.Exist)
                        {
                            if (exist || protein != null)
                            {
                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                            }

                            else exist = true;
                        }

                        else if (protein != null)
                        {
                            if (ComparisonOperatorParser.TryParse(token.Text, out ComparisonOperator? parsedOperator))
                            {
                                if (exist || protein == null || operation != null)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else
                                {
                                    if (parsedOperator.HasValue) operation = parsedOperator;

                                    else throw new Exception();
                                }
                            }

                            else if (float.TryParse(token.Text, out float parsedValue))
                            {
                                if (exist || protein == null || operation == null)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else value = parsedValue;
                            }

                            else if (token.Text == ReservedNames.And || token.Text == ReservedNames.Or)
                            {
                                if ((!exist && (protein == null || operation == null || value == null)) ||
                                    (exist && (protein == null || operation != null || value == null)) ||
                                    logicOperator != LogicOperator.None)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else logicOperator = token.Text == ReservedNames.And ? LogicOperator.And : LogicOperator.Or;
                            }


                            else if (operation != null &&
                                    !ReservedNames.NameExist(token.Text) &&
                                    !ReservedSymbols.SymbolExist(token.Text))
                            {
                                if (exist || protein == null)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else value = parsedValue;
                            }
                        }

                        else
                        {
                            if (!ReservedNames.NameExist(token.Text) &&
                                !ReservedSymbols.SymbolExist(token.Text))
                            {
                                if (protein != null)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else protein = token.Text;
                            }

                            else errors.Add(new ReservedNameError(token.LineNumber));
                        }

                        bool isEnd = j >= tokens.Count - 2;

                        if ((isEnd || logicOperator != LogicOperator.None) && errors.Count <= 0)
                        {
                            if (isEnd && logicOperator != LogicOperator.None)
                            {
                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                            }

                            else if (!isEnd && logicOperator == LogicOperator.None)
                            {
                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                            }

                            if (protein == null)
                            {
                                errors.Add(new NeedProteinNameError(token.LineNumber));
                            }

                            else
                            {
                                if (exist)
                                {
                                    if (protein == null)
                                    {
                                        throw new Exception();
                                    }

                                    NotCompiledCondition condition = new
                                    (
                                        invert,
                                        protein,
                                        ComparisonOperator.More,
                                        0f,
                                        logicOperator
                                    );

                                    conditions[genNames[i].Text].Add(condition);
                                }

                                else
                                {
                                    if (protein == null || conditions == null || value == null)
                                    {
                                        throw new Exception();
                                    }

                                    NotCompiledCondition condition = new
                                    (
                                        invert,
                                        protein,
                                        operation.Value,
                                        value.Value,
                                        logicOperator
                                    );

                                    conditions[genNames[i].Text].Add(condition);
                                }
                            }

                            invert = false;
                            exist = false;
                            protein = null;
                            operation = null;
                            value = null;
                            logicOperator = LogicOperator.None;
                        }
                    }
                }

                else if (tokens[0].Text != "{") 
                    errors.Add(new NeedBraceError(tokens[0].LineNumber));

                else if (tokens[tokens.Count - 1].Text != "}")
                    errors.Add(new NeedBraceError(tokens[tokens.Count - 1].LineNumber));
            }

            proteinCounts = 0;
            return null;
        }

        private static bool IsNotSpace(char symbol)
        {
            return symbol != '\r' && symbol != '\n' &&
                   symbol != '\t' && symbol != '\v' &&
                   symbol != '\a' && symbol != '\b' &&
                   symbol != '\f' && symbol != '\0' &&
                   symbol != ' ';
        }
    }
}
