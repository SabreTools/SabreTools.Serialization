# SabreTools.Serialization.Writers

This library contains methods for writing out data from models that are defined in `SabreTools.Data.Models` into various output sources into.

Writer classes can inherit from one or more interfaces, as seen in the table below:

| Interface Name | Source Type | Destination Type |
| --- | --- | --- |
| `SabreTools.Serialization.Writers.IByteWriter<TModel>` | `TModel` | `byte[]?` |
| `SabreTools.Serialization.Writers.IFileWriter<TModel>` | `TModel` | `string?` path |
| `SabreTools.Serialization.Writers.IStreamWriter<TModel>` | `TModel` | `Stream?` |
| `SabreTools.Serialization.Writers.IStringWriter<TModel>` | `TModel` | `string?` representation |
