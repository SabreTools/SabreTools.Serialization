using System;
using SabreTools.Data.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : ICrossModel<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile();

            if (obj?.ClrMamePro != null)
                metadataFile[Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj.ClrMamePro);

            if (obj?.Game != null && obj.Game.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.ClrMamePro.ClrMamePro"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Data.Models.ClrMamePro.ClrMamePro item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.NameKey] = item.Name,
                [Data.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Header.RootDirKey] = item.RootDir,
                [Data.Models.Metadata.Header.CategoryKey] = item.Category,
                [Data.Models.Metadata.Header.VersionKey] = item.Version,
                [Data.Models.Metadata.Header.DateKey] = item.Date,
                [Data.Models.Metadata.Header.AuthorKey] = item.Author,
                [Data.Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Data.Models.Metadata.Header.UrlKey] = item.Url,
                [Data.Models.Metadata.Header.CommentKey] = item.Comment,
                [Data.Models.Metadata.Header.HeaderKey] = item.Header,
                [Data.Models.Metadata.Header.TypeKey] = item.Type,
                [Data.Models.Metadata.Header.ForceMergingKey] = item.ForceMerging,
                [Data.Models.Metadata.Header.ForceZippingKey] = item.ForceZipping,
                [Data.Models.Metadata.Header.ForcePackingKey] = item.ForcePacking,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="GameBase"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine? ConvertMachineToInternalModel(GameBase? item)
        {
            if (item == null)
                return null;

            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.NameKey] = item.Name,
                [Data.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Machine.YearKey] = item.Year,
                [Data.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Data.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Data.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Data.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Data.Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
            };

            if (item.Release != null && item.Release.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ReleaseKey]
                    = Array.ConvertAll(item.Release, ConvertToInternalModel);
            }

            if (item.BiosSet != null && item.BiosSet.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.BiosSetKey]
                    = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);
            }

            if (item.Rom != null && item.Rom.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.Rom, ConvertToInternalModel);
            }

            if (item.Disk != null && item.Disk.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DiskKey]
                    = Array.ConvertAll(item.Disk, ConvertToInternalModel);
            }

            if (item.Media != null && item.Media.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.MediaKey]
                    = Array.ConvertAll(item.Media, ConvertToInternalModel);
            }

            if (item.Sample != null && item.Sample.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.SampleKey]
                    = Array.ConvertAll(item.Sample, ConvertToInternalModel);
            }

            if (item.Archive != null && item.Archive.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ArchiveKey]
                    = Array.ConvertAll(item.Archive, ConvertToInternalModel);
            }

            if (item.Chip != null && item.Chip.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.ChipKey]
                    = Array.ConvertAll(item.Chip, ConvertToInternalModel);
            }

            if (item.Video != null)
            {
                machine[Data.Models.Metadata.Machine.VideoKey]
                    = Array.ConvertAll(item.Video, ConvertToInternalModel);
            }

            if (item.Sound != null)
                machine[Data.Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Data.Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.DipSwitchKey]
                    = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);
            }

            if (item.Driver != null)
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
                [Data.Models.Metadata.Archive.NameKey] = item.Name,
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
                [Data.Models.Metadata.BiosSet.NameKey] = item.Name,
                [Data.Models.Metadata.BiosSet.DescriptionKey] = item.Description,
                [Data.Models.Metadata.BiosSet.DefaultKey] = item.Default,
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
                [Data.Models.Metadata.Chip.ChipTypeKey] = item.Type,
                [Data.Models.Metadata.Chip.NameKey] = item.Name,
                [Data.Models.Metadata.Chip.FlagsKey] = item.Flags,
                [Data.Models.Metadata.Chip.ClockKey] = item.Clock,
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
                [Data.Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Data.Models.Metadata.DipSwitch.EntryKey] = item.Entry,
                [Data.Models.Metadata.DipSwitch.DefaultKey] = item.Default,
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
                [Data.Models.Metadata.Disk.NameKey] = item.Name,
                [Data.Models.Metadata.Disk.MD5Key] = item.MD5,
                [Data.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Disk.MergeKey] = item.Merge,
                [Data.Models.Metadata.Disk.StatusKey] = item.Status,
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
                [Data.Models.Metadata.Driver.StatusKey] = item.Status,
                [Data.Models.Metadata.Driver.ColorKey] = item.Color,
                [Data.Models.Metadata.Driver.SoundKey] = item.Sound,
                [Data.Models.Metadata.Driver.PaletteSizeKey] = item.PaletteSize,
                [Data.Models.Metadata.Driver.BlitKey] = item.Blit,
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
                [Data.Models.Metadata.Input.PlayersKey] = item.Players,
                [Data.Models.Metadata.Input.ControlKey] = item.Control,
                [Data.Models.Metadata.Input.ButtonsKey] = item.Buttons,
                [Data.Models.Metadata.Input.CoinsKey] = item.Coins,
                [Data.Models.Metadata.Input.TiltKey] = item.Tilt,
                [Data.Models.Metadata.Input.ServiceKey] = item.Service,
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
                [Data.Models.Metadata.Media.NameKey] = item.Name,
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
                [Data.Models.Metadata.Release.NameKey] = item.Name,
                [Data.Models.Metadata.Release.RegionKey] = item.Region,
                [Data.Models.Metadata.Release.LanguageKey] = item.Language,
                [Data.Models.Metadata.Release.DateKey] = item.Date,
                [Data.Models.Metadata.Release.DefaultKey] = item.Default,
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
                [Data.Models.Metadata.Rom.NameKey] = item.Name,
                [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
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
                [Data.Models.Metadata.Rom.StatusKey] = item.Status,
                [Data.Models.Metadata.Rom.RegionKey] = item.Region,
                [Data.Models.Metadata.Rom.FlagsKey] = item.Flags,
                [Data.Models.Metadata.Rom.OffsetKey] = item.Offs,
                [Data.Models.Metadata.Rom.SerialKey] = item.Serial,
                [Data.Models.Metadata.Rom.HeaderKey] = item.Header,
                [Data.Models.Metadata.Rom.DateKey] = item.Date,
                [Data.Models.Metadata.Rom.InvertedKey] = item.Inverted,
                [Data.Models.Metadata.Rom.MIAKey] = item.MIA,
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
                [Data.Models.Metadata.Sample.NameKey] = item.Name,
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
                [Data.Models.Metadata.Sound.ChannelsKey] = item.Channels,
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
                [Data.Models.Metadata.Video.ScreenKey] = item.Screen,
                [Data.Models.Metadata.Video.OrientationKey] = item.Orientation,
                [Data.Models.Metadata.Video.WidthKey] = item.X,
                [Data.Models.Metadata.Video.HeightKey] = item.Y,
                [Data.Models.Metadata.Video.AspectXKey] = item.AspectX,
                [Data.Models.Metadata.Video.AspectYKey] = item.AspectY,
                [Data.Models.Metadata.Video.RefreshKey] = item.Freq,
            };
            return video;
        }
    }
}
