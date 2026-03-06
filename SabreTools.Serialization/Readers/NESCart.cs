using System.IO;
using SabreTools.Data.Models.NES;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.NES.Constants;

namespace SabreTools.Serialization.Readers
{
    public class NESCart : BaseBinaryReader<Cart>
    {
        /// <inheritdoc/>
        public override Cart? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new NES cart image to fill
                var cart = new Cart();

                #region Header

                // Try to parse the header
                var header = ParseHeader(data);
                if (header == null)
                    return null;

                // Set the header
                cart.Header = header;

                #endregion

                // Read the trainer data, if necessary
                if (cart.Header.TrainerPresent)
                    cart.Trainer = data.ReadBytes(512);

                // Derive the PRG-ROM and CHR-ROM data sizes
                // TODO: Make model for PRG-ROM data blocks
                int prgRomSize = cart.Header.PRGROMSize * 16384;
                int chrRomSize = cart.Header.CHRROMSize * 8192;
                if (cart.Header is Header2 header2)
                {
                    byte msb = (byte)(header2.PRGCHRMSB & 0x0F);
                    ushort extendedSize = (ushort)((msb << 8) | header.PRGROMSize);
                    prgRomSize = extendedSize * 16384;

                    msb = (byte)(header2.PRGCHRMSB >> 4);
                    extendedSize = (ushort)((msb << 8) | header.CHRROMSize);
                    chrRomSize = extendedSize * 8192;
                }

                // Read the PRG-ROM and CHR-ROM data
                if (prgRomSize > 0)
                    cart.PRGROMData = data.ReadBytes(prgRomSize);
                if (chrRomSize > 0)
                    cart.CHRROMData = data.ReadBytes(chrRomSize);

                // Read the PlayChoice INST-ROM and PROM data, if necessary
                if (cart.Header.ConsoleType == ConsoleType.PlayChoice10)
                {
                    cart.PlayChoiceINSTROM = data.ReadBytes(8192);
                    cart.PlayChoicePROM = data.ReadBytes(32);
                }

                // Read the cart title, if it exists
                // TODO: Read cart title

                return cart;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header? ParseHeader(Stream data)
        {
            // Cache data until NES 2.0 flag determined
            byte[] identificationString = data.ReadBytes(4);
            if (!identificationString.EqualsExactly(SignatureBytes))
                return null;

            byte prgRomSize = data.ReadByteValue();
            byte chrRomSize = data.ReadByteValue();

            byte flag6 = data.ReadByteValue();
            NametableArrangement nametableArrangement = (NametableArrangement)(flag6 & 0x01);
            bool batteryBackedPrgRam = ((flag6 >> 1) & 0x01) != 0;
            bool trainerPresent = ((flag6 >> 2) & 0x01) != 0;
            bool alternativeNametableLayout = ((flag6 >> 3) & 0x01) != 0;
            byte mapperLowerNibble = (byte)(flag6 >> 4);

            byte flag7 = data.ReadByteValue();
            ConsoleType consoleType = (ConsoleType)(flag7 & 0x03);
            bool nes20 = ((flag7 >> 2) & 0x02) == 0x02;
            byte mapperUpperNibble = (byte)(flag7 >> 4);

            // NES 2.0
            if (nes20)
            {
                var obj = new Header2();

                obj.IdentificationString = identificationString;
                obj.PRGROMSize = prgRomSize;
                obj.CHRROMSize = chrRomSize;

                // Flag 6
                obj.NametableArrangement = nametableArrangement;
                obj.BatteryBackedPRGRAM = batteryBackedPrgRam;
                obj.TrainerPresent = trainerPresent;
                obj.AlternativeNametableLayout = alternativeNametableLayout;
                obj.MapperLowerNibble = mapperLowerNibble;

                // Flag 7
                obj.ConsoleType = consoleType;
                obj.NES20 = nes20;
                obj.MapperUpperNibble = mapperUpperNibble;

                obj.MapperMSBSubmapper = data.ReadByteValue();
                obj.PRGCHRMSB = data.ReadByteValue();
                obj.PRGRAMEEPROMSize = data.ReadByteValue();
                obj.CHRRAMSize = data.ReadByteValue();
                obj.CPUPPUTiming = (CPUPPUTiming)data.ReadByteValue();
                obj.ExtendedSystemType = data.ReadByteValue();
                obj.MiscellaneousROMs = data.ReadByteValue();
                obj.DefaultExpansionDevice = (DefaultExpansionDevice)data.ReadByteValue();

                return obj;
            }

            // NES 1.0
            else
            {
                var obj = new Header1();

                obj.IdentificationString = identificationString;
                obj.PRGROMSize = prgRomSize;
                obj.CHRROMSize = chrRomSize;

                // Flag 6
                obj.NametableArrangement = nametableArrangement;
                obj.BatteryBackedPRGRAM = batteryBackedPrgRam;
                obj.TrainerPresent = trainerPresent;
                obj.AlternativeNametableLayout = alternativeNametableLayout;
                obj.MapperLowerNibble = mapperLowerNibble;

                // Flag 7
                obj.ConsoleType = consoleType;
                obj.NES20 = nes20;
                obj.MapperUpperNibble = mapperUpperNibble;

                obj.PRGRAMSize = data.ReadByteValue();
                obj.TVSystem = (TVSystem)data.ReadByteValue();
                obj.Flag10 = (Flag10)data.ReadByteValue();
                obj.Padding = data.ReadBytes(5);

                return obj;
            }
        }
    }
}
