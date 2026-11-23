using Adenine.CodeObjects;
using System;
using System.Xml.Linq;

namespace Adenine.Commands
{
    internal class NewProjectCommand : Command
    {
        public override string Name => "newproj";

        public override string Description => $"create new project. {Colors.Yellow}name{Colors.Gray} - name of project";

        public override Argument[] Arguments => [new Argument("name", ArgumentType.String)];

        private string BaseCode => "gen init\r\n{\r\n\tcondition\r\n\t{\r\n\t\tnot exist start\r\n\t}\r\n\r\n\tresult\r\n\t{\r\n\t\tset start(1) next\r\n\t\tset action kill(0)\r\n\t}\r\n}";

        public override bool Execute(string[] args)
        {
            if (args[0].IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || args[0].Length == 0)
            {
                Console.WriteLine($"{Colors.Red}Project name contains invalid chars!{Colors.Reset}");
            }

            else
            {
                string directory = $"projects\\{args[0]}";

                if (Directory.Exists(directory))
                {
                    Console.WriteLine($"{Colors.Red}Project witch this name exits!{Colors.Reset}");
                }

                else
                {
                    Directory.CreateDirectory(directory);
                    File.WriteAllText($"{directory}\\code.adn", BaseCode);

                    Program.FindProjects();
                    Console.WriteLine($"Project '{Colors.Cyan}{args[0]}{Colors.Reset}' was successful created!");
                }
            }

            return true;
        }
    }
}
