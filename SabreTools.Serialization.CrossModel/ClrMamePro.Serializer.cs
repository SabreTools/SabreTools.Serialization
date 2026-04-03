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
                metadataFile[Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj.ClrMamePro);

            if (obj?.Game is not null && obj.Game.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Game, ConvertMachineToInternalModel);
            }

            if (obj?.Info is not null)
                metadataFile[Data.Models.Metadata.MetadataFile.InfoSourceKey] = ConvertInfoSourceToInternalModel(obj.Info);

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
                [Data.Models.Metadata.Header.HeaderKey] = item.Header,
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
        private static Data.Models.Metadata.Machine? ConvertMachineToInternalModel(GameBase? item)
        {
            if (item is null)
                return null;

            var machine = new Data.Models.Metadata.Machine
            {
                Name = item.Name,
                Description = item.Description,
                // [Data.Models.Metadata.Machine.DriverKey] = item.DriverStatus, // TODO: Needs metadata mapping
                [Data.Models.Metadata.Machine.YearKey] = item.Year,
                [Data.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Data.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Data.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Data.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Data.Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
            };

            if (item.Release is not null && item.Release.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ReleaseKey]
                    = Array.ConvertAll(item.Release, ConvertToInternalModel);
            }

            if (item.BiosSet is not null && item.BiosSet.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.BiosSetKey]
                    = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);
            }

            if (item.Rom is not null && item.Rom.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.Rom, ConvertToInternalModel);
            }

            if (item.Disk is not null && item.Disk.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DiskKey]
                    = Array.ConvertAll(item.Disk, ConvertToInternalModel);
            }

            if (item.Media is not null && item.Media.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.MediaKey]
                    = Array.ConvertAll(item.Media, ConvertToInternalModel);
            }

            if (item.Sample is not null && item.Sample.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.SampleKey]
                    = Array.ConvertAll(item.Sample, ConvertToInternalModel);
            }

            if (item.Archive is not null && item.Archive.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ArchiveKey]
                    = Array.ConvertAll(item.Archive, ConvertToInternalModel);
            }

            if (item.Chip is not null && item.Chip.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ChipKey]
                    = Array.ConvertAll(item.Chip, ConvertToInternalModel);
            }

            if (item.Video is not null)
            {
                machine[Data.Models.Metadata.Machine.VideoKey]
                    = Array.ConvertAll(item.Video, ConvertToInternalModel);
            }

            if (item.Sound is not null)
                machine[Data.Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input is not null)
                machine[Data.Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch is not null && item.DipSwitch.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DipSwitchKey]
                    = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);
            }

            if (item.Driver is not null)
                machine[Data.Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

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
                [Data.Models.Metadata.DipSwitch.EntryKey] = item.Entry,
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
                [Data.Models.Metadata.Disk.MD5Key] = item.MD5,
                [Data.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Disk.MergeKey] = item.Merge,
                Status = item.Status,
                [Data.Models.Metadata.Disk.FlagsKey] = item.Flags,
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
                [Data.Models.Metadata.Input.ControlKey] = item.Control,
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
                [Data.Models.Metadata.Media.MD5Key] = item.MD5,
                [Data.Models.Metadata.Media.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Media.SHA256Key] = item.SHA256,
                [Data.Models.Metadata.Media.SpamSumKey] = item.SpamSum,
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
                [Data.Models.Metadata.Rom.CRC16Key] = item.CRC16,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Data.Models.Metadata.Rom.CRC64Key] = item.CRC64,
                [Data.Models.Metadata.Rom.MD2Key] = item.MD2,
                [Data.Models.Metadata.Rom.MD4Key] = item.MD4,
                [Data.Models.Metadata.Rom.MD5Key] = item.MD5,
                [Data.Models.Metadata.Rom.RIPEMD128Key] = item.RIPEMD128,
                [Data.Models.Metadata.Rom.RIPEMD160Key] = item.RIPEMD160,
                [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Data.Models.Metadata.Rom.SHA384Key] = item.SHA384,
                [Data.Models.Metadata.Rom.SHA512Key] = item.SHA512,
                [Data.Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                [Data.Models.Metadata.Rom.xxHash364Key] = item.xxHash364,
                [Data.Models.Metadata.Rom.xxHash3128Key] = item.xxHash3128,
                [Data.Models.Metadata.Rom.MergeKey] = item.Merge,
                Status = item.Status,
                [Data.Models.Metadata.Rom.RegionKey] = item.Region,
                [Data.Models.Metadata.Rom.FlagsKey] = item.Flags,
                [Data.Models.Metadata.Rom.OffsetKey] = item.Offs,
                [Data.Models.Metadata.Rom.SerialKey] = item.Serial,
                [Data.Models.Metadata.Rom.HeaderKey] = item.Header,
                [Data.Models.Metadata.Rom.DateKey] = item.Date,
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
            {
                string[] sourcesCopy = [.. sources];
                infoSource[Data.Models.Metadata.InfoSource.SourceKey] = sourcesCopy;
            }

            return infoSource;
        }
    }
}
