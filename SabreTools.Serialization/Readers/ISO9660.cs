using System;
using System.IO;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class ISO9660 : BaseBinaryReader<Volume>
    {
        /// <inheritdoc/>
        public override Volume? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new Volume to fill
                var volume = new Volume();

                //volume.SystemArea = ParseSystemArea(data);

                //volume.VolumeDescriptorSet = ParseVolumeDescriptors(data);

                //volume.RootDirectoryExtent = ParseDirectories(volume, volume.VolumeDescriptorSet);

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}
