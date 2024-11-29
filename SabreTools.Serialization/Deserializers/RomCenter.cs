using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.RomCenter;

namespace SabreTools.Serialization.Deserializers
{
    public class RomCenter : BaseBinaryDeserializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new IniReader(data, Encoding.UTF8)
                {
                    ValidateRows = false,
                };
                var dat = new MetadataFile();

                // Loop through and parse out the values
                var roms = new List<Rom>();
                while (!reader.EndOfStream)
                {
                    // If we have no next line
                    if (!reader.ReadNextLine())
                        break;

                    // Ignore certain row types
                    switch (reader.RowType)
                    {
                        case IniRowType.None:
                        case IniRowType.Comment:
                            continue;
                        case IniRowType.SectionHeader:
                            switch (reader.Section?.ToLowerInvariant())
                            {
                                case "credits":
                                    dat.Credits ??= new Credits();
                                    break;
                                case "dat":
                                    dat.Dat ??= new Dat();
                                    break;
                                case "emulator":
                                    dat.Emulator ??= new Emulator();
                                    break;
                                case "games":
                                    dat.Games ??= new Games();
                                    break;
                            }
                            continue;
                    }

                    // If we're in credits
                    if (reader.Section?.ToLowerInvariant() == "credits")
                    {
                        // Create the section if we haven't already
                        dat.Credits ??= new Credits();

                        switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                        {
                            case "author":
                                dat.Credits.Author = reader.KeyValuePair?.Value;
                                break;
                            case "version":
                                dat.Credits.Version = reader.KeyValuePair?.Value;
                                break;
                            case "email":
                                dat.Credits.Email = reader.KeyValuePair?.Value;
                                break;
                            case "homepage":
                                dat.Credits.Homepage = reader.KeyValuePair?.Value;
                                break;
                            case "url":
                                dat.Credits.Url = reader.KeyValuePair?.Value;
                                break;
                            case "date":
                                dat.Credits.Date = reader.KeyValuePair?.Value;
                                break;
                            case "comment":
                                dat.Credits.Comment = reader.KeyValuePair?.Value;
                                break;
                        }
                    }

                    // If we're in dat
                    else if (reader.Section?.ToLowerInvariant() == "dat")
                    {
                        // Create the section if we haven't already
                        dat.Dat ??= new Dat();

                        switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                        {
                            case "version":
                                dat.Dat.Version = reader.KeyValuePair?.Value;
                                break;
                            case "plugin":
                                dat.Dat.Plugin = reader.KeyValuePair?.Value;
                                break;
                            case "split":
                                dat.Dat.Split = reader.KeyValuePair?.Value;
                                break;
                            case "merge":
                                dat.Dat.Merge = reader.KeyValuePair?.Value;
                                break;
                        }
                    }

                    // If we're in emulator
                    else if (reader.Section?.ToLowerInvariant() == "emulator")
                    {
                        // Create the section if we haven't already
                        dat.Emulator ??= new Emulator();

                        switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                        {
                            case "refname":
                                dat.Emulator.RefName = reader.KeyValuePair?.Value;
                                break;
                            case "version":
                                dat.Emulator.Version = reader.KeyValuePair?.Value;
                                break;
                        }
                    }

                    // If we're in games
                    else if (reader.Section?.ToLowerInvariant() == "games")
                    {
                        // Create the section if we haven't already
                        dat.Games ??= new Games();

                        // If the line doesn't contain the delimiter
#if NETFRAMEWORK
                    if (!(reader.CurrentLine?.Contains("¬") ?? false))
#else
                        if (!(reader.CurrentLine?.Contains('¬') ?? false))
#endif
                            continue;

                        // Otherwise, separate out the line
                        string[] splitLine = reader.CurrentLine.Split('¬');
                        var rom = new Rom
                        {
                            // EMPTY = splitLine[0]
                            ParentName = splitLine[1],
                            ParentDescription = splitLine[2],
                            GameName = splitLine[3],
                            GameDescription = splitLine[4],
                            RomName = splitLine[5],
                            RomCRC = splitLine[6],
                            RomSize = splitLine[7],
                            RomOf = splitLine[8],
                            MergeName = splitLine[9],
                            // EMPTY = splitLine[10]
                        };

                        roms.Add(rom);
                    }
                }

                // Add extra pieces and return
                if (dat.Games != null && roms.Count > 0)
                {
                    dat.Games.Rom = [.. roms];
                    return dat;
                }

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}