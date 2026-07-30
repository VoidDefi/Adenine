using Adenine.VirtualMachineEngine;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal class WriteProtein : FunctionalProtein
    {
        public override string Name => "write";

        public override int Index => 0;

        public override void Invoke(float value, ref ProgramRunModifier modifier)
        {
            Console.Write((char)(int)value);
        }
    }
}
