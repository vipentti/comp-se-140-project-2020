using Serilog.Core;
using Serilog.Events;

namespace Common.Enumerations
{
    public class EnumerationDestructurePolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
        {
            result = null;

            if (value?.GetType().IsImplementationOf(typeof(Enumeration)) ?? false)
            {
                var name = ((Enumeration)value).Name;

                result = new ScalarValue(name);
            }

            return result != null;
        }
    }
}
