using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class TooManyTokensInSetValueError : Error
    {
        public TooManyTokensInSetValueError(int line) : base(line)
        {
        }

        public override string Message => "Too many tokens when setting protein value";
    }
}
