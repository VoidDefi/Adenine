using Adenine.Compiler;
using Adenine.Compiler.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReservedNames.SetupRegistry();

            List<Error> errors = AdenineCompiler.Compile(File.ReadAllText("code.adn"));

            if (errors.Count > 0)
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    Console.WriteLine(errors[i].ToString());
                }
            }

            else 
            {
                Console.WriteLine("Compilation was successful!");
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
