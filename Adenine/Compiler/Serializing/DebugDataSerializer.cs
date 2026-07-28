using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

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
    }
}
