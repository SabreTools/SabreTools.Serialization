# SabreTools.Data.Models

This library comprises of models that represent either directly serializable or representative structures for all SabreTools projects. The majority of models will have deserializers and wrappers, with some also having both regular and cross-model serializers.

## Namespaces

Below is a list of all existing namespaces with the `SabreTools.Data.Models` prefix removed, along with a brief description of the contained models.

| Namespace | Description |
| --- | --- |
| `AACS` | AACS media key block |
| `AdvancedInstaller` | Caphyon Advanced Installer SFX |
| `ArchiveDotOrg` | archive.org metadata files |
| `ASN1` | ASN.1 type/length/value |
| `Atari7800` | Atari 7800 carts |
| `AtariLynx` | Atari Lynx carts |
| `AttractMode` | AttractMode metadata files |
| `BDPlus` | BD+ SVM |
| `BFPK` | BFPK custom archive format |
| `Bitmap` | Bitmap graphics |
| `BSP` | Valve BSP and VBSP level file formats |
| `BZip2` | BZip2 archive |
| `CDROM` | ISO 10149 / ECMA-130 CD-ROM images |
| `CFB` | Microsoft Compound File Binary (CFB) file format |
| `Charts` | Rhythm game song and career formats |
| `CHD` | MAME Compressed Hunks of Data (CHD) |
| `ClrMamePro` | ClrMamePro (CMP) metadata files |
| `COFF` | Common Object File Format (COFF) |
| `CueSheets` | Cuesheet |
| `Delphi` | Delphi-specific PE blocks |
| `DosCenter` | DosCenter (DC) metadata files |
| `DVD` | Various DVD file formats |
| `EverdriveSMDB` | Everdrive SmokeMonster Database (SMDB) metadata files |
| `GameHeader` | GameHeader cart information metadata files |
| `GCF` | Half-Life Game Cache File (GCF) |
| `GZIP` | gzip archive |
| `Hashfile` | Individual hash metadata file formats |
| `InstallShieldArchiveV3` | InstallShield archive V3 (.z) |
| `InstallShieldCabinet` | InstallShield cabinet (IS-CAB) |
| `InstallShieldExecutable` | InstallShield Executable SFX |
| `IRD` | PS3 IRD encryption files |
| `ISO9660` | ISO9660 / EMCA-119 file system |
| `LDSCRYPT` | LDSCRYPT encrypted files |
| `LinearExecutable` | Linear Executable (LE/LX) |
| `Listrom` | MAME Listrom metadata files |
| `Listxml` | MAME ListXML metadata files |
| `Logiqx` | Logiqx XML-based metadata files |
| `LZ` | LZ compressed file variants |
| `Metadata` | Common unified metadata format |
| `MicrosoftCabinet` | Microsoft cabinet (MS-CAB) |
| `MoPaQ` | MoPaQ (MPQ) game installer archive |
| `MSDOS` | MS-DOS executable and stub |
| `N3DS` | Nintendo 3DS carts |
| `NCF` | Half-Life No Cache File |
| `NES` | Nintendo Entertainment System (NES) and Famicom (FC) carts |
| `NewExecutable` | New Executable (NE) |
| `Nitro` | Nintendo DS carts |
| `OfflineList` | OfflineList XML-based metadata files |
| `OLE` | Object Linking and Embedding (OLE) |
| `OpenMSX` | OpenMSX XML-based metadata files |
| `PAK` | Half-Life Package File |
| `PFF` | PFF archive |
| `PIC` | Disc Information and Emergency Brake data |
| `PKZIP` | PKWARE ZIP archive and derivatives |
| `PlayJ` | PlayJ propietary music format |
| `PlayStation3` | Sony PlayStation 3 (PS3) on-disc formats |
| `PlayStation4` | Sony PlayStation 4 (PS4) on-disc formats |
| `PortableExecutable` | Portable Executable (PE) |
| `Quantum` | Quantum archive |
| `RAR` | RAR archive |
| `RealArcade` | RealArcade installer formats |
| `RomCenter` | RomCenter INI-based metadata files |
| `SafeDisc` | SafeDisc encrypted file entry |
| `SecuROM` | SecuROM protection block formats |
| `SeparatedValue` | Separated-value (CSV, SSV, TSV) metadata files |
| `SevenZip` | 7-zip archive |
| `SGA` | SGA game archive |
| `SNES` | Super Nintendo Entertainment System (SNES) and Super Famicom (SFC) carts |
| `SoftwareList` | MAME software list metadata files |
| `SpoonInstaller` | Spoon Installer SFX |
| `StarForce` | StarForce File System (SFFS) |
| `TAR` | Tape Archive (TAR) |
| `VDF` | Steam SIS files |
| `VPK` | Valve Package File |
| `WAD3` | Half-Life Texture Package File |
| `WiseInstaller` | Wise script- and section-based installers |
| `Xbox` | Xbox and Xbox 360 file formats |
| `XboxExecutable` | Xbox Executable (XBE) |
| `XZ` | xz archive |
| `XZP` | XBox Package File |
| `ZSTD` | ZSTD archive |

## Notable Information Sources

Not all of this information was able to be gathered directly from the files in question, so here is a non-exhaustive list of sites and projects that allow this project to exist. Most of the models also have direct links to specific pages or code snippets, where available.

