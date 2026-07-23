using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class NotAvailableInContext : Error
    {
        public NotAvailableInContext(int line) : base(line)
        {
        }

        public override string Message => "Not available in this context";
    }
}
