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
        public CartHeader? Header => Model.Header;

        /// <inheritdoc cref="CartHeader.AlternativeNametableLayout"/>
        public bool AlternativeNametableLayout => Header?.AlternativeNametableLayout ?? false;

        /// <inheritdoc cref="CartHeader.BatteryBackedPrgRam"/>
        public bool BatteryBackedPrgRam => Header?.BatteryBackedPrgRam ?? false;

        /// <inheritdoc cref="Cart.ChrRomData"/>
        public byte[] ChrRomData => Model.ChrRomData;

        /// <summary>
        /// CHR-ROM size in bytes
        /// </summary>
        /// <remarks>Extended by NES 2.0</remarks>
        public int ChrRomSize
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                int chrRomSize = Header.ChrRomSize * 8192;
                if (Header is CartHeader2 header2)
                    chrRomSize = (header2.ChrRomSizeMSB << 8) | chrRomSize;

                return chrRomSize;
            }
        }

        /// <inheritdoc cref="CartHeader.ConsoleType"/>
        public ConsoleType ConsoleType => Header?.ConsoleType ?? ConsoleType.StandardSystem;

        /// <summary>
        /// Mapper ID
        /// </summary>
        /// <remarks>Extended by NES 2.0</remarks>
        public int Mapper
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                int mapperNumber = (Header.MapperUpperNibble << 4) | Header.MapperLowerNibble;
                if (Header is CartHeader2 header2)
                    mapperNumber = (header2.MapperMSB << 8) | mapperNumber;

                return mapperNumber;
            }
        }

        /// <inheritdoc cref="CartHeader.NametableArrangement"/>
        public NametableArrangement NametableArrangement
            => Header?.NametableArrangement ?? NametableArrangement.Vertical;

        /// <inheritdoc cref="CartHeader.NES20"/>
        public bool NES20 => Header?.NES20 ?? false;

        /// <summary>
        /// PRG-RAM size in bytes
        /// </summary>
        /// <remarks>Extended by NES 2.0</remarks>
        public int PrgRamSize
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                if (Header is CartHeader1 header1)
                    return header1.PrgRamSize > 0 ? header1.PrgRamSize * 8192 : 8192;
                else if (Header is CartHeader2 header2)
                    return header2.PrgRamShiftCount > 0 ? 64 << header2.PrgRamShiftCount : 0;
                else
                    return 0;
            }
        }

        /// <summary>
        /// PRG-ROM size in bytes
        /// </summary>
        /// <remarks>Extended by NES 2.0</remarks>
        public int PrgRomSize
        {
            get
            {
                // Missing header
                if (Header is null)
                    return 0;

                int prgRomSize = Header.PrgRomSize * 16384;
                if (Header is CartHeader2 header2)
                    prgRomSize = (header2.PrgRomSizeMSB << 8) | prgRomSize;

                return prgRomSize;
            }
        }

        /// <inheritdoc cref="Cart.PrgRomData"/>
        public byte[] PrgRomData => Model.PrgRomData;

        /// <inheritdoc cref="Cart.PlayChoiceInstRom"/>
        public byte[] PlayChoiceInstRom => Model.PlayChoiceInstRom;

        /// <inheritdoc cref="Cart.PlayChoiceProm"/>
        public byte[] PlayChoiceProm => Model.PlayChoiceProm;

        /// <inheritdoc cref="Cart.Title"/>
        public byte[] Title => Model.Title;

        /// <inheritdoc cref="Cart.Trainer"/>
        public byte[] Trainer => Model.Trainer;

        /// <inheritdoc cref="CartHeader.TrainerPresent"/>
        public bool TrainerPresent => Header?.TrainerPresent ?? false;

        #endregion

        #region NES 1.0

        /// <inheritdoc cref="CartHeader1.HasBusConflicts"/>
        /// <remarks>Defined only for NES 1.0</remarks>
        public bool HasBusConflicts
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader1 header1)
                    return false;

                return header1.HasBusConflicts;
            }
        }

        /// <inheritdoc cref="CartHeader1.PrgRamPresent"/>
        /// <remarks>Defined only for NES 1.0</remarks>
        public bool PrgRamPresent
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader1 header1)
                    return false;

                return header1.PrgRamPresent;
            }
        }

        /// <inheritdoc cref="CartHeader1.TVSystem"/>
        /// <remarks>Defined only for NES 1.0</remarks>
        public TVSystem TVSystem
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader1 header1)
                    return TVSystem.NTSC;

                return header1.TVSystem;
            }
        }

        /// <inheritdoc cref="CartHeader1.TVSystemExtended"/>
        /// <remarks>Defined only for NES 1.0</remarks>
        public TVSystemExtended TVSystemExtended
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader1 header1)
                    return TVSystemExtended.NTSC;

                return header1.TVSystemExtended;
            }
        }

        #endregion

        #region NES 2.0

        /// <summary>
        /// CHR-NVRAM size in bytes
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int ChrNvramSize
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader2 header2)
                    return 0;

                return header2.ChrNvramShiftCount > 0 ? 64 << header2.ChrNvramShiftCount : 0;
            }
        }

        /// <summary>
        /// CHR-RAM size in bytes
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int ChrRamSize
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader2 header2)
                    return 0;

                return header2.ChrRamShiftCount > 0 ? 64 << header2.ChrRamShiftCount : 0;
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
                if (Header is null || Header is not CartHeader2 header2)
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
                if (Header is null || Header is not CartHeader2 header2)
                    return DefaultExpansionDevice.Unspecified;

                return header2.DefaultExpansionDevice;
            }
        }

        /// <summary>
        /// Extended console type
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public ExtendedConsoleType ExtendedConsoleType
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader2 header2)
                    return ExtendedConsoleType.RegularSystem;

                // Invalid console type
                if (ConsoleType != ConsoleType.ExtendedConsoleType)
                    return ExtendedConsoleType.RegularSystem;

                return header2.ExtendedConsoleType;
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
                if (Header is null || Header is not CartHeader2 header2)
                    return 0;

                return header2.MiscellaneousROMs;
            }
        }

        /// <summary>
        /// PRG-NVRAM/EEPROM size in bytes
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int PrgNvramEepromSize
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader2 header2)
                    return 0;

                return header2.PrgNvramEepromShiftCount > 0 ? 64 << header2.PrgNvramEepromShiftCount : 0;
            }
        }

        /// <inheritdoc cref="CartHeader2.Submapper"/>
        /// <remarks>Defined only for NES 2.0</remarks>
        public int Submapper
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader2 header2)
                    return 0;

                return header2.Submapper;
            }
        }

        /// <summary>
        /// Vs. Hardware Type
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public VsHardwareType VsHardwareType
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader2 header2)
                    return VsHardwareType.VsUnisystem;

                // Invalid console type
                if (ConsoleType != ConsoleType.VSUnisystem)
                    return VsHardwareType.VsUnisystem;

                return header2.VsHardwareType;
            }
        }

        /// <summary>
        /// Vs. System Type
        /// </summary>
        /// <remarks>Defined only for NES 2.0</remarks>
        public VsSystemType VsSystemType
        {
            get
            {
                // Missing or invalid header
                if (Header is null || Header is not CartHeader2 header2)
                    return VsSystemType.AnyRP2C03RC2C03Variant;

                // Invalid console type
                if (ConsoleType != ConsoleType.VSUnisystem)
                    return VsSystemType.AnyRP2C03RC2C03Variant;

                return header2.VsSystemType;
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
