using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.ClrMamePro;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj) => Deserialize(obj, true);

        /// <inheritdoc cref="Deserialize(Serialization.Models.Metadata.MetadataFile)"/>
        public MetadataFile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj, bool game)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.ClrMamePro = ConvertHeaderFromInternalModel(header);

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
            {
                metadataFile.Game
                    = Array.ConvertAll(machines, m => ConvertMachineFromInternalModel(m, game));
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="Models.ClrMamePro.ClrMamePro"/>
        /// </summary>
        private static SabreTools.Serialization.Models.ClrMamePro.ClrMamePro ConvertHeaderFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var clrMamePro = new SabreTools.Serialization.Models.ClrMamePro.ClrMamePro
            {
                Name = item.ReadString(Serialization.Models.Metadata.Header.NameKey),
                Description = item.ReadString(Serialization.Models.Metadata.Header.DescriptionKey),
                RootDir = item.ReadString(Serialization.Models.Metadata.Header.RootDirKey),
                Category = item.ReadString(Serialization.Models.Metadata.Header.CategoryKey),
                Version = item.ReadString(Serialization.Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Serialization.Models.Metadata.Header.DateKey),
                Author = item.ReadString(Serialization.Models.Metadata.Header.AuthorKey),
                Homepage = item.ReadString(Serialization.Models.Metadata.Header.HomepageKey),
                Url = item.ReadString(Serialization.Models.Metadata.Header.UrlKey),
                Comment = item.ReadString(Serialization.Models.Metadata.Header.CommentKey),
                Header = item.ReadString(Serialization.Models.Metadata.Header.HeaderKey),
                Type = item.ReadString(Serialization.Models.Metadata.Header.TypeKey),
                ForceMerging = item.ReadString(Serialization.Models.Metadata.Header.ForceMergingKey),
                ForceZipping = item.ReadString(Serialization.Models.Metadata.Header.ForceZippingKey),
                ForcePacking = item.ReadString(Serialization.Models.Metadata.Header.ForcePackingKey),
            };
            return clrMamePro;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Archive"/> to <see cref="Machine"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item, bool game)
        {
            GameBase gameBase = game ? new Game() : new Machine();

            gameBase.Name = item.ReadString(Serialization.Models.Metadata.Machine.NameKey);
            gameBase.Description = item.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Serialization.Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Serialization.Models.Metadata.Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Serialization.Models.Metadata.Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Serialization.Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Serialization.Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Serialization.Models.Metadata.Machine.SampleOfKey);

            var releases = item.Read<Serialization.Models.Metadata.Release[]>(Serialization.Models.Metadata.Machine.ReleaseKey);
            if (releases != null && releases.Length > 0)
                gameBase.Release = Array.ConvertAll(releases, ConvertFromInternalModel);

            var biosSets = item.Read<Serialization.Models.Metadata.BiosSet[]>(Serialization.Models.Metadata.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Length > 0)
                gameBase.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
                gameBase.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Read<Serialization.Models.Metadata.Disk[]>(Serialization.Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Length > 0)
                gameBase.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var medias = item.Read<Serialization.Models.Metadata.Media[]>(Serialization.Models.Metadata.Machine.MediaKey);
            if (medias != null && medias.Length > 0)
                gameBase.Media = Array.ConvertAll(medias, ConvertFromInternalModel);

            var samples = item.Read<Serialization.Models.Metadata.Sample[]>(Serialization.Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Length > 0)
                gameBase.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var archives = item.Read<Serialization.Models.Metadata.Archive[]>(Serialization.Models.Metadata.Machine.ArchiveKey);
            if (archives != null && archives.Length > 0)
                gameBase.Archive = Array.ConvertAll(archives, ConvertFromInternalModel);

            var chips = item.Read<Serialization.Models.Metadata.Chip[]>(Serialization.Models.Metadata.Machine.ChipKey);
            if (chips != null && chips.Length > 0)
                gameBase.Chip = Array.ConvertAll(chips, ConvertFromInternalModel);

            var videos = item.Read<Serialization.Models.Metadata.Video[]>(Serialization.Models.Metadata.Machine.VideoKey);
            if (videos != null && videos.Length > 0)
                gameBase.Video = Array.ConvertAll(videos, ConvertFromInternalModel);

            var sound = item.Read<Serialization.Models.Metadata.Sound>(Serialization.Models.Metadata.Machine.SoundKey);
            if (sound != null)
                gameBase.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Serialization.Models.Metadata.Input>(Serialization.Models.Metadata.Machine.InputKey);
            if (input != null)
                gameBase.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Serialization.Models.Metadata.DipSwitch[]>(Serialization.Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Length > 0)
                gameBase.DipSwitch = Array.ConvertAll(dipSwitches, ConvertFromInternalModel);

            var driver = item.Read<Serialization.Models.Metadata.Driver>(Serialization.Models.Metadata.Machine.DriverKey);
            if (driver != null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            return gameBase;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Archive"/> to <see cref="Archive"/>
        /// </summary>
        private static Archive ConvertFromInternalModel(Serialization.Models.Metadata.Archive item)
        {
            var archive = new Archive
            {
                Name = item.ReadString(Serialization.Models.Metadata.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.BiosSet"/> to <see cref="BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Serialization.Models.Metadata.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.ReadString(Serialization.Models.Metadata.BiosSet.NameKey),
                Description = item.ReadString(Serialization.Models.Metadata.BiosSet.DescriptionKey),
                Default = item.ReadString(Serialization.Models.Metadata.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Chip"/> to <see cref="Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Serialization.Models.Metadata.Chip item)
        {
            var chip = new Chip
            {
                Type = item.ReadString(Serialization.Models.Metadata.Chip.ChipTypeKey),
                Name = item.ReadString(Serialization.Models.Metadata.Chip.NameKey),
                Flags = item.ReadString(Serialization.Models.Metadata.Chip.FlagsKey),
                Clock = item.ReadString(Serialization.Models.Metadata.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.DipSwitch"/> to <see cref="DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Serialization.Models.Metadata.DipSwitch item)
        {
            var dipswitch = new DipSwitch
            {
                Name = item.ReadString(Serialization.Models.Metadata.DipSwitch.NameKey),
                Entry = item[Serialization.Models.Metadata.DipSwitch.EntryKey] as string[],
                Default = item.ReadString(Serialization.Models.Metadata.DipSwitch.DefaultKey),
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Disk"/> to <see cref="Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Serialization.Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Serialization.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Serialization.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Serialization.Models.Metadata.Disk.MergeKey),
                Status = item.ReadString(Serialization.Models.Metadata.Disk.StatusKey),
                Flags = item.ReadString(Serialization.Models.Metadata.Disk.FlagsKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Driver"/> to <see cref="Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Serialization.Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.ReadString(Serialization.Models.Metadata.Driver.StatusKey),
                Color = item.ReadString(Serialization.Models.Metadata.Driver.ColorKey),
                Sound = item.ReadString(Serialization.Models.Metadata.Driver.SoundKey),
                PaletteSize = item.ReadString(Serialization.Models.Metadata.Driver.PaletteSizeKey),
                Blit = item.ReadString(Serialization.Models.Metadata.Driver.BlitKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Input"/> to <see cref="Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Serialization.Models.Metadata.Input item)
        {
            var input = new Input
            {
                Players = item.ReadString(Serialization.Models.Metadata.Input.PlayersKey),
                Control = item.ReadString(Serialization.Models.Metadata.Input.ControlKey),
                Buttons = item.ReadString(Serialization.Models.Metadata.Input.ButtonsKey),
                Coins = item.ReadString(Serialization.Models.Metadata.Input.CoinsKey),
                Tilt = item.ReadString(Serialization.Models.Metadata.Input.TiltKey),
                Service = item.ReadString(Serialization.Models.Metadata.Input.ServiceKey),
            };
            return input;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Media"/> to <see cref="Media"/>
        /// </summary>
        private static Media ConvertFromInternalModel(Serialization.Models.Metadata.Media item)
        {
            var media = new Media
            {
                Name = item.ReadString(Serialization.Models.Metadata.Media.NameKey),
                MD5 = item.ReadString(Serialization.Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Serialization.Models.Metadata.Media.SHA256Key),
                SpamSum = item.ReadString(Serialization.Models.Metadata.Media.SpamSumKey),
            };
            return media;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Release"/> to <see cref="Release"/>
        /// </summary>
        private static Release ConvertFromInternalModel(Serialization.Models.Metadata.Release item)
        {
            var release = new Release
            {
                Name = item.ReadString(Serialization.Models.Metadata.Release.NameKey),
                Region = item.ReadString(Serialization.Models.Metadata.Release.RegionKey),
                Language = item.ReadString(Serialization.Models.Metadata.Release.LanguageKey),
                Date = item.ReadString(Serialization.Models.Metadata.Release.DateKey),
                Default = item.ReadString(Serialization.Models.Metadata.Release.DefaultKey),
            };
            return release;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                MD2 = item.ReadString(Serialization.Models.Metadata.Rom.MD2Key),
                MD4 = item.ReadString(Serialization.Models.Metadata.Rom.MD4Key),
                MD5 = item.ReadString(Serialization.Models.Metadata.Rom.MD5Key),
                RIPEMD128 = item.ReadString(Serialization.Models.Metadata.Rom.RIPEMD128Key),
                RIPEMD160 = item.ReadString(Serialization.Models.Metadata.Rom.RIPEMD160Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Serialization.Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Serialization.Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Serialization.Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Serialization.Models.Metadata.Rom.SpamSumKey),
                xxHash364 = item.ReadString(Serialization.Models.Metadata.Rom.xxHash364Key),
                xxHash3128 = item.ReadString(Serialization.Models.Metadata.Rom.xxHash3128Key),
                Merge = item.ReadString(Serialization.Models.Metadata.Rom.MergeKey),
                Status = item.ReadString(Serialization.Models.Metadata.Rom.StatusKey),
                Region = item.ReadString(Serialization.Models.Metadata.Rom.RegionKey),
                Flags = item.ReadString(Serialization.Models.Metadata.Rom.FlagsKey),
                Offs = item.ReadString(Serialization.Models.Metadata.Rom.OffsetKey),
                Serial = item.ReadString(Serialization.Models.Metadata.Rom.SerialKey),
                Header = item.ReadString(Serialization.Models.Metadata.Rom.HeaderKey),
                Date = item.ReadString(Serialization.Models.Metadata.Rom.DateKey),
                Inverted = item.ReadString(Serialization.Models.Metadata.Rom.InvertedKey),
                MIA = item.ReadString(Serialization.Models.Metadata.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Sample"/> to <see cref="Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Serialization.Models.Metadata.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Serialization.Models.Metadata.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Sound"/> to <see cref="Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Serialization.Models.Metadata.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.ReadString(Serialization.Models.Metadata.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Video"/> to <see cref="Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Serialization.Models.Metadata.Video item)
        {
            var video = new Video
            {
                Screen = item.ReadString(Serialization.Models.Metadata.Video.ScreenKey),
                Orientation = item.ReadString(Serialization.Models.Metadata.Video.OrientationKey),
                X = item.ReadString(Serialization.Models.Metadata.Video.WidthKey),
                Y = item.ReadString(Serialization.Models.Metadata.Video.HeightKey),
                AspectX = item.ReadString(Serialization.Models.Metadata.Video.AspectXKey),
                AspectY = item.ReadString(Serialization.Models.Metadata.Video.AspectYKey),
                Freq = item.ReadString(Serialization.Models.Metadata.Video.RefreshKey),
            };
            return video;
        }
    }
}
