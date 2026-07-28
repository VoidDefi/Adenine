using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class ProteinNotInitedError : Error
    {
        public ProteinNotInitedError(int line) : base(line)
        {
        }

        public override string Message => "No gen initializes this protein";
    }
}
