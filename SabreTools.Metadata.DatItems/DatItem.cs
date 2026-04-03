using System;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Hashing;
using SabreTools.Logging;
using SabreTools.Metadata.DatItems.Formats;
using SabreTools.Metadata.Filter;

namespace SabreTools.Metadata.DatItems
{
    /// <summary>
    /// Base class for all items included in a set
    /// </summary>
    [JsonObject("datitem"), XmlRoot("datitem")]
    [XmlInclude(typeof(Adjuster))]
    [XmlInclude(typeof(Analog))]
    [XmlInclude(typeof(Archive))]
    [XmlInclude(typeof(BiosSet))]
    [XmlInclude(typeof(Blank))]
    [XmlInclude(typeof(Chip))]
    [XmlInclude(typeof(Condition))]
    [XmlInclude(typeof(Configuration))]
    [XmlInclude(typeof(ConfLocation))]
    [XmlInclude(typeof(ConfSetting))]
    [XmlInclude(typeof(Control))]
    [XmlInclude(typeof(DataArea))]
    [XmlInclude(typeof(Device))]
    [XmlInclude(typeof(DeviceRef))]
    [XmlInclude(typeof(DipLocation))]
    [XmlInclude(typeof(DipSwitch))]
    [XmlInclude(typeof(DipValue))]
    [XmlInclude(typeof(Disk))]
    [XmlInclude(typeof(DiskArea))]
    [XmlInclude(typeof(Display))]
    [XmlInclude(typeof(Driver))]
    [XmlInclude(typeof(Extension))]
    [XmlInclude(typeof(Feature))]
    [XmlInclude(typeof(Info))]
    [XmlInclude(typeof(Input))]
    [XmlInclude(typeof(Instance))]
    [XmlInclude(typeof(Media))]
    [XmlInclude(typeof(Part))]
    [XmlInclude(typeof(PartFeature))]
    [XmlInclude(typeof(Port))]
    [XmlInclude(typeof(RamOption))]
    [XmlInclude(typeof(Release))]
    [XmlInclude(typeof(ReleaseDetails))]
    [XmlInclude(typeof(Rom))]
    [XmlInclude(typeof(Sample))]
    [XmlInclude(typeof(SharedFeat))]
    [XmlInclude(typeof(Slot))]
    [XmlInclude(typeof(SlotOption))]
    [XmlInclude(typeof(SoftwareList))]
    [XmlInclude(typeof(Sound))]
    [XmlInclude(typeof(SourceDetails))]
    public abstract class DatItem : ModelBackedItem<Data.Models.Metadata.DatItem>, IEquatable<DatItem>, IComparable<DatItem>, ICloneable
    {
        #region Constants

        /// <summary>
        /// Duplicate type when compared to another item
        /// </summary>
        public const string DupeTypeKey = "DUPETYPE";

        #endregion

        #region Fields

        /// <summary>
        /// Item type for the object
        /// </summary>
        public abstract Data.Models.Metadata.ItemType ItemType { get; }

        /// <summary>
        /// Get the machine for a DatItem
        /// </summary>
        public Machine? Machine { get; set; }

        /// <summary>
        /// Flag if item should be removed
        /// </summary>
        public bool RemoveFlag { get; set; } = false;

        /// <summary>
        /// Source information
        /// </summary>
        public Source? Source { get; set; }

        #endregion

        #region Logging

        /// <summary>
        /// Static logger for static methods
        /// </summary>
        [JsonIgnore, XmlIgnore]
        protected static readonly Logger _staticLogger = new();

        #endregion

        #region Accessors

        /// <summary>
        /// Gets the name to use for a DatItem
        /// </summary>
        /// <returns>Name if available, null otherwise</returns>
        public virtual string? GetName() => _internal.GetName();

        /// <summary>
        /// Sets the name to use for a DatItem
        /// </summary>
        /// <param name="name">Name to set for the item</param>
        public virtual void SetName(string? name) => _internal.SetName(name);

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Clone the DatItem
        /// </summary>
        /// <returns>Clone of the DatItem</returns>
        public abstract object Clone();

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="item">Existing item to copy information from</param>
        public void CopyMachineInformation(DatItem item)
        {
            // If there is no machine
            if (item.Machine is null)
                return;

            CopyMachineInformation(item.Machine);
        }

