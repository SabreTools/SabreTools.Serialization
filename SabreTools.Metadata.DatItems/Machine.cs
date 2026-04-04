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

        public string? Board
        {
            get => _internal.Board;
            set => _internal.Board = value;
        }

        public string? Buttons
        {
            get => _internal.Buttons;
            set => _internal.Buttons = value;
        }

        public string? CloneOf
        {
            get => _internal.CloneOf;
            set => _internal.CloneOf = value;
        }

        public string? CloneOfId
        {
            get => _internal.CloneOfId;
            set => _internal.CloneOfId = value;
        }

        public string? Company
        {
            get => _internal.Company;
            set => _internal.Company = value;
        }

        public string? Control
        {
            get => _internal.Control;
            set => _internal.Control = value;
        }

        public string? Country
        {
            get => _internal.Country;
            set => _internal.Country = value;
        }

        public string? Description
        {
            get => _internal.Description;
            set => _internal.Description = value;
        }

        public string? DirName
        {
            get => _internal.DirName;
            set => _internal.DirName = value;
        }

        public string? DisplayCount
        {
            get => _internal.DisplayCount;
            set => _internal.DisplayCount = value;
        }

        public string? DisplayType
        {
            get => _internal.DisplayType;
            set => _internal.DisplayType = value;
        }

        public string? DuplicateID
        {
            get => _internal.DuplicateID;
            set => _internal.DuplicateID = value;
        }

        public string? Emulator
        {
            get => _internal.Emulator;
            set => _internal.Emulator = value;
        }

        public string? Extra
        {
            get => _internal.Extra;
            set => _internal.Extra = value;
        }

        public string? Favorite
        {
            get => _internal.Favorite;
            set => _internal.Favorite = value;
        }

        public string? GenMSXID
        {
            get => _internal.GenMSXID;
            set => _internal.GenMSXID = value;
        }

        public string? Hash
        {
            get => _internal.Hash;
            set => _internal.Hash = value;
        }

        public string? History
        {
            get => _internal.History;
            set => _internal.History = value;
        }

        public string? Id
        {
            get => _internal.Id;
            set => _internal.Id = value;
        }

        public string? Im1CRC
        {
            get => _internal.Im1CRC;
            set => _internal.Im1CRC = value;
        }

        public string? Im2CRC
        {
            get => _internal.Im2CRC;
            set => _internal.Im2CRC = value;
        }

        public string? ImageNumber
        {
            get => _internal.ImageNumber;
            set => _internal.ImageNumber = value;
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

        public string? Language
        {
            get => _internal.Language;
            set => _internal.Language = value;
        }

        public string? Location
        {
            get => _internal.Location;
            set => _internal.Location = value;
        }

        public string? Manufacturer
        {
            get => _internal.Manufacturer;
            set => _internal.Manufacturer = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public string? Notes
        {
            get => _internal.Notes;
            set => _internal.Notes = value;
        }

        public string? PlayedCount
        {
            get => _internal.PlayedCount;
            set => _internal.PlayedCount = value;
        }

        public string? PlayedTime
        {
            get => _internal.PlayedTime;
            set => _internal.PlayedTime = value;
        }

        public string? Players
        {
            get => _internal.Players;
            set => _internal.Players = value;
        }

        public string? Publisher
        {
            get => _internal.Publisher;
            set => _internal.Publisher = value;
        }

        public string? RebuildTo
        {
            get => _internal.RebuildTo;
            set => _internal.RebuildTo = value;
        }

        public string? ReleaseNumber
        {
            get => _internal.ReleaseNumber;
            set => _internal.ReleaseNumber = value;
        }

        public string? RomOf
        {
            get => _internal.RomOf;
            set => _internal.RomOf = value;
        }

        public string? Rotation
        {
            get => _internal.Rotation;
            set => _internal.Rotation = value;
        }

        public Data.Models.Metadata.Runnable? Runnable
        {
            get => _internal.Runnable;
            set => _internal.Runnable = value;
        }

        public string? SampleOf
        {
            get => _internal.SampleOf;
            set => _internal.SampleOf = value;
        }

        public string? SaveType
        {
            get => _internal.SaveType;
            set => _internal.SaveType = value;
        }

        public string? SourceFile
        {
            get => _internal.SourceFile;
            set => _internal.SourceFile = value;
        }

        public string? SourceRom
        {
            get => _internal.SourceRom;
            set => _internal.SourceRom = value;
        }

        public string? Status
        {
            get => _internal.Status;
            set => _internal.Status = value;
        }

        public Data.Models.Metadata.Supported? Supported
        {
            get => _internal.Supported;
            set => _internal.Supported = value;
        }

        public string? System
        {
            get => _internal.System;
            set => _internal.System = value;
        }

        public string? Tags
        {
            get => _internal.Tags;
            set => _internal.Tags = value;
        }

        public string? Url
        {
            get => _internal.Url;
            set => _internal.Url = value;
        }

        public string? Year
        {
            get => _internal.Year;
            set => _internal.Year = value;
        }

        #endregion

        #region Constructors

        public Machine()
        {
            _internal = [];
        }

        public Machine(Data.Models.Metadata.Machine machine)
        {
            _internal = machine.DeepClone() as Data.Models.Metadata.Machine ?? [];

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
            if (Im1CRC is not null)
                Im1CRC = TextHelper.NormalizeCRC32(Im1CRC);

            if (Im2CRC is not null)
                Im2CRC = TextHelper.NormalizeCRC32(Im2CRC);

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
                _internal = _internal.DeepClone() as Data.Models.Metadata.Machine ?? [],
            };
        }

        /// <summary>
        /// Get a clone of the current internal model
        /// </summary>
        public Data.Models.Metadata.Machine GetInternalClone() => (_internal.DeepClone() as Data.Models.Metadata.Machine)!;

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

            // Properties
            if ((Description is null) ^ (other.Description is null))
                return false;
            if (Description is not null && !Description.Equals(other.Description, StringComparison.OrdinalIgnoreCase))
                return false;

            if (IsBios != other.IsBios)
                return false;

            if (IsDevice != other.IsDevice)
                return false;

            if (IsMechanical != other.IsMechanical)
                return false;

            if ((Name is null) ^ (other.Name is null))
                return false;
            if (Name is not null && !Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Runnable != other.Runnable)
                return false;

            if (Supported != other.Supported)
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
