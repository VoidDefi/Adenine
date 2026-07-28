using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal class DebugData
    {
        public string[] ProteinNames { get; private set; } = [];

        public string[] GenNames { get; private set; } = [];

        public DebugData(Cell cell, string[] proteinNames, string[] genNames)
        {
            if (cell == null) throw new ArgumentNullException(nameof(cell));
            if (proteinNames == null) throw new ArgumentException(nameof(proteinNames));
            if (genNames == null) throw new ArgumentException(nameof(genNames));

            if (cell.Proteins.Length != proteinNames.Length) throw new ArgumentException();
            if (cell.Gens.Length != genNames.Length) throw new ArgumentException();

            ProteinNames = proteinNames;
            GenNames = genNames;
        }
    }
}
