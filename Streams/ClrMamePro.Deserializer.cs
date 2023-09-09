using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.ClrMamePro;

namespace SabreTools.Serialization.Streams
{
    public partial class ClrMamePro : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(Stream data) => Deserialize(data, true);
#else
        public MetadataFile? Deserialize(Stream? data) => Deserialize(data, true);
#endif

        /// <inheritdoc cref="Deserialize(Stream)"/>
#if NET48
        public MetadataFile Deserialize(Stream data, bool quotes)
#else
        public MetadataFile? Deserialize(Stream? data, bool quotes)
#endif
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the reader and output
            var reader = new ClrMameProReader(data, Encoding.UTF8) { Quotes = quotes };
            var dat = new MetadataFile();

            // Loop through and parse out the values
#if NET48
            string lastTopLevel = reader.TopLevel;
#else
            string? lastTopLevel = reader.TopLevel;
#endif

#if NET48
            GameBase game = null;
#else
            GameBase? game = null;
#endif
            var games = new List<GameBase>();
            var releases = new List<Release>();
            var biosSets = new List<BiosSet>();
            var roms = new List<Rom>();
            var disks = new List<Disk>();
            var medias = new List<Media>();
            var samples = new List<Sample>();
            var archives = new List<Archive>();
            var chips = new List<Chip>();
            var videos = new List<Video>();
            var dipSwitches = new List<DipSwitch>();

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
                                if (dat.ClrMamePro != null)
                                    dat.ClrMamePro.ADDITIONAL_ELEMENTS = headerAdditional.ToArray();

                                headerAdditional.Clear();
                                break;
                            case "game":
                            case "machine":
                            case "resource":
                            case "set":
                                if (game != null)
                                {
                                    game.Release = releases.ToArray();
                                    game.BiosSet = biosSets.ToArray();
                                    game.Rom = roms.ToArray();
                                    game.Disk = disks.ToArray();
                                    game.Media = medias.ToArray();
                                    game.Sample = samples.ToArray();
                                    game.Archive = archives.ToArray();
                                    game.Chip = chips.ToArray();
                                    game.Video = videos.ToArray();
                                    game.DipSwitch = dipSwitches.ToArray();
                                    game.ADDITIONAL_ELEMENTS = gameAdditional.ToArray();

                                    games.Add(game);
                                    game = null;
                                }

