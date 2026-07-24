using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotDefinedBodyError : Error
    {
        public NotDefinedBodyError(int line) : base(line)
        {
        }

        public override string Message => "The gen/condition/result body was not defined";
    }
}
