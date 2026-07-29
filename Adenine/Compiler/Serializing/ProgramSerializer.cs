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

            //

            return null;
        }
    }
}
