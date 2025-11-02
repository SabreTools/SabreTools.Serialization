using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.CDROM;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class CDROMVolume : ISO9660
    {
        #region Constants

        private const int SectorSize = 2352;

        #endregion

        /// <inheritdoc/>
        public override Volume? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            // Simple check for a valid stream length
            if (SectorSize * (Constants.SystemAreaSectors + 2) > data.Length - data.Position)
                return null;

            try
            {
                // Create a new Volume to fill
                var volume = new Volume();

                // Read the System Area
                volume.SystemArea = ParseCDROMSystemArea(data);

                // Read the set of Volume Descriptors
                var vdSet = ParseCDROMVolumeDescriptorSet(data);
                if (vdSet == null || vdSet.Length == 0)
                    return null;
                
                volume.PathTableGroups = [];
                volume.DirectoryDescriptors = [];

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into the System Area
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled byte[] on success, null on error</returns>
        public static byte[]? ParseCDROMSystemArea(Stream data)
        {
            var systemArea = new byte[Constants.SystemAreaSectors * Constants.MinimumSectorSize];
            // Process in sectors
            for (int i = 0; i < Constants.SystemAreaSectors; i++)
            {
                // Ignore sector header
                var mode = SkipSectorHeader(data);

                // Read user data
                var userData = data.ReadBytes(Constants.MinimumSectorSize);

                // Copy user data into System Area
                Buffer.BlockCopy(userData, 0, systemArea, i * Constants.MinimumSectorSize, Constants.MinimumSectorSize);

                // Ignore sector trailer
                SkipSectorTrailer(data, mode);
            }

            return systemArea;
        }

        /// <summary>
        /// Parse a CD-ROM Stream into the System Area
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled byte[] on success, null on error</returns>
        public static VolumeDescriptor[]? ParseCDROMVolumeDescriptorSet(Stream data)
        {
            var obj = new List<VolumeDescriptor>();

            bool setTerminated = false;
            while (data.Position < data.Length)
            {
                // Ignore sector header
                var mode = SkipSectorHeader(data);

                var volumeDescriptor = ParseVolumeDescriptor(data, Constants.MinimumSectorSize);

                // Ignore sector trailer
                SkipSectorTrailer(data, mode);

                // If no valid volume descriptor could be read, return the current set
                if (volumeDescriptor == null)
                    return [.. obj];

                // If the set has already been terminated and the returned volume descriptor is not another terminator,
                // assume the read volume descriptor is not a valid volume descriptor and return the current set
                if (setTerminated && volumeDescriptor.Type != VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR)
                {
                    // Reset stream to before the just-read volume descriptor
                    data.SeekIfPossible(-SectorSize, SeekOrigin.Current);
                    return [.. obj];
                }

                // Add the valid read volume descriptor to the set
                obj.Add(volumeDescriptor);

                // If the set terminator was read, set the set terminated flag (further set terminators may be present)
                if (!setTerminated && volumeDescriptor.Type == VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR)
                    setTerminated = true;
            }

            return [.. obj];
        }

        /// <summary>
        /// Skip the header bytes of a CD-ROM sector
        /// </summary>
        private static SectorMode SkipSectorHeader(Stream data)
        {
            // Ignore sector header
            _ = data.ReadBytes(15);

            // Read sector mode
            byte mode = data.ReadByteValue();
            if (mode == 0)
                return SectorMode.MODE0;
            else if (mode == 1)
                return SectorMode.MODE1;
            else if (mode == 2)
            {
                // Ignore subheader
                var subheader = data.ReadBytes(8);
                if ((subheader[2] & 0x20) == 0x20)
                    return SectorMode.MODE2_FORM2;
                else
                    return SectorMode.MODE2_FORM1;
            }
            else
                return SectorMode.UNKNOWN;
        }

        /// <summary>
        /// Skip the trailer bytes of a CD-ROM sector
        /// </summary>
        private static void SkipSectorTrailer(Stream data, SectorMode mode)
        {
            if (mode == SectorMode.MODE1 || mode == SectorMode.MODE0 || mode == SectorMode.UNKNOWN)
            {
                _ = data.ReadBytes(288);
            }
            else if (mode == SectorMode.MODE2 || mode == SectorMode.MODE2_FORM1 || mode == SectorMode.MODE2_FORM2)
            {
                // TODO: Better deal with Form 2
                _ = data.ReadBytes(280);
            }
        }
    }
}
