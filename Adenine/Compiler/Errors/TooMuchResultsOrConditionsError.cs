using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class TooMuchResultsOrConditionsError : Error
    {
        public TooMuchResultsOrConditionsError(int line) : base(line)
        {
        }

        public override string Message => "Too many conditions or results in one gen. Only one condition and one result are needed";
    }
}
