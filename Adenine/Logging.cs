using Adenine.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine
{
    internal class Logging
    {
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
    }
}
