using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal struct Result
    {
        public ProteinOperation Operation { get; private set; }

        public int ProteinIndex { get; private set; }

        public float Value { get; private set; }

        public Result(ProteinOperation operation, int proteinIndex, float value)
        {
            Operation = operation;
            ProteinIndex = proteinIndex;
            Value = value;
        }
    }
}
