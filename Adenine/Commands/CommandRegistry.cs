using Adenine.CodeObjects.FunctionalProteins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal static class CommandRegistry
    {
        public static List<Command> Commands { get; private set; }

        public static void Setup()
        {
            Assembly assembly = typeof(Command).Assembly;
            Type type = typeof(Command);

            Commands = new();

            List<Type> types = assembly.GetTypes().Where(t => t.IsSubclassOf(type)).ToList();

            foreach (Type command in types)
            {
                if (command.IsAbstract) continue;

                Commands.Add(Activator.CreateInstance(command) as Command);
            }

            foreach (Command command in Commands)
            {
                List<Command> matches = Commands.FindAll(c => c.Name == command.Name);

                if (matches.Count > 1)
                {
                    throw new Exception("Some parameters of command coincide");
                }
            }
        }
    }
}
