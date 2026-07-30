using Adenine.VirtualMachineEngine;

namespace Adenine.CodeObjects.FunctionalProteins
{
    internal abstract class FunctionalProtein
    {
        public abstract string Name { get; }

        public abstract int Index { get; }

        public abstract void Invoke(float value, ref ProgramRunModifier modifier);
    }
}
