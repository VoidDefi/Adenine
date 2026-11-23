using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal class CompileAndRunMode : Command
    {
        public override string Name => "t";

        public override string Description => "compile code and run";

        public override Argument[] Arguments => [];

        public override bool IsMode => true;

        public override bool Execute(string[] args)
        {
            if (Program.Compile())
                Program.Run();

            return true;
        }
    }
}
