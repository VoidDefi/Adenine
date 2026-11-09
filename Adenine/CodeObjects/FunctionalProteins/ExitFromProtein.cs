using Adenine.VirtualMachineEngine;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class ExitFromProtein : FunctionalProtein
    {
        public override string Name => "exit-from";

        public override int Index => 13;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            modifier = ProgramRunModifier.CreateExitFrom();
        }
    }
}
