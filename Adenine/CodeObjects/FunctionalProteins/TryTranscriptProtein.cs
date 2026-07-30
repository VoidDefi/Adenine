using Adenine.VirtualMachineEngine;
using Adenine.VirtualMachineEngine.RuntimeErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class TryTranscriptProtein : FunctionalProtein
    {
        public override string Name => "try-transcript";

        public override int Index => 6;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            int index = (int)value;

            modifier = new ProgramRunModifier(index);
        }
    }
}
