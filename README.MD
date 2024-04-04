# SabreTools.Serialization

This library comprises of serializers that both read and write from files and streams to the dedicated models as well as convert to and from the common internal models. This library is partially used by the current parsing and writing code but none of the internal model serialization is used.

Find the link to the Nuget package [here](https://www.nuget.org/packages/SabreTools.Serialization).

| Interface Name | Source Type | Destination Type |
| --- | --- | --- |
| `IByteDeserializer` | `byte[]?` | Model |
| `IFileDeserializer` | `string?` Path | Model |
| `IFileSerializer` | Model | `string?` Path |
| `IStreamDeserializer` | `Stream?` | Model |
| `IStreamSerializer` | Model | `Stream?` |

## `SabreTools.Serialization.CrossModel`

This namespace comprises of serializers and deserializers that convert models to other common ones. This is mainly used for metadata files converting to and from a common, `Dictionary`-based model.

## `SabreTools.Serialization.Strings`

This namespace comprises of serializers and deserializers that can convert to and from strings. Most of the serializers are symmetric, but this is not guaranteed. Unimplemented methods will throw `NotImplementedException`.

## `SabreTools.Serialization.Wrappers`

This namespace comrpises of wrapping classes that include keeping a reference to the source of each serializable model. Some of the wrappers may also include what are referred to as "extension properties", which are generated properties derived from either parts of the model or the underlying source.
