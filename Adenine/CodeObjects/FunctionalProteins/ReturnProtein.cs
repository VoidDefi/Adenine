using Adenine.VirtualMachineEngine;
using Adenine.VirtualMachineEngine.RuntimeErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class ReturnProtein : FunctionalProtein
    {
        public override string Name => "return";

        public override int Index => 9;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            if (VirtualMachine.EntryPoint < 0)
            {
                VirtualMachine.Throw<EmptyEntryPointRuntimeError>();
            }

            int index = VirtualMachine.EntryPoint + 1;

            if (index >= VirtualMachine.Cell.Gens.Length)
            {
                modifier = new ProgramRunModifier(0, false, false);
                VirtualMachine.EntryPoint = -1;

                VirtualMachine.IterationCounter++;

                return;
            }

            modifier = new ProgramRunModifier(index, false, false);
            VirtualMachine.EntryPoint = -1;
        }
    }
}
