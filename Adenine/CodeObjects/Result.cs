using Adenine.Compiler.NotCompiledObjects;
using Adenine.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal struct Result : ISerializing
    {
        public ProteinOperation Operation { get; private set; }

        public bool Action { get; private set; }

        public int ProteinIndex { get; private set; }

        public float Value { get; private set; }

        public int InputProtein { get; private set; } = -1;

        public bool GetValueFrom { get; private set; } = false;

        public bool UseProteinValue => InputProtein != -1;

        public int ByteSize =>
            1 + //Operation
            1 + //Action
            4 + //ProteinIndex
            4 + //Value
            4;//+ //InputProtein
            //1;  //GetValueFrom

        public Result(ProteinOperation operation, bool action, int proteinIndex, float value, bool getValueFrom = false)
        {
            Operation = operation;
            Action = action;
            ProteinIndex = proteinIndex;
            Value = value;
            InputProtein = -1;
            GetValueFrom = getValueFrom;
        }

        public Result(ProteinOperation operation, bool action, int proteinIndex, int inputProtein, bool getValueFrom = false)
        {
            Operation = operation;
            Action = action;
            ProteinIndex = proteinIndex;
            Value = 0;
            InputProtein = inputProtein;
            GetValueFrom = getValueFrom;
        }

        public override string ToString()
        {
            string operation = ProteinOperationParser.ToString(Operation);
            string action = Action ? "action " : "";

            if (UseProteinValue)
            {
                return $"{operation} {action}p#{ProteinIndex}(p#{InputProtein})";
            }

            return $"{operation} {action}p#{ProteinIndex}({Value})";
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new(ByteSize);

            bytes.Add((byte)Operation);
            bytes.Add((byte)(Action ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes(ProteinIndex));
            bytes.AddRange(BitConverter.GetBytes(Value));
            bytes.AddRange(BitConverter.GetBytes(InputProtein));

            if (bytes.Count != ByteSize)
                throw new Exception("Invalid size");

            return bytes.ToArray();
        }

        public void DeSerialize(byte[] bytes, int startIndex = 0)
        {
            if (bytes.Length < ByteSize)
                throw new Exception("Invalid size");

            int currentOffset = startIndex;

            byte rawOperation = bytes[currentOffset];
            currentOffset += 1;

            Action = bytes[currentOffset] > 0;
            currentOffset += 1;

            ProteinIndex = BitConverter.ToInt32(bytes, currentOffset);
            currentOffset += 4;

            Value = BitConverter.ToSingle(bytes, currentOffset);
            currentOffset += 4;

            InputProtein = BitConverter.ToInt32(bytes, currentOffset);
            currentOffset += 4;

            if (rawOperation > Enum.GetValues(typeof(ComparisonOperator)).Length - 1)
                throw new Exception("The Operation value was outside the bounds of the enum");

            Operation = (ProteinOperation)rawOperation;
        }
    }
}
