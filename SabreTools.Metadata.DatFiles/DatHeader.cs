using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Metadata.Filter;
using MergingFlag = SabreTools.Data.Models.Metadata.MergingFlag;
using NodumpFlag = SabreTools.Data.Models.Metadata.NodumpFlag;
using PackingFlag = SabreTools.Data.Models.Metadata.PackingFlag;

namespace SabreTools.Metadata.DatFiles
{
    /// <summary>
    /// Represents all possible DAT header information
    /// </summary>
    [JsonObject("header"), XmlRoot("header")]
    public sealed class DatHeader : ModelBackedItem<Data.Models.Metadata.Header>, ICloneable
    {
        #region Properties

        public string? Author
        {
            get => _internal.Author;
            set => _internal.Author = value;
        }

        public MergingFlag BiosMode
        {
            get => _internal.BiosMode;
            set => _internal.BiosMode = value;
        }

        public string? Build
        {
            get => _internal.Build;
            set => _internal.Build = value;
        }

        [JsonIgnore]
        public bool CanOpenSpecified
        {
            get
            {
                var canOpen = ReadStringArray(Data.Models.Metadata.Header.CanOpenKey);
                return canOpen is not null && canOpen.Length > 0;
            }
        }

        public string? Category
        {
            get => _internal.Category;
            set => _internal.Category = value;
        }

        public string? Comment
        {
            get => _internal.Comment;
            set => _internal.Comment = value;
        }

        public string? Date
        {
            get => _internal.Date;
            set => _internal.Date = value;
        }

        /// <summary>
        /// Read or write format
        /// </summary>
        public DatFormat? DatFormat { get; set; }

        public string? DatVersion
        {
            get => _internal.DatVersion;
            set => _internal.DatVersion = value;
        }

        public bool? Debug
        {
            get => _internal.Debug;
            set => _internal.Debug = value;
        }

        public string? Description
        {
            get => _internal.Description;
            set => _internal.Description = value;
        }

        public string? Email
        {
            get => _internal.Email;
            set => _internal.Email = value;
        }

        public string? EmulatorVersion
        {
            get => _internal.EmulatorVersion;
            set => _internal.EmulatorVersion = value;
        }

        /// <summary>
        /// External name of the DAT
        /// </summary>
        public string? FileName { get; set; }

        public MergingFlag ForceMerging
        {
            get => _internal.ForceMerging;
            set => _internal.ForceMerging = value;
        }

        public NodumpFlag ForceNodump
        {
            get => _internal.ForceNodump;
            set => _internal.ForceNodump = value;
        }

        public PackingFlag ForcePacking
        {
            get => _internal.ForcePacking;
            set => _internal.ForcePacking = value;
        }

        public bool? ForceZipping
        {
            get => _internal.ForceZipping;
            set => _internal.ForceZipping = value;
        }

        public string? Homepage
        {
            get => _internal.Homepage;
            set => _internal.Homepage = value;
        }

        public string? Id
        {
            get => _internal.Id;
            set => _internal.Id = value;
        }

        [JsonIgnore]
        public bool ImagesSpecified
        {
            get
            {
                return Read<Data.Models.OfflineList.Images?>(Data.Models.Metadata.Header.ImagesKey) is not null;
            }
        }

        [JsonIgnore]
        public bool InfosSpecified
        {
            get
            {
                return Read<Data.Models.OfflineList.Infos?>(Data.Models.Metadata.Header.InfosKey) is not null;
            }
        }

        public bool? LockBiosMode
        {
            get => _internal.LockBiosMode;
            set => _internal.LockBiosMode = value;
        }

        public bool? LockRomMode
        {
            get => _internal.LockRomMode;
            set => _internal.LockRomMode = value;
        }

        public bool? LockSampleMode
        {
            get => _internal.LockSampleMode;
            set => _internal.LockSampleMode = value;
        }

