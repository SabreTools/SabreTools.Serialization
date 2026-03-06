using System.IO;
using SabreTools.Data.Models.NES;

namespace SabreTools.Serialization.Wrappers
{
    public partial class NESCart : WrapperBase<Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo Entertainment System Cart Image";

        #endregion

        #region Extension Properties

        #region Common

        /// <inheritdoc cref="Cart.Header"/>
        public Header? Header => Model.Header;

        /// <inheritdoc cref="Header.AlternativeNametableLayout"/>
        public bool AlternativeNametableLayout => Header?.AlternativeNametableLayout ?? false;

        /// <inheritdoc cref="Header.BatteryBackedPRGRAM"/>
        public bool BatteryBackedPRGRAM => Header?.BatteryBackedPRGRAM ?? false;

        /// <inheritdoc cref="Cart.CHRROMData"/>
        public byte[] CHRROMData => Model.CHRROMData;

        /// <summary>
        /// CHR-ROM size in bytes
        /// </summary>
        /// <remarks>Extended by NES 2.0</remarks>
        public int CHRROMSize
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                int chrRomSize = Header.CHRROMSize * 8192;
                if (Header is Header2 header2)
                    chrRomSize = ((header2.PRGCHRMSB >> 4) << 8) | chrRomSize;

                return chrRomSize;
            }
        }

        /// <summary>
        /// Indicates if the game is meant for an extended console type
        /// </summary>
        /// <remarks>Possibly only valid for NES 2.0</remarks>
        public bool IsExtendedConsole
        {
            get
            {
                // Missing header
                if (Header is null)
                    return false;

#if NET20 || NET35
                return (Header.Flag7 & Flag7.ExtendedConsoleType) != 0;
#else
                return Header.Flag7.HasFlag(Flag7.ExtendedConsoleType);
#endif
            }
        }

        /// <summary>
        /// Indicates if the cart is using an NES 2.0 header
        /// </summary>
        public bool IsNES20
        {
            get
            {
                // Missing header
                if (Header is null)
                    return false;

#if NET20 || NET35
                return (Header.Flag7 & Flag7.NES20) != 0;
#else
                return Header.Flag7.HasFlag(Flag7.NES20);
#endif
            }
        }

        /// <summary>
        /// Indicates if the game is meant for PlayChoice-10
        /// </summary>
        public bool IsPlayChoice10
        {
            get
            {
                // Missing header
                if (Header is null)
                    return false;

#if NET20 || NET35
                return (Header.Flag7 & Flag7.PlayChoice10) != 0
                    && (Header.Flag7 & Flag7.VSUnisystem) == 0;
#else
                return Header.Flag7.HasFlag(Flag7.PlayChoice10)
                    && !Header.Flag7.HasFlag(Flag7.VSUnisystem);
#endif
            }
        }

        /// <summary>
        /// Indicates if the game is meant for a standard console
        /// </summary>
        public bool IsStandardConsole
        {
            get
            {
                // Missing header
                if (Header is null)
                    return false;

#if NET20 || NET35
                return (Header.Flag7 & Flag7.PlayChoice10) == 0
                    && (Header.Flag7 & Flag7.VSUnisystem) == 0;
#else
                return !Header.Flag7.HasFlag(Flag7.PlayChoice10)
                    && !Header.Flag7.HasFlag(Flag7.VSUnisystem);
#endif
            }
        }

        /// <summary>
        /// Indicates if the game is meant for Vs. Unisystem
        /// </summary>
        public bool IsVsUnisystem
        {
            get
            {
                // Missing header
                if (Header is null)
                    return false;

#if NET20 || NET35
                return (Header.Flag7 & Flag7.PlayChoice10) == 0
                    && (Header.Flag7 & Flag7.VSUnisystem) != 0;
#else
                return !Header.Flag7.HasFlag(Flag7.PlayChoice10)
                    && Header.Flag7.HasFlag(Flag7.VSUnisystem);
#endif
            }
        }

        /// <summary>
        /// Mapper number
        /// </summary>
        /// <remarks>Extended by NES 2.0</remarks>
        public int MapperNumber
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                int mapperNumber = (((byte)Header.Flag7 >> 4) << 4) | Header.MapperLowerNibble;
                if (Header is Header2 header2)
                    mapperNumber = ((header2.MapperMSBSubmapper & 0x0F) << 8) | mapperNumber;

                return mapperNumber;
            }
        }

        /// <inheritdoc cref="Header.NametableArrangement"/>
        public NametableArrangement NametableArrangement
            => Header?.NametableArrangement ?? NametableArrangement.Vertical;

        /// <summary>
        /// PRG-RAM size in bytes
        /// </summary>
        public int PRGRAMSize
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                if (Header is Header1 header1)
                {
                    return header1.PRGRAMSize > 0 ? header1.PRGRAMSize * 8192 : 8192;
                }
                else if (Header is Header2 header2)
                {
                    byte shift = (byte)(header2.PRGRAMEEPROMSize & 0x0F);
                    return shift > 0 ? 64 << shift : 0;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// PRG-ROM size in bytes
        /// </summary>
        /// <remarks>Extended by NES 2.0</remarks>
        public int PRGROMSize
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                int prgRomSize = Header.PRGROMSize * 16384;
                if (Header is Header2 header2)
                    prgRomSize = ((header2.PRGCHRMSB & 0x0F) << 8) | prgRomSize;

                return prgRomSize;
            }
        }

        /// <inheritdoc cref="Cart.PRGROMData"/>
        public byte[] PRGROMData => Model.PRGROMData;

        /// <inheritdoc cref="Cart.PlayChoiceINSTROM"/>
        public byte[] PlayChoiceINSTROM => Model.PlayChoiceINSTROM;

        /// <inheritdoc cref="Cart.PlayChoicePROM"/>
        public byte[] PlayChoicePROM => Model.PlayChoicePROM;

        /// <inheritdoc cref="Cart.Title"/>
        public byte[] Title => Model.Title;

        /// <inheritdoc cref="Cart.Trainer"/>
        public byte[] Trainer => Model.Trainer;

        /// <inheritdoc cref="Header.TrainerPresent"/>
        public bool TrainerPresent => Header?.TrainerPresent ?? false;

        #endregion

        #region NES 1.0

        /// <summary>
        /// Indicates if the board has bus conflicts
        /// </summary>
        /// <remarks>Defined only for NES 1.0</remarks>
        public bool HasBusConflicts
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header1 header1)
                    return false;

