using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class PlasmidNotExistError : Error
    {
        public PlasmidNotExistError(int line) : base(line)
        {
        }

        public override string Message => "Plasmid with this name does not exist";
    }
}
