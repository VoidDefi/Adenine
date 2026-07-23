using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler.Registry
{
    internal abstract class BaseRegistry
    {
        protected void CreateRegistry()
        {
            List<string> registry = new();

            Type type = GetType();
            var properties = type.GetProperties();

            PropertyInfo registryInfo = type.GetProperty("Registry");

            foreach (var property in properties)
            {
                if (property.GetMethod != null && property.SetMethod == null)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        string value = property.GetValue(null) as string ?? "";

                        if (value.Length > 0)
                        {
                            registry.Add(value);
                        }
                    }
                }
            }

            if (registryInfo?.GetMethod != null && registryInfo?.SetMethod != null)
            {
                if (registryInfo.SetMethod.IsStatic && registryInfo.PropertyType == typeof(string[]))
                {
                    registryInfo.SetValue(null, registry.ToArray());
                }
            }
        }
    }
}
