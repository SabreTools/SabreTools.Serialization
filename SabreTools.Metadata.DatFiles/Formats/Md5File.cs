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
        public override ItemType[] SupportedTypes
            => [
                ItemType.Disk,
                ItemType.Media,
                ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Md5File(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.MD5;
            Header.Write(DatHeader.DatFormatKey, DatFormat.RedumpMD5);
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Data.Models.Metadata.Rom.NameKey);

            switch (datItem)
            {
                case Disk disk:
                    if (string.IsNullOrEmpty(disk.ReadString(Data.Models.Metadata.Disk.MD5Key)))
                        missingFields.Add(Data.Models.Metadata.Disk.MD5Key);
                    break;

                case Media medium:
                    if (string.IsNullOrEmpty(medium.ReadString(Data.Models.Metadata.Media.MD5Key)))
                        missingFields.Add(Data.Models.Metadata.Media.MD5Key);
                    break;

                case Rom rom:
                    if (string.IsNullOrEmpty(rom.ReadString(Data.Models.Metadata.Rom.MD5Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.MD5Key);
                    break;

                default:
                    // Item type is not supported
                    break;
            }

            return missingFields;
        }
    }
}
