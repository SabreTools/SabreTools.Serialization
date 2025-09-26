using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.Logiqx;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Logiqx : IModelSerializer<Datafile, Serialization.Models.Metadata.MetadataFile>
    {
        public Serialization.Models.Metadata.MetadataFile? Serialize(Datafile? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            var machines = new List<Serialization.Models.Metadata.Machine>();

            if (item.Game != null && item.Game.Length > 0)
                machines.AddRange(Array.ConvertAll(item.Game, g => ConvertMachineToInternalModel(g)));

            foreach (var dir in item.Dir ?? [])
            {
                machines.AddRange(ConvertDirToInternalModel(dir));
            }

            if (machines.Count > 0)
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey] = machines.ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Datafile"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(Datafile item)
        {
            var header = item.Header != null ? ConvertHeaderToInternalModel(item.Header) : new Serialization.Models.Metadata.Header();

            header[Serialization.Models.Metadata.Header.BuildKey] = item.Build;
            header[Serialization.Models.Metadata.Header.DebugKey] = item.Debug;
            header[Serialization.Models.Metadata.Header.SchemaLocationKey] = item.SchemaLocation;

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Header"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(Header item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.IdKey] = item.Id,
                [Serialization.Models.Metadata.Header.NameKey] = item.Name,
                [Serialization.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Header.RootDirKey] = item.RootDir,
                [Serialization.Models.Metadata.Header.CategoryKey] = item.Category,
                [Serialization.Models.Metadata.Header.VersionKey] = item.Version,
                [Serialization.Models.Metadata.Header.DateKey] = item.Date,
                [Serialization.Models.Metadata.Header.AuthorKey] = item.Author,
                [Serialization.Models.Metadata.Header.EmailKey] = item.Email,
                [Serialization.Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Serialization.Models.Metadata.Header.UrlKey] = item.Url,
                [Serialization.Models.Metadata.Header.CommentKey] = item.Comment,
                [Serialization.Models.Metadata.Header.TypeKey] = item.Type,
            };

            if (item.ClrMamePro != null)
            {
                header[Serialization.Models.Metadata.Header.HeaderKey] = item.ClrMamePro.Header;
                header[Serialization.Models.Metadata.Header.ForceMergingKey] = item.ClrMamePro.ForceMerging;
                header[Serialization.Models.Metadata.Header.ForceNodumpKey] = item.ClrMamePro.ForceNodump;
                header[Serialization.Models.Metadata.Header.ForcePackingKey] = item.ClrMamePro.ForcePacking;
            }

            if (item.RomCenter != null)
            {
                header[Serialization.Models.Metadata.Header.PluginKey] = item.RomCenter.Plugin;
                header[Serialization.Models.Metadata.Header.RomModeKey] = item.RomCenter.RomMode;
                header[Serialization.Models.Metadata.Header.BiosModeKey] = item.RomCenter.BiosMode;
                header[Serialization.Models.Metadata.Header.SampleModeKey] = item.RomCenter.SampleMode;
                header[Serialization.Models.Metadata.Header.LockRomModeKey] = item.RomCenter.LockRomMode;
                header[Serialization.Models.Metadata.Header.LockBiosModeKey] = item.RomCenter.LockBiosMode;
                header[Serialization.Models.Metadata.Header.LockSampleModeKey] = item.RomCenter.LockSampleMode;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Dir"/> to an array of <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine[] ConvertDirToInternalModel(Dir item, string? parent = null)
        {
            // Get the directory name
            string? dirName = item.Name;
            if (parent != null)
                dirName = $"{parent}\\{item.Name}";

            // Handle machine items
            Serialization.Models.Metadata.Machine[] machines = [];
            if (item.Game != null && item.Game.Length > 0)
                machines = Array.ConvertAll(item.Game, g => ConvertMachineToInternalModel(g, dirName));

            // Handle dir items
            List<Serialization.Models.Metadata.Machine> dirs = [];
            foreach (var subdir in item.Subdir ?? [])
            {
                dirs.AddRange(ConvertDirToInternalModel(subdir, dirName));
            }

            return [.. machines, .. dirs];
        }

        /// <summary>
        /// Convert from <see cref="GameBase"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item, string? dir = null)
        {
            string? machineName = item.Name;
            if (machineName != null && dir != null)
                machineName = $"{dir}\\{machineName}";

            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = machineName,
                [Serialization.Models.Metadata.Machine.SourceFileKey] = item.SourceFile,
                [Serialization.Models.Metadata.Machine.IsBiosKey] = item.IsBios,
                [Serialization.Models.Metadata.Machine.IsDeviceKey] = item.IsDevice,
                [Serialization.Models.Metadata.Machine.IsMechanicalKey] = item.IsMechanical,
                [Serialization.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Serialization.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Serialization.Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
                [Serialization.Models.Metadata.Machine.BoardKey] = item.Board,
                [Serialization.Models.Metadata.Machine.RebuildToKey] = item.RebuildTo,
                [Serialization.Models.Metadata.Machine.IdKey] = item.Id,
                [Serialization.Models.Metadata.Machine.CloneOfIdKey] = item.CloneOfId,
                [Serialization.Models.Metadata.Machine.RunnableKey] = item.Runnable,
                [Serialization.Models.Metadata.Machine.CommentKey] = item.Comment,
                [Serialization.Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Machine.YearKey] = item.Year,
                [Serialization.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Serialization.Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Serialization.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Serialization.Models.Metadata.Machine.TruripKey] = item.Trurip,
            };

            if (item.Release != null && item.Release.Length > 0)
                machine[Serialization.Models.Metadata.Machine.ReleaseKey] = Array.ConvertAll(item.Release, ConvertToInternalModel);

            if (item.BiosSet != null && item.BiosSet.Length > 0)
                machine[Serialization.Models.Metadata.Machine.BiosSetKey] = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);

            if (item.Rom != null && item.Rom.Length > 0)
                machine[Serialization.Models.Metadata.Machine.RomKey] = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            if (item.Disk != null && item.Disk.Length > 0)
                machine[Serialization.Models.Metadata.Machine.DiskKey] = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            if (item.Media != null && item.Media.Length > 0)
                machine[Serialization.Models.Metadata.Machine.MediaKey] = Array.ConvertAll(item.Media, ConvertToInternalModel);

            if (item.DeviceRef != null && item.DeviceRef.Length > 0)
                machine[Serialization.Models.Metadata.Machine.DeviceRefKey] = Array.ConvertAll(item.DeviceRef, ConvertToInternalModel);

            if (item.Sample != null && item.Sample.Length > 0)
                machine[Serialization.Models.Metadata.Machine.SampleKey] = Array.ConvertAll(item.Sample, ConvertToInternalModel);

            if (item.Archive != null && item.Archive.Length > 0)
                machine[Serialization.Models.Metadata.Machine.ArchiveKey] = Array.ConvertAll(item.Archive, ConvertToInternalModel);

            if (item.Driver != null)
                machine[Serialization.Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.SoftwareList != null && item.SoftwareList.Length > 0)
                machine[Serialization.Models.Metadata.Machine.SoftwareListKey] = Array.ConvertAll(item.SoftwareList, ConvertToInternalModel);

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
        /// Convert from <see cref="DeviceRef"/> to <see cref="Serialization.Models.Metadata.DeviceRef"/>
        /// </summary>
        private static Serialization.Models.Metadata.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Serialization.Models.Metadata.DeviceRef
            {
                [Serialization.Models.Metadata.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
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
                [Serialization.Models.Metadata.Disk.RegionKey] = item.Region,
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
                [Serialization.Models.Metadata.Driver.EmulationKey] = item.Emulation,
                [Serialization.Models.Metadata.Driver.CocktailKey] = item.Cocktail,
                [Serialization.Models.Metadata.Driver.SaveStateKey] = item.SaveState,
                [Serialization.Models.Metadata.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Serialization.Models.Metadata.Driver.UnofficialKey] = item.Unofficial,
                [Serialization.Models.Metadata.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Serialization.Models.Metadata.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
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
        /// Convert from <see cref="SabreTools.Serialization.Models.Logiqx.SoftwareList"/> to <see cref="Serialization.Models.Metadata.SoftwareList"/>
        /// </summary>
        private static Serialization.Models.Metadata.SoftwareList ConvertToInternalModel(SabreTools.Serialization.Models.Logiqx.SoftwareList item)
        {
            var softwareList = new Serialization.Models.Metadata.SoftwareList
            {
                [Serialization.Models.Metadata.SoftwareList.TagKey] = item.Tag,
                [Serialization.Models.Metadata.SoftwareList.NameKey] = item.Name,
                [Serialization.Models.Metadata.SoftwareList.StatusKey] = item.Status,
                [Serialization.Models.Metadata.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }
    }
}
