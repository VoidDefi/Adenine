using Adenine.CodeObjects.FunctionalProteins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adenine.CodeObjects
{
    internal static class FunctionalProteinsRegistry
    {
        //public static List<FunctionalProtein> Proteins { get; private set; }

        public static Dictionary<int, FunctionalProtein> Proteins { get; private set; }

        public static void Setup()
        {
            Assembly assembly = typeof(FunctionalProtein).Assembly;
            Type type = typeof(FunctionalProtein);

            List<Type> types = assembly.GetTypes().Where(t => t.IsSubclassOf(type)).ToList();

            List<FunctionalProtein> proteins = new List<FunctionalProtein>();

            foreach (Type protein in types)
            {
                if (protein.IsAbstract) continue;

                proteins.Add(Activator.CreateInstance(protein) as FunctionalProtein);
            }

            foreach (FunctionalProtein action in proteins)
            {
                List<FunctionalProtein> matches = proteins.FindAll(p => p.Name == action.Name || p.Index == action.Index);

                if (matches.Count > 1)
                {
                    throw new Exception("Some parameters of functional proteins coincide");
                }
            }

            Proteins = new();

            for (int i = 0; i < proteins.Count; i++)
            {
                Proteins.Add(proteins[i].Index, proteins[i]);
            }
        }
    }
}