                                releases.Clear();
                                biosSets.Clear();
                                roms.Clear();
                                disks.Clear();
                                medias.Clear();
                                samples.Clear();
                                archives.Clear();
                                chips.Clear();
                                videos.Clear();
                                dipSwitches.Clear();
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
                        case "clrmamepro":
                            dat.ClrMamePro = new Models.ClrMamePro.ClrMamePro();
                            break;
                        case "game":
                            game = new Game();
                            break;
                        case "machine":
                            game = new Machine();
                            break;
                        case "resource":
                            game = new Resource();
                            break;
                        case "set":
                            game = new Set();
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                additional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in the doscenter block
                else if (reader.TopLevel == "clrmamepro"
                    && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
#if NET48
                    dat.ClrMamePro = dat.ClrMamePro != null ? dat.ClrMamePro : new Models.ClrMamePro.ClrMamePro();
#else
                    dat.ClrMamePro ??= new Models.ClrMamePro.ClrMamePro();
#endif

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name":
                            dat.ClrMamePro.Name = reader.Standalone?.Value;
                            break;
                        case "description":
                            dat.ClrMamePro.Description = reader.Standalone?.Value;
                            break;
                        case "rootdir":
                            dat.ClrMamePro.RootDir = reader.Standalone?.Value;
                            break;
                        case "category":
                            dat.ClrMamePro.Category = reader.Standalone?.Value;
                            break;
                        case "version":
                            dat.ClrMamePro.Version = reader.Standalone?.Value;
                            break;
                        case "date":
                            dat.ClrMamePro.Date = reader.Standalone?.Value;
                            break;
                        case "author":
                            dat.ClrMamePro.Author = reader.Standalone?.Value;
                            break;
                        case "homepage":
                            dat.ClrMamePro.Homepage = reader.Standalone?.Value;
                            break;
                        case "url":
                            dat.ClrMamePro.Url = reader.Standalone?.Value;
                            break;
                        case "comment":
                            dat.ClrMamePro.Comment = reader.Standalone?.Value;
                            break;
                        case "header":
                            dat.ClrMamePro.Header = reader.Standalone?.Value;
                            break;
                        case "type":
                            dat.ClrMamePro.Type = reader.Standalone?.Value;
                            break;
                        case "forcemerging":
                            dat.ClrMamePro.ForceMerging = reader.Standalone?.Value;
                            break;
                        case "forcezipping":
                            dat.ClrMamePro.ForceZipping = reader.Standalone?.Value;
                            break;
                        case "forcepacking":
                            dat.ClrMamePro.ForcePacking = reader.Standalone?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                headerAdditional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in a game, machine, resource, or set block
                else if ((reader.TopLevel == "game"
                        || reader.TopLevel == "machine"
                        || reader.TopLevel == "resource"
                        || reader.TopLevel == "set")
                    && reader.RowType == CmpRowType.Standalone)
                {
                    // Create the block if we haven't already
#if NET48
                    if (game == null)
                    {
                        switch (reader.TopLevel)
                        {
                            case "game":
                                game = new Game();
                                break;
                            case "machine":
                                game = new Machine();
                                break;
                            case "resource":
                                game = new Resource();
                                break;
                            case "set":
                                game = new Set();
                                break;
                            default:
                                throw new FormatException($"Unknown top-level block: {reader.TopLevel}");
                        }
                    }
#else
                    game ??= reader.TopLevel switch
                    {
                        "game" => new Game(),
                        "machine" => new Machine(),
                        "resource" => new Resource(),
                        "set" => new Set(),
                        _ => throw new FormatException($"Unknown top-level block: {reader.TopLevel}"),
                    };
#endif

                    switch (reader.Standalone?.Key?.ToLowerInvariant())
                    {
                        case "name":
                            game.Name = reader.Standalone?.Value;
                            break;
                        case "description":
                            game.Description = reader.Standalone?.Value;
                            break;
                        case "year":
                            game.Year = reader.Standalone?.Value;
                            break;
                        case "manufacturer":
                            game.Manufacturer = reader.Standalone?.Value;
                            break;
                        case "category":
                            game.Category = reader.Standalone?.Value;
                            break;
                        case "cloneof":
                            game.CloneOf = reader.Standalone?.Value;
                            break;
                        case "romof":
                            game.RomOf = reader.Standalone?.Value;
                            break;
                        case "sampleof":
                            game.SampleOf = reader.Standalone?.Value;
                            break;
                        case "sample":
                            var sample = new Sample
                            {
                                Name = reader.Standalone?.Value ?? string.Empty,
                                ADDITIONAL_ELEMENTS = Array.Empty<string>()
                            };
                            samples.Add(sample);
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                gameAdditional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in an item block
                else if ((reader.TopLevel == "game"
                        || reader.TopLevel == "machine"
                        || reader.TopLevel == "resource"
                        || reader.TopLevel == "set")
                    && game != null
                    && reader.RowType == CmpRowType.Internal)
                {
                    // Create the block
                    switch (reader.InternalName)
                    {
                        case "release":
                            var release = CreateRelease(reader);
                            if (release != null)
                                releases.Add(release);
                            break;
                        case "biosset":
                            var biosSet = CreateBiosSet(reader);
                            if (biosSet != null)
                                biosSets.Add(biosSet);
                            break;
                        case "rom":
                            var rom = CreateRom(reader);
                            if (rom != null)
                                roms.Add(rom);
                            break;
                        case "disk":
                            var disk = CreateDisk(reader);
                            if (disk != null)
                                disks.Add(disk);
                            break;
                        case "media":
                            var media = CreateMedia(reader);
                            if (media != null)
                                medias.Add(media);
                            break;
                        case "sample":
                            var sample = CreateSample(reader);
                            if (sample != null)
                                samples.Add(sample);
                            break;
                        case "archive":
                            var archive = CreateArchive(reader);
                            if (archive != null)
                                archives.Add(archive);
                            break;
                        case "chip":
                            var chip = CreateChip(reader);
                            if (chip != null)
                                chips.Add(chip);
                            break;
                        case "video":
                            var video = CreateVideo(reader);
                            if (video != null)
                                videos.Add(video);
                            break;
                        case "sound":
                            var sound = CreateSound(reader);
                            if (sound != null)
                                game.Sound = sound;
                            break;
                        case "input":
                            var input = CreateInput(reader);
                            if (input != null)
                                game.Input = input;
                            break;
                        case "dipswitch":
                            var dipSwitch = CreateDipSwitch(reader);
                            if (dipSwitch != null)
                                dipSwitches.Add(dipSwitch);
                            break;
                        case "driver":
                            var driver = CreateDriver(reader);
                            if (driver != null)
                                game.Driver = driver;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                gameAdditional.Add(reader.CurrentLine);
                            continue;
                    }
                }

                else
                {
                    if (reader.CurrentLine != null)
                        additional.Add(reader.CurrentLine);
                }
            }

            // Add extra pieces and return
            dat.Game = games.ToArray();
            dat.ADDITIONAL_ELEMENTS = additional.ToArray();
            return dat;
        }

        /// <summary>
        /// Create a Release object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Release object created from the reader context</returns>
#if NET48
        private static Release CreateRelease(ClrMameProReader reader)
#else
        private static Release? CreateRelease(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var release = new Release();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        release.Name = kvp.Value;
                        break;
                    case "region":
                        release.Region = kvp.Value;
                        break;
                    case "language":
                        release.Language = kvp.Value;
                        break;
                    case "date":
                        release.Date = kvp.Value;
                        break;
                    case "default":
                        release.Default = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            release.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return release;
        }

        /// <summary>
        /// Create a BiosSet object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>BiosSet object created from the reader context</returns>
#if NET48
        private static BiosSet CreateBiosSet(ClrMameProReader reader)
#else
        private static BiosSet? CreateBiosSet(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var biosset = new BiosSet();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        biosset.Name = kvp.Value;
                        break;
                    case "description":
                        biosset.Description = kvp.Value;
                        break;
                    case "default":
                        biosset.Default = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            biosset.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return biosset;
        }

        /// <summary>
        /// Create a Rom object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Rom object created from the reader context</returns>
#if NET48
        private static Rom CreateRom(ClrMameProReader reader)
#else
        private static Rom? CreateRom(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var rom = new Rom();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        rom.Name = kvp.Value;
                        break;
                    case "size":
                        rom.Size = kvp.Value;
                        break;
                    case "crc":
                        rom.CRC = kvp.Value;
                        break;
                    case "md5":
                        rom.MD5 = kvp.Value;
                        break;
                    case "sha1":
                        rom.SHA1 = kvp.Value;
                        break;
                    case "sha256":
                        rom.SHA256 = kvp.Value;
                        break;
                    case "sha384":
                        rom.SHA384 = kvp.Value;
                        break;
                    case "sha512":
                        rom.SHA512 = kvp.Value;
                        break;
                    case "spamsum":
                        rom.SpamSum = kvp.Value;
                        break;
                    case "xxh3_64":
                        rom.xxHash364 = kvp.Value;
                        break;
                    case "xxh3_128":
                        rom.xxHash3128 = kvp.Value;
                        break;
                    case "merge":
                        rom.Merge = kvp.Value;
                        break;
                    case "status":
                        rom.Status = kvp.Value;
                        break;
                    case "region":
                        rom.Region = kvp.Value;
                        break;
                    case "flags":
                        rom.Flags = kvp.Value;
                        break;
                    case "offs":
                        rom.Offs = kvp.Value;
                        break;
                    case "serial":
                        rom.Serial = kvp.Value;
                        break;
                    case "header":
                        rom.Header = kvp.Value;
                        break;
                    case "date":
                        rom.Date = kvp.Value;
                        break;
                    case "inverted":
                        rom.Inverted = kvp.Value;
                        break;
                    case "mia":
                        rom.MIA = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            rom.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return rom;
        }

        /// <summary>
        /// Create a Disk object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Disk object created from the reader context</returns>
#if NET48
        private static Disk CreateDisk(ClrMameProReader reader)
#else
        private static Disk? CreateDisk(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var disk = new Disk();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        disk.Name = kvp.Value;
                        break;
                    case "md5":
                        disk.MD5 = kvp.Value;
                        break;
                    case "sha1":
                        disk.SHA1 = kvp.Value;
                        break;
                    case "merge":
                        disk.Merge = kvp.Value;
                        break;
                    case "status":
                        disk.Status = kvp.Value;
                        break;
                    case "flags":
                        disk.Flags = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            disk.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return disk;
        }

        /// <summary>
        /// Create a Media object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Media object created from the reader context</returns>
#if NET48
        private static Media CreateMedia(ClrMameProReader reader)
#else
        private static Media? CreateMedia(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var media = new Media();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        media.Name = kvp.Value;
                        break;
                    case "md5":
                        media.MD5 = kvp.Value;
                        break;
                    case "sha1":
                        media.SHA1 = kvp.Value;
                        break;
                    case "sha256":
                        media.SHA256 = kvp.Value;
                        break;
                    case "spamsum":
                        media.SpamSum = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            media.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return media;
        }

        /// <summary>
        /// Create a Sample object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Sample object created from the reader context</returns>
#if NET48
        private static Sample CreateSample(ClrMameProReader reader)
#else
        private static Sample? CreateSample(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var sample = new Sample();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        sample.Name = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            sample.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return sample;
        }

        /// <summary>
        /// Create a Archive object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Archive object created from the reader context</returns>
#if NET48
        private static Archive CreateArchive(ClrMameProReader reader)
#else
        private static Archive? CreateArchive(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var archive = new Archive();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        archive.Name = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            archive.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return archive;
        }

        /// <summary>
        /// Create a Chip object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Chip object created from the reader context</returns>
#if NET48
        private static Chip CreateChip(ClrMameProReader reader)
#else
        private static Chip? CreateChip(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var chip = new Chip();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "type":
                        chip.Type = kvp.Value;
                        break;
                    case "name":
                        chip.Name = kvp.Value;
                        break;
                    case "flags":
                        chip.Flags = kvp.Value;
                        break;
                    case "clock":
                        chip.Clock = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            chip.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return chip;
        }

        /// <summary>
        /// Create a Video object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Video object created from the reader context</returns>
#if NET48
        private static Video CreateVideo(ClrMameProReader reader)
#else
        private static Video? CreateVideo(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var video = new Video();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "screen":
                        video.Screen = kvp.Value;
                        break;
                    case "orientation":
                        video.Orientation = kvp.Value;
                        break;
                    case "x":
                        video.X = kvp.Value;
                        break;
                    case "y":
                        video.Y = kvp.Value;
                        break;
                    case "aspectx":
                        video.AspectX = kvp.Value;
                        break;
                    case "aspecty":
                        video.AspectY = kvp.Value;
                        break;
                    case "freq":
                        video.Freq = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            video.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return video;
        }

        /// <summary>
        /// Create a Sound object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Sound object created from the reader context</returns>
#if NET48
        private static Sound CreateSound(ClrMameProReader reader)
#else
        private static Sound? CreateSound(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var sound = new Sound();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "channels":
                        sound.Channels = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            sound.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return sound;
        }

        /// <summary>
        /// Create a Input object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Input object created from the reader context</returns>
#if NET48
        private static Input CreateInput(ClrMameProReader reader)
#else
        private static Input? CreateInput(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var input = new Input();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "players":
                        input.Players = kvp.Value;
                        break;
                    case "control":
                        input.Control = kvp.Value;
                        break;
                    case "buttons":
                        input.Buttons = kvp.Value;
                        break;
                    case "coins":
                        input.Coins = kvp.Value;
                        break;
                    case "tilt":
                        input.Tilt = kvp.Value;
                        break;
                    case "service":
                        input.Service = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            input.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return input;
        }

        /// <summary>
        /// Create a DipSwitch object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>DipSwitch object created from the reader context</returns>
#if NET48
        private static DipSwitch CreateDipSwitch(ClrMameProReader reader)
#else
        private static DipSwitch? CreateDipSwitch(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var dipswitch = new DipSwitch();
            var entries = new List<string>();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "name":
                        dipswitch.Name = kvp.Value;
                        break;
                    case "entry":
                        entries.Add(kvp.Value);
                        break;
                    case "default":
                        dipswitch.Default = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            dipswitch.Entry = entries.ToArray();
            dipswitch.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return dipswitch;
        }

        /// <summary>
        /// Create a Driver object from the current reader context
        /// </summary>
        /// <param name="reader">ClrMameProReader representing the metadata file</param>
        /// <returns>Driver object created from the reader context</returns>
#if NET48
        private static Driver CreateDriver(ClrMameProReader reader)
#else
        private static Driver? CreateDriver(ClrMameProReader reader)
#endif
        {
            if (reader.Internal == null)
                return null;

            var itemAdditional = new List<string>();
            var driver = new Driver();
            foreach (var kvp in reader.Internal)
            {
                switch (kvp.Key?.ToLowerInvariant())
                {
                    case "status":
                        driver.Status = kvp.Value;
                        break;
                    case "color":
                        driver.Color = kvp.Value;
                        break;
                    case "sound":
                        driver.Sound = kvp.Value;
                        break;
                    case "palettesize":
                        driver.PaletteSize = kvp.Value;
                        break;
                    case "blit":
                        driver.Blit = kvp.Value;
                        break;
                    default:
                        itemAdditional.Add($"{kvp.Key}: {kvp.Value}");
                        break;
                }
            }

            driver.ADDITIONAL_ELEMENTS = itemAdditional.ToArray();
            return driver;
        }
    }
}