using System.Linq;
using SabreTools.Models.Logiqx;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Logiqx : IModelSerializer<Datafile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Datafile Deserialize(Models.Metadata.MetadataFile obj) => Deserialize(obj, false);
#else
        public Datafile? Deserialize(Models.Metadata.MetadataFile? obj) => Deserialize(obj, false);
#endif

        /// <inheritdoc/>
#if NET48
        public Datafile Deserialize(Models.Metadata.MetadataFile obj, bool game)
#else
        public Datafile? Deserialize(Models.Metadata.MetadataFile? obj, bool game)
#endif
        {
            if (obj == null)
                return null;

            var datafile = new Datafile
            {
                Build = obj.ReadString(Models.Metadata.Header.BuildKey),
                Debug = obj.ReadString(Models.Metadata.Header.DebugKey),
                SchemaLocation = obj.ReadString(Models.Metadata.Header.SchemaLocationKey),
            };

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                datafile.Header = ConvertHeaderFromInternalModel(header);

            // TODO: Handle Dir items - Currently need to be generated from the machines
            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                datafile.Game = machines
                    .Where(m => m != null)
                    .Select(machine => ConvertMachineFromInternalModel(machine, game))
                    .ToArray();
            }

            return datafile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.Logiqx.Header"/>
        /// </summary>
        private static Header ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var header = new Header
            {
                Id = item.ReadString(Models.Metadata.Header.IdKey),
                Name = item.ReadString(Models.Metadata.Header.NameKey),
                Description = item.ReadString(Models.Metadata.Header.DescriptionKey),
                RootDir = item.ReadString(Models.Metadata.Header.RootDirKey),
                Category = item.ReadString(Models.Metadata.Header.CategoryKey),
                Version = item.ReadString(Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Models.Metadata.Header.DateKey),
                Author = item.ReadString(Models.Metadata.Header.AuthorKey),
                Email = item.ReadString(Models.Metadata.Header.EmailKey),
                Homepage = item.ReadString(Models.Metadata.Header.HomepageKey),
                Url = item.ReadString(Models.Metadata.Header.UrlKey),
                Comment = item.ReadString(Models.Metadata.Header.CommentKey),
                Type = item.ReadString(Models.Metadata.Header.TypeKey),
            };

            if (item.ContainsKey(Models.Metadata.Header.HeaderKey)
                || item.ContainsKey(Models.Metadata.Header.ForceMergingKey)
                || item.ContainsKey(Models.Metadata.Header.ForceNodumpKey)
                || item.ContainsKey(Models.Metadata.Header.ForcePackingKey))
            {
                header.ClrMamePro = new Models.Logiqx.ClrMamePro
                {
                    Header = item.ReadString(Models.Metadata.Header.HeaderKey),
                    ForceMerging = item.ReadString(Models.Metadata.Header.ForceMergingKey),
                    ForceNodump = item.ReadString(Models.Metadata.Header.ForceNodumpKey),
                    ForcePacking = item.ReadString(Models.Metadata.Header.ForcePackingKey),
                };
            }

            if (item.ContainsKey(Models.Metadata.Header.PluginKey)
                || item.ContainsKey(Models.Metadata.Header.RomModeKey)
                || item.ContainsKey(Models.Metadata.Header.BiosModeKey)
                || item.ContainsKey(Models.Metadata.Header.SampleModeKey)
                || item.ContainsKey(Models.Metadata.Header.LockRomModeKey)
                || item.ContainsKey(Models.Metadata.Header.LockBiosModeKey)
                || item.ContainsKey(Models.Metadata.Header.LockSampleModeKey))
            {
                header.RomCenter = new Models.Logiqx.RomCenter
                {
                    Plugin = item.ReadString(Models.Metadata.Header.PluginKey),
                    RomMode = item.ReadString(Models.Metadata.Header.RomModeKey),
                    BiosMode = item.ReadString(Models.Metadata.Header.BiosModeKey),
                    SampleMode = item.ReadString(Models.Metadata.Header.SampleModeKey),
                    LockRomMode = item.ReadString(Models.Metadata.Header.LockRomModeKey),
                    LockBiosMode = item.ReadString(Models.Metadata.Header.LockBiosModeKey),
                    LockSampleMode = item.ReadString(Models.Metadata.Header.LockSampleModeKey),
                };
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.Logiqx.GameBase"/>
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
            GameBase gameBase = game ? new Game() : new Machine();
#endif

            gameBase.Name = item.ReadString(Models.Metadata.Machine.NameKey);
            gameBase.SourceFile = item.ReadString(Models.Metadata.Machine.SourceFileKey);
            gameBase.IsBios = item.ReadString(Models.Metadata.Machine.IsBiosKey);
            gameBase.IsDevice = item.ReadString(Models.Metadata.Machine.IsDeviceKey);
            gameBase.IsMechanical = item.ReadString(Models.Metadata.Machine.IsMechanicalKey);
            gameBase.CloneOf = item.ReadString(Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Models.Metadata.Machine.SampleOfKey);
            gameBase.Board = item.ReadString(Models.Metadata.Machine.BoardKey);
            gameBase.RebuildTo = item.ReadString(Models.Metadata.Machine.RebuildToKey);
            gameBase.Id = item.ReadString(Models.Metadata.Machine.IdKey);
            gameBase.CloneOfId = item.ReadString(Models.Metadata.Machine.CloneOfIdKey);
            gameBase.Runnable = item.ReadString(Models.Metadata.Machine.RunnableKey);
            gameBase.Comment = item.ReadStringArray(Models.Metadata.Machine.CommentKey);
            gameBase.Description = item.ReadString(Models.Metadata.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Models.Metadata.Machine.ManufacturerKey);
            gameBase.Publisher = item.ReadString(Models.Metadata.Machine.PublisherKey);
            gameBase.Category = item.ReadStringArray(Models.Metadata.Machine.CategoryKey);

            var trurip = item.Read<Trurip>(Models.Metadata.Machine.TruripKey);
            if (trurip != null)
                gameBase.Trurip = trurip;

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
                    .Where(b => b != null)
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

            var deviceRefs = item.Read<Models.Metadata.DeviceRef[]>(Models.Metadata.Machine.DeviceRefKey);
            if (deviceRefs != null && deviceRefs.Any())
            {
                gameBase.DeviceRef = deviceRefs
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

            var driver = item.Read<Models.Metadata.Driver>(Models.Metadata.Machine.DriverKey);
            if (driver != null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            var softwareLists = item.Read<Models.Metadata.SoftwareList[]>(Models.Metadata.Machine.SoftwareListKey);
            if (softwareLists != null && softwareLists.Any())
            {
                gameBase.SoftwareList = softwareLists
                    .Where(m => m != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return gameBase;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Archive"/> to <cref="Models.Logiqx.Archive"/>
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
        /// Convert from <cref="Models.Metadata.BiosSet"/> to <cref="Models.Logiqx.BiosSet"/>
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
        /// Convert from <cref="Models.Metadata.DeviceRef"/> to <cref="Models.Logiqx.DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromInternalModel(Models.Metadata.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                Name = item.ReadString(Models.Metadata.DipSwitch.NameKey),
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.Logiqx.Disk"/>
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
                Region = item.ReadString(Models.Metadata.Disk.RegionKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Driver"/> to <cref="Models.Logiqx.Driver"/>
        /// </summary>
        private static Driver ConvertFromInternalModel(Models.Metadata.Driver item)
        {
            var driver = new Driver
            {
                Status = item.ReadString(Models.Metadata.Driver.StatusKey),
                Emulation = item.ReadString(Models.Metadata.Driver.EmulationKey),
                Cocktail = item.ReadString(Models.Metadata.Driver.CocktailKey),
                SaveState = item.ReadString(Models.Metadata.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Models.Metadata.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Models.Metadata.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Models.Metadata.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Models.Metadata.Driver.IncompleteKey),
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Media"/> to <cref="Models.Logiqx.Media"/>
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
        /// Convert from <cref="Models.Metadata.Release"/> to <cref="Models.Logiqx.Release"/>
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
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Logiqx.Rom"/>
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
                Serial = item.ReadString(Models.Metadata.Rom.SerialKey),
                Header = item.ReadString(Models.Metadata.Rom.HeaderKey),
                Date = item.ReadString(Models.Metadata.Rom.DateKey),
                Inverted = item.ReadString(Models.Metadata.Rom.InvertedKey),
                MIA = item.ReadString(Models.Metadata.Rom.MIAKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Sample"/> to <cref="Models.Logiqx.Sample"/>
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
        /// Convert from <cref="Models.Metadata.SoftwareList"/> to <cref="Models.Logiqx.SoftwareList"/>
        /// </summary>
        private static Models.Logiqx.SoftwareList ConvertFromInternalModel(Models.Metadata.SoftwareList item)
        {
            var softwareList = new Models.Logiqx.SoftwareList
            {
                Tag = item.ReadString(Models.Metadata.SoftwareList.TagKey),
                Name = item.ReadString(Models.Metadata.SoftwareList.NameKey),
                Status = item.ReadString(Models.Metadata.SoftwareList.StatusKey),
                Filter = item.ReadString(Models.Metadata.SoftwareList.FilterKey),
            };
            return softwareList;
        }
    }
}