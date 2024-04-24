# SabreTools.Serialization

This library comprises of serializers that both read and write from files and streams to the dedicated models as well as convert to and from the common internal models. This library is partially used by the current parsing and writing code but none of the internal model serialization is used.

Find the link to the Nuget package [here](https://www.nuget.org/packages/SabreTools.Serialization).

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
| `IPrinter` | Provides a formatted output for a model |
| `IWrapper` / `IWrapper<T>` | Wraps a model or set of models to provide additional functionality |

## Namespaces

Below is a table of all namespaces within the library and what they represent

| Namespace | Description |
| --- | --- |
| `SabreTools.Serialization.CrossModel` | Convert between models; mainly used for metadata files converting to and from a common, `Dictionary`-based model |
| `SabreTools.Serialization.Deserializers` | Convert from external sources to models |
| `SabreTools.Serialization.Printers` | Export model information in a formatted manner |
| `SabreTools.Serialization.Serializers` | Convert from models to external sources |
| `SabreTools.Serialization.Wrappers` | Classes that wrap serialization and models to allow for including extension properties |
