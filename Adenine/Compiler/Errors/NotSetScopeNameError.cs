using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotSetScopeNameError : Error
    {
        public NotSetScopeNameError(int line) : base(line)
        {
        }

        public override string Message => "Name not found for this scope";
    }
}
