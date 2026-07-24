using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotDefineConditionError : Error
    {
        public NotDefineConditionError(int line) : base(line)
        {
        }

        public override string Message => "The condition was not fulfilled in this gen";
    }
}
