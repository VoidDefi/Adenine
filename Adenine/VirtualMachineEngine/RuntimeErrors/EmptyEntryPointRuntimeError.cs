using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class EmptyEntryPointRuntimeError : RuntimeError
    {
        public EmptyEntryPointRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Entry point does not exist";
    }
}
