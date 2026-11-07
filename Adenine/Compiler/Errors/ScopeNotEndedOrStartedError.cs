using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class ScopeNotEndedOrStartedError : Error
    {
        public ScopeNotEndedOrStartedError(int line) : base(line)
        {
        }

        public override string Message => "The scope was not started or ended";
    }
}
