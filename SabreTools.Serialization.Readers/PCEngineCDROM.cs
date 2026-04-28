using System.IO;
using System.Text;
using SabreTools.Data.Models.PCEngineCDROM;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class PCEngineCDROM : BaseBinaryReader<Header>
    {
        /// <inheritdoc/>
        public override Header? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Simple check for a valid stream length
            if (data.Length - data.Position < 2 * Constants.SectorSize)
                return null;

            try
            {
                // Verify expected signature
                var magic = data.PeekBytes(16);
                if (!magic.EqualsExactly(Constants.MagicBytes))
                    return null;

                // Deserialize the Header
                var header = new Header();
                header.BootSector = ParseBootSector(data);
                header.IPL = ParseIPL(data);

                return header;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a BootSector
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled BootSector on success, null on error</returns>
        public static BootSector ParseBootSector(Stream data)
        {
            var bootSector = new BootSector();

            bootSector.CopyrightString = data.ReadBytes(806);
            bootSector.BootROM = data.ReadBytes(432);
            bootSector.Padding = data.ReadBytes(810);

            return bootSector;
        }

        /// <summary>
        /// Parse a Stream into a IPL
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled IPL on success, null on error</returns>
        public static IPL ParseIPL(Stream data)
        {
            var ipl = new IPL();

            ipl.IPLBLK = data.ReadUInt24BigEndian();
            ipl.IPLBLN = data.ReadByteValue();
            ipl.IPLSTA = data.ReadUInt16LittleEndian();
            ipl.IPLJMP = data.ReadUInt16LittleEndian();
            ipl.IPLMPR = data.ReadBytes(5);
            ipl.OpenMode = (OpenMode)data.ReadByteValue();
            ipl.GRPBLK = data.ReadUInt24BigEndian();
            ipl.GRPBLN = data.ReadByteValue();
            ipl.GRPADR = data.ReadUInt16LittleEndian();
            ipl.ADPBLK = data.ReadUInt24BigEndian();
            ipl.ADPBLN = data.ReadByteValue();
            ipl.ADPRATE = data.ReadByteValue();
            ipl.Reserved = data.ReadBytes(7);
            ipl.SystemString = data.ReadBytes(24);
            ipl.CopyrightString = data.ReadBytes(50);
            ipl.ProgramName = data.ReadBytes(16);
            ipl.AdditionalString = data.ReadBytes(6);

            return ipl;
        }
    }
}
