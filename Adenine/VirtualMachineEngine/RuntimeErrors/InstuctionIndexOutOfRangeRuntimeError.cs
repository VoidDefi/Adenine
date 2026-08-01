using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class InstructionIndexOutOfRangeRuntimeError : RuntimeError
    {
        public InstructionIndexOutOfRangeRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Instruction index was outside the bounds of the gen";
    }
}
