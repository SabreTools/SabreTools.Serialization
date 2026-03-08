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
                if (header is null)
                    return null;

                // Set the header
                cart.Header = header;

                #endregion

                // Read the trainer data, if necessary
                if (cart.Header.TrainerPresent)
                    cart.Trainer = data.ReadBytes(512);

                // Derive the PRG-ROM and CHR-ROM data sizes
                // TODO: Make model for PRG-ROM data blocks
                int prgRomSize = cart.Header.PrgRomSize * 16384;
                int chrRomSize = cart.Header.ChrRomSize * 8192;
                if (cart.Header is Header2 header2)
                {
                    ushort extendedSize = (ushort)((header2.PrgRomSizeMSB << 8)
                        | header.PrgRomSize);
                    prgRomSize = extendedSize * 16384;

                    extendedSize = (ushort)((header2.ChrRomSizeMSB << 8)
                        | header.ChrRomSize);
                    chrRomSize = extendedSize * 8192;
                }

                // Read the PRG-ROM and CHR-ROM data
                if (prgRomSize > 0)
                    cart.PrgRomData = data.ReadBytes(prgRomSize);
                if (chrRomSize > 0)
                    cart.ChrRomData = data.ReadBytes(chrRomSize);

                // Read the PlayChoice INST-ROM and PROM data, if necessary
                if (cart.Header.ConsoleType == ConsoleType.PlayChoice10)
                {
                    cart.PlayChoiceInstRom = data.ReadBytes(8192);
                    cart.PlayChoiceProm = data.ReadBytes(32);
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

            byte byte6 = data.ReadByteValue();
            NametableArrangement nametableArrangement = (NametableArrangement)(byte6 & 0x01);
            bool batteryBackedPrgRam = ((byte6 >> 1) & 0x01) != 0;
            bool trainerPresent = ((byte6 >> 2) & 0x01) != 0;
            bool alternativeNametableLayout = ((byte6 >> 3) & 0x01) != 0;
            byte mapperLowerNibble = (byte)(byte6 >> 4);

            byte byte7 = data.ReadByteValue();
            ConsoleType consoleType = (ConsoleType)(byte7 & 0x03);
            bool nes20 = ((byte7 >> 2) & 0x02) == 0x02;
            byte mapperUpperNibble = (byte)(byte7 >> 4);

            // NES 2.0
            if (nes20)
            {
                var obj = new Header2();

                obj.IdentificationString = identificationString;
                obj.PrgRomSize = prgRomSize;
                obj.ChrRomSize = chrRomSize;

                // Byte 6
                obj.NametableArrangement = nametableArrangement;
                obj.BatteryBackedPrgRam = batteryBackedPrgRam;
                obj.TrainerPresent = trainerPresent;
                obj.AlternativeNametableLayout = alternativeNametableLayout;
                obj.MapperLowerNibble = mapperLowerNibble;

                // Byte 7
                obj.ConsoleType = consoleType;
                obj.NES20 = nes20;
                obj.MapperUpperNibble = mapperUpperNibble;

                // Byte 8
                byte byte8 = data.ReadByteValue();
                obj.MapperMSB = (byte)(byte8 & 0x0F);
                obj.Submapper = (byte)((byte8 >> 4) & 0x0F);

                // Byte 9
                byte byte9 = data.ReadByteValue();
                obj.PrgRomSizeMSB = (byte)(byte9 & 0x0F);
                obj.ChrRomSizeMSB = (byte)((byte9 >> 4) & 0x0F);

                // Byte 10
                byte byte10 = data.ReadByteValue();
                obj.PrgRamShiftCount = (byte)(byte10 & 0x0F);
                obj.PrgNvramEepromShiftCount = (byte)((byte10 >> 4) & 0x0F);

                // Byte 11
                byte byte11 = data.ReadByteValue();
                obj.ChrRamShiftCount = (byte)(byte11 & 0x0F);
                obj.ChrNvramShiftCount = (byte)((byte11 >> 4) & 0x0F);

                obj.CPUPPUTiming = (CPUPPUTiming)data.ReadByteValue();

                // Byte 13
                byte byte13 = data.ReadByteValue();
                if (obj.ConsoleType == ConsoleType.VSUnisystem)
                {
                    obj.VsSystemType = (VsSystemType)(byte13 & 0x0F);
                    obj.VsHardwareType = (VsHardwareType)((byte13 >> 4) & 0x0F);
                }
                else if (obj.ConsoleType == ConsoleType.ExtendedConsoleType)
                {
                    obj.ExtendedConsoleType = (ExtendedConsoleType)(byte13 & 0x0F);
                    obj.Byte13ReservedBits47 = (byte)((byte13 >> 4) & 0x0F);
                }
                else
                {
                    obj.Reserved13 = byte13;
                }

                obj.MiscellaneousROMs = data.ReadByteValue();
                obj.DefaultExpansionDevice = (DefaultExpansionDevice)data.ReadByteValue();

                return obj;
            }

            // NES 1.0
            else
            {
                var obj = new Header1();

                obj.IdentificationString = identificationString;
                obj.PrgRomSize = prgRomSize;
                obj.ChrRomSize = chrRomSize;

                // Byte 6
                obj.NametableArrangement = nametableArrangement;
                obj.BatteryBackedPrgRam = batteryBackedPrgRam;
                obj.TrainerPresent = trainerPresent;
                obj.AlternativeNametableLayout = alternativeNametableLayout;
                obj.MapperLowerNibble = mapperLowerNibble;

                // Byte 7
                obj.ConsoleType = consoleType;
                obj.NES20 = nes20;
                obj.MapperUpperNibble = mapperUpperNibble;

                obj.PrgRamSize = data.ReadByteValue();
                obj.TVSystem = (TVSystem)data.ReadByteValue();

                // Byte 10
                byte byte10 = data.ReadByteValue();
                obj.TVSystemExtended = (TVSystemExtended)(byte10 & 0x03);
                obj.Byte10ReservedBits23 = (byte)((byte10 >> 2) & 0x03);
                obj.PrgRamPresent = ((byte10 >> 4) & 0x01) == 0x01;
                obj.HasBusConflicts = ((byte10 >> 5) & 0x01) == 0x01;
                obj.Byte10ReservedBits67 = (byte)((byte10 >> 6) & 0x03);

                obj.Padding = data.ReadBytes(5);

                return obj;
            }
        }
    }
}