| Site/Project | Models Directory |
| --- | --- |
| [3DBrew](https://www.3dbrew.org/wiki/Main_Page) | N3DS |
| [Aaru Data Preservation Suite](https://github.com/aaru-dps) | PIC |
| [Advanced Access Content System Licensing Administrator (AACS LA)](https://aacsla.com/) | AACS |
| [Atari 7800 Development Wiki](https://7800.8bitdev.org/index.php/Main_Page) | Atari7800Cart |
| [BYTE*](https://web.archive.org/web/20240703222951/https://bytepointer.com/index.htm) | NewExecutable |
| [cabextract/libmspack](https://www.cabextract.org.uk/) | LZ |
| [cxbx](https://www.caustik.com/cxbx/) | XboxExecutable |
| [Cxbx-Reloaded](https://github.com/Cxbx-Reloaded/Cxbx-Reloaded/) | XboxExecutable |
| [DBox](https://dbox.tools/) | Xbox |
| [DebugInfo.com](https://www.debuginfo.com/index.html) | PortableExecutable |
| [Devilsclaw](https://devilsclaws.net/) | PFF |
| [DSiBrew](https://dsibrew.org/wiki/Main_Page) | Nitro |
| [DVD Resources for Open Source Development](https://dvd.sourceforge.net/) | DVD |
| [ECMA-119](https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf) | ISO9660 |
| [EDM/2](https://www.edm2.com/index.php/Main_Page) | LinearExecutable |
| [faydoc.tripod.com](https://faydoc.tripod.com/formats/) | LinearExecutable |
| [GuitarGame_ChartFormats](https://github.com/TheNathannator/GuitarGame_ChartFormats/) | Charts |
| [HandWiki](https://handwiki.org/wiki/Start) | Quantum |
| [Handy SDL](https://github.com/mozzwald/handy-sdl) | AtariLynxCart |
| [HLLib](https://github.com/RavuAlHemio/hllib/) | BSP, GCF, NCF, PAK, SGA, VPK, WAD3, XZP |
| [IBM Documentation](https://www.ibm.com/docs/en) | TAR |
| [IETF](https://www.ietf.org/) | GZIP |
| [Independent Commodore Library](https://petlibrary.tripod.com/) | PKZIP |
| [Joe Tsai](https://github.com/dsnet/compress/blob/master/doc/bzip2-format.pdf) | BZip2 |
| [Ladislav Zezula](http://zezula.net/en/tools/main.html) | MoPaQ |
| [libaacs](https://code.videolan.org/videolan/libaacs/) | AACS |
| [libbdplus](https://github.com/mwgoldsmith/bdplus) | BD+ |
| [libexe](https://github.com/libyal/libexe/) | NewExecutable |
| [MAME](https://github.com/mamedev/mame/) | CHD |
| [Matthew Russotto](http://www.russotto.net/quantumcomp.html) | Compression/Quantum |
| [Microsoft Learn](https://learn.microsoft.com/en-us/) | BMP, CFB, Compression/LZX, Compression/MSZIP, MicrosoftCabinet, OLE, PortableExecutable, SecuROM, WiseInstaller |
| [msitools](https://github.com/GNOME/msitools/) | CFB |
| [Nesdev Wiki](https://www.nesdev.org/wiki/Nesdev_Wiki) | FDS, NESCart, QD |
| [OSDev.org](https://wiki.osdev.org/Expanded_Main_Page) | MSDOS, NewExecutable |
| [PInvoke.net](http://www.pinvoke.net/index.aspx) | MSDOS |
| [PKWARE(?)](https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT) | PKZIP |
| [PS3 Developer Wiki](https://www.psdevwiki.com/ps3/) | IRD, PlayStation3 |
| [RanseiLink](https://github.com/Deijin27/RanseiLink) | Nitro |
| [raphnet](https://www.raphnet.net/divers/documentation/Sneskart.txt) | SNES |
| [Reverse Engineering Wise](https://codeberg.org/CYBERDEV/REWise) | WiseInstaller |
| [SharpCompress](https://github.com/adamhathcock/sharpcompress) | PKZIP |
| [Source SDK 2013](https://github.com/ValveSoftware/source-sdk-2013) | BSP |
| [Technical Committee T10](https://www.t10.org/) | PIC |
| [The Go tools for Windows + Assembler](https://www.godevtool.com/) | PortableExecutable |
| [The Whole Half-Life](https://twhl.info/wiki/page/Specification:_WAD3) | WAD3 |
| [Tukaani](https://tukaani.org/xz/format.html) | XZ |
| [Unshield](https://github.com/twogood/unshield) | InstallShieldCabinet |
| [unshieldv3](https://github.com/wfr/unshieldv3) | InstallShieldArchiveV3 |
| [Valve Developer Community](https://developer.valvesoftware.com/wiki/Main_Page) | BSP, VPK |
| [w4kfu's bl0g](https://web.archive.org/web/20250206143756/http://blog.w4kfu.com/) | SafeDisc |
| [Wine](https://github.com/wine-mirror/wine/) | LZ |
| [XboxDevWiki.net](https://xboxdevwiki.net/Main_Page) | Xbox |
| [XeNTaX Game Research Forum](http://web.archive.org/web/20231031162306/https://forum.xentax.com/) | SFFS |
