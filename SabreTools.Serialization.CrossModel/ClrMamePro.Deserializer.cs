using System;
using SabreTools.Data.Models.ClrMamePro;

#pragma warning disable CA1822 // Mark members as static
namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
            => Deserialize(obj, true);

        /// <inheritdoc cref="Deserialize(Data.Models.Metadata.MetadataFile)"/>
        public MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj, bool game)
        {
            if (obj is null)
                return null;

            var metadataFile = new MetadataFile();

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            if (header is not null)
                metadataFile.ClrMamePro = ConvertHeaderFromInternalModel(header);

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines is not null && machines.Length > 0)
            {
                metadataFile.Game
                    = Array.ConvertAll(machines, m => ConvertMachineFromInternalModel(m, game));
            }

            var info = obj.Read<Data.Models.Metadata.InfoSource>(Data.Models.Metadata.MetadataFile.InfoSourceKey);
            if (info is not null)
                metadataFile.Info = ConvertInfoSourceFromInternalModel(info);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.ClrMamePro.ClrMamePro"/>
        /// </summary>
        private static Data.Models.ClrMamePro.ClrMamePro ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var clrMamePro = new Data.Models.ClrMamePro.ClrMamePro
            {
                Name = item.Name,
                Description = item.Description,
                RootDir = item.RootDir,
                Category = item.Category,
                Version = item.Version,
                Date = item.Date,
                Author = item.Author,
                Homepage = item.Homepage,
                Url = item.Url,
                Comment = item.Comment,
                Header = item.ReadString(Data.Models.Metadata.Header.HeaderKey),
                Type = item.Type,
                ForceMerging = item.ForceMerging,
                ForceZipping = item.ForceZipping,
                ForcePacking = item.ForcePacking,
            };
            return clrMamePro;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Archive"/> to <see cref="Machine"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item, bool game)
        {
            GameBase gameBase = game ? new Game() : new Machine();

            gameBase.Name = item.Name;
            gameBase.Description = item.Description;
            // gameBase.DriverStatus = item.ReadString(Data.Models.Metadata.Machine.DriverKey); // TODO: Needs metadata mapping
            gameBase.Year = item.ReadString(Data.Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Data.Models.Metadata.Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Data.Models.Metadata.Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Data.Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Data.Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Data.Models.Metadata.Machine.SampleOfKey);

            var releases = item.Read<Data.Models.Metadata.Release[]>(Data.Models.Metadata.Machine.ReleaseKey);
            if (releases is not null && releases.Length > 0)
                gameBase.Release = Array.ConvertAll(releases, ConvertFromInternalModel);

            var biosSets = item.Read<Data.Models.Metadata.BiosSet[]>(Data.Models.Metadata.Machine.BiosSetKey);
            if (biosSets is not null && biosSets.Length > 0)
                gameBase.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms is not null && roms.Length > 0)
                gameBase.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Read<Data.Models.Metadata.Disk[]>(Data.Models.Metadata.Machine.DiskKey);
            if (disks is not null && disks.Length > 0)
                gameBase.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var medias = item.Read<Data.Models.Metadata.Media[]>(Data.Models.Metadata.Machine.MediaKey);
            if (medias is not null && medias.Length > 0)
                gameBase.Media = Array.ConvertAll(medias, ConvertFromInternalModel);

            var samples = item.Read<Data.Models.Metadata.Sample[]>(Data.Models.Metadata.Machine.SampleKey);
            if (samples is not null && samples.Length > 0)
                gameBase.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var archives = item.Read<Data.Models.Metadata.Archive[]>(Data.Models.Metadata.Machine.ArchiveKey);
            if (archives is not null && archives.Length > 0)
                gameBase.Archive = Array.ConvertAll(archives, ConvertFromInternalModel);

            var chips = item.Read<Data.Models.Metadata.Chip[]>(Data.Models.Metadata.Machine.ChipKey);
            if (chips is not null && chips.Length > 0)
                gameBase.Chip = Array.ConvertAll(chips, ConvertFromInternalModel);

            var videos = item.Read<Data.Models.Metadata.Video[]>(Data.Models.Metadata.Machine.VideoKey);
            if (videos is not null && videos.Length > 0)
                gameBase.Video = Array.ConvertAll(videos, ConvertFromInternalModel);

            var sound = item.Read<Data.Models.Metadata.Sound>(Data.Models.Metadata.Machine.SoundKey);
            if (sound is not null)
                gameBase.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Data.Models.Metadata.Input>(Data.Models.Metadata.Machine.InputKey);
            if (input is not null)
                gameBase.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Data.Models.Metadata.DipSwitch[]>(Data.Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches is not null && dipSwitches.Length > 0)
                gameBase.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            var driver = item.Read<Data.Models.Metadata.Driver>(Data.Models.Metadata.Machine.DriverKey);
            if (driver is not null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            return gameBase;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Archive"/> to <see cref="Archive"/>
        /// </summary>
        private static Archive ConvertFromInternalModel(Data.Models.Metadata.Archive item)
        {
            var archive = new Archive
            {
                Name = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.BiosSet"/> to <see cref="BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Data.Models.Metadata.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.Name,
                Description = item.Description,
                Default = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Chip"/> to <see cref="Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Data.Models.Metadata.Chip item)
        {
            var chip = new Chip
            {
                Type = item.ChipType,
                Name = item.Name,
                Flags = item.Flags,
                Clock = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.DipSwitch"/> to <see cref="DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Data.Models.Metadata.DipSwitch item)
        {
            var dipswitch = new DipSwitch
            {
                Name = item.Name,
                Entry = item[Data.Models.Metadata.DipSwitch.EntryKey] as string[],
                Default = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Data.Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.Name,
                MD5 = item.ReadString(Data.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Data.Models.Metadata.Disk.MergeKey),
                Status = item.Status,
                Flags = item.ReadString(Data.Models.Metadata.Disk.FlagsKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Driver"/> to <see cref="Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Data.Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.Status,
                Color = item.Color,
                Sound = item.Sound,
                PaletteSize = item.PaletteSize,
                Blit = item.Blit,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Input"/> to <see cref="Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Data.Models.Metadata.Input item)
        {
            var input = new Input
            {
                Players = item.Players,
                Control = item.ReadString(Data.Models.Metadata.Input.ControlKey),
                Buttons = item.Buttons,
                Coins = item.Coins,
                Tilt = item.Tilt,
                Service = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Media"/> to <see cref="Media"/>
        /// </summary>
        private static Media ConvertFromInternalModel(Data.Models.Metadata.Media item)
        {
            var media = new Media
            {
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                SHA256 = item.SHA256,
                SpamSum = item.SpamSum,
            };
            return media;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Release"/> to <see cref="Release"/>
        /// </summary>
        private static Release ConvertFromInternalModel(Data.Models.Metadata.Release item)
        {
            var release = new Release
            {
                Name = item.Name,
                Region = item.Region,
                Language = item.Language,
                Date = item.Date,
                Default = item.Default,
            };
            return release;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.Name,
                Size = item.Size,
                CRC16 = item.ReadString(Data.Models.Metadata.Rom.CRC16Key),
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                CRC64 = item.ReadString(Data.Models.Metadata.Rom.CRC64Key),
                MD2 = item.ReadString(Data.Models.Metadata.Rom.MD2Key),
                MD4 = item.ReadString(Data.Models.Metadata.Rom.MD4Key),
                MD5 = item.ReadString(Data.Models.Metadata.Rom.MD5Key),
                RIPEMD128 = item.ReadString(Data.Models.Metadata.Rom.RIPEMD128Key),
                RIPEMD160 = item.ReadString(Data.Models.Metadata.Rom.RIPEMD160Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Data.Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Data.Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Data.Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Data.Models.Metadata.Rom.SpamSumKey),
                xxHash364 = item.ReadString(Data.Models.Metadata.Rom.xxHash364Key),
                xxHash3128 = item.ReadString(Data.Models.Metadata.Rom.xxHash3128Key),
                Merge = item.ReadString(Data.Models.Metadata.Rom.MergeKey),
                Status = item.Status,
                Region = item.ReadString(Data.Models.Metadata.Rom.RegionKey),
                Flags = item.ReadString(Data.Models.Metadata.Rom.FlagsKey),
                Offs = item.ReadString(Data.Models.Metadata.Rom.OffsetKey),
                Serial = item.ReadString(Data.Models.Metadata.Rom.SerialKey),
                Header = item.ReadString(Data.Models.Metadata.Rom.HeaderKey),
                Date = item.ReadString(Data.Models.Metadata.Rom.DateKey),
                Inverted = item.Inverted,
                MIA = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Sample"/> to <see cref="Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Data.Models.Metadata.Sample item)
        {
            var sample = new Sample
            {
                Name = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Sound"/> to <see cref="Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Data.Models.Metadata.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Video"/> to <see cref="Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Data.Models.Metadata.Video item)
        {
            var video = new Video
            {
                Screen = item.Screen,
                Orientation = item.Orientation,
                X = item.Width,
                Y = item.Height,
                AspectX = item.AspectX,
                AspectY = item.AspectY,
                Freq = item.Refresh,
            };
            return video;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.InfoSource"/> to <see cref="Models.ClrMamePro.Info"/>
        /// </summary>
        private static Info ConvertInfoSourceFromInternalModel(Data.Models.Metadata.InfoSource item)
        {
            var info = new Info();

            var sources = item.Read<string[]>(Data.Models.Metadata.InfoSource.SourceKey);
            if (sources is not null && sources.Length > 0)
                info.Source = [.. sources];

            return info;
        }
    }
}
