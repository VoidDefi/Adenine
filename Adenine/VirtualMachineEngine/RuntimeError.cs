using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    internal abstract class RuntimeError
    {
        public abstract string Message { get; }

        public Trace Trace { get; private set; }

        public RuntimeError(Trace trace)
        {
            Trace = trace;
        }

        public override string ToString()
        {
            return "Error was throwed " + Trace.ToString() + ": " + Message;
        }
    }
}