#if NET20 || NET35
                return (header1.Flag10 & Flag10.BoardHasBusConflicts) != 0;
#else
                return header1.Flag10.HasFlag(Flag10.BoardHasBusConflicts);
#endif
            }
        }

        /// <summary>
        /// Indicates if PRG RAM at $6000-$7FFF is present
        /// </summary>
        /// <remarks>Defined only for NES 1.0</remarks>
        public bool HasPRGRAM
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header1 header1)
                    return false;

#if NET20 || NET35
                return (header1.Flag10 & Flag10.PRGRAMNotPresent) == 0;
#else
                return !header1.Flag10.HasFlag(Flag10.PRGRAMNotPresent);
#endif
            }
        }

        #endregion

        #region NES 2.0

        /// <summary>
        /// CHR-NVRAM size in bytes
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int CHRNVRAMSize
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return 0;

                byte shift = (byte)(header2.CHRRAMSize >> 4);
                return shift > 0 ? 64 << shift : 0;
            }
        }

        /// <summary>
        /// CHR-RAM size in bytes
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int CHRRAMSize
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return 0;

                byte shift = (byte)(header2.CHRRAMSize & 0x0F);
                return shift > 0 ? 64 << shift : 0;
            }
        }

        /// <summary>
        /// CPU/PPU Timing
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public CPUPPUTiming CPUPPUTiming
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return CPUPPUTiming.RP2C02;

                return header2.CPUPPUTiming;
            }
        }

        /// <summary>
        /// Default expansion device
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public DefaultExpansionDevice DefaultExpansionDevice
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return DefaultExpansionDevice.Unspecified;

                return header2.DefaultExpansionDevice;
            }
        }

        /// <summary>
        /// Extended console type
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        /// <remarks>Only valid if <see cref="Flag7.ExtendedConsoleType"/> is set</remarks>
        public ExtendedConsoleType ExtendedConsoleType
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return ExtendedConsoleType.RegularSystem;

#if NET20 || NET35
                if ((Header.Flag7 & Flag7.ExtendedConsoleType) != 0)
#else
                if (Header.Flag7.HasFlag(Flag7.ExtendedConsoleType))
#endif
                    return (ExtendedConsoleType)(header2.ExtendedSystemType & 0x0F);

                // If flag is unset
                return ExtendedConsoleType.RegularSystem;
            }
        }

        /// <summary>
        /// Number of miscellaneous ROMs present
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int MiscellaneousROMs
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return 0;

                return header2.MiscellaneousROMs;
            }
        }

        /// <summary>
        /// PRG-NVRAM/EEPROM size in bytes
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int PRGRAMEEPROMSize
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return 0;

                byte shift = (byte)(header2.PRGRAMEEPROMSize >> 4);
                return shift > 0 ? 64 << shift : 0;
            }
        }

        /// <summary>
        /// Submapper number
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int SubmapperNumber
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return 0;

                return header2.MapperMSBSubmapper >> 4;
            }
        }

        /// <summary>
        /// Vs. Hardware Type
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        /// <remarks>Only valid if <see cref="Flag7.VSUnisystem"/> is set</remarks>
        public VsHardwareType VsHardwareType
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return VsHardwareType.VsUnisystem;

#if NET20 || NET35
                else if ((Header.Flag7 & Flag7.VSUnisystem) != 0)
#else
                else if (Header.Flag7.HasFlag(Flag7.VSUnisystem))
#endif
                    return (VsHardwareType)(header2.ExtendedSystemType >> 4);

                // If flag is unset
                return VsHardwareType.VsUnisystem;
            }
        }

        /// <summary>
        /// Vs. System Type
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        /// <remarks>Only valid if <see cref="Flag7.VSUnisystem"/> is set</remarks>
        public VsSystemType VsSystemType
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not Header2 header2)
                    return VsSystemType.AnyRP2C03RC2C03Variant;

#if NET20 || NET35
                else if ((Header.Flag7 & Flag7.VSUnisystem) != 0)
#else
                else if (Header.Flag7.HasFlag(Flag7.VSUnisystem))
#endif
                    return (VsSystemType)(header2.ExtendedSystemType & 0x0F);

                // If flag is unset
                return VsSystemType.AnyRP2C03RC2C03Variant;
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public NESCart(Cart model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public NESCart(Cart model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NESCart(Cart model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public NESCart(Cart model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public NESCart(Cart model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public NESCart(Cart model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create an NES cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NES cart image wrapper on success, null on failure</returns>
        public static NESCart? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data is null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create an NES cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An NES cart image wrapper on success, null on failure</returns>
        public static NESCart? Create(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Readers.NESCart().Deserialize(data);
                if (model is null)
                    return null;

                return new NESCart(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
