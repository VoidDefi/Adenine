using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal interface ISerializing
    {
        public abstract int ByteSize { get; }

        public abstract byte[] Serialize();

        public abstract void DeSerialize(byte[] bytes, int startIndex = 0);
    }
}
