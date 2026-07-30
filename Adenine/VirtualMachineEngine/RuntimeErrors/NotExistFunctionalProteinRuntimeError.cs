using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class NotExistFunctionalProteinRuntimeError : RuntimeError
    {
        public NotExistFunctionalProteinRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Does not exist this functional protein in current Virtual Machine";
    }
}
