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
        #region Constants

        /// <summary>
        /// Read or write format
        /// </summary>
        public const string DatFormatKey = "DATFORMAT";

        /// <summary>
        /// External name of the DAT
        /// </summary>
        public const string FileNameKey = "FILENAME";

        #endregion

        #region Fields

        public MergingFlag BiosMode
        {
            get => _internal.BiosMode;
            set => _internal.BiosMode = value;
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

        public MergingFlag RomMode
        {
            get => _internal.RomMode;
            set => _internal.RomMode = value;
        }

        public MergingFlag SampleMode
        {
            get => _internal.SampleMode;
            set => _internal.SampleMode = value;
        }

        [JsonIgnore]
        public bool SearchSpecified
        {
            get
            {
                return Read<Data.Models.OfflineList.Search?>(Data.Models.Metadata.Header.SearchKey) is not null;
            }
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
        public object Clone() => new DatHeader(GetInternalClone());

        /// <summary>
        /// Clone just the format from the current header
        /// </summary>
        public DatHeader CloneFormat()
        {
            var header = new DatHeader();

            header.Write(DatFormatKey, Read<DatFormat>(DatFormatKey));

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
