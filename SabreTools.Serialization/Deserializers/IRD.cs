using System.IO;
using System.Text;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Deserializers
{
    public class IRD : BaseBinaryDeserializer<Models.IRD.File>
    {
        /// <inheritdoc/>
        public override Models.IRD.File? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Deserialize the IRD
                var ird = new Models.IRD.File();

                ird.Magic = data.ReadBytes(4);
                string magic = Encoding.ASCII.GetString(ird.Magic);
                if (magic != "3IRD")
                    return null;

                ird.Version = data.ReadByteValue();
                if (ird.Version < 6)
                    return null;

                byte[] titleId = data.ReadBytes(9);
                ird.TitleID = Encoding.ASCII.GetString(titleId);

                ird.TitleLength = data.ReadByteValue();
                byte[] title = data.ReadBytes(ird.TitleLength);
                ird.Title = Encoding.ASCII.GetString(title);

                byte[] systemVersion = data.ReadBytes(4);
                ird.SystemVersion = Encoding.ASCII.GetString(systemVersion);

                byte[] gameVersion = data.ReadBytes(5);
                ird.GameVersion = Encoding.ASCII.GetString(gameVersion);

                byte[] appVersion = data.ReadBytes(5);
                ird.AppVersion = Encoding.ASCII.GetString(appVersion);

                if (ird.Version == 7)
                    ird.UID = data.ReadUInt32LittleEndian();

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
                    ird.FileKeys[i] = data.ReadUInt64LittleEndian();
                    ird.FileHashes[i] = data.ReadBytes(16) ?? [];
                }

                ird.ExtraConfig = data.ReadUInt16LittleEndian();
                ird.Attachments = data.ReadUInt16LittleEndian();

                if (ird.Version >= 9)
                    ird.PIC = data.ReadBytes(115);

                ird.Data1Key = data.ReadBytes(16);
                ird.Data2Key = data.ReadBytes(16);

                if (ird.Version < 9)
                    ird.PIC = data.ReadBytes(115);

                if (ird.Version > 7)
                    ird.UID = data.ReadUInt32LittleEndian();

                ird.CRC = data.ReadUInt32LittleEndian();

                return ird;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}
