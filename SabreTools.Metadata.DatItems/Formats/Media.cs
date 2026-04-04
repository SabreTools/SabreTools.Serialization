using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents Aaruformat images which use internal hashes
    /// </summary>
    [JsonObject("media"), XmlRoot("media")]
    public sealed class Media : DatItem<Data.Models.Metadata.Media>
    {
        #region Fields

        public string? MD5
        {
            get => (_internal as Data.Models.Metadata.Media)?.MD5;
            set => (_internal as Data.Models.Metadata.Media)?.MD5 = value;
        }

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType
            => Data.Models.Metadata.ItemType.Media;

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Media)?.Name;
            set => (_internal as Data.Models.Metadata.Media)?.Name = value;
        }

        public string? SHA1
        {
            get => (_internal as Data.Models.Metadata.Media)?.SHA1;
            set => (_internal as Data.Models.Metadata.Media)?.SHA1 = value;
        }

        public string? SHA256
        {
            get => (_internal as Data.Models.Metadata.Media)?.SHA256;
            set => (_internal as Data.Models.Metadata.Media)?.SHA256 = value;
        }

        public string? SpamSum
        {
            get => (_internal as Data.Models.Metadata.Media)?.SpamSum;
            set => (_internal as Data.Models.Metadata.Media)?.SpamSum = value;
        }

        #endregion

        #region Constructors

        public Media() : base()
        {
            DupeType = 0x00;
        }

        public Media(Data.Models.Metadata.Media item) : base(item)
        {
            DupeType = 0x00;

            // Process hash values
            if (MD5 is not null)
                MD5 = TextHelper.NormalizeMD5(MD5);

            if (SHA1 is not null)
                SHA1 = TextHelper.NormalizeSHA1(SHA1);

            if (SHA256 is not null)
                SHA256 = TextHelper.NormalizeSHA256(SHA256);
        }

        public Media(Data.Models.Metadata.Media item, Machine machine, Source source) : this(item)
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
        public override object Clone() => new Media(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Media GetInternalClone()
            => (_internal as Data.Models.Metadata.Media)?.Clone() as Data.Models.Metadata.Media ?? [];

        /// <summary>
        /// Convert a media to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom((_internal as Data.Models.Metadata.Media).ConvertToRom()!);

            rom.DupeType = DupeType;
            rom.Machine = Machine?.Clone() as Machine;
            rom.RemoveFlag = RemoveFlag;
            rom.Source = Source?.Clone() as Source;

            return rom;
        }

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Fill any missing size and hash information from another Media
        /// </summary>
        /// <param name="other">Media to fill information from</param>
        public void FillMissingInformation(Media other)
            => (_internal as Data.Models.Metadata.Media).FillMissingHashes(other._internal as Data.Models.Metadata.Media);

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes() => (_internal as Data.Models.Metadata.Media)?.HasHashes() ?? false;

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash() => (_internal as Data.Models.Metadata.Media)?.HasZeroHash() ?? false;

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

                case ItemKey.SHA256:
                    key = SHA256;
                    break;

                case ItemKey.SpamSum:
                    key = SpamSum;
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
