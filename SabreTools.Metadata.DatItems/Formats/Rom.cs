using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
    [JsonObject("rom"), XmlRoot("rom")]
    public sealed class Rom : DatItem<Data.Models.Metadata.Rom>
    {
        #region Fields

        public DataArea? DataArea { get; set; }

        [JsonIgnore]
        public bool DataAreaSpecified
        {
            get
            {
                return DataArea is not null
                    && (!string.IsNullOrEmpty(DataArea.Name)
                        || DataArea.Size is not null
                        || DataArea.Width is not null
                        || DataArea.Endianness is not null);
            }
        }

        public bool? Dispose
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Dispose;
            set => (_internal as Data.Models.Metadata.Rom)?.Dispose = value;
        }

        public long? FileCount
        {
            get => (_internal as Data.Models.Metadata.Rom)?.FileCount;
            set => (_internal as Data.Models.Metadata.Rom)?.FileCount = value;
        }

        public bool? FileIsAvailable
        {
            get => (_internal as Data.Models.Metadata.Rom)?.FileIsAvailable;
            set => (_internal as Data.Models.Metadata.Rom)?.FileIsAvailable = value;
        }

        public bool? Inverted
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Inverted;
            set => (_internal as Data.Models.Metadata.Rom)?.Inverted = value;
        }

        /// <inheritdoc>/>
        public override ItemType ItemType => ItemType.Rom;

        public LoadFlag? LoadFlag
        {
            get => (_internal as Data.Models.Metadata.Rom)?.LoadFlag;
            set => (_internal as Data.Models.Metadata.Rom)?.LoadFlag = value;
        }

        public bool? MIA
        {
            get => (_internal as Data.Models.Metadata.Rom)?.MIA;
            set => (_internal as Data.Models.Metadata.Rom)?.MIA = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Name;
            set => (_internal as Data.Models.Metadata.Rom)?.Name = value;
        }

        public OpenMSXSubType? OpenMSXMediaType
        {
            get => (_internal as Data.Models.Metadata.Rom)?.OpenMSXMediaType;
            set => (_internal as Data.Models.Metadata.Rom)?.OpenMSXMediaType = value;
        }

        public bool? Optional
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Optional;
            set => (_internal as Data.Models.Metadata.Rom)?.Optional = value;
        }

        [JsonIgnore]
        public bool OriginalSpecified
        {
            get
            {
                var original = Read<Original?>("ORIGINAL");
                return original is not null && original != default;
            }
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

        public long? Size
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Size;
            set => (_internal as Data.Models.Metadata.Rom)?.Size = value;
        }

        public bool? SoundOnly
        {
            get => (_internal as Data.Models.Metadata.Rom)?.SoundOnly;
            set => (_internal as Data.Models.Metadata.Rom)?.SoundOnly = value;
        }

        public ItemStatus? Status
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Status;
            set => (_internal as Data.Models.Metadata.Rom)?.Status = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Value;
            set => (_internal as Data.Models.Metadata.Rom)?.Value = value;
        }

        #endregion

        #region Constructors

        public Rom() : base()
        {
            DupeType = 0x00;
            Status = ItemStatus.None;
        }

        public Rom(Dump item, Machine machine, Source source, int index)
        {
            // If we don't have rom data, we can't do anything
            Data.Models.Metadata.Rom? rom = null;
            OpenMSXSubType? subType = null;

            if (item.Read<Data.Models.Metadata.Rom>(Dump.RomKey) is not null)
            {
                rom = item.Read<Data.Models.Metadata.Rom>(Dump.RomKey);
                subType = OpenMSXSubType.Rom;
            }
            else if (item.Read<Data.Models.Metadata.Rom>(Dump.MegaRomKey) is not null)
            {
                rom = item.Read<Data.Models.Metadata.Rom>(Dump.MegaRomKey);
                subType = OpenMSXSubType.MegaRom;
            }
            else if (item.Read<Data.Models.Metadata.Rom>(Dump.SCCPlusCartKey) is not null)
            {
                rom = item.Read<Data.Models.Metadata.Rom>(Dump.SCCPlusCartKey);
                subType = OpenMSXSubType.SCCPlusCart;
            }

            // Just return if nothing valid was found
            if (rom is null)
                return;

            string name = $"{machine.Name}_{index++}{(!string.IsNullOrEmpty(rom!.ReadString(Data.Models.Metadata.Rom.RemarkKey)) ? $" {rom.ReadString(Data.Models.Metadata.Rom.RemarkKey)}" : string.Empty)}";

            Name = name;
            Write<string?>(Data.Models.Metadata.Rom.OffsetKey, rom.ReadString(Data.Models.Metadata.Rom.StartKey));
            OpenMSXMediaType = subType;
            Write<string?>(Data.Models.Metadata.Rom.OpenMSXType, rom.ReadString(Data.Models.Metadata.Rom.OpenMSXType) ?? rom.ItemType.ToString());
            Write<string?>(Data.Models.Metadata.Rom.RemarkKey, rom.ReadString(Data.Models.Metadata.Rom.RemarkKey));
            Write<string?>(Data.Models.Metadata.Rom.SHA1Key, rom.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Write<string?>(Data.Models.Metadata.Rom.StartKey, rom.ReadString(Data.Models.Metadata.Rom.StartKey));
            Source = source;

            var original = item.Read<Data.Models.Metadata.Original>(Dump.OriginalKey);
            if (original is not null)
            {
                Write<Original?>("ORIGINAL", new Original
                {
                    Value = original.Value,
                    Content = original.Content,
                });
            }

            CopyMachineInformation(machine);

            // Process hash values
            // TODO: This should be normalized to CRC-16
            string? crc16 = ReadString(Data.Models.Metadata.Rom.CRC16Key);
            if (crc16 is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC16Key, NormalizeHashData(crc16, 4));

            string? crc = ReadString(Data.Models.Metadata.Rom.CRCKey);
            if (crc is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(crc));

            // TODO: This should be normalized to CRC-64
            string? crc64 = ReadString(Data.Models.Metadata.Rom.CRC64Key);
            if (crc64 is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC64Key, NormalizeHashData(crc64, 16));

            string? md2 = ReadString(Data.Models.Metadata.Rom.MD2Key);
            if (md2 is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD2Key, TextHelper.NormalizeMD2(md2));

            string? md4 = ReadString(Data.Models.Metadata.Rom.MD4Key);
            if (md4 is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD4Key, TextHelper.NormalizeMD5(md4));

            string? md5 = ReadString(Data.Models.Metadata.Rom.MD5Key);
            if (md5 is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(md5));

            string? ripemd128 = ReadString(Data.Models.Metadata.Rom.RIPEMD128Key);
            if (ripemd128 is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD128Key, TextHelper.NormalizeRIPEMD128(ripemd128));

            string? ripemd160 = ReadString(Data.Models.Metadata.Rom.RIPEMD160Key);
            if (ripemd160 is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD160Key, TextHelper.NormalizeRIPEMD160(ripemd160));

            string? sha1 = ReadString(Data.Models.Metadata.Rom.SHA1Key);
            if (sha1 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(sha1));

            string? sha256 = ReadString(Data.Models.Metadata.Rom.SHA256Key);
            if (sha256 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(sha256));

            string? sha384 = ReadString(Data.Models.Metadata.Rom.SHA384Key);
            if (sha384 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(sha384));

            string? sha512 = ReadString(Data.Models.Metadata.Rom.SHA512Key);
            if (sha512 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(sha512));
        }

        public Rom(Data.Models.Metadata.Rom item) : base(item)
        {
            DupeType = 0x00;

            // Process hash values
            // TODO: This should be normalized to CRC-16
            string? crc16 = ReadString(Data.Models.Metadata.Rom.CRC16Key);
            if (crc16 is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC16Key, NormalizeHashData(crc16, 4));

            string? crc = ReadString(Data.Models.Metadata.Rom.CRCKey);
            if (crc is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(crc));

            // TODO: This should be normalized to CRC-64
            string? crc64 = ReadString(Data.Models.Metadata.Rom.CRC64Key);
            if (crc64 is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC64Key, NormalizeHashData(crc64, 16));

            string? md2 = ReadString(Data.Models.Metadata.Rom.MD2Key);
            if (md2 is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD2Key, TextHelper.NormalizeMD2(md2));

            string? md4 = ReadString(Data.Models.Metadata.Rom.MD4Key);
            if (md4 is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD4Key, TextHelper.NormalizeMD5(md4));

            string? md5 = ReadString(Data.Models.Metadata.Rom.MD5Key);
            if (md5 is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(md5));

            string? ripemd128 = ReadString(Data.Models.Metadata.Rom.RIPEMD128Key);
            if (ripemd128 is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD128Key, TextHelper.NormalizeRIPEMD128(ripemd128));

            string? ripemd160 = ReadString(Data.Models.Metadata.Rom.RIPEMD160Key);
            if (ripemd160 is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD160Key, TextHelper.NormalizeRIPEMD160(ripemd160));

            string? sha1 = ReadString(Data.Models.Metadata.Rom.SHA1Key);
            if (sha1 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(sha1));

            string? sha256 = ReadString(Data.Models.Metadata.Rom.SHA256Key);
            if (sha256 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(sha256));

            string? sha384 = ReadString(Data.Models.Metadata.Rom.SHA384Key);
            if (sha384 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(sha384));

            string? sha512 = ReadString(Data.Models.Metadata.Rom.SHA512Key);
            if (sha512 is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(sha512));
        }

        public Rom(Data.Models.Metadata.Rom item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        /// <summary>
        /// Normalize a hash string and pad to the correct size
        /// </summary>
        /// TODO: Remove when IO is updated
        private static string? NormalizeHashData(string? hash, int expectedLength)
        {
            // If we have a known blank hash, return blank
            if (hash is null)
                return null;
            else if (hash == string.Empty || hash == "-" || hash == "_")
                return string.Empty;

            // Check to see if it's a "hex" hash
            hash = hash!.Trim().Replace("0x", string.Empty);

            // If we have a blank hash now, return blank
            if (string.IsNullOrEmpty(hash))
                return string.Empty;

            // If the hash shorter than the required length, pad it
            if (hash.Length < expectedLength)
                hash = hash.PadLeft(expectedLength, '0');

            // If the hash is longer than the required length, it's invalid
            else if (hash.Length > expectedLength)
                return string.Empty;

            // Now normalize the hash
            hash = hash.ToLowerInvariant();

            // Otherwise, make sure that every character is a proper match
            for (int i = 0; i < hash.Length; i++)
            {
                char c = hash[i];
#if NET7_0_OR_GREATER
                if (!char.IsAsciiHexDigit(c))
#else
                if (!IsAsciiHexDigit(c))
#endif
                {
                    hash = string.Empty;
                    break;
                }
            }

            return hash;
        }

#if NETFRAMEWORK || NETCOREAPP3_1 || NET5_0 || NET6_0 || NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Indicates whether a character is categorized as an ASCII hexademical digit.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>true if c is a hexademical digit; otherwise, false.</returns>
        /// <remarks>This method determines whether the character is in the range '0' through '9', inclusive, 'A' through 'F', inclusive, or 'a' through 'f', inclusive.</remarks>
        /// TODO: Remove when IO is updated
        internal static bool IsAsciiHexDigit(char c)
        {
            return char.ToLowerInvariant(c) switch
            {
                '0' => true,
                '1' => true,
                '2' => true,
                '3' => true,
                '4' => true,
                '5' => true,
                '6' => true,
                '7' => true,
                '8' => true,
                '9' => true,
                'a' => true,
                'b' => true,
                'c' => true,
                'd' => true,
                'e' => true,
                'f' => true,
                _ => false,
            };
        }
#endif

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Rom(_internal.DeepClone() as Data.Models.Metadata.Rom ?? []);

        #endregion

        #region Comparision Methods

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other)
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
                case ItemKey.CRC16:
                    key = ReadString(Data.Models.Metadata.Rom.CRC16Key);
                    break;

                case ItemKey.CRC:
                    key = ReadString(Data.Models.Metadata.Rom.CRCKey);
                    break;

                case ItemKey.CRC64:
                    key = ReadString(Data.Models.Metadata.Rom.CRC64Key);
                    break;

                case ItemKey.MD2:
                    key = ReadString(Data.Models.Metadata.Rom.MD2Key);
                    break;

                case ItemKey.MD4:
                    key = ReadString(Data.Models.Metadata.Rom.MD4Key);
                    break;

                case ItemKey.MD5:
                    key = ReadString(Data.Models.Metadata.Rom.MD5Key);
                    break;

                case ItemKey.RIPEMD128:
                    key = ReadString(Data.Models.Metadata.Rom.RIPEMD128Key);
                    break;

                case ItemKey.RIPEMD160:
                    key = ReadString(Data.Models.Metadata.Rom.RIPEMD160Key);
                    break;

                case ItemKey.SHA1:
                    key = ReadString(Data.Models.Metadata.Rom.SHA1Key);
                    break;

                case ItemKey.SHA256:
                    key = ReadString(Data.Models.Metadata.Rom.SHA256Key);
                    break;

                case ItemKey.SHA384:
                    key = ReadString(Data.Models.Metadata.Rom.SHA384Key);
                    break;

                case ItemKey.SHA512:
                    key = ReadString(Data.Models.Metadata.Rom.SHA512Key);
                    break;

                case ItemKey.SpamSum:
                    key = ReadString(Data.Models.Metadata.Rom.SpamSumKey);
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
