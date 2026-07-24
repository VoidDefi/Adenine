using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal class DNA
    {
        public float[] Proteins { get; private set; } = [];

        public Gen[] Gens { get; private set; } = [];
    }
}
