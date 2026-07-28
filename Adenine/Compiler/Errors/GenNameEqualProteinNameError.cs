using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class GenNameEqualProteinNameError : Error
    {
        public GenNameEqualProteinNameError(int line) : base(line)
        {
        }

        public override string Message => "The name of the gene matches the name of the protein";
    }
}
