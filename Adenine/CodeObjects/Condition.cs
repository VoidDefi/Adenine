using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal struct Condition : ISerializing
    {
        public int ProteinIndex { get; private set; }

        public ComparisonOperator Operator { get; private set; }

        public float Value { get; private set; }

        public int ComparingProtein { get; private set; } = -1;

        public LogicOperator LogicOperator { get; private set; }

        public bool UseProteinValue => ComparingProtein != -1;

        public int ByteSize => 
            4 + //ProteinIndex
            1 + //Operator
            4 + //Value
            4 + //ComparingProtein
            1;  //LogicOperator

        public Condition(int proteinIndex, ComparisonOperator comparisonOperator, float value, LogicOperator logicOperator)
        {
            ProteinIndex = proteinIndex;
            Operator = comparisonOperator;
            Value = value;
            ComparingProtein = -1;
            LogicOperator = logicOperator;
        }

        public Condition(int proteinIndex, ComparisonOperator comparisonOperator, int comparingProtein, LogicOperator logicOperator)
        {
            ProteinIndex = proteinIndex;
            Operator = comparisonOperator;
            Value = 0;
            ComparingProtein = comparingProtein;
            LogicOperator = logicOperator;
        }

        public override string ToString()
        {
            string logic = LogicOperator != LogicOperator.None ? " " + LogicOperator.ToString().ToLower() : "";

            if (UseProteinValue)
            {
                return $"p#{ProteinIndex} {ComparisonOperatorParser.ToString(Operator)} p#{ComparingProtein}{logic}";
            }

            return $"p#{ProteinIndex} {ComparisonOperatorParser.ToString(Operator)} {Value}{logic}";
        }

        public byte[] Serialize()
        {
            List<byte> bytes = [];

            bytes.AddRange(BitConverter.GetBytes(ProteinIndex));
            bytes.Add((byte)Operator);
            bytes.AddRange(BitConverter.GetBytes(Value));
            bytes.AddRange(BitConverter.GetBytes(ComparingProtein));
            bytes.Add((byte)LogicOperator);

            if (bytes.Count != ByteSize)
                throw new Exception("Invalid size");

            return bytes.ToArray();
        }

        public void DeSerialize(byte[] bytes, int startIndex = 0)
        {
            if (bytes.Length < ByteSize)
                throw new Exception("Invalid size");

            int currentOffset = startIndex;

            ProteinIndex = BitConverter.ToInt32(bytes, currentOffset);
            currentOffset += 4;

            byte rawOperator = bytes[currentOffset];
            currentOffset += 1;

            Value = BitConverter.ToSingle(bytes, currentOffset);
            currentOffset += 4;

            ComparingProtein = BitConverter.ToInt32(bytes, currentOffset);
            currentOffset += 4;

            byte rawLogicOperator = bytes[currentOffset];
            currentOffset += 1;

            if (rawOperator > Enum.GetValues(typeof(ComparisonOperator)).Length - 1)
                throw new Exception("The Operator value was outside the bounds of the enum");

            if (rawLogicOperator > Enum.GetValues(typeof(LogicOperator)).Length - 1)
                throw new Exception("The LogicOperator value was outside the bounds of the enum");

            Operator = (ComparisonOperator)rawOperator;
            LogicOperator = (LogicOperator)rawLogicOperator;
        }
    }
}
