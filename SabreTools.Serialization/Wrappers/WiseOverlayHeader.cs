using System.IO;
using SabreTools.Serialization.Models.WiseInstaller;

namespace SabreTools.Serialization.Wrappers
{
    public partial class WiseOverlayHeader : WrapperBase<OverlayHeader>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Wise Installer Overlay Header";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Returns the offset relative to the start of the header
        /// where the compressed data lives
        /// </summary>
        public long CompressedDataOffset
        {
            get
            {
                long offset = 0;

                offset += 1; // DllNameLen
                if (Model.DllNameLen > 0)
                {
                    offset += Model.DllNameLen;
                    offset += 4; // DllSize
                }

                offset += 4; // Flags
                offset += 12; // GraphicsData
                offset += 4; // WiseScriptExitEventOffset
                offset += 4; // WiseScriptCancelEventOffset
                offset += 4; // WiseScriptInflatedSize
                offset += 4; // WiseScriptDeflatedSize
                offset += 4; // WiseDllDeflatedSize
                offset += 4; // Ctl3d32DeflatedSize
                offset += 4; // SomeData4DeflatedSize
                offset += 4; // RegToolDeflatedSize
                offset += 4; // ProgressDllDeflatedSize
                offset += 4; // SomeData7DeflatedSize
                offset += 4; // SomeData8DeflatedSize
                offset += 4; // SomeData9DeflatedSize
                offset += 4; // SomeData10DeflatedSize
                offset += 4; // FinalFileDeflatedSize
                offset += 4; // FinalFileInflatedSize
                offset += 4; // EOF

                if (DibDeflatedSize == 0 && Model.Endianness == 0)
                    return offset;

                offset += 4; // DibDeflatedSize
                offset += 4; // DibInflatedSize

                if (Model.InstallScriptDeflatedSize != null)
                    offset += 4; // InstallScriptDeflatedSize

                if (Model.CharacterSet != null)
                    offset += 4; // CharacterSet

                offset += 2; // Endianness
                offset += 1; // InitTextLen
                offset += Model.InitTextLen;

                return offset;
            }
        }

        /// <summary>
        /// Installer data offset
        /// </summary>
        /// <remarks>
        /// This is the offset marking the point after all of the
        /// header-defined files. It is only set during extraction
        /// and is not used otherwise. It is automatically set if
        /// <see cref="ExtractHeaderDefinedFiles"/> is called.
        /// </remarks>
        public long InstallerDataOffset { get; private set; }

        /// <inheritdoc cref="OverlayHeader.Ctl3d32DeflatedSize"/>
        public uint Ctl3d32DeflatedSize => Model.Ctl3d32DeflatedSize;

        /// <inheritdoc cref="OverlayHeader.DibDeflatedSize"/>
        public uint DibDeflatedSize => Model.DibDeflatedSize;

        /// <inheritdoc cref="OverlayHeader.DibInflatedSize"/>
        public uint DibInflatedSize => Model.DibInflatedSize;

        /// <inheritdoc cref="OverlayHeader.FinalFileDeflatedSize"/>
        public uint FinalFileDeflatedSize => Model.FinalFileDeflatedSize;

        /// <inheritdoc cref="OverlayHeader.FinalFileInflatedSize"/>
        public uint FinalFileInflatedSize => Model.FinalFileInflatedSize;

        /// <inheritdoc cref="OverlayHeader.Flags"/>
        public OverlayHeaderFlags Flags => Model.Flags;

        /// <inheritdoc cref="OverlayHeader.InstallScriptDeflatedSize"/>
        public uint InstallScriptDeflatedSize => Model.InstallScriptDeflatedSize ?? 0;

        /// <summary>
        /// Indicates if data is packed in PKZIP containers
        /// </summary>
        public bool IsPKZIP
        {
            get
            {
#if NET20 || NET35
                return (Flags & OverlayHeaderFlags.WISE_FLAG_PK_ZIP) != 0;
#else
                return Flags.HasFlag(OverlayHeaderFlags.WISE_FLAG_PK_ZIP);
#endif
            }
        }

        /// <inheritdoc cref="OverlayHeader.ProgressDllDeflatedSize"/>
        public uint ProgressDllDeflatedSize => Model.ProgressDllDeflatedSize;

        /// <inheritdoc cref="OverlayHeader.SomeData4DeflatedSize"/>
        public uint SomeData4DeflatedSize => Model.SomeData4DeflatedSize;

        /// <inheritdoc cref="OverlayHeader.SomeData7DeflatedSize"/>
        public uint SomeData7DeflatedSize => Model.SomeData7DeflatedSize;

        /// <inheritdoc cref="OverlayHeader.SomeData8DeflatedSize"/>
        public uint SomeData8DeflatedSize => Model.SomeData8DeflatedSize;

        /// <inheritdoc cref="OverlayHeader.SomeData9DeflatedSize"/>
        public uint SomeData9DeflatedSize => Model.SomeData9DeflatedSize;

        /// <inheritdoc cref="OverlayHeader.SomeData10DeflatedSize"/>
        public uint SomeData10DeflatedSize => Model.SomeData10DeflatedSize;

        /// <inheritdoc cref="OverlayHeader.RegToolDeflatedSize"/>
        public uint RegToolDeflatedSize => Model.RegToolDeflatedSize;

        /// <inheritdoc cref="OverlayHeader.WiseDllDeflatedSize"/>
        public uint WiseDllDeflatedSize => Model.WiseDllDeflatedSize;

        /// <inheritdoc cref="OverlayHeader.WiseScriptDeflatedSize"/>
        public uint WiseScriptDeflatedSize => Model.WiseScriptDeflatedSize;

        /// <inheritdoc cref="OverlayHeader.WiseScriptInflatedSize"/>
        public uint WiseScriptInflatedSize => Model.WiseScriptInflatedSize;

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public WiseOverlayHeader(OverlayHeader model, byte[] data) : base(model, data) { }

        /// <inheritdoc/>
        public WiseOverlayHeader(OverlayHeader model, byte[] data, int offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WiseOverlayHeader(OverlayHeader model, byte[] data, int offset, int length) : base(model, data, offset, length) { }

        /// <inheritdoc/>
        public WiseOverlayHeader(OverlayHeader model, Stream data) : base(model, data) { }

        /// <inheritdoc/>
        public WiseOverlayHeader(OverlayHeader model, Stream data, long offset) : base(model, data, offset) { }

        /// <inheritdoc/>
        public WiseOverlayHeader(OverlayHeader model, Stream data, long offset, long length) : base(model, data, offset, length) { }

        #endregion

        #region Static Constructors

        /// <summary>
        /// Create a Wise installer overlay header from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the header</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Wise installer overlay header wrapper on success, null on failure</returns>
        public static WiseOverlayHeader? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Wise installer overlay header from a Stream
        /// </summary>
        /// <param name="data">Stream representing the header</param>
        /// <returns>A Wise installer overlay header wrapper on success, null on failure</returns>
        public static WiseOverlayHeader? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = new Deserializers.WiseOverlayHeader().Deserialize(data);
                if (model == null)
                    return null;

                return new WiseOverlayHeader(model, data, currentOffset);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
