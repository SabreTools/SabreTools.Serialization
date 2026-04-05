using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an MD5 hashfile
    /// </summary>
    public sealed class Md5File : Hashfile
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
        public Md5File(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.MD5;
            Header.DatFormat = DatFormat.RedumpMD5;
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
                    if (string.IsNullOrEmpty(disk.MD5))
                        missingFields.Add(nameof(Data.Models.Metadata.Disk.MD5));
                    break;

                case Media medium:
                    if (string.IsNullOrEmpty(medium.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Media.Name));
                    if (string.IsNullOrEmpty(medium.MD5))
                        missingFields.Add(nameof(Data.Models.Metadata.Media.MD5));
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.Name))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.Name));
                    if (string.IsNullOrEmpty(rom.MD5))
                        missingFields.Add(nameof(Data.Models.Metadata.Rom.MD5));
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
