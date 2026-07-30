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

        public ProgramRunModifier(bool needEnd)
        {
            NeedEnd = needEnd;
            NeedGoto = false;
            GotoIndex = -1;
        }

        public ProgramRunModifier(int gotoIndex)
        {
            NeedEnd = false;
            NeedGoto = true;
            GotoIndex = gotoIndex;
        }
    }
}
