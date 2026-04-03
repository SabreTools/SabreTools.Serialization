using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("control"), XmlRoot("control")]
    public class Control : DatItem, ICloneable, IEquatable<Control>
    {
        #region Properties

        public long? Buttons { get; set; }

        /// <remarks>(joy|stick|paddle|pedal|lightgun|positional|dial|trackball|mouse|only_buttons|keypad|keyboard|mahjong|hanafuda|gambling)</remarks>
        public ControlType? ControlType { get; set; }

        public long? KeyDelta { get; set; }

        public long? Maximum { get; set; }

        public long? Minimum { get; set; }

        public long? Player { get; set; }

        public long? ReqButtons { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Reverse { get; set; }

        public long? Sensitivity { get; set; }

        public string? Ways { get; set; }

        public string? Ways2 { get; set; }

        public string? Ways3 { get; set; }

        #endregion

        public Control() => ItemType = ItemType.Control;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Control();

            obj.Buttons = Buttons;
            obj.ControlType = ControlType;
            obj.KeyDelta = KeyDelta;
            obj.Maximum = Maximum;
            obj.Minimum = Minimum;
            obj.Player = Player;
            obj.ReqButtons = ReqButtons;
            obj.Reverse = Reverse;
            obj.Sensitivity = Sensitivity;
            obj.Ways = Ways;
            obj.Ways2 = Ways2;
            obj.Ways3 = Ways3;

            return obj;
        }

        /// <inheritdoc/>
        public bool Equals(Control? other)
        {
            // Null never matches
            if (other is null)
                return false;

            // Properties
            if (Buttons != other.Buttons)
                return false;

            if (ControlType != other.ControlType)
                return false;

            if (KeyDelta != other.KeyDelta)
                return false;

            if (Maximum != other.Maximum)
                return false;

            if (Minimum != other.Minimum)
                return false;

            if (Player != other.Player)
                return false;

            if (ReqButtons != other.ReqButtons)
                return false;

            if (Reverse != other.Reverse)
                return false;

            if (Sensitivity != other.Sensitivity)
                return false;

            if ((Ways is null) ^ (other.Ways is null))
                return false;
            else if (Ways is not null && !Ways.Equals(other.Ways, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Ways2 is null) ^ (other.Ways2 is null))
                return false;
            else if (Ways2 is not null && !Ways2.Equals(other.Ways2, StringComparison.OrdinalIgnoreCase))
                return false;

            if ((Ways3 is null) ^ (other.Ways3 is null))
                return false;
            else if (Ways3 is not null && !Ways3.Equals(other.Ways3, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
