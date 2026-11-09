using Adenine.VirtualMachineEngine;

namespace Adenine.CodeObjects.FunctionalProteins
{
    public class KillProtein : FunctionalProtein
    {
        public override string Name => "kill";

        public override int Index => 1;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            modifier = ProgramRunModifier.CreateProgramEnd();
        }
    }
}
