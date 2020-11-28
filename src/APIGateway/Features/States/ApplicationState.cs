using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace APIGateway.Features.States
{
    public record Enumeration(int Id, string Name)
    {
        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static T FromName<T>(string name) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

    }

    public record ApplicationState : Enumeration
    {
        public static readonly ApplicationState Init = new ApplicationState(0, "INIT");
        public static readonly ApplicationState Paused = new ApplicationState(1, "PAUSED");
        public static readonly ApplicationState Running = new ApplicationState(1, "RUNNING");
        public static readonly ApplicationState Shutdown = new ApplicationState(1, "SHUTDOWN");

        public ApplicationState(int Id, string Name) : base(Id, Name)
        {
        }

        public override string ToString() => Name;

        public static ApplicationState? FromName(string name)
        {
            try {
                return FromName<ApplicationState>(name);
            } catch {
                return null;
            }
        }


    }
}
