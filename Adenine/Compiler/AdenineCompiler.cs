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
        public static void Compile(string code)
        {
            List<Line> lines = SplitLines(code);
            List<Token> tokens = SplitTokens(lines);

            List<TokenTreeObject> tokenTree = CreateTokensTree(tokens);

            TokenTreeLogging(tokenTree);
        }

        private static List<Line> SplitLines(string code)
        {
            string[] rawLines = (code + "\r").Split("\n");
            List<Line> lines = new List<Line>();

            for (int i = 0; i < rawLines.Length; i++)
            {
                lines.Add(new Line(rawLines[i], i));
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

                if (token.Text == "}" || token.Text == ")")
                {
                    if (braceCount <= 0) { }

                    currentBranch = stack.Pop();
                    braceCount--;
                }

                currentBranch.Add(new(token));
            }

            return tokenTree;
        }

        private static void TokenTreeLogging(List<TokenTreeObject> tokenTree)
        {
            List<TokenTreeObject> currentBranch = tokenTree;
            int offset = 0;

            StringBuilder stringBuilder = new();
            bool needExit = false;

            Stack<(List<TokenTreeObject> branch, int lastIndex)> stack = new();

            int startIndex = 0;

            while (!needExit)
            {
                if (startIndex >= currentBranch.Count)
                    break;

                for (int i = startIndex; i < currentBranch.Count; i++)
                {
                    startIndex = 0;

                    var treeToken = currentBranch[i];

                    if (i <= 0 && offset > 0)
                    {
                        stringBuilder.Append(new string(' ', offset - 1));
                        stringBuilder.Append("└");
                    }

                    else if (i >= currentBranch.Count - 1 && offset > 0)
                    {
                        stringBuilder.Append(new string(' ', offset - 1));
                        stringBuilder.Append("▼");
                    }

                    else
                    {
                        stringBuilder.Append(new string(' ', offset));
                    }

                    stringBuilder.Append(treeToken.Token.Text + "\n");

                    if (treeToken.Branch.Count > 0)
                    {
                        stack.Push((currentBranch, i));
                        currentBranch = treeToken.Branch;
                        offset++;
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
                            offset--;
                        }

                        break;
                    }
                }
            }

            string filePath = "treeLog.txt";

            File.WriteAllText(filePath, stringBuilder.ToString());
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
