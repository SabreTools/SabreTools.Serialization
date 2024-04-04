using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    /// <summary>
    /// Base class for all binary deserializers
    /// </summary>
    /// <typeparam name="TModel">Type of the model to deserialize</typeparam>
    /// <remarks>These methods assume there is a concrete implementation of the deserialzier for the model available</remarks>
    public abstract class BaseBinaryDeserializer<TModel> :
        IByteDeserializer<TModel>,
        IFileDeserializer<TModel>
    {
        #region IByteDeserializer

        /// <inheritdoc/>
        public virtual TModel? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return default;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return default;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc/>
        public virtual TModel? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream);
        }

        #endregion

        #region Static Implementations

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static TModel? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = GetType<IByteDeserializer<TModel>>();
            if (deserializer == null)
                return default;

            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static TModel? DeserializeFile(string? path)
        {
            var deserializer = GetType<IFileDeserializer<TModel>>();
            if (deserializer == null)
                return default;
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc cref="IStreamDeserializer.Deserialize(Stream?)"/>
        public static TModel? DeserializeStream(Stream? data)
        {
            var deserializer = GetType<IStreamDeserializer<TModel>>();
            if (deserializer == null)
                return default;

            return deserializer.Deserialize(data);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get a constructed instance of a type, if possible
        /// </summary>
        /// <typeparam name="TDeserializer">Deserializer type to construct</typeparam>
        /// <returns>Deserializer of the requested type, null on error</returns>
        private static TDeserializer? GetType<TDeserializer>()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null)
                return default;

            // If not all types can be loaded, use the ones that could be
            List<Type> assemblyTypes = [];
            try
            {
                assemblyTypes = assembly.GetTypes().ToList<Type>();
            }
            catch (ReflectionTypeLoadException rtle)
            {
                assemblyTypes = rtle.Types.Where(t => t != null)!.ToList<Type>();
            }

            // Loop through all types 
            foreach (Type type in assemblyTypes)
            {
                // If the type isn't a class or doesn't implement the interface
                if (!type.IsClass || type.GetInterface(typeof(TDeserializer).Name) == null)
                    continue;

                // Try to create a concrete instance of the type
                var instance = (TDeserializer?)Activator.CreateInstance(type);
                if (instance != null)
                    return instance;
            }

            return default;
        }
    
        #endregion
    }
}