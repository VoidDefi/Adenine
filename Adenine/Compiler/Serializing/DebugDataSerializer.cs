using Adenine.CodeObjects;
using System.Text;

namespace Adenine.Compiler.Serializing
{
    internal static class DebugDataSerializer
    {
        public static byte[] Serialize(DebugData debugData)
        {
            List<byte[]> proteins = new();
            List<byte[]> gens = new();

            foreach (string name in debugData.ProteinNames)
                proteins.Add(Encoding.UTF8.GetBytes(name));

            foreach (string name in debugData.GenNames)
                gens.Add(Encoding.UTF8.GetBytes(name));

            List<int> proteinsOffsets = new();
            List<byte> proteinsBytes = new();
            int proteinOffset = (proteins.Count + 1) * 4 + 4; //+4 is proteins count bytes in start

            foreach (byte[] protein in proteins)
            {
                proteinsBytes = proteinsBytes.Concat(protein).ToList();
                proteinsOffsets.Add(proteinOffset);
                proteinOffset += protein.Length;
            }

            proteinsOffsets.Add(proteinOffset);

            List<int> gensOffsets = new();
            List<byte> gensBytes = new();
            int genOffset = proteinOffset + (gens.Count) * 4 + 4 + 4; //+4 is proteins count bytes in file start. next +4 is gens count bytes in start.

            foreach (byte[] gen in gens)
            {
                gensBytes = gensBytes.Concat(gen).ToList();
                gensOffsets.Add(genOffset);
                genOffset += gen.Length;
            }

            gensOffsets.Add(genOffset);

            List<byte> proteinMetadata = new();

            //Add proteins count
            byte[] proteinsCount = BitConverter.GetBytes(proteins.Count);
            proteinMetadata.AddRange(proteinsCount);

            foreach (var offset in proteinsOffsets)
            {
                proteinMetadata.AddRange(BitConverter.GetBytes(offset));
            }

            List<byte> genMetadata = new();

            //Add gens count
            byte[] gensCount = BitConverter.GetBytes(gens.Count);
            genMetadata.AddRange(gensCount);

            foreach (var offset in gensOffsets)
            {
                genMetadata.AddRange(BitConverter.GetBytes(offset));
            }

            byte[] debugBytes =
            [
                .. proteinMetadata,
                .. proteinsBytes,
                .. genMetadata,
                .. gensBytes,
            ];

            return debugBytes;
        }

        public static DebugData DeSerialize(Cell cell, byte[] debugBytes)
        {
            int proteinsCount = BitConverter.ToInt32(debugBytes, 0);
            int[] proteinIndexes = new int[proteinsCount + 1];

            for (int i = 0; i < proteinsCount + 1; i++)
            {
                proteinIndexes[i] = BitConverter.ToInt32(debugBytes, i * 4 + 4);
            }

            string[] proteins = new string[proteinsCount];

            for (int i = 0; i < proteinsCount; i++)
            {
                int start = proteinIndexes[i];
                int end = proteinIndexes[i + 1];

                proteins[i] = Encoding.UTF8.GetString(debugBytes, start, end - start);
            }

            int gensStart = proteinIndexes[proteinsCount];

            int gensCount = BitConverter.ToInt32(debugBytes, gensStart);
            int[] genIndexes = new int[gensCount + 1];

            for (int i = 0; i < gensCount + 1; i++)
            {
                genIndexes[i] = BitConverter.ToInt32(debugBytes, gensStart + i * 4 + 4);
            }

            string[] gens = new string[gensCount];

            for (int i = 0; i < gensCount; i++)
            {
                int start = genIndexes[i];
                int end = genIndexes[i + 1];

                gens[i] = Encoding.UTF8.GetString(debugBytes, start, end - start);
            }

            return new DebugData(cell, proteins, gens);
        }
    }
}
