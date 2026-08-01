using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    public class EntryPoint
    {
        public int gen = -1;
        public int instruction = -1;

        public EntryPoint(int gen, int instruction)
        {
            this.gen = gen;
            this.instruction = instruction;
        }

        public EntryPoint() : this(-1, -1)
        {

        }

        public void Clear()
        {
            gen = -1;
            instruction = -1;
        }
    }
}
