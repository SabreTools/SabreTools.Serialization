using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
    [JsonObject("rom"), XmlRoot("rom")]
    public sealed class Rom : DatItem<Data.Models.Metadata.Rom>
    {
        #region Constants

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string DataAreaKey = "DATAAREA";

        /// <summary>
        /// Non-standard key for inverted logic
        /// </summary>
        public const string PartKey = "PART";

        #endregion

        #region Fields

        /// <inheritdoc>/>
        protected override ItemType ItemType => ItemType.Rom;

        [JsonIgnore]
        public bool ItemStatusSpecified
        {
            get
            {
                var status = ReadString(Data.Models.Metadata.Rom.StatusKey).AsItemStatus();
                return status != ItemStatus.NULL && status != ItemStatus.None;
            }
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

        [JsonIgnore]
        public bool DataAreaSpecified
        {
            get
            {
                var dataArea = Read<DataArea?>(DataAreaKey);
                return dataArea is not null
                    && (!string.IsNullOrEmpty(dataArea.GetName())
                        || dataArea.ReadLong(Data.Models.Metadata.DataArea.SizeKey) is not null
                        || dataArea.ReadLong(Data.Models.Metadata.DataArea.WidthKey) is not null
                        || dataArea.ReadString(Data.Models.Metadata.DataArea.EndiannessKey).AsEndianness() != Endianness.NULL);
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

        public Rom() : base()
        {
            Write<DupeType>(DupeTypeKey, 0x00);
            Write<string?>(Data.Models.Metadata.Rom.StatusKey, ItemStatus.None.AsStringValue());
        }

        public Rom(Data.Models.Metadata.Dump item, Machine machine, Source source, int index)
        {
            // If we don't have rom data, we can't do anything
            Data.Models.Metadata.Rom? rom = null;
            OpenMSXSubType subType = OpenMSXSubType.NULL;
            if (item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.RomKey) is not null)
            {
                rom = item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.RomKey);
                subType = OpenMSXSubType.Rom;
            }
            else if (item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.MegaRomKey) is not null)
            {
                rom = item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.MegaRomKey);
                subType = OpenMSXSubType.MegaRom;
            }
            else if (item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.SCCPlusCartKey) is not null)
            {
                rom = item.Read<Data.Models.Metadata.Rom>(Data.Models.Metadata.Dump.SCCPlusCartKey);
                subType = OpenMSXSubType.SCCPlusCart;
            }

            // Just return if nothing valid was found
            if (rom is null)
                return;

            string name = $"{machine.GetName()}_{index++}{(!string.IsNullOrEmpty(rom!.ReadString(Data.Models.Metadata.Rom.RemarkKey)) ? $" {rom.ReadString(Data.Models.Metadata.Rom.RemarkKey)}" : string.Empty)}";

            SetName(name);
            Write<string?>(Data.Models.Metadata.Rom.OffsetKey, rom.ReadString(Data.Models.Metadata.Rom.StartKey));
            Write<string?>(Data.Models.Metadata.Rom.OpenMSXMediaType, subType.AsStringValue());
            Write<string?>(Data.Models.Metadata.Rom.OpenMSXType, rom.ReadString(Data.Models.Metadata.Rom.OpenMSXType) ?? rom.ReadString(Data.Models.Metadata.DatItem.TypeKey));
            Write<string?>(Data.Models.Metadata.Rom.RemarkKey, rom.ReadString(Data.Models.Metadata.Rom.RemarkKey));
            Write<string?>(Data.Models.Metadata.Rom.SHA1Key, rom.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Write<string?>(Data.Models.Metadata.Rom.StartKey, rom.ReadString(Data.Models.Metadata.Rom.StartKey));
            Write<Source?>(SourceKey, source);

            if (item.Read<Data.Models.Metadata.Original>(Data.Models.Metadata.Dump.OriginalKey) is not null)
            {
                var original = item.Read<Data.Models.Metadata.Original>(Data.Models.Metadata.Dump.OriginalKey)!;
                Write<Original?>("ORIGINAL", new Original
                {
                    Value = original.ReadBool(Data.Models.Metadata.Original.ValueKey),
                    Content = original.ReadString(Data.Models.Metadata.Original.ContentKey),
                });
            }

            CopyMachineInformation(machine);

            // Process hash values
            if (ReadLong(Data.Models.Metadata.Rom.SizeKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SizeKey, ReadLong(Data.Models.Metadata.Rom.SizeKey).ToString());
            // TODO: This should be normalized to CRC-16
            if (ReadString(Data.Models.Metadata.Rom.CRC16Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC16Key, NormalizeHashData(ReadString(Data.Models.Metadata.Rom.CRC16Key), 4));
            if (ReadString(Data.Models.Metadata.Rom.CRCKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(ReadString(Data.Models.Metadata.Rom.CRCKey)));
            // TODO: This should be normalized to CRC-64
            if (ReadString(Data.Models.Metadata.Rom.CRC64Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC64Key, NormalizeHashData(ReadString(Data.Models.Metadata.Rom.CRC64Key), 16));
            if (ReadString(Data.Models.Metadata.Rom.MD2Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD2Key, TextHelper.NormalizeMD2(ReadString(Data.Models.Metadata.Rom.MD2Key)));
            if (ReadString(Data.Models.Metadata.Rom.MD4Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD4Key, TextHelper.NormalizeMD5(ReadString(Data.Models.Metadata.Rom.MD4Key)));
            if (ReadString(Data.Models.Metadata.Rom.MD5Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(ReadString(Data.Models.Metadata.Rom.MD5Key)));
            if (ReadString(Data.Models.Metadata.Rom.RIPEMD128Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD128Key, TextHelper.NormalizeRIPEMD128(ReadString(Data.Models.Metadata.Rom.RIPEMD128Key)));
            if (ReadString(Data.Models.Metadata.Rom.RIPEMD160Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD160Key, TextHelper.NormalizeRIPEMD160(ReadString(Data.Models.Metadata.Rom.RIPEMD160Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA1Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(ReadString(Data.Models.Metadata.Rom.SHA1Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA256Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(ReadString(Data.Models.Metadata.Rom.SHA256Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA384Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(ReadString(Data.Models.Metadata.Rom.SHA384Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA512Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(ReadString(Data.Models.Metadata.Rom.SHA512Key)));
        }

        public Rom(Data.Models.Metadata.Rom item) : base(item)
        {
            Write<DupeType>(DupeTypeKey, 0x00);

            // Process flag values
            if (ReadBool(Data.Models.Metadata.Rom.DisposeKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.DisposeKey, ReadBool(Data.Models.Metadata.Rom.DisposeKey).FromYesNo());
            if (ReadBool(Data.Models.Metadata.Rom.InvertedKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.InvertedKey, ReadBool(Data.Models.Metadata.Rom.InvertedKey).FromYesNo());
            if (ReadString(Data.Models.Metadata.Rom.LoadFlagKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.LoadFlagKey, ReadString(Data.Models.Metadata.Rom.LoadFlagKey).AsLoadFlag().AsStringValue());
            if (ReadString(Data.Models.Metadata.Rom.OpenMSXMediaType) is not null)
                Write<string?>(Data.Models.Metadata.Rom.OpenMSXMediaType, ReadString(Data.Models.Metadata.Rom.OpenMSXMediaType).AsOpenMSXSubType().AsStringValue());
            if (ReadBool(Data.Models.Metadata.Rom.MIAKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.MIAKey, ReadBool(Data.Models.Metadata.Rom.MIAKey).FromYesNo());
            if (ReadBool(Data.Models.Metadata.Rom.OptionalKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.OptionalKey, ReadBool(Data.Models.Metadata.Rom.OptionalKey).FromYesNo());
            if (ReadBool(Data.Models.Metadata.Rom.SoundOnlyKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SoundOnlyKey, ReadBool(Data.Models.Metadata.Rom.SoundOnlyKey).FromYesNo());
            if (ReadString(Data.Models.Metadata.Rom.StatusKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.StatusKey, ReadString(Data.Models.Metadata.Rom.StatusKey).AsItemStatus().AsStringValue());

            // Process hash values
            if (ReadLong(Data.Models.Metadata.Rom.SizeKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SizeKey, ReadLong(Data.Models.Metadata.Rom.SizeKey).ToString());
            // TODO: This should be normalized to CRC-16
            if (ReadString(Data.Models.Metadata.Rom.CRC16Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC16Key, NormalizeHashData(ReadString(Data.Models.Metadata.Rom.CRC16Key), 4));
            if (ReadString(Data.Models.Metadata.Rom.CRCKey) is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRCKey, TextHelper.NormalizeCRC32(ReadString(Data.Models.Metadata.Rom.CRCKey)));
            // TODO: This should be normalized to CRC-64
            if (ReadString(Data.Models.Metadata.Rom.CRC64Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.CRC64Key, NormalizeHashData(ReadString(Data.Models.Metadata.Rom.CRC64Key), 16));
            if (ReadString(Data.Models.Metadata.Rom.MD2Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD2Key, TextHelper.NormalizeMD2(ReadString(Data.Models.Metadata.Rom.MD2Key)));
            if (ReadString(Data.Models.Metadata.Rom.MD4Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD4Key, TextHelper.NormalizeMD4(ReadString(Data.Models.Metadata.Rom.MD4Key)));
            if (ReadString(Data.Models.Metadata.Rom.MD5Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.MD5Key, TextHelper.NormalizeMD5(ReadString(Data.Models.Metadata.Rom.MD5Key)));
            if (ReadString(Data.Models.Metadata.Rom.RIPEMD128Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD128Key, TextHelper.NormalizeRIPEMD128(ReadString(Data.Models.Metadata.Rom.RIPEMD128Key)));
            if (ReadString(Data.Models.Metadata.Rom.RIPEMD160Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.RIPEMD160Key, TextHelper.NormalizeRIPEMD160(ReadString(Data.Models.Metadata.Rom.RIPEMD160Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA1Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA1Key, TextHelper.NormalizeSHA1(ReadString(Data.Models.Metadata.Rom.SHA1Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA256Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA256Key, TextHelper.NormalizeSHA256(ReadString(Data.Models.Metadata.Rom.SHA256Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA384Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA384Key, TextHelper.NormalizeSHA384(ReadString(Data.Models.Metadata.Rom.SHA384Key)));
            if (ReadString(Data.Models.Metadata.Rom.SHA512Key) is not null)
                Write<string?>(Data.Models.Metadata.Rom.SHA512Key, TextHelper.NormalizeSHA512(ReadString(Data.Models.Metadata.Rom.SHA512Key)));
        }

        public Rom(Data.Models.Metadata.Rom item, Machine machine, Source source) : this(item)
        {
            Write<Source?>(SourceKey, source);
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
