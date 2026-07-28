using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal class Cell
    {
        public float[] Proteins { get; private set; } = [];

        public Gen[] Gens { get; private set; } = [];

        public Cell(Gen[] gens, int proteinCount)
        {
            if (gens == null) throw new ArgumentNullException(nameof(gens));

            Gens = gens;
            Proteins = new float[proteinCount];
        }
    }
}
