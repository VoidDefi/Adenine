using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    public class NotEndCommentError : Error
    {
        public NotEndCommentError(int line) : base(line)
        {
        }

        public override string Message => "A multi-line comment must be closed with '*/'";
    }
}
