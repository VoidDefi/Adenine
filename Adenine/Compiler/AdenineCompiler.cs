using Adenine.CodeObjects;
using Adenine.Compiler.Errors;
using Adenine.Compiler.NotCompiledObjects;
using Adenine.Compiler.Registry;
using System.Text;

namespace Adenine.Compiler
{
    internal static class AdenineCompiler
    {
        public static Cell Compile(string code, bool loggingTokens, out DebugData debugData, out List<Error> errors)
        {
            debugData = null;
            errors = new();

            List<Line> lines = SplitLines(code);
            List<Token> tokens = SplitTokens(lines);

            List<TokenTreeObject> tokenTree = CreateTokensTree(tokens);

            if (loggingTokens)
                Logging.SaveTokenTree(tokenTree);

            CheckTree(tokenTree, out List<Error> treeErrors);
            errors = errors.Concat(treeErrors).ToList();

            if (errors.Count > 0) return null;

            Cell cell = AssemblyCell(tokenTree, out debugData, out List<Error> assemblyErrors);
            errors = errors.Concat(assemblyErrors).ToList();

            if (errors.Count > 0) return null;

            return cell;
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
                        if (branchDeep < 2)
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

        private static Cell AssemblyCell(List<TokenTreeObject> tokenTree, out DebugData debugData, out List<Error> errors)
        {
            debugData = null;
            errors = new();

            List<Token> genNames = new();
            List<List<Token>> genConditions = new();
            List<List<TokenTreeObject>> genResults = new();

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

            int conditionCount = 0;
            int resultCount = 0;

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
                        //genResults[genResults.Count - 1].Add(token);
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
                            genResults.Add(treeToken.Branch);
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

                                if (conditionCount > 1 || resultCount > 1)
                                {
                                    errors.Add(new TooMuchResultsOrConditionsError(token.LineNumber));
                                }

                                conditionHasEnd = false;
                                resultHasEnd = false;

                                conditionCount = 0;
                                resultCount = 0;
                            }

                            if (conditionDefine && branchDeep <= 1)
                            {
                                conditionDefine = false;
                                conditionHasEnd = true;

                                conditionCount++;
                            }

                            if (resultDefine && branchDeep <= 1)
                            {
                                resultDefine = false;
                                resultHasEnd = true;

                                resultCount++;
                            }
                        }

