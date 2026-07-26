using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NeedBracketError : Error
    {
        public NeedBracketError(int line) : base(line)
        {
        }

        public override string Message => "A brace \"(\" or \")\" is required";


    }
}
