using Adenine.CodeObjects;
using Adenine.Commands;
using Adenine.Compiler;
using Adenine.Compiler.Registry;
using Adenine.Compiler.Serializing;
using Adenine.Decompiler;
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
            CommandRegistry.Setup();
            FunctionalProteinsRegistry.Setup();

            Console.WriteLine("Adenine Lang was started. Enter 'help' to get all tool modes/commands.");

            while (true)
            {
                Console.Write("> ");

                string input = Console.ReadLine() ?? "";
                string name = input;

                int index = CommandRegistry.Commands.FindIndex(c => c.Name == name);

                string[] arguments = [];

                if (index >= 0)
                {
                    CommandRegistry.Commands[index].Execute(arguments);
                }

                else
                {
                    Console.WriteLine("Incorrect mode or command!");
                }
            }

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

        internal static bool Compile()
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

                Console.WriteLine("Compilation was successful!");

                if (debugData != null)
                {
                    byte[] debugBytes = DebugDataSerializer.Serialize(debugData);
                    File.WriteAllBytes(DebugDataFile, debugBytes);
                }

                byte[] programBytes = ProgramSerializer.Serialize(cell);
                File.WriteAllBytes(ProgramFile, programBytes);
            }

            return true;
        }

        internal static void Run()
        {
            if (!File.Exists(ProgramFile)) return;

            Cell cell = ProgramSerializer.DeSerialize(File.ReadAllBytes(ProgramFile));

            Console.WriteLine("Program was loaded");

            DebugData debugData = null;

            if (File.Exists(DebugDataFile) && cell != null)
            {
                byte[] debugByte = File.ReadAllBytes(DebugDataFile);
                debugData = DebugDataSerializer.DeSerialize(cell, debugByte);

                Console.WriteLine("Debug file was loaded");
            }

            Thread.Sleep(1);

            //go to new window
            Console.Write("\x1b[?1049h\x1b[H");

            VirtualMachine.Setup(cell, debugData);
            VirtualMachine.Start();

            //Logging.Program.Save();

            Console.ReadLine();

            Thread.Sleep(1);

            //return to start window
            Console.Write("\x1b[?1049l");
        }

        internal static void Decompile()
        {
            if (!File.Exists(ProgramFile)) return;

            Cell cell = ProgramSerializer.DeSerialize(File.ReadAllBytes(ProgramFile));
            List<CodeFile> files = AdenineDecompiler.Decompile(cell);

            string outDirectory = "decompiled";

            if (!Directory.Exists(outDirectory))
                Directory.CreateDirectory(outDirectory);

            DateTime dateTime = DateTime.Now;
            string curDecompileDir = outDirectory + "\\" + dateTime.ToString().Replace(':', '.');

            foreach (CodeFile file in files)
            {
                Directory.CreateDirectory(curDecompileDir);

                File.WriteAllText(curDecompileDir + "\\" + file.FileName, file.Code);
            }

            Console.WriteLine($"Program decompiled in '{curDecompileDir}'."); 
        }
    }
}
