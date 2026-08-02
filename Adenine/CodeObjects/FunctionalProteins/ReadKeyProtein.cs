using Adenine.VirtualMachineEngine.RuntimeErrors;
using Adenine.VirtualMachineEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class ReadKeyProtein : FunctionalProtein
    {
        public override string Name => "read-key";

        public override int Index => 11;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            int index = (int)value;

            if (index < 0 || index >= VirtualMachine.Cell.Proteins.Length)
            {
                VirtualMachine.Throw<ProteinIndexOutOfRangeRuntimeError>();
                modifier = new ProgramRunModifier(true);
                return;
            }

            VirtualMachine.Cell.Proteins[index] = (int)Console.ReadKey().Key;
        }
    }
}