                        break;
                    }
                }
            }

            if (errors.Count > 0) return null;

            ProcessConditionsAndResults
            (
                genNames,
                genConditions,
                genResults,
                out Dictionary<string, List<NotCompiledCondition>> conditions,
                out Dictionary<string, List<NotCompiledResult>> results,
                out List<Error> processErrors
            );

            errors = errors.Concat(processErrors).ToList();

            if (errors.Count > 0)
                return null;

            if (conditions.Count != results.Count)
                throw new Exception();

            if (conditions.Count != genNames.Count || results.Count != genNames.Count)
                throw new Exception();

            //Check names 

            var functionalProteins = FunctionalProteinsRegistry.Proteins.ToList();

            List<Token> resultProteins = new();
            List<Token> actionsProteins = new();
            List<Token> inputProteins = new();
            List<Token> inputGens = new();

            List<Token> conditionProteins = new();
            List<Token> comperingProteins = new();

            foreach (var resultBlocks in results)
            {
                for (int i = 0; i < resultBlocks.Value.Count; i++)
                {
                    var result = resultBlocks.Value[i];

                    if (result.Action)
                        actionsProteins.Add(result.ProteinName);

                    else resultProteins.Add(result.ProteinName);

                    if (result.InputName != null)
                    {
                        if (result.TranslateMode == NameTranslateMode.GetGen)
                            inputGens.Add(result.InputName.Value);

                        else inputProteins.Add(result.InputName.Value);
                    }
                }
            }

            foreach (var conditionBlocks in conditions)
            {
                for (int i = 0; i < conditionBlocks.Value.Count; i++)
                {
                    var condition = conditionBlocks.Value[i];

                    conditionProteins.Add(condition.ProteinName);

                    if (condition.ComparingVariable.HasValue)
                        comperingProteins.Add(condition.ComparingVariable.Value);
                } 
            }

            foreach (Token token in inputGens)
            {
                if (genNames.FindIndex(g => g.Text == token.Text) < 0)
                {
                    errors.Add(new GenNotExistError(token.LineNumber));
                }
            }

            List<Token> usedProteins = conditionProteins.Concat(comperingProteins.Concat(inputProteins)).ToList();

            foreach (Token token in usedProteins)
            {
                if (resultProteins.FindIndex(p => p.Text == token.Text) < 0)
                {
                    errors.Add(new ProteinNotInitedError(token.LineNumber));
                }
            }

            foreach (Token token in resultProteins)
            {
                if (genNames.FindIndex(g => g.Text == token.Text) >= 0)
                {
                    errors.Add(new GenNameEqualProteinNameError(token.LineNumber));
                }
            }

            foreach (Token token in actionsProteins)
            {
                if (functionalProteins.FindIndex(p => p.Value.Name == token.Text) < 0)
                {
                    errors.Add(new ActionNotExistError(token.LineNumber));
                }
            }

            List<string> proteins = new();

            for (int i = 0; i < resultProteins.Count; i++)
            {
                Token token = resultProteins[i];

                if (proteins.FindIndex(p => p == token.Text) < 0)
                {
                    proteins.Add(token.Text);
                }
            }

            if (errors.Count > 0)
                return null;

            //Assembly cell

            Dictionary<string, List<Condition>> compiledConditions = new();
            Dictionary<string, List<Result>> compiledResults = new();

            //Compiling conditions
            for (int i = 0; i < conditions.Count; i++)
            {
                var conditionBlock = conditions.ElementAt(i);

                compiledConditions.Add(conditionBlock.Key, new());

                for (int j = 0; j < conditionBlock.Value.Count; j++)
                {
                    NotCompiledCondition condition = conditionBlock.Value[j];

                    string proteinName = condition.ProteinName.Text;
                    ComparisonOperator operation = condition.Operator;
                    float? value = condition.Value;
                    string? variable = condition.ComparingVariable?.Text;
                    LogicOperator logicOperator = condition.LogicOperator;

                    int proteinIndex = proteins.IndexOf(proteinName);

                    if (proteinIndex < 0) 
                        throw new Exception();

                    if (condition.IsInverted)
                    {
                        switch (operation)
                        {
                            case ComparisonOperator.Equal:
                                operation = ComparisonOperator.NotEqual;
                                break;

                            case ComparisonOperator.NotEqual:
                                operation = ComparisonOperator.Equal;
                                break;

                            case ComparisonOperator.Greater:
                                operation = ComparisonOperator.LessOrEqual;
                                break;

                            case ComparisonOperator.Less:
                                operation = ComparisonOperator.GreaterOrEqual;
                                break;

                            case ComparisonOperator.LessOrEqual:
                                operation = ComparisonOperator.Greater;
                                break;

                            case ComparisonOperator.GreaterOrEqual:
                                operation = ComparisonOperator.Less;
                                break;
                        }
                    }

                    if (value.HasValue && variable == null)
                    {
                        compiledConditions[conditionBlock.Key].Add
                        (
                            new Condition(proteinIndex, operation, (float)value.Value, logicOperator)
                        );
                    }

                    else if (!value.HasValue && variable != null)
                    {
                        int varIndex = proteins.IndexOf(variable);

                        if (varIndex < 0)
                            throw new Exception();

                        compiledConditions[conditionBlock.Key].Add
                        (
                            new Condition(proteinIndex, operation, (int)varIndex, logicOperator)
                        );
                    }

                    else throw new Exception();
                }
            }

            //Compiling results
            for (int i = 0; i < results.Count; i++)
            {
                var resultBlock = results.ElementAt(i);

                compiledResults.Add(resultBlock.Key, new());

                for (int j = 0; j < resultBlock.Value.Count; j++)
                {
                    NotCompiledResult result = resultBlock.Value[j];

                    ProteinOperation operation = result.Operation;
                    bool action = result.Action;
                    string proteinName = result.ProteinName.Text;
                    float? value = result.Value;
                    string? inputName = result.InputName?.Text;
                    NameTranslateMode? translateMode = result.TranslateMode;

                    int proteinIndex = -1;

                    if (action)
                    {
                        int actionIndex = functionalProteins.FindIndex(p => p.Value.Name == proteinName);

                        if (actionIndex < 0)
                            throw new Exception();

                        var actionProtein = functionalProteins[actionIndex].Value;
                        proteinIndex = actionProtein.Index;
                    }

                    else 
                    {
                       proteinIndex = proteins.IndexOf(proteinName);
                    }

                    if (proteinIndex < 0)
                        throw new Exception();

                    if (value.HasValue && inputName == null)
                    {
                        compiledResults[resultBlock.Key].Add
                        (
                            new Result(operation, action, proteinIndex, (float)value.Value)
                        );
                    }

                    else if(!value.HasValue && inputName != null)
                    {
                        if (translateMode.HasValue)
                        {
                            int index = -1;

                            if (translateMode == NameTranslateMode.GetLink)
                            {
                                index = proteins.IndexOf(proteinName);
                            }

                            else if (translateMode == NameTranslateMode.GetGen)
                            {
                                index = genNames.FindIndex(t => t.Text == inputName);
                            }

                            if (index < 0)
                                throw new Exception();

                            compiledResults[resultBlock.Key].Add
                            (
                                new Result(operation, action, proteinIndex, (float)index)
                            );
                        }

                        else
                        {
                            int varIndex = proteins.IndexOf(inputName);

                            if (varIndex < 0)
                                throw new Exception();

                            compiledResults[resultBlock.Key].Add
                            (
                                new Result(operation, action, proteinIndex, (int)varIndex)
                            );
                        }
                    }

                    else throw new Exception();
                }
            }

            if (compiledResults.Count != compiledConditions.Count)
                throw new Exception();

            if (compiledConditions.Count != genNames.Count)
                throw new Exception();

            List<Gen> gens = new();

            for (int i = 0; i < genNames.Count; i++)
            {
                var conditionBlock = compiledConditions.ElementAt(i);
                var resultBlock = compiledResults.ElementAt(i);

                string genName = genNames[i].Text;

                if (conditionBlock.Key != resultBlock.Key) throw new Exception();
                if (conditionBlock.Key != genName) throw new Exception();

                gens.Add(new Gen(conditionBlock.Value.ToArray(), resultBlock.Value.ToArray()));
            }

            Cell cell = new Cell(gens.ToArray(), proteins.Count);

            string[] debugGenNames = new string[genNames.Count];

            for (int i = 0; i < genNames.Count; i++)
            {
                debugGenNames[i] = genNames[i].Text;
            }

            debugData = new DebugData(cell, proteins.ToArray(), debugGenNames);

            return cell;
        }

        private static void ProcessConditionsAndResults
        (
            List<Token> genNames, 
            List<List<Token>> genConditions, 
            List<List<TokenTreeObject>> genResults, 
            out Dictionary<string, List<NotCompiledCondition>> conditions, 
            out Dictionary<string, List<NotCompiledResult>> results, 
            out List<Error> errors
        )
        {
            conditions = new();
            results = new();

            errors = new();

            for (int i = 0; i < genNames.Count; i++)
            {
                Token gen = genNames[i];

                List<Token> gens = genNames.FindAll(t => t.Text == gen.Text);

                if (gens.Count > 1)
                {
                    foreach (Token name in gens)
                    {
                        errors.Add(new GenExistError(name.LineNumber));
                    }
                }
            }

            if (errors.Count > 0)
                return;

            //Conditions
            for (int i = 0; i < genConditions.Count; i++)
            {
                List<Token> tokens = genConditions[i];

                conditions.Add(genNames[i].Text, new());

                if (tokens[0].Text == "{" && tokens[tokens.Count - 1].Text == "}")
                {
                    bool invert = false;
                    bool exist = false;

                    Token? protein = null;

                    ComparisonOperator? operation = null;
                    float? value = null;

                    Token? variable = null;

                    LogicOperator logicOperator = LogicOperator.None;

                    for (int j = 1; j < tokens.Count - 1; j++)
                    {
                        Token token = tokens[j];
                        if (token.Text == ReservedNames.Not)
                        {
                            if (exist || protein != null || invert || variable != null)
                            {
                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                            }

                            else invert = true;
                        }

                        else if (token.Text == ReservedNames.Exist)
                        {
                            if (exist || protein != null || variable != null)
                            {
                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                            }

                            else exist = true;
                        }

                        else if (protein != null)
                        {
                            if (operation != null &&
                                !ReservedNames.NameExist(token.Text) &&
                                !ReservedSymbols.SymbolExist(token.Text) &&
                                !float.TryParse(token.Text.Replace(',', '.'), out _))
                            {
                                if (exist || protein == null || value != null || variable != null)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else variable = token;
                            }

                            else if (ComparisonOperatorParser.TryParse(token.Text, out ComparisonOperator? parsedOperator))
                            {
                                if (exist || protein == null || operation != null || variable != null)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else
                                {
                                    if (parsedOperator.HasValue) operation = parsedOperator;

                                    else throw new Exception();
                                }
                            }

                            else if (float.TryParse(token.Text.Replace(',', '.'), out float parsedValue))
                            {
                                if (exist || protein == null || operation == null || variable != null)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else value = parsedValue;
                            }

                            else if (token.Text == ReservedNames.And || token.Text == ReservedNames.Or)
                            {
                                if ((!exist && (protein == null || operation == null || value == null)) ||
                                    (exist && (protein == null || operation != null || value != null)) ||
                                    logicOperator != LogicOperator.None)
                                {
                                    //errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                
                                if (exist)
                                {
                                    if (protein == null || operation != null || value != null || variable != null || logicOperator != LogicOperator.None)
                                    {
                                        errors.Add(new NotAvailableInContextError(token.LineNumber));
                                    }
                                }

                                else if (protein == null || operation == null || logicOperator != LogicOperator.None || 
                                        (value == null && variable == null) || (variable != null && value != null))
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else logicOperator = token.Text == ReservedNames.And ? LogicOperator.And : LogicOperator.Or;
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

                                else protein = token;
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
                                        protein.Value,
                                        ComparisonOperator.Greater,
                                        0f,
                                        logicOperator
                                    );

                                    conditions[genNames[i].Text].Add(condition);
                                }

                                else
                                {
                                    if (protein == null || conditions == null || 
                                        (value == null && variable == null) || 
                                        (value != null && variable != null))
                                    {
                                        throw new Exception();
                                    }

                                    NotCompiledCondition? condition = null;

                                    if (value != null)
                                    {
                                        condition = new
                                        (
                                            invert,
                                            protein.Value,
                                            operation.Value,
                                            value.Value,
                                            logicOperator
                                        );
                                    }

                                    else
                                    {
                                        condition = new
                                        (
                                            invert,
                                            protein.Value,
                                            operation.Value,
                                            variable.Value,
                                            logicOperator
                                        );
                                    }

                                    if (condition.HasValue)
                                        conditions[genNames[i].Text].Add(condition.Value);

                                    else throw new Exception();
                                }
                            }

                            invert = false;
                            exist = false;
                            protein = null;
                            operation = null;
                            value = null;
                            variable = null;
                            logicOperator = LogicOperator.None;
                        }
                    }
                }

                else if (tokens[0].Text != "{") 
                    errors.Add(new NeedBraceError(tokens[0].LineNumber));

                else if (tokens[tokens.Count - 1].Text != "}")
                    errors.Add(new NeedBraceError(tokens[tokens.Count - 1].LineNumber));
            }

            //Results
            for (int i = 0; i < genResults.Count; i++)
            {
                results.Add(genNames[i].Text, new());

                List<TokenTreeObject> currentBranch = genResults[i];
                Stack<(List<TokenTreeObject> branch, int lastIndex)> stack = new();

                int branchDeep = 0;

                int startIndex = 0;
                bool needExit = false;

                ProteinOperation? operation = null;
                bool action = false; 
                Token? protein = null;
                float? value = null;
                Token? inputVar = null;
                NameTranslateMode? translateMode = null;
                bool next = false;

                int start = 1;
                int end = currentBranch.Count - 2;

                if (currentBranch[0].Token.Text != "{")
                    errors.Add(new NeedBraceError(currentBranch[0].Token.LineNumber));

                else if (currentBranch[end + 1].Token.Text != "}")
                    errors.Add(new NeedBraceError(currentBranch[end + 1].Token.LineNumber));

                else
                {
                    while (!needExit)
                    {
                        if (startIndex >= currentBranch.Count)
                            break;

                        for (int j = startIndex; j < currentBranch.Count; j++)
                        {
                            startIndex = 0;

                            var treeToken = currentBranch[j];
                            Token token = treeToken.Token;

                            if ((j < start || j > end) && branchDeep == 0) continue;

                            //operation
                            if (ProteinOperationParser.TryParse(token.Text, out ProteinOperation? parseOperation))
                            {
                                if (operation != null || action || protein != null || value != null ||
                                    inputVar != null || translateMode != null || next || branchDeep > 0)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else operation = parseOperation;
                            }

                            //action
                            else if (token.Text == ReservedNames.Action)
                            {
                                if (operation == null || action || protein != null || value != null ||
                                    inputVar != null || translateMode != null || next || branchDeep > 0)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                if (operation.Value != ProteinOperation.Set)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else action = true;
                            }

                            //next
                            else if (token.Text == ReservedNames.Next)
                            {
                                if (operation == null || protein == null ||
                                    (value == null && inputVar == null) ||
                                    (value != null && inputVar != null) ||
                                    next || branchDeep > 0)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else next = true;
                            }

                            //protein name
                            else if (!ReservedNames.NameExist(token.Text) &&
                                     !ReservedSymbols.SymbolExist(token.Text) &&
                                     !float.TryParse(token.Text.Replace(',', '.'), out _) && branchDeep <= 0)
                            {
                                if (operation == null || protein != null || value != null ||
                                    inputVar != null || translateMode != null || next)
                                {
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));
                                }

                                else protein = token;

                                var branch = treeToken.Branch;

                                if (branch.Count < 3)
                                    errors.Add(new NotProteinSetValueError(token.LineNumber));

                                else if (branch.Count > 4)
                                    errors.Add(new TooManyTokensInSetValueError(token.LineNumber));

                                else
                                {
                                    bool isNormalBracket = true;

                                    if (branch[0].Token.Text != "(")
                                    {
                                        errors.Add(new NeedBracketError(branch[0].Token.LineNumber));
                                        isNormalBracket = false;
                                    }

                                    if (branch[branch.Count - 1].Token.Text != ")")
                                    {
                                        errors.Add(new NeedBracketError(branch[branch.Count - 1].Token.LineNumber));
                                        isNormalBracket = false;
                                    }

                                    if (isNormalBracket)
                                    {
                                        Token firstToken = branch[1].Token;

                                        if (float.TryParse(firstToken.Text.Replace(',', '.'), out float parsedValue))
                                        {
                                            value = parsedValue;
                                        }

                                        else
                                        {
                                            inputVar = firstToken;
                                        }

                                        //getlink getgen
                                        if (branch.Count >= 4)
                                        {
                                            if (value != null)
                                            {
                                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                                            }

                                            Token secondToken = branch[2].Token;

                                            if (secondToken.Text == ReservedNames.GetLink)
                                                translateMode = NameTranslateMode.GetLink;

                                            else if (secondToken.Text == ReservedNames.GetGen)
                                                translateMode = NameTranslateMode.GetGen;

                                            else
                                            {
                                                errors.Add(new NotAvailableInContextError(secondToken.LineNumber));
                                            }
                                        }
                                    }
                                }
                            }

                            else if (branchDeep == 0)
                            {
                                errors.Add(new NotAvailableInContextError(token.LineNumber));
                            }

                            bool isEnd = j >= end && branchDeep == 0;
                            if (isEnd || next)
                            {
                                if (next && isEnd)
                                    errors.Add(new NotAvailableInContextError(token.LineNumber));

                                if (operation != null && protein != null)
                                {
                                    if (value != null && inputVar == null)
                                    {
                                        results[genNames[i].Text].Add(new NotCompiledResult
                                        (
                                            operation.Value,
                                            action,
                                            protein.Value,
                                            value.Value
                                        ));
                                    }

                                    else if (inputVar != null)
                                    {
                                        results[genNames[i].Text].Add(new NotCompiledResult
                                        (
                                            operation.Value,
                                            action,
                                            protein.Value,
                                            inputVar.Value,
                                            translateMode
                                        ));
                                    }

                                    else
                                    {
                                        throw new Exception();
                                    }
                                }

                                else
                                {
                                    throw new Exception();
                                }

                                operation = null;
                                action = false;
                                protein = null;
                                value = null;
                                inputVar = null;
                                translateMode = null;
                                next = false;

                                if (isEnd) needExit = true;
                            }

                            if (treeToken.Branch.Count > 0)
                            {
                                stack.Push((currentBranch, j));
                                currentBranch = treeToken.Branch;
                                branchDeep++;

                                break;
                            }

                            else if (j == currentBranch.Count - 1)
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
                }
            }

            return;
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
