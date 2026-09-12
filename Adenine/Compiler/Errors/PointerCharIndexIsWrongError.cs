using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class PointerCharIndexIsWrongError : Error
    {
        public PointerCharIndexIsWrongError(int line) : base(line)
        {
        }

        public override string Message => "The pointer designation '#' must appear exclusively at the beginning of the protein name";
    }
}
