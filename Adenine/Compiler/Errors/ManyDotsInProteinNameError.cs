using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class ManyDotsInProteinNameError : Error
    {
        public ManyDotsInProteinNameError(int line) : base(line)
        {
        }

        public override string Message => "you cannot use more than one dot in the name of protein.";
    }
}