        public string? MameConfig
        {
            get => _internal.MameConfig;
            set => _internal.MameConfig = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        [JsonIgnore]
        public bool NewDatSpecified
        {
            get
            {
                return Read<Data.Models.OfflineList.NewDat?>(Data.Models.Metadata.Header.NewDatKey) is not null;
            }
        }

        public string? Notes
        {
            get => _internal.Notes;
            set => _internal.Notes = value;
        }

        public string? Plugin
        {
            get => _internal.Plugin;
            set => _internal.Plugin = value;
        }

        public string? RefName
        {
            get => _internal.RefName;
            set => _internal.RefName = value;
        }

        public MergingFlag RomMode
        {
            get => _internal.RomMode;
            set => _internal.RomMode = value;
        }

        public string? RomTitle
        {
            get => _internal.RomTitle;
            set => _internal.RomTitle = value;
        }

        public string? RootDir
        {
            get => _internal.RootDir;
            set => _internal.RootDir = value;
        }

        public MergingFlag SampleMode
        {
            get => _internal.SampleMode;
            set => _internal.SampleMode = value;
        }

        public string? ScreenshotsHeight
        {
            get => _internal.ScreenshotsHeight;
            set => _internal.ScreenshotsHeight = value;
        }

        public string? ScreenshotsWidth
        {
            get => _internal.ScreenshotsWidth;
            set => _internal.ScreenshotsWidth = value;
        }

        [JsonIgnore]
        public bool SearchSpecified
        {
            get
            {
                return Read<Data.Models.OfflineList.Search?>(Data.Models.Metadata.Header.SearchKey) is not null;
            }
        }

        public string? System
        {
            get => _internal.System;
            set => _internal.System = value;
        }

        public string? Timestamp
        {
            get => _internal.Timestamp;
            set => _internal.Timestamp = value;
        }

        public string? Type
        {
            get => _internal.Type;
            set => _internal.Type = value;
        }

        public string? Url
        {
            get => _internal.Url;
            set => _internal.Url = value;
        }

        public string? Version
        {
            get => _internal.Version;
            set => _internal.Version = value;
        }

        #endregion

        #region Constructors

        public DatHeader() { }

        public DatHeader(Data.Models.Metadata.Header header)
        {
            _internal = header;
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the current header
        /// </summary>
        public object Clone() => new DatHeader(GetInternalClone())
        {
            DatFormat = DatFormat,
            FileName = FileName,
        };

        /// <summary>
        /// Clone just the format from the current header
        /// </summary>
        public DatHeader CloneFormat()
        {
            var header = new DatHeader();

            header.DatFormat = DatFormat;

            return header;
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Data.Models.Metadata.Header GetInternalClone()
        {
            var header = (_internal.DeepClone() as Data.Models.Metadata.Header)!;

            // Convert subheader values
            if (CanOpenSpecified)
                header[Data.Models.Metadata.Header.CanOpenKey] = new Data.Models.OfflineList.CanOpen { Extension = ReadStringArray(Data.Models.Metadata.Header.CanOpenKey) };
            if (ImagesSpecified)
                header[Data.Models.Metadata.Header.ImagesKey] = Read<Data.Models.OfflineList.Images>(Data.Models.Metadata.Header.ImagesKey);
            if (InfosSpecified)
                header[Data.Models.Metadata.Header.InfosKey] = Read<Data.Models.OfflineList.Infos>(Data.Models.Metadata.Header.InfosKey);
            if (NewDatSpecified)
                header[Data.Models.Metadata.Header.NewDatKey] = Read<Data.Models.OfflineList.NewDat>(Data.Models.Metadata.Header.NewDatKey);
            if (SearchSpecified)
                header[Data.Models.Metadata.Header.SearchKey] = Read<Data.Models.OfflineList.Search>(Data.Models.Metadata.Header.SearchKey);

            return header;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not DatHeader otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.Header>? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not DatHeader otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the Machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner) => filterRunner.Run(_internal);

        #endregion
    }
}
