# SabreTools.Serialization.CrossModel

This library contains methods for converting between different model formats. The majority of the implementations convert from specific Datfile/metadata file formats to the unified Metadata internal format.

All cross-model converters implement `SabreTools.Serialization.CrossModel.ICrossModel<TSource, TDest>` which enforces bi-directional conversion.
