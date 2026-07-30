using Adenine.VirtualMachineEngine;
using Adenine.VirtualMachineEngine.RuntimeErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class GetCursorXProtein : FunctionalProtein
    {
        public override string Name => "get-cursor-x";

        public override int Index => 2;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            int index = (int)value;

            if (index < 0 || index >= VirtualMachine.Cell.Proteins.Length)
            {
                VirtualMachine.Throw<ProteinIndexOutOfRangeRuntimeError>();
                modifier = new ProgramRunModifier(true);
                return;
            }

            var position = Console.GetCursorPosition();

            VirtualMachine.Cell.Proteins[index] = position.Left;
        }
    }
}
