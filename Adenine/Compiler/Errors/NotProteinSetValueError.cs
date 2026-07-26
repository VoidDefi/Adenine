using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotProteinSetValueError : Error
    {
        public NotProteinSetValueError(int line) : base(line)
        {
        }

        public override string Message => "Undefined value assignment";
    }
}
