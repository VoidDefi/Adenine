using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine.RuntimeErrors
{
    internal class CanNotExitFromMainRuntimeError : RuntimeError
    {
        public CanNotExitFromMainRuntimeError(Trace trace) : base(trace)
        {
        }

        public override string Message => "Can not do exit from main. exit-from must be in any plasmid";
    }
}