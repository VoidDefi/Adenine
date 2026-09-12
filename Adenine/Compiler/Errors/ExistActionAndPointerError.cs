using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class ExistActionAndPointerError : Error
    {
        public ExistActionAndPointerError(int line) : base(line)
        {
        }

        public override string Message => "A pointer cannot be used on functional proteins";
    }
}
