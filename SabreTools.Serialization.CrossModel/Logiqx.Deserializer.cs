using System;
using SabreTools.Data.Models.Logiqx;

#pragma warning disable CA1822 // Mark members as static
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
            if (obj is null)
                return null;

            var datafile = new Datafile();

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            if (header is not null)
            {
                datafile.Build = header.Build;
                datafile.Debug = header.Debug;
                datafile.SchemaLocation = header.ReadString(Data.Models.Metadata.Header.SchemaLocationKey);
                datafile.Header = ConvertHeaderFromInternalModel(header);
            }

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines is not null && machines.Length > 0)
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
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                RootDir = item.RootDir,
                Category = item.Category,
                Version = item.Version,
                Date = item.Date,
                Author = item.Author,
                Email = item.Email,
                Homepage = item.Homepage,
                Url = item.Url,
                Comment = item.Comment,
                Type = item.Type,
            };

            string? headerVal = item.ReadString(Data.Models.Metadata.Header.HeaderKey);
            Data.Models.Metadata.MergingFlag forceMerging = item.ForceMerging;
            Data.Models.Metadata.NodumpFlag forceNodump = item.ForceNodump;
            Data.Models.Metadata.PackingFlag forcePacking = item.ForcePacking;

            if (headerVal is not null
                || forceMerging is not Data.Models.Metadata.MergingFlag.None
                || forceNodump is not Data.Models.Metadata.NodumpFlag.None
                || forcePacking is not Data.Models.Metadata.PackingFlag.None)
            {
                header.ClrMamePro = new Data.Models.Logiqx.ClrMamePro();
                if (headerVal is not null)
                    header.ClrMamePro.Header = headerVal;
                if (forceMerging is not Data.Models.Metadata.MergingFlag.None)
                    header.ClrMamePro.ForceMerging = forceMerging;
                if (forceNodump is not Data.Models.Metadata.NodumpFlag.None)
                    header.ClrMamePro.ForceNodump = forceNodump;
                if (forcePacking is not Data.Models.Metadata.PackingFlag.None)
                    header.ClrMamePro.ForcePacking = forcePacking;
            }

            string? plugin = item.Plugin;
            Data.Models.Metadata.MergingFlag romMode = item.RomMode;
            Data.Models.Metadata.MergingFlag biosMode = item.BiosMode;
            Data.Models.Metadata.MergingFlag sampleMode = item.SampleMode;
            bool? lockRomMode = item.LockRomMode;
            bool? lockBiosMode = item.LockBiosMode;
            bool? lockSampleMode = item.LockSampleMode;

            if (plugin is not null
                || romMode is not Data.Models.Metadata.MergingFlag.None
                || biosMode is not Data.Models.Metadata.MergingFlag.None
                || sampleMode is not Data.Models.Metadata.MergingFlag.None
                || lockRomMode is not null
                || lockBiosMode is not null
                || lockSampleMode is not null)
            {
                header.RomCenter = new Data.Models.Logiqx.RomCenter();
                if (plugin is not null)
                    header.RomCenter.Plugin = plugin;
                if (romMode is not Data.Models.Metadata.MergingFlag.None)
                    header.RomCenter.RomMode = romMode;
                if (biosMode is not Data.Models.Metadata.MergingFlag.None)
                    header.RomCenter.BiosMode = biosMode;
                if (sampleMode is not Data.Models.Metadata.MergingFlag.None)
                    header.RomCenter.SampleMode = sampleMode;
                if (lockRomMode is not null)
                    header.RomCenter.LockRomMode = lockRomMode;
                if (lockBiosMode is not null)
                    header.RomCenter.LockBiosMode = lockBiosMode;
                if (lockSampleMode is not null)
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

            gameBase.Name = item.Name;
            gameBase.SourceFile = item.ReadString(Data.Models.Metadata.Machine.SourceFileKey);
            gameBase.IsBios = item.IsBios;
            gameBase.IsDevice = item.IsDevice;
            gameBase.IsMechanical = item.IsMechanical;
            gameBase.CloneOf = item.ReadString(Data.Models.Metadata.Machine.CloneOfKey);
            gameBase.RomOf = item.ReadString(Data.Models.Metadata.Machine.RomOfKey);
            gameBase.SampleOf = item.ReadString(Data.Models.Metadata.Machine.SampleOfKey);
            gameBase.Board = item.ReadString(Data.Models.Metadata.Machine.BoardKey);
            gameBase.RebuildTo = item.ReadString(Data.Models.Metadata.Machine.RebuildToKey);
            gameBase.Id = item.ReadString(Data.Models.Metadata.Machine.IdKey);
            gameBase.CloneOfId = item.ReadString(Data.Models.Metadata.Machine.CloneOfIdKey);
            gameBase.Runnable = item.Runnable;
            gameBase.Comment = item.ReadStringArray(Data.Models.Metadata.Machine.CommentKey);
            gameBase.Description = item.Description;
            gameBase.Year = item.ReadString(Data.Models.Metadata.Machine.YearKey);
            gameBase.Manufacturer = item.ReadString(Data.Models.Metadata.Machine.ManufacturerKey);
            gameBase.Publisher = item.ReadString(Data.Models.Metadata.Machine.PublisherKey);
            gameBase.Category = item.ReadStringArray(Data.Models.Metadata.Machine.CategoryKey);

            var trurip = item.Read<Trurip>(Data.Models.Metadata.Machine.TruripKey);
            if (trurip is not null)
                gameBase.Trurip = trurip;

            var releases = item.Read<Data.Models.Metadata.Release[]>(Data.Models.Metadata.Machine.ReleaseKey);
            if (releases is not null && releases.Length > 0)
                gameBase.Release = Array.ConvertAll(releases, ConvertFromInternalModel);

            var biosSets = item.Read<Data.Models.Metadata.BiosSet[]>(Data.Models.Metadata.Machine.BiosSetKey);
            if (biosSets is not null && biosSets.Length > 0)
                gameBase.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms is not null && roms.Length > 0)
                gameBase.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Read<Data.Models.Metadata.Disk[]>(Data.Models.Metadata.Machine.DiskKey);
            if (disks is not null && disks.Length > 0)
                gameBase.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var medias = item.Read<Data.Models.Metadata.Media[]>(Data.Models.Metadata.Machine.MediaKey);
            if (medias is not null && medias.Length > 0)
                gameBase.Media = Array.ConvertAll(medias, ConvertFromInternalModel);

            var deviceRefs = item.Read<Data.Models.Metadata.DeviceRef[]>(Data.Models.Metadata.Machine.DeviceRefKey);
            if (deviceRefs is not null && deviceRefs.Length > 0)
                gameBase.DeviceRef = Array.ConvertAll(deviceRefs, ConvertFromInternalModel);

            var samples = item.Read<Data.Models.Metadata.Sample[]>(Data.Models.Metadata.Machine.SampleKey);
            if (samples is not null && samples.Length > 0)
                gameBase.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var archives = item.Read<Data.Models.Metadata.Archive[]>(Data.Models.Metadata.Machine.ArchiveKey);
            if (archives is not null && archives.Length > 0)
                gameBase.Archive = Array.ConvertAll(archives, ConvertFromInternalModel);

            var driver = item.Read<Data.Models.Metadata.Driver>(Data.Models.Metadata.Machine.DriverKey);
            if (driver is not null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            var softwareLists = item.Read<Data.Models.Metadata.SoftwareList[]>(Data.Models.Metadata.Machine.SoftwareListKey);
            if (softwareLists is not null && softwareLists.Length > 0)
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
                Name = item.Name,
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
                Name = item.Name,
                Description = item.Description,
                Default = item.Default,
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
                Name = item.Name,
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
                Name = item.Name,
                MD5 = item.ReadString(Data.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Disk.SHA1Key),
                Merge = item.ReadString(Data.Models.Metadata.Disk.MergeKey),
                Status = item.Status,
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
                Status = item.Status,
                Emulation = item.Emulation,
                Cocktail = item.Cocktail,
                SaveState = item.SaveState,
                RequiresArtwork = item.RequiresArtwork,
                Unofficial = item.Unofficial,
                NoSoundHardware = item.NoSoundHardware,
                Incomplete = item.Incomplete,
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
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                SHA256 = item.SHA256,
                SpamSum = item.SpamSum,
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
                Name = item.Name,
                Region = item.Region,
                Language = item.Language,
                Date = item.Date,
                Default = item.Default,
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
                Name = item.Name,
                Size = item.Size,
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
                Status = item.Status,
                Serial = item.ReadString(Data.Models.Metadata.Rom.SerialKey),
                Header = item.ReadString(Data.Models.Metadata.Rom.HeaderKey),
                Date = item.ReadString(Data.Models.Metadata.Rom.DateKey),
                Inverted = item.Inverted,
                MIA = item.MIA,
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
                Name = item.Name,
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
                Tag = item.Tag,
                Name = item.Name,
                Status = item.Status,
                Filter = item.Filter,
            };
            return softwareList;
        }
    }
}
