using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class ReservedNameError : Error
    {
        public ReservedNameError(int line) : base(line)
        {
        }

        public override string Message => "This name is already taken";
    }
}
