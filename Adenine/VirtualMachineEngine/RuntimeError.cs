using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    public abstract class RuntimeError
    {
        public abstract string Message { get; }

        public Trace Trace { get; private set; }

        public RuntimeError(Trace trace)
        {
            Trace = trace;
        }

        public override string ToString()
        {
            return "Error was throwed: " + Message + "\n" + Trace.ToString();
        }
    }
}
