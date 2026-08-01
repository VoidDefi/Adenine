using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotReadCurrentResultError : Error
    {
        public NotReadCurrentResultError(int line) : base(line)
        {
        }

        public override string Message => "The signature of One Result in result block is invalid and therefore it was not possible to read it";
    }
}
