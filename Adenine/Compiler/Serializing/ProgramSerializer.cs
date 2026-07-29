using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Serializing
{
    internal static class ProgramSerializer
    {
        public static byte[] Serialize(Cell cell)
        {
            // 4b gens count
            // 4b proteins count
            // ...
            // 4b conditions count
            // 4b results count
            // ?b condition array
            // ?b result array
            // ...

            List<byte> bytes = new List<byte>();

            int gensCount = cell.Gens.Length;
            int proteinsCount = cell.Proteins.Length;

            bytes.AddRange(BitConverter.GetBytes(gensCount));
            bytes.AddRange(BitConverter.GetBytes(proteinsCount));

            for (int i = 0; i < gensCount; i++)
            {
                Condition[] conditions = cell.Gens[i].Conditions;
                Result[] results = cell.Gens[i].Results;

                bytes.AddRange(BitConverter.GetBytes(conditions.Length));
                bytes.AddRange(BitConverter.GetBytes(results.Length));

                foreach (Condition condition in conditions)
                {
                    bytes.AddRange(condition.Serialize());
                }

                foreach (Result result in results)
                {
                    bytes.AddRange(result.Serialize());
                }
            }

            return bytes.ToArray();
        }

        public static Cell DeSerialize(byte[] bytes)
        {
            int currentIndex = 0;

            int gensCount = BitConverter.ToInt32(bytes, currentIndex);
            int proteinsCount = BitConverter.ToInt32(bytes, currentIndex + 4);

            currentIndex += 4 * 2;

            Gen[] gens = new Gen[gensCount];

            for (int i = 0; i < gensCount; i++)
            {
                int conditionsCount = BitConverter.ToInt32(bytes, currentIndex);
                int resultsCount = BitConverter.ToInt32(bytes, currentIndex + 4);

                currentIndex += 4 * 2;

                Condition[] conditions = new Condition[conditionsCount];
                Result[] results = new Result[resultsCount];

                for (int j = 0; j < conditionsCount; j++)
                {
                    Condition condition = new Condition();
                    condition.DeSerialize(bytes, currentIndex); 

                    conditions[j] = condition;

                    currentIndex += condition.ByteSize;
                }

                for (int j = 0; j < resultsCount; j++)
                {
                    Result result = new Result();
                    result.DeSerialize(bytes, currentIndex);

                    results[j] = result;

                    currentIndex += result.ByteSize;
                }

                if (conditions.Length <= 0 || results.Length <= 0)
                    throw new Exception("Conditions or results count <= 0");

                if (conditions[conditions.Length - 1].LogicOperator != LogicOperator.None)
                    throw new Exception("End logic operator must be none");

                gens[i] = new Gen(conditions, results);
            }

            return new Cell(gens, proteinsCount);
        }
    }
}
