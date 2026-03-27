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
        #region Constructors

        public Machine()
        {
            _internal = [];
        }

        public Machine(Data.Models.Metadata.Machine machine)
        {
            // Get all fields to automatically copy without processing
            var nonItemFields = TypeHelper.GetConstants(typeof(Data.Models.Metadata.Machine));
            if (nonItemFields is null)
                return;

            // Populate the internal machine from non-filter fields
            _internal = [];
            foreach (string fieldName in nonItemFields)
            {
                if (machine.TryGetValue(fieldName, out var fieldValue))
                    _internal[fieldName] = fieldValue;
            }

            // Process flag values
            string? im1Crc = ReadString(Data.Models.Metadata.Machine.Im1CRCKey);
            if (im1Crc is not null)
                Write<string?>(Data.Models.Metadata.Machine.Im1CRCKey, TextHelper.NormalizeCRC32(im1Crc));

            string? im2Crc = ReadString(Data.Models.Metadata.Machine.Im2CRCKey);
            if (im2Crc is not null)
                Write<string?>(Data.Models.Metadata.Machine.Im2CRCKey, TextHelper.NormalizeCRC32(im2Crc));

            bool? isBios = ReadBool(Data.Models.Metadata.Machine.IsBiosKey);
            if (isBios is not null)
                Write<string?>(Data.Models.Metadata.Machine.IsBiosKey, isBios.FromYesNo());

            bool? isDevice = ReadBool(Data.Models.Metadata.Machine.IsDeviceKey);
            if (isDevice is not null)
                Write<string?>(Data.Models.Metadata.Machine.IsDeviceKey, isDevice.FromYesNo());

            bool? isMechanical = ReadBool(Data.Models.Metadata.Machine.IsMechanicalKey);
            if (isMechanical is not null)
                Write<string?>(Data.Models.Metadata.Machine.IsMechanicalKey, isMechanical.FromYesNo());

            string? supported = ReadString(Data.Models.Metadata.Machine.SupportedKey);
            if (supported is not null)
                Write<string?>(Data.Models.Metadata.Machine.SupportedKey, supported.AsSupported()?.AsStringValue());

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
        public string? GetName() => _internal.GetName();

        /// <summary>
        /// Sets the name to use for a Machine
        /// </summary>
        /// <param name="name">Name to set for the item</param>
        public void SetName(string? name) => _internal.SetName(name);

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
