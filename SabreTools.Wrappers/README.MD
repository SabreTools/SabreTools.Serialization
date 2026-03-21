# SabreTools.Wrappers

This library contains "wrapper" classes that combine both a model type and a reader with extension properties and methods. Models are defined in `SabreTools.Data.Models` and readers are defined in `SabreTools.Serialization.Readers`.

Common extension functionality includes:

- Exposing model fields
- Transformed model fields and generated properties
- Extension methods for reference or conversion
- Format information printing
- Content extraction

Wrapper classes can inherit from one or more interfaces, as seen in the table below:

| Interface Name | Purpose |
| --- | --- |
| `SabreTools.Wrappers.IExtractable` | Marks a wrapper as able to be extracted |
| `SabreTools.Wrappers.IPrintable` | Marks a wrapper as able to print model information |
| `SabreTools.Wrappers.IWrapper` | **REQUIRED** Represents an item with a description and JSON serializable state, allowing for extensions |
| `SabreTools.Wrappers.IWrapper<TModel>` | Wraps a `TModel` with source data, allowing for extensions |

## Additional Information

The following projects have influenced this library:

- [libmspack](https://github.com/kyz/libmspack) - Documentation around the MS-CAB format and associated compression methods.
- [Unshield](https://github.com/twogood/unshield/) - InstallShield CAB extraction tool that influenced internal handling

The following non-project libraries (or ports thereof) are used for file handling:

- [GrindCore.SharpCompress](https://github.com/Nanook/GrindCore.SharpCompress) - Common archive format extraction
- [StormLibSharp](https://github.com/robpaveza/stormlibsharp) - MoPaQ extraction [Unused in non-Windows builds due to Windows-specific libraries]
