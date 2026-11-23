using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal class GetCurrentProjectCommand : Command
    {
        public override string Name => "curproj";

        public override string Description => "show selected project";

        public override Argument[] Arguments => [];

        public override bool Execute(string[] args)
        {
            if (Program.CurrentProject == "")
            {
                Console.WriteLine($"Project was not selected.");
            }

            else
            {
                Console.WriteLine($"Current project is {Colors.Cyan}{Program.CurrentProject}{Colors.Reset}");
            }

            return true;
        }
    }
}
