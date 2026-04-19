using System;
using System.IO;
using System.Text;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class IRD : BaseBinaryWriter<Data.Models.IRD.File>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Data.Models.IRD.File? obj)
        {
            #region Validation

            // If the data is invalid
            if (obj?.Magic is null)
                return null;

            // If the magic doesn't match
            string magic = Encoding.ASCII.GetString(obj.Magic);
            if (magic != "3IRD")
                return null;

            // If the version is less than the supported
            if (obj.Version < 6)
                return null;

            // If any static-length fields aren't the correct length
            if (obj.TitleID.Length != 9)
                return null;
            if (obj.Title.Length != obj.TitleLength)
                return null;
            if (obj.SystemVersion.Length != 4)
                return null;
            if (obj.GameVersion.Length != 5)
                return null;
            if (obj.AppVersion.Length != 5)
                return null;
            if (obj.Header.Length != obj.HeaderLength)
                return null;
            if (obj.Footer.Length != obj.FooterLength)
                return null;
            if (obj.RegionHashes.Length != obj.RegionCount || !Array.TrueForAll(obj.RegionHashes, h => h is null || h.Length != 16))
                return null;
            if (obj.FileKeys.Length != obj.FileCount)
                return null;
            if (obj.FileHashes.Length != obj.FileCount || !Array.TrueForAll(obj.FileHashes, h => h is null || h.Length != 16))
                return null;
            if (obj.PIC.Length != 115)
                return null;
            if (obj.Data1Key.Length != 16)
                return null;
            if (obj.Data2Key.Length != 16)
                return null;

            #endregion

            // Create the output stream
            var stream = new MemoryStream();

            stream.Write(obj.Magic);
            stream.Write(obj.Version);

            byte[] titleId = Encoding.ASCII.GetBytes(obj.TitleID);
            stream.Write(titleId);

            stream.Write(obj.TitleLength);
            byte[] title = Encoding.ASCII.GetBytes(obj.Title);
            stream.Write(title);

            byte[] systemVersion = Encoding.ASCII.GetBytes(obj.SystemVersion);
            stream.Write(systemVersion);

            byte[] gameVersion = Encoding.ASCII.GetBytes(obj.GameVersion);
            stream.Write(gameVersion);

            byte[] appVersion = Encoding.ASCII.GetBytes(obj.AppVersion);
            stream.Write(appVersion);

            if (obj.Version == 7)
                stream.WriteLittleEndian(obj.UID);

            stream.WriteLittleEndian(obj.HeaderLength);
            stream.Write(obj.Header);

            stream.WriteLittleEndian(obj.FooterLength);
            stream.Write(obj.Footer);

            stream.Write(obj.RegionCount);
            for (int i = 0; i < obj.RegionCount; i++)
            {
                stream.Write(obj.RegionHashes[i]);
            }

            stream.WriteLittleEndian(obj.FileCount);
            for (int i = 0; i < obj.FileCount; i++)
            {
                stream.WriteLittleEndian(obj.FileKeys[i]);
                stream.Write(obj.FileHashes[i]);
            }

            stream.WriteLittleEndian(obj.ExtraConfig);
            stream.WriteLittleEndian(obj.Attachments);

            if (obj.Version >= 9)
                stream.Write(obj.PIC);

            stream.Write(obj.Data1Key);
            stream.Write(obj.Data2Key);

            if (obj.Version < 9)
                stream.Write(obj.PIC);

            if (obj.Version > 7)
                stream.WriteLittleEndian(obj.UID);

            stream.WriteLittleEndian(obj.CRC);

            return stream;
        }
    }
}
