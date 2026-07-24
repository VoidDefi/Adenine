using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotDefineResultError : Error
    {
        public NotDefineResultError(int line) : base(line)
        {
        }

        public override string Message => "The result was not fulfilled in this gen";
    }
}
