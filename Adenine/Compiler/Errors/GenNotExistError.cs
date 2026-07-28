using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class GenNotExistError : Error
    {
        public GenNotExistError(int line) : base(line)
        {
        }

        public override string Message => "Gen with this name does not exist";
    }
}
