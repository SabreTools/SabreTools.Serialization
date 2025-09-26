using System;
using System.Collections.Generic;
using SabreTools.Data.Models.Logiqx;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Logiqx : IModelSerializer<Datafile, Data.Models.Metadata.MetadataFile>
    {
        public Data.Models.Metadata.MetadataFile? Serialize(Datafile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            var machines = new List<Data.Models.Metadata.Machine>();

            if (item.Game != null && item.Game.Length > 0)
                machines.AddRange(Array.ConvertAll(item.Game, g => ConvertMachineToInternalModel(g)));

            foreach (var dir in item.Dir ?? [])
            {
                machines.AddRange(ConvertDirToInternalModel(dir));
            }

            if (machines.Count > 0)
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey] = machines.ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Datafile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Datafile item)
        {
            var header = item.Header != null ? ConvertHeaderToInternalModel(item.Header) : new Data.Models.Metadata.Header();

            header[Data.Models.Metadata.Header.BuildKey] = item.Build;
            header[Data.Models.Metadata.Header.DebugKey] = item.Debug;
            header[Data.Models.Metadata.Header.SchemaLocationKey] = item.SchemaLocation;

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Header"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Header item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.IdKey] = item.Id,
                [Data.Models.Metadata.Header.NameKey] = item.Name,
                [Data.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Header.RootDirKey] = item.RootDir,
                [Data.Models.Metadata.Header.CategoryKey] = item.Category,
                [Data.Models.Metadata.Header.VersionKey] = item.Version,
                [Data.Models.Metadata.Header.DateKey] = item.Date,
                [Data.Models.Metadata.Header.AuthorKey] = item.Author,
                [Data.Models.Metadata.Header.EmailKey] = item.Email,
                [Data.Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Data.Models.Metadata.Header.UrlKey] = item.Url,
                [Data.Models.Metadata.Header.CommentKey] = item.Comment,
                [Data.Models.Metadata.Header.TypeKey] = item.Type,
            };

            if (item.ClrMamePro != null)
            {
                header[Data.Models.Metadata.Header.HeaderKey] = item.ClrMamePro.Header;
                header[Data.Models.Metadata.Header.ForceMergingKey] = item.ClrMamePro.ForceMerging;
                header[Data.Models.Metadata.Header.ForceNodumpKey] = item.ClrMamePro.ForceNodump;
                header[Data.Models.Metadata.Header.ForcePackingKey] = item.ClrMamePro.ForcePacking;
            }

            if (item.RomCenter != null)
            {
                header[Data.Models.Metadata.Header.PluginKey] = item.RomCenter.Plugin;
                header[Data.Models.Metadata.Header.RomModeKey] = item.RomCenter.RomMode;
                header[Data.Models.Metadata.Header.BiosModeKey] = item.RomCenter.BiosMode;
                header[Data.Models.Metadata.Header.SampleModeKey] = item.RomCenter.SampleMode;
                header[Data.Models.Metadata.Header.LockRomModeKey] = item.RomCenter.LockRomMode;
                header[Data.Models.Metadata.Header.LockBiosModeKey] = item.RomCenter.LockBiosMode;
                header[Data.Models.Metadata.Header.LockSampleModeKey] = item.RomCenter.LockSampleMode;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Dir"/> to an array of <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine[] ConvertDirToInternalModel(Dir item, string? parent = null)
        {
            // Get the directory name
            string? dirName = item.Name;
            if (parent != null)
                dirName = $"{parent}\\{item.Name}";

            // Handle machine items
            Data.Models.Metadata.Machine[] machines = [];
            if (item.Game != null && item.Game.Length > 0)
                machines = Array.ConvertAll(item.Game, g => ConvertMachineToInternalModel(g, dirName));

            // Handle dir items
            List<Data.Models.Metadata.Machine> dirs = [];
            foreach (var subdir in item.Subdir ?? [])
            {
                dirs.AddRange(ConvertDirToInternalModel(subdir, dirName));
            }

            return [.. machines, .. dirs];
        }

        /// <summary>
        /// Convert from <see cref="GameBase"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item, string? dir = null)
        {
            string? machineName = item.Name;
            if (machineName != null && dir != null)
                machineName = $"{dir}\\{machineName}";

            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.NameKey] = machineName,
                [Data.Models.Metadata.Machine.SourceFileKey] = item.SourceFile,
                [Data.Models.Metadata.Machine.IsBiosKey] = item.IsBios,
                [Data.Models.Metadata.Machine.IsDeviceKey] = item.IsDevice,
                [Data.Models.Metadata.Machine.IsMechanicalKey] = item.IsMechanical,
                [Data.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Data.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Data.Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
                [Data.Models.Metadata.Machine.BoardKey] = item.Board,
                [Data.Models.Metadata.Machine.RebuildToKey] = item.RebuildTo,
                [Data.Models.Metadata.Machine.IdKey] = item.Id,
                [Data.Models.Metadata.Machine.CloneOfIdKey] = item.CloneOfId,
                [Data.Models.Metadata.Machine.RunnableKey] = item.Runnable,
                [Data.Models.Metadata.Machine.CommentKey] = item.Comment,
                [Data.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Machine.YearKey] = item.Year,
                [Data.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Data.Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Data.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Data.Models.Metadata.Machine.TruripKey] = item.Trurip,
            };

            if (item.Release != null && item.Release.Length > 0)
                machine[Data.Models.Metadata.Machine.ReleaseKey] = Array.ConvertAll(item.Release, ConvertToInternalModel);

            if (item.BiosSet != null && item.BiosSet.Length > 0)
                machine[Data.Models.Metadata.Machine.BiosSetKey] = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);

            if (item.Rom != null && item.Rom.Length > 0)
                machine[Data.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            if (item.Disk != null && item.Disk.Length > 0)
                machine[Data.Models.Metadata.Machine.DiskKey] = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            if (item.Media != null && item.Media.Length > 0)
                machine[Data.Models.Metadata.Machine.MediaKey] = Array.ConvertAll(item.Media, ConvertToInternalModel);

            if (item.DeviceRef != null && item.DeviceRef.Length > 0)
                machine[Data.Models.Metadata.Machine.DeviceRefKey] = Array.ConvertAll(item.DeviceRef, ConvertToInternalModel);

            if (item.Sample != null && item.Sample.Length > 0)
                machine[Data.Models.Metadata.Machine.SampleKey] = Array.ConvertAll(item.Sample, ConvertToInternalModel);

            if (item.Archive != null && item.Archive.Length > 0)
                machine[Data.Models.Metadata.Machine.ArchiveKey] = Array.ConvertAll(item.Archive, ConvertToInternalModel);

            if (item.Driver != null)
                machine[Data.Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.SoftwareList != null && item.SoftwareList.Length > 0)
                machine[Data.Models.Metadata.Machine.SoftwareListKey] = Array.ConvertAll(item.SoftwareList, ConvertToInternalModel);

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
        /// Convert from <see cref="DeviceRef"/> to <see cref="Models.Metadata.DeviceRef"/>
        /// </summary>
        private static Data.Models.Metadata.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Data.Models.Metadata.DeviceRef
            {
                [Data.Models.Metadata.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
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
                [Data.Models.Metadata.Disk.RegionKey] = item.Region,
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
                [Data.Models.Metadata.Driver.EmulationKey] = item.Emulation,
                [Data.Models.Metadata.Driver.CocktailKey] = item.Cocktail,
                [Data.Models.Metadata.Driver.SaveStateKey] = item.SaveState,
                [Data.Models.Metadata.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Data.Models.Metadata.Driver.UnofficialKey] = item.Unofficial,
                [Data.Models.Metadata.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Data.Models.Metadata.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
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
        /// Convert from <see cref="Models.Logiqx.SoftwareList"/> to <see cref="Models.Metadata.SoftwareList"/>
        /// </summary>
        private static Data.Models.Metadata.SoftwareList ConvertToInternalModel(Data.Models.Logiqx.SoftwareList item)
        {
            var softwareList = new Data.Models.Metadata.SoftwareList
            {
                [Data.Models.Metadata.SoftwareList.TagKey] = item.Tag,
                [Data.Models.Metadata.SoftwareList.NameKey] = item.Name,
                [Data.Models.Metadata.SoftwareList.StatusKey] = item.Status,
                [Data.Models.Metadata.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }
    }
}
