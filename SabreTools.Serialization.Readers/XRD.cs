using System.IO;
using SabreTools.Data.Models.XRD;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class XRD : BaseBinaryReader<Data.Models.XRD.File>
    {
        /// <inheritdoc/>
        public override Data.Models.XRD.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Simple check for a valid stream length
            if (2320 > data.Length - data.Position)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new Volume to fill
                var xrd = new Data.Models.XRD.File();

                xrd.Magic = data.ReadBytes(5);
                if (!xrd.Magic.EqualsExactly(Constants.MagicBytes))
                    return null;

                xrd.Version = data.ReadByteValue();
                if (xrd.Version != 0x01 || xrd.Version != 0x02)
                    return null;

                xrd.XGDType = data.ReadByteValue();
                xrd.XGDSubtype = data.ReadByteValue();
                xrd.Ringcode = data.ReadBytes(8);
                xrd.RedumpSize = data.ReadUInt64LittleEndian();
                xrd.RedumpCRC = data.ReadBytes(4);
                xrd.RedumpMD5 = data.ReadBytes(16);
                xrd.RedumpSHA1 = data.ReadBytes(20);
                xrd.RawXISOSize = data.ReadUInt64LittleEndian();
                xrd.RawXISOCRC = data.ReadBytes(4);
                xrd.RawXISOMD5 = data.ReadBytes(16);
                xrd.RawXISOSHA1 = data.ReadBytes(20);
                xrd.CookedXISOSize = data.ReadUInt64LittleEndian();
                xrd.CookedXISOCRC = data.ReadBytes(4);
                xrd.CookedXISOMD5 = data.ReadBytes(16);
                xrd.CookedXISOSHA1 = data.ReadBytes(20);
                xrd.VideoISOSize = data.ReadUInt64LittleEndian();
                xrd.VideoISOCRC = data.ReadBytes(4);
                xrd.VideoISOMD5 = data.ReadBytes(16);
                xrd.VideoISOSHA1 = data.ReadBytes(20);

                if (xrd.Version == 0x02)
                {
                    xrd.WipedVideoISOSize = data.ReadUInt64LittleEndian();
                    xrd.WipedVideoISOCRC = data.ReadBytes(4);
                    xrd.WipedVideoISOMD5 = data.ReadBytes(16);
                    xrd.WipedVideoISOSHA1 = data.ReadBytes(20);
                }

                xrd.FillerSize = data.ReadUInt64LittleEndian();
                xrd.FillerCRC = data.ReadBytes(4);
                xrd.FillerMD5 = data.ReadBytes(16);
                xrd.FillerSHA1 = data.ReadBytes(20);

                if (xrd.XGDType == 1)
                {
                    xrd.SecuritySectors = new uint[16];
                    for (int i = 0; i < 16; i++)
                    {
                        xrd.SecuritySectors[i] = data.ReadUInt32LittleEndian();
                    }

                    xrd.XboxCertificate = XboxExecutable.ParseCertificate(data);
                }
                else if (xrd.XGDType == 2 || xrd.XGDType == 3)
                {
                    xrd.SecuritySectors = new uint[2];
                    for (int i = 0; i < 2; i++)
                    {
                        xrd.SecuritySectors[i] = data.ReadUInt32LittleEndian();
                    }

                    xrd.Xbox360Certificate = XenonExecutable.ParseCertificate(data);
                }

                xrd.FileCount = data.ReadInt32LittleEndian();

                xrd.FileInfo = new FileEntry[xrd.FileCount];
                for (int i = 0; i < xrd.FileCount; i++)
                {
                    FileEntry file = new FileEntry();
                    file.Offset = data.ReadUInt32LittleEndian();
                    file.Size = data.ReadUInt64LittleEndian();
                    file.SHA1 = data.ReadBytes(20);
                    xrd.FileInfo[i] = file;
                }

                var vd = XDVDFS.ParseVolumeDescriptor(data);
                if (vd is null)
                    return null;

                xrd.VolumeDescriptor = vd;
                xrd.LayoutDescriptor = XDVDFS.ParseLayoutDescriptor(data);

                xrd.DirectoryCount = data.ReadInt32LittleEndian();

                xrd.DirectoryInfo = new DirectoryEntry[xrd.DirectoryCount];
                for (int i = 0; i < xrd.DirectoryCount; i++)
                {
                    DirectoryEntry directory = new DirectoryEntry();
                    directory.Offset = data.ReadUInt32LittleEndian();
                    directory.Size = data.ReadUInt32LittleEndian();
                    var dd = XDVDFS.ParseDirectoryDescriptor(data, initialOffset, directory.Offset, directory.Size);
                    if (dd is null)
                        return null;
                    directory.DirectoryDescriptor = dd;
                    xrd.DirectoryInfo[i] = directory;
                }

                if (xrd.Version == 0x02)
                {
                    xrd.VideoISOFileCount = data.ReadInt32LittleEndian();

                    xrd.VideoISOFileInfo = new FileEntry[xrd.VideoISOFileCount ?? 0];
                    for (int i = 0; i < xrd.VideoISOFileCount; i++)
                    {
                        FileEntry file = new FileEntry();
                        file.Offset = data.ReadUInt32LittleEndian();
                        file.Size = data.ReadUInt64LittleEndian();
                        file.SHA1 = data.ReadBytes(20);
                        xrd.VideoISOFileInfo[i] = file;
                    }
                }

                xrd.XRDSize = data.ReadUInt32LittleEndian();
                xrd.XRDSHA1 = data.ReadBytes(20);

                return xrd;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}
