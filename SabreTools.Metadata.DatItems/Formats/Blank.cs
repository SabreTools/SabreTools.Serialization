using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a blank set from an input DAT
    /// </summary>
    [JsonObject("blank"), XmlRoot("blank")]
    public sealed class Blank : DatItem
    {
        #region Fields

        /// <inheritdoc>/>
        public override Data.Models.Metadata.ItemType ItemType => Data.Models.Metadata.ItemType.Blank;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Blank object
        /// </summary>
        public Blank() { }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            var blank = new Blank();
            blank.Write(MachineKey, GetMachine());
            blank.Write(RemoveKey, ReadBool(RemoveKey));
            blank.Write<Source?>(SourceKey, Read<Source?>(SourceKey));

            return blank;
        }

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not DatItem otherItem)
                return false;

            // Compare internal models
            return Equals(otherItem);
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.DatItem>? other)
        {
            // If other is null
            if (other is null)
                return false;

            // If the type is mismatched
            if (other is not DatItem otherItem)
                return false;

            // Compare internal models
            return Equals(otherItem);
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If we don't have a blank, return false
            if (ItemType != other?.ItemType)
                return false;

            // Otherwise, treat it as a Blank
            Blank? newOther = other as Blank;

            // If the machine information matches
            return GetMachine() == newOther!.GetMachine();
        }

        #endregion
    }
}
