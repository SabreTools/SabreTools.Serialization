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
#if NET20 || NET35
                if ((cart.Header.Flag6 & Flag6.TrainerPresent) != 0)
#else
                if (cart.Header.Flag6.HasFlag(Flag6.TrainerPresent))
#endif
                    cart.Trainer = data.ReadBytes(512);

                // Read the PRG-ROM data
                // TODO: Make model for PRG-ROM data blocks
                int prgRomSize = cart.Header.PRGROMSize * 16384;
                if (prgRomSize > 0)
                    cart.PRGROMData = data.ReadBytes(prgRomSize);

                // Read the CHR-ROM data, if necessary
                int chrRomSize = cart.Header.CHRROMSize * 8192;
                if (chrRomSize > 0)
                    cart.CHRROMData = data.ReadBytes(chrRomSize);

                // Read the PlayChoice INST-ROM and PROM data, if necessary
#if NET20 || NET35
                if ((cart.Header.Flag7 & Flag7.PlayChoice10) != 0
                    && (cart.Header.Flag7 & Flag7.VSUnisystem) == 0)
#else
                if (cart.Header.Flag7.HasFlag(Flag7.PlayChoice10)
                    && !cart.Header.Flag7.HasFlag(Flag7.VSUnisystem))
#endif
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
            if (identificationString.EqualsExactly(SignatureBytes))
                return null;

            byte prgRomSize = data.ReadByteValue();
            byte chrRomSize = data.ReadByteValue();
            Flag6 flag6 = (Flag6)data.ReadByteValue();
            Flag7 flag7 = (Flag7)data.ReadByteValue();

            // NES 2.0
#if NET20 || NET35
            if ((flag7 & Flag7.NES20) != 0)
#else
            if (flag7.HasFlag(Flag7.NES20))
#endif
            {
                var obj = new Header2();

                obj.IdentificationString = identificationString;
                obj.PRGROMSize = prgRomSize;
                obj.CHRROMSize = chrRomSize;
                obj.Flag6 = flag6;
                obj.Flag7 = flag7;
                obj.MapperMSBSubmapper = data.ReadByteValue();
                obj.PRGCHRMSB = data.ReadByteValue();
                obj.PRGRAMEEPROMSize = data.ReadByteValue();
                obj.CHRRAMSize = data.ReadByteValue();
                obj.CPUPPUTiming = (CPUPPUTiming)data.ReadByteValue();
                obj.ExtendedSystemType = (ExtendedSystemType)data.ReadByteValue();
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
                obj.Flag6 = flag6;
                obj.Flag7 = flag7;
                obj.PRGRAMSize = data.ReadByteValue();
                obj.TVSystem = (TVSystem)data.ReadByteValue();
                obj.Flag10 = (Flag10)data.ReadByteValue();
                obj.Padding = data.ReadBytes(5);

                return obj;
            }
        }
    }
}
