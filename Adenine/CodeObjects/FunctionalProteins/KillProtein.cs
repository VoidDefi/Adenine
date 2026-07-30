using Adenine.VirtualMachineEngine;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class KillProtein : FunctionalProtein
    {
        public override string Name => "kill";

        public override int Index => 1;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            modifier = new ProgramRunModifier(true);
        }
    }
}
