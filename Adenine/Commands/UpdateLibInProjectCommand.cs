using Adenine.CodeObjects;
using System;
using System.IO;
using System.Xml.Linq;

namespace Adenine.Commands
{
    internal class UpdateLibInProjectCommand : Command
    {
        public override string Name => "updprojlib";

        public override string Description => $"update project lib. {Colors.Orange}WARNING!{Colors.Gray} old lib folder will be deleted! {Colors.Yellow}i{Colors.Gray} - index of project";

        public override Argument[] Arguments => [new Argument("i", ArgumentType.Number)];

        public override bool Execute(string[] args)
        {
            if (int.TryParse(args[0], out int index))
            {
                if (index < 0 || index >= Program.Projects.Length)
                {
                    Console.WriteLine($"{Colors.Red}Index was out of Projects range!{Colors.Reset}");
                }

                else
                {
                    string project = Program.Projects[index];

                    Console.Write($"{Colors.Orange}WARNING!{Colors.Reset} old lib folder will be deleted! (y/n): ");

                    if ((Console.ReadLine() ?? "n").ToLower() == "y")
                    {
                        string libDir = $"projects\\{project}\\lib";

                        Directory.Delete(libDir, true);
                        Program.CopyDirectory("lib", libDir);

                        Console.WriteLine($"Lib in project '{Colors.Cyan}{project}{Colors.Reset}' was successful updated!");
                    }

                    else
                    {
                        Console.WriteLine($"The lib update in the project was halted.");
                    }
                }
            }

            return true;
        }
    }
}
