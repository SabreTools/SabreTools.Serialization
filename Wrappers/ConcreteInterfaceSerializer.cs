#if !NETFRAMEWORK
using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SabreTools.Serialization.Wrappers
{
    /// <summary>
    /// Serializer class for abstract classes
    /// </summary>
    /// <see href="https://stackoverflow.com/a/72775719"/>
    internal class ConcreteAbstractSerializer : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsAbstract;

        class ConcreteAbstractSerializerOfType<TAbstract> : JsonConverter<TAbstract>
        {
            static ConcreteAbstractSerializerOfType()
            {
                if (!typeof(TAbstract).IsAbstract && !typeof(TAbstract).IsInterface)
                    throw new NotImplementedException(string.Format("Concrete class {0} is not supported", typeof(TAbstract)));
            }

            public override TAbstract? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotImplementedException();

            public override void Write(Utf8JsonWriter writer, TAbstract value, JsonSerializerOptions options) =>
                JsonSerializer.Serialize<object>(writer, value!, options);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options) =>
            (JsonConverter)Activator.CreateInstance(
                typeof(ConcreteAbstractSerializerOfType<>).MakeGenericType(new Type[] { type }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: Array.Empty<object>(),
                culture: null).ThrowOnNull();
    }

    /// <summary>
    /// Serializer class for interfaces
    /// </summary>
    /// <see href="https://stackoverflow.com/a/72775719"/>
    internal class ConcreteInterfaceSerializer : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsInterface;

        class ConcreteInterfaceSerializerOfType<TInterface> : JsonConverter<TInterface>
        {
            static ConcreteInterfaceSerializerOfType()
            {
                if (!typeof(TInterface).IsAbstract && !typeof(TInterface).IsInterface)
                    throw new NotImplementedException(string.Format("Concrete class {0} is not supported", typeof(TInterface)));
            }

            public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                throw new NotImplementedException();

            public override void Write(System.Text.Json.Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options) =>
                JsonSerializer.Serialize<object>(writer, value!, options);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options) =>
            (JsonConverter)Activator.CreateInstance(
                typeof(ConcreteInterfaceSerializerOfType<>).MakeGenericType(new Type[] { type }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: Array.Empty<object>(),
                culture: null).ThrowOnNull();
    }

    /// <summary>
    /// Extensions for generic object types
    /// </summary>
    /// <see href="https://stackoverflow.com/a/72775719"/>
    internal static class ObjectExtensions
    {
        public static T ThrowOnNull<T>(this T? value) where T : class => value ?? throw new ArgumentNullException();
    }
}
#endif