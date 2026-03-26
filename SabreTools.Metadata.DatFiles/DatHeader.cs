using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Metadata.Filter;

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

        [JsonIgnore]
        public bool CanOpenSpecified
        {
            get
            {
                var canOpen = ReadStringArray(Data.Models.Metadata.Header.CanOpenKey);
                return canOpen is not null && canOpen.Length > 0;
            }
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

        [JsonIgnore]
        public bool NewDatSpecified
        {
            get
            {
                return Read<Data.Models.OfflineList.NewDat?>(Data.Models.Metadata.Header.NewDatKey) is not null;
            }
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
            // Create a new internal model
            _internal = [];

            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Data.Models.Metadata.Header));
            if (nonItemFields is not null)
            {
                // Populate the internal machine from non-filter fields
                foreach (string fieldName in nonItemFields)
                {
                    if (header.TryGetValue(fieldName, out var fieldValue))
                        _internal[fieldName] = fieldValue;
                }
            }

            // Get all fields specific to the DatFiles implementation
            var nonStandardFields = TypeHelper.GetConstants(typeof(DatHeader));
            if (nonStandardFields is not null)
            {
                // Populate the internal machine from filter fields
                foreach (string fieldName in nonStandardFields)
                {
                    if (header.TryGetValue(fieldName, out var fieldValue))
                        _internal[fieldName] = fieldValue;
                }
            }

            // Get all no-filter fields
            if (header.TryGetValue(Data.Models.Metadata.Header.CanOpenKey, out var canOpenValue))
                _internal[Data.Models.Metadata.Header.CanOpenKey] = canOpenValue;
            if (header.TryGetValue(Data.Models.Metadata.Header.ImagesKey, out var imageValue))
                _internal[Data.Models.Metadata.Header.ImagesKey] = imageValue;
            if (header.TryGetValue(Data.Models.Metadata.Header.InfosKey, out var infosValue))
                _internal[Data.Models.Metadata.Header.InfosKey] = infosValue;
            if (header.TryGetValue(Data.Models.Metadata.Header.NewDatKey, out var newDatValue))
                _internal[Data.Models.Metadata.Header.NewDatKey] = newDatValue;
            if (header.TryGetValue(Data.Models.Metadata.Header.SearchKey, out var searchValue))
                _internal[Data.Models.Metadata.Header.SearchKey] = searchValue;
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
            var header = (_internal.Clone() as Data.Models.Metadata.Header)!;

            // Remove fields with default values
            if (header.ReadString(Data.Models.Metadata.Header.ForceMergingKey).AsMergingFlag() == MergingFlag.None)
                header.Remove(Data.Models.Metadata.Header.ForceMergingKey);
            if (header.ReadString(Data.Models.Metadata.Header.ForceNodumpKey).AsNodumpFlag() == NodumpFlag.None)
                header.Remove(Data.Models.Metadata.Header.ForceNodumpKey);
            if (header.ReadString(Data.Models.Metadata.Header.ForcePackingKey).AsPackingFlag() == PackingFlag.None)
                header.Remove(Data.Models.Metadata.Header.ForcePackingKey);
            if (header.ReadString(Data.Models.Metadata.Header.BiosModeKey).AsMergingFlag() == MergingFlag.None)
                header.Remove(Data.Models.Metadata.Header.BiosModeKey);
            if (header.ReadString(Data.Models.Metadata.Header.RomModeKey).AsMergingFlag() == MergingFlag.None)
                header.Remove(Data.Models.Metadata.Header.RomModeKey);
            if (header.ReadString(Data.Models.Metadata.Header.SampleModeKey).AsMergingFlag() == MergingFlag.None)
                header.Remove(Data.Models.Metadata.Header.SampleModeKey);

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
