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

        public EntryPoint GotoEndPoint { get; private set; } = new();

        public bool ForcedlyExecuteJumpedGen { get; private set; } = false;

        public bool SaveEntryPoint { get; private set; } = true;

        public ProgramRunModifier(bool needEnd)
        {
            NeedEnd = needEnd;
            NeedGoto = false;
            GotoEndPoint.Clear();
            ForcedlyExecuteJumpedGen = false;
            SaveEntryPoint = true;
        }

        public ProgramRunModifier(int gotoIndex, bool forcedlyExecute, bool saveEntryPoint = true, int gotoIndexProtein = -1)
        {
            NeedEnd = false;
            NeedGoto = true;
            GotoEndPoint.gen = gotoIndex;
            GotoEndPoint.instruction = gotoIndexProtein;
            ForcedlyExecuteJumpedGen = forcedlyExecute;
            SaveEntryPoint = saveEntryPoint;
        }
    }
}
