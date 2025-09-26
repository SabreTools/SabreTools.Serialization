using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace SabreTools.Serialization
{
    public static class WrapperFactory
    {
        /// <summary>
        /// Create an instance of a wrapper based on file type
        /// </summary>
        public static IWrapper? CreateWrapper(WrapperType fileType, Stream? data)
        {
            return fileType switch
            {
                WrapperType.AACSMediaKeyBlock => AACSMediaKeyBlock.Create(data),
                WrapperType.BDPlusSVM => BDPlusSVM.Create(data),
                WrapperType.BFPK => BFPK.Create(data),
                WrapperType.BSP => BSP.Create(data),
                WrapperType.BZip2 => BZip2.Create(data),
                WrapperType.CFB => CFB.Create(data),
                WrapperType.CHD => CHD.Create(data),
                WrapperType.CIA => CIA.Create(data),
                WrapperType.Executable => CreateExecutableWrapper(data),
                WrapperType.GCF => GCF.Create(data),
                WrapperType.GZip => GZip.Create(data),
                WrapperType.IniFile => null,// TODO: Implement wrapper
                WrapperType.InstallShieldArchiveV3 => InstallShieldArchiveV3.Create(data),
                WrapperType.InstallShieldCAB => InstallShieldCabinet.Create(data),
                WrapperType.LDSCRYPT => LDSCRYPT.Create(data),
                WrapperType.LZKWAJ => LZKWAJ.Create(data),
                WrapperType.LZQBasic => LZQBasic.Create(data),
                WrapperType.LZSZDD => LZSZDD.Create(data),
                WrapperType.MicrosoftCAB => MicrosoftCabinet.Create(data),
                WrapperType.MoPaQ => MoPaQ.Create(data),
                WrapperType.N3DS => N3DS.Create(data),
                WrapperType.NCF => NCF.Create(data),
                WrapperType.Nitro => Nitro.Create(data),
                WrapperType.PAK => PAK.Create(data),
                WrapperType.PFF => PFF.Create(data),
                WrapperType.PIC => PIC.Create(data),
                WrapperType.PKZIP => PKZIP.Create(data),
                WrapperType.PlayJAudioFile => PlayJAudioFile.Create(data),
                WrapperType.PlayJPlaylist => PlayJPlaylist.Create(data),
                WrapperType.Quantum => Quantum.Create(data),
                WrapperType.RAR => RAR.Create(data),
                WrapperType.RealArcadeInstaller => RealArcadeInstaller.Create(data),
                WrapperType.RealArcadeMezzanine => RealArcadeMezzanine.Create(data),
                WrapperType.SecuROMDFA => SecuROMDFA.Create(data),
                WrapperType.SevenZip => SevenZip.Create(data),
                WrapperType.SFFS => SFFS.Create(data),
                WrapperType.SGA => SGA.Create(data),
                WrapperType.TapeArchive => TapeArchive.Create(data),
                WrapperType.Textfile => null,// TODO: Implement wrapper
                WrapperType.VBSP => VBSP.Create(data),
                WrapperType.VPK => VPK.Create(data),
                WrapperType.WAD => WAD3.Create(data),
                WrapperType.XZ => XZ.Create(data),
                WrapperType.XZP => XZP.Create(data),
                _ => null,
            };
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

            // Cache the current offset
            long initialOffset = stream.Position;

            // Try to get an MS-DOS wrapper first
            var wrapper = MSDOS.Create(stream);
            if (wrapper == null || wrapper is not MSDOS msdos)
                return null;

            // Check for a valid new executable address
            if (msdos.Model.Header?.NewExeHeaderAddr == null || initialOffset + msdos.Model.Header.NewExeHeaderAddr >= stream.Length)
                return wrapper;

            // Try to read the executable info
            stream.Seek(initialOffset + msdos.Model.Header.NewExeHeaderAddr, SeekOrigin.Begin);
            var magic = stream.ReadBytes(4);

            // If we didn't get valid data at the offset
            if (magic == null)
            {
                return wrapper;
            }

            // New Executable
            else if (magic.StartsWith(Models.NewExecutable.Constants.SignatureBytes))
            {
                stream.Seek(initialOffset, SeekOrigin.Begin);
                return NewExecutable.Create(stream);
            }

            // Linear Executable
            else if (magic.StartsWith(Models.LinearExecutable.Constants.LESignatureBytes)
                || magic.StartsWith(Models.LinearExecutable.Constants.LXSignatureBytes))
            {
                stream.Seek(initialOffset, SeekOrigin.Begin);
                return LinearExecutable.Create(stream);
            }

            // Portable Executable
            else if (magic.StartsWith(Models.PortableExecutable.Constants.SignatureBytes))
            {
                stream.Seek(initialOffset, SeekOrigin.Begin);
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

            // TODO: Use constants from Models here
            #region AACSMediaKeyBlock

            // Block starting with verify media key record
            if (magic.StartsWith([0x81, 0x00, 0x00, 0x14]))
                return WrapperType.AACSMediaKeyBlock;

            // Block starting with type and version record
            if (magic.StartsWith([0x10, 0x00, 0x00, 0x0C]))
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

            if (magic.StartsWith(Models.BDPlus.Constants.SignatureBytes))
                return WrapperType.BDPlusSVM;

            if (extension.Equals("svm", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BDPlusSVM;

            #endregion

            #region BFPK

            if (magic.StartsWith(Models.BFPK.Constants.SignatureBytes))
                return WrapperType.BFPK;

            #endregion

            // TODO: Use constants from Models here
            #region BSP

            // Shares a first 4 bytes with some .mc files
            // Shares an extension with VBSP
            if (magic.StartsWith([0x1d, 0x00, 0x00, 0x00]) && extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BSP;

            // Shares a first 4 bytes with some .mc files
            // Shares an extension with VBSP
            if (magic.StartsWith([0x1e, 0x00, 0x00, 0x00]) && extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BSP;

            #endregion

            #region BZip2

            if (magic.StartsWith(Models.BZip2.Constants.SignatureBytes))
                return WrapperType.BZip2;

            if (extension.Equals("bz2", StringComparison.OrdinalIgnoreCase))
                return WrapperType.BZip2;

            #endregion

            #region CFB

            if (magic.StartsWith(Models.CFB.Constants.SignatureBytes))
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

            if (magic.StartsWith(Models.CHD.Constants.SignatureBytes))
                return WrapperType.CHD;

            #endregion

            #region CIA

            if (extension.Equals("cia", StringComparison.OrdinalIgnoreCase))
                return WrapperType.CIA;

            #endregion

            #region Executable

            // DOS MZ executable file format (and descendants)
            if (magic.StartsWith(Models.MSDOS.Constants.SignatureBytes))
                return WrapperType.Executable;

            // None of the following are supported yet
            /*
            // Executable and Linkable Format
            if (magic.StartsWith([0x7f, 0x45, 0x4c, 0x46]))
                return WrapperType.Executable;

            // Mach-O binary (32-bit)
            if (magic.StartsWith([0xfe, 0xed, 0xfa, 0xce]))
                return WrapperType.Executable;

            // Mach-O binary (32-bit, reverse byte ordering scheme)
            if (magic.StartsWith([0xce, 0xfa, 0xed, 0xfe]))
                return WrapperType.Executable;

            // Mach-O binary (64-bit)
            if (magic.StartsWith([0xfe, 0xed, 0xfa, 0xcf]))
                return WrapperType.Executable;

            // Mach-O binary (64-bit, reverse byte ordering scheme)
            if (magic.StartsWith([0xcf, 0xfa, 0xed, 0xfe]))
                return WrapperType.Executable;

            // Prefrred Executable File Format
            if (magic.StartsWith([0x4a, 0x6f, 0x79, 0x21, 0x70, 0x65, 0x66, 0x66]))
                return WrapperType.Executable;
            */

            // DOS MZ executable file format (and descendants)
            if (extension.Equals("exe", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Executable;

            // DOS MZ library file format (and descendants)
            if (extension.Equals("dll", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Executable;

            #endregion

            // TODO: Use constants from Models here
            #region GCF

            if (magic.StartsWith([0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00]))
                return WrapperType.GCF;

            if (extension.Equals("gcf", StringComparison.OrdinalIgnoreCase))
                return WrapperType.GCF;

            #endregion

            #region GZip

            if (magic.StartsWith(new byte[] { Models.GZIP.Constants.ID1, Models.GZIP.Constants.ID2 }))
                return WrapperType.GZip;

            if (extension.Equals("gz", StringComparison.OrdinalIgnoreCase))
                return WrapperType.GZip;

            #endregion

            #region IniFile

            if (extension.Equals("ini", StringComparison.OrdinalIgnoreCase))
                return WrapperType.IniFile;

            #endregion

            #region InstallShieldArchiveV3

            if (magic.StartsWith(Models.InstallShieldArchiveV3.Constants.HeaderSignatureBytes))
                return WrapperType.InstallShieldArchiveV3;

            if (extension.Equals("z", StringComparison.OrdinalIgnoreCase))
                return WrapperType.InstallShieldArchiveV3;

            #endregion

            #region InstallShieldCAB

            if (magic.StartsWith(Models.InstallShieldCabinet.Constants.SignatureBytes))
                return WrapperType.InstallShieldCAB;

            // Both InstallShieldCAB and MicrosoftCAB share the same extension

            #endregion

            #region LDSCRYPT

            if (magic.StartsWith(Models.LDSCRYPT.Constants.SignatureBytes))
                return WrapperType.LDSCRYPT;

            #endregion

            #region LZ

            if (magic.StartsWith(Models.LZ.Constants.KWAJSignatureBytes))
                return WrapperType.LZKWAJ;

            if (magic.StartsWith(Models.LZ.Constants.QBasicSignatureBytes))
                return WrapperType.LZQBasic;

            if (magic.StartsWith(Models.LZ.Constants.SZDDSignatureBytes))
                return WrapperType.LZSZDD;

            #endregion

            #region MicrosoftCAB

            if (magic.StartsWith(Models.MicrosoftCabinet.Constants.SignatureBytes))
                return WrapperType.MicrosoftCAB;

            // Both InstallShieldCAB and MicrosoftCAB share the same extension

            #endregion

            #region MoPaQ

            if (magic.StartsWith(Models.MoPaQ.Constants.ArchiveHeaderSignatureBytes))
                return WrapperType.MoPaQ;

            if (magic.StartsWith(Models.MoPaQ.Constants.UserDataSignatureBytes))
                return WrapperType.MoPaQ;

            if (extension.Equals("mpq", StringComparison.OrdinalIgnoreCase))
                return WrapperType.MoPaQ;

            #endregion

            #region N3DS

            // 3DS cart image
            if (extension.Equals("3ds", StringComparison.OrdinalIgnoreCase))
                return WrapperType.N3DS;

            else if (extension.Equals("cci", StringComparison.OrdinalIgnoreCase))
                return WrapperType.N3DS;

            // CIA package -- Not currently supported
            // else if (extension.Equals("cia", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.N3DS;

            #endregion

            // TODO: Use constants from Models here
            #region NCF

            if (magic.StartsWith([0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00]))
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

            if (magic.StartsWith(Models.PAK.Constants.SignatureBytes))
                return WrapperType.PAK;

            // Both PAK and Quantum share one extension
            // if (extension.Equals("pak", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.PAK;

            #endregion

            // TODO: Use constants from Models here
            #region PFF

            // Version 0
            if (magic.StartsWith([0x14, 0x00, 0x00, 0x00, 0x50, 0x46, 0x46, 0x30]))
                return WrapperType.PFF;

            // Version 2
            if (magic.StartsWith([0x14, 0x00, 0x00, 0x00, 0x50, 0x46, 0x46, 0x32]))
                return WrapperType.PFF;

            // Version 3
            if (magic.StartsWith([0x14, 0x00, 0x00, 0x00, 0x50, 0x46, 0x46, 0x33]))
                return WrapperType.PFF;

            // Version 4
            if (magic.StartsWith([0x14, 0x00, 0x00, 0x00, 0x50, 0x46, 0x46, 0x34]))
                return WrapperType.PFF;

            if (extension.Equals("pff", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PFF;

            #endregion

            // TODO: Use constants from Models here
            #region PKZIP

            // PKZIP (Unknown)
            if (magic.StartsWith([0x50, 0x4b, 0x00, 0x00]))
                return WrapperType.PKZIP;

            // PKZIP
            if (magic.StartsWith(Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                return WrapperType.PKZIP;

            // PKZIP (Empty Archive)
            if (magic.StartsWith(Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                return WrapperType.PKZIP;
            else if (magic.StartsWith(Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                return WrapperType.PKZIP;

            // PKZIP (Spanned Archive)
            if (magic.StartsWith(Models.PKZIP.Constants.DataDescriptorSignatureBytes))
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
            if (magic.StartsWith(Models.PlayJ.Constants.SignatureBytes))
                return WrapperType.PlayJAudioFile;

            // https://www.iana.org/assignments/media-types/audio/vnd.everad.plj
            if (extension.Equals("plj", StringComparison.OrdinalIgnoreCase))
                return WrapperType.PlayJAudioFile;

            #endregion

            #region Quantum

            if (magic.StartsWith(Models.Quantum.Constants.SignatureBytes))
                return WrapperType.Quantum;

            if (extension.Equals("q", StringComparison.OrdinalIgnoreCase))
                return WrapperType.Quantum;

            // Both PAK and Quantum share one extension
            // if (extension.Equals("pak", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.Quantum;

            #endregion

            #region RAR

            // RAR archive version 1.50 onwards
            if (magic.StartsWith(Models.RAR.Constants.OldSignatureBytes))
                return WrapperType.RAR;

            // RAR archive version 5.0 onwards
            if (magic.StartsWith(Models.RAR.Constants.NewSignatureBytes))
                return WrapperType.RAR;

            if (extension.Equals("rar", StringComparison.OrdinalIgnoreCase))
                return WrapperType.RAR;

            #endregion

            #region RealArcade

            // RASGI2.0
            // Found in the ".rgs files in IA item "Nova_RealArcadeCD_USA".
            if (magic.StartsWith(Models.RealArcade.Constants.RgsSignatureBytes))
                return WrapperType.RealArcadeInstaller;

            // XZip2.0
            // Found in the ".mez" files in IA item "Nova_RealArcadeCD_USA".
            if (magic.StartsWith(Models.RealArcade.Constants.MezzanineSignatureBytes))
                return WrapperType.RealArcadeMezzanine;

            #endregion

            #region SecuROM DFA

            if (magic.StartsWith(Models.SecuROM.Constants.DFAMagicBytes))
                return WrapperType.SecuROMDFA;

            #endregion

            #region SevenZip

            if (magic.StartsWith(Models.SevenZip.Constants.SignatureBytes))
                return WrapperType.SevenZip;

            if (extension.Equals("7z", StringComparison.OrdinalIgnoreCase))
                return WrapperType.SevenZip;

            #endregion

            #region SFFS

            // Found in Redump entry 81756, confirmed to be "StarForce Filesystem" by PiD.
            if (magic.StartsWith(Models.StarForce.Constants.SignatureBytes))
                return WrapperType.SFFS;

            #endregion 

            #region SGA

            if (magic.StartsWith(Models.SGA.Constants.SignatureBytes))
                return WrapperType.SGA;

            if (extension.Equals("sga", StringComparison.OrdinalIgnoreCase))
                return WrapperType.SGA;

            #endregion

            // TODO: Use constants from Models here
            #region TapeArchive

            if (magic.StartsWith([0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30]))
                return WrapperType.TapeArchive;

            if (magic.StartsWith([0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00]))
                return WrapperType.TapeArchive;

            if (extension.Equals("tar", StringComparison.OrdinalIgnoreCase))
                return WrapperType.TapeArchive;

            #endregion

            // TODO: Use constants from Models here
            #region Textfile

            // Not all textfiles can be determined through magic number

            // HTML
            if (magic.StartsWith([0x3c, 0x68, 0x74, 0x6d, 0x6c]))
                return WrapperType.Textfile;

            // HTML and XML
            if (magic.StartsWith([0x3c, 0x21, 0x44, 0x4f, 0x43, 0x54, 0x59, 0x50, 0x45]))
                return WrapperType.Textfile;

            // InstallShield Compiled Rules
            if (magic.StartsWith([0x61, 0x4C, 0x75, 0x5A]))
                return WrapperType.Textfile;

            // Microsoft Office File (old)
            if (magic.StartsWith([0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1]))
                return WrapperType.Textfile;

            // Rich Text File
            if (magic.StartsWith([0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31]))
                return WrapperType.Textfile;

            // Windows Help File
            if (magic.StartsWith([0x3F, 0x5F, 0x03, 0x00]))
                return WrapperType.Textfile;

            // XML 
            // "<?xml"
            if (magic.StartsWith([0x3C, 0x3F, 0x78, 0x6D, 0x6C]))
                return WrapperType.Textfile;

            // "<?xml" in UTF-16 encoding
            if (magic.StartsWith([0x3C, 0x00, 0x3F, 0x00, 0x78, 0x00, 0x6D, 0x00, 0x6C, 0x00]))
                return WrapperType.Textfile;

            // "<?xml" in UTF-16 encoding with byte order marks
            if (magic.StartsWith([0xFF, 0xFE, 0x3C, 0x00, 0x3F, 0x00, 0x78, 0x00, 0x6D, 0x00, 0x6C, 0x00]))
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

            if (magic.StartsWith(Models.BSP.Constants.SignatureBytes))
                return WrapperType.VBSP;

            // Shares an extension with BSP
            if (extension.Equals("bsp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.VBSP;

            #endregion

            #region VPK

            if (magic.StartsWith(Models.VPK.Constants.SignatureBytes))
                return WrapperType.VPK;

            // Common extension so this cannot be used accurately
            // if (extension.Equals("vpk", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.VPK;

            #endregion

            #region WAD

            if (magic.StartsWith(Models.WAD3.Constants.SignatureBytes))
                return WrapperType.WAD;

            // Common extension so this cannot be used accurately
            // if (extension.Equals("wad", StringComparison.OrdinalIgnoreCase))
            //     return WrapperType.WAD;

            #endregion

            #region XZ

            if (magic.StartsWith(Models.XZ.Constants.SignatureBytes))
                return WrapperType.XZ;

            if (extension.Equals("xz", StringComparison.OrdinalIgnoreCase))
                return WrapperType.XZ;

            #endregion

            #region XZP

            if (magic.StartsWith(Models.XZP.Constants.HeaderSignatureBytes))
                return WrapperType.XZP;

            if (extension.Equals("xzp", StringComparison.OrdinalIgnoreCase))
                return WrapperType.XZP;

            #endregion

            // We couldn't find a supported match
            return WrapperType.UNKNOWN;
        }
    }
}
