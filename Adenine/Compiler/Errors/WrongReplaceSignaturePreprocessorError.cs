using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class WrongReplaceSignaturePreprocessorError : Error
    {
        public WrongReplaceSignaturePreprocessorError(int line) : base(line)
        {
        }

        public override string Message => "Preprocessor: replace signature is wrong? please use: ~replace MY-REPLACE : text, after replacing";
    }
}
