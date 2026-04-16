using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;
using SabreTools.Metadata.Filter;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents Compressed Hunks of Data (CHD) formatted disks which use internal hashes
    /// </summary>
    [JsonObject("disk"), XmlRoot("disk")]
    public sealed class Disk : DatItem<Data.Models.Metadata.Disk>
    {
        #region Properties

        public string? DiskAreaName { get; set; }

        public string? Flags
        {
            get => _internal.Flags;
            set => _internal.Flags = value;
        }

        public long? Index
        {
            get => _internal.Index;
            set => _internal.Index = value;
        }

        /// <inheritdoc>/>
        public override ItemType ItemType => ItemType.Disk;

        public string? MD5
        {
            get => _internal.MD5;
            set => _internal.MD5 = value;
        }

        public string? Merge
        {
            get => _internal.Merge;
            set => _internal.Merge = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public bool? Optional
        {
            get => _internal.Optional;
            set => _internal.Optional = value;
        }

        public string? PartInterface { get; set; }

        public string? PartName { get; set; }

        public string? Region
        {
            get => _internal.Region;
            set => _internal.Region = value;
        }

        public string? SHA1
        {
            get => _internal.SHA1;
            set => _internal.SHA1 = value;
        }

        public ItemStatus? Status
        {
            get => _internal.Status;
            set => _internal.Status = value;
        }

        public bool? Writable
        {
            get => _internal.Writable;
            set => _internal.Writable = value;
        }

        #endregion

        #region Constructors

        public Disk() : base()
        {
            DupeType = 0x00;
            Status = ItemStatus.None;
        }

        public Disk(Data.Models.Metadata.Disk item) : base(item)
        {
            DupeType = 0x00;

            // Process hash values
            if (MD5 is not null)
                MD5 = TextHelper.NormalizeMD5(MD5);

            if (SHA1 is not null)
                SHA1 = TextHelper.NormalizeSHA1(SHA1);
        }

        public Disk(Data.Models.Metadata.Disk item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        public Disk(Data.Models.Metadata.Disk item, long machineIndex, long sourceIndex) : this(item)
        {
            SourceIndex = sourceIndex;
            MachineIndex = machineIndex;
        }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Disk(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Disk GetInternalClone()
            => _internal.Clone() as Data.Models.Metadata.Disk ?? new();

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom()!);

            // Create a DataArea if there was an existing DiskArea
            if (DiskAreaName is not null)
            {
                rom.DataAreaEndianness = Endianness.Little;
                rom.DataAreaName = DiskAreaName;
            }

            rom.DupeType = DupeType;
            rom.Machine = Machine?.Clone() as Machine;
            rom.MachineIndex = MachineIndex;
            rom.PartInterface = PartInterface;
            rom.PartName = PartName;
            rom.RemoveFlag = RemoveFlag;
            rom.Source = Source?.Clone() as Source;
            rom.SourceIndex = SourceIndex;

            return rom;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Disk otherDisk)
                return _internal.PartialEquals(otherDisk._internal);

            // Everything else fails
            return false;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Disk
        /// </summary>
        /// <param name="other">Disk to fill information from</param>
        public void FillMissingInformation(Disk other)
            => _internal.FillMissingHashes(other._internal);

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes() => _internal.HasHashes();

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash() => _internal.HasZeroHash();

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool PassesFilter(FilterRunner filterRunner)
        {
            if (Machine is not null && !Machine.PassesFilter(filterRunner))
                return false;

            // TODO: DiskArea
            // TODO: Part

            return filterRunner.Run(_internal);
        }

        /// <inheritdoc/>
        public override bool PassesFilterDB(FilterRunner filterRunner)
        {
            // TODO: DiskArea
            // TODO: Part

            return filterRunner.Run(_internal);
        }

        #endregion

        #region Sorting and Merging

        /// <inheritdoc/>
        public override string GetKey(ItemKey bucketedBy, Machine? machine, Source? source, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string? key;

#pragma warning disable IDE0010
            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case ItemKey.MD5:
                    key = MD5;
                    break;

                case ItemKey.SHA1:
                    key = SHA1;
                    break;

                // Let the base handle generic stuff
                default:
                    return base.GetKey(bucketedBy, machine, source, lower, norename);
            }
#pragma warning restore IDE0010

            // Double and triple check the key for corner cases
            key ??= string.Empty;
            if (lower)
                key = key.ToLowerInvariant();

            return key;
        }

        #endregion
    }
}
