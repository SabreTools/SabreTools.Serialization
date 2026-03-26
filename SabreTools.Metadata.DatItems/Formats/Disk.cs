using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents Compressed Hunks of Data (CHD) formatted disks which use internal hashes
    /// </summary>
    [JsonObject("disk"), XmlRoot("disk")]
    public sealed class Disk : DatItem<Data.Models.Metadata.Disk>
    {
        #region Constants

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string DiskAreaKey = "DISKAREA";

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string PartKey = "PART";

        #endregion

        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Disk;

        [JsonIgnore]
        public bool DiskAreaSpecified
        {
            get
            {
                var diskArea = Read<DiskArea?>(DiskAreaKey);
                return diskArea is not null && !string.IsNullOrEmpty(diskArea.GetName());
            }
        }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                var part = Read<Part?>(PartKey);
                return part is not null
                    && (!string.IsNullOrEmpty(part.GetName())
                        || !string.IsNullOrEmpty(part.ReadString(Data.Models.Metadata.Part.InterfaceKey)));
            }
        }

        #endregion

        #region Constructors

        public Disk() : base()
        {
            Write<DupeType>(DupeTypeKey, 0x00);
            Write<string?>(Data.Models.Metadata.Disk.StatusKey, ItemStatus.None.AsStringValue());
        }

        public Disk(Data.Models.Metadata.Disk item) : base(item)
        {
            Write<DupeType>(DupeTypeKey, 0x00);

            // Process flag values
            if (ReadBool(Data.Models.Metadata.Disk.OptionalKey) is not null)
                Write<string?>(Data.Models.Metadata.Disk.OptionalKey, ReadBool(Data.Models.Metadata.Disk.OptionalKey).FromYesNo());
            if (ReadString(Data.Models.Metadata.Disk.StatusKey) is not null)
                Write<string?>(Data.Models.Metadata.Disk.StatusKey, ReadString(Data.Models.Metadata.Disk.StatusKey).AsItemStatus().AsStringValue());
            if (ReadBool(Data.Models.Metadata.Disk.WritableKey) is not null)
                Write<string?>(Data.Models.Metadata.Disk.WritableKey, ReadBool(Data.Models.Metadata.Disk.WritableKey).FromYesNo());

            // Process hash values
            if (ReadString(Data.Models.Metadata.Disk.MD5Key) is not null)
                Write<string?>(Data.Models.Metadata.Disk.MD5Key, TextHelper.NormalizeMD5(ReadString(Data.Models.Metadata.Disk.MD5Key)));
            if (ReadString(Data.Models.Metadata.Disk.SHA1Key) is not null)
                Write<string?>(Data.Models.Metadata.Disk.SHA1Key, TextHelper.NormalizeSHA1(ReadString(Data.Models.Metadata.Disk.SHA1Key)));
        }

        public Disk(Data.Models.Metadata.Disk item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <summary>
        /// Convert a disk to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom()!);

            // Create a DataArea if there was an existing DiskArea
            var diskArea = Read<DiskArea?>(DiskAreaKey);
            if (diskArea is not null)
            {
                var dataArea = new DataArea();

                string? diskAreaName = diskArea.ReadString(Data.Models.Metadata.DiskArea.NameKey);
                dataArea.Write(Data.Models.Metadata.DataArea.NameKey, diskAreaName);

                rom.Write<DataArea?>(Rom.DataAreaKey, dataArea);
            }

            rom.Write(DupeTypeKey, Read<DupeType>(DupeTypeKey));
            rom.Write(MachineKey, GetMachine()?.Clone() as Machine);
            rom.Write(Rom.PartKey, Read<Part>(PartKey)?.Clone() as Part);
            rom.Write(RemoveKey, ReadBool(RemoveKey));
            rom.Write<Source?>(SourceKey, Read<Source?>(SourceKey)?.Clone() as Source);

            return rom;
        }

        #endregion

        #region Comparision Methods

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
                    key = ReadString(Data.Models.Metadata.Disk.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = ReadString(Data.Models.Metadata.Disk.SHA1Key);
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
