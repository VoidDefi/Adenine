using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class EmptyConditionOrResultError : Error
    {
        public EmptyConditionOrResultError(int line) : base(line)
        {
        }

        public override string Message => "Condition or Result block is empty";
    }
}
