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
            EntryPoint entryPoint = VirtualMachine.CallStack.Peek().EntryPoint;

            if (entryPoint.gen < 0 || entryPoint.instruction < 0)
            {
                VirtualMachine.Throw<EmptyEntryPointRuntimeError>();
                modifier = ProgramRunModifier.CreateProgramEnd();
                return;
            }

            modifier = ProgramRunModifier.CreateTranscript(entryPoint.gen, false, entryPoint.instruction + 1);

            entryPoint.Clear();
        }
    }
}
