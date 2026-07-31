using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    internal struct ProgramRunModifier
    {
        public bool NeedEnd { get; private set; } = false;

        public bool NeedGoto { get; private set; } = false;

        public int GotoIndex { get; private set; } = -1;

        public bool ForcedlyExecuteJumpedGen { get; private set; } = false;

        public ProgramRunModifier(bool needEnd)
        {
            NeedEnd = needEnd;
            NeedGoto = false;
            GotoIndex = -1;
        }

        public ProgramRunModifier(int gotoIndex, bool forcedlyExecuteJumpedGen)
        {
            NeedEnd = false;
            NeedGoto = true;
            GotoIndex = gotoIndex;
            ForcedlyExecuteJumpedGen = forcedlyExecuteJumpedGen;
        }
    }
}
