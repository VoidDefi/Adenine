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
            /*f (VirtualMachine.CurrentEntryPoint.gen < 0 || VirtualMachine.CurrentEntryPoint.instruction < 0)
            {
                VirtualMachine.Throw<EmptyEntryPointRuntimeError>();
            }

            var index = VirtualMachine.CurrentEntryPoint.instruction + 1;

            if (VirtualMachine.CurrentEntryPoint.gen >= 0)
            {
                if (index >= VirtualMachine.Cell.Gens[VirtualMachine.CurrentEntryPoint.gen].Results.Length)
                {
                    int genIndex = VirtualMachine.CurrentEntryPoint.gen;

                    if (genIndex < VirtualMachine.Cell.Gens.Length - 1)
                    {
                        modifier = new ProgramRunModifier(genIndex + 1, false, false);
                    }

                    else
                    {
                        modifier = new ProgramRunModifier(0, false, false);
                        VirtualMachine.IterationCounter++;
                    }

                    VirtualMachine.CurrentEntryPoint.Clear();
                    //VirtualMachine.CurrentEntryPoint.instruction = -1;
                    entryPoint
                }

                modifier = new ProgramRunModifier(VirtualMachine.CurrentEntryPoint.gen, true, false, index);
                VirtualMachine.CurrentEntryPoint.Clear();
            }*/

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
