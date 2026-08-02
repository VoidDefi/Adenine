using Adenine.CodeObjects;
using Adenine.Compiler;
using Adenine.Compiler.Registry;
using Adenine.Compiler.Serializing;
using Adenine.VirtualMachineEngine;

namespace Adenine
{
    public class Program
    {
        private static string CodeFile => "code.adn";

        private static string ProgramFile => "program.gex";

        private static string DebugDataFile => "program.df";

        public static void Main(string[] args)
        {   
            ReservedNames.SetupRegistry();
            ReservedSymbols.SetupRegistry();
            FunctionalProteinsRegistry.Setup();

            Console.Write("mode c/r: ");

            string mode = Console.ReadLine() ?? "";

            if (mode == "c") Compile();
            if (mode == "r") Run();
            if (mode == "t") if (Compile()) Run();

            /*if (args == null || args.Length <= 0)
            {
                Console.Write("work mode. run or compile: ");
                string mode = Console.ReadLine() ?? "";
                mode = mode.ToLower();

                Console.Write("file path: ");
                string filePath = Console.ReadLine() ?? "";

                if (mode == "compile")
                {
                    string extension = Path.GetExtension(filePath);
                }
            }*/
        }

        private static bool Compile()
        {
            string code = File.ReadAllText(CodeFile);

            List<Error> errors;
            DebugData debugData = null;

            Cell cell = AdenineCompiler.Compile(code, false, out debugData, out errors);

            if (errors.Count > 0)
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    Console.WriteLine(errors[i].ToString());
                }
                return false;
            }

            else
            {
                if (cell == null) throw new Exception();
                if (debugData == null) throw new Exception();

                Console.WriteLine("Compilation was successful!");

                byte[] debugBytes = DebugDataSerializer.Serialize(debugData);
                File.WriteAllBytes(DebugDataFile, debugBytes);

                byte[] programBytes = ProgramSerializer.Serialize(cell);
                File.WriteAllBytes(ProgramFile, programBytes);
            }

            return true;
        }

        private static void Run()
        {
            if (!File.Exists(ProgramFile)) return;

            Cell cell = ProgramSerializer.DeSerialize(File.ReadAllBytes(ProgramFile));

            Console.WriteLine("\r\nProgram was loaded");

            DebugData debugData = null;

            if (File.Exists(DebugDataFile) && cell != null) 
            {
                byte[] debugByte = File.ReadAllBytes(DebugDataFile);
                debugData = DebugDataSerializer.DeSerialize(cell, debugByte);

                Console.WriteLine("Debug file was loaded");
            }

            Console.WriteLine();

            VirtualMachine.Setup(cell, debugData);
            VirtualMachine.Start();

            Logging.Program.Save();

            Console.ReadLine();
        }
    }
}
