# SabreTools.Serialization

This library comprises of serializers that both read and write from files and streams to the dedicated models as well as convert to and from the common internal models. This library is partially used by the current parsing and writing code but none of the internal model serialization is used.

Find the link to the Nuget package [here](https://www.nuget.org/packages/SabreTools.Serialization).

## SabreTools.Serialization.Wrappers

This is a special set of wrapping classes that include keeping a reference to the source of each serializable model. Some of the wrappers may also include what are referred to as "extension properties", which are generated properties derived from either parts of the model or the underlying source.
