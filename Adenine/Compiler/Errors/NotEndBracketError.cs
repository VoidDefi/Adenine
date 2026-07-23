using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotEndBracketError : Error
    {
        public NotEndBracketError(int line) : base(line)
        {
        }

        public override string Message => "Closing bracket is required: \"}\" or \")\"";


    }
}
