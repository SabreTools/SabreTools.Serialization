using System;
using System.Collections.Generic;
using SabreTools.Models.Logiqx;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Logiqx : IModelSerializer<Datafile, Models.Metadata.MetadataFile>
    {
        public Models.Metadata.MetadataFile? Serialize(Datafile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            var machines = new List<Models.Metadata.Machine>();

            if (item.Game != null && item.Game.Length > 0)
                machines.AddRange(Array.ConvertAll(item.Game, ConvertMachineToInternalModel));

            foreach (var dir in item.Dir ?? [])
            {
                machines.AddRange(ConvertDirToInternalModel(dir));
            }

            if (machines.Count > 0)
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = machines.ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Datafile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Datafile item)
        {
            var header = item.Header != null ? ConvertHeaderToInternalModel(item.Header) : new Models.Metadata.Header();

            header[Models.Metadata.Header.BuildKey] = item.Build;
            header[Models.Metadata.Header.DebugKey] = item.Debug;
            header[Models.Metadata.Header.SchemaLocationKey] = item.SchemaLocation;

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Header"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Header item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.IdKey] = item.Id,
                [Models.Metadata.Header.NameKey] = item.Name,
                [Models.Metadata.Header.DescriptionKey] = item.Description,
                [Models.Metadata.Header.RootDirKey] = item.RootDir,
                [Models.Metadata.Header.CategoryKey] = item.Category,
                [Models.Metadata.Header.VersionKey] = item.Version,
                [Models.Metadata.Header.DateKey] = item.Date,
                [Models.Metadata.Header.AuthorKey] = item.Author,
                [Models.Metadata.Header.EmailKey] = item.Email,
                [Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Models.Metadata.Header.UrlKey] = item.Url,
                [Models.Metadata.Header.CommentKey] = item.Comment,
                [Models.Metadata.Header.TypeKey] = item.Type,
            };

            if (item.ClrMamePro != null)
            {
                header[Models.Metadata.Header.HeaderKey] = item.ClrMamePro.Header;
                header[Models.Metadata.Header.ForceMergingKey] = item.ClrMamePro.ForceMerging;
                header[Models.Metadata.Header.ForceNodumpKey] = item.ClrMamePro.ForceNodump;
                header[Models.Metadata.Header.ForcePackingKey] = item.ClrMamePro.ForcePacking;
            }

            if (item.RomCenter != null)
            {
                header[Models.Metadata.Header.PluginKey] = item.RomCenter.Plugin;
                header[Models.Metadata.Header.RomModeKey] = item.RomCenter.RomMode;
                header[Models.Metadata.Header.BiosModeKey] = item.RomCenter.BiosMode;
                header[Models.Metadata.Header.SampleModeKey] = item.RomCenter.SampleMode;
                header[Models.Metadata.Header.LockRomModeKey] = item.RomCenter.LockRomMode;
                header[Models.Metadata.Header.LockBiosModeKey] = item.RomCenter.LockBiosMode;
                header[Models.Metadata.Header.LockSampleModeKey] = item.RomCenter.LockSampleMode;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Dir"/> to an array of <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine[] ConvertDirToInternalModel(Dir item, string? parent = null)
        {
            // Get the directory name
            string? dirName = item.Name;
            if (parent != null)
                dirName = $"{parent}\\{item.Name}";

            // Handle machine items
            Models.Metadata.Machine[] machines = [];
            if (item.Game != null && item.Game.Length > 0)
                machines = Array.ConvertAll(item.Game, g => ConvertMachineToInternalModel(g, dirName));

            // Handle dir items
            List<Models.Metadata.Machine> dirs = [];
            foreach (var subdir in item.Subdir ?? [])
            {
                dirs.AddRange(ConvertDirToInternalModel(subdir, dirName));
            }

            return [.. machines, .. dirs];
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.GameBase"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item, string? dir = null)
        {
            string? machineName = item.Name;
            if (machineName != null && dir != null)
                machineName = $"{dir}\\{machineName}";

            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = machineName,
                [Models.Metadata.Machine.SourceFileKey] = item.SourceFile,
                [Models.Metadata.Machine.IsBiosKey] = item.IsBios,
                [Models.Metadata.Machine.IsDeviceKey] = item.IsDevice,
                [Models.Metadata.Machine.IsMechanicalKey] = item.IsMechanical,
                [Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
                [Models.Metadata.Machine.BoardKey] = item.Board,
                [Models.Metadata.Machine.RebuildToKey] = item.RebuildTo,
                [Models.Metadata.Machine.IdKey] = item.Id,
                [Models.Metadata.Machine.CloneOfIdKey] = item.CloneOfId,
                [Models.Metadata.Machine.RunnableKey] = item.Runnable,
                [Models.Metadata.Machine.CommentKey] = item.Comment,
                [Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Models.Metadata.Machine.CategoryKey] = item.Category,
                [Models.Metadata.Machine.TruripKey] = item.Trurip,
            };

            if (item.Release != null && item.Release.Length > 0)
                machine[Models.Metadata.Machine.ReleaseKey] = Array.ConvertAll(item.Release, ConvertToInternalModel);

            if (item.BiosSet != null && item.BiosSet.Length > 0)
                machine[Models.Metadata.Machine.BiosSetKey] = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);

            if (item.Rom != null && item.Rom.Length > 0)
                machine[Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            if (item.Disk != null && item.Disk.Length > 0)
                machine[Models.Metadata.Machine.DiskKey] = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            if (item.Media != null && item.Media.Length > 0)
                machine[Models.Metadata.Machine.MediaKey] = Array.ConvertAll(item.Media, ConvertToInternalModel);

            if (item.DeviceRef != null && item.DeviceRef.Length > 0)
                machine[Models.Metadata.Machine.DeviceRefKey] = Array.ConvertAll(item.DeviceRef, ConvertToInternalModel);

            if (item.Sample != null && item.Sample.Length > 0)
                machine[Models.Metadata.Machine.SampleKey] = Array.ConvertAll(item.Sample, ConvertToInternalModel);

            if (item.Archive != null && item.Archive.Length > 0)
                machine[Models.Metadata.Machine.ArchiveKey] = Array.ConvertAll(item.Archive, ConvertToInternalModel);

            if (item.Driver != null)
                machine[Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.SoftwareList != null && item.SoftwareList.Length > 0)
                machine[Models.Metadata.Machine.SoftwareListKey] = Array.ConvertAll(item.SoftwareList, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Archive"/> to <see cref="Models.Metadata.Archive"/>
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
        /// Convert from <see cref="Models.Logiqx.BiosSet"/> to <see cref="Models.Metadata.BiosSet"/>
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
        /// Convert from <see cref="Models.Logiqx.DeviceRef"/> to <see cref="Models.Metadata.DeviceRef"/>
        /// </summary>
        private static Models.Metadata.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Models.Metadata.DeviceRef
            {
                [Models.Metadata.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Disk"/> to <see cref="Models.Metadata.Disk"/>
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
                [Models.Metadata.Disk.RegionKey] = item.Region,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Driver"/> to <see cref="Models.Metadata.Driver"/>
        /// </summary>
        private static Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Models.Metadata.Driver
            {
                [Models.Metadata.Driver.StatusKey] = item.Status,
                [Models.Metadata.Driver.EmulationKey] = item.Emulation,
                [Models.Metadata.Driver.CocktailKey] = item.Cocktail,
                [Models.Metadata.Driver.SaveStateKey] = item.SaveState,
                [Models.Metadata.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Models.Metadata.Driver.UnofficialKey] = item.Unofficial,
                [Models.Metadata.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Models.Metadata.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Media"/> to <see cref="Models.Metadata.Media"/>
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
        /// Convert from <see cref="Models.Logiqx.Release"/> to <see cref="Models.Metadata.Release"/>
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
        /// Convert from <see cref="Models.Logiqx.Rom"/> to <see cref="Models.Metadata.Rom"/>
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
                [Models.Metadata.Rom.SerialKey] = item.Serial,
                [Models.Metadata.Rom.HeaderKey] = item.Header,
                [Models.Metadata.Rom.DateKey] = item.Date,
                [Models.Metadata.Rom.InvertedKey] = item.Inverted,
                [Models.Metadata.Rom.MIAKey] = item.MIA,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <see cref="Models.Logiqx.Sample"/> to <see cref="Models.Metadata.Sample"/>
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
        /// Convert from <see cref="Models.Logiqx.SoftwareList"/> to <see cref="Models.Metadata.SoftwareList"/>
        /// </summary>
        private static Models.Metadata.SoftwareList ConvertToInternalModel(Models.Logiqx.SoftwareList item)
        {
            var softwareList = new Models.Metadata.SoftwareList
            {
                [Models.Metadata.SoftwareList.TagKey] = item.Tag,
                [Models.Metadata.SoftwareList.NameKey] = item.Name,
                [Models.Metadata.SoftwareList.StatusKey] = item.Status,
                [Models.Metadata.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }
    }
}
