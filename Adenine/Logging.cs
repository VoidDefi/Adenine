using Adenine.CodeObjects;
using Adenine.Compiler;
using Adenine.VirtualMachineEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine
{
    internal class Logging
    {
        public static StringBuilder ProgramOutput { get; private set; } = new StringBuilder();

        public static void ProgramOutputClear()
        {
            ProgramOutput = new StringBuilder();
        }

        public static void ProgramOutputAppend(char c)
        {
            ProgramOutput.Append(c);
        }

        internal static void SaveTokenTree(List<TokenTreeObject> tokenTree)
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

        public static class Program
        {
            static StringBuilder CurrentSession { get; set; }

            public static void Start()
            {
                if (!VirtualMachine.IsProgramStarted)
                    return;

                CurrentSession = new();
            }

            public static void Save()
            {
                if (CurrentSession == null) return;

                File.WriteAllText("programLogs.txt", CurrentSession.ToString());

                CurrentSession = null;
            }

            public static void EmitText(string text)
            {
                if (CurrentSession == null) return;

                CurrentSession.Append(text);
            }

            public static void EmitNewLine() => EmitText("\r\n");

            public static void EmitLine(string newLine) 
            {
                EmitText(newLine);
                EmitNewLine();
            } 

            public static void EmitError(RuntimeError error) => EmitLine(error.ToString());

            public static void EmitCurrentProteinsState()
            {
                if (CurrentSession == null) return;

                Cell cell = VirtualMachine.Cell;
                DebugData debugData = VirtualMachine.DebugData;

                if (cell?.Proteins == null) return;

                EmitNewLine();

                for (int i = 0; i < cell.Proteins.Length; i++)
                {
                    float value = cell.Proteins[i];

                    string name = $"p#{i}";

                    if (debugData?.ProteinNames != null)
                    {
                        if (i < debugData.ProteinNames.Length)
                        {
                            name = debugData.ProteinNames[i];
                        }
                    }

                    EmitLine($"{name} = {value}");
                }

                EmitNewLine();
            }

            public static void EmitCurrentGen()
            {
                if (CurrentSession == null) return;

                Cell cell = VirtualMachine.Cell;
                DebugData debugData = VirtualMachine.DebugData;

                if (cell?.Gens == null) return;

                if (VirtualMachine.GenIndex < 0 || VirtualMachine.GenIndex >= cell.Gens.Length)
                    return;

                EmitNewLine();

                string name = $"g#{VirtualMachine.GenIndex}";

                if (debugData?.GenNames != null)
                {
                    if (VirtualMachine.GenIndex < debugData.GenNames.Length)
                    {
                        name = debugData.GenNames[VirtualMachine.GenIndex];
                    }
                }

                EmitLine($"Current Gen: {name}");
                EmitNewLine();
            }
        }
    }
}
