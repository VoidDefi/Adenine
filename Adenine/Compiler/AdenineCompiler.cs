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
                            errors.Add(new NotAvailableInContext(token.LineNumber));
                        }
                    }

                    else if (token.Text == ReservedNames.Condition ||
                             token.Text == ReservedNames.Result)
                    {
                        if (branchDeep != 1)
                        {
                            errors.Add(new NotAvailableInContext(token.LineNumber));
                        }
                    }

                    //Check, all why not equals gen, condition, result
                    else if (ReservedNames.NameExist(token.Text)) 
                    {
                        if (branchDeep != 2)
                        {
                            errors.Add(new NotAvailableInContext(token.LineNumber));
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

        private static List<Error> AssemblyDNA(List<TokenTreeObject> tokenTree)
        {
            List<Error> errors = new();

            List<TokenTreeObject> currentBranch = tokenTree;
            Stack<(List<TokenTreeObject> branch, int lastIndex)> stack = new();

            int branchDeep = 0;

            int startIndex = 0;
            bool needExit = false;

            while (!needExit)
            {
                if (startIndex >= currentBranch.Count)
                    break;

                for (int i = startIndex; i < currentBranch.Count; i++)
                {
                    startIndex = 0;

                    var treeToken = currentBranch[i];
                    Token token = treeToken.Token;



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

            return errors;
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
