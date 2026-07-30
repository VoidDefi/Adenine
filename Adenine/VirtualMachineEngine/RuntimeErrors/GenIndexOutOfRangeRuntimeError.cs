using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class GenIndexOutOfRangeRuntimeError : RuntimeError
    {
        public GenIndexOutOfRangeRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Gen index was outside the bounds of the gens";
    }
}
