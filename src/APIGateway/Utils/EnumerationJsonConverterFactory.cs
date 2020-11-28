using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common;

namespace APIGateway.Utils
{
    /// <summary>
    /// Converter factory for types inheriting from <see cref="Enumeration"/>
    /// </summary>
    public class EnumerationJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsClass && !typeToConvert.IsAbstract && typeToConvert.IsSubclassOf(typeof(Enumeration));
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Activator.CreateInstance(
                typeof(EnumerationJsonConverterInner<>).MakeGenericType(
                    new Type[] { typeToConvert }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null) as JsonConverter;
        }

        private class EnumerationJsonConverterInner<TEnumeration> : JsonConverter<TEnumeration>
            where TEnumeration : Enumeration
        {
            public EnumerationJsonConverterInner(JsonSerializerOptions _)
            {
            }

            public override TEnumeration Read(ref Utf8JsonReader reader, Type typeToConvert,
                                              JsonSerializerOptions options)
            {
                return Enumeration.FromName<TEnumeration>(JsonSerializer.Deserialize<string>(ref reader, options) ?? "");
            }

            public override void Write(Utf8JsonWriter writer, TEnumeration value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value.Name, options);
            }
        }
    }
}
