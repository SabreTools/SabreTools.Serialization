using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.DosCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class DosCenter :
        BaseBinaryDeserializer<MetadataFile>,
        IStreamDeserializer<MetadataFile>
    {
        #region IStreamDeserializer

        /// <inheritdoc/>
        public MetadataFile? Deserialize(Stream? data)
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the reader and output
            var reader = new ClrMameProReader(data, Encoding.UTF8) { DosCenter = true };
            var dat = new MetadataFile();

            // Loop through and parse out the values
            string? lastTopLevel = reader.TopLevel;

            Game? game = null;
            var games = new List<Game>();
            var files = new List<Models.DosCenter.File>();

            var additional = new List<string>();
            var headerAdditional = new List<string>();
            var gameAdditional = new List<string>();
            while (!reader.EndOfStream)
            {
                // If we have no next line
                if (!reader.ReadNextLine())
                    break;

                // Ignore certain row types
                switch (reader.RowType)
                {
                    case CmpRowType.None:
                    case CmpRowType.Comment:
                        continue;
                    case CmpRowType.EndTopLevel:
                        switch (lastTopLevel)
                        {
                            case "doscenter":
                                if (dat.DosCenter != null)
                                    dat.DosCenter.ADDITIONAL_ELEMENTS = headerAdditional.ToArray();

                                headerAdditional.Clear();
                                break;
                            case "game":
                                if (game != null)
                                {
                                    game.File = files.ToArray();
                                    game.ADDITIONAL_ELEMENTS = gameAdditional.ToArray();
                                    games.Add(game);
                                }

                                game = null;
                                files.Clear();
                                gameAdditional.Clear();
                                break;
                            default:
                                // No-op
                                break;
                        }
                        continue;
                }

                // If we're at the root
                if (reader.RowType == CmpRowType.TopLevel)
                {
                    lastTopLevel = reader.TopLevel;
                    switch (reader.TopLevel)
                    {
                        case "doscenter":
                            dat.DosCenter = new Models.DosCenter.DosCenter();
                            break;
                        case "game":
                            game = new Game();
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                additional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in the doscenter block
                else if (reader.TopLevel == "doscenter" && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
                    dat.DosCenter ??= new Models.DosCenter.DosCenter();

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name:":
                            dat.DosCenter.Name = reader.Standalone?.Value;
                            break;
                        case "description:":
                            dat.DosCenter.Description = reader.Standalone?.Value;
                            break;
                        case "version:":
                            dat.DosCenter.Version = reader.Standalone?.Value;
                            break;
                        case "date:":
                            dat.DosCenter.Date = reader.Standalone?.Value;
                            break;
                        case "author:":
                            dat.DosCenter.Author = reader.Standalone?.Value;
                            break;
                        case "homepage:":
                            dat.DosCenter.Homepage = reader.Standalone?.Value;
                            break;
                        case "comment:":
                            dat.DosCenter.Comment = reader.Standalone?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                headerAdditional.Add(item: reader.CurrentLine);
                            break;
                    }
                }

                // If we're in a game block
                else if (reader.TopLevel == "game" && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
                    game ??= new Game();

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name":
                            game.Name = reader.Standalone?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                gameAdditional.Add(item: reader.CurrentLine);
                            break;
                    }
                }

                // If we're in a file block
                else if (reader.TopLevel == "game" && reader.RowType == CmpRowType.Internal)
                {
                    // If we have an unknown type, log it
                    if (reader.InternalName != "file")
                    {
                        if (reader.CurrentLine != null)
                            gameAdditional.Add(reader.CurrentLine);
                        continue;
                    }

                    // Create the file and add to the list
                    var file = CreateFile(reader);
                    if (file != null)
                        files.Add(file);
                }

                else
                {
                    if (reader.CurrentLine != null)
                        additional.Add(item: reader.CurrentLine);
                }
            }

            // Add extra pieces and return
            dat.Game = games.ToArray();
            dat.ADDITIONAL_ELEMENTS = additional.ToArray();
            return dat;
        }

        /// <summary>
        /// Create a File object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>File object created from the reader context</returns>
        private static Models.DosCenter.File? CreateFile(ClrMameProReader reader)
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var file = new Models.DosCenter.File();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        file.Name = kvp.Value;
                        break;
                    case "size":
                        file.Size = kvp.Value;
                        break;
                    case "crc":
                        file.CRC = kvp.Value;
                        break;
                    case "date":
                        file.Date = kvp.Value;
                        break;
                    default:
                        if (reader.CurrentLine != null)
                            itemAdditional.Add(item: reader.CurrentLine);
                        break;
                }
            }

            file.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return file;
        }

        #endregion
    }
}