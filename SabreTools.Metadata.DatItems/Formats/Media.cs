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

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Media)?.Name;
            set => (_internal as Data.Models.Metadata.Media)?.Name = value;
        }

        #endregion

        #region Constructors

        public Media() : base()
        {
            Write<DupeType>(DupeTypeKey, 0x00);
        }

        public Media(Data.Models.Metadata.Media item) : base(item)
        {
            Write<DupeType>(DupeTypeKey, 0x00);

            // Process hash values
            string? md5 = ReadString(Data.Models.Metadata.Media.MD5Key);
            if (md5 is not null)
                Write<string?>(Data.Models.Metadata.Media.MD5Key, TextHelper.NormalizeMD5(md5));

            string? sha1 = ReadString(Data.Models.Metadata.Media.SHA1Key);
            if (sha1 is not null)
                Write<string?>(Data.Models.Metadata.Media.SHA1Key, TextHelper.NormalizeSHA1(sha1));

            string? sha256 = ReadString(Data.Models.Metadata.Media.SHA256Key);
            if (sha256 is not null)
                Write<string?>(Data.Models.Metadata.Media.SHA256Key, TextHelper.NormalizeSHA256(sha256));
        }

        public Media(Data.Models.Metadata.Media item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
            CopyMachineInformation(machine);
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Media(_internal.DeepClone() as Data.Models.Metadata.Media ?? []);

        /// <summary>
        /// Convert a media to the closest Rom approximation
        /// </summary>
        /// <returns></returns>
        public Rom ConvertToRom()
        {
            var rom = new Rom(_internal.ConvertToRom()!);

            rom.Write(DupeTypeKey, Read<DupeType>(DupeTypeKey));
            rom.Write(MachineKey, GetMachine());
            rom.Write(RemoveKey, ReadBool(RemoveKey));
            rom.Write<Source?>(SourceKey, Read<Source?>(SourceKey));

            return rom;
        }

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Fill any missing size and hash information from another Media
        /// </summary>
        /// <param name="other">Media to fill information from</param>
        public void FillMissingInformation(Media other)
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
                    key = ReadString(Data.Models.Metadata.Media.MD5Key);
                    break;

                case ItemKey.SHA1:
                    key = ReadString(Data.Models.Metadata.Media.SHA1Key);
                    break;

                case ItemKey.SHA256:
                    key = ReadString(Data.Models.Metadata.Media.SHA256Key);
                    break;

                case ItemKey.SpamSum:
                    key = ReadString(Data.Models.Metadata.Media.SpamSumKey);
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
