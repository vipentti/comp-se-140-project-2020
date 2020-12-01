using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Enumerations
{
    public abstract record Enumeration(int Id, string Name)
    {
        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            return GetAll(typeof(T)).Cast<T>();
        }

        public static IEnumerable<Enumeration> GetAll(Type type)
        {
            if (!type.ImplementsOrDerives(typeof(Enumeration)))
            {
                throw new InvalidOperationException($"{type} is not an implementaion of {typeof(Enumeration)}");
            }

            var fields = type.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields
                .Select(it => it.GetValue(null))
                .Cast<Enumeration>();
        }

        public static IEnumerable<Type> GetEnumerationTypes(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(typ => typ.IsImplementationOf(typeof(Enumeration)))
                .ToList();
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            return Parse<T, int>(value, "value", item => item.Id == value);
        }

        public static T FromName<T>(string name) where T : Enumeration
        {
            return Parse<T, string>(name, "name", item => item.Name == name);
        }

        public static object FromName(Type type, string name)
        {
            var matchingItem = GetAll(type).FirstOrDefault(it => it.Name == name);
            var description = "name";

            if (matchingItem == null)
            {
                throw new InvalidOperationException($"'{name}' is not a valid {description} in {type}");
            }

            return matchingItem;
        }

        public static bool TryFromName<T>(string name, out T? value) where T : Enumeration
        {
            try
            {
                value = FromName<T>(name);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
            }

            return matchingItem;
        }
    }
}
