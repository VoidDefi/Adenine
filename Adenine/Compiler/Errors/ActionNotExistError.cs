using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class ActionNotExistError : Error
    {
        public ActionNotExistError(int line) : base(line)
        {
        }

        public override string Message => "does not exist functional protein (action) with this name";
    }
}
