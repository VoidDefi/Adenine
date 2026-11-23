using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal class RunMode : Command
    {
        public override string Name => "r";

        public override string Description => "run compiled program";

        public override Argument[] Arguments => [];

        public override bool IsMode => true;

        public override bool Execute(string[] args)
        {
            Program.Run();
            return true;
        }
    }
}
