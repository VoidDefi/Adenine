using Adenine.CodeObjects;
using Adenine.CodeObjects.FunctionalProteins;
using Adenine.VirtualMachineEngine.RuntimeErrors;
using System.Diagnostics.CodeAnalysis;

namespace Adenine.VirtualMachineEngine
{
    public static class VirtualMachine
    {
        public static Cell Cell { get; private set; }

        public static DebugData DebugData { get; private set; }

        public static bool IsStarted { get; private set; }

        public static long Iteration { get; private set; }

        public static int CurrentPlasmidIndex { get; private set; } = -1;

        public static int CurrentGenIndex { get; private set; }

        public static int CurrentInstruction { get; private set; } = -1;

        public static Stack<Frame> CallStack { get; private set; }

        private static TraceBlock traceBlock = TraceBlock.Result;

        public static void Setup(Cell cell, DebugData debugData = null)
        {
            Cell = cell;
            DebugData = debugData;

            CallStack = new Stack<Frame>();

            IsStarted = false;
            Iteration = 0;
        }

        public static void Start()
        {
            IsStarted = true;

            CallStack.Push(new Frame(-1));

            while (IsStarted)
            {
                bool needEnd = PassGens(Cell.Gens);

                if (needEnd)
                {
                    IsStarted = false;
                    break;
                }

                Iteration++;
            }
        }

        public static bool PassGens(Gen[] gens)
        {
            bool forcefullyExecute = false;
            int startInstruction = 0;

            for (int i = 0; i < gens.Length; i++)
            {
                Gen gen = gens[i];
                CallStack.Peek().GenIndex = i;

                if (gen == null) throw new NullReferenceException();

                bool isSuccess = CheckAllConditions(gen, out bool errorThrowed);

                if (errorThrowed)
                    return true;

                if (isSuccess || forcefullyExecute)
                {
                    forcefullyExecute = false;

                    int transcriptGenIndex = -1;

                    for (int j = startInstruction; j < gen.Results.Length; j++)
                    {
                        startInstruction = 0;

                        var modifier = ExecuteOneResult(gen.Results[j]);

                        if (modifier.HasValue)
                        {
                            if (modifier.Value.NeedEnd)
                                return true;

                            if (modifier.Value.NeedExitFrom)
                            {
                                if (CallStack.Count <= 1)
                                {
                                    Throw<CanNotExitFromMainRuntimeError>();
                                    return true;
                                }

                                return false;
                            }

                            if (modifier.Value.TranscriptGenIndex.HasValue)
                            {
                                if (modifier.Value.TranscriptSaveEntryPoint)
                                {
                                    EntryPoint entry = CallStack.Peek().EntryPoint;
                                    entry.gen = i;
                                    entry.instruction = j;
                                }

                                transcriptGenIndex = modifier.Value.TranscriptGenIndex.Value;

                                if (transcriptGenIndex < 0 || transcriptGenIndex >= gens.Length)
                                {
                                    Throw<GenIndexOutOfRangeRuntimeError>();
                                    return true;
                                }

                                int? instruction = modifier.Value.TranscriptGenIndex.Value;

                                if (instruction.HasValue)
                                {
                                    if (instruction < 0)
                                    {
                                        Throw<InstructionIndexOutOfRangeRuntimeError>();
                                        return true;
                                    }

                                    startInstruction = instruction.Value;
                                }

                                break;
                            }

                            if (modifier.Value.InjectedPlasmidIndex.HasValue)
                            {
                                int plasmidIndex = modifier.Value.InjectedPlasmidIndex.Value;

                                if (plasmidIndex < 0 || plasmidIndex >= Cell.Plasmids.Length)
                                {
                                    Throw<PlasmidIndexOutOfRangeRuntimeError>();
                                    return true;
                                }

                                if (CallStack.Count > 1000)
                                {
                                    Throw<StackOverflowRuntimeError>();
                                    return true;
                                }

                                CallStack.Push(new Frame(plasmidIndex));

                                bool needEnd = PassGens(Cell.Plasmids[plasmidIndex].ToArray());

                                if (needEnd)
                                    return true;

                                CallStack.Pop();
                            }
                        }
                    }

                    if (transcriptGenIndex >= 0)
                    {
                        i = transcriptGenIndex - 1;
                        forcefullyExecute = true;

                        continue;
                    }
                }
            }

            return false;
        }

        public static ProgramRunModifier? ExecuteOneResult(Result result)
        {
            float value = result.Value;

            if (result.UseProteinValue)
            {
                if (IsInvalidProteinIndex(result.InputProtein))
                {
                    Throw<ProteinIndexOutOfRangeRuntimeError>();
                    return ProgramRunModifier.CreateProgramEnd();
                }

                if (result.GetValueFrom)
                {
                    int index = (int)Cell.Proteins[result.InputProtein];

                    if (IsInvalidProteinIndex(index))
                    {
                        Throw<ProteinIndexOutOfRangeRuntimeError>();
                        return ProgramRunModifier.CreateProgramEnd();
                    }

                    value = Cell.Proteins[index];
                }
                else
                {
                    value = Cell.Proteins[result.InputProtein];
                }
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
                        return ProgramRunModifier.CreateProgramEnd();
                    }

                    ProgramRunModifier modifier = new();

                    action.Invoke(value, ref modifier);

                    return modifier;
                }
            }

