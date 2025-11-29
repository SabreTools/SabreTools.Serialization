using System;
using SabreTools.Data.Models.Logiqx;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Logiqx : BaseMetadataSerializer<Datafile>
    {
        /// <inheritdoc/>
        public override Datafile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
            => Deserialize(obj, false);

        /// <inheritdoc/>
        public Datafile? Deserialize(Data.Models.Metadata.MetadataFile? obj, bool game)
        {
            if (obj == null)
                return null;

            var datafile = new Datafile();

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
            {
                datafile.Build = header.ReadString(Data.Models.Metadata.Header.BuildKey);
                datafile.Debug = header.ReadString(Data.Models.Metadata.Header.DebugKey);
                datafile.SchemaLocation = header.ReadString(Data.Models.Metadata.Header.SchemaLocationKey);
                datafile.Header = ConvertHeaderFromInternalModel(header);
            }

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                datafile.Game = Array.ConvertAll(machines, m => ConvertMachineFromInternalModel(m, game));

            return datafile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var header = new Header
            {
                Id = item.ReadString(Data.Models.Metadata.Header.IdKey),
                Name = item.ReadString(Data.Models.Metadata.Header.NameKey),
                Description = item.ReadString(Data.Models.Metadata.Header.DescriptionKey),
                RootDir = item.ReadString(Data.Models.Metadata.Header.RootDirKey),
                Category = item.ReadString(Data.Models.Metadata.Header.CategoryKey),
                Version = item.ReadString(Data.Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Data.Models.Metadata.Header.DateKey),
                Author = item.ReadString(Data.Models.Metadata.Header.AuthorKey),
                Email = item.ReadString(Data.Models.Metadata.Header.EmailKey),
                Homepage = item.ReadString(Data.Models.Metadata.Header.HomepageKey),
                Url = item.ReadString(Data.Models.Metadata.Header.UrlKey),
                Comment = item.ReadString(Data.Models.Metadata.Header.CommentKey),
                Type = item.ReadString(Data.Models.Metadata.Header.TypeKey),
            };

            string? headerVal = item.ReadString(Data.Models.Metadata.Header.HeaderKey);
            string? forceMerging = item.ReadString(Data.Models.Metadata.Header.ForceMergingKey);
            string? forceNodump = item.ReadString(Data.Models.Metadata.Header.ForceNodumpKey);
            string? forceUnpacking = item.ReadString(Data.Models.Metadata.Header.ForcePackingKey);

            if (headerVal != null
                || forceMerging != null
                || forceNodump != null
                || forceUnpacking != null)
            {
                header.ClrMamePro = new Data.Models.Logiqx.ClrMamePro();
                if (headerVal != null)
                    header.ClrMamePro.Header = headerVal;
                if (forceMerging != null)
                    header.ClrMamePro.ForceMerging = forceMerging;
                if (forceNodump != null)
                    header.ClrMamePro.ForceNodump = forceNodump;
                if (forceUnpacking != null)
                    header.ClrMamePro.ForcePacking = forceUnpacking;
            }

            string? plugin = item.ReadString(Data.Models.Metadata.Header.PluginKey);
            string? romMode = item.ReadString(Data.Models.Metadata.Header.RomModeKey);
            string? biosMode = item.ReadString(Data.Models.Metadata.Header.BiosModeKey);
            string? sampleMode = item.ReadString(Data.Models.Metadata.Header.SampleModeKey);
            string? lockRomMode = item.ReadString(Data.Models.Metadata.Header.LockRomModeKey);
            string? lockBiosMode = item.ReadString(Data.Models.Metadata.Header.LockBiosModeKey);
            string? lockSampleMode = item.ReadString(Data.Models.Metadata.Header.LockSampleModeKey);

            if (plugin != null
                || romMode != null
                || biosMode != null
                || sampleMode != null
                || lockRomMode != null
                || lockBiosMode != null
                || lockSampleMode != null)
            {
                header.RomCenter = new Data.Models.Logiqx.RomCenter();
                if (plugin != null)
                    header.RomCenter.Plugin = plugin;
                if (romMode != null)
                    header.RomCenter.RomMode = romMode;
                if (biosMode != null)
                    header.RomCenter.BiosMode = biosMode;
                if (sampleMode != null)
                    header.RomCenter.SampleMode = sampleMode;
                if (lockRomMode != null)
                    header.RomCenter.LockRomMode = lockRomMode;
                if (lockBiosMode != null)
                    header.RomCenter.LockBiosMode = lockBiosMode;
                if (lockSampleMode != null)
                    header.RomCenter.LockSampleMode = lockSampleMode;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="GameBase"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item, bool game = false)
        {
            GameBase gameBase = game ? new Game() : new Machine();

            gameBase.Name = item.ReadString(Data.Models.Metadata.Machine.NameKey);
            gameBase.SourceFile = item.ReadString(Data.Models.Metadata.Machine.SourceFileKey);
            gameBase.IsBios = item.ReadString(Data.Models.Metadata.Machine.IsBiosKey);
            gameBase.IsDevice = item.ReadString(Data.Models.Metadata.Machine.IsDeviceKey);
            gameBase.IsMechanical = item.ReadString(Data.Models.Metadata.Machine.IsMechanicalKey);
            gameBase.CloneOf = item.ReadString(Data.Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Data.Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Data.Models.Metadata.Machine.SampleOfKey);
            gameBase.Board = item.ReadString(Data.Models.Metadata.Machine.BoardKey);
            gameBase.RebuildTo = item.ReadString(Data.Models.Metadata.Machine.RebuildToKey);
            gameBase.Id = item.ReadString(Data.Models.Metadata.Machine.IdKey);
            gameBase.CloneOfId = item.ReadString(Data.Models.Metadata.Machine.CloneOfIdKey);
            gameBase.Runnable = item.ReadString(Data.Models.Metadata.Machine.RunnableKey);
            gameBase.Comment = item.ReadStringArray(Data.Models.Metadata.Machine.CommentKey);
            gameBase.Description = item.ReadString(Data.Models.Metadata.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Data.Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Data.Models.Metadata.Machine.ManufacturerKey);
            gameBase.Publisher = item.ReadString(Data.Models.Metadata.Machine.PublisherKey);
            gameBase.Category = item.ReadStringArray(Data.Models.Metadata.Machine.CategoryKey);

            var trurip = item.Read<Trurip>(Data.Models.Metadata.Machine.TruripKey);
            if (trurip != null)
                gameBase.Trurip = trurip;

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

            var deviceRefs = item.Read<Data.Models.Metadata.DeviceRef[]>(Data.Models.Metadata.Machine.DeviceRefKey);
            if (deviceRefs != null && deviceRefs.Length > 0)
                gameBase.DeviceRef = Array.ConvertAll(deviceRefs, ConvertFromInternalModel);

            var samples = item.Read<Data.Models.Metadata.Sample[]>(Data.Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Length > 0)
                gameBase.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var archives = item.Read<Data.Models.Metadata.Archive[]>(Data.Models.Metadata.Machine.ArchiveKey);
            if (archives != null && archives.Length > 0)
                gameBase.Archive = Array.ConvertAll(archives, ConvertFromInternalModel);

            var driver = item.Read<Data.Models.Metadata.Driver>(Data.Models.Metadata.Machine.DriverKey);
            if (driver != null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            var softwareLists = item.Read<Data.Models.Metadata.SoftwareList[]>(Data.Models.Metadata.Machine.SoftwareListKey);
            if (softwareLists != null && softwareLists.Length > 0)
                gameBase.SoftwareList = Array.ConvertAll(softwareLists, ConvertFromInternalModel);

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
        /// Convert from <see cref="Models.Metadata.DeviceRef"/> to <see cref="DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromInternalModel(Data.Models.Metadata.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                Name = item.ReadString(Data.Models.Metadata.DipSwitch.NameKey),
            };
            return deviceRef;
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
                Region = item.ReadString(Data.Models.Metadata.Disk.RegionKey),
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
                Emulation = item.ReadString(Data.Models.Metadata.Driver.EmulationKey),
                Cocktail = item.ReadString(Data.Models.Metadata.Driver.CocktailKey),
                SaveState = item.ReadString(Data.Models.Metadata.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Data.Models.Metadata.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Data.Models.Metadata.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Data.Models.Metadata.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Data.Models.Metadata.Driver.IncompleteKey),
            };
            return driver;
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
                Status = item.ReadString(Data.Models.Metadata.Rom.StatusKey),
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
        /// Convert from <see cref="Models.Metadata.SoftwareList"/> to <see cref="Models.Logiqx.SoftwareList"/>
        /// </summary>
        private static Data.Models.Logiqx.SoftwareList ConvertFromInternalModel(Data.Models.Metadata.SoftwareList item)
        {
            var softwareList = new Data.Models.Logiqx.SoftwareList
            {
                Tag = item.ReadString(Data.Models.Metadata.SoftwareList.TagKey),
                Name = item.ReadString(Data.Models.Metadata.SoftwareList.NameKey),
                Status = item.ReadString(Data.Models.Metadata.SoftwareList.StatusKey),
                Filter = item.ReadString(Data.Models.Metadata.SoftwareList.FilterKey),
            };
            return softwareList;
        }
    }
}
