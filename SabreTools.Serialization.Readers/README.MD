# SabreTools.Serialization.Readers

This library contains methods for reading in data from various input sources into models that are defined in `SabreTools.Data.Models`.

Reader classes can inherit from one or more interfaces, as seen in the table below:

| Interface Name | Source Type | Destination Type |
| --- | --- | --- |
| `SabreTools.Serialization.Readers.IByteReader<TModel>` | `byte[]?` | `TModel` |
| `SabreTools.Serialization.Readers.IFileReader<TModel>` | `string?` path | `TModel` |
| `SabreTools.Serialization.Readers.IStreamReader<TModel>` | `Stream?` | `TModel` |
| `SabreTools.Serialization.Readers.IStringReader<TModel>` | `string?` representation | `TModel` |
