using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal class DecompileMode : Command
    {
        public override string Name => "d";

        public override string Description => "decompile program";

        public override Argument[] Arguments => [];

        public override bool IsMode => true;

        public override bool Execute(string[] args)
        {
            Program.Decompile();
            return true;
        }
    }
}
