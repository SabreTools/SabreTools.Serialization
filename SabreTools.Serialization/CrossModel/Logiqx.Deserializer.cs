using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.Logiqx;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Logiqx : IModelSerializer<Datafile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Datafile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj) => Deserialize(obj, false);

        /// <inheritdoc/>
        public Datafile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj, bool game)
        {
            if (obj == null)
                return null;

            var datafile = new Datafile();

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
            {
                datafile.Build = header.ReadString(Serialization.Models.Metadata.Header.BuildKey);
                datafile.Debug = header.ReadString(Serialization.Models.Metadata.Header.DebugKey);
                datafile.SchemaLocation = header.ReadString(Serialization.Models.Metadata.Header.SchemaLocationKey);
                datafile.Header = ConvertHeaderFromInternalModel(header);
            }

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                datafile.Game = Array.ConvertAll(machines, m => ConvertMachineFromInternalModel(m, game));

            return datafile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="Header"/>
        /// </summary>
        private static Header ConvertHeaderFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var header = new Header
            {
                Id = item.ReadString(Serialization.Models.Metadata.Header.IdKey),
                Name = item.ReadString(Serialization.Models.Metadata.Header.NameKey),
                Description = item.ReadString(Serialization.Models.Metadata.Header.DescriptionKey),
                RootDir = item.ReadString(Serialization.Models.Metadata.Header.RootDirKey),
                Category = item.ReadString(Serialization.Models.Metadata.Header.CategoryKey),
                Version = item.ReadString(Serialization.Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Serialization.Models.Metadata.Header.DateKey),
                Author = item.ReadString(Serialization.Models.Metadata.Header.AuthorKey),
                Email = item.ReadString(Serialization.Models.Metadata.Header.EmailKey),
                Homepage = item.ReadString(Serialization.Models.Metadata.Header.HomepageKey),
                Url = item.ReadString(Serialization.Models.Metadata.Header.UrlKey),
                Comment = item.ReadString(Serialization.Models.Metadata.Header.CommentKey),
                Type = item.ReadString(Serialization.Models.Metadata.Header.TypeKey),
            };

            string? headerVal = item.ReadString(Serialization.Models.Metadata.Header.HeaderKey);
            string? forceMerging = item.ReadString(Serialization.Models.Metadata.Header.ForceMergingKey);
            string? forceNodump = item.ReadString(Serialization.Models.Metadata.Header.ForceNodumpKey);
            string? forceUnpacking = item.ReadString(Serialization.Models.Metadata.Header.ForcePackingKey);

            if (headerVal != null
                || forceMerging != null
                || forceNodump != null
                || forceUnpacking != null)
            {
                header.ClrMamePro = new SabreTools.Serialization.Models.Logiqx.ClrMamePro();
                if (headerVal != null)
                    header.ClrMamePro.Header = headerVal;
                if (forceMerging != null)
                    header.ClrMamePro.ForceMerging = forceMerging;
                if (forceNodump != null)
                    header.ClrMamePro.ForceNodump = forceNodump;
                if (forceUnpacking != null)
                    header.ClrMamePro.ForcePacking = forceUnpacking;
            }

            string? plugin = item.ReadString(Serialization.Models.Metadata.Header.PluginKey);
            string? romMode = item.ReadString(Serialization.Models.Metadata.Header.RomModeKey);
            string? biosMode = item.ReadString(Serialization.Models.Metadata.Header.BiosModeKey);
            string? sampleMode = item.ReadString(Serialization.Models.Metadata.Header.SampleModeKey);
            string? lockRomMode = item.ReadString(Serialization.Models.Metadata.Header.LockRomModeKey);
            string? lockBiosMode = item.ReadString(Serialization.Models.Metadata.Header.LockBiosModeKey);
            string? lockSampleMode = item.ReadString(Serialization.Models.Metadata.Header.LockSampleModeKey);

            if (plugin != null
                || romMode != null
                || biosMode != null
                || sampleMode != null
                || lockRomMode != null
                || lockBiosMode != null
                || lockSampleMode != null)
            {
                header.RomCenter = new SabreTools.Serialization.Models.Logiqx.RomCenter();
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
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to <see cref="GameBase"/>
        /// </summary>
        private static GameBase ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item, bool game = false)
        {
            GameBase gameBase = game ? new Game() : new Machine();

            gameBase.Name = item.ReadString(Serialization.Models.Metadata.Machine.NameKey);
            gameBase.SourceFile = item.ReadString(Serialization.Models.Metadata.Machine.SourceFileKey);
            gameBase.IsBios = item.ReadString(Serialization.Models.Metadata.Machine.IsBiosKey);
            gameBase.IsDevice = item.ReadString(Serialization.Models.Metadata.Machine.IsDeviceKey);
            gameBase.IsMechanical = item.ReadString(Serialization.Models.Metadata.Machine.IsMechanicalKey);
            gameBase.CloneOf = item.ReadString(Serialization.Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Serialization.Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Serialization.Models.Metadata.Machine.SampleOfKey);
            gameBase.Board = item.ReadString(Serialization.Models.Metadata.Machine.BoardKey);
            gameBase.RebuildTo = item.ReadString(Serialization.Models.Metadata.Machine.RebuildToKey);
            gameBase.Id = item.ReadString(Serialization.Models.Metadata.Machine.IdKey);
            gameBase.CloneOfId = item.ReadString(Serialization.Models.Metadata.Machine.CloneOfIdKey);
            gameBase.Runnable = item.ReadString(Serialization.Models.Metadata.Machine.RunnableKey);
            gameBase.Comment = item.ReadStringArray(Serialization.Models.Metadata.Machine.CommentKey);
            gameBase.Description = item.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey);
            gameBase.Year = item.ReadString(Serialization.Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Serialization.Models.Metadata.Machine.ManufacturerKey);
            gameBase.Publisher = item.ReadString(Serialization.Models.Metadata.Machine.PublisherKey);
            gameBase.Category = item.ReadStringArray(Serialization.Models.Metadata.Machine.CategoryKey);

            var trurip = item.Read<Trurip>(Serialization.Models.Metadata.Machine.TruripKey);
            if (trurip != null)
                gameBase.Trurip = trurip;

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

            var deviceRefs = item.Read<Serialization.Models.Metadata.DeviceRef[]>(Serialization.Models.Metadata.Machine.DeviceRefKey);
            if (deviceRefs != null && deviceRefs.Length > 0)
                gameBase.DeviceRef = Array.ConvertAll(deviceRefs, ConvertFromInternalModel);

            var samples = item.Read<Serialization.Models.Metadata.Sample[]>(Serialization.Models.Metadata.Machine.SampleKey);
            if (samples != null && samples.Length > 0)
                gameBase.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var archives = item.Read<Serialization.Models.Metadata.Archive[]>(Serialization.Models.Metadata.Machine.ArchiveKey);
            if (archives != null && archives.Length > 0)
                gameBase.Archive = Array.ConvertAll(archives, ConvertFromInternalModel);

            var driver = item.Read<Serialization.Models.Metadata.Driver>(Serialization.Models.Metadata.Machine.DriverKey);
            if (driver != null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            var softwareLists = item.Read<Serialization.Models.Metadata.SoftwareList[]>(Serialization.Models.Metadata.Machine.SoftwareListKey);
            if (softwareLists != null && softwareLists.Length > 0)
                gameBase.SoftwareList = Array.ConvertAll(softwareLists, ConvertFromInternalModel);

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
        /// Convert from <see cref="Serialization.Models.Metadata.DeviceRef"/> to <see cref="DeviceRef"/>
        /// </summary>
        private static DeviceRef ConvertFromInternalModel(Serialization.Models.Metadata.DeviceRef item)
        {
            var deviceRef = new DeviceRef
            {
                Name = item.ReadString(Serialization.Models.Metadata.DipSwitch.NameKey),
            };
            return deviceRef;
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
                Region = item.ReadString(Serialization.Models.Metadata.Disk.RegionKey),
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
                Emulation = item.ReadString(Serialization.Models.Metadata.Driver.EmulationKey),
                Cocktail = item.ReadString(Serialization.Models.Metadata.Driver.CocktailKey),
                SaveState = item.ReadString(Serialization.Models.Metadata.Driver.SaveStateKey),
                RequiresArtwork = item.ReadString(Serialization.Models.Metadata.Driver.RequiresArtworkKey),
                Unofficial = item.ReadString(Serialization.Models.Metadata.Driver.UnofficialKey),
                NoSoundHardware = item.ReadString(Serialization.Models.Metadata.Driver.NoSoundHardwareKey),
                Incomplete = item.ReadString(Serialization.Models.Metadata.Driver.IncompleteKey),
            };
            return driver;
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
        /// Convert from <see cref="Serialization.Models.Metadata.SoftwareList"/> to <see cref="SabreTools.Serialization.Models.Logiqx.SoftwareList"/>
        /// </summary>
        private static SabreTools.Serialization.Models.Logiqx.SoftwareList ConvertFromInternalModel(Serialization.Models.Metadata.SoftwareList item)
        {
            var softwareList = new SabreTools.Serialization.Models.Logiqx.SoftwareList
            {
                Tag = item.ReadString(Serialization.Models.Metadata.SoftwareList.TagKey),
                Name = item.ReadString(Serialization.Models.Metadata.SoftwareList.NameKey),
                Status = item.ReadString(Serialization.Models.Metadata.SoftwareList.StatusKey),
                Filter = item.ReadString(Serialization.Models.Metadata.SoftwareList.FilterKey),
            };
            return softwareList;
        }
    }
}
