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

            if (index < 0 || index >= VirtualMachine.Cell.Gens.Length)
            {
                VirtualMachine.Throw<GenIndexOutOfRangeRuntimeError>();
                modifier = new ProgramRunModifier(true);
                return;
            }

            Gen gen = VirtualMachine.Cell.Gens[index];

            bool flag = VirtualMachine.ExecuteCondition(gen, out bool errorThrowed);

            if (errorThrowed)
            {
                modifier = new ProgramRunModifier(true);
                return;
            }

            if (flag) 
            {
                modifier = new ProgramRunModifier(index, true);
            } 
        }
    }
}
