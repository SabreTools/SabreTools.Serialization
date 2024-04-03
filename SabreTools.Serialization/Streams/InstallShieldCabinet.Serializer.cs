using System;
using System.IO;
using SabreTools.Models.InstallShieldCabinet;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class InstallShieldCabinet : IStreamSerializer<Cabinet>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Cabinet? obj)
        {
            var serializer = new InstallShieldCabinet();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc/>
        public Stream? Serialize(Cabinet? obj) => throw new NotImplementedException();
    }
}