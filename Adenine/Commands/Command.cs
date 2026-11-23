using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Commands
{
    internal abstract class Command
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract Argument[] Arguments { get; }

        public virtual bool IsMode { get; } = false;

        public abstract bool Execute(string[] args);
    }
}
