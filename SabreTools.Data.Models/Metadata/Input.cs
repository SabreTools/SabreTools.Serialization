using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("input"), XmlRoot("input")]
    public class Input : DatItem, ICloneable, IEquatable<Input>
    {
        #region Properties

        public long? Buttons { get; set; }

        public long? Coins { get; set; }

        public Control[]? Control { get; set; }

        /// <remarks>Attribute also named Control</remarks>
        public string? ControlAttr { get; set; }

        public long? Players { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Service { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Tilt { get; set; }

        #endregion

        public Input() => ItemType = ItemType.Input;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Input();

            obj.Buttons = Buttons;
            obj.Coins = Coins;
            if (Control is not null)
                obj.Control = Array.ConvertAll(Control, i => (Control)i.Clone());
            obj.ControlAttr = ControlAttr;
            obj.Players = Players;
            obj.Service = Service;
            obj.Tilt = Tilt;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Input? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Buttons != other.Buttons)
                return false;

            if (Coins != other.Coins)
                return false;

            if ((ControlAttr is null) ^ (other.ControlAttr is null))
                return false;
            else if (ControlAttr is not null && !ControlAttr.Equals(other.ControlAttr, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Players != other.Players)
                return false;

            if (Service != other.Service)
                return false;

            if (Tilt != other.Tilt)
                return false;

            // Sub-items
            // TODO: Figure out how to properly check arrays

            return true;
        }
    }
}
