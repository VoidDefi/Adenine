using Adenine.CodeObjects;
using Adenine.Compiler;
using Adenine.Compiler.Registry;
using Adenine.Compiler.Serializing;

namespace Adenine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReservedNames.SetupRegistry();
            ReservedSymbols.SetupRegistry();
            FunctionalProteinsRegistry.Setup();

            string code = File.ReadAllText("code.adn");

            List<Error> errors;
            DebugData debugData = null;

            Cell cell = AdenineCompiler.Compile(code, false, out debugData, out errors);

            if (errors.Count > 0)
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    Console.WriteLine(errors[i].ToString());
                }
            }

            else 
            {
                if (cell == null) throw new Exception();
                if (debugData == null) throw new Exception();

                Console.WriteLine("Compilation was successful!");

                byte[] debugBytes = DebugDataSerializer.Serialize(debugData);
                File.WriteAllBytes("program.df", debugBytes);
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
    }
}
