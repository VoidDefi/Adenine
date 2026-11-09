using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    public class Frame
    {
        /*int plasmidIndex = -1;

        public int PlasmidIndex
        {
            get => plasmidIndex;
            set
            {
                plasmidIndex = value;
            }
        }

        /// <summary>
        /// Entry Point of call plasmid
        /// </summary>
        public EntryPoint EntryPoint { get; private set; }

        public CallsStackObject(int plasmidIndex, EntryPoint entryPoint)
        {
            PlasmidIndex = plasmidIndex;
            EntryPoint = entryPoint;
        }*/

        public int PlasmidIndex { get; private set; }

        public int GenIndex { get; set; }

        public Frame(int plasmidIndex)
        { 
            if (plasmidIndex < -1 || plasmidIndex >= VirtualMachine.Cell.Plasmids.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            PlasmidIndex = plasmidIndex;
        }
    }
}
