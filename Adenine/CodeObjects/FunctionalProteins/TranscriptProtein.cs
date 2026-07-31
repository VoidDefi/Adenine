using Adenine.VirtualMachineEngine;
using Adenine.VirtualMachineEngine.RuntimeErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class TranscriptProtein : FunctionalProtein
    {
        public override string Name => "transcript";

        public override int Index => 7;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            int index = (int)value;

            modifier = new ProgramRunModifier(index, true);
        }
    }
}
