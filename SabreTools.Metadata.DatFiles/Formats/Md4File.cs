using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;

#pragma warning disable IDE0290 // Use primary constructor
namespace SabreTools.Metadata.DatFiles.Formats
{
    /// <summary>
    /// Represents an MD4 hashfile
    /// </summary>
    public sealed class Md4File : Hashfile
    {
        /// <inheritdoc/>
        public override ItemType[] SupportedTypes
            => [
                ItemType.Rom,
            ];

        /// <summary>
        /// Constructor designed for casting a base DatFile
        /// </summary>
        /// <param name="datFile">Parent DatFile to copy from</param>
        public Md4File(DatFile? datFile) : base(datFile)
        {
            _hash = HashType.MD4;
            Header.SetFieldValue(DatHeader.DatFormatKey, DatFormat.RedumpMD4);
        }

        /// <inheritdoc/>
        protected internal override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            List<string> missingFields = [];

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Data.Models.Metadata.Rom.NameKey);

#pragma warning disable IDE0010
            switch (datItem)
            {
                case Rom rom:
                    if (string.IsNullOrEmpty(rom.GetStringFieldValue(Data.Models.Metadata.Rom.MD4Key)))
                        missingFields.Add(Data.Models.Metadata.Rom.MD4Key);
                    break;
            }
#pragma warning restore IDE0010

            return missingFields;
        }
    }
}
