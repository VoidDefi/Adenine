using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class ProteinIndexOutOfRangeRuntimeError : RuntimeError
    {
        public ProteinIndexOutOfRangeRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Protein index was outside the bounds of the proteins";
    }
}
