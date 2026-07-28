using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class WrongOperationOnActionError : Error
    {
        public WrongOperationOnActionError(int line) : base(line)
        {
        }

        public override string Message => "Cannot use operations other than \"set\" for \"action\"";
    }
}
