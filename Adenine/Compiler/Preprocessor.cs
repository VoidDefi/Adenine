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

            return lines;
        }
    }
}
