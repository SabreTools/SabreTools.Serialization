using System.Linq;
using SabreTools.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile();

            if (obj?.ClrMamePro != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj.ClrMamePro);

            if (obj?.Game != null && obj.Game.Length > 0)
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = obj.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel)
                    .Where(m => m != null)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.ClrMamePro"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Models.ClrMamePro.ClrMamePro item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = item.Name,
                [Models.Metadata.Header.DescriptionKey] = item.Description,
                [Models.Metadata.Header.RootDirKey] = item.RootDir,
                [Models.Metadata.Header.CategoryKey] = item.Category,
                [Models.Metadata.Header.VersionKey] = item.Version,
                [Models.Metadata.Header.DateKey] = item.Date,
                [Models.Metadata.Header.AuthorKey] = item.Author,
                [Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Models.Metadata.Header.UrlKey] = item.Url,
                [Models.Metadata.Header.CommentKey] = item.Comment,
                [Models.Metadata.Header.HeaderKey] = item.Header,
                [Models.Metadata.Header.TypeKey] = item.Type,
                [Models.Metadata.Header.ForceMergingKey] = item.ForceMerging,
                [Models.Metadata.Header.ForceZippingKey] = item.ForceZipping,
                [Models.Metadata.Header.ForcePackingKey] = item.ForcePacking,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.GameBase"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine? ConvertMachineToInternalModel(GameBase? item)
        {
            if (item == null)
                return null;

            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
                [Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Metadata.Machine.CategoryKey] = item.Category,
                [Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
            };

            if (item.Release != null && item.Release.Length > 0)
            {
                machine[Models.Metadata.Machine.ReleaseKey] = item.Release
                    .Where(r => r != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.BiosSet != null && item.BiosSet.Length > 0)
            {
                machine[Models.Metadata.Machine.BiosSetKey] = item.BiosSet
                    .Where(b => b != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Rom != null && item.Rom.Length > 0)
            {
                machine[Models.Metadata.Machine.RomKey] = item.Rom
                    .Where(r => r != null)
                .Select(ConvertToInternalModel)
                .ToArray();
            }

            if (item.Disk != null && item.Disk.Length > 0)
            {
                machine[Models.Metadata.Machine.DiskKey] = item.Disk
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Media != null && item.Media.Length > 0)
            {
                machine[Models.Metadata.Machine.MediaKey] = item.Media
                    .Where(m => m != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Sample != null && item.Sample.Length > 0)
            {
                machine[Models.Metadata.Machine.SampleKey] = item.Sample
                    .Where(s => s != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Archive != null && item.Archive.Length > 0)
            {
                machine[Models.Metadata.Machine.ArchiveKey] = item.Archive
                    .Where(a => a != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Chip != null && item.Chip.Length > 0)
            {
                machine[Models.Metadata.Machine.ChipKey] = item.Chip
                    .Where(c => c != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Video != null)
            {
                machine[Models.Metadata.Machine.VideoKey] = item.Video
                    .Where(v => v != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Sound != null)
                machine[Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Length > 0)
            {
                machine[Models.Metadata.Machine.DipSwitchKey] = item.DipSwitch
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Driver != null)
                machine[Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Archive"/> to <cref="Models.Metadata.Archive"/>
        /// </summary>
        private static Models.Metadata.Archive ConvertToInternalModel(Archive item)
        {
            var archive = new Models.Metadata.Archive
            {
                [Models.Metadata.Archive.NameKey] = item.Name,
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.BiosSet"/> to <cref="Models.Metadata.BiosSet"/>
        /// </summary>
        private static Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Models.Metadata.BiosSet
            {
                [Models.Metadata.BiosSet.NameKey] = item.Name,
                [Models.Metadata.BiosSet.DescriptionKey] = item.Description,
                [Models.Metadata.BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Chip"/> to <cref="Models.Metadata.Chip"/>
        /// </summary>
        private static Models.Metadata.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Models.Metadata.Chip
            {
                [Models.Metadata.Chip.ChipTypeKey] = item.Type,
                [Models.Metadata.Chip.NameKey] = item.Name,
                [Models.Metadata.Chip.FlagsKey] = item.Flags,
                [Models.Metadata.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.DipSwitch"/> to <cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipswitch = new Models.Metadata.DipSwitch
            {
                [Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Models.Metadata.DipSwitch.EntryKey] = item.Entry,
                [Models.Metadata.DipSwitch.DefaultKey] = item.Default,
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Disk"/> to <cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Models.Metadata.Disk
            {
                [Models.Metadata.Disk.NameKey] = item.Name,
                [Models.Metadata.Disk.MD5Key] = item.MD5,
                [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Models.Metadata.Disk.MergeKey] = item.Merge,
                [Models.Metadata.Disk.StatusKey] = item.Status,
                [Models.Metadata.Disk.FlagsKey] = item.Flags,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Driver"/> to <cref="Models.Metadata.Driver"/>
        /// </summary>
        private static Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Models.Metadata.Driver
            {
                [Models.Metadata.Driver.StatusKey] = item.Status,
                [Models.Metadata.Driver.ColorKey] = item.Color,
                [Models.Metadata.Driver.SoundKey] = item.Sound,
                [Models.Metadata.Driver.PaletteSizeKey] = item.PaletteSize,
                [Models.Metadata.Driver.BlitKey] = item.Blit,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Input"/> to <cref="Models.Metadata.Input"/>
        /// </summary>
        private static Models.Metadata.Input ConvertToInternalModel(Input item)
        {
            var input = new Models.Metadata.Input
            {
                [Models.Metadata.Input.PlayersKey] = item.Players,
                [Models.Metadata.Input.ControlKey] = item.Control,
                [Models.Metadata.Input.ButtonsKey] = item.Buttons,
                [Models.Metadata.Input.CoinsKey] = item.Coins,
                [Models.Metadata.Input.TiltKey] = item.Tilt,
                [Models.Metadata.Input.ServiceKey] = item.Service,
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Media"/> to <cref="Models.Metadata.Media"/>
        /// </summary>
        private static Models.Metadata.Media ConvertToInternalModel(Media item)
        {
            var media = new Models.Metadata.Media
            {
                [Models.Metadata.Media.NameKey] = item.Name,
                [Models.Metadata.Media.MD5Key] = item.MD5,
                [Models.Metadata.Media.SHA1Key] = item.SHA1,
                [Models.Metadata.Media.SHA256Key] = item.SHA256,
                [Models.Metadata.Media.SpamSumKey] = item.SpamSum,
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Release"/> to <cref="Models.Metadata.Release"/>
        /// </summary>
        private static Models.Metadata.Release ConvertToInternalModel(Release item)
        {
            var release = new Models.Metadata.Release
            {
                [Models.Metadata.Release.NameKey] = item.Name,
                [Models.Metadata.Release.RegionKey] = item.Region,
                [Models.Metadata.Release.LanguageKey] = item.Language,
                [Models.Metadata.Release.DateKey] = item.Date,
                [Models.Metadata.Release.DefaultKey] = item.Default,
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Rom"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SizeKey] = item.Size,
                [Models.Metadata.Rom.CRCKey] = item.CRC,
                [Models.Metadata.Rom.MD5Key] = item.MD5,
                [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Models.Metadata.Rom.SHA384Key] = item.SHA384,
                [Models.Metadata.Rom.SHA512Key] = item.SHA512,
                [Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                [Models.Metadata.Rom.xxHash364Key] = item.xxHash364,
                [Models.Metadata.Rom.xxHash3128Key] = item.xxHash3128,
                [Models.Metadata.Rom.MergeKey] = item.Merge,
                [Models.Metadata.Rom.StatusKey] = item.Status,
                [Models.Metadata.Rom.RegionKey] = item.Region,
                [Models.Metadata.Rom.FlagsKey] = item.Flags,
                [Models.Metadata.Rom.OffsetKey] = item.Offs,
                [Models.Metadata.Rom.SerialKey] = item.Serial,
                [Models.Metadata.Rom.HeaderKey] = item.Header,
                [Models.Metadata.Rom.DateKey] = item.Date,
                [Models.Metadata.Rom.InvertedKey] = item.Inverted,
                [Models.Metadata.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sample"/> to <cref="Models.Metadata.Sample"/>
        /// </summary>
        private static Models.Metadata.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Models.Metadata.Sample
            {
                [Models.Metadata.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Sound"/> to <cref="Models.Metadata.Sound"/>
        /// </summary>
        private static Models.Metadata.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Models.Metadata.Sound
            {
                [Models.Metadata.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.ClrMamePro.Video"/> to <cref="Models.Metadata.Video"/>
        /// </summary>
        private static Models.Metadata.Video ConvertToInternalModel(Video item)
        {
            var video = new Models.Metadata.Video
            {
                [Models.Metadata.Video.ScreenKey] = item.Screen,
                [Models.Metadata.Video.OrientationKey] = item.Orientation,
                [Models.Metadata.Video.WidthKey] = item.X,
                [Models.Metadata.Video.HeightKey] = item.Y,
                [Models.Metadata.Video.AspectXKey] = item.AspectX,
                [Models.Metadata.Video.AspectYKey] = item.AspectY,
                [Models.Metadata.Video.RefreshKey] = item.Freq,
            };
            return video;
        }
    }
}