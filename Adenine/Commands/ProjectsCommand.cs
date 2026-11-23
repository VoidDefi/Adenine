using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal class ProjectsCommand : Command
    {
        public override string Name => "projects";

        public override string Description => "show all projects in directory 'projects'";

        public override Argument[] Arguments => [];

        public override bool Execute(string[] args)
        {
            if (Program.Projects.Length == 0)
            {
                Console.WriteLine($"Projects do not exist. Please, create first project. To do this, use '{Colors.Green}newproj{Colors.Reset}' command");
            }

            else
            {
                Console.WriteLine($"{Colors.Green}Pojects:{Colors.Reset}");
                Console.WriteLine($"-1    {Colors.Gray}- reset selected project{Colors.Reset}");

                for (int i = 0; i < Program.Projects.Length; i++)
                {
                    string index = i.ToString();

                    string space = new string(' ', Math.Clamp(4 - index.Length, 0, 4));

                    Console.WriteLine($" {index}{space} {Colors.Gray}- {Program.Projects[i]}{Colors.Reset}");
                }

                Console.WriteLine($"");
            }

            return true;
        }
    }
}
