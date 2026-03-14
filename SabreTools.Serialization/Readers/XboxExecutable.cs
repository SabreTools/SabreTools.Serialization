using System.IO;
using SabreTools.Data.Models.XboxExecutable;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.XboxExecutable.Constants;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class XboxExecutable : BaseBinaryReader<Executable>
    {
        /// <inheritdoc/>
        public override Executable? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create a new executable to fill
                var xbe = new Executable();

                #region ParseHeader

                // Parse the file header
                var header = ParseHeader(data);
                if (!header.MagicNumber.EqualsExactly(MagicBytes))
                    return null;

                // Set the file header
                xbe.Header = header;

                #endregion

                #region Certificate

                // Get the certificate address
                long certificateOffset = initialOffset + (header.CertificateAddress - header.BaseAddress);
                if (certificateOffset >= initialOffset && certificateOffset < data.Length)
                {
                    // Seek to the certificate
                    data.SeekIfPossible(certificateOffset, SeekOrigin.Begin);

                    // Set the certificate
                    xbe.Certificate = ParseCertificate(data);
                }

                #endregion

                #region Section Headers

                // Get the section table address
                long sectionTableOffset = initialOffset + (header.SectionHeadersAddress - header.BaseAddress);
                if (sectionTableOffset >= initialOffset && sectionTableOffset < data.Length)
                {
                    // Seek to the section table
                    data.SeekIfPossible(sectionTableOffset, SeekOrigin.Begin);

                    // Set the section table
                    xbe.SectionHeaders = new SectionHeader[xbe.Header.NumberOfSections];
                    for (int i = 0; i < xbe.Header.NumberOfSections; i++)
                    {
                        xbe.SectionHeaders[i] = ParseSectionHeader(data);
                    }
                }

                #endregion

                #region TLS

                // Get the TLS address
                long tlsOffset = initialOffset + (header.TLSAddress - header.BaseAddress);
                if (tlsOffset >= initialOffset && tlsOffset < data.Length)
                {
                    // Seek to the TLS
                    data.SeekIfPossible(tlsOffset, SeekOrigin.Begin);

                    // Set the TLS
                    xbe.ThreadLocalStorage = ParseThreadLocalStorage(data);
                }

                #endregion

                #region Library Versions

                // Get the library versions table address
                long libraryVersionsOffset = initialOffset + (header.LibraryVersionsAddress - header.BaseAddress);
                if (libraryVersionsOffset >= initialOffset && libraryVersionsOffset < data.Length)
                {
                    // Seek to the library versions table
                    data.SeekIfPossible(libraryVersionsOffset, SeekOrigin.Begin);

                    // Set the library versions table
                    xbe.LibraryVersions = new LibraryVersion[xbe.Header.NumberOfLibraryVersions];
                    for (int i = 0; i < xbe.Header.NumberOfLibraryVersions; i++)
                    {
                        xbe.LibraryVersions[i] = ParseLibraryVersion(data);
                    }
                }

                #endregion

                #region Kernel Library Version

                // Get the kernel library version address
                long klvOffset = initialOffset + (header.KernelLibraryVersionAddress - header.BaseAddress);
                if (klvOffset >= initialOffset && klvOffset < data.Length)
                {
                    // Seek to the kernel library version
                    data.SeekIfPossible(klvOffset, SeekOrigin.Begin);

                    // Set the kernel library version
                    xbe.KernelLibraryVersion = ParseLibraryVersion(data);
                }

                #endregion

                #region XAPI Library Version

                // Get the XAPI library version address
                long xapiOffset = initialOffset + (header.XAPILibraryVersionAddress - header.BaseAddress);
                if (xapiOffset >= initialOffset && xapiOffset < data.Length)
                {
                    // Seek to the XAPI library version
                    data.SeekIfPossible(xapiOffset, SeekOrigin.Begin);

                    // Set the XAPI library version
                    xbe.XAPILibraryVersion = ParseLibraryVersion(data);
                }

                #endregion

                return xbe;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Certificate
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Certificate on success, null on error</returns>
        public static Certificate ParseCertificate(Stream data)
        {
            var obj = new Certificate();

            obj.SizeOfCertificate = data.ReadUInt32LittleEndian();
            obj.TimeDate = data.ReadUInt32LittleEndian();
            obj.TitleID = data.ReadBytes(4);
            obj.TitleName = data.ReadBytes(0x50);

            obj.AlternativeTitleIDs = new byte[16][];
            for (int i = 0; i < obj.AlternativeTitleIDs.Length; i++)
            {
                obj.AlternativeTitleIDs[i] = data.ReadBytes(4);
            }

            obj.AllowedMediaTypes = (AllowedMediaTypes)data.ReadUInt32LittleEndian();
            obj.GameRegion = (GameRegion)data.ReadUInt32LittleEndian();
            obj.GameRatings = data.ReadUInt32LittleEndian();
            obj.DiskNumber = data.ReadUInt32LittleEndian();
            obj.Version = data.ReadUInt32LittleEndian();
            obj.LANKey = data.ReadBytes(16);
            obj.SignatureKey = data.ReadBytes(16);

            obj.AlternateSignatureKeys = new byte[16][];
            for (int i = 0; i < obj.AlternateSignatureKeys.Length; i++)
            {
                obj.AlternateSignatureKeys[i] = data.ReadBytes(16);
            }

            obj.OriginalCertificateSize = data.ReadUInt32LittleEndian();
            obj.OnlineService = data.ReadUInt32LittleEndian();
            obj.SecurityFlags = data.ReadUInt32LittleEndian();
            obj.CodeEncKey = data.ReadBytes(16);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Header on success, null on error</returns>
        public static Header ParseHeader(Stream data)
        {
            var obj = new Header();

            obj.MagicNumber = data.ReadBytes(4);
            obj.DigitalSignature = data.ReadBytes(256);
            obj.BaseAddress = data.ReadUInt32LittleEndian();
            obj.SizeOfHeaders = data.ReadUInt32LittleEndian();
            obj.SizeOfImage = data.ReadUInt32LittleEndian();
            obj.SizeOfImageHeader = data.ReadUInt32LittleEndian();
            obj.TimeDate = data.ReadUInt32LittleEndian();
            obj.CertificateAddress = data.ReadUInt32LittleEndian();
            obj.NumberOfSections = data.ReadUInt32LittleEndian();
            obj.SectionHeadersAddress = data.ReadUInt32LittleEndian();
            obj.InitializationFlags = (InitializationFlags)data.ReadUInt32LittleEndian();
            obj.EntryPoint = data.ReadUInt32LittleEndian();
            obj.TLSAddress = data.ReadUInt32LittleEndian();
            obj.PEStackCommit = data.ReadUInt32LittleEndian();
            obj.PEHeapReserve = data.ReadUInt32LittleEndian();
            obj.PEHeapCommit = data.ReadUInt32LittleEndian();
            obj.PEBaseAddress = data.ReadUInt32LittleEndian();
            obj.PESizeOfImage = data.ReadUInt32LittleEndian();
            obj.PEChecksum = data.ReadUInt32LittleEndian();
            obj.PETimeDate = data.ReadUInt32LittleEndian();
            obj.DebugPathNameAddress = data.ReadUInt32LittleEndian();
            obj.DebugFileNameAddress = data.ReadUInt32LittleEndian();
            obj.DebugUnicodeFileNameAddress = data.ReadUInt32LittleEndian();
            obj.KernelImageThunkAddress = data.ReadUInt32LittleEndian();
            obj.NonKernelImportDirectoryAddress = data.ReadUInt32LittleEndian();
            obj.NumberOfLibraryVersions = data.ReadUInt32LittleEndian();
            obj.LibraryVersionsAddress = data.ReadUInt32LittleEndian();
            obj.KernelLibraryVersionAddress = data.ReadUInt32LittleEndian();
            obj.XAPILibraryVersionAddress = data.ReadUInt32LittleEndian();
            obj.LogoBitmapAddress = data.ReadUInt32LittleEndian();
            obj.LogoBitmapSize = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a LibraryVersion
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled LibraryVersion on success, null on error</returns>
        public static LibraryVersion ParseLibraryVersion(Stream data)
        {
            var obj = new LibraryVersion();

            obj.LibraryName = data.ReadBytes(8);
            obj.MajorVersion = data.ReadUInt16LittleEndian();
            obj.MinorVersion = data.ReadUInt16LittleEndian();
            obj.BuildVersion = data.ReadUInt16LittleEndian();
            obj.LibraryFlags = (LibraryFlags)data.ReadUInt16LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SectionHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SectionHeader on success, null on error</returns>
        public static SectionHeader ParseSectionHeader(Stream data)
        {
            var obj = new SectionHeader();

            obj.SectionFlags = (SectionFlags)data.ReadUInt32LittleEndian();
            obj.VirtualAddress = data.ReadUInt32LittleEndian();
            obj.VirtualSize = data.ReadUInt32LittleEndian();
            obj.RawAddress = data.ReadUInt32LittleEndian();
            obj.RawSize = data.ReadUInt32LittleEndian();
            obj.SectionNameAddress = data.ReadUInt32LittleEndian();
            obj.SectionNameReferenceCount = data.ReadUInt32LittleEndian();
            obj.HeadSharedPageReferenceCountAddress = data.ReadUInt32LittleEndian();
            obj.TailSharedPageReferenceCountAddress = data.ReadUInt32LittleEndian();
            obj.SectionDigest = data.ReadBytes(20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ThreadLocalStorage
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ThreadLocalStorage on success, null on error</returns>
        public static ThreadLocalStorage ParseThreadLocalStorage(Stream data)
        {
            var obj = new ThreadLocalStorage();

            obj.DataStartAddress = data.ReadUInt32LittleEndian();
            obj.DataEndAddress = data.ReadUInt32LittleEndian();
            obj.TLSIndexAddress = data.ReadUInt32LittleEndian();
            obj.TLSCallbackAddress = data.ReadUInt32LittleEndian();
            obj.SizeOfZeroFill = data.ReadUInt32LittleEndian();
            obj.Characteristics = data.ReadUInt32LittleEndian();

            return obj;
        }
    }
}
