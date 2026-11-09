using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class PlasmidIndexOutOfRangeRuntimeError : RuntimeError
    {
        public PlasmidIndexOutOfRangeRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Plasmid index was outside the bounds of the plasmids";
    }
}
