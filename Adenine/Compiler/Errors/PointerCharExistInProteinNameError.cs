using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class PointerCharExistInProteinNameError : Error
    {
        public PointerCharExistInProteinNameError(int line) : base(line)
        {
        }

        public override string Message => "The protein name must not contain '#'";
    }
}
