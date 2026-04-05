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

            switch (datItem)
            {
                case Disk disk:
                    if (string.IsNullOrEmpty(disk.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Disk.Name));
                    if (string.IsNullOrEmpty(disk.SHA1))
                        missingFields.Add(nameof(Data.Models.Metadata.Disk.SHA1));
                    break;

                case Media medium:
                    if (string.IsNullOrEmpty(medium.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Media.Name));
                    if (string.IsNullOrEmpty(medium.SHA1))
                        missingFields.Add(nameof(Data.Models.Metadata.Media.SHA1));
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.Name));
                    if (string.IsNullOrEmpty(rom.SHA1))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.SHA1));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
