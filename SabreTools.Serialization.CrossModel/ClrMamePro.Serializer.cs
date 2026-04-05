using System;
using SabreTools.Data.Models.ClrMamePro;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile();

            if (obj?.ClrMamePro is not null)
                metadataFile.Header = ConvertHeaderToInternalModel(obj.ClrMamePro);

            if (obj?.Game is not null && obj.Game.Length > 0)
                metadataFile.Machine = Array.ConvertAll(obj.Game, ConvertMachineToInternalModel);

            if (obj?.Info is not null)
                metadataFile.InfoSource = ConvertInfoSourceToInternalModel(obj.Info);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.ClrMamePro.ClrMamePro"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Data.Models.ClrMamePro.ClrMamePro item)
        {
            var header = new Data.Models.Metadata.Header
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
                HeaderSkipper = item.Header,
                Type = item.Type,
                ForceMerging = item.ForceMerging,
                ForceZipping = item.ForceZipping,
                ForcePacking = item.ForcePacking,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="GameBase"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                Name = item.Name,
                Description = item.Description,
                // Driver = item.DriverStatus, // TODO: Needs metadata mapping
                Year = item.Year,
                Manufacturer = item.Manufacturer,
                Category = item.Category is null ? null : [item.Category],
                CloneOf = item.CloneOf,
                RomOf = item.RomOf,
                SampleOf = item.SampleOf,
            };

            if (item.Release is not null && item.Release.Length > 0)
                machine.Release = Array.ConvertAll(item.Release, ConvertToInternalModel);

            if (item.BiosSet is not null && item.BiosSet.Length > 0)
                machine.BiosSet = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);

            if (item.Rom is not null && item.Rom.Length > 0)
                machine.Rom = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            if (item.Disk is not null && item.Disk.Length > 0)
                machine.Disk = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            if (item.Media is not null && item.Media.Length > 0)
                machine.Media = Array.ConvertAll(item.Media, ConvertToInternalModel);

            if (item.Sample is not null && item.Sample.Length > 0)
                machine.Sample = Array.ConvertAll(item.Sample, ConvertToInternalModel);

            if (item.Archive is not null && item.Archive.Length > 0)
                machine.Archive = Array.ConvertAll(item.Archive, ConvertToInternalModel);

            if (item.Chip is not null && item.Chip.Length > 0)
                machine.Chip = Array.ConvertAll(item.Chip, ConvertToInternalModel);

            if (item.Video is not null)
                machine.Video = Array.ConvertAll(item.Video, ConvertToInternalModel);

            if (item.Sound is not null)
                machine.Sound = ConvertToInternalModel(item.Sound);

            if (item.Input is not null)
                machine.Input = ConvertToInternalModel(item.Input);

            if (item.DipSwitch is not null && item.DipSwitch.Length > 0)
                machine.DipSwitch = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);

            if (item.Driver is not null)
                machine.Driver = ConvertToInternalModel(item.Driver);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Archive"/> to <see cref="Models.Metadata.Archive"/>
        /// </summary>
        private static Data.Models.Metadata.Archive ConvertToInternalModel(Archive item)
        {
            var archive = new Data.Models.Metadata.Archive
            {
                Name = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <see cref="BiosSet"/> to <see cref="Models.Metadata.BiosSet"/>
        /// </summary>
        private static Data.Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Data.Models.Metadata.BiosSet
            {
                Name = item.Name,
                Description = item.Description,
                Default = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Chip"/> to <see cref="Models.Metadata.Chip"/>
        /// </summary>
        private static Data.Models.Metadata.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Data.Models.Metadata.Chip
            {
                ChipType = item.Type,
                Name = item.Name,
                Flags = item.Flags,
                Clock = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="DipSwitch"/> to <see cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Data.Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipswitch = new Data.Models.Metadata.DipSwitch
            {
                Name = item.Name,
                Entry = item.Entry,
                Default = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <see cref="Disk"/> to <see cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Data.Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Data.Models.Metadata.Disk
            {
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Merge = item.Merge,
                Status = item.Status,
                Flags = item.Flags,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Driver"/> to <see cref="Models.Metadata.Driver"/>
        /// </summary>
        private static Data.Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Data.Models.Metadata.Driver
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
        /// Convert from <see cref="Input"/> to <see cref="Models.Metadata.Input"/>
        /// </summary>
        private static Data.Models.Metadata.Input ConvertToInternalModel(Input item)
        {
            var input = new Data.Models.Metadata.Input
            {
                Players = item.Players,
                ControlAttr = item.Control,
                Buttons = item.Buttons,
                Coins = item.Coins,
                Tilt = item.Tilt,
                Service = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <see cref="Media"/> to <see cref="Models.Metadata.Media"/>
        /// </summary>
        private static Data.Models.Metadata.Media ConvertToInternalModel(Media item)
        {
            var media = new Data.Models.Metadata.Media
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
        /// Convert from <see cref="Release"/> to <see cref="Models.Metadata.Release"/>
        /// </summary>
        private static Data.Models.Metadata.Release ConvertToInternalModel(Release item)
        {
            var release = new Data.Models.Metadata.Release
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
        /// Convert from <see cref="Rom"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                Name = item.Name,
                Size = item.Size,
                CRC16 = item.CRC16,
                CRC = item.CRC,
                CRC64 = item.CRC64,
                MD2 = item.MD2,
                MD4 = item.MD4,
                MD5 = item.MD5,
                RIPEMD128 = item.RIPEMD128,
                RIPEMD160 = item.RIPEMD160,
                SHA1 = item.SHA1,
                SHA256 = item.SHA256,
                SHA384 = item.SHA384,
                SHA512 = item.SHA512,
                SpamSum = item.SpamSum,
                xxHash364 = item.xxHash364,
                xxHash3128 = item.xxHash3128,
                Merge = item.Merge,
                Status = item.Status,
                Region = item.Region,
                Flags = item.Flags,
                Offset = item.Offs,
                Serial = item.Serial,
                Header = item.Header,
                Date = item.Date,
                Inverted = item.Inverted,
                MIA = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Sample"/> to <see cref="Models.Metadata.Sample"/>
        /// </summary>
        private static Data.Models.Metadata.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Data.Models.Metadata.Sample
            {
                Name = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Sound"/> to <see cref="Models.Metadata.Sound"/>
        /// </summary>
        private static Data.Models.Metadata.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Data.Models.Metadata.Sound
            {
                Channels = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Video"/> to <see cref="Models.Metadata.Video"/>
        /// </summary>
        private static Data.Models.Metadata.Video ConvertToInternalModel(Video item)
        {
            var video = new Data.Models.Metadata.Video
            {
                Screen = item.Screen,
                Orientation = item.Orientation,
                Width = item.X,
                Height = item.Y,
                AspectX = item.AspectX,
                AspectY = item.AspectY,
                Refresh = item.Freq,
            };
            return video;
        }

        /// <summary>
        /// Convert from <see cref="Models.ClrMamePro.Info"/> to <see cref="Models.Metadata.InfoSource"/>
        /// </summary>
        private static Data.Models.Metadata.InfoSource ConvertInfoSourceToInternalModel(Info item)
        {
            var infoSource = new Data.Models.Metadata.InfoSource();

            var sources = item.Source;
            if (sources is not null && sources.Length > 0)
                infoSource.Source = [.. sources];

            return infoSource;
        }
    }
}
