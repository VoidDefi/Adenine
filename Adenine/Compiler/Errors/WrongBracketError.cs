using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class WrongBracketError : Error
    {
        public WrongBracketError(int line) : base(line)
        {
        }

        public override string Message => "The bracket must be closed by a bracket of the same type. \"{ ... }\", \"( ... )\"";


    }
}
