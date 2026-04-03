using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an SHA-1 hashfile
    /// </summary>
    public sealed class Sha1File : Hashfile
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.ItemType[] SupportedTypes
            => [
                Data.Models.Metadata.ItemType.Disk,
                Data.Models.Metadata.ItemType.Media,
                Data.Models.Metadata.ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Sha1File(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.SHA1;
            Header.DatFormat = DatFormat.RedumpSHA1;
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(nameof(Data.Models.Metadata.Rom.Name));

            switch (datItem)
            {
                case Disk disk:
                    if (string.IsNullOrEmpty(disk.ReadString(Data.Models.Metadata.Disk.SHA1Key)))
                        missingFields.Add(Data.Models.Metadata.Disk.SHA1Key);
                    break;

                case Media medium:
                    if (string.IsNullOrEmpty(medium.SHA1))
                        missingFields.Add(nameof(Data.Models.Metadata.Media.SHA1));
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.SHA1Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.SHA1Key);
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
