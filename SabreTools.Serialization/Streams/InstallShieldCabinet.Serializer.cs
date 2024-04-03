using System;
using System.IO;
using SabreTools.Models.InstallShieldCabinet;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class InstallShieldCabinet : IStreamSerializer<Cabinet>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Cabinet? obj)
        {
            var serializer = new InstallShieldCabinet();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc/>
        public Stream? SerializeImpl(Cabinet? obj) => throw new NotImplementedException();
    }
}