using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public static class WrapperFactory
    {
        /// <summary>
        /// Create an instance of a wrapper based on file type
        /// </summary>
        public static IWrapper? CreateWrapper(WrapperType fileType, Stream? data)
        {
            switch (fileType)
            {
                case WrapperType.AACSMediaKeyBlock: return AACSMediaKeyBlock.Create(data);
                case WrapperType.BDPlusSVM: return BDPlusSVM.Create(data);
                case WrapperType.BFPK: return BFPK.Create(data);
                case WrapperType.BSP: return BSP.Create(data);
                case WrapperType.BZip2: return null; // TODO: Implement wrapper
                case WrapperType.CFB: return CFB.Create(data);
                case WrapperType.CHD: return CHD.Create(data);
                case WrapperType.CIA: return CIA.Create(data);
                case WrapperType.Executable: return CreateExecutableWrapper(data);
                case WrapperType.GCF: return GCF.Create(data);
                case WrapperType.GZIP: return null; // TODO: Implement wrapper
                case WrapperType.IniFile: return null; // TODO: Implement wrapper
                case WrapperType.InstallShieldArchiveV3: return null; // TODO: Implement wrapper
                case WrapperType.InstallShieldCAB: return InstallShieldCabinet.Create(data);
                case WrapperType.LDSCRYPT: return null; // TODO: Implement wrapper
                case WrapperType.MicrosoftCAB: return MicrosoftCabinet.Create(data);
                case WrapperType.MicrosoftLZ: return null; // TODO: Implement wrapper
                case WrapperType.MoPaQ: return MoPaQ.Create(data);
                case WrapperType.N3DS: return N3DS.Create(data);
                case WrapperType.NCF: return NCF.Create(data);
                case WrapperType.Nitro: return Nitro.Create(data);
                case WrapperType.PAK: return PAK.Create(data);
                case WrapperType.PFF: return PFF.Create(data);
                case WrapperType.PIC: return PIC.Create(data);
                case WrapperType.PKZIP: return PKZIP.Create(data);
                case WrapperType.PlayJAudioFile: return PlayJAudioFile.Create(data);
                case WrapperType.PlayJPlaylist: return PlayJPlaylist.Create(data);
                case WrapperType.Quantum: return Quantum.Create(data);
                case WrapperType.RAR: return null; // TODO: Implement wrapper
                case WrapperType.RealArcadeInstaller: return null; // TODO: Implement wrapper
                case WrapperType.RealArcadeMezzanine: return null; // TODO: Implement wrapper
                case WrapperType.SevenZip: return null; // TODO: Implement wrapper
                case WrapperType.SFFS: return null; // TODO: Implement wrapper
                case WrapperType.SGA: return SGA.Create(data);
                case WrapperType.TapeArchive: return null; // TODO: Implement wrapper
                case WrapperType.Textfile: return null; // TODO: Implement wrapper
                case WrapperType.VBSP: return VBSP.Create(data);
                case WrapperType.VPK: return VPK.Create(data);
                case WrapperType.WAD: return WAD.Create(data);
                case WrapperType.XZ: return null; // TODO: Implement wrapper
                case WrapperType.XZP: return XZP.Create(data);
                default: return null;
            }
        }

        /// <summary>
        /// Create an instance of a wrapper based on the executable type
        /// </summary>
        /// <param name="stream">Stream data to parse</param>
        /// <returns>IWrapper representing the executable, null on error</returns>
        public static IWrapper? CreateExecutableWrapper(Stream? stream)
        {
            // If we have no stream
            if (stream == null)
                return null;

            // Try to get an MS-DOS wrapper first
            var wrapper = MSDOS.Create(stream);
            if (wrapper == null || wrapper is not MSDOS msdos)
                return null;

            // Check for a valid new executable address
            if (msdos.Model.Header?.NewExeHeaderAddr == null || msdos.Model.Header.NewExeHeaderAddr >= stream.Length)
                return wrapper;

            // Try to read the executable info
            stream.Seek(msdos.Model.Header.NewExeHeaderAddr, SeekOrigin.Begin);
            var magic = stream.ReadBytes(4);

            // If we didn't get valid data at the offset
            if (magic == null)
            {
                return wrapper;
            }

            // New Executable
            else if (magic.StartsWith(Models.NewExecutable.Constants.SignatureBytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return NewExecutable.Create(stream);
            }

            // Linear Executable
            else if (magic.StartsWith(Models.LinearExecutable.Constants.LESignatureBytes)
                || magic.StartsWith(Models.LinearExecutable.Constants.LXSignatureBytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return LinearExecutable.Create(stream);
            }

            // Portable Executable
            else if (magic.StartsWith(Models.PortableExecutable.Constants.SignatureBytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return PortableExecutable.Create(stream);
            }

            // Everything else fails
            return null;
        }

        /// <summary>
        /// Get the supported file type for a magic string and an extension
        /// </summary>
        /// <remarks>Recommend sending in 16 bytes to check</remarks>
        public static WrapperType GetFileType(byte[]? magic, string? extension)
        {
            // If we have an invalid magic byte array and extension
            if (magic == null || magic.Length == 0 || extension == null)
                return WrapperType.UNKNOWN;

            // Normalize the extension
            extension = extension.TrimStart('.').Trim();

            // TODO: For all modelled types, use the constants instead of hardcoded values here
            #region AACSMediaKeyBlock

            // Block starting with verify media key record
            if (magic.StartsWith(new byte?[] { 0x81, 0x00, 0x00, 0x14 }))
                return WrapperType.AACSMediaKeyBlock;

            // Block starting with type and version record
            if (magic.StartsWith(new byte?[] { 0x10, 0x00, 0x00, 0x0C }))
                return WrapperType.AACSMediaKeyBlock;

            // Shares an extension with INF setup information so it can't be used accurately
            // Blu-ray
            // if (extension.Equals("inf", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.AACSMediaKeyBlock;

            // HD-DVD
            if (extension.Equals("aacs", StringComparison.OrdinalIgnoreCase))
                return WrapperType.AACSMediaKeyBlock;

            #endregion

            #region BDPlusSVM

            if (magic.StartsWith(new byte?[] { 0x42, 0x44, 0x53, 0x56, 0x4D, 0x5F, 0x43, 0x43 }))
                return WrapperType.BDPlusSVM;

            if (extension.Equals("svm", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BDPlusSVM;

            #endregion

            #region BFPK

            if (magic.StartsWith(new byte?[] { 0x42, 0x46, 0x50, 0x4b }))
                return WrapperType.BFPK;

            #endregion

            #region BSP

            // Shares a first 4 bytes with some .mc files
            // Shares an extension with VBSP
            if (magic.StartsWith(new byte?[] { 0x1d, 0x00, 0x00, 0x00 }) && extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BSP;

            // Shares a first 4 bytes with some .mc files
            // Shares an extension with VBSP
            if (magic.StartsWith(new byte?[] { 0x1e, 0x00, 0x00, 0x00 }) && extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BSP;

            #endregion

            #region BZip2

            if (magic.StartsWith(new byte?[] { 0x42, 0x52, 0x68 }))
                return WrapperType.BZip2;

            if (extension.Equals("bz2", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BZip2;

            #endregion

            #region CFB

            if (magic.StartsWith(new byte?[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }))
                return WrapperType.CFB;

            // Installer package
            if (extension.Equals("msi", StringComparison.OrdinalIgnoreCase))
                return WrapperType.CFB;

            // Merge module
            else if (extension.Equals("msm", StringComparison.OrdinalIgnoreCase))
                return WrapperType.CFB;

            // Patch Package
            else if (extension.Equals("msp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.CFB;

            // Transform
            else if (extension.Equals("mst", StringComparison.OrdinalIgnoreCase))
                return WrapperType.CFB;

            // Patch Creation Properties
            else if (extension.Equals("pcp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.CFB;

            #endregion

            #region CHD

            if (magic.StartsWith(new byte?[] { 0x4D, 0x43, 0x6F, 0x6D, 0x70, 0x72, 0x48, 0x44 }))
                return WrapperType.CHD;

            #endregion

            #region CIA

            if (extension.Equals("cia", StringComparison.OrdinalIgnoreCase))
                return WrapperType.CIA;

            #endregion

            #region Executable

            // DOS MZ executable file format (and descendants)
            if (magic.StartsWith(new byte?[] { 0x4d, 0x5a }))
                return WrapperType.Executable;

            /*
            // None of the following are supported yet

            // Executable and Linkable Format
            if (magic.StartsWith(new byte?[] { 0x7f, 0x45, 0x4c, 0x46 }))
                return FileTypes.Executable;

            // Mach-O binary (32-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xce }))
                return FileTypes.Executable;

            // Mach-O binary (32-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xce, 0xfa, 0xed, 0xfe }))
                return FileTypes.Executable;

            // Mach-O binary (64-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xcf }))
                return FileTypes.Executable;

            // Mach-O binary (64-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xcf, 0xfa, 0xed, 0xfe }))
                return FileTypes.Executable;

            // Prefrred Executable File Format
            if (magic.StartsWith(new byte?[] { 0x4a, 0x6f, 0x79, 0x21, 0x70, 0x65, 0x66, 0x66 }))
                return FileTypes.Executable;
            */

            // DOS MZ executable file format (and descendants)
            if (extension.Equals("exe", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Executable;

            // DOS MZ library file format (and descendants)
            if (extension.Equals("dll", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Executable;

            #endregion

            #region GCF

            if (magic.StartsWith(new byte?[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }))
                return WrapperType.GCF;

            if (extension.Equals("gcf", StringComparison.OrdinalIgnoreCase))
                return WrapperType.GCF;

            #endregion

            #region GZIP

            if (magic.StartsWith(new byte?[] { 0x1f, 0x8b }))
                return WrapperType.GZIP;

            if (extension.Equals("gz", StringComparison.OrdinalIgnoreCase))
                return WrapperType.GZIP;

            #endregion

            #region IniFile

            if (extension.Equals("ini", StringComparison.OrdinalIgnoreCase))
                return WrapperType.IniFile;

            #endregion

            #region InstallShieldArchiveV3

            if (magic.StartsWith(new byte?[] { 0x13, 0x5D, 0x65, 0x8C }))
                return WrapperType.InstallShieldArchiveV3;

            if (extension.Equals("z", StringComparison.OrdinalIgnoreCase))
                return WrapperType.InstallShieldArchiveV3;

            #endregion

            #region InstallShieldCAB

            if (magic.StartsWith(new byte?[] { 0x49, 0x53, 0x63 }))
                return WrapperType.InstallShieldCAB;

            // Both InstallShieldCAB and MicrosoftCAB share the same extension

            #endregion

            #region LDSCRYPT

            if (magic.StartsWith(new byte?[] { 0x4C, 0x44, 0x53, 0x43, 0x52, 0x59, 0x50, 0x54 }))
                return WrapperType.LDSCRYPT;

            #endregion

            #region MicrosoftCAB

            if (magic.StartsWith(new byte?[] { 0x4d, 0x53, 0x43, 0x46 }))
                return WrapperType.MicrosoftCAB;

            // Both InstallShieldCAB and MicrosoftCAB share the same extension

            #endregion

            #region MicrosoftLZ

            if (magic.StartsWith(new byte?[] { 0x53, 0x5a, 0x44, 0x44, 0x88, 0xf0, 0x27, 0x33 }))
                return WrapperType.MicrosoftLZ;

            #endregion

            #region MoPaQ

            if (magic.StartsWith(new byte?[] { 0x4d, 0x50, 0x51, 0x1a }))
                return WrapperType.MoPaQ;

            if (magic.StartsWith(new byte?[] { 0x4d, 0x50, 0x51, 0x1b }))
                return WrapperType.MoPaQ;

            if (extension.Equals("mpq", StringComparison.OrdinalIgnoreCase))
                return WrapperType.MoPaQ;

            #endregion

            #region N3DS

            // 3DS cart image
            if (extension.Equals("3ds", StringComparison.OrdinalIgnoreCase))
                return WrapperType.N3DS;

            // CIA package -- Not currently supported
            // else if (extension.Equals("cia", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.N3DS;

            #endregion

            #region NCF

            if (magic.StartsWith(new byte?[] { 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 }))
                return WrapperType.NCF;

            if (extension.Equals("ncf", StringComparison.OrdinalIgnoreCase))
                return WrapperType.NCF;

            #endregion

            #region Nitro

            // DS cart image
            if (extension.Equals("nds", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Nitro;

            // DS development cart image
            else if (extension.Equals("srl", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Nitro;

            // DSi cart image
            else if (extension.Equals("dsi", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Nitro;

            // iQue DS cart image
            else if (extension.Equals("ids", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Nitro;

            #endregion

            #region PAK

            if (magic.StartsWith(new byte?[] { 0x50, 0x41, 0x43, 0x4B }))
                return WrapperType.PAK;

            // Both PAK and Quantum share one extension
            // if (extension.Equals("pak", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.PAK;

            #endregion

            #region PFF

            // Version 2
            if (magic.StartsWith(new byte?[] { 0x14, 0x00, 0x00, 0x00, 0x50, 0x46, 0x46, 0x32 }))
                return WrapperType.PFF;

            // Version 3
            if (magic.StartsWith(new byte?[] { 0x14, 0x00, 0x00, 0x00, 0x50, 0x46, 0x46, 0x33 }))
                return WrapperType.PFF;

            // Version 4
            if (magic.StartsWith(new byte?[] { 0x14, 0x00, 0x00, 0x00, 0x50, 0x46, 0x46, 0x34 }))
                return WrapperType.PFF;

            if (extension.Equals("pff", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PFF;

            #endregion

            #region PKZIP

            // PKZIP (Unknown)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x00, 0x00 }))
                return WrapperType.PKZIP;

            // PKZIP
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x03, 0x04 }))
                return WrapperType.PKZIP;

            // PKZIP (Empty Archive)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x05, 0x06 }))
                return WrapperType.PKZIP;

            // PKZIP (Spanned Archive)
            if (magic.StartsWith(new byte?[] { 0x50, 0x4b, 0x07, 0x08 }))
                return WrapperType.PKZIP;

            // PKZIP
            if (extension.Equals("zip", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Android package
            if (extension.Equals("apk", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Java archive
            if (extension.Equals("jar", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Google Earth saved working session file
            if (extension.Equals("kmz", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // KWord document
            if (extension.Equals("kwd", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Microsoft Office Open XML Format (OOXML) Document
            if (extension.Equals("docx", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Microsoft Office Open XML Format (OOXML) Presentation
            if (extension.Equals("pptx", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Microsoft Office Open XML Format (OOXML) Spreadsheet
            if (extension.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // OpenDocument text document
            if (extension.Equals("odt", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // OpenDocument presentation
            if (extension.Equals("odp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // OpenDocument text document template
            if (extension.Equals("ott", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Microsoft Open XML paper specification file
            if (extension.Equals("oxps", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // OpenOffice spreadsheet
            if (extension.Equals("sxc", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // OpenOffice drawing
            if (extension.Equals("sxd", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // OpenOffice presentation
            if (extension.Equals("sxi", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // OpenOffice word processing
            if (extension.Equals("sxw", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // StarOffice spreadsheet
            if (extension.Equals("sxc", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Windows Media compressed skin file
            if (extension.Equals("wmz", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // Mozilla Browser Archive
            if (extension.Equals("xpi", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // XML paper specification file
            if (extension.Equals("xps", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            // eXact Packager Models
            if (extension.Equals("xpt", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PKZIP;

            #endregion

            #region PLJ

            // https://www.iana.org/assignments/media-types/audio/vnd.everad.plj
            if (magic.StartsWith(new byte?[] { 0xFF, 0x9D, 0x53, 0x4B }))
                return WrapperType.PlayJAudioFile;

            // https://www.iana.org/assignments/media-types/audio/vnd.everad.plj
            if (extension.Equals("plj", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PlayJAudioFile;

            #endregion

            #region Quantum

            if (magic.StartsWith(new byte?[] { 0x44, 0x53 }))
                return WrapperType.Quantum;

            if (extension.Equals("q", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Quantum;

            // Both PAK and Quantum share one extension
            // if (extension.Equals("pak", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.Quantum;

            #endregion

            #region RAR

            // RAR archive version 1.50 onwards
            if (magic.StartsWith(new byte?[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x00 }))
                return WrapperType.RAR;

            // RAR archive version 5.0 onwards
            if (magic.StartsWith(new byte?[] { 0x52, 0x61, 0x72, 0x21, 0x1a, 0x07, 0x01, 0x00 }))
                return WrapperType.RAR;

            if (extension.Equals("rar", StringComparison.OrdinalIgnoreCase))
                return WrapperType.RAR;

            #endregion

            #region RealArcade

            // RASGI2.0
            // Found in the ".rgs files in IA item "Nova_RealArcadeCD_USA".
            if (magic.StartsWith(new byte?[] { 0x52, 0x41, 0x53, 0x47, 0x49, 0x32, 0x2E, 0x30 }))
                return WrapperType.RealArcadeInstaller;

            // XZip2.0
            // Found in the ".mez" files in IA item "Nova_RealArcadeCD_USA".
            if (magic.StartsWith(new byte?[] { 0x58, 0x5A, 0x69, 0x70, 0x32, 0x2E, 0x30 }))
                return WrapperType.RealArcadeMezzanine;

            #endregion

            #region SevenZip

            if (magic.StartsWith(new byte?[] { 0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c }))
                return WrapperType.SevenZip;

            if (extension.Equals("7z", StringComparison.OrdinalIgnoreCase))
                return WrapperType.SevenZip;

            #endregion

            #region SFFS

            // Found in Redump entry 81756, confirmed to be "StarForce Filesystem" by PiD.
            if (magic.StartsWith(new byte?[] { 0x53, 0x46, 0x46, 0x53 }))
                return WrapperType.SFFS;

            #endregion 

            #region SGA

            if (magic.StartsWith(new byte?[] { 0x5F, 0x41, 0x52, 0x43, 0x48, 0x49, 0x56, 0x45 }))
                return WrapperType.SGA;

            if (extension.Equals("sga", StringComparison.OrdinalIgnoreCase))
                return WrapperType.SGA;

            #endregion

            #region TapeArchive

            if (magic.StartsWith(new byte?[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30 }))
                return WrapperType.TapeArchive;

            if (magic.StartsWith(new byte?[] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00 }))
                return WrapperType.TapeArchive;

            if (extension.Equals("tar", StringComparison.OrdinalIgnoreCase))
                return WrapperType.SevenZip;

            #endregion

            #region Textfile

            // Not all textfiles can be determined through magic number

            // HTML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x68, 0x74, 0x6d, 0x6c }))
                return WrapperType.Textfile;

            // HTML and XML
            if (magic.StartsWith(new byte?[] { 0x3c, 0x21, 0x44, 0x4f, 0x43, 0x54, 0x59, 0x50, 0x45 }))
                return WrapperType.Textfile;

            // InstallShield Compiled Rules
            if (magic.StartsWith(new byte?[] { 0x61, 0x4C, 0x75, 0x5A }))
                return WrapperType.Textfile;

            // Microsoft Office File (old)
            if (magic.StartsWith(new byte?[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 }))
                return WrapperType.Textfile;

            // Rich Text File
            if (magic.StartsWith(new byte?[] { 0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31 }))
                return WrapperType.Textfile;

            // Windows Help File
            if (magic.StartsWith(new byte?[] { 0x3F, 0x5F, 0x03, 0x00 }))
                return WrapperType.Textfile;

            // XML 
            // "<?xml"
            if (magic.StartsWith(new byte?[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C }))
                return WrapperType.Textfile;

            // "Description in Zip"
            if (extension.Equals("diz", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // Generic textfile (no header)
            if (extension.Equals("txt", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // HTML
            if (extension.Equals("htm", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;
            if (extension.Equals("html", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // InstallShield Script
            if (extension.Equals("ins", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // Microsoft Office File (old)
            if (extension.Equals("doc", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // Property list
            if (extension.Equals("plist", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // Rich Text File
            if (extension.Equals("rtf", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // Setup information
            if (extension.Equals("inf", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // Windows Help File
            if (extension.Equals("hlp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // WZC
            if (extension.Equals("wzc", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            // XML
            if (extension.Equals("xml", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Textfile;

            #endregion

            #region VBSP

            if (magic.StartsWith(new byte?[] { 0x56, 0x42, 0x53, 0x50 }))
                return WrapperType.VBSP;

            // Shares an extension with BSP
            if (extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.VBSP;

            #endregion

            #region VPK

            if (magic.StartsWith(new byte?[] { 0x34, 0x12, 0xaa, 0x55 }))
                return WrapperType.VPK;

            // Common extension so this cannot be used accurately
            // if (extension.Equals("vpk", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.VPK;

            #endregion

            #region WAD

            if (magic.StartsWith(new byte?[] { 0x57, 0x41, 0x44, 0x33 }))
                return WrapperType.WAD;

            // Common extension so this cannot be used accurately
            // if (extension.Equals("wad", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.WAD;

            #endregion

            #region XZ

            if (magic.StartsWith(new byte?[] { 0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00 }))
                return WrapperType.XZ;

            if (extension.Equals("xz", StringComparison.OrdinalIgnoreCase))
                return WrapperType.XZ;

            #endregion

            #region XZP

            if (magic.StartsWith(new byte?[] { 0x70, 0x69, 0x5A, 0x78 }))
                return WrapperType.XZP;

            if (extension.Equals("xzp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.XZP;

            #endregion

            // We couldn't find a supported match
            return WrapperType.UNKNOWN;
        }
    }
}
