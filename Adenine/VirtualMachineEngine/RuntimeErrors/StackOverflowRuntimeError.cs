using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class StackOverflowRuntimeError : RuntimeError
    {
        public StackOverflowRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Stack overflow";
    }
}