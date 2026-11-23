using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal class CompileMode : Command
    {
        public override string Name => "c";

        public override string Description => "compile code";

        public override Argument[] Arguments => [];

        public override bool IsMode => true;

        public override bool Execute(string[] args)
        {
            Program.Compile();
            return true;
        }
    }
}
