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
        public static string CurrentProject { get; internal set; } = "";

        internal static string CurrentProjectFile => "curproj.txt";

        private static string CodeFile => "code.adn";

        private static string ProgramFile => "program.gex";

        private static string DebugDataFile => "program.df";

        public static string[] Projects { get; private set; } = [];

        public static void Main(string[] args)
        {
            ReservedNames.SetupRegistry();
            ReservedSymbols.SetupRegistry();
            CommandRegistry.Setup();
            FunctionalProteinsRegistry.Setup();

            FindProjects();

            if (!Directory.Exists("projects"))
                Directory.CreateDirectory("projects");

            if (File.Exists(CurrentProjectFile))
            {
                CurrentProject = File.ReadAllText(CurrentProjectFile);

                if (!Projects.Contains(CurrentProject))
                {
                    CurrentProject = "";
                }
            }

            Console.WriteLine($"Adenine Lang was started. Enter '{Colors.Green}help{Colors.Reset}' to get all tool modes/commands.");

            if (CurrentProject != "")
                Console.WriteLine($"{Colors.Gray}Current project is {Colors.DarkCyan}{CurrentProject}{Colors.Reset}");

            while (true)
            {
                Console.Write("> ");

                string input = Console.ReadLine() ?? "";
                input += ' ';

                string name = "";
                List<string> arguments = new();

                string buffer = "";

                for (int i = 0; i < input.Length; i++)
                {
                    char symbol = input[i];

                    if (symbol != ' ')
                        buffer += input[i];

                    else
                    {
                        if (buffer != "")
                        {
                            if (name == "")
                                name = buffer;

                            else
                            {
                                arguments.Add(buffer);
                            }

                            buffer = "";
                        }
                    }
                }

                int index = CommandRegistry.Commands.FindIndex(c => c.Name == name);

                if (index >= 0)
                {
                    if (CommandRegistry.Commands[index].Arguments.Length != arguments.Count)
                    {
                        Console.WriteLine("Wrong arguments count!");
                    }

                    else CommandRegistry.Commands[index].Execute(arguments.ToArray());
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
            if (CurrentProject == "" || !Directory.Exists($"projects\\{CurrentProject}\\"))
            {
                Console.WriteLine($"Project not selected or does not exist. Please if necessary, create new project using the '{Colors.Green}newproj{Colors.Reset}' command, then select project which '{Colors.Green}projects{Colors.Reset}' -> '{Colors.Green}chooseproj{Colors.Reset}'");
                return false;
            }

            string code = File.ReadAllText($"projects\\{CurrentProject}\\" + CodeFile);

            List<Error> errors;
            DebugData debugData = null;

            Cell cell = AdenineCompiler.Compile(code, false, out debugData, out errors);

            if (errors.Count > 0)
            {
                Console.WriteLine($"Compilation of '{Colors.Cyan}{CurrentProject}{Colors.Reset}' ended which {errors.Count} error(s):{Colors.Red}");

                for (int i = 0; i < errors.Count; i++)
                {
                    Console.WriteLine(" " + errors[i].ToString());
                }

                Console.Write(Colors.Reset);

                return false;
            }

            else
            {
                if (cell == null) throw new Exception();

                Console.WriteLine($"Compilation of '{Colors.Cyan}{CurrentProject}{Colors.Reset}' was successful!");

                string binDirectory = $"projects\\{CurrentProject}\\bin";

                if (!Directory.Exists(binDirectory))
                    Directory.CreateDirectory(binDirectory);

                if (debugData != null)
                {
                    byte[] debugBytes = DebugDataSerializer.Serialize(debugData);
                    File.WriteAllBytes($"{binDirectory}\\{DebugDataFile}", debugBytes);
                }

                byte[] programBytes = ProgramSerializer.Serialize(cell);
                File.WriteAllBytes($"{binDirectory}\\{ProgramFile}", programBytes);
            }

            return true;
        }

        internal static void Run()
        {
            if (CurrentProject == "" || !Directory.Exists($"projects\\{CurrentProject}\\"))
            {
                Console.WriteLine($"Project not selected or does not exist. Please if necessary, create new project using the '{Colors.Green}newproj{Colors.Reset}' command, then select project which '{Colors.Green}projects{Colors.Reset}' -> '{Colors.Green}chooseproj{Colors.Reset}'");
                return;
            }

            string binDirectory = $"projects\\{CurrentProject}\\bin\\";

            if (!File.Exists($"{binDirectory}{ProgramFile}"))
            {
                Console.WriteLine($"Genetic executable file of '{Colors.Cyan}{CurrentProject}{Colors.Reset}' does not exist!");
                return;
            }

            Cell cell = ProgramSerializer.DeSerialize(File.ReadAllBytes($"{binDirectory}{ProgramFile}"));

            Console.WriteLine($"Program '{Colors.Cyan}{CurrentProject}{Colors.Reset}' was loaded");

            DebugData debugData = null;

            if (File.Exists($"{binDirectory}{DebugDataFile}") && cell != null)
            {
                byte[] debugByte = File.ReadAllBytes($"{binDirectory}{DebugDataFile}");
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
            if (CurrentProject == "" || !Directory.Exists($"projects\\{CurrentProject}\\"))
            {
                Console.WriteLine($"Project not selected or does not exist. Please if necessary, create new project using the '{Colors.Green}newproj{Colors.Reset}' command, then select project which '{Colors.Green}projects{Colors.Reset}' -> '{Colors.Green}chooseproj{Colors.Reset}'");
                return;
            }

            string binDirectory = $"projects\\{CurrentProject}\\bin\\";

            if (!File.Exists($"{binDirectory}{ProgramFile}"))
            {
                Console.WriteLine($"Genetic executable file of '{Colors.Cyan}{CurrentProject}{Colors.Reset}' does not exist!");
                return;
            }

            Cell cell = ProgramSerializer.DeSerialize(File.ReadAllBytes($"{binDirectory}{ProgramFile}"));
            List<CodeFile> files = AdenineDecompiler.Decompile(cell);

            string outDirectory = "decompiled";

            if (!Directory.Exists(outDirectory))
                Directory.CreateDirectory(outDirectory);

            DateTime dateTime = DateTime.Now;
            string curDecompileDir = outDirectory + "\\" + CurrentProject + "_" + dateTime.ToString().Replace(':', '.');

            foreach (CodeFile file in files)
            {
                Directory.CreateDirectory(curDecompileDir);

                File.WriteAllText(curDecompileDir + "\\" + file.FileName, file.Code);
            }

            Console.WriteLine($"Program '{Colors.Cyan}{CurrentProject}{Colors.Reset}' decompiled in '{curDecompileDir}'.");
        }

        internal static void FindProjects()
        {
            string[] projectsDirs = Directory.GetDirectories("projects");

            List<string> projects = new();

            foreach (string project in projectsDirs)
            {
                if (File.Exists($"{project}\\{CodeFile}"))
                {
                    projects.Add(Path.GetFileName(project) ?? $"{Colors.Red}Error{Colors.Reset}");
                }
            }

            Projects = projects.ToArray();
        }
    }
}
