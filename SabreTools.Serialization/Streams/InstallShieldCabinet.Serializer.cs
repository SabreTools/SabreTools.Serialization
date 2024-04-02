using System;
using System.IO;
using SabreTools.Models.InstallShieldCabinet;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class InstallShieldCabinet : IStreamSerializer<Cabinet>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Cabinet? obj) => throw new NotImplementedException();
    }
}