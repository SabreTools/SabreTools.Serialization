using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Metadata.Filter;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems
{
    /// <summary>
    /// Represents the information specific to a set/game/machine
    /// </summary>
    [JsonObject("machine"), XmlRoot("machine")]
    public sealed class Machine : ModelBackedItem<Data.Models.Metadata.Machine>, ICloneable, IEquatable<Machine>
    {
        #region Fields

        public string? Description
        {
            get => _internal.Description;
            set => _internal.Description = value;
        }

        public bool? IsBios
        {
            get => _internal.IsBios;
            set => _internal.IsBios = value;
        }

        public bool? IsDevice
        {
            get => _internal.IsDevice;
            set => _internal.IsDevice = value;
        }

        public bool? IsMechanical
        {
            get => _internal.IsMechanical;
            set => _internal.IsMechanical = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public Data.Models.Metadata.Runnable? Runnable
        {
            get => _internal.Runnable;
            set => _internal.Runnable = value;
        }

        public Data.Models.Metadata.Supported? Supported
        {
            get => _internal.Supported;
            set => _internal.Supported = value;
        }

        #endregion

        #region Constructors

        public Machine()
        {
            _internal = [];
        }

        public Machine(Data.Models.Metadata.Machine machine)
        {
            _internal = machine.Clone() as Data.Models.Metadata.Machine ?? [];

            // Remove all inverted fields
            Remove(Data.Models.Metadata.Machine.AdjusterKey);
            Remove(Data.Models.Metadata.Machine.ArchiveKey);
            Remove(Data.Models.Metadata.Machine.BiosSetKey);
            Remove(Data.Models.Metadata.Machine.ChipKey);
            Remove(Data.Models.Metadata.Machine.ConfigurationKey);
            Remove(Data.Models.Metadata.Machine.DeviceKey);
            Remove(Data.Models.Metadata.Machine.DeviceRefKey);
            Remove(Data.Models.Metadata.Machine.DipSwitchKey);
            Remove(Data.Models.Metadata.Machine.DiskKey);
            Remove(Data.Models.Metadata.Machine.DisplayKey);
            Remove(Data.Models.Metadata.Machine.DriverKey);
            Remove(Data.Models.Metadata.Machine.DumpKey);
            Remove(Data.Models.Metadata.Machine.FeatureKey);
            Remove(Data.Models.Metadata.Machine.InfoKey);
            Remove(Data.Models.Metadata.Machine.InputKey);
            Remove(Data.Models.Metadata.Machine.MediaKey);
            Remove(Data.Models.Metadata.Machine.PartKey);
            Remove(Data.Models.Metadata.Machine.PortKey);
            Remove(Data.Models.Metadata.Machine.RamOptionKey);
            Remove(Data.Models.Metadata.Machine.ReleaseKey);
            Remove(Data.Models.Metadata.Machine.RomKey);
            Remove(Data.Models.Metadata.Machine.SampleKey);
            Remove(Data.Models.Metadata.Machine.SharedFeatKey);
            Remove(Data.Models.Metadata.Machine.SlotKey);
            Remove(Data.Models.Metadata.Machine.SoftwareListKey);
            Remove(Data.Models.Metadata.Machine.SoundKey);
            Remove(Data.Models.Metadata.Machine.TruripKey);
            Remove(Data.Models.Metadata.Machine.VideoKey);

            // Process flag values
            string? im1Crc = ReadString(Data.Models.Metadata.Machine.Im1CRCKey);
            if (im1Crc is not null)
                Write<string?>(Data.Models.Metadata.Machine.Im1CRCKey, TextHelper.NormalizeCRC32(im1Crc));

            string? im2Crc = ReadString(Data.Models.Metadata.Machine.Im2CRCKey);
            if (im2Crc is not null)
                Write<string?>(Data.Models.Metadata.Machine.Im2CRCKey, TextHelper.NormalizeCRC32(im2Crc));

            // Handle Trurip object, if it exists
            var truripItem = machine.Read<Data.Models.Logiqx.Trurip>(Data.Models.Metadata.Machine.TruripKey);
            if (truripItem is not null)
                Write(Data.Models.Metadata.Machine.TruripKey, new Trurip(truripItem));
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the name to use for a Machine
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public string? GetName() => Name;

        /// <summary>
        /// Sets the name to use for a Machine
        /// </summary>
        /// <param name="name">Name to set for the item</param>
        public void SetName(string? name) => Name = name;

        #endregion

        #region Cloning methods

        /// <summary>
        /// Create a clone of the current machine
        /// </summary>
        /// <returns>New machine with the same values as the current one</returns>
        public object Clone()
        {
            return new Machine()
            {
                _internal = _internal.Clone() as Data.Models.Metadata.Machine ?? [],
            };
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Data.Models.Metadata.Machine GetInternalClone() => (_internal.Clone() as Data.Models.Metadata.Machine)!;

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not Machine otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.Machine>? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not Machine otherItem)
                return false;

            // Compare internal models
            return _internal.EqualTo(otherItem._internal);
        }

        /// <inheritdoc/>
        public bool Equals(Machine? other)
        {
            // If other is null
            if (other is null)
                return false;

            // Compare internal models
            return _internal.EqualTo(other._internal);
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
