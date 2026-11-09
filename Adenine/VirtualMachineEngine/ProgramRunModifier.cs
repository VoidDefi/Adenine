using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    public struct ProgramRunModifier
    {
        public bool NeedEnd { get; private set; } = false;

        public bool NeedExitFrom { get; private set; } = false;

        public int? InjectedPlasmidIndex { get; private set; } = null;

        public int? TranscriptGenIndex { get; private set; } = null;

        public bool TranscriptSaveEntryPoint {  get; private set; } = false;

        public int? TranscriptInstructionIndex { get; private set; } = null;

        public ProgramRunModifier()
        {
            NeedEnd = false;
        }

        public static ProgramRunModifier CreateProgramEnd()
        {
            ProgramRunModifier modifier = new();
            modifier.NeedEnd = true;

            return modifier;
        }

        public static ProgramRunModifier CreateExitFrom()
        {
            ProgramRunModifier modifier = new();
            modifier.NeedExitFrom = true;

            return modifier;
        }

        public static ProgramRunModifier CreateInject(int plasmid)
        {
            ProgramRunModifier modifier = new();
            modifier.InjectedPlasmidIndex = plasmid;

            return modifier;
        }

        public static ProgramRunModifier CreateTranscript(int genIndex, bool saveEntryPoint, int? instructionIndex = null)
        {
            ProgramRunModifier modifier = new();
            modifier.TranscriptGenIndex = genIndex;
            modifier.TranscriptSaveEntryPoint = saveEntryPoint;
            modifier.TranscriptInstructionIndex = instructionIndex;

            return modifier;
        }
    }
}