        /// <summary>
        /// Copy all machine information over in one shot
        /// </summary>
        /// <param name="machine">Existing machine to copy information from</param>
        public void CopyMachineInformation(Machine? machine)
        {
            if (machine is null)
                return;

            if (machine.Clone() is Machine cloned)
                Machine = cloned;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public int CompareTo(DatItem? other)
        {
            // If the other item doesn't exist
            if (other is null)
                return 1;

            // Get the names to avoid changing values
            string? selfName = GetName();
            string? otherName = other.GetName();

            // If the names are equal
            if (selfName == otherName)
                return Equals(other) ? 0 : 1;

            // If `otherName` is null, Compare will return > 0
            // If `selfName` is null, Compare will return < 0
            return string.Compare(selfName, otherName, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not DatItem otherItem)
                return false;

            // Compare internal models
            return _internal.Equals(otherItem);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.DatItem>? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not DatItem otherItem)
                return false;

            // Compare internal models
            return _internal.Equals(otherItem);
        }

        /// <summary>
        /// Determine if an item is a duplicate using partial matching logic
        /// </summary>
        /// <param name="other">DatItem to use as a baseline</param>
        /// <returns>True if the items are duplicates, false otherwise</returns>
        public virtual bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If we don't have a matched type, return false
            if (ItemType != other.ItemType)
                return false;

            // Compare the internal models
            return _internal.EqualTo(other._internal);
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the item and its machine passes the filter, false otherwise</returns>
        public bool PassesFilter(FilterRunner filterRunner)
        {
            var machine = Machine;
            if (machine is not null && !machine.PassesFilter(filterRunner))
                return false;

            return filterRunner.Run(_internal);
        }

        /// <summary>
        /// Runs a filter and determines if it passes or not
        /// </summary>
        /// <param name="filterRunner">Filter runner to use for checking</param>
        /// <returns>True if the item passes the filter, false otherwise</returns>
        public bool PassesFilterDB(FilterRunner filterRunner)
            => filterRunner.Run(_internal);

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Get the dictionary key that should be used for a given item and bucketing type
        /// </summary>
        /// <param name="bucketedBy">ItemKey value representing what key to get</param>
        /// <param name="machine">Machine associated with the item for renaming</param>
        /// <param name="source">Source associated with the item for renaming</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns>String representing the key to be used for the DatItem</returns>
        public virtual string GetKey(ItemKey bucketedBy, Machine? machine, Source? source, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string key = string.Empty;

            string sourceKeyPadded = source?.Index.ToString().PadLeft(10, '0') + '-';
            string machineName = machine?.GetName() ?? "Default";

            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case ItemKey.CRC16:
                    key = HashType.CRC16.ZeroString;
                    break;

                case ItemKey.CRC:
                    key = HashType.CRC32.ZeroString;
                    break;

                case ItemKey.CRC64:
                    key = HashType.CRC64.ZeroString;
                    break;

                case ItemKey.Machine:
                    key = (norename ? string.Empty : sourceKeyPadded) + machineName;
                    break;

                case ItemKey.MD2:
                    key = HashType.MD2.ZeroString;
                    break;

                case ItemKey.MD4:
                    key = HashType.MD4.ZeroString;
                    break;

                case ItemKey.MD5:
                    key = HashType.MD5.ZeroString;
                    break;

                case ItemKey.RIPEMD128:
                    key = HashType.RIPEMD128.ZeroString;
                    break;

                case ItemKey.RIPEMD160:
                    key = HashType.RIPEMD160.ZeroString;
                    break;

                case ItemKey.SHA1:
                    key = HashType.SHA1.ZeroString;
                    break;

                case ItemKey.SHA256:
                    key = HashType.SHA256.ZeroString;
                    break;

                case ItemKey.SHA384:
                    key = HashType.SHA384.ZeroString;
                    break;

                case ItemKey.SHA512:
                    key = HashType.SHA512.ZeroString;
                    break;

                case ItemKey.SpamSum:
                    key = HashType.SpamSum.ZeroString;
                    break;

                case ItemKey.NULL:
                default:
                    // This should never happen
                    break;
            }

            // Double and triple check the key for corner cases
            key ??= string.Empty;
            if (lower)
                key = key.ToLowerInvariant();

            return key;
        }

        #endregion
    }
}
