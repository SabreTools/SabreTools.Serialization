using System;
using SabreTools.Data.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : IModelSerializer<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj) => Deserialize(obj, true);

        /// <inheritdoc cref="Deserialize(Data.Models.Metadata.MetadataFile)"/>
        public MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj, bool game)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.ClrMamePro = ConvertHeaderFromInternalModel(header);

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
            {
                metadataFile.Game
                    = Array.ConvertAll(machines, m => ConvertMachineFromInternalModel(m, game));
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.ClrMamePro.ClrMamePro"/>
        /// </summary>
        private static Data.Models.ClrMamePro.ClrMamePro ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var clrMamePro = new Data.Models.ClrMamePro.ClrMamePro
            {
                Name = item.ReadString(Data.Models.Metadata.Header.NameKey),
                Description = item.ReadString(Data.Models.Metadata.Header.DescriptionKey),
                RootDir = item.ReadString(Data.Models.Metadata.Header.RootDirKey),
                Category = item.ReadString(Data.Models.Metadata.Header.CategoryKey),
                Version = item.ReadString(Data.Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Data.Models.Metadata.Header.DateKey),
                Author = item.ReadString(Data.Models.Metadata.Header.AuthorKey),
                Homepage = item.ReadString(Data.Models.Metadata.Header.HomepageKey),
                Url = item.ReadString(Data.Models.Metadata.Header.UrlKey),
                Comment = item.ReadString(Data.Models.Metadata.Header.CommentKey),
                Header = item.ReadString(Data.Models.Metadata.Header.HeaderKey),
                Type = item.ReadString(Data.Models.Metadata.Header.TypeKey),
                ForceMerging = item.ReadString(Data.Models.Metadata.Header.ForceMergingKey),
                ForceZipping = item.ReadString(Data.Models.Metadata.Header.ForceZippingKey),
                ForcePacking = item.ReadString(Data.Models.Metadata.Header.ForcePackingKey),
            };
            return clrMamePro;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Archive"/> to <see cref="Machine"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item, bool game)
        {
            GameBase gameBase = game ? new Game() : new Machine();

            gameBase.Name = item.ReadString(Data.Models.Metadata.Machine.NameKey);
            gameBase.Description = item.ReadString(Data.Models.Metadata.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Data.Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Data.Models.Metadata.Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Data.Models.Metadata.Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Data.Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Data.Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Data.Models.Metadata.Machine.SampleOfKey);

            var releases = item.Read<Data.Models.Metadata.Release[]>(Data.Models.Metadata.Machine.ReleaseKey);
            if (releases != null && releases.Length > 0)
                gameBase.Release = Array.ConvertAll(releases, ConvertFromInternalModel);

            var biosSets = item.Read<Data.Models.Metadata.BiosSet[]>(Data.Models.Metadata.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Length > 0)
                gameBase.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
                gameBase.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Read<Data.Models.Metadata.Disk[]>(Data.Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Length > 0)
                gameBase.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var medias = item.Read<Data.Models.Metadata.Media[]>(Data.Models.Metadata.Machine.MediaKey);
            if (medias != null && medias.Length > 0)
                gameBase.Media = Array.ConvertAll(medias, ConvertFromInternalModel);

            var samples = item.Read<Data.Models.Metadata.Sample[]>(Data.Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Length > 0)
                gameBase.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var archives = item.Read<Data.Models.Metadata.Archive[]>(Data.Models.Metadata.Machine.ArchiveKey);
            if (archives != null && archives.Length > 0)
                gameBase.Archive = Array.ConvertAll(archives, ConvertFromInternalModel);

            var chips = item.Read<Data.Models.Metadata.Chip[]>(Data.Models.Metadata.Machine.ChipKey);
            if (chips != null && chips.Length > 0)
                gameBase.Chip = Array.ConvertAll(chips, ConvertFromInternalModel);

            var videos = item.Read<Data.Models.Metadata.Video[]>(Data.Models.Metadata.Machine.VideoKey);
            if (videos != null && videos.Length > 0)
                gameBase.Video = Array.ConvertAll(videos, ConvertFromInternalModel);

            var sound = item.Read<Data.Models.Metadata.Sound>(Data.Models.Metadata.Machine.SoundKey);
            if (sound != null)
                gameBase.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Data.Models.Metadata.Input>(Data.Models.Metadata.Machine.InputKey);
            if (input != null)
                gameBase.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Data.Models.Metadata.DipSwitch[]>(Data.Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Length > 0)
                gameBase.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            var driver = item.Read<Data.Models.Metadata.Driver>(Data.Models.Metadata.Machine.DriverKey);
            if (driver != null)
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
                Name = item.ReadString(Data.Models.Metadata.Archive.NameKey),
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
                Name = item.ReadString(Data.Models.Metadata.BiosSet.NameKey),
                Description = item.ReadString(Data.Models.Metadata.BiosSet.DescriptionKey),
                Default = item.ReadString(Data.Models.Metadata.BiosSet.DefaultKey),
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
                Type = item.ReadString(Data.Models.Metadata.Chip.ChipTypeKey),
                Name = item.ReadString(Data.Models.Metadata.Chip.NameKey),
                Flags = item.ReadString(Data.Models.Metadata.Chip.FlagsKey),
                Clock = item.ReadString(Data.Models.Metadata.Chip.ClockKey),
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
                Name = item.ReadString(Data.Models.Metadata.DipSwitch.NameKey),
                Entry = item[Data.Models.Metadata.DipSwitch.EntryKey] as string[],
                Default = item.ReadString(Data.Models.Metadata.DipSwitch.DefaultKey),
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
                Name = item.ReadString(Data.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Data.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Data.Models.Metadata.Disk.MergeKey),
                Status = item.ReadString(Data.Models.Metadata.Disk.StatusKey),
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
                Status = item.ReadString(Data.Models.Metadata.Driver.StatusKey),
                Color = item.ReadString(Data.Models.Metadata.Driver.ColorKey),
                Sound = item.ReadString(Data.Models.Metadata.Driver.SoundKey),
                PaletteSize = item.ReadString(Data.Models.Metadata.Driver.PaletteSizeKey),
                Blit = item.ReadString(Data.Models.Metadata.Driver.BlitKey),
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
                Players = item.ReadString(Data.Models.Metadata.Input.PlayersKey),
                Control = item.ReadString(Data.Models.Metadata.Input.ControlKey),
                Buttons = item.ReadString(Data.Models.Metadata.Input.ButtonsKey),
                Coins = item.ReadString(Data.Models.Metadata.Input.CoinsKey),
                Tilt = item.ReadString(Data.Models.Metadata.Input.TiltKey),
                Service = item.ReadString(Data.Models.Metadata.Input.ServiceKey),
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
                Name = item.ReadString(Data.Models.Metadata.Media.NameKey),
                MD5 = item.ReadString(Data.Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Data.Models.Metadata.Media.SHA256Key),
                SpamSum = item.ReadString(Data.Models.Metadata.Media.SpamSumKey),
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
                Name = item.ReadString(Data.Models.Metadata.Release.NameKey),
                Region = item.ReadString(Data.Models.Metadata.Release.RegionKey),
                Language = item.ReadString(Data.Models.Metadata.Release.LanguageKey),
                Date = item.ReadString(Data.Models.Metadata.Release.DateKey),
                Default = item.ReadString(Data.Models.Metadata.Release.DefaultKey),
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
                Name = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
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
                Status = item.ReadString(Data.Models.Metadata.Rom.StatusKey),
                Region = item.ReadString(Data.Models.Metadata.Rom.RegionKey),
                Flags = item.ReadString(Data.Models.Metadata.Rom.FlagsKey),
                Offs = item.ReadString(Data.Models.Metadata.Rom.OffsetKey),
                Serial = item.ReadString(Data.Models.Metadata.Rom.SerialKey),
                Header = item.ReadString(Data.Models.Metadata.Rom.HeaderKey),
                Date = item.ReadString(Data.Models.Metadata.Rom.DateKey),
                Inverted = item.ReadString(Data.Models.Metadata.Rom.InvertedKey),
                MIA = item.ReadString(Data.Models.Metadata.Rom.MIAKey),
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
                Name = item.ReadString(Data.Models.Metadata.Sample.NameKey),
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
                Channels = item.ReadString(Data.Models.Metadata.Sound.ChannelsKey),
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
                Screen = item.ReadString(Data.Models.Metadata.Video.ScreenKey),
                Orientation = item.ReadString(Data.Models.Metadata.Video.OrientationKey),
                X = item.ReadString(Data.Models.Metadata.Video.WidthKey),
                Y = item.ReadString(Data.Models.Metadata.Video.HeightKey),
                AspectX = item.ReadString(Data.Models.Metadata.Video.AspectXKey),
                AspectY = item.ReadString(Data.Models.Metadata.Video.AspectYKey),
                Freq = item.ReadString(Data.Models.Metadata.Video.RefreshKey),
            };
            return video;
        }
    }
}
