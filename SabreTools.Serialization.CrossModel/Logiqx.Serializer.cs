using System;
using System.Collections.Generic;
using SabreTools.Data.Models.Logiqx;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Logiqx : BaseMetadataSerializer<Datafile>
    {
        public override Data.Models.Metadata.MetadataFile? Serialize(Datafile? item)
        {
            if (item is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            var machines = new List<Data.Models.Metadata.Machine>();

            if (item.Game is not null && item.Game.Length > 0)
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
            var header = item.Header is not null ? ConvertHeaderToInternalModel(item.Header) : [];

            header.Build = item.Build;
            header.Debug = item.Debug;
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

            if (item.ClrMamePro is not null)
            {
                header[Data.Models.Metadata.Header.HeaderKey] = item.ClrMamePro.Header;
                header.ForceMerging = item.ClrMamePro.ForceMerging;
                header.ForceNodump = item.ClrMamePro.ForceNodump;
                header.ForcePacking = item.ClrMamePro.ForcePacking;
            }

            if (item.RomCenter is not null)
            {
                header.Plugin = item.RomCenter.Plugin;
                header.RomMode = item.RomCenter.RomMode;
                header.BiosMode = item.RomCenter.BiosMode;
                header.SampleMode = item.RomCenter.SampleMode;
                header.LockRomMode = item.RomCenter.LockRomMode;
                header.LockBiosMode = item.RomCenter.LockBiosMode;
                header.LockSampleMode = item.RomCenter.LockSampleMode;
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
            if (parent is not null)
                dirName = $"{parent}\\{item.Name}";

            // Handle machine items
            Data.Models.Metadata.Machine[] machines = [];
            if (item.Game is not null && item.Game.Length > 0)
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
            if (machineName is not null && dir is not null)
                machineName = $"{dir}\\{machineName}";

            var machine = new Data.Models.Metadata.Machine
            {
                Name = machineName,
                SourceFile = item.SourceFile,
                IsBios = item.IsBios,
                IsDevice = item.IsDevice,
                IsMechanical = item.IsMechanical,
                CloneOf = item.CloneOf,
                RomOf = item.RomOf,
                SampleOf = item.SampleOf,
                Board = item.Board,
                RebuildTo = item.RebuildTo,
                Id = item.Id,
                CloneOfId = item.CloneOfId,
                Runnable = item.Runnable,
                [Data.Models.Metadata.Machine.CommentKey] = item.Comment,
                Description = item.Description,
                Year = item.Year,
                Manufacturer = item.Manufacturer,
                Publisher = item.Publisher,
                [Data.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Data.Models.Metadata.Machine.TruripKey] = item.Trurip,
            };

            if (item.Release is not null && item.Release.Length > 0)
                machine.Release = Array.ConvertAll(item.Release, ConvertToInternalModel);

            if (item.BiosSet is not null && item.BiosSet.Length > 0)
                machine.BiosSet = Array.ConvertAll(item.BiosSet, ConvertToInternalModel);

            if (item.Rom is not null && item.Rom.Length > 0)
                machine.Rom = Array.ConvertAll(item.Rom, ConvertToInternalModel);

            if (item.Disk is not null && item.Disk.Length > 0)
                machine.Disk = Array.ConvertAll(item.Disk, ConvertToInternalModel);

            if (item.Media is not null && item.Media.Length > 0)
                machine.Media = Array.ConvertAll(item.Media, ConvertToInternalModel);

            if (item.DeviceRef is not null && item.DeviceRef.Length > 0)
                machine.DeviceRef = Array.ConvertAll(item.DeviceRef, ConvertToInternalModel);

            if (item.Sample is not null && item.Sample.Length > 0)
                machine.Sample = Array.ConvertAll(item.Sample, ConvertToInternalModel);

            if (item.Archive is not null && item.Archive.Length > 0)
                machine.Archive = Array.ConvertAll(item.Archive, ConvertToInternalModel);

            if (item.Driver is not null)
                machine.Driver = ConvertToInternalModel(item.Driver);

            if (item.SoftwareList is not null && item.SoftwareList.Length > 0)
                machine.SoftwareList = Array.ConvertAll(item.SoftwareList, ConvertToInternalModel);

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Archive"/> to <see cref="Models.Metadata.Archive"/>
        /// </summary>
        private static Data.Models.Metadata.Archive ConvertToInternalModel(Archive item)
        {
            var archive = new Data.Models.Metadata.Archive
            {
                Name = item.Name,
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
                Name = item.Name,
                Description = item.Description,
                Default = item.Default,
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
                Name = item.Name,
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
        /// Convert from <see cref="Driver"/> to <see cref="Models.Metadata.Driver"/>
        /// </summary>
        private static Data.Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Data.Models.Metadata.Driver
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
        /// Convert from <see cref="Media"/> to <see cref="Models.Metadata.Media"/>
        /// </summary>
        private static Data.Models.Metadata.Media ConvertToInternalModel(Media item)
        {
            var media = new Data.Models.Metadata.Media
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
        /// Convert from <see cref="Release"/> to <see cref="Models.Metadata.Release"/>
        /// </summary>
        private static Data.Models.Metadata.Release ConvertToInternalModel(Release item)
        {
            var release = new Data.Models.Metadata.Release
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
        /// Convert from <see cref="Rom"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                Name = item.Name,
                Size = item.Size,
                [Data.Models.Metadata.Rom.CRC16Key] = item.CRC16,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Data.Models.Metadata.Rom.CRC64Key] = item.CRC64,
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
                Status = item.Status,
                [Data.Models.Metadata.Rom.SerialKey] = item.Serial,
                [Data.Models.Metadata.Rom.HeaderKey] = item.Header,
                [Data.Models.Metadata.Rom.DateKey] = item.Date,
                Inverted = item.Inverted,
                MIA = item.MIA,
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
                Name = item.Name,
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
                Tag = item.Tag,
                Name = item.Name,
                Status = item.Status,
                Filter = item.Filter,
            };
            return softwareList;
        }
    }
}
