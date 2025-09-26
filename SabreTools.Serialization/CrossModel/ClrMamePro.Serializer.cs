using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.ClrMamePro;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile();

            if (obj?.ClrMamePro != null)
                metadataFile[Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj.ClrMamePro);

            if (obj?.Game != null && obj.Game.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.ClrMamePro.ClrMamePro"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(SabreTools.Serialization.Models.ClrMamePro.ClrMamePro item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.NameKey] = item.Name,
                [Serialization.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Header.RootDirKey] = item.RootDir,
                [Serialization.Models.Metadata.Header.CategoryKey] = item.Category,
                [Serialization.Models.Metadata.Header.VersionKey] = item.Version,
                [Serialization.Models.Metadata.Header.DateKey] = item.Date,
                [Serialization.Models.Metadata.Header.AuthorKey] = item.Author,
                [Serialization.Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Serialization.Models.Metadata.Header.UrlKey] = item.Url,
                [Serialization.Models.Metadata.Header.CommentKey] = item.Comment,
                [Serialization.Models.Metadata.Header.HeaderKey] = item.Header,
                [Serialization.Models.Metadata.Header.TypeKey] = item.Type,
                [Serialization.Models.Metadata.Header.ForceMergingKey] = item.ForceMerging,
                [Serialization.Models.Metadata.Header.ForceZippingKey] = item.ForceZipping,
                [Serialization.Models.Metadata.Header.ForcePackingKey] = item.ForcePacking,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="GameBase"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine? ConvertMachineToInternalModel(GameBase? item)
        {
            if (item == null)
                return null;

            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = item.Name,
                [Serialization.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Machine.YearKey] = item.Year,
                [Serialization.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Serialization.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Serialization.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Serialization.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Serialization.Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
            };

            if (item.Release != null && item.Release.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.ReleaseKey]
                    = Array.ConvertAll(item.Release, ConvertToInternalModel);
            }

            if (item.BiosSet != null && item.BiosSet.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.BiosSetKey]
                    = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);
            }

            if (item.Rom != null && item.Rom.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.Rom, ConvertToInternalModel);
            }

            if (item.Disk != null && item.Disk.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DiskKey]
                    = Array.ConvertAll(item.Disk, ConvertToInternalModel);
            }

            if (item.Media != null && item.Media.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.MediaKey]
                    = Array.ConvertAll(item.Media, ConvertToInternalModel);
            }

            if (item.Sample != null && item.Sample.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.SampleKey]
                    = Array.ConvertAll(item.Sample, ConvertToInternalModel);
            }

            if (item.Archive != null && item.Archive.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.ArchiveKey]
                    = Array.ConvertAll(item.Archive, ConvertToInternalModel);
            }

            if (item.Chip != null && item.Chip.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.ChipKey]
                    = Array.ConvertAll(item.Chip, ConvertToInternalModel);
            }

            if (item.Video != null)
            {
                machine[Serialization.Models.Metadata.Machine.VideoKey]
                    = Array.ConvertAll(item.Video, ConvertToInternalModel);
            }

            if (item.Sound != null)
                machine[Serialization.Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Serialization.Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.DipSwitchKey]
                    = Array.ConvertAll(item.DipSwitch, ConvertToInternalModel);
            }

            if (item.Driver != null)
                machine[Serialization.Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Archive"/> to <see cref="Serialization.Models.Metadata.Archive"/>
        /// </summary>
        private static Serialization.Models.Metadata.Archive ConvertToInternalModel(Archive item)
        {
            var archive = new Serialization.Models.Metadata.Archive
            {
                [Serialization.Models.Metadata.Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <see cref="BiosSet"/> to <see cref="Serialization.Models.Metadata.BiosSet"/>
        /// </summary>
        private static Serialization.Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Serialization.Models.Metadata.BiosSet
            {
                [Serialization.Models.Metadata.BiosSet.NameKey] = item.Name,
                [Serialization.Models.Metadata.BiosSet.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <see cref="Chip"/> to <see cref="Serialization.Models.Metadata.Chip"/>
        /// </summary>
        private static Serialization.Models.Metadata.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Serialization.Models.Metadata.Chip
            {
                [Serialization.Models.Metadata.Chip.ChipTypeKey] = item.Type,
                [Serialization.Models.Metadata.Chip.NameKey] = item.Name,
                [Serialization.Models.Metadata.Chip.FlagsKey] = item.Flags,
                [Serialization.Models.Metadata.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <see cref="DipSwitch"/> to <see cref="Serialization.Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Serialization.Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipswitch = new Serialization.Models.Metadata.DipSwitch
            {
                [Serialization.Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Serialization.Models.Metadata.DipSwitch.EntryKey] = item.Entry,
                [Serialization.Models.Metadata.DipSwitch.DefaultKey] = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <see cref="Disk"/> to <see cref="Serialization.Models.Metadata.Disk"/>
        /// </summary>
        private static Serialization.Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Serialization.Models.Metadata.Disk
            {
                [Serialization.Models.Metadata.Disk.NameKey] = item.Name,
                [Serialization.Models.Metadata.Disk.MD5Key] = item.MD5,
                [Serialization.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Disk.MergeKey] = item.Merge,
                [Serialization.Models.Metadata.Disk.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Disk.FlagsKey] = item.Flags,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Driver"/> to <see cref="Serialization.Models.Metadata.Driver"/>
        /// </summary>
        private static Serialization.Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Serialization.Models.Metadata.Driver
            {
                [Serialization.Models.Metadata.Driver.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Driver.ColorKey] = item.Color,
                [Serialization.Models.Metadata.Driver.SoundKey] = item.Sound,
                [Serialization.Models.Metadata.Driver.PaletteSizeKey] = item.PaletteSize,
                [Serialization.Models.Metadata.Driver.BlitKey] = item.Blit,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Input"/> to <see cref="Serialization.Models.Metadata.Input"/>
        /// </summary>
        private static Serialization.Models.Metadata.Input ConvertToInternalModel(Input item)
        {
            var input = new Serialization.Models.Metadata.Input
            {
                [Serialization.Models.Metadata.Input.PlayersKey] = item.Players,
                [Serialization.Models.Metadata.Input.ControlKey] = item.Control,
                [Serialization.Models.Metadata.Input.ButtonsKey] = item.Buttons,
                [Serialization.Models.Metadata.Input.CoinsKey] = item.Coins,
                [Serialization.Models.Metadata.Input.TiltKey] = item.Tilt,
                [Serialization.Models.Metadata.Input.ServiceKey] = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <see cref="Media"/> to <see cref="Serialization.Models.Metadata.Media"/>
        /// </summary>
        private static Serialization.Models.Metadata.Media ConvertToInternalModel(Media item)
        {
            var media = new Serialization.Models.Metadata.Media
            {
                [Serialization.Models.Metadata.Media.NameKey] = item.Name,
                [Serialization.Models.Metadata.Media.MD5Key] = item.MD5,
                [Serialization.Models.Metadata.Media.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Media.SHA256Key] = item.SHA256,
                [Serialization.Models.Metadata.Media.SpamSumKey] = item.SpamSum,
            };
            return media;
        }

        /// <summary>
        /// Convert from <see cref="Release"/> to <see cref="Serialization.Models.Metadata.Release"/>
        /// </summary>
        private static Serialization.Models.Metadata.Release ConvertToInternalModel(Release item)
        {
            var release = new Serialization.Models.Metadata.Release
            {
                [Serialization.Models.Metadata.Release.NameKey] = item.Name,
                [Serialization.Models.Metadata.Release.RegionKey] = item.Region,
                [Serialization.Models.Metadata.Release.LanguageKey] = item.Language,
                [Serialization.Models.Metadata.Release.DateKey] = item.Date,
                [Serialization.Models.Metadata.Release.DefaultKey] = item.Default,
            };
            return release;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.NameKey] = item.Name,
                [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Serialization.Models.Metadata.Rom.MD2Key] = item.MD2,
                [Serialization.Models.Metadata.Rom.MD4Key] = item.MD4,
                [Serialization.Models.Metadata.Rom.MD5Key] = item.MD5,
                [Serialization.Models.Metadata.Rom.RIPEMD128Key] = item.RIPEMD128,
                [Serialization.Models.Metadata.Rom.RIPEMD160Key] = item.RIPEMD160,
                [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Serialization.Models.Metadata.Rom.SHA384Key] = item.SHA384,
                [Serialization.Models.Metadata.Rom.SHA512Key] = item.SHA512,
                [Serialization.Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                [Serialization.Models.Metadata.Rom.xxHash364Key] = item.xxHash364,
                [Serialization.Models.Metadata.Rom.xxHash3128Key] = item.xxHash3128,
                [Serialization.Models.Metadata.Rom.MergeKey] = item.Merge,
                [Serialization.Models.Metadata.Rom.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Rom.RegionKey] = item.Region,
                [Serialization.Models.Metadata.Rom.FlagsKey] = item.Flags,
                [Serialization.Models.Metadata.Rom.OffsetKey] = item.Offs,
                [Serialization.Models.Metadata.Rom.SerialKey] = item.Serial,
                [Serialization.Models.Metadata.Rom.HeaderKey] = item.Header,
                [Serialization.Models.Metadata.Rom.DateKey] = item.Date,
                [Serialization.Models.Metadata.Rom.InvertedKey] = item.Inverted,
                [Serialization.Models.Metadata.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Sample"/> to <see cref="Serialization.Models.Metadata.Sample"/>
        /// </summary>
        private static Serialization.Models.Metadata.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Serialization.Models.Metadata.Sample
            {
                [Serialization.Models.Metadata.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <see cref="Sound"/> to <see cref="Serialization.Models.Metadata.Sound"/>
        /// </summary>
        private static Serialization.Models.Metadata.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Serialization.Models.Metadata.Sound
            {
                [Serialization.Models.Metadata.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <see cref="Video"/> to <see cref="Serialization.Models.Metadata.Video"/>
        /// </summary>
        private static Serialization.Models.Metadata.Video ConvertToInternalModel(Video item)
        {
            var video = new Serialization.Models.Metadata.Video
            {
                [Serialization.Models.Metadata.Video.ScreenKey] = item.Screen,
                [Serialization.Models.Metadata.Video.OrientationKey] = item.Orientation,
                [Serialization.Models.Metadata.Video.WidthKey] = item.X,
                [Serialization.Models.Metadata.Video.HeightKey] = item.Y,
                [Serialization.Models.Metadata.Video.AspectXKey] = item.AspectX,
                [Serialization.Models.Metadata.Video.AspectYKey] = item.AspectY,
                [Serialization.Models.Metadata.Video.RefreshKey] = item.Freq,
            };
            return video;
        }
    }
}
