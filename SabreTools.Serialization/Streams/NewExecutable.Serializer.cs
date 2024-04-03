using System;
using System.IO;
using SabreTools.Models.NewExecutable;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class NewExecutable : IStreamSerializer<Executable>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Executable? obj)
        {
            var serializer = new NewExecutable();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Executable? obj) => throw new NotImplementedException();
    }
}