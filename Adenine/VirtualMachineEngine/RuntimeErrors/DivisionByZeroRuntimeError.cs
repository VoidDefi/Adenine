using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class DivisionByZeroRuntimeError : RuntimeError
    {
        public DivisionByZeroRuntimeError(Trace trace) : base(trace)
        { 
        }

        public override string Message => "Division by zero is prohibited";
    }
}
