using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using static SabreTools.Models.PlayStation4.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class AppPkgHeader : BaseBinaryDeserializer<SabreTools.Models.PlayStation4.AppPkgHeader>
    {
        /// <inheritdoc/>
        public override SabreTools.Models.PlayStation4.AppPkgHeader? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Create a new app.pkg header to fill
                var appPkgHeader = new SabreTools.Models.PlayStation4.AppPkgHeader();

                appPkgHeader.Magic = data.ReadUInt32BigEndian();
                if (appPkgHeader.Magic != AppPkgMagic)
                    return null;

                appPkgHeader.Type = data.ReadUInt32BigEndian();
                appPkgHeader.PKGUnknown = data.ReadUInt32BigEndian();
                appPkgHeader.FileCount = data.ReadUInt32BigEndian();
                appPkgHeader.EntryCount = data.ReadUInt32BigEndian();
                appPkgHeader.SCEntryCount = data.ReadUInt16BigEndian();
                appPkgHeader.EntryCount2 = data.ReadUInt16BigEndian();
                appPkgHeader.TableOffset = data.ReadUInt32BigEndian();
                appPkgHeader.EntryDataSize = data.ReadUInt32BigEndian();
                appPkgHeader.BodyOffset = data.ReadUInt64BigEndian();
                appPkgHeader.BodySize = data.ReadUInt64BigEndian();
                appPkgHeader.ContentOffset = data.ReadUInt64BigEndian();
                appPkgHeader.ContentSize = data.ReadUInt64BigEndian();
                byte[] contentID = data.ReadBytes(0x24);
                appPkgHeader.ContentID = Encoding.ASCII.GetString(contentID).TrimEnd('\0');
                appPkgHeader.ContentZeroes = data.ReadBytes(0xC);
                appPkgHeader.DRMType = data.ReadUInt32BigEndian();
                appPkgHeader.ContentType = data.ReadUInt32BigEndian();
                appPkgHeader.ContentFlags = data.ReadUInt32BigEndian();
                appPkgHeader.PromoteSize = data.ReadUInt32BigEndian();
                appPkgHeader.VersionDate = data.ReadUInt32BigEndian();
                appPkgHeader.VersionHash = data.ReadUInt32BigEndian();
                appPkgHeader.Zeroes1 = data.ReadBytes(0x78);
                appPkgHeader.MainEntry1SHA256 = data.ReadBytes(0x20);
                appPkgHeader.MainEntry2SHA256 = data.ReadBytes(0x20);
                appPkgHeader.DigestTableSHA256 = data.ReadBytes(0x20);
                appPkgHeader.MainTableSHA256 = data.ReadBytes(0x20);
                appPkgHeader.Zeroes2 = data.ReadBytes(0x280);
                appPkgHeader.PFSUnknown = data.ReadUInt32BigEndian();
                appPkgHeader.PFSImageCount = data.ReadUInt32BigEndian();
                appPkgHeader.PFSImageFlags = data.ReadUInt64BigEndian();
                appPkgHeader.PFSImageOffset = data.ReadUInt64BigEndian();
                appPkgHeader.PFSImageSize = data.ReadUInt64BigEndian();
                appPkgHeader.MountImageOffset = data.ReadUInt64BigEndian();
                appPkgHeader.MountImageSize = data.ReadUInt64BigEndian();
                appPkgHeader.PKGSize = data.ReadUInt64BigEndian();
                appPkgHeader.PKGSignedSize = data.ReadUInt32BigEndian();
                appPkgHeader.PKGCacheSize = data.ReadUInt32BigEndian();
                appPkgHeader.PFSImageSHA256 = data.ReadBytes(0x20);
                appPkgHeader.PFSSignedSHA256 = data.ReadBytes(0x20);
                appPkgHeader.PFSSplitSize0 = data.ReadUInt64BigEndian();
                appPkgHeader.PFSSplitSize1 = data.ReadUInt64BigEndian();
                appPkgHeader.Zeroes3 = data.ReadBytes(0xB50);
                appPkgHeader.PKGSHA256 = data.ReadBytes(0x20);

                return appPkgHeader;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}
