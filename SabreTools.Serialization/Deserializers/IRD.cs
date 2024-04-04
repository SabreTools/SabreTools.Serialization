using System.IO;
using System.Text;
using SabreTools.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class IRD :
        BaseBinaryDeserializer<Models.IRD.File>,
        IByteDeserializer<Models.IRD.File>,
        IFileDeserializer<Models.IRD.File>,
        IStreamDeserializer<Models.IRD.File>
    {
        #region IByteDeserializer

        /// <inheritdoc/>
        public Models.IRD.File? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc/>
        public Models.IRD.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc/>
        public Models.IRD.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new media key block to fill
            var ird = new Models.IRD.File();

            ird.Magic = data.ReadBytes(4);
            if (ird.Magic == null)
                return null;

            string magic = Encoding.ASCII.GetString(ird.Magic);
            if (magic != "3IRD")
                return null;

            ird.Version = data.ReadByteValue();
            if (ird.Version < 6)
                return null;

            var titleId = data.ReadBytes(9);
            if (titleId == null)
                return null;

            ird.TitleID = Encoding.ASCII.GetString(titleId);

            ird.TitleLength = data.ReadByteValue();
            var title = data.ReadBytes(ird.TitleLength);
            if (title == null)
                return null;

            ird.Title = Encoding.ASCII.GetString(title);

            var systemVersion = data.ReadBytes(4);
            if (systemVersion == null)
                return null;

            ird.SystemVersion = Encoding.ASCII.GetString(systemVersion);

            var gameVersion = data.ReadBytes(5);
            if (gameVersion == null)
                return null;

            ird.GameVersion = Encoding.ASCII.GetString(gameVersion);

            var appVersion = data.ReadBytes(5);
            if (appVersion == null)
                return null;

            ird.AppVersion = Encoding.ASCII.GetString(appVersion);

            if (ird.Version == 7)
                ird.UID = data.ReadUInt32();

            ird.HeaderLength = data.ReadByteValue();
            ird.Header = data.ReadBytes((int)ird.HeaderLength);
            ird.FooterLength = data.ReadByteValue();
            ird.Footer = data.ReadBytes((int)ird.FooterLength);

            ird.RegionCount = data.ReadByteValue();
            ird.RegionHashes = new byte[ird.RegionCount][];
            for (int i = 0; i < ird.RegionCount; i++)
            {
                ird.RegionHashes[i] = data.ReadBytes(16) ?? [];
            }

            ird.FileCount = data.ReadByteValue();
            ird.FileKeys = new ulong[ird.FileCount];
            ird.FileHashes = new byte[ird.FileCount][];
            for (int i = 0; i < ird.FileCount; i++)
            {
                ird.FileKeys[i] = data.ReadUInt64();
                ird.FileHashes[i] = data.ReadBytes(16) ?? [];
            }

            ird.ExtraConfig = data.ReadUInt16();
            ird.Attachments = data.ReadUInt16();

            if (ird.Version >= 9)
                ird.PIC = data.ReadBytes(115);

            ird.Data1Key = data.ReadBytes(16);
            ird.Data2Key = data.ReadBytes(16);

            if (ird.Version < 9)
                ird.PIC = data.ReadBytes(115);

            if (ird.Version > 7)
                ird.UID = data.ReadUInt32();

            ird.CRC = data.ReadUInt32();

            return ird;
        }

        #endregion
    }
}