using Adenine.CodeObjects;
using Adenine.CodeObjects.FunctionalProteins;
using Adenine.Compiler.Registry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Decompiler
{
    internal static class AdenineDecompiler
    {
        public static List<CodeFile> Decompile(Cell cell)
        {
            List<CodeFile> files = new List<CodeFile>();

            Dictionary<int, List<int>> proteinInScopes = new Dictionary<int, List<int>>();

            for (int i = 0; i < cell.Plasmids.Length; i++)
            {
                AddProteinToScope(cell, cell.Plasmids[i].ToArray(), i, proteinInScopes);
            }

            AddProteinToScope(cell, cell.Gens, -1, proteinInScopes);

            files.Add(DecompileFile(cell, cell.Gens, -1, proteinInScopes));

            for (int i = 0; i < cell.Plasmids.Length; i++)
            {
                files.Add(DecompileFile(cell, cell.Plasmids[i].ToArray(), i, proteinInScopes));
            }

            return files;
        }

        private static void AddProteinToScope(Cell cell, Gen[] gens, int plasmid, Dictionary<int, List<int>> proteinInScopes)
        {
            for (int i = 0; i < gens.Length; i++)
            {
                for (int j = 0; j < gens[i].Results.Length; j++)
                {
                    Result result = gens[i].Results[j];

                    int protein = result.ProteinIndex;

                    if (!result.Action)
                    {
                        bool exist = false;

                        foreach (var scope in proteinInScopes)
                        {
                            if (scope.Value.FindIndex(p => p == protein) >= 0)
                            {
                                exist = true;
                                break;
                            }
                        }

                        if (!exist)
                        {
                            if (!proteinInScopes.ContainsKey(plasmid))
                            {
                                proteinInScopes.Add(plasmid, new List<int>());
                            }

                            proteinInScopes[plasmid].Add(protein);
                        }
                    }
                }
            }
        }

        private static CodeFile DecompileFile(Cell cell, Gen[] gens, int plasmid, Dictionary<int, List<int>> proteinInScopes)
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < cell.Plasmids.Length; i++)
            {
                buffer.Append(ReservedNames.Insert);
                buffer.Append(" plasmid");
                buffer.Append(i);
                buffer.Append("\r\n");
            }

            if (cell.Plasmids.Length > 0)
                buffer.Append("\r\n");

            for (int i = 0; i < gens.Length; i++)
            {
                Gen gen = gens[i];

                buffer.Append(ReservedNames.Gen);
                buffer.Append(" g");
                buffer.Append(i);
                buffer.Append("\r\n{\r\n");

                buffer.Append("\tcondition\r\n\t{\r\n");

                for (int j = 0; j < gen.Conditions.Length; j++)
                {
                    Condition condition = gen.Conditions[j];

                    buffer.Append("\t\t");
                    buffer.Append(GetPrefixName(condition.ProteinIndex, proteinInScopes));
                    buffer.Append("p");
                    buffer.Append(condition.ProteinIndex);

                    buffer.Append(" ");
                    buffer.Append(ComparisonOperatorParser.ToString(condition.Operator));

                    buffer.Append(" ");

                    if (condition.UseProteinValue)
                    {
                        buffer.Append(GetPrefixName(condition.ComparingProtein, proteinInScopes));
                        buffer.Append("p");
                        buffer.Append(condition.ComparingProtein);
                    }

                    else buffer.Append(condition.Value.ToString().Replace(',', '.'));

                    if (condition.LogicOperator == LogicOperator.And)
                        buffer.Append("and");

                    if (condition.LogicOperator == LogicOperator.Or)
                        buffer.Append("or");

                    buffer.Append("\r\n");
                }

                buffer.Append("\t}\r\n\r\n");

                buffer.Append("\tresult\r\n\t{\r\n");

                for (int j = 0; j < gen.Results.Length; j++)
                {
                    Result result = gen.Results[j];

                    buffer.Append("\t\t");

                    buffer.Append(ProteinOperationParser.ToString(result.Operation));

                    bool isInject = false;

                    if (result.Action)
                    {
                        buffer.Append(" action ");

                        if (FunctionalProteinsRegistry.Proteins.TryGetValue(result.ProteinIndex, out FunctionalProtein? protein))
                        {
                            buffer.Append(protein.Name);

                            if (protein.Index == 12)
                            {
                                isInject = true;
                            }
                        }
                    }

                    else
                    {
                        buffer.Append(" ");

                        if (result.Pointer)
                        {
                            buffer.Append("#");
                        }

                        buffer.Append(GetPrefixName(result.ProteinIndex, proteinInScopes));
                        buffer.Append("p");
                        buffer.Append(result.ProteinIndex);
                    }


                    buffer.Append("(");

                    if (result.UseProteinValue)
                    {
                        buffer.Append(GetPrefixName(result.InputProtein, proteinInScopes));
                        buffer.Append("p");
                        buffer.Append(result.InputProtein);

                        if (result.GetValueFrom)
                        {
                            buffer.Append(" valuefrom");
                        }
                    }

                    else
                    {
                        if (isInject)
                        {
                            buffer.Append("plasmid");
                            buffer.Append((int)result.Value);
                            buffer.Append(" ");
                            buffer.Append(ReservedNames.GetPlasmid);
                        }

                        else
                        {
                            buffer.Append(result.Value.ToString().Replace(',', '.'));
                        }
                    }

                    buffer.Append(")");

                    if (j < gen.Results.Length - 1)
                    {
                        buffer.Append(" next");
                    }

                    buffer.Append("\r\n");
                }

                buffer.Append("\t}\r\n");

                buffer.Append("}\r\n\r\n");
            }

            string fileName = "code.adn";

            if (plasmid >= 0)
            {
                fileName = $"plasmid{plasmid}.plm";
            }

            return new CodeFile(buffer.ToString(), fileName);
        }

        private static string GetPrefixName(int protein, Dictionary<int, List<int>> proteinInScopes)
        {
            string prefix = "";

            foreach (var scope in proteinInScopes)
            {
                if (scope.Value.FindIndex(p => p == protein) >= 0)
                {
                    if (scope.Key < 0)
                    {
                        prefix = ".";
                    }

                    else
                    {
                        prefix = "plasmid" + scope.Key + ".";
                    }

                    break;
                }
            }

            return prefix;
        }
    }
}
