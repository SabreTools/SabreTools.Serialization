using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Serializers
{
    /// <summary>
    /// Base class for all binary serializers
    /// </summary>
    /// <typeparam name="TModel">Type of the model to serialize</typeparam>
    /// <remarks>These methods assume there is a concrete implementation of the serializer for the model available</remarks>
    public abstract class BaseBinarySerializer<TModel> :
        IByteSerializer<TModel>,
        IFileSerializer<TModel>,
        IStreamSerializer<TModel>
    {
        #region IByteSerializer

        /// <inheritdoc/>
        public virtual byte[]? SerializeArray(TModel? obj)
        {
            using var stream = SerializeStream(obj);
            if (stream == null)
                return null;

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region IFileSerializer

        /// <inheritdoc/>
        public virtual bool Serialize(TModel? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc/>
        public abstract Stream? Serialize(TModel? obj);

        #endregion

        #region Static Implementations

        /// <inheritdoc cref="IByteSerializer.Deserialize(T?)"/>
        public static byte[]? SerializeBytes(TModel? obj)
        {
            var serializer = GetType<IByteSerializer<TModel>>();
            if (serializer == null)
                return default;

            return serializer.SerializeArray(obj);
        }

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(TModel? obj, string? path)
        {
            var serializer = GetType<IFileSerializer<TModel>>();
            if (serializer == null)
                return default;

            return serializer.Serialize(obj, path);
        }

        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(TModel? obj)
        {
            var serializer = GetType<IStreamSerializer<TModel>>();
            if (serializer == null)
                return default;

            return serializer.Serialize(obj);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get a constructed instance of a type, if possible
        /// </summary>
        /// <typeparam name="TSerializer">Serializer type to construct</typeparam>
        /// <returns>Serializer of the requested type, null on error</returns>
        private static TSerializer? GetType<TSerializer>()
        {
            // If the serializer type is invalid
            string? serializerName = typeof(TSerializer)?.Name;
            if (serializerName == null)
                return default;

            // If the serializer has no generic arguments
            var genericArgs = typeof(TSerializer).GetGenericArguments();
            if (genericArgs == null || genericArgs.Length == 0)
                return default;

            // Loop through all loaded assemblies
            Type modelType = genericArgs[0];
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // If the assembly is invalid
                if (assembly == null)
                    return default;

                // If not all types can be loaded, use the ones that could be
                List<Type> assemblyTypes = [];
                try
                {
                    assemblyTypes = [.. assembly.GetTypes()];
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    assemblyTypes = [.. rtle.Types];
                }

                // Loop through all types 
                foreach (Type type in assemblyTypes)
                {
                    // If the type isn't a class
                    if (!type.IsClass)
                        continue;

                    // If the type doesn't implement the interface
                    var interfaceType = type.GetInterface(serializerName);
                    if (interfaceType == null)
                        continue;

                    // If the interface doesn't use the correct type parameter
                    var genericTypes = interfaceType.GetGenericArguments();
                    if (genericTypes.Length != 1 || genericTypes[0] != modelType)
                        continue;

                    // Try to create a concrete instance of the type
                    var instance = (TSerializer?)Activator.CreateInstance(type);
                    if (instance != null)
                        return instance;
                }
            }

            return default;
        }

        #endregion
    }
}