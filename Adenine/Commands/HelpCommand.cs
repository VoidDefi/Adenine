using Adenine.CodeObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal class HelpCommand : Command
    {
        public override string Name => "help";

        public override string Description => "show all commands";

        public override Argument[] Arguments => [];

        public override bool Execute(string[] args)
        {
            int modeMaxLength = CommandRegistry.Commands.Max(c => c.IsMode ? GetLength(c) : 0);
            int commandMaxLength = CommandRegistry.Commands.Max(c => !c.IsMode ? GetLength(c) : 0);

            Console.WriteLine($"{Colors.Green}Modes:{Colors.Reset}");
            foreach (Command command in CommandRegistry.Commands)
            {
                if (command.IsMode)
                {
                    WriteCommand(command, modeMaxLength);
                }
            }

            Console.WriteLine($"");

            Console.WriteLine($"{Colors.Green}Commands:{Colors.Reset}");
            foreach (Command command in CommandRegistry.Commands)
            {
                if (!command.IsMode)
                {
                    WriteCommand(command, commandMaxLength);
                }
            }

            Console.WriteLine($"");

            return true;
        }

        private int GetLength(Command command)
        {
            int length = command.Name.Length;

            for (int i = 0; i < command.Arguments.Length; i++)
            {
                length += command.Arguments[i].Name.Length + 1;
            }

            return length;
        }

        private void WriteCommand(Command command, int maxLength)
        {
            int length = command.Name.Length;
            string commandHelp = command.Name;

            if (command.Arguments.Length > 0)
            {
                commandHelp += Colors.Yellow;
            }

            for (int i = 0; i < command.Arguments.Length; i++)
            {
                length += command.Arguments[i].Name.Length + 1;
                commandHelp += $" {command.Arguments[i].Name}";
            }

            commandHelp += new string(' ', maxLength - length);

            Console.WriteLine($" {commandHelp} {Colors.Gray}- {command.Description}{Colors.Reset}");
        }
    }
}
