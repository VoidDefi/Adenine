using Adenine.CodeObjects;
using Adenine.CodeObjects.FunctionalProteins;
using Adenine.Compiler.Registry;
using System;
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

            files.Add(DecompileFile(cell, cell.Gens, -1));

            for (int i = 0; i < cell.Plasmids.Length; i++)
            {
                files.Add(DecompileFile(cell, cell.Plasmids[i].ToArray(), i));
            }

            return files;
        }

        private static CodeFile DecompileFile(Cell cell, Gen[] gens, int plasmid)
        {
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < cell.Plasmids.Length; i++)
            {
                buffer.Append(ReservedNames.Insert);
                buffer.Append(" plasmid");
                buffer.Append(i);
                buffer.Append("\r\n");
            }

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

                    buffer.Append("\t\tp");
                    buffer.Append(i);

                    buffer.Append(" ");
                    buffer.Append(ComparisonOperatorParser.ToString(condition.Operator));

                    buffer.Append(" ");

                    if (condition.UseProteinValue)
                    {
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

                    if (result.Action)
                    {
                        buffer.Append(" action ");

                        if (FunctionalProteinsRegistry.Proteins.TryGetValue(result.ProteinIndex, out FunctionalProtein? protein))
                        {
                            buffer.Append(protein.Name);
                        }
                    }

                    else
                    {
                        buffer.Append(" ");

                        if (result.Pointer)
                        {
                            buffer.Append("#");
                        }

                        buffer.Append("p");
                        buffer.Append(result.ProteinIndex);
                    }


                    buffer.Append("(");

                    if (result.UseProteinValue)
                    {
                        buffer.Append("p");
                        buffer.Append(result.InputProtein);

                        if (result.GetValueFrom)
                        {
                            buffer.Append(" valuefrom");
                        }
                    }

                    else buffer.Append(result.Value.ToString().Replace(',', '.'));

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
    }
}
