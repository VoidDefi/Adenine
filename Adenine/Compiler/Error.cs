using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler
{
    public abstract class Error
    {
        public abstract string Message { get; }

        public string? Path { get; set; }

        public int Line { get; set; }
    }
}
