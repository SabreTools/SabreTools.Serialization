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

            var header = obj.Header;
            if (header is not null)
            {
                datafile.Build = header.Build;
                datafile.Debug = header.Debug;
                datafile.SchemaLocation = header.SchemaLocation;
                datafile.Header = ConvertHeaderFromInternalModel(header);
            }

            var machines = obj.Machine;
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

            if (item.HeaderSkipper is not null
                || item.ForceMerging is not Data.Models.Metadata.MergingFlag.None
                || item.ForceNodump is not Data.Models.Metadata.NodumpFlag.None
                || item.ForcePacking is not Data.Models.Metadata.PackingFlag.None)
            {
                header.ClrMamePro = new Data.Models.Logiqx.ClrMamePro();
                if (item.HeaderSkipper is not null)
                    header.ClrMamePro.Header = item.HeaderSkipper;
                if (item.ForceMerging is not Data.Models.Metadata.MergingFlag.None)
                    header.ClrMamePro.ForceMerging = item.ForceMerging;
                if (item.ForceNodump is not Data.Models.Metadata.NodumpFlag.None)
                    header.ClrMamePro.ForceNodump = item.ForceNodump;
                if (item.ForcePacking is not Data.Models.Metadata.PackingFlag.None)
                    header.ClrMamePro.ForcePacking = item.ForcePacking;
            }

            if (item.Plugin is not null
                || item.RomMode is not Data.Models.Metadata.MergingFlag.None
                || item.BiosMode is not Data.Models.Metadata.MergingFlag.None
                || item.SampleMode is not Data.Models.Metadata.MergingFlag.None
                || item.LockRomMode is not null
                || item.LockBiosMode is not null
                || item.LockSampleMode is not null)
            {
                header.RomCenter = new Data.Models.Logiqx.RomCenter();
                if (item.Plugin is not null)
                    header.RomCenter.Plugin = item.Plugin;
                if (item.RomMode is not Data.Models.Metadata.MergingFlag.None)
                    header.RomCenter.RomMode = item.RomMode;
                if (item.BiosMode is not Data.Models.Metadata.MergingFlag.None)
                    header.RomCenter.BiosMode = item.BiosMode;
                if (item.SampleMode is not Data.Models.Metadata.MergingFlag.None)
                    header.RomCenter.SampleMode = item.SampleMode;
                if (item.LockRomMode is not null)
                    header.RomCenter.LockRomMode = item.LockRomMode;
                if (item.LockBiosMode is not null)
                    header.RomCenter.LockBiosMode = item.LockBiosMode;
                if (item.LockSampleMode is not null)
                    header.RomCenter.LockSampleMode = item.LockSampleMode;
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
            gameBase.SourceFile = item.SourceFile;
            gameBase.IsBios = item.IsBios;
            gameBase.IsDevice = item.IsDevice;
            gameBase.IsMechanical = item.IsMechanical;
            gameBase.CloneOf = item.CloneOf;
            gameBase.RomOf = item.RomOf;
            gameBase.SampleOf = item.SampleOf;
            gameBase.Board = item.Board;
            gameBase.RebuildTo = item.RebuildTo;
            gameBase.Id = item.Id;
            gameBase.CloneOfId = item.CloneOfId;
            gameBase.Runnable = item.Runnable;
            gameBase.Comment = item.Comment;
            gameBase.Description = item.Description;
            gameBase.Year = item.Year;
            gameBase.Manufacturer = item.Manufacturer;
            gameBase.Publisher = item.Publisher;
            gameBase.Category = item.Category;

            if (item.TitleID is not null
                || item.Developer is not null
                || item.Genre is not null
                || item.Subgenre is not null
                || item.Ratings is not null
                || item.Score is not null
                || item.Enabled is not null
                || item.CRC is not null
                || item.Source is not null
                || item.RelatedTo is not null)
            {
                gameBase.Trurip = new Trurip();
                if (item.TitleID is not null)
                    gameBase.Trurip.TitleID = item.TitleID;
                if (item.Publisher is not null)
                    gameBase.Trurip.Publisher = item.Publisher;
                if (item.Developer is not null)
                    gameBase.Trurip.Developer = item.Developer;
                if (item.Year is not null)
                    gameBase.Trurip.Year = item.Year;
                if (item.Genre is not null)
                    gameBase.Trurip.Genre = item.Genre;
                if (item.Subgenre is not null)
                    gameBase.Trurip.Subgenre = item.Subgenre;
                if (item.Ratings is not null)
                    gameBase.Trurip.Ratings = item.Ratings;
                if (item.Score is not null)
                    gameBase.Trurip.Score = item.Score;
                if (item.Players is not null)
                    gameBase.Trurip.Players = item.Players;
                if (item.Enabled is not null)
                    gameBase.Trurip.Enabled = item.Enabled;
                if (item.CRC is not null)
                    gameBase.Trurip.CRC = item.CRC;
                if (item.Source is not null)
                    gameBase.Trurip.Source = item.Source;
                if (item.CloneOf is not null)
                    gameBase.Trurip.CloneOf = item.CloneOf;
                if (item.RelatedTo is not null)
                    gameBase.Trurip.RelatedTo = item.RelatedTo;
            }

            var releases = item.Release;
            if (releases is not null && releases.Length > 0)
                gameBase.Release = Array.ConvertAll(releases, ConvertFromInternalModel);

            var biosSets = item.BiosSet;
            if (biosSets is not null && biosSets.Length > 0)
                gameBase.BiosSet = Array.ConvertAll(biosSets, ConvertFromInternalModel);

            var roms = item.Rom;
            if (roms is not null && roms.Length > 0)
                gameBase.Rom = Array.ConvertAll(roms, ConvertFromInternalModel);

            var disks = item.Disk;
            if (disks is not null && disks.Length > 0)
                gameBase.Disk = Array.ConvertAll(disks, ConvertFromInternalModel);

            var medias = item.Media;
            if (medias is not null && medias.Length > 0)
                gameBase.Media = Array.ConvertAll(medias, ConvertFromInternalModel);

            var deviceRefs = item.DeviceRef;
            if (deviceRefs is not null && deviceRefs.Length > 0)
                gameBase.DeviceRef = Array.ConvertAll(deviceRefs, ConvertFromInternalModel);

            var samples = item.Sample;
            if (samples is not null && samples.Length > 0)
                gameBase.Sample = Array.ConvertAll(samples, ConvertFromInternalModel);

            var archives = item.Archive;
            if (archives is not null && archives.Length > 0)
                gameBase.Archive = Array.ConvertAll(archives, ConvertFromInternalModel);

            var driver = item.Driver;
            if (driver is not null)
                gameBase.Driver = ConvertFromInternalModel(driver);

            var softwareLists = item.SoftwareList;
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
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                Merge = item.Merge,
                Status = item.Status,
                Region = item.Region,
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
            var sample = new Sample { Name = item.Name, };
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
