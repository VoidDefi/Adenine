using Adenine.VirtualMachineEngine;
using Adenine.VirtualMachineEngine.RuntimeErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class SetCursorYProtein : FunctionalProtein
    {
        public override string Name => "set-cursor-y";

        public override int Index => 5;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            var position = Console.GetCursorPosition();
            Console.SetCursorPosition(position.Left, (int)value);
        }
    }
}
