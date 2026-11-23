using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal struct Argument
    {
        public string Name { get; private set; }

        public ArgumentType Type { get; private set; }

        public Argument(string name, ArgumentType type)
        {
            Name = name;
            Type = type;
        }
    }

    internal enum ArgumentType
    {
        Number,
        String
    }
}
