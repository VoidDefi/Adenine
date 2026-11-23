using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Decompiler
{
    internal class CodeFile
    {
        public string Code { get; private set; }

        public string FileName { get; private set; }

        public CodeFile(string code, string fileName) 
        {
            Code = code;
            FileName = fileName;
        }
    }
}
