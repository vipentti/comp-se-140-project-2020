using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    public static class TypeExtensions
    {
        public static bool IsImplementationOf(this Type @this, Type baseType)
        {
            return @this.IsClass && !@this.IsAbstract && @this.IsSubclassOf(baseType);
        }

        public static bool ImplementsOrDerives(this Type @this, Type from)
        {
            if (from is null)
            {
                return false;
            }
            else if (!from.IsGenericType && !from.IsGenericTypeDefinition)
            {
                return from.IsAssignableFrom(@this);
            }
            else if (from.IsInterface)
            {
                foreach (Type @interface in @this.GetInterfaces())
                {
                    if (@interface.IsGenericInterfaceOfType(from))
                    {
                        return true;
                    }
                }
            }

            if (@this.IsGenericInterfaceOfType(from))
            {
                return true;
            }

            return @this.BaseType?.ImplementsOrDerives(from) ?? false;
        }

        public static bool IsGenericInterfaceOfType(this Type type, Type @interface)
        {
            if (type.IsGenericType && @interface.IsGenericType)
            {
                var inter = @interface.GetGenericTypeDefinition();
                var typer = type.GetGenericTypeDefinition();
                return inter.IsAssignableFrom(typer);
            }

            return type.IsGenericType && type.GetGenericTypeDefinition() == @interface;
        }

        public static IEnumerable<Type> GetTypesWithInterface(this Assembly assembly, Type @interface)
        {
            return assembly
                .GetTypes()
                .Where(type => type.ImplementsOrDerives(@interface));
        }
    }
}
