using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    internal struct Trace
    {
        public string GenName { get; private set; }

        public TraceBlock Block { get; private set; }

        public Trace(string genName, TraceBlock block)
        {
            GenName = genName;
            Block = block;
        }

        public override string ToString()
        {
            return $"In {GenName}/{Block}";
        }
    }

    internal enum TraceBlock
    {
        Condition,
        Result
    }
}
