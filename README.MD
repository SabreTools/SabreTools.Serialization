# SabreTools.Serialization

[![Build and Test](https://github.com/SabreTools/SabreTools.Serialization/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/SabreTools/SabreTools.Serialization/actions/workflows/build_and_test.yml)

This meta-library comprises of serializers that both read and write from files and streams to the dedicated models as well as convert to and from the common internal models. This library is partially used by the current parsing and writing code but none of the internal model serialization is used.

Find the link to the Nuget package [here](https://www.nuget.org/packages/SabreTools.Serialization).

The following non-project libraries (or ports thereof) are used for file handling:

- [GrindCore.SharpCompress](https://github.com/Nanook/GrindCore.SharpCompress) - Common archive format extraction
- [StormLibSharp](https://github.com/robpaveza/stormlibsharp) - MoPaQ extraction [Unused in non-Windows builds due to Windows-specific libraries]

The following projects have influenced this library:

- [libmspack](https://github.com/kyz/libmspack) - Documentation around the MS-CAB format and associated compression methods.
- [Unshield](https://github.com/twogood/unshield/) - InstallShield CAB extraction tool that influenced internal handling

## Releases

For the most recent stable build, download the latest release here: [Releases Page](https://github.com/SabreTools/SabreTools.Serialization/releases)

For the latest WIP build here: [Rolling Release](https://github.com/SabreTools/SabreTools.Serialization/releases/tag/rolling)

## InfoPrint

**InfoPrint** is a reference implementation for the deserialization and printing features of the library, packaged as a standalone executable for all supported platforms. It will attempt to detect and display information about many supported file types, optionally both hashing the file and outputting the information to a JSON file (.NET Core 3.1 and above only).

```text
InfoPrint <options> file|directory ...

Options:
-?, -h, --help           Display this help text
-d, --debug              Enable debug mode
-c, --hash               Output file hashes
-f, --file               Print to file only
-j, --json               Print info as JSON
```

## ExtractionTool

**ExtractionTool** is a reference implementation for the extraction features of the library, packaged as a standalone executable for all supported platforms. It will attempt to detect and extract many supported file types. See the table below for supported extraction functionality.

```text
ExtractionTool <options> file|directory ...

Options:
-?, -h, --help           Display this help text
-d, --debug              Enable debug mode
-o, --outdir [PATH]      Set output path for extraction (required)
```

| Format Name | Notes |
| --- | --- |
| 7-zip archive | .NET Framework 4.6.2 and greater |
| Atari 7800 (A78) Cart Image | Header and ROM data |
| Atari Lynx (LNX) Cart Image | Header and ROM data |
| BFPK custom archive format | |
| bzip2 archive | .NET Framework 4.6.2 and greater |
| Compound File Binary (CFB) | Only CFB common pieces extractable |
| fwNES FDS file | Header and disk data |
| gzip archive | |
| Half-Life Game Cache File (GCF) | |
| Half-Life Level (BSP) | |
| Half-Life Package File (PAK) | |
| Half-Life Texture Package File (WAD3) | |
| Half-Life 2 Level (VBSP) | |
| InstallShield Archive V3 (Z) | |
| InstallShield CAB | |
| Microsoft cabinet file | Does not support LZX or Quantum compression |
| Microsoft LZ-compressed files | KWAJ, QBasic, and SZDD variants |
| MoPaQ game data archive (MPQ) | Windows only |
| New Exectuable | Embedded archives and executables in the overlay and Wise installer |
| Nintendo Entertainment System (NES) Cart Image | Header, Trainer data, PRG-ROM, CHR-ROM, Unheadered ROM, and PlayChoice-10 INST-ROM |
| NovaLogic Game Archive Format (PFF) | |
| PKZIP and derived files (ZIP, etc.) | .NET Framework 4.6.2 and greater |
| Portable Executable | Embedded archives and executables in the resources and overlay, CExe-packed data, SFX archives (7-zip, PKZIP, and RAR), and Wise installer |
| Quantum archive (Q) | Currently not working |
| RAR archive (RAR) | .NET Framework 4.6.2 and greater |
| SGA game archive | |
| Tape archive (TAR) | |
| Valve Package File (VPK) | |
| XBox Package File (XZP) | |
| xz archive (XZ) | .NET Framework 4.6.2 and greater |

## Interfaces

Below is a table representing the various conversion interfaces that are implemented within this library.

| Interface Name | Source Type | Destination Type |
| --- | --- | --- |
| `SabreTools.Serialization.CrossModel.ICrossModel<TSource, TDest>` | `TSource`/`TDest` | `TDest`/`TSource` |
| `SabreTools.Serialization.Readers.IByteReader<TModel>` | `byte[]?` | `TModel` |
| `SabreTools.Serialization.Readers.IFileReader<TModel>` | `string?` path | `TModel` |
| `SabreTools.Serialization.Readers.IStreamReader<TModel>` | `Stream?` | `TModel` |
| `SabreTools.Serialization.Readers.IStringReader<TModel>` | `string?` representation | `TModel` |
| `SabreTools.Serialization.Writers.IByteWriter<TModel>` | `TModel` | `byte[]?` |
| `SabreTools.Serialization.Writers.IFileWriter<TModel>` | `TModel` | `string?` path |
| `SabreTools.Serialization.Writers.IStreamWriter<TModel>` | `TModel` | `Stream?` |
| `SabreTools.Serialization.Writers.IStringWriter<TModel>` | `TModel` | `string?` representation |

Below is a table representing the various non-conversion interfaces that are implemented within this library.

| Interface Name | Purpose |
| --- | --- |
| `SabreTools.Serialization.Wrappers.IExtractable` | Marks a wrapper as able to be extracted |
| `SabreTools.Serialization.Wrappers.IPrintable` | Marks a wrapper as able to print model information |
| `SabreTools.Serialization.Wrappers.IWrapper` | Represents an item with a description and JSON serializable state, allowing for extensions |
| `SabreTools.Serialization.Wrappers.IWrapper<TModel>` | Wraps a model with source data, allowing for extensions |

## Namespaces

Below is a table of all namespaces within the library and what they represent

| Namespace | Description |
| --- | --- |
| `SabreTools.Data.Attributes` | Common attributes for data marking and manipulation |
| `SabreTools.Data.Extensions` | Extension methods related to models |
| `SabreTools.Data.Models` | Models representing different file and structure types |
| `SabreTools.ObjectIdentifier` | Object Identifier (OID) parsing |
| `SabreTools.Serialization.CrossModel` | Convert between models; mainly used for metadata files converting to and from a common, `Dictionary`-based model |
| `SabreTools.Serialization.Readers` | Convert from external sources to models |
| `SabreTools.Serialization.Wrappers` | Classes that wrap serialization and models to allow for including extension properties |
| `SabreTools.Serialization.Writers` | Convert from models to external sources |
