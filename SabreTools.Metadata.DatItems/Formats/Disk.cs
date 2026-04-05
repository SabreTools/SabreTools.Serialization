using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;
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

        public DiskArea? DiskArea { get; set; }

        [JsonIgnore]
        public bool DiskAreaSpecified
            => DiskArea is not null && !string.IsNullOrEmpty(DiskArea.Name);

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

        public Part? Part { get; set; }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                return Part is not null
                    && (!string.IsNullOrEmpty(Part.Name)
                        || !string.IsNullOrEmpty(Part.Interface));
            }
        }

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
            if (DiskArea is not null)
            {
                var dataArea = new DataArea { Name = DiskArea.Name };
                rom.DataArea = dataArea;
            }

            rom.DupeType = DupeType;
            rom.Machine = Machine?.Clone() as Machine;
            rom.Part = Part?.Clone() as Part;
            rom.RemoveFlag = RemoveFlag;
            rom.Source = Source?.Clone() as Source;

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

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Disk>? other)
        {
            // If the other value is invalid
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
