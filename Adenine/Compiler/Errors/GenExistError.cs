using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class GenExistError : Error
    {
        public GenExistError(int line) : base(line)
        {
        }

        public override string Message => "Gen with this name already exists";
    }
}
