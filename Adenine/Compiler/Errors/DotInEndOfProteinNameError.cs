using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class DotInEndOfProteinNameError : Error
    {
        public DotInEndOfProteinNameError(int line) : base(line)
        {
        }

        public override string Message => "the plasmid name exists, but there is no name for the protein.";
    }
}
