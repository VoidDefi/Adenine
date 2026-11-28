using Adenine.Compiler.Errors;
using Adenine.Compiler.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler
{
    internal static class Preprocessor
    {
        public static List<Line> GetProcessedCode(string code, List<Error> errors)
        {
            List<Line> lines = AdenineCompiler.SplitLines(code);

            List<Token> insertsHistory = new();

            while (true)
            {
                List<Token> inserts = new();

                for (int i = 0; i < lines.Count; i++)
                {
                    int insertIndex = lines[i].Text.IndexOf(ReservedNames.Insert);
                    int startCommentIndex = lines[i].Text.IndexOf("//");

                    int end = lines[i].Text.Length;

                    if (startCommentIndex >= 0)
                    {
                        end = startCommentIndex;
                    }

                    if (insertIndex >= 0)
                    {
                        int start = insertIndex + ReservedNames.Insert.Length;

                        StringBuilder path = new();

                        for (int j = start; j < end; j++)
                        {
                            char symbol = lines[i].Text[j];

                            if (AdenineCompiler.IsNotSpace(symbol))
                            {
                                path.Append(symbol);
                            }
                        }

                        if (insertsHistory.FindIndex(t => t.Text == path.ToString()) < 0)
                        {
                            Token pathToken = new Token(path.ToString(), lines[i].Number);
                            insertsHistory.Add(pathToken);
                            inserts.Add(pathToken);
                        }

                        lines[i] = new Line("\r", lines[i].Number);
                    }
                }

                if (inserts.Count == 0)
                {
                    break;
                }

                List<Line> addedLines = new List<Line>();

                foreach (Token insert in inserts)
                {
                    if (insert.Text.Length <= 0)
                    {
                        errors.Add(new WrongInsertPathPreprocessorError(insert.LineNumber));
                    }

                    else
                    {
                        string path = $"projects\\{Program.CurrentProject}\\" + insert.Text + ".plm";

                        if (File.Exists(path))
                        {
                            string name = Path.GetFileNameWithoutExtension(path);
                            string newCode = File.ReadAllText(path);

                            addedLines.Add(new Line(ReservedNames.Scope + " " + name + "\r", 0));
                            addedLines.AddRange(AdenineCompiler.SplitLines(newCode));
                            addedLines.Add(new Line(ReservedNames.EndScope + "\r", 0));
                        }

                        else
                        {
                            errors.Add(new WrongInsertPathPreprocessorError(insert.LineNumber));
                        }
                    }
                }

                lines.InsertRange(0, addedLines);
            }

            List<(Token original, Token replaced)> replaces = new();

            for (int i = 0; i < lines.Count; i++)
            {
                int replaceIndex = lines[i].Text.IndexOf(ReservedNames.Replace);
                int startCommentIndex = lines[i].Text.IndexOf("//");

                int end = lines[i].Text.Length;

                if (startCommentIndex >= 0)
                {
                    end = startCommentIndex;
                }

                if (replaceIndex >= 0)
                {
                    int start = replaceIndex + ReservedNames.Replace.Length;

                    StringBuilder original = new();
                    int replacedStart = -1;

                    for (int j = start; j < end; j++)
                    {
                        char symbol = lines[i].Text[j];

                        if (symbol == ':')
                        {
                            replacedStart = j + 1;
                            break;
                        }

                        if (AdenineCompiler.IsNotSpace(symbol))
                        {
                            original.Append(symbol);
                        }
                    }

                    if (replacedStart > start)
                    {
                        StringBuilder replacedText = new();

                        for (int j = replacedStart; j < end; j++)
                        {
                            char symbol = lines[i].Text[j];
                            replacedText.Append(symbol);
                        }

                        if (replaces.FindIndex(t => t.original.Text == original.ToString()) < 0)
                        {
                            Token originalToken = new Token(original.ToString(), lines[i].Number);
                            Token replacedTextToken = new Token(replacedText.ToString(), lines[i].Number);

                            replaces.Add((originalToken, replacedTextToken));
                        }
                    }

                    else
                    {
                        errors.Add(new WrongReplaceSignaturePreprocessorError(lines[i].Number));
                    }

                    lines[i] = new Line("\r", lines[i].Number);
                }
            }

            foreach (var replace in replaces)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    Line line = lines[i];

                    line.Text = line.Text.Replace(replace.original.Text, replace.replaced.Text);
                    lines[i] = line;
                }
            }

            return lines;
        }
    }
}
