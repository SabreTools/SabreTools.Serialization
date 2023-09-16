using System.Linq;
using SabreTools.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ClrMamePro : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(Models.Metadata.MetadataFile obj) => Deserialize(obj, false);
#else
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj) => Deserialize(obj, false);
#endif

        /// <inheritdoc cref="Deserialize(Models.Metadata.MetadataFile)"/>
#if NET48
        public MetadataFile Deserialize(Models.Metadata.MetadataFile obj, bool game)
#else
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj, bool game)
#endif
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.ClrMamePro = ConvertHeaderFromInternalModel(header);

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Game = machines
                    .Where(m => m != null)
                    .Select(machine => ConvertMachineFromInternalModel(machine, game))
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.ClrMamePro.ClrMamePro"/>
        /// </summary>
        private static Models.ClrMamePro.ClrMamePro ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var clrMamePro = new Models.ClrMamePro.ClrMamePro
            {
                Name = item.ReadString(Models.Metadata.Header.NameKey),
                Description = item.ReadString(Models.Metadata.Header.DescriptionKey),
                RootDir = item.ReadString(Models.Metadata.Header.RootDirKey),
                Category = item.ReadString(Models.Metadata.Header.CategoryKey),
                Version = item.ReadString(Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Models.Metadata.Header.DateKey),
                Author = item.ReadString(Models.Metadata.Header.AuthorKey),
                Homepage = item.ReadString(Models.Metadata.Header.HomepageKey),
                Url = item.ReadString(Models.Metadata.Header.UrlKey),
                Comment = item.ReadString(Models.Metadata.Header.CommentKey),
                Header = item.ReadString(Models.Metadata.Header.HeaderKey),
                Type = item.ReadString(Models.Metadata.Header.TypeKey),
                ForceMerging = item.ReadString(Models.Metadata.Header.ForceMergingKey),
                ForceZipping = item.ReadString(Models.Metadata.Header.ForceZippingKey),
                ForcePacking = item.ReadString(Models.Metadata.Header.ForcePackingKey),
            };
            return clrMamePro;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Archive"/> to <cref="Models.ClrMamePro.Machine"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Models.Metadata.Machine item, bool game = false)
        {
#if NET48
            GameBase gameBase;
            if (game)
                gameBase = new Game();
            else
                gameBase = new Machine();
#else
            GameBase gameBase = game ? new Models.ClrMamePro.Game() : new Models.ClrMamePro.Machine();
#endif

            gameBase.Name = item.ReadString(Models.Metadata.Machine.NameKey);
            gameBase.Description = item.ReadString(Models.Metadata.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Models.Metadata.Machine.ManufacturerKey);
            gameBase.Category = item.ReadString(Models.Metadata.Machine.CategoryKey);
            gameBase.CloneOf = item.ReadString(Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Models.Metadata.Machine.SampleOfKey);

            var releases = item.Read<Models.Metadata.Release[]>(Models.Metadata.Machine.ReleaseKey);
            if (releases != null && releases.Any())
            {
                gameBase.Release = releases
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var biosSets = item.Read<Models.Metadata.BiosSet[]>(Models.Metadata.Machine.BiosSetKey);
            if (biosSets != null && biosSets.Any())
            {
                gameBase.BiosSet = biosSets
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                gameBase.Rom = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                gameBase.Disk = disks
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var medias = item.Read<Models.Metadata.Media[]>(Models.Metadata.Machine.MediaKey);
            if (medias != null && medias.Any())
            {
                gameBase.Media = medias
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var samples = item.Read<Models.Metadata.Sample[]>(Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Any())
            {
                gameBase.Sample = samples
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var archives = item.Read<Models.Metadata.Archive[]>(Models.Metadata.Machine.ArchiveKey);
            if (archives != null && archives.Any())
            {
                gameBase.Archive = archives
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var chips = item.Read<Models.Metadata.Chip[]>(Models.Metadata.Machine.ChipKey);
            if (chips != null && chips.Any())
            {
                gameBase.Chip = chips
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var videos = item.Read<Models.Metadata.Video[]>(Models.Metadata.Machine.VideoKey);
            if (videos != null && videos.Any())
            {
                gameBase.Video = videos
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var sound = item.Read<Models.Metadata.Sound>(Models.Metadata.Machine.SoundKey);
            if (sound != null)
                gameBase.Sound = ConvertFromInternalModel(sound);

            var input = item.Read<Models.Metadata.Input>(Models.Metadata.Machine.InputKey);
            if (input != null)
                gameBase.Input = ConvertFromInternalModel(input);

            var dipSwitches = item.Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Machine.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                gameBase.DipSwitch = dipSwitches
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var driver = item.Read<Models.Metadata.Driver>(Models.Metadata.Machine.DriverKey);
            if (driver != null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Archive"/> to <cref="Models.ClrMamePro.Archive"/>
        /// </summary>
        private static Archive ConvertFromInternalModel(Models.Metadata.Archive item)
        {
            var archive = new Archive
            {
                Name = item.ReadString(Models.Metadata.Archive.NameKey),
            };
            return archive;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.BiosSet"/> to <cref="Models.ClrMamePro.BiosSet"/>
        /// </summary>
        private static BiosSet ConvertFromInternalModel(Models.Metadata.BiosSet item)
        {
            var biosset = new BiosSet
            {
                Name = item.ReadString(Models.Metadata.BiosSet.NameKey),
                Description = item.ReadString(Models.Metadata.BiosSet.DescriptionKey),
                Default = item.ReadString(Models.Metadata.BiosSet.DefaultKey),
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Chip"/> to <cref="Models.ClrMamePro.Chip"/>
        /// </summary>
        private static Chip ConvertFromInternalModel(Models.Metadata.Chip item)
        {
            var chip = new Chip
            {
                Type = item.ReadString(Models.Metadata.Chip.ChipTypeKey),
                Name = item.ReadString(Models.Metadata.Chip.NameKey),
                Flags = item.ReadString(Models.Metadata.Chip.FlagsKey),
                Clock = item.ReadString(Models.Metadata.Chip.ClockKey),
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DipSwitch"/> to <cref="Models.ClrMamePro.DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Models.Metadata.DipSwitch item)
        {
            var dipswitch = new DipSwitch
            {
                Name = item.ReadString(Models.Metadata.DipSwitch.NameKey),
                Entry = item[Models.Metadata.DipSwitch.EntryKey] as string[],
                Default = item.ReadString(Models.Metadata.DipSwitch.DefaultKey),
            };
            return dipswitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.ClrMamePro.Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Models.Metadata.Disk.MergeKey),
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
                Flags = item.ReadString(Models.Metadata.Disk.FlagsKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Driver"/> to <cref="Models.ClrMamePro.Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.ReadString(Models.Metadata.Driver.StatusKey),
                Color = item.ReadString(Models.Metadata.Driver.ColorKey),
                Sound = item.ReadString(Models.Metadata.Driver.SoundKey),
                PaletteSize = item.ReadString(Models.Metadata.Driver.PaletteSizeKey),
                Blit = item.ReadString(Models.Metadata.Driver.BlitKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Input"/> to <cref="Models.ClrMamePro.Input"/>
        /// </summary>
        private static Input ConvertFromInternalModel(Models.Metadata.Input item)
        {
            var input = new Input
            {
                Players = item.ReadString(Models.Metadata.Input.PlayersKey),
                Control = item.ReadString(Models.Metadata.Input.ControlKey),
                Buttons = item.ReadString(Models.Metadata.Input.ButtonsKey),
                Coins = item.ReadString(Models.Metadata.Input.CoinsKey),
                Tilt = item.ReadString(Models.Metadata.Input.TiltKey),
                Service = item.ReadString(Models.Metadata.Input.ServiceKey),
            };
            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Media"/> to <cref="Models.ClrMamePro.Media"/>
        /// </summary>
        private static Media ConvertFromInternalModel(Models.Metadata.Media item)
        {
            var media = new Media
            {
                Name = item.ReadString(Models.Metadata.Media.NameKey),
                MD5 = item.ReadString(Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Metadata.Media.SpamSumKey),
            };
            return media;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Release"/> to <cref="Models.ClrMamePro.Release"/>
        /// </summary>
        private static Release ConvertFromInternalModel(Models.Metadata.Release item)
        {
            var release = new Release
            {
                Name = item.ReadString(Models.Metadata.Release.NameKey),
                Region = item.ReadString(Models.Metadata.Release.RegionKey),
                Language = item.ReadString(Models.Metadata.Release.LanguageKey),
                Date = item.ReadString(Models.Metadata.Release.DateKey),
                Default = item.ReadString(Models.Metadata.Release.DefaultKey),
            };
            return release;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.ClrMamePro.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                MD5 = item.ReadString(Models.Metadata.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Metadata.Rom.SpamSumKey),
                xxHash364 = item.ReadString(Models.Metadata.Rom.xxHash364Key),
                xxHash3128 = item.ReadString(Models.Metadata.Rom.xxHash3128Key),
                Merge = item.ReadString(Models.Metadata.Rom.MergeKey),
                Status = item.ReadString(Models.Metadata.Rom.StatusKey),
                Region = item.ReadString(Models.Metadata.Rom.RegionKey),
                Flags = item.ReadString(Models.Metadata.Rom.FlagsKey),
                Offs = item.ReadString(Models.Metadata.Rom.OffsetKey),
                Serial = item.ReadString(Models.Metadata.Rom.SerialKey),
                Header = item.ReadString(Models.Metadata.Rom.HeaderKey),
                Date = item.ReadString(Models.Metadata.Rom.DateKey),
                Inverted = item.ReadString(Models.Metadata.Rom.InvertedKey),
                MIA = item.ReadString(Models.Metadata.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Sample"/> to <cref="Models.ClrMamePro.Sample"/>
        /// </summary>
        private static Sample ConvertFromInternalModel(Models.Metadata.Sample item)
        {
            var sample = new Sample
            {
                Name = item.ReadString(Models.Metadata.Sample.NameKey),
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Sound"/> to <cref="Models.ClrMamePro.Sound"/>
        /// </summary>
        private static Sound ConvertFromInternalModel(Models.Metadata.Sound item)
        {
            var sound = new Sound
            {
                Channels = item.ReadString(Models.Metadata.Sound.ChannelsKey),
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Video"/> to <cref="Models.ClrMamePro.Video"/>
        /// </summary>
        private static Video ConvertFromInternalModel(Models.Metadata.Video item)
        {
            var video = new Video
            {
                Screen = item.ReadString(Models.Metadata.Video.ScreenKey),
                Orientation = item.ReadString(Models.Metadata.Video.OrientationKey),
                X = item.ReadString(Models.Metadata.Video.WidthKey),
                Y = item.ReadString(Models.Metadata.Video.HeightKey),
                AspectX = item.ReadString(Models.Metadata.Video.AspectXKey),
                AspectY = item.ReadString(Models.Metadata.Video.AspectYKey),
                Freq = item.ReadString(Models.Metadata.Video.RefreshKey),
            };
            return video;
        }
    }
}