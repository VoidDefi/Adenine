using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    public struct Trace
    {
        public Frame[] TracedStack { get; private set; }

        public TraceBlock TraceBlock { get; private set; }

        public Trace(Stack<Frame> stack, TraceBlock traceBlock)
        {
            if (stack == null) throw new ArgumentNullException(nameof(stack));

            TracedStack = new Frame[stack.Count];
            stack.CopyTo(TracedStack, 0);
            TraceBlock = traceBlock;
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < TracedStack.Length; i++)
            {
                Frame frame = TracedStack[i];

                string plasmid = frame.PlasmidIndex == -1 ? "main" : $"plm#{frame.PlasmidIndex}";
                string gen = $"g#{frame.GenIndex}";

                /*if (VirtualMachine.DebugData != null)
                {
                    gen = VirtualMachine.DebugData.GenNames[frame.GenIndex];
                }*/

                buffer.Append($"  at {plasmid}: {gen}");

                if (i == 0)
                {
                    buffer.Append($" in {TraceBlock}");
                }

                if (i < TracedStack.Length - 1)
                buffer.Append("\n");
            }

            return buffer.ToString();
        }
    }

    public enum TraceBlock
    {
        Condition,
        Result
    }
}
