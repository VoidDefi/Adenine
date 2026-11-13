using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Errors
{
    internal class WrongInsertPathPreprocessorError : Error
    {
        public WrongInsertPathPreprocessorError(int line) : base(line)
        {
        }

        public override string Message => "Preprocessor: insert path is incorrect";
    }
}
