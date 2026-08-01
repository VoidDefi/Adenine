using Adenine.CodeObjects;
using Adenine.CodeObjects.FunctionalProteins;
using Adenine.VirtualMachineEngine.RuntimeErrors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.VirtualMachineEngine
{
    internal static class VirtualMachine
    {
        public static Cell Cell { get; private set; }

        public static DebugData DebugData { get; private set; }

        public static bool IsProgramStarted { get; private set; }

        public static int GenIndex { get; private set; }

        private static TraceBlock CurrentBlock { get; set; }

        public static int StepTimeMs { get; set; } = 0;

        public static bool LoggingActivated { get; set; } = false;

        public static int EntryPoint { get; set; } = -1;

        public static long IterationCounter { get; set; } = 0;

        public static void Setup(Cell cell, DebugData debugData)
        {
            if (cell == null) throw new ArgumentNullException(nameof(cell));

            if (!IsProgramStarted)
            {
                Cell = cell;
                DebugData = debugData;

                GenIndex = 0;
                CurrentBlock = 0;
                EntryPoint = -1;
                IterationCounter = 0;
            }
        }

        public static void Start()
        {
            if (Cell == null) return;

            IsProgramStarted = true;

            if (LoggingActivated)
            Logging.Program.Start();


            bool forcedlyExecute = false;

            while (IsProgramStarted)
            {
                EntryPoint = -1;

                for (int i = 0; i < Cell.Gens.Length; i++)
                {
                    if (StepTimeMs > 0)
                        Thread.Sleep(StepTimeMs);

                    Logging.Program.EmitLine($"Main iterate: {IterationCounter}");

                    Gen gen = Cell.Gens[i];
                    GenIndex = i;

                    bool resultFlag = false;
                    LogicOperator logicOperator = LogicOperator.None;

                    CurrentBlock = TraceBlock.Condition;

                    Logging.Program.EmitCurrentGen();
                    Logging.Program.EmitCurrentProteinsState();
                    Logging.Program.EmitLine("condition: ");

                    bool needGoto = false;

                    if (!forcedlyExecute)
                    {
                        for (int j = 0; j < gen.Conditions.Length; j++)
                        {
                            Condition condition = gen.Conditions[j];

                            if (IsValidProteinIndex(condition.ProteinIndex))
                            {
                                Throw<ProteinIndexOutOfRangeRuntimeError>();
                                return;
                            }

                            float proteinValue = Cell.Proteins[condition.ProteinIndex];

                            float comparingValue = condition.Value;

                            if (condition.UseProteinValue)
                            {
                                if (IsValidProteinIndex(condition.ComparingProtein))
                                {
                                    Throw<ProteinIndexOutOfRangeRuntimeError>();
                                    return;
                                }

                                comparingValue = Cell.Proteins[condition.ComparingProtein];
                            }

                            bool flag = false;

                            //comparing
                            switch (condition.Operator)
                            {
                                case ComparisonOperator.Equal:
                                    flag = proteinValue == comparingValue;
                                    break;
                                case ComparisonOperator.NotEqual:
                                    flag = proteinValue != comparingValue;
                                    break;
                                case ComparisonOperator.Greater:
                                    flag = proteinValue > comparingValue;
                                    break;
                                case ComparisonOperator.Less:
                                    flag = proteinValue < comparingValue;
                                    break;
                                case ComparisonOperator.LessOrEqual:
                                    flag = proteinValue <= comparingValue;
                                    break;
                                case ComparisonOperator.GreaterOrEqual:
                                    flag = proteinValue >= comparingValue;
                                    break;
                            }

                            if (logicOperator == LogicOperator.None)
                            {
                                resultFlag = flag;
                            }

                            else
                            {
                                if (logicOperator == LogicOperator.And)
                                    resultFlag = resultFlag && flag;

                                else if (logicOperator == LogicOperator.Or)
                                    resultFlag = resultFlag || flag;
                            }

                            logicOperator = condition.LogicOperator;
                        }
                    }

                    if (resultFlag || forcedlyExecute)
                    {
                        forcedlyExecute = false;

                        Logging.Program.EmitLine("result: ");

                        CurrentBlock = TraceBlock.Result;

                        for (int j = 0; j < gen.Results.Length; j++)
                        {
                            Result result = gen.Results[j];

                            float value = result.Value;

                            if (result.UseProteinValue)
                            {
                                if (IsValidProteinIndex(result.InputProtein))
                                {
                                    Throw<ProteinIndexOutOfRangeRuntimeError>();
                                    return;
                                }

                                value = Cell.Proteins[result.InputProtein];
                            }

                            if (result.Action)
                            {
                                if (result.Operation != ProteinOperation.Set)
                                    throw new Exception("invalid signature");

                                if (FunctionalProteinsRegistry.Proteins.TryGetValue(result.ProteinIndex, out FunctionalProtein action))
                                {
                                    if (action == null)
                                    {
                                        Throw<NotExistFunctionalProteinRuntimeError>();
                                        return;
                                    }

                                    ProgramRunModifier modifier = new();

                                    action.Invoke(value, ref modifier);

                                    if (modifier.NeedEnd)
                                        return;

                                    if (modifier.NeedGoto)
                                    {
                                        int index = modifier.GotoIndex;

                                        if (index < 0 || index >= Cell.Gens.Length)
                                        {
                                            Throw<GenIndexOutOfRangeRuntimeError>();
                                            return;
                                        }

                                        needGoto = true;
                                        forcedlyExecute = modifier.ForcedlyExecuteJumpedGen;

                                        if (modifier.SaveEntryPoint)
                                            EntryPoint = i;

                                        i = index;
                                        GenIndex = index;
                                        break;
                                    }
                                }

                                else
                                {
                                    Throw<NotExistFunctionalProteinRuntimeError>();
                                    return;
                                }
                            }

                            else
                            {
                                if (IsValidProteinIndex(result.ProteinIndex))
                                {
                                    Throw<ProteinIndexOutOfRangeRuntimeError>();
                                    return;
                                }

                                switch (result.Operation)
                                {
                                    case ProteinOperation.Set:
                                        Cell.Proteins[result.ProteinIndex] = value;
                                        break;
                                    case ProteinOperation.Add:
                                        Cell.Proteins[result.ProteinIndex] += value;
                                        break;
                                    case ProteinOperation.Subtract:
                                        Cell.Proteins[result.ProteinIndex] -= value;
                                        break;
                                    case ProteinOperation.Multiply:
                                        Cell.Proteins[result.ProteinIndex] *= value;
                                        break;
                                    case ProteinOperation.Divide:

                                        if (value == 0)
                                        {
                                            Throw<DivisionByZeroRuntimeError>();
                                            return;
                                        }

                                        Cell.Proteins[result.ProteinIndex] /= value;
                                        break;
                                    case ProteinOperation.DivideByModule:

                                        if (value == 0)
                                        {
                                            Throw<DivisionByZeroRuntimeError>();
                                            return;
                                        }

                                        Cell.Proteins[result.ProteinIndex] %= value;
                                        break;
                                }
                            }
                        }
                    }

                    if (needGoto)
                    {
                        i--;
                        continue;
                    }
                }

                IterationCounter++;

                /*if (Console.CapsLock)
                {
                    IsProgramStarted = false;
                    return;
                }*/
            }
        }

        private static void Throw(RuntimeError error)
        {
            Console.WriteLine();
            Console.WriteLine(error.ToString());
            Logging.Program.EmitError(error);

            IsProgramStarted = false;
        }

        public static void Throw<T>() where T : RuntimeError
        {
            Throw(Activator.CreateInstance(typeof(T), [GetTrace()]) as RuntimeError);
        }

        private static Trace GetTrace()
        {
            if (GenIndex < 0 || GenIndex >= (Cell?.Gens.Length ?? -1))
                throw new IndexOutOfRangeException("gen index was outside the bounds of the gens");

            string genName = $"g#{GenIndex}";

            if (DebugData != null)
            {
                if (GenIndex >= DebugData.GenNames.Length)
                    throw new IndexOutOfRangeException("gen index was outside the bounds of the debug gen name");

                genName = DebugData.GenNames[GenIndex];
            }

            return new Trace(genName, CurrentBlock);
        }

        private static bool IsValidProteinIndex(int index)
        {
            return index < 0 || index >= Cell.Proteins.Length;
        }
    }
}
