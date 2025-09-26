# SabreTools.Serialization

[![Build and Test](https://github.com/SabreTools/SabreTools.Serialization/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/SabreTools/SabreTools.Serialization/actions/workflows/build_and_test.yml)

This library comprises of serializers that both read and write from files and streams to the dedicated models as well as convert to and from the common internal models. This library is partially used by the current parsing and writing code but none of the internal model serialization is used.

Find the link to the Nuget package [here](https://www.nuget.org/packages/SabreTools.Serialization).

The following non-project libraries (or ports thereof) are used for file handling:

- [SharpCompress](https://github.com/adamhathcock/sharpcompress) - Common archive format extraction
- [StormLibSharp](https://github.com/robpaveza/stormlibsharp) - MoPaQ extraction [Unused in .NET Framework 2.0/3.5/4.0, non-Windows, and non-x86 builds due to Windows-specific libraries]

The following projects have influenced this library:

- [libmspack](https://github.com/kyz/libmspack) - Documentation around the MS-CAB format and associated compression methods.
- [Unshield](https://github.com/twogood/unshield/) - InstallShield CAB extraction tool that influenced internal handling

## Releases

For the most recent stable build, download the latest release here: [Releases Page](https://github.com/SabreTools/SabreTools.Serialization/releases)

For the latest WIP build here: [Rolling Release](https://github.com/SabreTools/SabreTools.Serialization/releases/tag/rolling)

## InfoPrint

**InfoPrint** is a reference implementation for the deserialization and printing features of the library, packaged as a standalone executable for all supported platforms. It will attempt to detect and display information about many supported file types, optionally both hashing the file and outputting the information to a JSON file (.NET Core 3.1 and above only).

```
InfoPrint <options> file|directory ...

Options:
-?, -h, --help           Display this help text and quit
-d, --debug              Enable debug mode
-c, --hash               Output file hashes
-j, --json               Print info as JSON
```

## ExtractionTool

**ExtractionTool** is a reference implementation for the extraction features of the library, packaged as a standalone executable for all supported platforms. It will attempt to detect and extract many supported file types. See the table below for supported extraction functionality.

```
ExtractionTool.exe <options> file|directory ...

Options:
-?, -h, --help           Display this help text and quit
-d, --debug              Enable debug mode
-o, --outdir [PATH]      Set output path for extraction (required)
```

| Format Name | Notes |
| --- | --- |
| 7-zip archive | .NET Framework 4.6.2 and greater |
| BFPK custom archive format | |
| bzip2 archive | .NET Framework 4.6.2 and greater |
| Compound File Binary (CFB) | Only CFB common pieces extractable. .NET Framework 4.0 and greater |
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
| MoPaQ game data archive (MPQ) | Currently not working. Windows only. .NET Framework 4.5.2 and above |
| New Exectuable | Embedded archives and executables in the overlay and Wise installer |
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
| `IByteDeserializer` | `byte[]?` | Model |
| `IByteSerializer` | Model | `byte[]?` |
| `IFileDeserializer` | `string?` path | Model |
| `IFileSerializer` | Model | `string?` path |
| `IModelSerializer` | Model | Model |
| `IStreamDeserializer` | `Stream?` | Model |
| `IStreamSerializer` | Model | `Stream?` |
| `IStringDeserializer` | `string?` representation | Model |
| `IStringSerializer` | Model | `string?` representation |

Below is a table representing the various non-conversion interfaces that are implemented within this library.

| Interface Name | Purpose |
| --- | --- |
| `IExtractable` | Marks a wrapper as able to be extracted |
| `IPrinter` | Provides a formatted output for a model |
| `IWrapper` / `IWrapper<T>` | Wraps a model or set of models to provide additional functionality |

## Namespaces

Below is a table of all namespaces within the library and what they represent

| Namespace | Description |
| --- | --- |
| `SabreTools.Serialization.CrossModel` | Convert between models; mainly used for metadata files converting to and from a common, `Dictionary`-based model |
| `SabreTools.Serialization.Deserializers` | Convert from external sources to models |
| `SabreTools.Serialization.Extensions` | Extension methods for both models and wrappers |
| `SabreTools.Serialization.Interfaces` | Interfaces used commonly throughout the library |
| `SabreTools.Serialization.Models` | Models representing different file and structure types |
| `SabreTools.Serialization.ObjectIdentifier` | Object Identifier (OID) parsing |
| `SabreTools.Serialization.Printers` | Export model information in a formatted manner |
| `SabreTools.Serialization.Serializers` | Convert from models to external sources |
| `SabreTools.Serialization.Wrappers` | Classes that wrap serialization and models to allow for including extension properties |
