using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class IRD : IStreamSerializer<Models.IRD.IRD>
    {
        /// <inheritdoc/>
        public Stream? Serialize(Models.IRD.IRD? obj)
        {
            // If the data is invalid
            if (obj?.Magic == null)
                return null;

            // If the magic doesn't match
            string magic = Encoding.ASCII.GetString(obj.Magic);
            if (magic != "3IRD")
                return null;

            // If the version is less than the supported
            if (obj.Version < 6)
                return null;

            // If any static-length fields aren't the correct length
            if (obj.TitleID == null || obj.TitleID.Length != 9)
                return null;
            if (obj.Title == null || obj.Title.Length != obj.TitleLength)
                return null;
            if (obj.SystemVersion == null || obj.SystemVersion.Length != 4)
                return null;
            if (obj.GameVersion == null || obj.GameVersion.Length != 5)
                return null;
            if (obj.AppVersion == null || obj.AppVersion.Length != 5)
                return null;
            if (obj.Header == null || obj.Header.Length != obj.HeaderLength)
                return null;
            if (obj.Footer == null || obj.Footer.Length != obj.FooterLength)
                return null;
            if (obj.RegionHashes == null || obj.RegionHashes.Length != obj.RegionCount || obj.RegionHashes.Any(h => h == null || h.Length != 16))
                return null;
            if (obj.FileKeys == null || obj.FileKeys.Length != obj.FileCount)
                return null;
            if (obj.FileHashes == null || obj.FileHashes.Length != obj.FileCount || obj.FileHashes.Any(h => h == null || h.Length != 16))
                return null;
            if (obj.PIC == null || obj.PIC.Length != 115)
                return null;
            if (obj.Data1Key == null || obj.Data1Key.Length != 16)
                return null;
            if (obj.Data2Key == null || obj.Data2Key.Length != 16)
                return null;

            // Create the output stream
            var stream = new MemoryStream();

            stream.Write(obj.Magic, 0, obj.Magic.Length);
            stream.WriteByte(obj.Version);

            byte[] titleId = Encoding.ASCII.GetBytes(obj.TitleID);
            stream.Write(titleId, 0, titleId.Length);

            stream.WriteByte(obj.TitleLength);
            byte[] title = Encoding.ASCII.GetBytes(obj.Title);
            stream.Write(title, 0, title.Length);

            byte[] systemVersion = Encoding.ASCII.GetBytes(obj.SystemVersion);
            stream.Write(systemVersion, 0, systemVersion.Length);

            byte[] gameVersion = Encoding.ASCII.GetBytes(obj.GameVersion);
            stream.Write(gameVersion, 0, gameVersion.Length);

            byte[] appVersion = Encoding.ASCII.GetBytes(obj.AppVersion);
            stream.Write(appVersion, 0, appVersion.Length);

            if (obj.Version == 7)
            {
                byte[] uid = BitConverter.GetBytes(obj.UID);
                stream.Write(uid, 0, uid.Length);
            }

            byte[] headerLength = BitConverter.GetBytes(obj.HeaderLength);
            stream.Write(headerLength, 0, headerLength.Length);
            stream.Write(obj.Header, 0, obj.Header.Length);

            byte[] footerLength = BitConverter.GetBytes(obj.FooterLength);
            stream.Write(footerLength, 0, footerLength.Length);
            stream.Write(obj.Footer, 0, obj.Footer.Length);

            stream.WriteByte(obj.RegionCount);
            for (int i = 0; i < obj.RegionCount; i++)
            {
                stream.Write(obj.RegionHashes[i], 0, obj.RegionHashes[i].Length);
            }

            byte[] fileCount = BitConverter.GetBytes(obj.FileCount);
            stream.Write(fileCount, 0, fileCount.Length);
            for (int i = 0; i < obj.FileCount; i++)
            {
                byte[] fileKey = BitConverter.GetBytes(obj.FileKeys[i]);
                stream.Write(fileKey, 0, fileKey.Length);
                stream.Write(obj.FileHashes[i], 0, obj.FileHashes[i].Length);
            }

            byte[] extraConfig = BitConverter.GetBytes(obj.ExtraConfig);
            stream.Write(extraConfig, 0, extraConfig.Length);
            byte[] attachments = BitConverter.GetBytes(obj.Attachments);
            stream.Write(attachments, 0, attachments.Length);

            if (obj.Version >= 9)
                stream.Write(obj.PIC, 0, obj.PIC.Length);

            stream.Write(obj.Data1Key, 0, obj.Data1Key.Length);
            stream.Write(obj.Data2Key, 0, obj.Data2Key.Length);

            if (obj.Version < 9)
                stream.Write(obj.PIC, 0, obj.PIC.Length);

            if (obj.Version > 7)
            {
                byte[] uid = BitConverter.GetBytes(obj.UID);
                stream.Write(uid, 0, uid.Length);
            }

            byte[] crc = BitConverter.GetBytes(obj.CRC);
            stream.Write(crc, 0, crc.Length);

            return stream;
        }
    }
}