            else
            {
                if (IsInvalidProteinIndex(result.ProteinIndex))
                {
                    Throw<ProteinIndexOutOfRangeRuntimeError>();
                    return ProgramRunModifier.CreateProgramEnd();
                }

                int proteinIndex = result.ProteinIndex;

                if (result.Pointer)
                {
                    proteinIndex = (int)Cell.Proteins[proteinIndex];

                    if (IsInvalidProteinIndex(proteinIndex))
                    {
                        Throw<ProteinIndexOutOfRangeRuntimeError>();
                        return ProgramRunModifier.CreateProgramEnd();
                    }
                }

                switch (result.Operation)
                {
                    case ProteinOperation.Set:
                        Cell.Proteins[proteinIndex] = value;
                        break;
                    case ProteinOperation.Add:
                        Cell.Proteins[proteinIndex] += value;
                        break;
                    case ProteinOperation.Subtract:
                        Cell.Proteins[proteinIndex] -= value;
                        break;
                    case ProteinOperation.Multiply:
                        Cell.Proteins[proteinIndex] *= value;
                        break;
                    case ProteinOperation.Divide:

                        if (value == 0)
                        {
                            Throw<DivisionByZeroRuntimeError>();
                            return ProgramRunModifier.CreateProgramEnd();
                        }

                        Cell.Proteins[proteinIndex] /= value;
                        break;
                    case ProteinOperation.DivideByModule:

                        if (value == 0)
                        {
                            Throw<DivisionByZeroRuntimeError>();
                            return ProgramRunModifier.CreateProgramEnd();
                        }

                        Cell.Proteins[proteinIndex] %= value;
                        break;
                }
            }

            return null;
        }

        public static bool CheckAllConditions(Gen gen, out bool errorThrowed)
        {
            if (gen == null) 
            { 
                errorThrowed = false;
                return false; 
            }

            int count = gen.Conditions.Length;

            if (count == 1)
            {
                bool result = CheckOneCondition(gen.Conditions[0], out bool hasError);
                errorThrowed = hasError;
                return result;
            }

            else if (count > 1)
            {
                bool result = CheckOneCondition(gen.Conditions[0], out bool hasError);

                if (hasError)
                {
                    errorThrowed = true;
                    return false;
                }

                for (int i = 1; i < count; i++)
                {
                    Condition condition = gen.Conditions[i];
                    bool currentResult = CheckOneCondition(condition, out bool hasError1);

                    if (hasError1)
                    {
                        errorThrowed = true;
                        return false;
                    }

                    switch (gen.Conditions[i - 1].LogicOperator)
                    {
                        case LogicOperator.And:
                            result = result && currentResult;
                            break;
                        case LogicOperator.Or:
                            result = result || currentResult;
                            break;
                    }
                }

                errorThrowed = false;
                return result;
            }

            errorThrowed = false;
            return false;
        }

        private static bool CheckOneCondition(Condition condition, out bool errorThrowed)
        {
            traceBlock = TraceBlock.Condition;

            if (IsInvalidProteinIndex(condition.ProteinIndex))
            {
                Throw<ProteinIndexOutOfRangeRuntimeError>();
                errorThrowed = true;
                traceBlock = TraceBlock.Result;
                return false;
            }

            float proteinValue = Cell.Proteins[condition.ProteinIndex];

            float comparingValue = condition.Value;

            if (condition.UseProteinValue)
            {
                if (IsInvalidProteinIndex(condition.ComparingProtein))
                {
                    Throw<ProteinIndexOutOfRangeRuntimeError>();
                    errorThrowed = true;
                    traceBlock = TraceBlock.Result;
                    return false;
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

            errorThrowed = false;
            traceBlock = TraceBlock.Result;
            return flag;
        }

        private static void Throw(RuntimeError error)
        {
            Console.WriteLine();
            Console.WriteLine(error.ToString());
            //Logging.Program.EmitError(error);

            IsStarted = false;
        }

        public static void Throw<T>() where T : RuntimeError
        {
            Throw(Activator.CreateInstance(typeof(T), [GetTrace()]) as RuntimeError ?? throw new Exception());
        }

        private static Trace GetTrace()
        {
            /*if (GenIndex < 0 || GenIndex >= (Cell?.Gens.Length ?? -1))
                throw new IndexOutOfRangeException("gen index was outside the bounds of the gens");

            string genName = $"g#{GenIndex}";

            if (DebugData != null)
            {
                if (GenIndex >= DebugData.GenNames.Length)
                    throw new IndexOutOfRangeException("gen index was outside the bounds of the debug gen name");

                genName = DebugData.GenNames[GenIndex];
            }

            return new Trace(genName, CurrentBlock, null);*/

            return new Trace(CallStack, traceBlock);
        }

        private static bool IsInvalidProteinIndex(int index)
        {
            return index < 0 || index >= Cell.Proteins.Length;
        }
    }
}
