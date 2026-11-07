using Adenine.VirtualMachineEngine;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class InjectProtein : FunctionalProtein
    {
        public override string Name => "inject";

        public override int Index => 12;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            int index = (int)value;

            modifier = new ProgramRunModifier(true, index);
        }
    }
}
