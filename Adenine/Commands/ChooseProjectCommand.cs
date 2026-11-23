using Adenine.CodeObjects;
using System;

namespace Adenine.Commands
{
    internal class ChooseProjectCommand : Command
    {
        public override string Name => "chooseproj";

        public override string Description => $"select project. {Colors.Yellow}i{Colors.Gray} - index of project. It can be obtained from {Colors.Green}projects{Colors.Gray} command";

        public override Argument[] Arguments => [ new Argument("i", ArgumentType.Number) ];

        public override bool Execute(string[] args)
        {
            if (int.TryParse(args[0], out int index))
            {
                if (index < 0)
                {
                    Program.CurrentProject = "";
                    File.WriteAllText(Program.CurrentProjectFile, Program.CurrentProject);

                    Console.WriteLine($"Selected project was successful reset!");
                }

                else
                {
                    if (index >= Program.Projects.Length)
                    {
                        Console.WriteLine($"{Colors.Red}Index was out of Projects range!{Colors.Reset}");
                    }

                    else
                    {
                        Program.CurrentProject = Program.Projects[index];
                        File.WriteAllText(Program.CurrentProjectFile, Program.CurrentProject);

                        Console.WriteLine($"Project '{Colors.Cyan}{Program.CurrentProject}{Colors.Reset}' was successful selected!");
                    }
                }
            }

            return true;
        }
    }
}
